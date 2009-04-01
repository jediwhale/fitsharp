using System;
using System.Collections.Generic;
using fitSharp.Machine.Model;

namespace fitnesse.slim.test {

    public class ShouldIBuyMilk {
        public int CashInWallet;
        public int PintsOfMilkRemaining { get; set; }
        private bool creditCard;

        public void setCreditCard(string valid) {
            creditCard = "yes".Equals(valid);
        }

        public string goToStore() {
            return (PintsOfMilkRemaining == 0 && (CashInWallet > 2 || creditCard)) ? "yes" : "no";
        }
    }

    public class EmployeesHiredBefore {
        private DateTime date;

        public EmployeesHiredBefore(DateTime date) {
            this.date = date;
        }

        public List<object> query() {
            return new List<object> {
                new List<object> {
                    new List<object> {"employee number", "1429"},
                    new List<object> {"first name", "Bob"},
                    new List<object> {"last name", "Martin"},
                    new List<object> {"hire date", "10-Oct-1974"}
                },
                new List<object> {
                    new List<object> {"employee number", "8832"},
                    new List<object> {"first name", "James"},
                    new List<object> {"last name", "Grenning"},
                    new List<object> {"hire date", "15-Dec-1979"}
                }
            };
        }
    }

    public class LoginDialogDriver : DomainAdapter {
        private readonly LoginDialog loginDialog;

        public LoginDialogDriver(String userName, String password) {
            loginDialog = new LoginDialog(userName, password);
        }

        public String loginMessage() {
            return loginDialog.Message;
        }

        public int numberOfLoginAttempts() {
            return loginDialog.LoginAttempts;
        }

        public object SystemUnderTest {
            get { return loginDialog; }
        }
    }

    public class LoginDialog {
        private readonly string userName;
        private readonly string password;
        public string Message { get; private set; }
        public int LoginAttempts { get; private set; }

        public LoginDialog(string userName, string password) {
            this.userName = userName;
            this.password = password;
        }

        public bool loginWithUsernameAndPassword(String userName, String password) {
            LoginAttempts++;
            bool result = this.userName.Equals(userName) && this.password.Equals(password);
            Message = result ? String.Format("{0} logged in.", this.userName) : String.Format("{0} not logged in.", this.userName);
            return result;
        }
    }

    public class Bowling {
        public List<object> doTable(List<List<String>> table) {
            Game g = new Game();
            var rollResults = new List<object>
                              {"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""};
            var scoreResults = new List<object>
                               {"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""};
            rollBalls(table, g);
            evaluateScores(g, table[1], scoreResults);
            return new List<object> {rollResults, scoreResults};
        }

        private void evaluateScores(Game g, List<String> scoreRow, List<object> scoreResults) {
            for (int frame = 0; frame < 10; frame++) {
                int actualScore = g.score(frame + 1);
                int expectedScore = int.Parse(scoreRow[frameCoordinate(frame)]);
                if (expectedScore == actualScore)
                    scoreResults[frameCoordinate(frame)] = "pass";
                else
                    scoreResults[frameCoordinate(frame)] = String.Format("Was:{0}, expected:{1}.", actualScore,
                                                                         expectedScore);
            }
        }

        private int frameCoordinate(int frame) {
            return frame < 9 ? frame*2 + 1 : frame*2 + 2;
        }

        private void rollBalls(List<List<String>> table, Game g) {
            List<String> rollRow = table[0];
            for (int frame = 0; frame < 10; frame++) {
                String firstRoll = rollRow[frame*2];
                String secondRoll = rollRow[frame*2 + 1];
                if (firstRoll.ToUpper() == "X")
                    g.roll(10);
                else {
                    int firstRollInt = 0;
                    if (firstRoll.Equals("-"))
                        g.roll(0);
                    else {
                        firstRollInt = int.Parse(firstRoll);
                        g.roll(firstRollInt);
                    }
                    if (secondRoll.Equals("/"))
                        g.roll(10 - firstRollInt);
                    else if (secondRoll.Equals("-"))
                        g.roll(0);
                    else
                        g.roll(int.Parse(secondRoll));
                }
            }
        }

        private class Game {
            private int[] rolls = new int[21];
            private int currentRoll = 0;

            public void roll(int pins) {
                rolls[currentRoll++] = pins;
            }

            public int score(int frame) {
                int score = 0;
                int firstBall = 0;
                for (int f = 0; f < frame; f++) {
                    if (isStrike(firstBall)) {
                        score += 10 + nextTwoBallsForStrike(firstBall);
                        firstBall += 1;
                    }
                    else if (isSpare(firstBall)) {
                        score += 10 + nextBallForSpare(firstBall);
                        firstBall += 2;
                    }
                    else {
                        score += twoBallsInFrame(firstBall);
                        firstBall += 2;
                    }
                }
                return score;
            }

            private int twoBallsInFrame(int firstBall) {
                return rolls[firstBall] + rolls[firstBall + 1];
            }

            private int nextBallForSpare(int firstBall) {
                return rolls[firstBall + 2];
            }

            private int nextTwoBallsForStrike(int firstBall) {
                return rolls[firstBall + 1] + rolls[firstBall + 2];
            }

            private bool isSpare(int firstBall) {
                return rolls[firstBall] + rolls[firstBall + 1] == 10;
            }

            private bool isStrike(int firstBall) {
                return rolls[firstBall] == 10;
            }
        }
    }
}
