using System;
using System.Web.UI.WebControls;

namespace fitSharp.Samples {
    public class SampleWebAdapter: SampleWeb._Default {
        public SampleWebAdapter() {
            TextBox1 = new TextBox();
            TextBox2 = new TextBox();
            resultLabel = new Label();
        }

        public string Text1 { set { TextBox1.Text = value; } }
        public string Text2 { set { TextBox2.Text = value; } }
        public string Result { get { return resultLabel.Text; } }
        public void Plus() { plusButton_Click(null, new EventArgs()); }
    }
}
