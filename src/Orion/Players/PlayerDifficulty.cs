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

using System.Diagnostics.CodeAnalysis;

namespace Orion.Players {
    /// <summary>
    /// Specifies a player difficulty.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum PlayerDifficulty : byte {
        /// <summary>
        /// Indicates softcore. This is the normal difficulty.
        /// </summary>
        Softcore = 0,

        /// <summary>
        /// Indicates mediumcore. All items are dropped on death.
        /// </summary>
        Mediumcore = 1,

        /// <summary>
        /// Indicates hardcore. Death is permanent.
        /// </summary>
        Hardcore = 2
    }
}
