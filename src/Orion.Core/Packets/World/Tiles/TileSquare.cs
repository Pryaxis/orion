﻿// Copyright (c) 2020 Pryaxis & Orion Contributors
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Orion.Core.Packets.DataStructures;
using Orion.Core.World.Tiles;

namespace Orion.Core.Packets.World.Tiles
{
    /// <summary>
    /// A packet sent to set a square of tiles.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct TileSquare : IPacket
    {
        // The shifts for the tile header.
        private const int SlopeShift = 12;

        // The masks for the tile header.
        private const ushort IsBlockActiveMask   /* */ = 0b_00000000_00000001;
        private const ushort HasWallMask         /* */ = 0b_00000000_00000100;
        private const ushort HasLiquidMask       /* */ = 0b_00000000_00001000;
        private const ushort HasRedWireMask      /* */ = 0b_00000000_00010000;
        private const ushort IsBlockHalvedMask   /* */ = 0b_00000000_00100000;
        private const ushort HasActuatorMask     /* */ = 0b_00000000_01000000;
        private const ushort IsBlockActuatedMask /* */ = 0b_00000000_10000000;
        private const ushort HasBlueWireMask     /* */ = 0b_00000001_00000000;
        private const ushort HasGreenWireMask    /* */ = 0b_00000010_00000000;
        private const ushort HasBlockColorMask   /* */ = 0b_00000100_00000000;
        private const ushort HasWallColorMask    /* */ = 0b_00001000_00000000;
        private const ushort SlopeMask           /* */ = 0b_01110000_00000000;
        private const ushort HasYellowWireMask   /* */ = 0b_10000000_00000000;

        [FieldOffset(0)] private byte _changeType;  // Unused.
        [FieldOffset(1)] private byte _bytes;  // Used to obtain an interior reference.
        [FieldOffset(8)] private ITileSlice? _tiles;

        /// <summary>
        /// Gets or sets the top-left tile's X coordinate.
        /// </summary>
        /// <value>The top-left tile's X coordinate.</value>
        [field: FieldOffset(1)] public short X { get; set; }

        /// <summary>
        /// Gets or sets the top-left tile's Y coordinate.
        /// </summary>
        /// <value>The top-left tile's Y coordinate.</value>
        [field: FieldOffset(3)] public short Y { get; set; }

        /// <summary>
        /// Gets or sets the square of tiles.
        /// </summary>
        /// <value>The square of tiles.</value>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not a square.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        public ITileSlice Tiles
        {
            get => _tiles ??= NetworkTileSlice.Empty;

            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value.Width != value.Height)
                {
                    throw new ArgumentException("Value is not a square", nameof(value));
                }

                _tiles = value;
            }
        }

        PacketId IPacket.Id => PacketId.TileSquare;

        int IPacket.ReadBody(Span<byte> span, PacketContext context)
        {
            var length = 2;
            var size = Unsafe.ReadUnaligned<short>(ref span.At(0));
            if (size < 0)
            {
                _changeType = span[length++];
                size &= short.MaxValue;
            }

            length += span[length..].Read(ref _bytes, 4);

            _tiles = new NetworkTileSlice(size, size);
            for (var i = 0; i < size; ++i)
            {
                for (var j = 0; j < size; ++j)
                {
                    length += ReadTile(span[length..], ref _tiles[i, j]);
                }
            }

            return length;
        }

        int IPacket.WriteBody(Span<byte> span, PacketContext context)
        {
            var tiles = Tiles;

            var length = 2;
            var size = (short)tiles.Width;
            if (_changeType > 0)
            {
                span[length++] = _changeType;
                size |= ~short.MaxValue;
            }

            Unsafe.WriteUnaligned(ref span.At(0), size);
            length += span[length..].Write(ref _bytes, 4);

            for (var i = 0; i < tiles.Width; ++i)
            {
                for (var j = 0; j < tiles.Height; ++j)
                {
                    length += WriteTile(span[length..], ref tiles[i, j]);
                }
            }

            return length;
        }

        // TODO: look into optimizing this if bottleneck?
        private int ReadTile(Span<byte> span, ref Tile tile)
        {
            ref var header = ref Unsafe.As<byte, ushort>(ref span.At(0));
            var index = 2;

            tile.IsBlockActive        /* */ = (header & IsBlockActiveMask) != 0;
            tile.HasRedWire           /* */ = (header & HasRedWireMask) != 0;
            tile.HasActuator          /* */ = (header & HasActuatorMask) != 0;
            tile.IsBlockActuated      /* */ = (header & IsBlockActuatedMask) != 0;
            tile.HasBlueWire          /* */ = (header & HasBlueWireMask) != 0;
            tile.HasGreenWire         /* */ = (header & HasGreenWireMask) != 0;
            tile.HasYellowWire        /* */ = (header & HasYellowWireMask) != 0;

            if ((header & HasBlockColorMask) != 0)
            {
                tile.BlockColor = (PaintColor)span[index++];
            }

            if ((header & HasWallColorMask) != 0)
            {
                tile.WallColor = (PaintColor)span[index++];
            }

            if (tile.IsBlockActive)
            {
                tile.BlockId = Unsafe.ReadUnaligned<BlockId>(ref span[index]);
                index += 2;

                if (tile.BlockId.HasFrames())
                {
                    Unsafe.CopyBlockUnaligned(ref Unsafe.Add(ref tile.AsByte(), 5), ref span[index], 4);
                    index += 4;
                }

                if ((header & IsBlockHalvedMask) != 0)
                {
                    tile.BlockShape = BlockShape.Halved;
                }
                else
                {
                    var slope = header & SlopeMask;
                    if (slope > 0)
                    {
                        tile.BlockShape = (BlockShape)((slope >> SlopeShift) + 1);
                    }
                }
            }

            if ((header & HasWallMask) != 0)
            {
                tile.WallId = Unsafe.ReadUnaligned<WallId>(ref span[index]);
                index += 2;
            }

            if ((header & HasLiquidMask) != 0)
            {
                tile.Liquid = new Liquid(amount: span[index++], type: (LiquidType)span[index++]);
            }

            return index;
        }

        // TODO: look into optimizing this if bottleneck?
        private int WriteTile(Span<byte> span, ref Tile tile)
        {
            ref var header = ref Unsafe.As<byte, ushort>(ref span.At(0));
            header = 0;
            var index = 2;

            if (tile.IsBlockActive)   /* */ header |= IsBlockActiveMask;
            if (tile.HasRedWire)      /* */ header |= HasRedWireMask;
            if (tile.HasActuator)     /* */ header |= HasActuatorMask;
            if (tile.IsBlockActuated) /* */ header |= IsBlockActuatedMask;
            if (tile.HasBlueWire)     /* */ header |= HasBlueWireMask;
            if (tile.HasGreenWire)    /* */ header |= HasGreenWireMask;
            if (tile.HasYellowWire)   /* */ header |= HasYellowWireMask;

            if (tile.BlockColor != PaintColor.None)
            {
                span[index++] = (byte)tile.BlockColor;

                header |= HasBlockColorMask;
            }

            if (tile.WallColor != PaintColor.None)
            {
                span[index++] = (byte)tile.WallColor;

                header |= HasWallColorMask;
            }

            if (tile.IsBlockActive)
            {
                Unsafe.WriteUnaligned(ref span[index], tile.BlockId);
                index += 2;

                if (tile.BlockId.HasFrames())
                {
                    Unsafe.CopyBlockUnaligned(ref span[index], ref Unsafe.Add(ref tile.AsByte(), 5), 4);
                    index += 4;
                }

                if (tile.BlockShape == BlockShape.Halved)
                {
                    header |= IsBlockHalvedMask;
                }
                else
                {
                    if (tile.BlockShape != BlockShape.Normal)
                    {
                        header |= (ushort)((int)(tile.BlockShape - 1) << SlopeShift);
                    }
                }
            }

            if (tile.WallId != WallId.None)
            {
                Unsafe.WriteUnaligned(ref span[index], tile.WallId);
                index += 2;

                header |= HasWallMask;
            }

            if (!tile.Liquid.IsEmpty)
            {
                var liquid = tile.Liquid;
                span[index++] = liquid.Amount;
                span[index++] = (byte)liquid.Type;

                header |= HasLiquidMask;
            }

            return index;
        }
    }
}
