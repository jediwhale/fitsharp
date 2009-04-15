// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using fit.exception;
using fitlibrary.exception;
using fitSharp.Fit.Model;

namespace fit.Engine {
    public class TestStatus {
        public Counts Counts { get; private set; }
        public bool IsAbandoned { get; set; }
        public Hashtable Summary { get; private set; }
        
        public TestStatus() {
            Counts = new Counts();
            Summary = new Hashtable();
        }

		public void MarkRight(Cell cell) {
			cell.SetAttribute(CellAttributes.StatusKey, CellAttributes.PassStatus);
			Counts.Right++;
		}

		public void MarkWrong(Cell cell) {
			cell.SetAttribute(CellAttributes.StatusKey, CellAttributes.FailStatus);
			Counts.Wrong++;
		}

		public void MarkWrong(Cell cell, string actual) {
		    cell.SetAttribute(CellAttributes.ActualKey, actual);
		    MarkWrong(cell);
		}

		public void MarkIgnore(Cell cell) {
			cell.SetAttribute(CellAttributes.StatusKey, CellAttributes.IgnoreStatus);
			Counts.Ignores++;
		}

		public void MarkException(Cell cell, System.Exception exception) {
            if (exception is IgnoredException) return;

            if (ContainsAbandonStoryTestException(exception) && IsAbandoned) throw new AbandonStoryTestException();

            if (cell.GetAttribute(CellAttributes.StatusKey) != CellAttributes.ErrorStatus) {
                cell.SetAttribute(CellAttributes.ExceptionKey, exception.ToString());
                cell.SetAttribute(CellAttributes.StatusKey, CellAttributes.ErrorStatus);
                Counts.Exceptions++;
            }

		    if (ContainsAbandonStoryTestException(exception)) {
		        IsAbandoned = true;
		        throw new AbandonStoryTestException();
		    }
		}

        private static bool ContainsAbandonStoryTestException(System.Exception exception) {
		    for (System.Exception e = exception; e != null; e = e.InnerException) {
		        if (e is AbandonStoryTestException) return true;
		    }
            return false;
        }

        public void ColorCell(Cell cell, bool isRight) {
            if (isRight)
                MarkRight(cell);
            else
                MarkWrong(cell);
        }
    }
}