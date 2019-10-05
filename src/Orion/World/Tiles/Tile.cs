﻿// Copyright (c) 2019 Pryaxis & Orion Contributors
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

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Orion.World.Tiles.Extensions;

namespace Orion.World.Tiles {
    /// <summary>
    /// Represents an optimized Terraria tile. Tiles are represented as structures for optimal packing and GC overhead.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Tile {
        private const int SlopeShift = 12;
        private const int WallColorShift = 16;
        private const int LiquidTypeShift = 21;

        // Masks for the tile header. We *could* theoretically squeeze the header into 3 bytes instead of 4, but this
        // means that sizeof(Tile) is 11, which would result in a whole bunch of alignment issues. This is probably
        // not worth the ~8% savings in memory usage.
        private const int BlockColorMask /*       */ = 0b_00000000_00000000_00000000_00011111;
        private const int IsBlockActiveMask /*    */ = 0b_00000000_00000000_00000000_00100000;
        private const int IsBlockActuatedMask /*  */ = 0b_00000000_00000000_00000000_01000000;
        private const int HasRedWireMask /*       */ = 0b_00000000_00000000_00000000_10000000;
        private const int HasBlueWireMask /*      */ = 0b_00000000_00000000_00000001_00000000;
        private const int HasGreenWireMask /*     */ = 0b_00000000_00000000_00000010_00000000;
        private const int IsBlockHalvedMask /*    */ = 0b_00000000_00000000_00000100_00000000;
        private const int HasActuatorMask /*      */ = 0b_00000000_00000000_00001000_00000000;
        private const int SlopeMask /*            */ = 0b_00000000_00000000_01110000_00000000;
        private const int WallColorMask /*        */ = 0b_00000000_00011111_00000000_00000000;
        private const int IsLavaMask /*           */ = 0b_00000000_00100000_00000000_00000000;
        private const int IsHoneyMask /*          */ = 0b_00000000_01000000_00000000_00000000;
        private const int LiquidTypeMask /*       */ = 0b_00000000_01100000_00000000_00000000;
        private const int HasYellowWireMask /*    */ = 0b_00000000_10000000_00000000_00000000;
        private const int IsCheckingLiquidMask /* */ = 0b_00001000_00000000_00000000_00000000;
        private const int ShouldSkipLiquidMask /* */ = 0b_00010000_00000000_00000000_00000000;

        /// <summary>
        /// The tile's block type.
        /// </summary>
        [FieldOffset(0)] public BlockType BlockType;

        /// <summary>
        /// The tile's wall type.
        /// </summary>
        [FieldOffset(2)] public WallType WallType;

        /// <summary>
        /// The tile's liquid amount.
        /// </summary>
        [FieldOffset(3)] public byte LiquidAmount;

        /// <summary>
        /// The tile's block's X frame.
        /// </summary>
        [FieldOffset(8)] public short BlockFrameX;

        /// <summary>
        /// The tile's block's Y frame.
        /// </summary>
        [FieldOffset(10)] public short BlockFrameY;

        // internal and type punning here allows us to easily provide ITile compatibility while preserving integer bit
        // arithmetic.
        [FieldOffset(4)] internal short _sTileHeader;
        [FieldOffset(6)] internal byte _bTileHeader;
        [FieldOffset(7)] internal byte _bTileHeader2;

        [FieldOffset(4)] private int _tileHeader;

        /// <summary>
        /// Gets or sets the tile's block color.
        /// </summary>
        public PaintColor BlockColor {
            readonly get => (PaintColor)(_tileHeader & BlockColorMask);
            set {
                Debug.Assert((int)value >= 0 && (int)value <= 31, "value should be valid");

                _tileHeader = (_tileHeader & ~BlockColorMask) | (int)value;
            }
        }

        // For the following bool-valued setters, *(int*)&value is by far the fastest way of converting the bool into
        // either a 1 or 0. The resulting code is still very readable, so we might as well use it... These setters could
        // very easily end up in a tight loop.

