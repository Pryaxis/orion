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

using Orion.Items;
using TGCTE = Terraria.GameContent.Tile_Entities;

namespace Orion.World.TileEntities {
    internal sealed class OrionItemFrame : OrionTileEntity<TGCTE.TEItemFrame>, IItemFrame {
        public ItemType ItemType {
            get => (ItemType)Wrapped.item.type;
            set => Wrapped.item.type = (int)value;
        }

        public int ItemStackSize {
            get => Wrapped.item.stack;
            set => Wrapped.item.stack = value;
        }

        public ItemPrefix ItemPrefix {
            get => (ItemPrefix)Wrapped.item.prefix;
            set => Wrapped.item.prefix = (byte)value;
        }

        public OrionItemFrame(TGCTE.TEItemFrame itemFrame) : base(itemFrame) { }
    }
}
