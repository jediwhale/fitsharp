// Copyright © 2022 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;

namespace fitSharp.Machine.Model {
    public class Break<TKey, TValue> where TKey: IComparable<TKey>  {

        public Break(Action<TKey> start, Action<TKey, TValue> item, Action<TKey> end) {
            this.start = start;
            this.item = item;
            this.end = end;
        }

        public void Process(IEnumerable<Tuple<TKey, TValue>> input) {
            Maybe<TKey> lastKey = Maybe<TKey>.Nothing;
            foreach (var (key, value) in input) {
                if (KeyChange(lastKey, key)) {
                    lastKey.IfPresent(k => end(k));
                    start(key);
                }
                lastKey = Maybe<TKey>.Of(key);
                item(key, value);
            }
            lastKey.IfPresent(k => end(k));
        }

        static bool KeyChange(Maybe<TKey> last, TKey current) {
            return last.Select(k => k.CompareTo(current) != 0).OrElse(true);
        }

        readonly Action<TKey> start;
        readonly Action<TKey, TValue> item;
        readonly Action<TKey> end;
    }
}