// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class Logging: Copyable {
        readonly LogItem item;
        
        public string Show { get { return item.ToString(); } }

        WriteStrategy writeStrategy = new DoNotWrite();

        public Logging() {
            item = new LogItem(string.Empty);
        }

        Logging(LogItem item) {
            this.item = item;
        }

        public void StartWrite(string message) {
            writeStrategy.StartWrite(message, item);
        }

        public void Write(string message) {
            writeStrategy.Write(message, item);
        }

        public void EndWrite(string message) {
            Write(message);
            writeStrategy.EndWrite();
        }

        public void WriteItem(string message) {
            StartWrite(message);
            EndWrite(string.Empty);
        }

        public void Start() {
            writeStrategy = new WriteToMemory();
        }

        public void Stop() {
            writeStrategy = new DoNotWrite();
        }

        public Copyable Copy() {
            return new Logging(item);
        }

        interface WriteStrategy {
            void StartWrite(string message, LogItem item);
            void Write(string message, LogItem item);
            void EndWrite();
        }

        class DoNotWrite: WriteStrategy {
            public void StartWrite(string message, LogItem item) {}
            public void Write(string message, LogItem item) {}
            public void EndWrite() {}
        }

        class WriteToMemory: WriteStrategy {
            int depth;

            public void StartWrite(string message, LogItem item) {
                depth++;
                item.Add(message, depth);
            }

            public void Write(string message, LogItem item) {
                item.Append(message, depth);
            }

            public void EndWrite() {
                depth--;
            }
        }

        class LogItem {
            string message;
            List<LogItem> subItems;

            public LogItem(string message) {
                this.message = message;
            }

            public void AppendMessage(string newMessage) {
                message += newMessage;
            }

            public void Add(string newMessage, int depth) {
                if (subItems == null) subItems = new List<LogItem>();
                if (depth > 1) subItems.Last().Add(newMessage, depth - 1);
                else subItems.Add(new LogItem(newMessage));
            }

            public void Append(string newMessage, int depth) {
                if (subItems == null) return;
                if (depth > 1) subItems.Last().Append(newMessage, depth - 1);
                else subItems.Last().AppendMessage(newMessage);
            }

            public override string ToString() {
                var result = new StringBuilder(message);
                if (subItems != null && subItems.Count > 0) {
                    result.Append("<ul>");
                    foreach (LogItem item in subItems) result.AppendFormat("<li>{0}</li>", item);
                    result.Append("</ul>");
                }
                return result.ToString();
            }
        }
    }
}