        /// <summary>
        /// Gets or sets a value indicating whether the tile's block is active.
        /// </summary>
        public bool IsBlockActive {
            readonly get => (_tileHeader & IsBlockActiveMask) != 0;
            set => _tileHeader = (_tileHeader & ~IsBlockActiveMask) | (*(int*)&value * IsBlockActiveMask);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile's block is acutuated.
        /// </summary>
        public bool IsBlockActuated {
            readonly get => (_tileHeader & IsBlockActuatedMask) != 0;
            set => _tileHeader = (_tileHeader & ~IsBlockActuatedMask) | (*(int*)&value * IsBlockActuatedMask);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile has red wire.
        /// </summary>
        public bool HasRedWire {
            readonly get => (_tileHeader & HasRedWireMask) != 0;
            set => _tileHeader = (_tileHeader & ~HasRedWireMask) | (*(int*)&value * HasRedWireMask);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile has blue wire.
        /// </summary>
        public bool HasBlueWire {
            readonly get => (_tileHeader & HasBlueWireMask) != 0;
            set => _tileHeader = (_tileHeader & ~HasBlueWireMask) | (*(int*)&value * HasBlueWireMask);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile has green wire.
        /// </summary>
        public bool HasGreenWire {
            readonly get => (_tileHeader & HasGreenWireMask) != 0;
            set => _tileHeader = (_tileHeader & ~HasGreenWireMask) | (*(int*)&value * HasGreenWireMask);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile's block is halved.
        /// </summary>
        public bool IsBlockHalved {
            readonly get => (_tileHeader & IsBlockHalvedMask) != 0;
            set => _tileHeader = (_tileHeader & ~IsBlockHalvedMask) | (*(int*)&value * IsBlockHalvedMask);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile has an actuator.
        /// </summary>
        public bool HasActuator {
            readonly get => (_tileHeader & HasActuatorMask) != 0;
            set => _tileHeader = (_tileHeader & ~HasActuatorMask) | (*(int*)&value * HasActuatorMask);
        }

        /// <summary>
        /// Gets or sets the tile's slope type.
        /// </summary>
        public Slope Slope {
            readonly get => (Slope)((_tileHeader & SlopeMask) >> SlopeShift);
            set {
                Debug.Assert((int)value >= 0 && (int)value <= 7, "value should be valid");

                _tileHeader = (_tileHeader & ~SlopeMask) | ((int)value << SlopeShift);
            }
        }

        /// <summary>
        /// Gets or sets the tile's wall color.
        /// </summary>
        public PaintColor WallColor {
            readonly get => (PaintColor)((_tileHeader & WallColorMask) >> WallColorShift);
            set {
                Debug.Assert((int)value >= 0 && (int)value <= 31, "value should be valid");

                _tileHeader = (_tileHeader & ~WallColorMask) | ((int)value << WallColorShift);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile's liquid is lava.
        /// </summary>
        public bool IsLava {
            readonly get => (_tileHeader & IsLavaMask) != 0;
            set => _tileHeader = (_tileHeader & ~IsLavaMask) | (*(int*)&value * IsLavaMask);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile's liquid is honey.
        /// </summary>
        public bool IsHoney {
            readonly get => (_tileHeader & IsHoneyMask) != 0;
            set => _tileHeader = (_tileHeader & ~IsHoneyMask) | (*(int*)&value * IsHoneyMask);
        }

        /// <summary>
        /// Gets or sets the tile's liquid type.
        /// </summary>
        public LiquidType LiquidType {
            readonly get => (LiquidType)((_tileHeader & LiquidTypeMask) >> LiquidTypeShift);
            set {
                Debug.Assert(value >= 0 && (int)value <= 3, "value should be valid");

                _tileHeader = (_tileHeader & ~LiquidTypeMask) | ((int)value << LiquidTypeShift);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile has yellow wire.
        /// </summary>
        public bool HasYellowWire {
            readonly get => (_tileHeader & HasYellowWireMask) != 0;
            set => _tileHeader = (_tileHeader & ~HasYellowWireMask) | (*(int*)&value * HasYellowWireMask);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile is checking liquid.
        /// </summary>
        public bool IsCheckingLiquid {
            readonly get => (_tileHeader & IsCheckingLiquidMask) != 0;
            set => _tileHeader = (_tileHeader & ~IsCheckingLiquidMask) | (*(int*)&value * IsCheckingLiquidMask);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile should skip liquids.
        /// </summary>
        public bool ShouldSkipLiquid {
            readonly get => (_tileHeader & ShouldSkipLiquidMask) != 0;
            set => _tileHeader = (_tileHeader & ~ShouldSkipLiquidMask) | (*(int*)&value * ShouldSkipLiquidMask);
        }

        /// <summary>
        /// Returns a value indicating whether the tile is the same as the given <paramref name="otherTile"/>.
        /// </summary>
        /// <param name="otherTile">The other tile.</param>
        /// <returns><see langword="true"/> if the tiles are the same; otherwise, <see langword="false"/>.</returns>
        [Pure]
        public readonly bool IsTheSameAs(in Tile otherTile) {
            // TODO: this method can be optimized due to structure layout
            if (_sTileHeader != otherTile._sTileHeader) {
                return false;
            }

            if (IsBlockActive) {
                if (BlockType != otherTile.BlockType) {
                    return false;
                }

                if (BlockType.AreFramesImportant() &&
                    (BlockFrameX != otherTile.BlockFrameX || BlockFrameY != otherTile.BlockFrameY)) {
                    return false;
                }
            }

            if (WallType != otherTile.WallType || LiquidAmount != otherTile.LiquidAmount) {
                return false;
            }

            if (LiquidAmount == 0) {
                if (WallColor != otherTile.WallColor || HasYellowWire != otherTile.HasYellowWire) {
                    return false;
                }
            } else if (_bTileHeader != otherTile._bTileHeader) {
                return false;
            }

            return true;
        }
    }
}
