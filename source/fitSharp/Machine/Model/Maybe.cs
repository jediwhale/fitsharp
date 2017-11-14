// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;

namespace fitSharp.Machine.Model {
    public class Maybe<T> {
        public static Maybe<T> Nothing { get { return new Maybe<T>();} }

        public Maybe(T aValue) {
            this.aValue = aValue;
            hasValue = true;
        }

        Maybe() {
            hasValue = false;
        }

        public bool HasValue { get { return hasValue; } }

        public void Apply(Action<T> action) {
            if (hasValue) action(aValue);
        }

        public IEnumerable<T> Value {
            get {
                if (hasValue) yield return aValue;
            }
        }

        public Maybe<T> OrMaybe(Func<Maybe<T>> otherValue) {
            return hasValue ? this : otherValue();
        }

        public Maybe<U> Select<U>(Func<T, U> withValue) {
            return hasValue ? new Maybe<U>(withValue(aValue)) : Maybe<U>.Nothing;
        }

        public T OrDefault(T defaultValue) {
            return hasValue ? aValue : defaultValue;
        }

        public TypedValue TypedValue {
            get {
                return hasValue
                    ? new TypedValue(aValue, typeof(T))
                    : new TypedValue(null, typeof(T));
            }
        }

        readonly T aValue;
        readonly bool hasValue;
    }
}
