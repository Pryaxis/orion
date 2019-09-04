﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Orion.Framework;

namespace Orion.World.Signs {
    /// <summary>
    /// Orion's implementation of <see cref="ISignService"/>.
    /// </summary>
    internal sealed class OrionSignService : OrionService, ISignService {
        private readonly IList<Terraria.Sign> _terrariaSigns;
        private readonly IList<OrionSign> _signs;

        public override string Author => "Pryaxis";
        public override string Name => "Orion Sign Service";

        public int Count => _signs.Count;

        public ISign this[int index] {
            get {
                if (index < 0 || index >= Count) {
                    throw new IndexOutOfRangeException(nameof(index));
                }

                if (_signs[index] == null || _signs[index].WrappedSign != _terrariaSigns[index]) {
                    if (_terrariaSigns[index] == null) {
                        return null;
                    } else {
                        _signs[index] = new OrionSign(_terrariaSigns[index]);
                    }
                }

                var sign = _signs[index];
                Debug.Assert(sign != null, $"{nameof(sign)} should not be null.");
                Debug.Assert(sign.WrappedSign != null, $"{nameof(sign.WrappedSign)} should not be null.");
                return sign;
            }
        }

        public OrionSignService() {
            _terrariaSigns = Terraria.Main.sign;
            _signs = new OrionSign[_terrariaSigns.Count];
        }

        public IEnumerator<ISign> GetEnumerator() {
            for (var i = 0; i < Count; ++i) {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
