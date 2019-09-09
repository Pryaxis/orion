﻿using System.IO;
using FluentAssertions;
using Orion.Networking.Packets;
using Xunit;

namespace Orion.Tests.Networking.Packets.World {
    public class EnterWorldPacketTests {
        public static readonly byte[] EnterWorldBytes = {3, 0, 49};

        [Fact]
        public void ReadFromStream_IsCorrect() {
            using (var stream = new MemoryStream(EnterWorldBytes)) {
                Packet.ReadFromStream(stream);
            }
        }

        [Fact]
        public void WriteToStream_IsCorrect() {
            using (var stream = new MemoryStream(EnterWorldBytes))
            using (var stream2 = new MemoryStream()) {
                var packet = Packet.ReadFromStream(stream);

                packet.WriteToStream(stream2);

                stream2.ToArray().Should().BeEquivalentTo(EnterWorldBytes);
            }
        }
    }
}
