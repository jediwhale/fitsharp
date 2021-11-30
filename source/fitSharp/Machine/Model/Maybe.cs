// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;

namespace fitSharp.Machine.Model {
    public class Maybe<T> {
        public static Maybe<T> Nothing => new Maybe<T>();
        public static Maybe<T> Of(T aValue) => new Maybe<T>(aValue);

        public Maybe(T aValue) {
            this.aValue = aValue;
            IsPresent = true;
        }

        Maybe() {
            IsPresent = false;
        }

        public bool IsPresent { get; }

        public void IfPresent(Action<T> action) {
            if (IsPresent) action(aValue);
        }

        public IEnumerable<T> Value {
            get {
                if (IsPresent) yield return aValue;
            }
        }

        public Maybe<T> OrMaybe(Func<Maybe<T>> otherValue) {
            return IsPresent ? this : otherValue();
        }

        public Maybe<U> Select<U>(Func<T, U> withValue) {
            return IsPresent ? Maybe<U>.Of(withValue(aValue)) : Maybe<U>.Nothing;
        }

        public T OrElse(T defaultValue) {
            return IsPresent ? aValue : defaultValue;
        }

        public T OrElseThrow(Func<System.Exception> exception) {
            if (!IsPresent) throw exception();
            return aValue;
        }

        public T OrElseGet(Func<T> defaultAction) {
            return IsPresent ? aValue : defaultAction();
        }

        public TypedValue TypedValue =>
            IsPresent
                ? new TypedValue(aValue, typeof(T))
                : new TypedValue(null, typeof(T));

        readonly T aValue;
    }
}
