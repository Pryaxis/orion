﻿// Copyright (c) 2015-2019 Pryaxis & Orion Contributors
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
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Orion.Events.Extensions;
using Xunit;

namespace Orion.Projectiles {
    [Collection("TerrariaTestsCollection")]
    public class OrionProjectileServiceTests : IDisposable {
        private readonly IProjectileService _projectileService;

        public OrionProjectileServiceTests() {
            for (var i = 0; i < Terraria.Main.projectile.Length; ++i) {
                Terraria.Main.projectile[i] = new Terraria.Projectile {whoAmI = i};
            }

            _projectileService = new OrionProjectileService();
        }

        public void Dispose() {
            _projectileService.Dispose();
        }

        [Fact]
        public void GetItem_IsCorrect() {
            var projectile = _projectileService[0];

            ((OrionProjectile)projectile).Wrapped.Should().BeSameAs(Terraria.Main.projectile[0]);
        }

        [Fact]
        public void GetItem_MultipleTimes_ReturnsSameInstance() {
            var projectile = _projectileService[0];
            var projectile2 = _projectileService[0];

            projectile.Should().BeSameAs(projectile2);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(10000)]
        public void GetItem_InvalidIndex_ThrowsIndexOutOfRangeException(int index) {
            Func<IProjectile> func = () => _projectileService[index];

            func.Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void ProjectileSetDefaults_IsCorrect() {
            var isRun = false;
            _projectileService.ProjectileSetDefaults += (sender, args) => {
                isRun = true;
                ((OrionProjectile)args.Projectile).Wrapped.Should().BeSameAs(Terraria.Main.projectile[0]);
                args.ProjectileType.Should().Be(ProjectileType.CrystalBullet);
            };

            Terraria.Main.projectile[0].SetDefaults((int)ProjectileType.CrystalBullet);

            isRun.Should().BeTrue();
        }

        [Theory]
        [InlineData(ProjectileType.CrystalBullet, ProjectileType.WoodenArrow)]
        [InlineData(ProjectileType.CrystalBullet, ProjectileType.None)]
        public void ProjectileSetDefaults_ModifyProjectileType_IsCorrect(ProjectileType oldType, ProjectileType newType) {
            _projectileService.ProjectileSetDefaults += (sender, args) => {
                args.ProjectileType = newType;
            };

            Terraria.Main.projectile[0].SetDefaults((int)oldType);

            Terraria.Main.projectile[0].type.Should().Be((int)newType);
        }

        [Fact]
        public void ProjectileSetDefaults_Canceled_IsCorrect() {
            _projectileService.ProjectileSetDefaults += (sender, args) => {
                args.Cancel();
            };

            Terraria.Main.projectile[0].SetDefaults((int)ProjectileType.CrystalBullet);

            Terraria.Main.projectile[0].type.Should().Be(0);
        }

        [Fact]
        public void ProjectileUpdate_IsCorrect() {
            var isRun = false;
            _projectileService.ProjectileUpdate += (sender, args) => {
                isRun = true;
                ((OrionProjectile)args.Projectile).Wrapped.Should().BeSameAs(Terraria.Main.projectile[0]);
            };

            Terraria.Main.projectile[0].Update(0);
            
            isRun.Should().BeTrue();
        }

        [Fact]
        public void ProjectileRemove_IsCorrect() {
            var isRun = false;
            _projectileService.ProjectileRemove += (sender, args) => {
                isRun = true;
                ((OrionProjectile)args.Projectile).Wrapped.Should().BeSameAs(Terraria.Main.projectile[0]);
            };

            Terraria.Main.projectile[0].Kill();
            
            isRun.Should().BeTrue();
        }

        [Fact]
        public void ProjectileRemove_Canceled_IsCorrect() {
            _projectileService.ProjectileRemove += (sender, args) => {
                args.Cancel();
            };
            Terraria.Main.projectile[0].SetDefaults((int)ProjectileType.CrystalBullet);

            Terraria.Main.projectile[0].Kill();

            Terraria.Main.projectile[0].active.Should().BeTrue();
        }

        [Fact]
        public void GetEnumerator_IsCorrect() {
            var projectiles = _projectileService.ToList();

            for (var i = 0; i < projectiles.Count; ++i) {
                ((OrionProjectile)projectiles[i]).Wrapped.Should().BeSameAs(Terraria.Main.projectile[i]);
            }
        }

        [Fact]
        public void SpawnProjectile_IsCorrect() {
            var projectile =
                _projectileService.SpawnProjectile(ProjectileType.CrystalBullet, Vector2.Zero, Vector2.Zero, 100, 0);

            Debug.Assert(projectile != null);
            projectile.Type.Should().Be(ProjectileType.CrystalBullet);
        }

        [Fact]
        public void SpawnProjectile_AiValues_IsCorrect() {
            var projectile = _projectileService.SpawnProjectile(ProjectileType.CrystalBullet, Vector2.Zero,
                                                                Vector2.Zero, 100, 0, new float[] {1, 2});

            Debug.Assert(projectile != null);
            projectile.Type.Should().Be(ProjectileType.CrystalBullet);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void SpawnProjectile_AiValuesWrongLength_ThrowsArgumentException(int aiValuesLength) {
            Func<IProjectile?> func = () =>
                _projectileService.SpawnProjectile(ProjectileType.CrystalBullet, Vector2.Zero, Vector2.Zero, 100, 0,
                                                   new float[aiValuesLength]);

            func.Should().Throw<ArgumentException>();
        }
    }
}
