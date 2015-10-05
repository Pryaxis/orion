﻿using Orion.Framework;
using OTA.Logging;
using OTA.Plugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orion
{
    /// <summary>
    /// The core center piece of Orion. All Orion modules will have a reference
    /// to this Orion core.
    /// </summary>
    public partial class Orion : IDisposable
    {
        private bool disposedValue = false; // To detect redundant calls
        private readonly OTAPIPlugin plugin;

        public const string kOrionBasePath = "Orion";

        /// <summary>
        /// Gets a reference to the OTAPI plugin container in which this Orion core
        /// runs inside of
        /// </summary>
        /// <remarks>
        /// Marked as internal, Orion modules should be using hooks from the IHookProvider
        /// interface Orion provides itself
        /// </remarks>
        internal OTAPIPlugin Plugin => plugin;

        /// <summary>
        /// Contains a cached list of all available Orion modules
        /// </summary>
        internal List<Type> availablePlugins;

        /// <summary>
        /// Contains references to all the Orion modules loaded inside Orion
        /// </summary>
        internal readonly List<OrionModuleBase> moduleContainer;

        /// <summary>
        /// Returns the base path for all Orion-based disk information
        /// </summary>
        public string OrionBasePath { get; set; } = kOrionBasePath;

        /// <summary>
        /// Gets the Orion module directory path 
        /// </summary>
        public string OrionModulePath => Path.Combine(OrionBasePath, "Modules");

        /// <summary>
        /// Gets the Orion databse directory path
        /// </summary>
        public string OrionDatabasePath => Path.Combine(OrionBasePath, "Database");

        /// <summary>
        /// Gets the Orion configuration directory path
        /// </summary>
        public string OrionConfigurationPath => Path.Combine(OrionBasePath, "Configuration");

        /// <summary>
        /// Internal synch root for Orion to lock on when access to a member must be thread-safe.
        /// </summary>
        protected readonly object syncRoot = new object();

        public Orion(OTAPIPlugin plugin)
        {
            this.moduleContainer = new List<OrionModuleBase>();
            this.plugin = plugin;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }


        public void Initialize()
        {
            CheckDirectories();
            LoadModules();
            RunModules();
        }

        /// <summary>
        /// Checks for, and creates Orion's directory structure if it doesn't exist
        /// </summary>
        protected void CheckDirectories()
        {
            if (Directory.Exists(OrionBasePath) == false)
            {
                Directory.CreateDirectory(OrionBasePath);

                Directory.CreateDirectory(OrionModulePath);
                Directory.CreateDirectory(OrionConfigurationPath);
                Directory.CreateDirectory(OrionDatabasePath);
            }
            else
            {
                if (Directory.Exists(OrionModulePath) == false)
                {
                    Directory.CreateDirectory(OrionModulePath);
                }

                if (Directory.Exists(OrionConfigurationPath) == false)
                {
                    Directory.CreateDirectory(OrionConfigurationPath);
                }

                if (Directory.Exists(OrionDatabasePath) == false)
                {
                    Directory.CreateDirectory(OrionDatabasePath);
                }
            }
        }

        /// <summary>
        /// Returns the first orion module matching the type parameter <typeparamref name="TModule"/>
        /// </summary>
        /// <typeparam name="TModule">
        /// TModule is the type of a module
        /// </typeparam>
        /// <returns>
        /// A <typeparamref name="TModule"/> instance if one was found in the module container
        /// matching the specified type, or null if none could be found.
        /// </returns>
        public TModule Get<TModule>()
            where TModule : OrionModuleBase
        {
            TModule module = null;

            lock (syncRoot)
            {
                module = moduleContainer.FirstOrDefault(i => i is TModule) as TModule;
            }

            return module;
        }

        #region Orion Module Loader

        public void RunModules()
        {
            List<OrionModuleBase> failedModules = new List<OrionModuleBase>();
            lock (syncRoot)
            {
                foreach (OrionModuleBase module in moduleContainer)
                {
                    try
                    {
                        module.Initialize();
                    }
                    catch
                    {
                        /*
                         * Module init exceptions should not interfere with other 
                         * module inits
                         */

                        failedModules.Add(module);
                        continue;
                    }
                }

                if (failedModules.Count > 0) {
                    ProgramLog.Error.Log($"orion modules:  These following modules failed to initialize and were disabled.");

                    foreach (OrionModuleBase failedModule in failedModules)
                    {
                        ProgramLog.Error.Log($" * {failedModule.ModuleName} by {failedModule.Author}");
                        failedModule.Dispose();
                    }

                    moduleContainer.RemoveAll(i => failedModules.Contains(i));
                }
            }
        }

        /// <summary>
        /// Loads all modules into the module container.
        /// </summary>
        public void LoadModules()
        {
            /*
             * All OrionModule types inside Orion.dll itself are considered internal.
             */
            IEnumerable<Type> internalModuleList = GetOrionModulesFromAssembly(typeof(Orion).Assembly);

            /*
             * Note:  Internal modules will load first before all other modules, completely
             * disregarding the global order.  So the internal module list is *unioned* with
             * the external module list so that internal ones always get instantiated first.
             */
            IEnumerable<Type> externalModuleList = ExternalModules();

            foreach (Type module in internalModuleList.Union(externalModuleList))
            {
                OrionModuleAttribute attr = Attribute.GetCustomAttribute(module, typeof(OrionModuleAttribute)) as OrionModuleAttribute;

                if (attr == null)
                {
                    continue;
                }

                try
                {
                    LoadModule(module);
                }
                catch
                {
                    ProgramLog.Log($"orion modules: Could not load module {attr.ModuleName} because of an error", ConsoleColor.Yellow);
                    continue;
                }
            }
        }

        /// <summary>
        /// Scans an entire Assembly definition for Orion modules that are not disabled,
        /// and returns the types of those modules in OrionModule-qualified order.
        /// </summary>
        /// <param name="asm">
        /// A reference to the assembly to scan for Orion modules in
        /// </param>
        /// <returns>
        /// An array of types of valid orion modules.
        /// </returns>
        public IEnumerable<Type> GetOrionModulesFromAssembly(Assembly asm)
        {
            return from i in asm.GetTypes()
                   let orionModuleAttr = Attribute.GetCustomAttribute(i, typeof(OrionModuleAttribute)) as OrionModuleAttribute
                   where orionModuleAttr != null
                       && orionModuleAttr.Enabled == true
                       && i.BaseType == typeof(OrionModuleBase)
                   orderby orionModuleAttr.Order
                   select i;
        }

        /// <summary>
        /// Enumerates all DLL assemblies in the specified directory, and returns a
        /// list of all enabled OrionModules in those assemblies.
        /// </summary>
        /// <param name="pluginsDirectory">
        /// (Optional) A reference to the plugins directory to search in.  Defaults to
        /// OTA's plugins directory.
        /// </param>
        public IEnumerable<Type> ExternalModules()
        {
            Assembly asm;
            List<Type> moduleList = new List<Type>();

            foreach (string pluginFile in Directory.EnumerateFiles(OrionModulePath, "*.dll"))
            {
                ProgramLog.Debug.Log($"orion modules: trying assembly {pluginFile}");
                try
                {
                    asm = Assembly.LoadFrom(pluginFile);
                }
                catch (Exception ex) when (ex is BadImageFormatException || ex is FileLoadException)
                {
                    //TODO: Print warning about assembly being ignored because it was invalid
                    continue;
                }

                /*
                 * Important:  Do not do orderby selectors here, as they are re-ordered later
                 * in the process.
                 */

                moduleList.AddRange(GetOrionModulesFromAssembly(asm));
            }

            return moduleList;
        }

        /// <summary>
        /// Loads a module type and inserts it into the module container.
        /// </summary>
        /// <param name="moduleType">
        /// A reference to the runtime type of the module to load
        /// </param>
        public void LoadModule(Type moduleType)
        {
            OrionModuleBase moduleInstance;

            if (moduleType.BaseType == null || moduleType.BaseType != typeof(OrionModuleBase))
            {
                throw new InvalidOperationException($"Module {moduleType.Name} does not inherit from OrionModuleBase");
            }

            moduleInstance = Activator.CreateInstance(moduleType, this) as OrionModuleBase;

            ProgramLog.Debug.Log($"orion modules: activating {moduleInstance.ModuleName} by {moduleInstance.Author}");

            lock (syncRoot)
            {
                moduleContainer.Add(moduleInstance);
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return null;
        }
        #endregion

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        ~Orion()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
