﻿// Copyright (c) 2015-2019 Pryaxis & Orion Contributors
// 
// This file is part of Orion.
// 
// Orion is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Orion is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Orion.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Orion.Hooks;
using Orion.World.Events;
using Orion.World.TileEntities;
using Orion.World.Tiles;
using OTAPI;
using OTAPI.Tile;
using Terraria;
using Terraria.IO;
using TDS = Terraria.DataStructures;
using TGCTE = Terraria.GameContent.Tile_Entities;
using Tile = Orion.World.Tiles.Tile;

namespace Orion.World {
    internal sealed unsafe class OrionWorldService : OrionService, IWorldService, ITileCollection {
        private static byte* _tilesPtr = null;

        private readonly IChestService _chestService;
        private readonly ISignService _signService;

        [ExcludeFromCodeCoverage] public override string Author => "Pryaxis";

        public string WorldName {
            get => Main.worldName;
            set => Main.worldName = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int WorldWidth => Main.maxTilesX;
        public int WorldHeight => Main.maxTilesY;

        public Tile this[int x, int y] {
            get => new OrionTile(_tilesPtr + OrionTile.ByteCount * ((WorldWidth + 1) * y + x));
            set => ((ITileCollection)this)[x, y] = value;
        }

        int ITileCollection.Width => WorldWidth;
        int ITileCollection.Height => WorldHeight;

        ITile ITileCollection.this[int x, int y] {
            get => this[x, y];
            set {
                var location = new OrionTile(_tilesPtr + OrionTile.ByteCount * ((WorldWidth + 1) * y + x));
                ((ITile)location).CopyFrom(value);
            }
        }

        public double Time {
            get => Main.time;
            set => Main.time = value;
        }

        public bool IsDaytime {
            get => Main.dayTime;
            set => Main.dayTime = value;
        }

        public bool IsHardmode {
            get => Main.hardMode;
            set => Main.hardMode = value;
        }

        public bool IsExpertMode {
            get => Main.expertMode;
            set => Main.expertMode = value;
        }

        public InvasionType CurrentInvasionType => (InvasionType)Main.invasionType;
        public HookHandlerCollection<CheckingHalloweenEventArgs> CheckingHalloween { get; set; }
        public HookHandlerCollection<CheckingChristmasEventArgs> CheckingChristmas { get; set; }
        public HookHandlerCollection<LoadingWorldEventArgs> LoadingWorld { get; set; }
        public HookHandlerCollection<LoadedWorldEventArgs> LoadedWorld { get; set; }
        public HookHandlerCollection<SavingWorldEventArgs> SavingWorld { get; set; }
        public HookHandlerCollection<SavedWorldEventArgs> SavedWorld { get; set; }
        public HookHandlerCollection<StartingHardmodeEventArgs> StartingHardmode { get; set; }
        public HookHandlerCollection<StartedHardmodeEventArgs> StartedHardmode { get; set; }
        public HookHandlerCollection<UpdatingHardmodeBlockEventArgs> UpdatingHardmodeBlock { get; set; }

        public OrionWorldService(IChestService chestService, ISignService signService) {
            Debug.Assert(chestService != null, $"{nameof(chestService)} should not be null.");
            Debug.Assert(signService != null, $"{nameof(signService)} should not be null.");

            _chestService = chestService;
            _signService = signService;

            OTAPI.Hooks.Tile.CreateCollection = () => {
                // Allocate with AllocHGlobal so that the memory is pre-pinned.
                if (_tilesPtr == null) {
                    _tilesPtr = (byte*)Marshal.AllocHGlobal(OrionTile.ByteCount * (WorldWidth + 1) * (WorldHeight + 1));
                }

                return this;
            };

            if (Main.tile != this) {
                Main.tile = OTAPI.Hooks.Tile.CreateCollection();
            }

            OTAPI.Hooks.Game.Halloween = HalloweenHandler;
            OTAPI.Hooks.Game.Christmas = ChristmasHandler;
            OTAPI.Hooks.World.IO.PreLoadWorld = PreLoadWorldHandler;
            OTAPI.Hooks.World.IO.PostLoadWorld = PostLoadWorldHandler;
            OTAPI.Hooks.World.IO.PreSaveWorld = PreSaveWorldHandler;
            OTAPI.Hooks.World.IO.PostSaveWorld = PostSaveWorldHandler;
            OTAPI.Hooks.World.PreHardmode = PreHardmodeHandler;
            OTAPI.Hooks.World.PostHardmode = PostHardmodeHandler;
            OTAPI.Hooks.World.HardmodeTileUpdate = HardmodeTileUpdateHandler;
        }

        protected override void Dispose(bool disposeManaged) {
            if (!disposeManaged) return;

            OTAPI.Hooks.Tile.CreateCollection = null;
            OTAPI.Hooks.Game.Halloween = null;
            OTAPI.Hooks.Game.Christmas = null;
            OTAPI.Hooks.World.IO.PreLoadWorld = null;
            OTAPI.Hooks.World.IO.PostLoadWorld = null;
            OTAPI.Hooks.World.IO.PreSaveWorld = null;
            OTAPI.Hooks.World.IO.PostSaveWorld = null;
            OTAPI.Hooks.World.PreHardmode = null;
            OTAPI.Hooks.World.PostHardmode = null;
            OTAPI.Hooks.World.HardmodeTileUpdate = null;
        }

        public bool StartInvasion(InvasionType invasionType) {
            Main.StartInvasion((int)invasionType);
            return CurrentInvasionType == invasionType;
        }

        public ITargetDummy AddTargetDummy(int x, int y) {
            if (TDS.TileEntity.ByPosition.ContainsKey(new TDS.Point16(x, y))) {
                return null;
            }

            var targetDummyIndex = TGCTE.TETrainingDummy.Place(x, y);
            return new OrionTargetDummy((TGCTE.TETrainingDummy)TDS.TileEntity.ByID[targetDummyIndex]);
        }

        public IItemFrame AddItemFrame(int x, int y) {
            if (TDS.TileEntity.ByPosition.ContainsKey(new TDS.Point16(x, y))) {
                return null;
            }

            var itemFrameIndex = TGCTE.TEItemFrame.Place(x, y);
            return new OrionItemFrame((TGCTE.TEItemFrame)TDS.TileEntity.ByID[itemFrameIndex]);
        }

        public ILogicSensor AddLogicSensor(int x, int y) {
            if (TDS.TileEntity.ByPosition.ContainsKey(new TDS.Point16(x, y))) {
                return null;
            }

            var logicSensor = TGCTE.TELogicSensor.Place(x, y);
            return new OrionLogicSensor((TGCTE.TELogicSensor)TDS.TileEntity.ByID[logicSensor]);
        }

        public ITileEntity GetTileEntity(int x, int y) {
            ITileEntity GetTerrariaTileEntity() {
                if (!TDS.TileEntity.ByPosition.TryGetValue(new TDS.Point16(x, y), out var terrariaTileEntity)) {
                    return null;
                }

                switch (terrariaTileEntity) {
                case TGCTE.TETrainingDummy trainingDummy: return new OrionTargetDummy(trainingDummy);
                case TGCTE.TEItemFrame itemFrame: return new OrionItemFrame(itemFrame);
                case TGCTE.TELogicSensor logicSensor: return new OrionLogicSensor(logicSensor);
                default: return null;
                }
            }

            return _chestService.GetChest(x, y) ?? _signService.GetSign(x, y) ?? GetTerrariaTileEntity();
        }

        public bool RemoveTileEntity(ITileEntity tileEntity) {
            bool RemoveTerrariaTileEntity() {
                // We use the & operator here instead of && since we always need to execute both operands.
                return TDS.TileEntity.ByPosition.Remove(new TDS.Point16(tileEntity.X, tileEntity.Y)) &
                       TDS.TileEntity.ByID.Remove(tileEntity.Index);
            }

            switch (tileEntity) {
            case IChest chest: return _chestService.RemoveChest(chest);
            case ISign sign: return _signService.RemoveSign(sign);
            default: return RemoveTerrariaTileEntity();
            }
        }

        public void SaveWorld() => WorldFile.saveWorld();


        private HookResult HalloweenHandler() {
            var args = new CheckingHalloweenEventArgs();
            CheckingHalloween?.Invoke(this, args);
            return args.Handled ? HookResult.Cancel : HookResult.Continue;
        }

        private HookResult ChristmasHandler() {
            var args = new CheckingChristmasEventArgs();
            CheckingChristmas?.Invoke(this, args);
            return args.Handled ? HookResult.Cancel : HookResult.Continue;
        }

        private HookResult PreLoadWorldHandler(ref bool loadFromCloud) {
            var args = new LoadingWorldEventArgs();
            LoadingWorld?.Invoke(this, args);
            return args.Handled ? HookResult.Cancel : HookResult.Continue;
        }

        private void PostLoadWorldHandler(bool loadFromCloud) {
            var args = new LoadedWorldEventArgs();
            LoadedWorld?.Invoke(this, args);
        }

        private HookResult PreSaveWorldHandler(ref bool useCloudSaving, ref bool resetTime) {
            var args = new SavingWorldEventArgs {ShouldResetTime = resetTime};
            SavingWorld?.Invoke(this, args);
            return args.Handled ? HookResult.Cancel : HookResult.Continue;
        }

        private void PostSaveWorldHandler(bool useCloudSaving, bool resetTime) {
            var args = new SavedWorldEventArgs {ShouldResetTime = resetTime};
            SavedWorld?.Invoke(this, args);
        }

        private HookResult PreHardmodeHandler() {
            var args = new StartingHardmodeEventArgs();
            StartingHardmode?.Invoke(this, args);
            return args.Handled ? HookResult.Cancel : HookResult.Continue;
        }

        private void PostHardmodeHandler() {
            var args = new StartedHardmodeEventArgs();
            StartedHardmode?.Invoke(this, args);
        }

        private HardmodeTileUpdateResult HardmodeTileUpdateHandler(int x, int y, ref ushort type) {
            var args = new UpdatingHardmodeBlockEventArgs(x, y, (BlockType)type);
            UpdatingHardmodeBlock?.Invoke(this, args);

            type = (ushort)args.BlockType;
            return args.Handled ? HardmodeTileUpdateResult.Cancel : HardmodeTileUpdateResult.Continue;
        }
    }
}
