// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;

namespace fitSharp.Machine.Extension {
    public delegate void Action<T>(T target);
    public delegate void Action<T1, T2>(T1 arg1, T2 arg2);
    public delegate TResult Func<T1, TResult>(T1 arg1);
    public delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);

    public static class EnumerableExtension {
        public static T Aggregate<T, U>(this IEnumerable<U> collection, Func<T, U, T> aggregator) where T: new() {
            var total = new T();
            foreach (U unit in collection) total = aggregator(total, unit);
            return total;
        }

        public static T Aggregate<T, U>(this IEnumerable<U> collection, Action<T, U> aggregator) where T: new() {
            return AggregateTo(collection, new T(), aggregator);
        }

        public static T AggregateTo<T, U>(this IEnumerable<U> collection, T total, Action<T, U> aggregator) {
            foreach (U unit in collection) aggregator(total, unit);
            return total;
        }

        public static T Aggregate<T>(this IEnumerable<T> collection, Func<T, T, T> aggregator) where T: new() {
            var total = new T();
            foreach (T unit in collection) total = aggregator(total, unit);
            return total;
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> actor) {
            foreach (T unit in collection) actor(unit);
        }

        public static IEnumerable<T> Alternate<T>(this IEnumerable<T> collection) {
            bool alternate = true;
            foreach (T unit in collection) {
                if (alternate) yield return unit;
                alternate = !alternate;
            }
        }

        public static IEnumerable<T> From<T>(this IEnumerable<T> collection, int start) {
            int count = 0;
            foreach (T unit in collection) {
                if (count >= start) yield return unit;
                count++;
            }
        }

        public static IEnumerable<T> First<T>(this IEnumerable<T> collection, int number) {
            int count = 0;
            foreach (T unit in collection) {
                if (count >= number) break;
                yield return unit;
                count++;
            }
        }
    }
}
