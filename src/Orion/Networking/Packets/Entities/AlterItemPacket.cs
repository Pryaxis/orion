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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Xna.Framework;
using Orion.Entities;

namespace Orion.Networking.Packets.Entities {
    /// <summary>
    /// Packet sent from the server to the client to alter an item.
    /// </summary>

    // TODO: write tests for this.
    public sealed class AlterItemPacket : Packet {
        private short _itemIndex;
        private Color? _itemColorOverride;
        private ushort? _itemDamageOverride;
        private float? _itemKnockbackOverride;
        private ushort? _itemAnimationTimeOverride;
        private ushort? _itemUseTimeOverride;
        private ProjectileType _itemProjectileTypeOverride;
        private float? _itemProjectileSpeedOverride;
        private short? _itemWidthOverride;
        private short? _itemHeightOverride;
        private float? _itemScaleOverride;
        private ItemType? _itemAmmoTypeOverride;
        private ItemType? _itemUsesAmmoTypeOverride;
        private bool? _itemIsNotAmmoOverride;

        /// <inheritdoc />
        public override PacketType Type => PacketType.AlterItem;

        /// <summary>
        /// Gets or sets the item index.
        /// </summary>
        public short ItemIndex {
            get => _itemIndex;
            set {
                _itemIndex = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the item's color. A value of <c>null</c> indicates no override.
        /// </summary>
        public Color? ItemColorOverride {
            get => _itemColorOverride;
            set {
                _itemColorOverride = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the item's damage. A value of <c>null</c> indicates no override.
        /// </summary>
        public ushort? ItemDamageOverride {
            get => _itemDamageOverride;
            set {
                _itemDamageOverride = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the item's knockback. A value of <c>null</c> indicates no override.
        /// </summary>
        public float? ItemKnockbackOverride {
            get => _itemKnockbackOverride;
            set {
                _itemKnockbackOverride = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the item's animation time. A value of <c>null</c> indicates no override.
        /// </summary>
        public ushort? ItemAnimationTimeOverride {
            get => _itemAnimationTimeOverride;
            set {
                _itemAnimationTimeOverride = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the item's use time. A value of <c>null</c> indicates no override.
        /// </summary>
        public ushort? ItemUseTimeOverride {
            get => _itemUseTimeOverride;
            set {
                _itemUseTimeOverride = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the item's projectile type. A value of <c>null</c> indicates no override.
        /// </summary>
        public ProjectileType ItemProjectileTypeOverride {
            get => _itemProjectileTypeOverride;
            set {
                _itemProjectileTypeOverride = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the item's projectile speed. A value of <c>null</c> indicates no override.
        /// </summary>
        public float? ItemProjectileSpeedOverride {
            get => _itemProjectileSpeedOverride;
            set {
                _itemProjectileSpeedOverride = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the item's width. A value of <c>null</c> indicates no override.
        /// </summary>
        public short? ItemWidthOverride {
            get => _itemWidthOverride;
            set {
                _itemWidthOverride = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the item's height. A value of <c>null</c> indicates no override.
        /// </summary>
        public short? ItemHeightOverride {
            get => _itemHeightOverride;
            set {
                _itemHeightOverride = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the item's scale. A value of <c>null</c> indicates no override.
        /// </summary>
        public float? ItemScaleOverride {
            get => _itemScaleOverride;
            set {
                _itemScaleOverride = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the item's ammo type. A value of <c>null</c> indicates no override.
        /// </summary>
        public ItemType? ItemAmmoTypeOverride {
            get => _itemAmmoTypeOverride;
            set {
                _itemAmmoTypeOverride = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the ammo type that the item uses. A value of <c>null</c> indicates no override.
        /// </summary>
        public ItemType? ItemUsesAmmoTypeOverride {
            get => _itemUsesAmmoTypeOverride;
            set {
                _itemUsesAmmoTypeOverride = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the override for the value indicating whether the item is not ammo. A value of <c>null</c>
        /// indicates no override.
        /// </summary>
        public bool? ItemIsNotAmmoOverride {
            get => _itemIsNotAmmoOverride;
            set {
                _itemIsNotAmmoOverride = value;
                _isDirty = true;
            }
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string ToString() => $"{Type}[#={ItemIndex}, ...]";

        private protected override void ReadFromReader(BinaryReader reader, PacketContext context) {
            ItemIndex = reader.ReadInt16();

            Terraria.BitsByte flags = reader.ReadByte();
            if (flags[0]) ItemColorOverride = new Color(reader.ReadUInt32());
            if (flags[1]) ItemDamageOverride = reader.ReadUInt16();
            if (flags[2]) ItemKnockbackOverride = reader.ReadSingle();
            if (flags[3]) ItemAnimationTimeOverride = reader.ReadUInt16();
            if (flags[4]) ItemUseTimeOverride = reader.ReadUInt16();

            if (flags[5]) {
                ItemProjectileTypeOverride = ProjectileType.FromId(reader.ReadInt16()) ??
                                             throw new PacketException("Projectile type is invalid.");
            }

            if (flags[6]) ItemProjectileSpeedOverride = reader.ReadSingle();
            if (!flags[7]) return;

            Terraria.BitsByte flags2 = reader.ReadByte();
            if (flags2[0]) ItemWidthOverride = reader.ReadInt16();
            if (flags2[1]) ItemHeightOverride = reader.ReadInt16();
            if (flags2[2]) ItemScaleOverride = reader.ReadSingle();
            if (flags2[3]) ItemAmmoTypeOverride = (ItemType)reader.ReadInt16();
            if (flags2[4]) ItemUsesAmmoTypeOverride = (ItemType)reader.ReadInt16();

            if (flags2[5]) ItemIsNotAmmoOverride = reader.ReadBoolean();
        }

        private protected override void WriteToWriter(BinaryWriter writer, PacketContext context) {
            writer.Write(ItemIndex);

            Terraria.BitsByte flags2 = 0;
            flags2[0] = ItemWidthOverride != null;
            flags2[1] = ItemHeightOverride != null;
            flags2[2] = ItemScaleOverride != null;
            flags2[3] = ItemAmmoTypeOverride != null;
            flags2[4] = ItemUsesAmmoTypeOverride != null;
            flags2[5] = ItemIsNotAmmoOverride != null;

            Terraria.BitsByte flags = 0;
            flags[0] = ItemColorOverride != null;
            flags[1] = ItemDamageOverride != null;
            flags[2] = ItemKnockbackOverride != null;
            flags[3] = ItemAnimationTimeOverride != null;
            flags[4] = ItemUseTimeOverride != null;
            flags[5] = ItemProjectileTypeOverride != null;
            flags[6] = ItemProjectileSpeedOverride != null;
            flags[7] = flags2 != 0;

            writer.Write(flags);
            if (flags[0]) writer.Write(ItemColorOverride.Value.PackedValue);
            if (flags[1]) writer.Write(ItemDamageOverride.Value);
            if (flags[2]) writer.Write(ItemKnockbackOverride.Value);
            if (flags[3]) writer.Write(ItemAnimationTimeOverride.Value);
            if (flags[4]) writer.Write(ItemUseTimeOverride.Value);
            if (flags[5]) writer.Write(ItemProjectileTypeOverride.Id);
            if (flags[6]) writer.Write(ItemProjectileSpeedOverride.Value);
            if (flags[7]) writer.Write(flags2);

            if (flags2[0]) writer.Write(ItemWidthOverride.Value);
            if (flags2[1]) writer.Write(ItemHeightOverride.Value);
            if (flags2[2]) writer.Write(ItemScaleOverride.Value);
            if (flags2[3]) writer.Write((short)ItemAmmoTypeOverride);
            if (flags2[4]) writer.Write((short)ItemUsesAmmoTypeOverride);
            if (flags2[5]) writer.Write(ItemIsNotAmmoOverride.Value);
        }
    }
}
