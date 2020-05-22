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
using System.Diagnostics.CodeAnalysis;
using Moq;
using Orion.Packets.Players;
using Orion.Players;
using Xunit;

namespace Orion.Events.Players {
    [SuppressMessage("Style", "IDE0017:Simplify object initialization", Justification = "Testing")]
    public class PlayerTeamEventTests {
        [Fact]
        public void Ctor_NullPlayer_ThrowsArgumentNullException() {
            var packet = new PlayerTeamPacket();

            Assert.Throws<ArgumentNullException>(() => new PlayerTeamEvent(null!, ref packet));
        }

        [Fact]
        public void Team_Get() {
            var player = new Mock<IPlayer>().Object;
            var packet = new PlayerTeamPacket { Team = PlayerTeam.Red };
            var evt = new PlayerTeamEvent(player, ref packet);

            Assert.Equal(PlayerTeam.Red, evt.Team);
        }

        [Fact]
        public void Team_Set() {
            var player = new Mock<IPlayer>().Object;
            var packet = new PlayerTeamPacket();
            var evt = new PlayerTeamEvent(player, ref packet);

            evt.Team = PlayerTeam.Red;

            Assert.Equal(PlayerTeam.Red, packet.Team);
        }
    }
}
