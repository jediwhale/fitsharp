// Copyright (c) 2003 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

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