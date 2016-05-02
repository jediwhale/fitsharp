// Copyright © 2016 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using fit.exception;
using fit.Model;
using fitlibrary;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Fixtures;

namespace fit.Fixtures {
    public class FlowKeywords {
        public FlowKeywords(FlowInterpreter fixture, CellProcessor processor) {
            this.fixture = fixture;
            this.processor = processor;
        }

        public void AbandonStoryTest(Parse theCells) {
            processor.TestStatus.MarkIgnore(theCells);
            processor.TestStatus.IsAbandoned = true;
            throw new AbandonStoryTestException();
        }

        public Fixture Calculate(Parse theCells) {
            return new CalculateFixture(fixture.SystemUnderTest ?? fixture);
        }

        public CommentFixture Comment(Parse cells) {
            return new CommentFixture();
        }

        public Fixture Ignored(Parse cells) {
            return new Fixture();
        }

        readonly FlowInterpreter fixture;
        readonly CellProcessor processor;
    }
}
