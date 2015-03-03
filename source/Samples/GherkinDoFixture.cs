using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fitlibrary;

namespace fitSharp.Samples
{
    /// <summary>
    /// !|import|
    /// |fitSharp.Samples|
    /// 
    /// Must pass:
    /// !|Gherkin DoFixture              |
    /// |Given|Parameter     |one|and|two|
    /// |And  |Parameters are|one|and|two|
    /// |When |Parameters are reverted   |
    /// |Then |Parameters are|two|and|one|
    /// |And  |Parameters are|two|and|one|
    /// 
    /// Must fail:
    /// !|Gherkin DoFixture              |
    /// |Given|Parameter     |one|and|two|
    /// |When |Parameters are reverted   |
    /// |But  |Parameters are reverted   |
    /// |Then |Parameters are|two|and|one|
    /// 
    /// !|Gherkin DoFixture|
    /// |When|Always true|
    /// |And|always false|
    /// |Then|always true|
    /// |And|always false|
    /// </summary>
    public class GherkinDoFixture : DoFixture
    {
        private string par1;
        private string par2;

        public bool ParameterAnd(string par1, string par2)
        {
            this.par1 = par1;
            this.par2 = par2;
            return true;
        }

        public bool ParametersAreReverted()
        {
            var tmp = this.par1;
            this.par1 = this.par2;
            this.par2 = tmp;
            return true;
        }

        public bool ParametersAreAnd(string par1, string par2)
        {
            return (this.par1 == par1 && this.par2 == par2);
        }

        public bool AlwaysTrue = true;

        public bool AlwaysFalse = false;
    }
}
