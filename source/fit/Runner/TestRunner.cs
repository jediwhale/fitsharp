// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.IO;
using fit.Runner;
using fit.Service;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fitnesse.fitserver
{
	public class TestRunner: Runnable
	{
		public string pageName;
		public bool usingDownloadedPaths = true;
		public string host;
		public int port;
		public bool debug;
		public bool verbose;
	    public ResultWriter resultWriter = new NullResultWriter();
		public bool deleteCacheOnExit;
		public TestCounts pageCounts = new TestCounts();
		public TextWriter output = Console.Out;
		public string assemblyPath;
	    public string suiteFilter;
	    private Configuration configuration;
	    private readonly TestCounts totalCounts = new TestCounts();
	    private SocketServer server;
	    private FitSocket clientSocket;

	    public int Run(string[] commandLineArguments, Configuration configuration, ProgressReporter reporter) {
	        this.configuration = configuration;
			if(!ParseArgs(configuration, commandLineArguments))
			{
				PrintUsage();
				return ExitCode();
			}
	        clientSocket = new FitSocket(new SocketModelImpl(host, port), MakeReporter());
		    server = new SocketServer(clientSocket, new Service(configuration), MakeReporter(), false);
			clientSocket.EstablishConnection(Protocol.FormatRequest(pageName, usingDownloadedPaths, suiteFilter));
			AddAssemblies();
		    server.ProcessTestDocuments(WriteTestRunner);
			HandleFinalCount(totalCounts);
			clientSocket.Close();
			Exit();
			resultWriter.Close();
	        return ExitCode();
	    }

	    private void Exit()
		{
			WriteLogMessage("exiting...");
			WriteLogMessage("End results: " + totalCounts.Description);
		}

		private void WriteLogMessage(string logMessage)
		{
			if (verbose)
				Console.WriteLine(logMessage);
		}

        private ProgressReporter MakeReporter() {
            if (verbose) return new ConsoleReporter();
            return new NullReporter();
        }

		private void AddAssemblies()
		{
			if(usingDownloadedPaths)
				assemblyPath += clientSocket.ReceiveDocument();
			if(!string.IsNullOrEmpty(assemblyPath))
			{
				if(verbose)
					output.WriteLine("Adding assemblies: " + assemblyPath);
		        new PathParser(assemblyPath).AddAssemblies(configuration);
			}
		}

		private int ExitCode()
		{
			return totalCounts.FailCount;
		}

	    public bool ParseArgs(Configuration configuration, string[] args) {
	        string resultsFile = null;
	        string outputType = "text";
	        int index = 0;
	        try {
	            while (args[index].StartsWith("-")) {
	                string option = args[index++];
	                if ("-results".Equals(option))
	                    resultsFile = args[index++];
	                else if ("-v".Equals(option))
	                    verbose = true;
	                else if ("-debug".Equals(option))
	                    debug = true;
	                else if ("-nopaths".Equals(option))
	                    usingDownloadedPaths = false;
	                else if (option == "-suiteFilter")
	                    suiteFilter = args[index++];
	                else if ("-format".Equals(option))
	                    outputType = args[index++];
	                else
	                    throw new Exception("Bad option: " + option);
	            }
	            CreateResultWriter(resultsFile, outputType,
	                               new FileSystemModel(configuration.GetItem<Settings>().CodePageNumber));
	            host = args[index++];
	            port = Int32.Parse(args[index++]);
	            pageName = args[index++];
	            string assemblies = null;
	            while (args.Length > index)
	                assemblies = assemblies == null ? args[index++] : (assemblies + ";" + args[index++]);
	            if (assemblies != null) {
	                new PathParser(assemblies).AddAssemblies(configuration);
	            }
	            return true;
	        }
	        catch (Exception) {
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

	    private void CreateResultWriter(string fileName, string outputType, FolderModel folders) {
	        if (string.IsNullOrEmpty(fileName)) return;
	        if (outputType == "xml")
	            resultWriter = new XmlResultWriter(fileName, folders);
	        else
	            resultWriter = new TextResultWriter(fileName, folders);
	    }

	    public void HandleFinalCount(TestCounts summary)
		{
			if(verbose)
			{
				output.WriteLine();
				output.WriteLine("Test Pages: " + pageCounts.Description);
				output.WriteLine("Assertions: " + summary.Description);
			}
			resultWriter.WriteFinalCount(summary);
		}

		private void WriteTestRunner(string data, TestCounts counts)
		{
			int indexOfFirstLineBreak = data.IndexOf("\n");
			string pageTitle = data.Substring(0, indexOfFirstLineBreak);
			data = data.Substring(indexOfFirstLineBreak + 1);
			AcceptResults(new PageResult(pageTitle, data, counts));
		}

		private void AcceptResults(PageResult results)
		{
			TestCounts counts = results.TestCounts;
			pageCounts.TallyPageCounts(counts);
            totalCounts.TallyCounts(counts);
			clientSocket.SendCounts(counts);
			if(verbose)
			{
				for(int i = 0; i < counts.GetCount(TestStatus.Right); i++)
					output.Write(".");
				if(counts.GetCount(TestStatus.Wrong) > 0)
				{
					output.WriteLine();
					output.WriteLine(PageDescription(results) + " has failures");
				}
				if(counts.GetCount(TestStatus.Exception) > 0)
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
	}
}
