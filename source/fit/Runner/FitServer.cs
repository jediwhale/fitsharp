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
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitnesse.fitserver
{
	public class FitServer: Runnable
	{
	    public TestStatus Status { get; private set; }

		private Socket clientSocket;
	    private SocketStream socketStream;
		private bool verbose = false;
		private string host;
		private int port;
		private string socketToken;
		public FixtureListener fixtureListener;
	    private bool IMaybeProcessingSuiteSetup = true;
	    private Configuration configuration;

		private const int ASSEMBLYLIST = 0;
		private const int HOST = 1;
		private const int PORT = 2;
		private const int SOCKET_TOKEN = 3;
		private const int DONE = 4;

		public static int Main(string[] CommandLineArguments)
		{
			FitServer fitServer = new FitServer();
			fitServer.Run(CommandLineArguments);
			return fitServer.ExitCode();
		}

		public FitServer() {
		    Status = new TestStatus();
		    fixtureListener = new TablePrintingFixtureListener(this);
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
			PathParser parser = new PathParser(path);
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

			int errorCount = ProcessTestDocuments();
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
			WriteLogMessage("End results: " + Status.CountDescription);
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

		public int ProcessTestDocuments()
		{
			string document;

            while ((document = ReceiveDocument()).Length > 0)
			{
				WriteLogMessage("processing document of size: " + document.Length);
				TestStatus currentStatus = ProcessTestDocument(document);
				Status.TallyCounts(currentStatus);
				WriteLogMessage("\tresults: " + currentStatus.CountDescription);
		        IMaybeProcessingSuiteSetup = false;
			}
			WriteLogMessage("\ncompletion signal recieved");

			return ExitCode();
		}

		public int ExitCode()
		{
			return Status.FailCount;
		}

		private TestStatus ProcessTestDocument(string document)
		{
			return RunTest(document);
		}

		private TestStatus RunTest(string document)
		{
			try
			{
                Tree<Cell> result = configuration.GetItem<Service>().Compose(new StoryTestString(document));
                var parse = result != null ? (Parse)result.Value : null;
			    StoryTest storyTest = new StoryTest(parse, fixtureListener);
			    WriteLogMessage(parse.Leader);
                if (ourSuiteSetupIdentifier.IsStartOf(parse.Leader) || IMaybeProcessingSuiteSetup)
                    storyTest.ExecuteOnConfiguration();
                else
				    storyTest.Execute();
				return storyTest.TestStatus;
			}
			catch (Exception e)
			{
			    Fixture fixture = new Fixture();
				Parse parse = new Parse("body", "Unable to parse input. Input ignored.", null, null);
				fixture.Exception(parse, e);
				fixtureListener.TableFinished(parse);
				return fixture.TestStatus;
			}
		}

		public string FirstTableOf(Parse tables)
		{
            return configuration.GetItem<Service>().Parse<StoryTestString>(tables).ToString();
			/*Parse more = tables.More;
			tables.More = null;
			StringWriter writer = new StringWriter();
			tables.Print(writer);
			string firstTable = writer.ToString();
			tables.More = more;
			return firstTable;*/
		}

		public void WriteLogMessage(string logMessage)
		{
			if (verbose)
				Console.WriteLine(logMessage);
		}

		private static Socket SocketConnectionTo(string hostName, int port)
		{
			Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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

		private static string ReadFixedLengthString(StreamReader reader, int stringLength)
		{
			char[] numberCharacters = new char[stringLength];
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

	    private static readonly IdentifierName ourSuiteSetupIdentifier = new IdentifierName("suitesetup");
	}

	public class TablePrintingFixtureListener : FixtureListener
	{
		private FitServer fitServer;

		public TablePrintingFixtureListener(FitServer fitServer)
		{
			this.fitServer = fitServer;
		}

		public void TableFinished(Parse finishedTable)
		{
			string testResultDocument = fitServer.FirstTableOf(finishedTable);
			fitServer.WriteLogMessage("\tTransmitting table of length " + testResultDocument.Length);
			fitServer.Transmit(Protocol.FormatDocument(testResultDocument));
		}

		public void TablesFinished(Parse theTables, TestStatus status)
		{
			fitServer.WriteLogMessage("\tTest Document finished");
			fitServer.Transmit(Protocol.FormatCounts(status));
		}
		
	}
}