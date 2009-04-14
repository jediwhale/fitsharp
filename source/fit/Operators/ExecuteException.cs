// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Reflection;
using System.Text.RegularExpressions;
using fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
	public class ExecuteException : ExecuteBase {
	    private static readonly IdentifierName exceptionIdentifier = new IdentifierName("exception[");
		private static readonly Regex regexForMessageOnly = new Regex("^\".*\"$");
		private static readonly Regex regexForExceptionTypeNameOnly = new Regex("^.*: \".*\"$");

	    public override bool TryExecute(Processor<Cell> processor, ExecuteParameters parameters, ref TypedValue result) {
		    if (parameters.Verb != ExecuteParameters.Check
                || !exceptionIdentifier.IsStartOf(parameters.Cell.Text) || !parameters.Cell.Text.EndsWith("]")) return false;

			string exceptionContent = parameters.Cell.Text.Substring("exception[".Length, parameters.Cell.Text.Length - ("exception[".Length + 1));
			try {
				parameters.GetActual(processor);
			    parameters.TestStatus.MarkWrong(parameters.ParseCell, "no exception");
			}
			catch (TargetInvocationException e) {
				if (isMessageOnly(exceptionContent)) {
					evaluateException(e.InnerException.Message == exceptionContent.Substring(1, exceptionContent.Length - 2), parameters, e);
				}
				else if (isExceptionTypeNameOnly(exceptionContent)) {
					string actual = e.InnerException.GetType().Name + ": \"" + e.InnerException.Message + "\"";
					evaluateException(exceptionContent == actual, parameters, e);
				}
				else {
					evaluateException(e.InnerException.GetType().Name == exceptionContent, parameters, e);
				}
			}
	        return true;
	    }

		private static bool isExceptionTypeNameOnly(string exceptionContent) {
			return regexForExceptionTypeNameOnly.IsMatch(exceptionContent);
		}

		private static bool isMessageOnly(string exceptionContent) {
			return regexForMessageOnly.IsMatch(exceptionContent);
		}

		private static void evaluateException(bool expression, ExecuteParameters parameters, TargetInvocationException e) {
			if (expression) {
				parameters.TestStatus.MarkRight(parameters.ParseCell);
			}
			else {
				parameters.TestStatus.MarkWrong(parameters.ParseCell, "exception[" + e.InnerException.GetType().Name + ": \"" + e.InnerException.Message + "\"]");
			}
		}
	}
}