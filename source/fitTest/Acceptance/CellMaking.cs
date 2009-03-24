using System.Text;
using fitlibrary;
using fitlibrary.table;

namespace fit.Test.Acceptance {
    public class CellMaking: DoFixture {

        public string MakeString(string theValue) {
            return Encode(CellFactoryRepository.Instance.Make(theValue));
        }

        public string MakeTable(Table theValue) {
            return Encode(CellFactoryRepository.Instance.Make(theValue));
        }

        private string Encode(Parse theCell) {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("tag: {0}", theCell.Tag);
            if (theCell.Body != null && theCell.Body.Length > 0) result.AppendFormat(" body: {0}", theCell.Body);
            if (theCell.Parts != null) result.AppendFormat(" [ {0}]", Encode(theCell.Parts));
            if (theCell.More != null) result.AppendFormat(", {0}", Encode(theCell.More));
            return result.ToString();
        }

    }
}