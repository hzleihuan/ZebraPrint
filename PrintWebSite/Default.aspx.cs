using System;
using System.Data;
using PrintLib.Printers.Zebra;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
       
    }
    //打印 小票（标签）
    protected void Button3_Click(object sender, EventArgs e)
    {
        DataTable dt = new DataTable("table1");
        dt.Columns.Add(new DataColumn("HName", typeof(string)));
        dt.Columns.Add(new DataColumn("AName", typeof(string)));
        dt.Columns.Add(new DataColumn("ANo", typeof(string)));
        dt.Columns.Add(new DataColumn("UResponsible", typeof(string)));
        dt.Columns.Add(new DataColumn("MResponsible", typeof(string)));
        dt.Columns.Add(new DataColumn("RPerson", typeof(string)));
        dt.Columns.Add(new DataColumn("UPrice", typeof(string)));
        dt.Columns.Add(new DataColumn("BDate", typeof(string)));
        DataRow row = dt.NewRow();
        row["HName"] = "湖州检验检疫局";
        row["AName"] = "资产名称：" + " 笔记本";
        row["ANo"] = "资产编号："+"  200933HFC0006";
        row["UResponsible"] = "使用单位：" + "  财务处";
        row["MResponsible"] = "管理单位：" + "  综合处";
        row["RPerson"] = "负责人：" +"  蒲新根";
        row["BDate"] = "购置日期： "+System.DateTime.Now.ToShortDateString();
        row["UPrice"] = "单价：5800.00";
        dt.Rows.Add(row);
        Printer printer = new Printer();
        printer.ZPLPrintDeviceLabel(dt, 1);
        #region old print mode
        //PrinterWSR.ZebraPrinter zebraPrinterWS = new PrinterWSR.ZebraPrinter();
        ////zebraPrinterWS.de

        //Process p = new Process();

        //try
        //{

        //    p.StartInfo.FileName = @"D:\Debug\PrintApp.exe";
        //    p.StartInfo.UseShellExecute = false;
        //    p.StartInfo.RedirectStandardInput = true;
        //    p.StartInfo.RedirectStandardOutput = true;
        //    p.StartInfo.CreateNoWindow = false;
            
        //    p.Start();
        //    //p.StandardInput.WriteLine(TextBox1.Text.Trim());

        //    //Process.Start("D:/Debug/PrintApp.exe", TextBox1.Text.Trim());
        //}
        //catch
        //{

        //}
        //finally
        //{
        //    p.WaitForExit();
        //    p.Dispose();
        //}
        ////p.Kill();
        #endregion
    }
    //生成图片
    protected void Button1_Click(object sender, EventArgs e)
    {
        //根据一定的规则，生成序列号作为唯一标识 如：
        string num = "01008D004Q-0";
        string fileName = num+".png";
        //生成条形码
        try
        {
            String savePath = Server.MapPath("BarcodeImages") + "/" + fileName;
            new PrintLib.Printers.Zebra.Printer().CreateBarcodeImage(num, savePath);
            this.Image1.ImageUrl = "BarcodeImages/" + fileName;
        }
        catch (Exception)
        {

        }
        //生成二维码图片
        try
        {
            String savePath = Server.MapPath("QRCodeImages") + "/" + fileName;
            new PrintLib.Printers.Zebra.Printer().CreateQRCodeImage(num, savePath);
            this.Image2.ImageUrl = "QRCodeImages/" + fileName;
        }
        catch (Exception)
        {

        }
    }
    //打印
    protected void Button2_Click(object sender, EventArgs e)
    {
        //批量打印时先从选中的记录中取出序列号放入一个字符型数组中 如：{"序列号1,名称1","序列号2,名称2"}
        string[] str = new string[] { "01008D004Q-0,试件名称！" ,"01008D004Q-2,试件名称2！" };
        /*
         单列打印请修改
         * PrintLib.Printers.Zebra.Printer().PrintQRCode方法中的打印指令
         */
        //执行批量打印（双列标签）
        for (int i = 0; i < str.Length; i++)
        {
            if ((i + 1) == str.Length)
            {
                new PrintLib.Printers.Zebra.Printer().PrintQRCode(0, 0, 0, 0, str[i], str[i]);
            }
            else
            {
                new PrintLib.Printers.Zebra.Printer().PrintQRCode(0, 0, 0, 0, str[i], str[i + 1]);
            }
            i++;
        }
        /*
        for (int i = 0; i < str.Length; i++)
        {
            if ((i + 1) == str.Length)
            {
                new PrintLib.Printers.Zebra.Printer().PrintBarcode(0, 0, 0, 0, str[i], str[i]);
            }
            else
            {
                new PrintLib.Printers.Zebra.Printer().PrintBarcode(0, 0, 0, 0, str[i], str[i + 1]);
            }
            i++;
        }*/
    }
}
