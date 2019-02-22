using System;
using PrintLib.Printers.Zebra;

public partial class To : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btn_Click(object sender, EventArgs e)
    {
        Printer printer = new Printer();

        string text = this.tb.Text.Trim();

        string contents = printer.TextToHex(text, "li", 40);
        lit.Text = contents;
    }
}
