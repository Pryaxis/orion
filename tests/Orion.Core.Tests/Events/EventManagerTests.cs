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
using Serilog.Core;
using Xunit;

namespace Orion.Core.Events
{
    public class EventManagerTests
    {
        [Fact]
        public void RegisterHandler_NullHandler_ThrowsArgumentNullException()
        {
            var manager = new EventManager();

            Assert.Throws<ArgumentNullException>(() => manager.RegisterHandler<TestEvent>(null!, Logger.None));
        }
        
        [Fact]
        public void RegisterHandler_NullLog_ThrowsArgumentNullException()
        {
            var manager = new EventManager();

            Assert.Throws<ArgumentNullException>(() => manager.RegisterHandler<TestEvent>(evt => { }, null!));
        }

        [Fact]
        public void RegisterHandlers_NullObj_ThrowsArgumentNullException()
        {
            var manager = new EventManager();

            Assert.Throws<ArgumentNullException>(() => manager.RegisterHandlers(null!, Logger.None));
        }

        [Fact]
        public void RegisterHandlers_NullLog_ThrowsArgumentNullException()
        {
            var manager = new EventManager();

            Assert.Throws<ArgumentNullException>(() => manager.RegisterHandlers(new object(), null!));
        }

        [Fact]
        public void DeregisterHandler_NullHandler_ThrowsArgumentNullException()
        {
            var manager = new EventManager();

            Assert.Throws<ArgumentNullException>(() => manager.DeregisterHandler<TestEvent>(null!, Logger.None));
        }

        [Fact]
        public void DeregisterHandler_NullLog_ThrowsArgumentNullException()
        {
            var manager = new EventManager();

            Assert.Throws<ArgumentNullException>(() => manager.DeregisterHandler<TestEvent>(evt => { }, null!));
        }

        [Fact]
        public void DeregisterHandlers_NullObj_ThrowsArgumentNullException()
        {
            var manager = new EventManager();

            Assert.Throws<ArgumentNullException>(() => manager.DeregisterHandlers(null!, Logger.None));
        }

        [Fact]
        public void DeregisterHandlers_NullLog_ThrowsArgumentNullException()
        {
            var manager = new EventManager();

            Assert.Throws<ArgumentNullException>(() => manager.DeregisterHandlers(new object(), null!));
        }

        [Fact]
        public void Raise_NullEvt_ThrowsArgumentNullException()
        {
            var manager = new EventManager();

            Assert.Throws<ArgumentNullException>(() => manager.Raise<TestEvent>(null!, Logger.None));
        }

        [Fact]
        public void Raise_NullLog_ThrowsArgumentNullException()
        {
            var manager = new EventManager();

            Assert.Throws<ArgumentNullException>(() => manager.Raise(new TestEvent(), null!));
        }

        [Fact]
        public void RegisterHandler_Raise()
        {
            var manager = new EventManager();
            manager.RegisterHandler<TestEvent>(evt => evt.Value = 100, Logger.None);
            var evt = new TestEvent();

            manager.Raise(evt, Logger.None);

            Assert.Equal(100, evt.Value);
        }

        [Fact]
        public void RegisterHandlers_Raise()
        {
            var manager = new EventManager();
            manager.RegisterHandlers(new TestClass(), Logger.None);
            var evt = new TestEvent();

            manager.Raise(evt, Logger.None);

            Assert.Equal(100, evt.Value);
        }

        [Fact]
        public void RegisterHandlers_Private_Raise()
        {
            var manager = new EventManager();
            manager.RegisterHandlers(new TestClass_Private(), Logger.None);
            var evt = new TestEvent();

            manager.Raise(evt, Logger.None);

            Assert.Equal(100, evt.Value);
        }

        [Fact]
        public void DeregisterHandler_ReturnsTrue()
        {
            static void Handler(TestEvent evt) => evt.Value = 100;

            var manager = new EventManager();
            manager.RegisterHandler<TestEvent>(Handler, Logger.None);

            Assert.True(manager.DeregisterHandler<TestEvent>(Handler, Logger.None));

            var evt = new TestEvent();
            manager.Raise(evt, Logger.None);

            Assert.NotEqual(100, evt.Value);
        }

        [Fact]
        public void DeregisterHandler_ReturnsFalse()
        {
            var manager = new EventManager();

            Assert.False(manager.DeregisterHandler<TestEvent>(evt => { }, Logger.None));
        }

        [Fact]
        public void DeregisterHandlers_ReturnsTrue()
        {
            var manager = new EventManager();
            var testClass = new TestClass();
            manager.RegisterHandlers(testClass, Logger.None);

            Assert.True(manager.DeregisterHandlers(testClass, Logger.None));

            var evt = new TestEvent();
            manager.Raise(evt, Logger.None);

            Assert.NotEqual(100, evt.Value);
        }

        [Fact]
        public void DeregisterHandlers_ReturnsFalse()
        {
            var manager = new EventManager();

            Assert.False(manager.DeregisterHandlers(new TestClass(), Logger.None));
        }

        [Event("test")]
        private class TestEvent : Event
        {
            public int Value { get; set; }
        }

        private class TestClass
        {
            [EventHandler("test")]
            public void OnTest(TestEvent evt) => evt.Value = 100;

            [EventHandler("test")]
            public void OnTest_MissingParam() { }

            [EventHandler("test")]
            [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Testing")]
            public void OnTest_TooManyParams(TestEvent evt, int x) { }

            [EventHandler("test")]
            [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Testing")]
            public void OnTest_InvalidParamType(int x) { }

            [EventHandler("test")]
            [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Testing")]
            public int OnTest_NotVoid(TestEvent evt) => 0;
        }

        private class TestClass_Private
        {
            [EventHandler("test")]
            [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Implicit usage")]
            private void OnTest(TestEvent evt) => evt.Value = 100;
        }
    }
}
