// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitnesse.fixtures;

namespace fit.Test.Acceptance {
    public class BowlingGameFixture : TableFixture
    {
        protected override void DoStaticTable(int rows)
        {
            for (int row = 0; row < rows; row++)
            {
                BowlingGame game = new BowlingGame();
                for (int column = 0; column < 21; column++)
                {
                    if (!Blank(row, column))
                    {
                        game.Roll(GetInt(row, column));
                    }
                }
                int expectedScore = GetInt(row, 21);
                if (game.GetScore() == expectedScore)
                {
                    Right(row, 21);
                }
                else
                {
                    Wrong(row, 21, game.GetScore().ToString());
                }
            }
        }
    }
}