// Copyright © 2012 Syterra Software Inc. All rights reserved.
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

        public void ForValue(Action<T> action) {
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

        public U ForValue<U>(Func<T, U> withValue, Func<U> withoutValue) {
            return hasValue ? withValue(aValue) : withoutValue();
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
