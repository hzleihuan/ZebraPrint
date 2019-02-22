using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;

namespace PrintLib.Printers.Zebra
{
    using System = global::System;
    using ThoughtWorks.QRCode.Codec;
    using System.Drawing;
    using System.Drawing.Printing;
    public class Printer
    {
        public string Name;
        #region 从Web.config文件中获取打印机名称，如
        public Printer()
        {
            this.Name = System.Configuration.ConfigurationManager.AppSettings["Printer"];
        }

        public Printer(string name)
        {
            this.Name = name;
        }
        #endregion

        #region DLL声明
        //ZPL
        [DllImport(@"FNTHEX32.DLL", CharSet = CharSet.Ansi)]
        public static extern int GETFONTHEX(
                          string chnstr,
                          string fontname,
                          string chnname,
                          int orient,
                          int height,
                          int width,
                          int bold,
                          int italic,
                          StringBuilder param1);
        //EPL
        [DllImport(@"Eltronp.dll", CharSet = CharSet.Ansi)]
        public static extern int PrintHZ(int Lpt, //0：LPT1，1 LPT2
                                         int x,
                                         int y,
                                         string HZBuf,
                                         string FontName,
                                         int FontSize,
                                         int FontStyle);
        #endregion

        #region 指令说明
        /**
        ^XA 开始 ^XZ 结束
        ^LH起始坐标  ^PR进纸回纸速度 ^MD 对比度
        ^FO标签左上角坐标  ^XG打印图片参数1图片名称后两个为坐标
        ^FS标签结束符  ^CI切换国际字体 ^FT坐标 ^FD定义一个字符串
        ^A定义字体  ^FH十六进制数 ^BY模块化label ^BC条形码128  
        ^PQ打印设置 参数一 打印数量 参数二暂停 参数三重复数量  参数四为Y时表明无暂停
         **/
        #endregion

        #region PrintDocument 打印条码、二维码
        public void Print()
        {
            System.Drawing.Printing.PrintDocument _Document = new System.Drawing.Printing.PrintDocument();
            _Document.PrintPage += _Document_PrintPage;
            PageSettings pageSet = new PageSettings();
            pageSet.Landscape = false;
            pageSet.Margins.Top = 0;
            pageSet.Margins.Left = 1;
            pageSet.PaperSize = new System.Drawing.Printing.PaperSize("小票", 2, 2);
            _Document.DefaultPageSettings = pageSet;
            _Document.Print();
        }
        void _Document_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            float x = 0;
            float y = 0;
            float width = 300;
            float height = 60;
            e.Graphics.DrawImage(CreateBarcodeImage("--test--",""), x, y, width, height);
            e.HasMorePages = false;
        }
        #endregion

        #region ZPL 打印条码、二维码
        /// <summary>
        /// 条码打印(标签两列)
        /// </summary>
        /// <param name="l">左边距 推荐值：0</param>
        /// <param name="h">上边距 推荐值：0</param>
        /// <param name="cl">第一列与第二列的距离 推荐值：0</param>
        /// <param name="bch">条码高 推荐值：0</param>
        /// <param name="str">条码内容1，内容2两个字符串 推荐值：11位字母数字组成</param>
        /// <returns>true /false 执行状态</returns>
        public bool PrintBarcode(int l,int h,int cl,int bch, params string[] str)
        {
            l = l == 0 ? 0 : l;
            h = h == 0 ? 8 : h;
            cl = cl == 0 ? 30 : cl;
            bch = bch == 0 ? 100 : bch;
            StringBuilder sb = new StringBuilder();
            sb.Append("^XA");
            //连续打印两列（单列只写一条）
            sb.Append(string.Format("^MD30^LH{0},{1}^FO{2},{3}^ACN,18,10^BY1.8,3,{4}^BC,,Y,N^FD{5}^FS", l, h, l, h, bch, str[0]));
            sb.Append(string.Format("^MD30^LH{0},{1}^FO{2},{3}^ACN,18,10^BY1.8,3,{4}^BC,,Y,N^FD{5}^FS", l, h, l + cl, h, bch, str[1]));
        
            sb.Append("^XZ");
            return RawPrinterHelper.SendStringToPrinter(this.Name, sb.ToString());
        }
        /// <summary>
        /// 二维码打印(标签两列)
        /// </summary>
        /// <param name="l">左边距 推荐值：0</param>
        /// <param name="h">上边距 推荐值：0</param>
        /// <param name="cl">第一列与第二列的距离 推荐值：0</param>
        /// <param name="bch">二维码放大倍数 推荐值：0</param>
        /// <param name="str">内容1，内容2两个字符串 推荐值：字母数字组成（位数不限制）</param>
        /// <returns>true /false 执行状态</returns>
        public bool PrintQRCode(int l, int h, int cl, int bch, params string[] str)
        {
            if (str.Length <2) return false;
            string[] str0 = str[0].Split(',');
            string[] str1 = str[1].Split(',');

            l = l == 0 ? 5 : l;
            h = h == 0 ? 8 : h;
            cl = cl == 0 ? 360 : cl;
            bch = bch == 0 ? 5 : bch;
            
            StringBuilder sb = new StringBuilder();
            sb.Append("^XA");
            sb.Append(string.Format("^LH{0},{1}^FO{2},{3}^BQ,2,{4}^FDQA,{5}^FS", l, h, 0, h, bch, str0[0]));
            sb.Append(TextToHex(str0[1], str0[0], 24));
            sb.Append(string.Format("^FT{0},{1}^XG{2},1,1^FS", 125+l, h+60, str0[0]));
            sb.Append(TextToHex(str0[0], str0[0], 20));
            sb.Append(string.Format("^FT{0},{1}^XG{2},1,1^FS", 125 + l, h + 90, str0[0]));

            sb.Append(string.Format("^LH{0},{1}^FO{2},{3}^BQ,2,{4}^FDQA,{5}^FS", l, h, 0 + cl, h, bch, str1[0]));
            sb.Append(TextToHex(str1[1], str1[0], 24));
            sb.Append(string.Format("^FT{0},{1}^XG{2},1,1^FS", 125 + cl + l, h + 60, str1[0]));
            sb.Append(TextToHex(str1[0], str1[0], 20));
            sb.Append(string.Format("^FT{0},{1}^XG{2},1,1^FS", 125 + cl + l, h + 90, str1[0]));
            sb.Append("^XZ");
            return RawPrinterHelper.SendStringToPrinter(this.Name, sb.ToString());
        }
        #endregion

