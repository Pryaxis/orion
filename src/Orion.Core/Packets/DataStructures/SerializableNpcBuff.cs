﻿using System;
using System.Runtime.InteropServices;
using Orion.Core.Utils;

namespace Orion.Core.Packets.DataStructures
{
    /// <summary>
    /// Represents a serializable NPC buff.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct SerializableNpcBuff
    {
        [FieldOffset(0)] private byte _bytes; // Used to obtain an interior reference

        /// <summary>
        /// Gets or sets the type of buff.
        /// </summary>
        [field: FieldOffset(0)] public ushort Type { get; set; }

        /// <summary>
        /// Gets or sets the buff time.
        /// </summary>
        [field: FieldOffset(2)] public short Time { get; set; }

        /// <summary>
        /// Writes the current buff to the specified span and returns the number of bytes written.
        /// </summary>
        /// <param name="span">The span to write to.</param>
        /// <returns>The number of bytes written.</returns>
        public int Write(Span<byte> span) => span.Write(ref _bytes, 4);

        /// <summary>
        /// Reads an <see cref="SerializableNpcBuff"/> from the specified span and returns the number of bytes read.
        /// </summary>
        /// <param name="span">The span to read from.</param>
        /// <param name="npcBuff">The resulting <see cref="SerializableNpcBuff"/></param>
        /// <returns>The read <see cref="SerializableNpcBuff"/>.</returns>
        public static int Read(Span<byte> span, out SerializableNpcBuff npcBuff)
        {
            npcBuff = default;
            return span.Read(ref npcBuff._bytes, 4);
        }
    }
}
