namespace fit.Operators
{
    using fitSharp.Fit.Engine;
    using fitSharp.Machine.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This strategy is used in the ArraySubFixture in order to only compare items that are in the list.
    /// Not the whole list of actual items must exist nor must they be ordered.
    /// </summary>
    public class ArrayUnorderedSubMatchStrategy : NamedMatchStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArraySubMatchStrategy"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="theHeaderRow">The header row.</param>
        public ArrayUnorderedSubMatchStrategy(CellProcessor processor, Tree<Cell> theHeaderRow) : base(processor, theHeaderRow) { }
        
        /// <summary>
        /// Gets a value indicating whether items in compared lists have to be ordered.
        /// </summary>
        /// <value>
        /// <c>true</c> if compared lists have to have the same order; otherwise, <c>false</c>.
        /// </value>
        public override bool IsOrdered {get { return false; }}
        
        /// <summary>
        /// Gets a value indicating whether additional elements in the actual list are marked as failure.
        /// </summary>
        /// <value>
        ///   <c>true</c> if additional elements in the actual list are allowed; otherwise, <c>false</c>.
        /// </value>
        public override bool SurplusAllowed {get {return true;}}
    }
}