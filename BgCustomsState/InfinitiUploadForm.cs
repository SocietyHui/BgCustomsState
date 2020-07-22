using BLL;
using MODEL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BgCustomsState
{
    public partial class InfinitiUploadForm : Form
    {
        public HtmlDocument htmlDoc;
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer3 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer4 = new System.Windows.Forms.Timer();
        public List<InfinitiCarInfo> list = new List<InfinitiCarInfo>();
        CarInfoBll miBll = new CarInfoBll();
        InfinitiCarInfoBll InfinitiBll = new InfinitiCarInfoBll();
        InfinitiCarInfo infinitiCarInfo = new InfinitiCarInfo();


        String TextField_0 = "";//国外贸易商中文名称：
        String TextField_2 = "";//国外贸易商英文名称
        String eCnyNa_h = "";//国外贸易商注册地国别

        String TextField_1 = "";//国外生产商中文名称
        String TextField_3 = "";//国外生产商英文名称
        String eOrCnyNa_h = "";//国外生产商注册地国别

        //日本
        String pathNMLDOC = "";
        String pathNMLPDF = "";
        //美国
        String pathNNADOC = "";
        String pathNNAPDF = "";
        //英国
        String pathUKDOC = "";
        String pathUKPDF = "";

        String path1 = "";
        String path2 = "";

        string[] str = new string[2];
        public InfinitiUploadForm()
        {
            InitializeComponent();

            wb1 = webBrowser2.ActiveXInstance as SHDocVw.WebBrowser_V1;

            wb1.NewWindow += wb1_NewWindow;

            timer1.Interval = 3000;
            timer1.Enabled = true;
            timer1.Tick += new EventHandler(timer1EventProcessor);//添加事件
            timer1.Stop();

            timer2.Interval = 3000;
            timer2.Enabled = true;
            timer2.Tick += new EventHandler(timer2EventProcessor);//添加事件
            timer2.Stop();


            timer3.Interval = 3000;
            timer3.Enabled = true;
            timer3.Tick += new EventHandler(timer3EventProcessor);//添加事件
            timer3.Stop();

            timer4.Interval = 2000;
            timer4.Enabled = true;
            timer4.Tick += new EventHandler(timer4EventProcessor);//添加事件
            timer4.Stop();


            List<InfinitiCarInfo> cblist = InfinitiBll.GetList();
            comboBox1.DataSource = cblist;
            comboBox1.DisplayMember = "batch";
            comboBox1.ValueMember = "batch";
        }

        private void Button3_Click(object sender, EventArgs e)
        {

            this.webBrowser2.Url = new Uri("https://ecomp.mofcom.gov.cn/loginCorp.html");
            webBrowser2.ScriptErrorsSuppressed = true;
            //等待加载完毕
            while (webBrowser2.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();

            htmlDoc = this.webBrowser2.Document;
            if (htmlDoc.All["userName"] != null && htmlDoc.All["id_password"] != null)
            {
                htmlDoc.All["userName"].SetAttribute("value", "ic11005399");
                htmlDoc.All["id_password"].SetAttribute("value", "v7b4W7J91234");
            }



        }


        /// <summary>
        /// 开始写入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            htmlDoc = this.webBrowser2.Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("请确认是否已启动浏览器？");

                return;
            }


            HtmlElementCollection dhl = htmlDoc.GetElementsByTagName("A");
            foreach (HtmlElement item in dhl)
            {
                if (item.InnerText == "单证申请")
                {
                    item.InvokeMember("click");
                }

            }
            if (comboBox1.Text=="") {
                MessageBox.Show("请选择批次号！");
                return;
            }
            infinitiCarInfo.batch = comboBox1.Text;
            list = InfinitiBll.GetList(infinitiCarInfo);
            if (list.Count==0) {
                MessageBox.Show("查询无数据！");
                return;
            }
            bool_impot = true;
            boolfj = true;
            flag = 0;
            timer1.Start();
        }

        Boolean boolfj = true;
        Boolean bool_impot = true;
        Boolean first = false;
        int flag = 0;
        public void timer1EventProcessor(object source, EventArgs e)
        {
            timer1.Stop();
            while (webBrowser2.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
            htmlDoc = this.webBrowser2.Document;
            if (list.Count == 0)
            {
                MessageBox.Show("写入完成！");
                return;
            }
            foreach (HtmlElement he in htmlDoc.GetElementsByTagName("TH"))
            {


                if (he.InnerHtml == "单证申请")
                {
                    first = true;
                }

            }
            if (bool_impot)
            {
                if (!first)
                {
                    HtmlElementCollection dhl = htmlDoc.GetElementsByTagName("A");
                    foreach (HtmlElement item in dhl)
                    {
                        if (item.InnerText == "单证申请")
                        {
                            item.InvokeMember("click");
                        }

                    }
                    timer1.Start();
                    return;
                }
                if (htmlDoc.All["corpCde"] == null)
                {
                    webBrowser2.GoBack();
                    timer1.Start();
                    return;
                }
                String country = list[0].contract;
                String my1_h = "";//编码：
                String my2_h = "";//名称：
                if (country.Equals("日本"))
                {
                    my1_h = "JP";
                    my2_h = "US";

                    TextField_0 = "日产汽车公司";
                    TextField_2 = "Nissan Motor Co.,Ltd";
                    eCnyNa_h = "JP";

                    TextField_1 = "日产汽车公司";
                    TextField_3 = "Nissan Motor Co.,Ltd";
                    eOrCnyNa_h = "JP";

                    //日本对应文件
                    path1 =pathNMLPDF;
                    path2 = pathNMLDOC;
                }
                else if (country.Equals("美国"))
                {
                    my1_h = "US";
                    my2_h = "JP";
                    TextField_0 = "北美日产公司";
                    TextField_2 = "Nissan North America,INC";
                    eCnyNa_h = "US";

                    TextField_1 = "北美日产公司";
                    TextField_3 = "Nissan North America,INC";
                    eOrCnyNa_h = "US";

                    path1 = pathNNAPDF;
                    path2 = pathNNADOC;               
                }
                else if (country.Equals("英国"))
                {
                    my1_h = "CH";
                    my2_h = "GB";

                    TextField_0 = "日产（欧洲）汽车有限公司";
                    TextField_2 = "Nissan International SA";
                    eCnyNa_h = "CH";

                    TextField_1 = "日产汽车制造（英国）有限公司";
                    TextField_3 = "Nissan Motor Manufacturing (UK) Limited";
                    eOrCnyNa_h = "GB";

                    path1 = pathUKPDF;
                    path2 = pathUKDOC;
                }

                //进口商编码及名称
                htmlDoc.All["corpCde"].SetAttribute("value", "1100717850555");//编码：
                htmlDoc.All["corpCna"].SetAttribute("value", "日产（中国）投资有限公司");//名称：
                htmlDoc.All["tradeScCode"].SetAttribute("value", "91110000717850555P");//统一社会信用代码：

                //申请单位经办人
                htmlDoc.All["TextField_4"].SetAttribute("value", "王媛");//申请单位经办人：
                htmlDoc.All["TextField_5"].SetAttribute("value", "59251637");//经办人电话：
                htmlDoc.All["TextField_6"].SetAttribute("value", "13488871866");//手机号：

                //进口用户
                htmlDoc.All["corpTradeScCode"].SetAttribute("value", "91110000717850555P");//统一社会信用代码：
                htmlDoc.All["jkUser"].SetAttribute("value", "日产（中国）投资有限公司");//名称：

                //贸易方式
                HtmlElement el = htmlDoc.GetElementById("trdCde");
                foreach (HtmlElement item in el.Children)
                {
                    if (item.InnerText != null && item.InnerText.Trim() == "一般贸易")
                    {
                        item.SetAttribute("selected", "selected");
                        item.InvokeMember("onchange");
                        break;
                    }
                }

                //贸易国------------日本------------------------
                htmlDoc.All["my1_h"].SetAttribute("value", my1_h);
                htmlDoc.All["my1_h"].Focus();
                htmlDoc.All["my2_h"].SetAttribute("value", my2_h);
                htmlDoc.All["my2_h"].Focus();


                //外汇来源
                HtmlElement ell = htmlDoc.GetElementById("currOrCde");
                foreach (HtmlElement item in ell.Children)
                {
                    if (item.InnerText != null && item.InnerText.Trim() == "银行购汇")
                    {
                        item.SetAttribute("selected", "selected");
                        item.InvokeMember("onchange");
                        break;
                    }
                }

                //原产地国---------------------日本----------------------
                htmlDoc.All["yc1_h"].SetAttribute("value", my1_h);
                htmlDoc.All["yc1_h"].Focus();
                htmlDoc.All["yc2_h"].SetAttribute("value", my2_h);
                htmlDoc.All["yc2_h"].Focus();



                //报关口岸
                htmlDoc.All["bg1_h"].SetAttribute("value", "2200");
                htmlDoc.All["bg1_h"].Focus();
                //htmlDoc.All["bg1"].SetAttribute("value", "上海海关");
                htmlDoc.All["bg2_h"].SetAttribute("value", "5200");
                htmlDoc.All["bg2_h"].Focus();
                SendKeys.Send("{TAB}");
                //htmlDoc.All["bg2"].SetAttribute("value", "黄埔关区");

                //商品用途
                HtmlElement elll = htmlDoc.GetElementById("cdUseCde");
                foreach (HtmlElement item in elll.Children)
                {
                    if (item.InnerText != null && item.InnerText.Trim() == "直接销售")
                    {
                        item.SetAttribute("selected", "selected");
                        item.InvokeMember("onchange");
                        break;
                    }

                }

                //商品代码
                htmlDoc.All["businessCode"].SetAttribute("value", list[0].commodity_code);
                htmlDoc.All["businessCode"].Focus();

                //商品名称
                //htmlDoc.All["businessName"].SetAttribute("value", list[0].commodity_code);



                //设备状态
                htmlDoc.All["choose0"].InvokeMember("click");

                //货币
                HtmlElement elV = htmlDoc.GetElementById("currCde");
                foreach (HtmlElement item in elV.Children)
                {
                    if (item.InnerText != null && item.InnerText.Trim() == "美元")
                    {
                        item.SetAttribute("selected", "selected");
                        item.InvokeMember("onchange");
                        break;
                    }
                }


                HtmlElement table = htmlDoc.GetElementById("infoList");
                HtmlElement tbody = table.Children[2];
                int rows = tbody.Children.Count;
                HtmlElement tr = tbody.Children[0];
                int i = 0;
                foreach (HtmlElement item in tr.FirstChild.All)
                {
                    if (item.TagName == "INPUT")
                    {
                        i++;
                        if (i == 1)
                        {
                            item.SetAttribute("value", list[0].goods_no);
                        }
                        if (i == 2)
                        {
                            item.SetAttribute("value", list[0].quantity);
                        }
                        if (i == 3)
                        {
                            item.SetAttribute("value", list[0].unit_price);
                        }
                    }
                }


                //备注
                htmlDoc.All["oneMk1"].InvokeMember("click");//非一批一证

                //进口申请说明
                htmlDoc.All["TextArea"].SetAttribute("value", list[0].import_application);//极特殊原因，请在此栏进行文字说明，非企业进口说明，最多100字



                bool_impot = false;
                timer1.Start();
                return;
            }

            if (boolfj)
            {
                //上传附件
                htmlDoc.All["uploadAttachment"].InvokeMember("click");
                boolfj = false;
                flag = 1;
                timer1.Interval = 2000;
                timer1.Start();
                return;
            }
            if (flag == 1)
            {
                htmlDoc = this.webBrowser2.Document;
                HtmlElement file = htmlDoc.All["file"];
                if (file == null)
                {
                    timer1.Interval = 1000;
                    timer1.Start();
                    boolfj = true;
                    return;
                }
                if ("".Equals(path1.Trim()) || "".Equals(path2.Trim()))
                {
                    MessageBox.Show("未找到需要上传的文件，请检查！");
                    return;
                }
                //上传附件
                //htmlDoc.All["file"].SetAttribute("value", path1);
                SendKeys.Send(path1.Replace(".pdf", ".pdf"));//写入值     
                SendKeys.Send("{ENTER}");
                //Thread.Sleep(1000);
                htmlDoc.All["file"].InvokeMember("click");
                //htmlDoc.All["file"].Focus();//选中输入框
                //上传附件
                //Thread.Sleep(1000);
                htmlDoc.All["add"].InvokeMember("click");
                //htmlDoc.Window.Close();
                flag = 2;
                timer1.Interval = 1000;
                timer1.Start();
                return;
            }

            if (flag == 2)
            {

                htmlDoc = this.webBrowser2.Document;
                SendKeys.Send(path2.Replace(".doc", ".doc"));//写入值               
                SendKeys.Send("{ENTER}");
                //Thread.Sleep(1000);
                htmlDoc.All["file"].InvokeMember("click");
                //上传附件
                htmlDoc.All["add"].InvokeMember("click");
                //htmlDoc.Window.Close();
                flag = 3;
                timer1.Interval = 1000;
                timer1.Start();
                return;
            }

            if (flag == 3)
            {
                htmlDoc = this.webBrowser2.Document;              
                if (htmlDoc.All["inputForm"] == null)
                {
                    webBrowser2.GoBack();
                    timer1.Start();
                    return;
                }
                //申请表合同信息表
                htmlDoc.All["inputForm"].InvokeMember("click");
                timer2flag = true; ;
                timer1.Interval = 1000;
                timer2.Start();
            }





        }

        Boolean timer2flag = true;
        public void timer2EventProcessor(object source, EventArgs e)
        {
            timer1.Stop();
            timer2.Stop();
            htmlDoc = this.webBrowser2.Document;
            //------------[申请表合同信息表]弹出框----------------//
            if (timer2flag)
            {
                //合同号
                htmlDoc.All["TextField"].SetAttribute("value", list[0].contract_no);

                //合同签订日期
                htmlDoc.All["econtSignDte"].SetAttribute("value", list[0].contract_signing_date);


                //国外贸易商中文名称
                htmlDoc.All["TextField_0"].SetAttribute("value", TextField_0);

                //国外贸易商英文名称
                htmlDoc.All["TextField_2"].SetAttribute("value", TextField_2);

                //国外贸易商注册地国别
                htmlDoc.All["eCnyNa_h"].SetAttribute("value", eCnyNa_h);
                this.Activate();
                htmlDoc.All["eCnyNa_h"].Focus();
                //SendKeys.Send("{TAB}");

                //Thread.Sleep(2000);

                //国外生产商中文名称
                htmlDoc.All["TextField_1"].SetAttribute("value", TextField_1);

                //国外生产商英文名称
                htmlDoc.All["TextField_3"].SetAttribute("value", TextField_3);

                //国外生产商注册地国别
                htmlDoc.All["eOrCnyNa_h"].SetAttribute("value", eOrCnyNa_h);
                this.Activate();
                htmlDoc.All["eOrCnyNa_h"].Focus();
                SendKeys.Send("{TAB}");
                //Thread.Sleep(2000);
                //----------------------------------日本----END-----------------------------


                //合同总数量
                htmlDoc.All["econtQty"].SetAttribute("value", list[0].contract_total_num);

                //合同总金额
                htmlDoc.All["econtAmt"].SetAttribute("value", list[0].contract_amount);


                //结算币种
                HtmlElement eV = htmlDoc.GetElementById("ecoutCurr");
                foreach (HtmlElement item in eV.Children)
                {
                    if (item.InnerText != null && item.InnerText.Trim() == "美元")
                    {
                        item.SetAttribute("selected", "selected");
                        item.InvokeMember("onchange");
                        break;
                    }
                }

                //预计到港日期
                htmlDoc.All["eContPodDte"].SetAttribute("value", list[0].estimated_arrival_date);

                //品牌
                htmlDoc.All["brandName"].SetAttribute("value", "英菲尼迪");

                //型号
                htmlDoc.All["brandNum"].SetAttribute("value", list[0].specific_model);

                //具体规格型号描述
                htmlDoc.All["TextArea"].SetAttribute("value", list[0].specific_goods_no);

                timer2flag = false;
                timer2.Start();
                return;
            }

            //保存
            htmlDoc.All["LinkSubmit"].InvokeMember("click");
            //return;
            //SendKeys.Send("{ESC}");
            //webBrowser2.GoBack();
            //------------[申请表合同信息表]弹出框----END------------//
            timer3.Start();
            j = 0;
        }

        int j = 0;
        public void timer3EventProcessor(object source, EventArgs e)
        {

            timer1.Stop();
            timer3.Stop();
            if (j == 0)
            {

                SendKeys.Send("{ENTER}");
                j = 1;
                timer3.Start();
                return;
            }
            if (j == 1)
            {
                SendKeys.Send("{TAB}");
                SendKeys.Send("{ENTER}");
                j = 2;
                timer3.Start();
                return;
            }


            //页面跳转不需判断
            if (flag < 7)
            {
                htmlDoc = this.webBrowser2.Document;
                if (htmlDoc == null)
                {
                    webBrowser2.GoBack();
                    //webBrowser2.Refresh();
                    //timer3.Start();
                    flag = 5;
                    return;
                }

                if (htmlDoc.All["h_save"] == null)
                {

                    webBrowser2.GoBack();
                    //webBrowser2.Refresh();
                    timer3.Start();
                    flag = 5;
                    return;
                }
            }

            if (flag == 5)
            {
                //商品代码
                //htmlDoc.All["businessCode"].SetAttribute("value", list[0].commodity_code);
                this.Activate();
                htmlDoc.All["businessCode"].Focus();

                HtmlElement table = htmlDoc.GetElementById("infoList");
                HtmlElement tbody = table.Children[2];
                int rows = tbody.Children.Count;
                HtmlElement tr = tbody.Children[0];
                int i = 0;
                foreach (HtmlElement item in tr.FirstChild.All)
                {
                    if (item.TagName == "INPUT")
                    {
                        i++;
                        if (i == 1)
                        {
                            item.SetAttribute("value", list[0].goods_no);
                            item.Focus();
                        }
                        if (i == 2)
                        {
                            item.SetAttribute("value", list[0].quantity);
                            item.Focus();
                        }
                        if (i == 3)
                        {
                            item.SetAttribute("value", list[0].unit_price);
                            item.Focus();
                        }
                    }
                }
                //备注
                htmlDoc.All["oneMk1"].InvokeMember("click");//非一批一证

                //进口申请说明
                htmlDoc.All["TextArea"].SetAttribute("value", list[0].import_application);//极特殊原因，请在此栏进行文字说明，非企业进口说明，最多100字

                timer3.Start();
                flag = 6;
                return;
            }

            if (flag == 6)
            {

                HtmlElement table = htmlDoc.GetElementById("infoList");
                HtmlElement tbody = table.Children[2];
                int rows = tbody.Children.Count;
                HtmlElement tr = tbody.Children[0];
                int i = 0;
                foreach (HtmlElement item in tr.FirstChild.All)
                {
                    if (item.TagName == "INPUT")
                    {
                        i++;
                        if (i == 1)
                        {
                            item.SetAttribute("value", list[0].goods_no);
                            item.Focus();
                        }
                        if (i == 2)
                        {
                            item.SetAttribute("value", list[0].quantity);
                            item.Focus();
                        }
                        if (i == 3)
                        {
                            item.SetAttribute("value", list[0].unit_price);
                            item.Focus();
                            SendKeys.Send("{TAB}");
                        }
                    }
                }

                timer3.Start();
                flag = 7;
                return;
            }

            //跳转新页面
            if (flag == 7)
            {
                htmlDoc.All["TextArea"].Focus();
                //暂存
                HtmlElementCollection saveInput = htmlDoc.GetElementsByTagName("INPUT");
                foreach (HtmlElement item in saveInput)
                {
                    if (item.GetAttribute("value") == "暂存")
                    {

                        item.InvokeMember("click");
                    }
                }
                timer3.Start();
                flag = 8;
                return;
            }

            timer4.Start();
        }

        public void timer4EventProcessor(object source, EventArgs e)
        {
            timer4.Stop();
            htmlDoc = this.webBrowser2.Document;
            HtmlElementCollection dhl = htmlDoc.GetElementsByTagName("H2");
            foreach (HtmlElement item in dhl)
            {
                if (item.InnerText.Contains("您已成功提交申请"))
                {
                    String text =   System.Text.RegularExpressions.Regex.Replace(item.InnerText, @"[^0-9]+", "");
                    list[0].serial_no = text;
                }

            }
           
            InfinitiBll.UpdateCarInfo(list[0]);
            webBrowser2.Navigate("http://jdlicenceapp.ec.com.cn/jdlic/pages/lic/JdDzTabEdit,Sadd.html");
            list.Remove(list[0]);
            bool_impot = true;
            boolfj = true;
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            flag = 0;
            timer1.Start();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button4_Click(object sender, EventArgs e)
        {

        }

        private void Button8_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
            folderBrowserDialog1.Description = "请选择文件夹";
            DialogResult d = folderBrowserDialog1.ShowDialog();
            if (d == DialogResult.OK)
            {
                DirectoryInfo directory = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                String msg = "";
                if (directory.Exists)
                {
                    foreach (FileInfo info in directory.GetFiles())
                    {
                        #region 获取 日本文件
                        if (info.FullName.Contains(".pdf") && info.FullName.Contains("NML"))
                        {
                            pathNMLDOC = folderBrowserDialog1.SelectedPath+"\\"+ info.Name.ToString();
                            String message = this.checkPath(pathNMLDOC);
                            if (message != null)
                            {
                                pathNMLDOC = "";
                                MessageBox.Show("需要上传的pdf文件名称不能包含特殊符号"+ message + ",会影响上传操作！");
                                return;
                            }

                        }
                        if (info.FullName.Contains(".doc") && info.FullName.Contains("NML"))
                        {
                            pathNMLPDF = folderBrowserDialog1.SelectedPath + "\\" + info.Name.ToString();
                            String message = this.checkPath(pathNMLPDF);
                            if (message != null)
                            {
                                pathNMLPDF = "";
                                MessageBox.Show("需要上传的doc文件名称不能包含特殊符号" + message + ",会影响上传操作！");
                                return;
                            }                            
                        }
                        #endregion
                        #region 获取 美国文件
                        if (info.FullName.Contains(".pdf") && info.FullName.Contains("NNA"))
                        {
                            pathNNAPDF = folderBrowserDialog1.SelectedPath + "\\" + info.Name.ToString();
                            String message = this.checkPath(pathNNAPDF);
                            if (message != null)
                            {
                                pathNNAPDF = "";
                                MessageBox.Show("需要上传的pdf文件名称不能包含特殊符号" + message + ",会影响上传操作！");
                                return;
                            }

                        }
                        if (info.FullName.Contains(".doc") && info.FullName.Contains("NNA"))
                        {
                            pathNNADOC = folderBrowserDialog1.SelectedPath + "\\" + info.Name.ToString();
                            String message = this.checkPath(pathNNADOC);
                            if (message != null)
                            {
                                pathNNADOC = "";
                                MessageBox.Show("需要上传的doc文件名称不能包含特殊符号" + message + ",会影响上传操作！");
                                return;
                            }
                        }
                        #endregion
                        #region 获取 英国文件
                        if (info.FullName.Contains(".pdf") && info.FullName.Contains("UK"))
                        {
                            pathUKPDF = folderBrowserDialog1.SelectedPath + "\\" + info.Name.ToString();
                            String message = this.checkPath(pathUKPDF);
                            if (message != null)
                            {
                                pathUKPDF = "";
                                MessageBox.Show("需要上传的pdf文件名称不能包含特殊符号" + message + ",会影响上传操作！");
                                return;
                            }

                        }
                        if (info.FullName.Contains(".doc") && info.FullName.Contains("UK"))
                        {
                            pathUKDOC = folderBrowserDialog1.SelectedPath + "\\" + info.Name.ToString();
                            String message = this.checkPath(pathUKDOC);
                            if (message != null)
                            {
                                pathUKDOC = "";
                                MessageBox.Show("需要上传的doc文件名称不能包含特殊符号" + message + ",会影响上传操作！");
                                return;
                            }
                        }
                        #endregion
                    }
                }
                //txtFilesName指的是界面一个文本框获取路径
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
            else MessageBox.Show("请选择目录！");
        }


        private String checkPath(String path)
        {
            String messge = null;
            if (path.Contains("(") || path.Contains("（"))
            {
                messge = "(";
            }
            if (path.Contains(")") || path.Contains("）"))
            {
                messge += ")";
            }
            if (path.Contains("{"))
            {
                messge = "{";
            }
            if (path.Contains("}"))
            {
                messge += "}";
            }
            return messge;
        }


        SHDocVw.WebBrowser_V1 wb1;

        private void WebBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            foreach (HtmlElement links in this.webBrowser2.Document.Links)
            {
                links.SetAttribute("target", "_self");
            }
            foreach (HtmlElement form in this.webBrowser2.Document.Forms)
            {
                form.SetAttribute("target", "_self");
            }
        }

        private void WebBrowser2_NewWindow(object sender, CancelEventArgs e)
        {

            //WebBrowser web = (WebBrowser)sender;
            //string strPageUrl = web.StatusText;
            //CreatePages(strPageUrl, web.DocumentTitle); //创建新的webbrowser(不用ie弹出)
            try
            {
                Console.WriteLine(webBrowser2.StatusText);
                string url = this.webBrowser2.Document.ActiveElement.GetAttribute("href");

                if (!url.Equals(""))
                {
                    e.Cancel = true;
                    this.webBrowser2.Url = new Uri(url);
                }
            }
            catch
            {
            }



        }

        void wb1_NewWindow(string URL, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Processed)

        {

            try

            {

                Processed = true;//设置为依据处理
               
                webBrowser2.Navigate(URL);//在当前的 浏览器控件中打开 

            }

            catch (Exception)

            {

            }

        }

        private void Button7_Click(object sender, EventArgs e)
        {
            //bool_impot = true;
            //boolfj = true;
            //webBrowser2.GoBack();
            //timer2flag = true; ;
            //timer2.Start();
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            timer4.Stop();

            webBrowser2.Navigate("http://jdlicenceapp.ec.com.cn/jdlic/pages/lic/JdDzTabEdit,Sadd.html");
        }
    }
}
