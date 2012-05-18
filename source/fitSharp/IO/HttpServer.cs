// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Net;
using System.Text;

namespace fitSharp.IO {
    public class HttpServer: RequestReplyServer {
        public HttpServer(string exitRequest) {
            this.exitRequest = exitRequest;
        }
        
        public void Run(Func<string, string> processRequest) {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:9090/");
            listener.Start();
            while (true) {
                var context = listener.GetContext();
                var request = context.Request.QueryString["test"];
                var response = context.Response;
                var responseString = processRequest(request);
                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentEncoding = Encoding.UTF8;
                response.ContentLength64 = buffer.Length;
                response.ContentType = "text/html";
                response.AddHeader("Access-Control-Allow-Origin", "*");
                var output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
                if (request == exitRequest) break;
            }
            listener.Stop();
        }

        readonly string exitRequest;
    }
}
