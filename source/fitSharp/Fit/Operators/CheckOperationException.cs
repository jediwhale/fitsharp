// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Reflection;
using System.Text.RegularExpressions;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CheckOperationException: CellOperator, InvokeOperator<Cell> {
        private static readonly IdentifierName exceptionIdentifier = new IdentifierName("exception[");
        private static readonly Regex regexForMessageOnly = new Regex("^\".*\"$");
        private static readonly Regex regexForExceptionTypeNameOnly = new Regex("^.*: \".*\"$");

        public bool CanInvoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            return instance.Type == typeof (CellOperationContext) && memberName == CellOperationContext.CheckCommand
                && exceptionIdentifier.IsStartOf(parameters.Value.Text) && parameters.Value.Text.EndsWith("]");
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<Cell> parameters) {
            var context = instance.GetValue<CellOperationContext>();
            string exceptionContent = parameters.Value.Text.Substring("exception[".Length, parameters.Value.Text.Length - ("exception[".Length + 1));
            try {
                context.GetActual(Processor);
                Processor.TestStatus.MarkWrong(parameters.Value, "no exception");
            }
            catch (TargetInvocationException e) {
                if (IsMessageOnly(exceptionContent)) {
                    EvaluateException(e.InnerException.Message == exceptionContent.Substring(1, exceptionContent.Length - 2), parameters, e);
                }
                else if (IsExceptionTypeNameOnly(exceptionContent)) {
                    string actual = e.InnerException.GetType().Name + ": \"" + e.InnerException.Message + "\"";
                    EvaluateException(exceptionContent == actual, parameters, e);
                }
                else {
                    EvaluateException(e.InnerException.GetType().Name == exceptionContent, parameters, e);
                }
            }
            return TypedValue.Void;
        }

        private static bool IsExceptionTypeNameOnly(string exceptionContent) {
            return regexForExceptionTypeNameOnly.IsMatch(exceptionContent);
        }

        private static bool IsMessageOnly(string exceptionContent) {
            return regexForMessageOnly.IsMatch(exceptionContent);
        }

        private void EvaluateException(bool expression, Tree<Cell> parameters, TargetInvocationException e) {
            if (expression) {
                Processor.TestStatus.MarkRight(parameters.Value);
            }
            else {
                Processor.TestStatus.MarkWrong(parameters.Value, "exception[" + e.InnerException.GetType().Name + ": \"" + e.InnerException.Message + "\"]");
            }
        }

    }
}