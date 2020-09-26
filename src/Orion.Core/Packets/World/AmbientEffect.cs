﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Orion.Core.Utils;

namespace Orion.Core.Packets.World
{
    /// <summary>
    /// A packet sent to display ambient effects.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct AmbientEffect : IPacket
    {
        [FieldOffset(0)] private byte _bytes; // Used to obtain an interior reference

        /// <summary>
        /// Gets or sets the type of effect.
        /// </summary>
        [field: FieldOffset(0)] public AmbientEffectType EffectType { get; set; }

        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        [field: FieldOffset(1)] public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate.
        /// </summary>
        [field: FieldOffset(5)] public int Y { get; set; }

        /// <summary>
        /// Gets or sets a value that denotes the height of the tree when 
        /// <see cref="EffectType"/> == <see cref="AmbientEffectType.TreeGrow"/> or the type of particle when
        /// <see cref="EffectType"/> == <see cref="AmbientEffectType.FairyParticles"/>.
        /// </summary>
        [field: FieldOffset(9)] public byte TreeHeightOrParticleType { get; set; }

        /// <summary>
        /// Gets or sets the tree gore.
        /// </summary>
        [field: FieldOffset(10)] public short TreeGore { get; set; }

        PacketId IPacket.Id => PacketId.AmbientEffect;

        int IPacket.ReadBody(Span<byte> span, PacketContext context) => span.Read(ref _bytes, 12);

        int IPacket.WriteBody(Span<byte> span, PacketContext context) => span.Write(ref _bytes, 12);
    }

    /// <summary>
    /// Specifies an ambient effect's type.
    /// </summary>
    public enum AmbientEffectType : byte
    {
        /// <summary>
        /// Indicates the tree growth effect.
        /// </summary>
        TreeGrow = 1,

        /// <summary>
        /// Indicates the fairy particle effect.
        /// </summary>
        FairyParticles = 2
    }
}