        #region 条码、二维码图片生成
        /// <summary>
        /// 生成条形码图片
        /// </summary>
        /// <param name="num">条形码序列号</param>
        /// <param name="path">图片存放路径（绝对路径）</param>
        /// <returns>返回图片</returns>
        public System.Drawing.Image CreateBarcodeImage(string num, string path)
        {
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            b.BackColor = System.Drawing.Color.White;
            b.ForeColor = System.Drawing.Color.Black;
            b.IncludeLabel = true;
            b.Alignment = BarcodeLib.AlignmentPositions.CENTER;
            b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
            b.ImageFormat = System.Drawing.Imaging.ImageFormat.Png;
            System.Drawing.Font font = new System.Drawing.Font("verdana", 10f);
            b.LabelFont = font;
            try
            {
                System.Drawing.Image image = b.Encode(BarcodeLib.TYPE.CODE128B, num);
                image.Save(path);
                return image;
            }
            catch (Exception)
            {

            }
            return null;
        }
        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="num">二维码序列号</param>
        /// <param name="path">图片存放路径（绝对路径）</param>
        /// <returns>返回图片</returns>
        public System.Drawing.Image CreateQRCodeImage(string num,string path)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            String encoding = "Byte";
            if (encoding == "Byte")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            }
            else if (encoding == "AlphaNumeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            }
            else if (encoding == "Numeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
            }
            try
            {
                int scale = Convert.ToInt16(4);
                qrCodeEncoder.QRCodeScale = scale;
            }
            catch (Exception)
            {

            }
            try
            {
                int version = Convert.ToInt16(7);
                qrCodeEncoder.QRCodeVersion = version;
            }
            catch (Exception)
            {

            }

            string errorCorrect = "M";
            if (errorCorrect == "L")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            else if (errorCorrect == "M")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            else if (errorCorrect == "Q")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (errorCorrect == "H")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
            try
            {
                Bitmap bm = qrCodeEncoder.Encode(num);
                bm.Save(path);
                MemoryStream ms = new MemoryStream();
                bm.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return System.Drawing.Image.FromStream(ms);
            }
            catch (Exception)
            {

            }
            return null;
        }
        #endregion

        #region ZPL 打印中文小票
        public string TextToHex(string text, string textId, int height)
        {
            StringBuilder hexBuilder = new StringBuilder(4 * 1024);
            int subStrCount = 0;
            subStrCount = GETFONTHEX(text, "Arial", textId, 0, height, 0, 1, 0, hexBuilder);
            return hexBuilder.ToString().Substring(0, subStrCount);
        }
        public string DeviceLabelToHex(string text, string textId)
        {
            StringBuilder hexBuilder = new StringBuilder(4 * 1024);
            int subStrCount = 0;
            subStrCount = GETFONTHEX(text, "Arial", textId, 0, 40, 0, 1, 0, hexBuilder);
            return hexBuilder.ToString().Substring(0, subStrCount);
        }
        public bool IsZebraPrinter(string printerName)
        {
            return printerName.IndexOf("ZDesigner") + printerName.IndexOf("Zebra") >= -1;
        }
        public void ZPLPrintDeviceLabel(DataTable dt, int copies)
        {
            ZPLPrintDeviceLabel(dt, true, copies);
        }
        public void ZPLPrintDeviceLabel(DataTable dt, bool isRequireTextToHex, int copies)
        {
            Project.Printers.Prints.Label label = null;
            for (int i = 0; i < copies; i++)
            {
                foreach (DataRow row in dt.Rows)
                {
                    List<Project.Printers.Prints.Label> labelList = new List<Project.Printers.Prints.Label>();
                    label = new Project.Printers.Prints.Label();
                    #region Data
                    label.Id = "HName";
                    label.Text = row["HName"].ToString();
                    label.XPos = 120;
                    label.YPos = 80;
                    labelList.Add(label);

                    label = new Project.Printers.Prints.Label();
                    label.Id = "AName";
                    label.Text = row["AName"].ToString();
                    label.XPos = 10;
                    label.YPos = 110;
                    labelList.Add(label);

                    //label = new Project.Printers.Prints.Label();
                    //label.Id = "name1";
                    //label.Text = row["name1"].ToString();
                    //label.XPos = 70;
                    //label.YPos = 180;
                    //labelList.Add(label);

                    label = new Project.Printers.Prints.Label();
                    label.Id = "ANo";
                    label.Text = row["ANo"].ToString();
                    label.XPos = 10;
                    label.YPos = 130;
                    labelList.Add(label);

                    //label = new Project.Printers.Prints.Label();
                    //label.Id = "No1";
                    //label.Text = row["No1"].ToString();
                    //label.XPos = 70;
                    //label.YPos = 245;
                    //labelList.Add(label);

                    label = new Project.Printers.Prints.Label();
                    label.Id = "UResponsible";
                    label.Text = row["UResponsible"].ToString();
                    label.XPos = 10;
                    label.YPos = 150;
                    labelList.Add(label);

                    //label = new Project.Printers.Prints.Label();
                    //label.Id = "Responsible1";
                    //label.Text = row["Responsible1"].ToString();
                    //label.XPos = 70;
                    //label.YPos = 305;
                    //labelList.Add(label);

                    label = new Project.Printers.Prints.Label();
                    label.Id = "RPerson";
                    label.Text = row["RPerson"].ToString();
                    label.XPos = 10;
                    label.YPos = 170;
                    labelList.Add(label);

                    //label = new Project.Printers.Prints.Label();
                    //label.Id = "Person1";
                    //label.Text = row["Person1"].ToString();
                    //label.XPos = 355;
                    //label.YPos = 180;
                    //labelList.Add(label);

                    label = new Project.Printers.Prints.Label();
                    label.Id = "BDate";
                    label.Text = row["BDate"].ToString();
                    label.XPos = 10;
                    label.YPos =190;

                    labelList.Add(label); label = new Project.Printers.Prints.Label();
                    label.Id = "UPrice";
                    label.Text = row["UPrice"].ToString();
                    label.XPos = 10;
                    label.YPos = 210;
                    labelList.Add(label);

                    labelList.Add(label); label = new Project.Printers.Prints.Label();
                    label.Id = "MResponsible";
                    label.Text = row["MResponsible"].ToString();
                    label.XPos = 10;
                    label.YPos = 230;
                    #endregion
                    labelList.Add(label);

                    if (isRequireTextToHex)
                    {
                        ZPLPrintLabels(this.Name, labelList.ToArray(), 20);
                    }
                    else
                    {
                        ZPLPrintLabelsWithHexText(this.Name, labelList.ToArray());
                    }
                }
            }
        }
        private void ZPLPrintLabelsWithHexText(string printerName, Project.Printers.Prints.Label[] labels)
        {
            string labelIdCmd = string.Empty;
            string labelContentCmd = string.Empty;
            foreach (Project.Printers.Prints.Label label in labels)
            {
                labelIdCmd += "^FT" + label.XPos.ToString() + "," + label.YPos.ToString() + "^XG" + label.Id + ",1,1^FS";
                labelContentCmd += label.Text;
            }
            string content = labelContentCmd
                + "^XA^LH0,0^PR2,2^MD20^FO0,0"
                + labelIdCmd
                + "^PQ1,0,1,Y^XZ";
            RawPrinterHelper.SendStringToPrinter(printerName, content);
        }
        private void ZPLPrintLabels(string printerName, Project.Printers.Prints.Label[] labels, int height)
        {
            string labelIdCmd = string.Empty;
            string labelContentCmd = string.Empty;
            string headTitle = string.Empty;
            string barcodeNo = string.Empty;
            foreach (Project.Printers.Prints.Label label in labels)
            {
                labelIdCmd += "^FT" + label.XPos.ToString() + "," + label.YPos.ToString() + "^XG" + label.Id + ",1,1^FS";
                if (label.Id == "HName")
                {
                    //headTitle += "^FDMA," + label.Id + "^FS";
                }
                if (label.Id == "ANo")
                {
                    barcodeNo += "^FDMA,YF10069^FS";
                }
                labelContentCmd += TextToHex(label.Text, label.Id, height);
            }
            #region 打印具有格式的小票
            string content = labelContentCmd
                + "^XA^LH0,0^PR2,2^MD20^FO0,0"
                //+ headTitle
                + "^FO20,80^GB560,0,3^FS"
                + labelIdCmd
                + "^FT455,340^BQN,2,4"
                + barcodeNo
                + "^PQ1,0,1,Y^XZ";
            RawPrinterHelper.SendStringToPrinter(printerName, content);
            #endregion
        }
        #endregion
    }
}
