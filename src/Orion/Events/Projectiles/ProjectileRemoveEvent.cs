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
using Orion.Projectiles;

namespace Orion.Events.Projectiles {
    /// <summary>
    /// An event that occurs when a projectile is removed. This event can be canceled.
    /// </summary>
    [Event("proj-remove")]
    public sealed class ProjectileRemoveEvent : ProjectileEvent, ICancelable {
        /// <inheritdoc/>
        public string? CancellationReason { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectileRemoveEvent"/> class with the specified projectile.
        /// </summary>
        /// <param name="projectile">The projectile.</param>
        /// <exception cref="ArgumentNullException"><paramref name="projectile"/> is <see langword="null"/>.</exception>
        public ProjectileRemoveEvent(IProjectile projectile) : base(projectile) { }
    }
}
