// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;

namespace fit.Test.Acceptance {
    public class Coloring {

        private const string COLOR_ATTRIBUTE = " bgcolor=";
        private const string CLASS_ATTRIBUTE = " class=";

        private int myRed;
        private int myGreen;
        private int myBlue;

        public Coloring(int theRed, int theGreen, int theBlue) {
            myRed = theRed;
            myGreen = theGreen;
            myBlue = theBlue;
        }
        
        public string GetCode() {
            if (IsGreen())
                return "r";
            if (IsRed())
                return "w";
            if (IsGray())
                return "i";
            if (IsYellow())
                return "e";
            return "-";
        }

        public static Coloring Parsing(string theCellTag) {
            int index = theCellTag.IndexOf(COLOR_ATTRIBUTE);
            if (index >= 0) {
                try {
                    index += COLOR_ATTRIBUTE.Length;
                    if (theCellTag[index] == '"') index++;
                    if (theCellTag[index] == '#') index++;
                    string hex = theCellTag.Substring(index, 6);
                    int rgb = Convert.ToInt32(hex, 16);
                    return new Coloring(rgb>>16&255, rgb>>8&255, rgb&255);
                } catch (Exception) {
                    throw new Exception ("Can't parse bgcolor in: "+ theCellTag);
                }
            } 
            return null;
        }

        private bool IsRed()     {return myRed > myGreen && myRed > myBlue;}
        private bool IsGreen()   {return myGreen > myRed && myGreen > myBlue;}
        private bool IsYellow()  {return myRed > myBlue && myGreen > myBlue;}
        private bool IsGray()    {return myRed == myBlue && myGreen == myBlue;}

        public static string GetColor(Parse theCell) {
            try {
                if (theCell != null) {
                    string cellTag = theCell.Tag.ToLower();
                    if (HasTagAttribute(COLOR_ATTRIBUTE, cellTag)) return Parsing(cellTag).GetCode();
                    if (HasTagAttribute(CLASS_ATTRIBUTE, cellTag)) return GetClassCode(cellTag); 
                }

            } catch (Exception) {}
            return "-";
        }

        private static bool HasTagAttribute(string theAttribute, string theTag) {
            return (theTag.IndexOf(theAttribute) > 0);
        }

        private static string GetClassCode(string theCellTag) {
            int index = theCellTag.IndexOf(CLASS_ATTRIBUTE);
            if (index >= 0) {
                index += CLASS_ATTRIBUTE.Length;
                if (theCellTag[index] == '"') index++;
                string cssClass = theCellTag.Substring(index, 4);
                if (cssClass == "pass") return "r";
                if (cssClass == "fail") return "w";
                if (cssClass == "igno") return "i";
                if (cssClass == "erro") return "e";
            }
            return "-";
        }

    }
}