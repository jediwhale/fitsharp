namespace fit.Fixtures
{
    using fit.Operators;
    using fitlibrary;
    using fitSharp.Fit.Engine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This fixture compares the items of a list or array to items in the wiki.
    /// It works like the ArrayFixture but won't check order and surplus items will be ignored.
    /// </summary>
    public class ArrayUnorderedSubFixture : NamedCollectionFixtureBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArraySubFixture"/> class.
        /// </summary>
        public ArrayUnorderedSubFixture(): base() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ArraySubFixture"/> class.
        /// </summary>
        /// <param name="theArray">The array.</param>
        public ArrayUnorderedSubFixture(IEnumerable<object> theArray): base(theArray) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ArraySubFixture"/> class.
        /// </summary>
        /// <param name="theCollection">The collection.</param>
        public ArrayUnorderedSubFixture(IEnumerable theCollection): base(theCollection) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ArraySubFixture"/> class.
        /// </summary>
        /// <param name="theEnumerator">The enumerator.</param>
        public ArrayUnorderedSubFixture(IEnumerator theEnumerator): base(theEnumerator) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ArraySubFixture"/> class.
        /// </summary>
        /// <param name="theTable">The table.</param>
        public ArrayUnorderedSubFixture(DataTable theTable) : base(theTable.Rows.GetEnumerator()) { }

        /// <summary>
        /// Gets the match strategy.
        /// </summary>
        /// <value>
        /// The match strategy.
        /// </value>
        protected override ListMatchStrategy MatchStrategy
        {
            get 
            {
                return new ArrayUnorderedSubMatchStrategy(this.Processor, this.myHeaderRow);
            }
        }
    }
}