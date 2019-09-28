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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Orion.Events;
using Orion.Events.Extensions;
using Orion.Events.Items;
using OTAPI;

namespace Orion.Items {
    internal sealed class OrionItemService : OrionService, IItemService {
        private readonly IList<Terraria.Item> _terrariaItems;
        private readonly IList<OrionItem> _items;

        [ExcludeFromCodeCoverage] public override string Author => "Pryaxis";

        // Subtract 1 from the count. This is because Terraria has an extra slot.
        public int Count => _items.Count - 1;

        public IItem this[int index] {
            get {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException();

                if (_items[index]?.Wrapped != _terrariaItems[index]) {
                    _items[index] = new OrionItem(_terrariaItems[index]);
                }

                Debug.Assert(_items[index] != null, "_items[index] != null");
                return _items[index];
            }
        }

        public EventHandlerCollection<ItemSetDefaultsEventArgs>? ItemSetDefaults { get; set; }
        public EventHandlerCollection<ItemUpdateEventArgs>? ItemUpdate { get; set; }

        public OrionItemService() {
            Debug.Assert(Terraria.Main.item != null, "Terraria.Main.item != null");
            Debug.Assert(Terraria.Main.item.All(i => i != null), "Terraria.Main.item.All(i => i != null)");

            _terrariaItems = Terraria.Main.item;
            _items = new OrionItem[_terrariaItems.Count];

            Hooks.Item.PreSetDefaultsById = PreSetDefaultsByIdHandler;
            Hooks.Item.PreUpdate = PreUpdateHandler;
        }

        protected override void Dispose(bool disposeManaged) {
            if (!disposeManaged) return;

            Hooks.Item.PreSetDefaultsById = null;
            Hooks.Item.PreUpdate = null;
        }

        public IEnumerator<IItem> GetEnumerator() {
            for (var i = 0; i < Count; ++i) {
                yield return this[i];
            }
        }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public IItem? SpawnItem(ItemType itemType, Vector2 position, int stackSize = 1,
                                ItemPrefix prefix = ItemPrefix.None) {
            // Terraria has a mechanism of item caching which allows, for instance, The Plan to drop all wires at once.
            // We need to disable that temporarily so that our item *definitely* spawns.
            var oldItemCache = Terraria.Item.itemCaches[(int)itemType];
            Terraria.Item.itemCaches[(int)itemType] = -1;

            var itemIndex =
                Terraria.Item.NewItem(position, Vector2.Zero, (int)itemType, stackSize, prefixGiven: (int)prefix);
            Terraria.Item.itemCaches[(int)itemType] = oldItemCache;
            return itemIndex >= 0 && itemIndex < Count ? this[itemIndex] : null;
        }

        private HookResult PreSetDefaultsByIdHandler(Terraria.Item terrariaItem, ref int itemType,
                                                     ref bool noMaterialCheck) {
            Debug.Assert(terrariaItem != null, "terrariaItem != null");

            var item = new OrionItem(terrariaItem);
            var args = new ItemSetDefaultsEventArgs(item, (ItemType)itemType);
            ItemSetDefaults?.Invoke(this, args);
            if (args.IsCanceled()) return HookResult.Cancel;

            itemType = (int)args.ItemType;
            return HookResult.Continue;
        }

        private HookResult PreUpdateHandler(Terraria.Item terrariaItem, ref int itemIndex) {
            Debug.Assert(terrariaItem != null, "terrariaItem != null");

            var item = new OrionItem(terrariaItem);
            var args = new ItemUpdateEventArgs(item);
            ItemUpdate?.Invoke(this, args);
            return args.IsCanceled() ? HookResult.Cancel : HookResult.Continue;
        }
    }
}
