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
using System.Collections.Generic;
using System.Diagnostics;
using Orion.Entities;

namespace Orion.Collections {
    // Wraps an array of type `TWrapped` to act as a read-only array of type `T`. This is extremely useful for wrapping
    // Terraria arrays as Orion interface arrays.
    internal sealed class WrappedReadOnlyArray<T, TWrapped> : IReadOnlyArray<T> where T : class, IWrapping<TWrapped> {
        private readonly ReadOnlyMemory<TWrapped> _wrappedItems;
        private readonly Func<int, TWrapped, T> _converter;
        private readonly T?[] _items;

        public int Count => _items.Length;

        public T this[int index] {
            get {
                var wrappedItem = _wrappedItems.Span[index];
                ref var item = ref _items[index];
#pragma warning disable CS0618 // Type or member is obsolete
                if (item is null || !ReferenceEquals(item.Wrapped, wrappedItem)) {
#pragma warning restore CS0618 // Type or member is obsolete
                    return item = _converter(index, wrappedItem);
                }

                return item;
            }
        }

        public WrappedReadOnlyArray(ReadOnlyMemory<TWrapped> wrappedItems, Func<int, TWrapped, T> converter) {
            Debug.Assert(converter != null);

            _wrappedItems = wrappedItems;
            _converter = converter;
            _items = new T?[wrappedItems.Length];
        }
    }
}
