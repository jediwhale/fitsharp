// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using fit;
using fit.Service;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitnesse.fitserver
{
	public class FitServer: Runnable
	{
	    public TestCounts Counts { get; private set; }

		private Socket clientSocket;
	    private SocketStream socketStream;
		private bool verbose;
		private string host;
		private int port;
		private string socketToken;
	    private bool IMaybeProcessingSuiteSetup = true;
	    private Configuration configuration;

		private const int ASSEMBLYLIST = 0;
		private const int HOST = 1;
		private const int PORT = 2;
		private const int SOCKET_TOKEN = 3;
		private const int DONE = 4;
	    private static readonly IdentifierName ourSuiteSetupIdentifier = new IdentifierName("suitesetup");

		public static int Main(string[] CommandLineArguments)
		{
			var fitServer = new FitServer();
			fitServer.Run(CommandLineArguments);
			return fitServer.ExitCode();
		}

		public FitServer() {
		    Counts = new TestCounts();
		}

	    public FitServer(Configuration configuration, string host, int port, bool verbose) : this()
		{
		    this.configuration = configuration;
			this.host = host;
			this.port = port;
			this.verbose = verbose;
		    IMaybeProcessingSuiteSetup = false;
		}

	    public int Run(string[] commandLineArguments, Configuration configuration, ProgressReporter reporter) {
	        this.configuration = configuration;
	        Run(commandLineArguments);
	        return ExitCode();
	    }

		private void ParseCommandLineArguments(string[] args)
		{
			int argumentPosition = 0;

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].StartsWith("-"))
				{
					if ("-v".Equals(args[i]))
						verbose = true;
                    else
						PrintUsageAndExit();
				}
				else
				{
					switch (argumentPosition)
					{
						case ASSEMBLYLIST:
							ParseAssemblyList(args[i]);
							break;
						case HOST:
							host = args[i];
							break;
						case PORT:
							port = Int32.Parse(args[i]);
							break;
						case SOCKET_TOKEN:
							socketToken = args[i];
							break;
					}
					argumentPosition++;
				}
			}
			if (argumentPosition != DONE)
				PrintUsageAndExit();
		}

		private static void PrintUsageAndExit()
		{
			Console.Error.WriteLine("Usage: FitServer [-v] [-c config] <assemblies> <host> <port> <socket-token>");
			Console.Error.WriteLine("\t-v\tverbose: print log messages to stdout");
			Console.Error.WriteLine("\tassemblies:\t';' separated list of assembly filenames");
			Environment.Exit(1);
		}

		public void ParseAssemblyList(string path)
		{
			var parser = new PathParser(path);
			foreach (string assemblyPath in parser.AssemblyPaths) {
                if (assemblyPath == "defaultPath") continue;
                configuration.GetItem<ApplicationUnderTest>().AddAssembly(assemblyPath.Replace("\"", string.Empty));
			}
		    if (parser.HasConfigFilePath())
				AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE",parser.ConfigFilePath);
		}

		public int Run(string[] CommandLineArguments)
		{
			ParseCommandLineArguments(CommandLineArguments);

			EstablishConnection();
			ValidateConnection();

			int errorCount = ProcessTestDocuments(WriteFitProtocol);
			CloseConnection();
			Exit();
			return errorCount;
		}

		public void CloseConnection()
		{
			clientSocket.Close();
		}

		public void Exit()
		{
			WriteLogMessage("exiting...");
			WriteLogMessage("End results: " + Counts.Description);
		}

		private void EstablishConnection()
		{
			WriteLogMessage("Host:Port:\t" + host + ":" + port);

			string httpRequest = "GET /?responder=socketCatcher&ticket=" + socketToken + " HTTP/1.1\r\n\r\n";
			EstablishConnection(httpRequest);
		}

		public void EstablishConnection(string request)
		{
			WriteLogMessage("\tHTTP request: " + request);

			clientSocket = SocketConnectionTo(host, port);
            socketStream = new SocketStream(new SocketModelImpl(clientSocket));
			Transmit(request);
		}

		public void ValidateConnection()
		{
			WriteLogMessage("Validating connection...");
			int StatusSize = ReceiveInteger();
			if (StatusSize == 0)
				WriteLogMessage("\t...ok\n");
			else
			{
				String errorMessage = socketStream.ReadBytes(StatusSize);
				WriteLogMessage("\t...failed because: " + errorMessage);
				Console.WriteLine("An error occured while connecting to client.");
				Console.WriteLine(errorMessage);
				Environment.Exit(-1);
			}
		}

		public int ProcessTestDocuments(WriteTestResult writer)
		{
			string document;

            while ((document = ReceiveDocument()).Length > 0)
			{
				WriteLogMessage("processing document of size: " + document.Length);
				ProcessTestDocument(document, writer);
		        IMaybeProcessingSuiteSetup = false;
			}
			WriteLogMessage("\ncompletion signal recieved");

			return ExitCode();
		}

		public int ExitCode()
		{
			return Counts.FailCount;
		}

		private void ProcessTestDocument(string document, WriteTestResult writer)
		{
			try
			{
                Tree<Cell> result = configuration.GetItem<Service>().Compose(new TypedValue(new StoryTestString(document)));
                var parse = result != null ? (Parse)result.Value : null;
			    var storyTest = new StoryTest(parse, writer);
			    WriteLogMessage(parse.Leader);
                if (ourSuiteSetupIdentifier.IsStartOf(parse.Leader) || IMaybeProcessingSuiteSetup)
                    storyTest.ExecuteOnConfiguration();
                else
				    storyTest.Execute();
			}
			catch (Exception e)
			{
			    var testStatus = new TestStatus();
				var parse = new Parse("div", "Unable to parse input. Input ignored.", null, null);
			    testStatus.MarkException(parse, e);
                writer(parse, testStatus.Counts);
			}
		}

		public string TablesToString(Parse tables)
		{
            return configuration.GetItem<Service>().Parse(typeof(StoryTestString), TypedValue.Void, tables).ValueString;
		}

		public void WriteLogMessage(string logMessage)
		{
			if (verbose)
				Console.WriteLine(logMessage);
		}

		private static Socket SocketConnectionTo(string hostName, int port)
		{
			var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		    clientSocket.Connect(hostName, port);
			return clientSocket;
		}

		public string ReceiveDocument()
		{
			int documentLength = ReceiveInteger();
			if (documentLength == 0)
				return "";
		    return socketStream.ReadBytes(documentLength);
		}

		public int ReceiveInteger()
		{
			return DecodeInteger(socketStream.ReadBytes(10));
		}

		public void Transmit(string message)
		{
            socketStream.Write(message);
		}

		public int DecodeInteger(string encodedInteger)
		{
			return Convert.ToInt32(encodedInteger);
		}

		private static string ReadFixedLengthString(TextReader reader, int stringLength)
		{
			var numberCharacters = new char[stringLength];
			reader.Read(numberCharacters, 0, stringLength);

			return new StringBuilder(stringLength).Append(numberCharacters).ToString();
		}

		public int ReadIntegerFrom(StreamReader reader)
		{
			return DecodeInteger(ReadFixedLengthString(reader, 10));
		}

		public void WriteTo(StreamWriter writer, string writeContent)
		{
			writer.Write(Protocol.FormatDocument(writeContent));
			writer.Flush();
		}

		public string ReadFrom(StreamReader reader)
		{
			int contentLength = ReadIntegerFrom(reader);
			return ReadFixedLengthString(reader, contentLength);
		}

	    private void WriteFitProtocol(Tree<Cell> theTables, TestCounts counts)
	    {
            var tables = (Parse) theTables.Value;
		    string testResultDocument = TablesToString(tables);
		    WriteLogMessage("\tTransmitting tables of length " + testResultDocument.Length);
		    Transmit(Protocol.FormatDocument(testResultDocument));

		    WriteLogMessage("\tTest Document finished");
		    Transmit(Protocol.FormatCounts(counts));

            Counts.TallyCounts(counts);
			WriteLogMessage("\tresults: " + counts.Description);
        }
	}
}
