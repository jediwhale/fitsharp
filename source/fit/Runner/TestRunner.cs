// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.IO;
using fit;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;

namespace fitnesse.fitserver
{
	public class TestRunner: Runnable
	{
		public string pageName;
		public bool usingDownloadedPaths = true;
		private FitServer fitServer;
		private FixtureListener fixtureListener;
		public string host;
		public int port;
		public bool debug;
		public bool verbose;
	    public ResultWriter resultWriter = new NullResultWriter();
		public bool deleteCacheOnExit;
		public TestStatus pageCounts = new TestStatus();
		public TextWriter output = Console.Out;
		public string assemblyPath;
	    public string suiteFilter = null;

	    public int Run(string[] commandLineArguments, Configuration configuration, ProgressReporter reporter) {
			if(!ParseArgs(configuration, commandLineArguments))
			{
				PrintUsage();
				return ExitCode();
			}
			fixtureListener = new TestRunnerFixtureListener(this, configuration);
			fitServer.fixtureListener = fixtureListener;
			fitServer.EstablishConnection(MakeHttpRequest());
			fitServer.ValidateConnection();
			AddAssemblies();
			fitServer.ProcessTestDocuments();
			HandleFinalCount(fitServer.Status);
			fitServer.CloseConnection();
			fitServer.Exit();
			resultWriter.Close();
	        return ExitCode();
	    }

		public void AddAssemblies()
		{
			if(usingDownloadedPaths)
				assemblyPath += fitServer.ReceiveDocument();
			if(assemblyPath != null && assemblyPath.Length > 0)
			{
				if(verbose)
					output.WriteLine("Adding assemblies: " + assemblyPath);
				fitServer.ParseAssemblyList(assemblyPath);	
			}
		}

		public int ExitCode()
		{
			return fitServer == null ? -1 : fitServer.ExitCode();
		}

		public bool ParseArgs(Configuration configuration, string[] args)
		{
		    string resultsFile = null;
		    string outputType = "text";
			int index = 0;
			try
			{
				while(args[index].StartsWith("-"))
				{
					string option = args[index++];
					if("-results".Equals(option))
					    resultsFile = args[index++];
					else if("-v".Equals(option))
						verbose = true;
					else if("-debug".Equals(option))
						debug = true;
					else if("-nopaths".Equals(option))
						usingDownloadedPaths = false;
                    else if (option == "-suiteFilter")
                        suiteFilter = args[index++];
                    else if ("-format".Equals(option))
                        outputType = args[index++];
					else
						throw new Exception("Bad option: " + option);
				}
			    CreateResultWriter(resultsFile, outputType);
				host = args[index++];
				port = Int32.Parse(args[index++]);
				pageName = args[index++];
			    fitServer = new FitServer(configuration, host, port, debug);
			    string assemblies = null;
				while(args.Length > index)
					assemblies = assemblies == null ? args[index++] : (assemblies + ";" + args[index++]);
                if (assemblies != null) {
                    fitServer.ParseAssemblyList(assemblies);
                }
				return true;
			}
			catch(Exception)
			{
				return false;
			}
		}

		private static void PrintUsage()
		{
			Console.WriteLine("Usage: TestRunner [options] <host> <port> <page name> [assembly[;assembly]]");
			Console.WriteLine("\t-c <configfile>\tconfig: loads configuration from file");
			Console.WriteLine("\t-v\tverbose: prints test progress to console");
			Console.WriteLine("\t-debug\tprints FitServer actions to console");
			Console.WriteLine("\t-nopaths\tprevents addition of assemblies from remote FitNesse");
			Console.WriteLine("\t-results <filename|'stdout'>\tsends test results data to the specified file or the console");
			Console.WriteLine("\t-format <xml|text>\toutputs test results data to the specified format");
			Console.WriteLine("\t-suiteFilter <filter>\tonly run test pages with tag <filter>");
		}

		public string MakeHttpRequest()
		{
			string request = "GET /" + pageName + "?responder=fitClient";
			if(usingDownloadedPaths)
				request += "&includePaths=yes";
            if (suiteFilter != null)
                request += "&suiteFilter=" + suiteFilter;
			return request + " HTTP/1.1\r\n\r\n";
		}

		public void AcceptResults(PageResult results)
		{
			TestStatus status = results.TestStatus;
			pageCounts.TallyPageCounts(status);
			fitServer.Transmit(Protocol.FormatCounts(status));
			if(verbose)
			{
				for(int i = 0; i < status.GetCount(CellAttributes.RightStatus); i++)
					output.Write(".");
				if(status.GetCount(CellAttributes.WrongStatus) > 0)
				{
					output.WriteLine();
					output.WriteLine(PageDescription(results) + " has failures");
				}
				if(status.GetCount(CellAttributes.ExceptionStatus) > 0)
				{
					output.WriteLine();
					output.WriteLine(PageDescription(results) + " has errors");
				}

			}
		    resultWriter.WritePageResult(results);
		}

		private static string PageDescription(PageResult result)
		{
			String description = result.Title;
			if("".Equals(description))
				description = "The test";
			return description;
		}

	    private void CreateResultWriter(string fileName, string outputType)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (outputType.Equals("xml"))
                    resultWriter = new XmlResultWriter(fileName, new FileSystemModel());
                else
                    resultWriter = new TextResultWriter(fileName, new FileSystemModel());
            }
        }

		public void HandleFinalCount(TestStatus summary)
		{
			if(verbose)
			{
				output.WriteLine();
				output.WriteLine("Test Pages: " + pageCounts.CountDescription);
				output.WriteLine("Assertions: " + summary.CountDescription);
			}
			resultWriter.WriteFinalCount(summary);
		}
	}
}