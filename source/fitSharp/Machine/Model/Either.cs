// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitSharp.Machine.Model {
    public class Either<T, U> {

        public Either(T first) {
            this.first = first;
            isFirst = true;
        }

        public Either(U second) {
            this.second = second;
            isFirst = false;
        }

        public Either(bool isFirst, T first, U second) {
            this.isFirst = isFirst;
            if (isFirst) {
                this.first = first;
            }
            else {
                this.second = second;
            }
        }

        public R OneOf<R>(Func<T, R> withFirst, Func<U, R> withSecond) {
            return isFirst ? withFirst(first) : withSecond(second);
        }

        readonly bool isFirst;
        readonly T first;
        readonly U second;
    }
}
