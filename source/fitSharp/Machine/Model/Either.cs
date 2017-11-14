// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitSharp.Machine.Model {
    public class Either<T, U> {

        public Either(T left) {
            this.left = left;
            isLeft = true;
        }

        public Either(U right) {
            this.right = right;
            isLeft = false;
        }

        public Either(bool isLeft, T left, U right) {
            this.isLeft = isLeft;
            if (isLeft) {
                this.left = left;
            }
            else {
                this.right = right;
            }
        }

        public R Select<R>(Func<T, R> withLeft, Func<U, R> withRight) {
            return isLeft ? withLeft(left) : withRight(right);
        }

        readonly bool isLeft;
        readonly T left;
        readonly U right;
    }
}
