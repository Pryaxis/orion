﻿using System;
using NUnit.Framework;
using Orion.Events.Player;

namespace Orion.Tests.Events.Player
{
	[TestFixture]
	public class PlayedJoiningEventArgsTests
	{
		[Test]
		public void Constructor_NullPlayer_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new PlayerJoiningEventArgs(null));
		}
	}
}
