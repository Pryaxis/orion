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

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Orion.Packets.World {
    /// <summary>
    /// Packet sent from the server to the client to set Old One's Army information.
    /// </summary>
    public sealed class OldOnesArmyInfoPacket : Packet {
        private TimeSpan _timeBetweenWaves;

        /// <inheritdoc />
        public override PacketType Type => PacketType.OldOnesArmyInfo;

        /// <summary>
        /// Gets or sets the time between waves.
        /// </summary>
        public TimeSpan TimeBetweenWaves {
            get => _timeBetweenWaves;
            set {
                _timeBetweenWaves = value;
                _isDirty = true;
            }
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => $"{Type}[T={TimeBetweenWaves}]";

        private protected override void ReadFromReader(BinaryReader reader, PacketContext context) {
            _timeBetweenWaves = TimeSpan.FromSeconds(reader.ReadInt32() / 60.0);
        }

        private protected override void WriteToWriter(BinaryWriter writer, PacketContext context) {
            writer.Write((int)(_timeBetweenWaves.TotalSeconds * 60.0));
        }
    }
}
