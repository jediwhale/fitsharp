using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseArray: ParseOperator<Cell> {
        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (!type.IsArray) return false;

			string[] strings = parameters.Value.Text.Split(new [] {','});

			Array list = Array.CreateInstance(type.GetElementType(), strings.Length);
			for (int i = 0; i < strings.Length; i++) {
                //todo: use cellsubstring?
			    list.SetValue(processor.ParseString(type.GetElementType(), strings[i]).Value, i);
			}

            result = new TypedValue(list);
            return true;
        }
    }
}
