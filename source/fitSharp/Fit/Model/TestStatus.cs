// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using System.Collections.Generic;
using fitSharp.Fit.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {

    public interface AbandonException {}

    public class TestStatus {
        public bool IsAbandoned { get; set; }
        public string LastAction { get; set; }
        public Hashtable Summary { get; private set; }
        public TestCounts Counts { get; private set; }

        readonly Stack<TypedValue> returnValues = new Stack<TypedValue>();
        
        public TestStatus() {
            Summary = new Hashtable();
            Counts = new TestCounts();
        }

        public TypedValue PopReturn() { return returnValues.Pop(); }
        public void PushReturn(TypedValue value) { returnValues.Push(value); }

        public void SetReturn(TypedValue value) {
            if (returnValues.Count == 0) return;
            PopReturn();
            PushReturn(value);
        }

        public void MarkRight(Cell cell) {
            cell.SetAttribute(CellAttribute.Status, CellAttributes.RightStatus);
            AddCount(CellAttributes.RightStatus);
        }

        public void MarkWrong(Cell cell) {
            cell.SetAttribute(CellAttribute.Status, CellAttributes.WrongStatus);
            AddCount(CellAttributes.WrongStatus);
        }

        public void MarkWrong(Cell cell, string actual) {
            cell.SetAttribute(CellAttribute.Actual, actual);
            MarkWrong(cell);
        }

        public void MarkIgnore(Cell cell) {
            cell.SetAttribute(CellAttribute.Status, CellAttributes.IgnoreStatus);
            AddCount(CellAttributes.IgnoreStatus);
        }

        public void MarkException(Cell cell, System.Exception exception) {
            if (exception is IgnoredException) return;

            System.Exception abandonException = GetAbandonStoryTestException(exception);

            if (abandonException != null && IsAbandoned) throw abandonException;

            if (cell.GetAttribute(CellAttribute.Status) != CellAttributes.ExceptionStatus) {
                cell.SetAttribute(CellAttribute.Exception, exception.ToString());
                cell.SetAttribute(CellAttribute.Status, CellAttributes.ExceptionStatus);
                AddCount(CellAttributes.ExceptionStatus);
            }

            if (abandonException == null) return;

            IsAbandoned = true;
            throw abandonException;
        }

        void AddCount(string cellStatus) {
            Counts.AddCount(cellStatus);
        }

        static System.Exception GetAbandonStoryTestException(System.Exception exception) {
            for (System.Exception e = exception; e != null; e = e.InnerException) {
                if (typeof(AbandonException).IsAssignableFrom(e.GetType())) return e;
            }
            return null;
        }

        public void ColorCell(Cell cell, bool isRight) {
            if (isRight)
                MarkRight(cell);
            else
                MarkWrong(cell);
        }
    }
}