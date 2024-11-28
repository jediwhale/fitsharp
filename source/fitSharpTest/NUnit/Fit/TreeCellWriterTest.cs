﻿// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Fit
{
    [TestFixture] public class TreeCellWriterTest {
        [Test] public void FoldedWithExtraTextIsFormatted() {
            var cell = new CellBase(string.Empty);
            cell.SetAttribute(CellAttribute.Body, string.Empty);
            cell.SetAttribute(CellAttribute.Folded, "more");
            ClassicAssert.AreEqual(FormatFolded("more"), TreeCellWriter.Body(cell));
        }

        [Test] public void FoldedWithoutExtraTextFormatsBody() {
            var cell = new CellBase(string.Empty);
            cell.SetAttribute(CellAttribute.Body, "stuff");
            cell.SetAttribute(CellAttribute.Folded, string.Empty);
            ClassicAssert.AreEqual(FormatFolded("stuff"), TreeCellWriter.Body(cell));
        }

        static string FormatFolded(string text) {
            return "<span><a href=\"javascript:void(0)\" onclick=\"this.parentNode.nextSibling.style.display=this.parentNode.nextSibling.style.display=='none'?'':'none'\">&#8659;</a></span><div style=\"display:none\"><div class=\"fit_extension\">" + text + "</div></div>";
        }
    }
}
