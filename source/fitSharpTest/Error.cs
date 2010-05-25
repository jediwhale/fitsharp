using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace fitSharp.Test {
    public class Error {
        /// <summary>
        /// Asserts that the given Action throws an error that is assignable to <typeparam name="T" />
        /// and returns the thrown exception.
        /// </summary>
        public static T Expect<T>(Action f) where T : Exception {
            try {
                f();
                Assert.Fail("Exception of type " + typeof(T).Name + " was expected, but no exception was thrown.");
            } catch (AssertionException) {
                throw;
            } catch (Exception ex) {
                if (!typeof(T).IsAssignableFrom(ex.GetType()))
                    Assert.Fail("Exception of type " + typeof(T).Name + " was expected, but a " + ex.GetType().Name + " was thrown");
                return (T)ex;
            }
            return null;
        }
    }
}
