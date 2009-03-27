// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

namespace fit.Test.Acceptance {
    public class Rectangle {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public void SetLocation(int theX, int theY) {
            X = theX;
            Y = theY;
        }
        public void SetSize(int theX, int theY) {
            Width = theX;
            Height = theY;
        }
        public Point Location {get {return new Point(X, Y);}}
    }

    public class Point {
        public int X;
        public int Y;
        public Point(int theX, int theY) {
            X = theX;
            Y = theY;
        }
        public void Move(int theX, int theY) {
            X = theX;
            Y = theY;
        }
    }
}