// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitlibrary.tree;

namespace fit.Test.Acceptance {
    public class SetFixtureUnderTestGraphics: SetFixture {

        public SetFixtureUnderTestGraphics(): base(new GraphicElement[]{
            new GraphicElement(1,new ListTree("a")),
            new GraphicElement(1,new ListTree(string.Empty, new ListTree[]{new ListTree("a")})),
            new GraphicElement(2,new ListTree(string.Empty, new ListTree[]{new ListTree("a"), new ListTree("BB")}))
        }) {}
    }
}