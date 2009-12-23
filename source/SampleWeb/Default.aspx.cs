using System;

namespace SampleWeb {
    public partial class _Default : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {}

        protected void plusButton_Click(object sender, EventArgs e) {
            resultLabel.Text = (int.Parse(TextBox1.Text)
                + int.Parse(TextBox2.Text)).ToString();
        }
    }
}
