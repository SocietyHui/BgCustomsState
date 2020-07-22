using BLL;
using MODEL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using mshtml;
using Common;
using Microsoft.Win32;
using System.Diagnostics;
using System.Collections;

namespace BgCustomsState
{
    public partial class OriginForm : Form
    {
        public HtmlDocument htmlDoc;
        public List<SingleInfo> list = new List<SingleInfo>();
        public List<SingleInfo> detailIdlist;
        public List<SingleInfo> vinlist;
        public List<SingleInfo> vinlistCopy;
        public List<SingleInfo> getVinList = new List<SingleInfo>();
        public List<SingleInfo> UpdateCerNoList = new List<SingleInfo>();
        List<SingleInfo> notList = new List<SingleInfo>();
       String messageShow= "数据校验完成！\n";
       
        public SingleInfo singItem;

        public int controlStep = 0;
        public HtmlElement tr = null;
        public int detailnum = 0;

        DateTime startTime;

        //数据导入
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer3 = new System.Windows.Forms.Timer();

       

       
        System.Windows.Forms.Timer timer5 = null;
        System.Windows.Forms.Timer timer6 = null;
        System.Windows.Forms.Timer timer7 = null;

      
        System.Windows.Forms.Timer timer8 = null;
        System.Windows.Forms.Timer timer9 = null;
        System.Windows.Forms.Timer timer10 = null;
        public int number1 = 0;
        public int number2 = 0;
        public int number3 = 0;
        public int number4 = 0;
        public int number5 = 0;

        public int decCount = 0;//原产地证项数总计
        public int getVinCount = 0;
        public int getDetailCount = 0;
        public int seqno;//当前项
        public int curVin = 0;//当前证明书第几辆
        public int flag = 0;
        public int timer2Flash = 1000;
        public int timer3Flash = 1000;
        bool repeatFlag = false;
        Hashtable ht_dfNo = new Hashtable();

        public String dfNoStr = "";
        SingleInfo carInfo = new SingleInfo();
        public String model = "";

        public String brand = "";
        //是否查询到数据
        public bool bl = true;
        //创建业务逻辑层对象
        OriginBill miBll = new OriginBill();

        
        public OriginForm()
        {
            InitializeComponent();
            //webBrowser3.BeforeNewWindow2 += WebBrowser3_BeforeNewWindow2;
            //禁用按钮
            setButtonEnable(true);
        }

        private void WebBrowser3_BeforeNewWindow2(IED.WebBrowserUrl2 webPra, IED.WebBrowserEvent cancel)
        {
            if (webPra.Url!=null)
            {
                this.webBrowser3.Navigate(webPra.Url);
                cancel.cancel = true;
            }
        }


        /// <summary>
        /// 初始化登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OriginForm_Load(object sender, EventArgs e)
        {
            //設置當前文檔為百度
            //webBrowser2.ScriptErrorsSuppressed = true;
            List<SingleInfo> cblist = miBll.GetList();
            comboBox1.DataSource = cblist;
            comboBox1.DisplayMember = "Batch";
            comboBox1.ValueMember = "Batch";

            IList<Info> infoList = new List<Info>();
            Info info1 = new Info() { Id = "10", Name = "暂不处理" };
            Info info2 = new Info() { Id = "0", Name = "待写入" };
            Info info3 = new Info() { Id = "7", Name = "写入完成" };
            //Info info4 = new Info() { Id = "8", Name = "抓取完成" };
            Info info5 = new Info() { Id = "9", Name = "提交完成" };
            infoList.Add(info1);
            infoList.Add(info2);
            infoList.Add(info3);
            //infoList.Add(info4);
            infoList.Add(info5);
            comboBox2.DataSource = infoList;
            comboBox2.ValueMember = "Id";
            comboBox2.DisplayMember = "Name";


            timer1.Interval = 2000;
            timer1.Enabled = true;
            timer1.Tick += new EventHandler(timer1EventProcessor);//添加事件
            timer1.Stop();

            timer2.Interval = timer2Flash;
            timer2.Enabled = true;
            timer2.Tick += new EventHandler(timer2EventProcessor);//添加事件
            timer2.Stop();


            timer3.Interval = timer3Flash;
            timer3.Enabled = true;
            timer3.Tick += new EventHandler(timer3EventProcessor);//添加事件
            timer3.Stop();

        }

        //计数器
        int num = 0;

        public class Info
        {
            public string Id { get; set; }
            public string Name { get; set; }

        }





        /// <summary>
        /// 点击进入查询界面 开启timer
        /// </summary>
        /// <param name="carInfo"></param>
        private void getOrigionNo()
        {
            //等待加载完毕
            while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();

            htmlDoc = this.webBrowser3.Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("请确认是否已启动浏览器？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }         
         

            htmlDoc = webBrowser3.Document;
            Application.DoEvents();

            htmlDoc = webBrowser3.Document.Window.Frames[0].Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("没有获取到菜单项，请确认是否已登录？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }

            String batch = comboBox1.Text;

            if (batch == "")
            {
                MessageBox.Show("请输入批次号！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            carInfo.batch = batch;
            carInfo.certificate_origin_no = "";
            if (dataGridView1.SelectedRows.Count > 0)
            {
                carInfo.certificate_origin_no = dataGridView1.SelectedCells[1].Value.ToString();
            }

            //抓取
            if (flag == 2)
            {
                carInfo.single_state = "8";
            }


            #region //勾选处理
            dfNoStr = "";
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if ((Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true))
                {
                    dfNoStr += "'" + dataGridView1.Rows[i].Cells[1].Value.ToString() + "',";
                }
                else
                    continue;
            }
            //有勾选项
            if (!dfNoStr.Equals(""))
            {
                dfNoStr = dfNoStr.Substring(0, dfNoStr.Length - 1);
                SingleInfo carinfo1 = new SingleInfo();
                carinfo1.batch = batch;
                carinfo1.certificate_origin_no = dfNoStr;
                carinfo1.single_state = carInfo.single_state;
                list = miBll.selectChecked(carinfo1);
            }
            else
            {
                list = miBll.GetList(carInfo);

            }
            #endregion
            if (list.Count == 0)
            {
                MessageBox.Show("该批次查询暂无数据！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            else
            {
                if (flag == 1 && MessageBox.Show("当前将会帮您自动上传" + list.Count + "票原产地证的原产地证明书数据,请确认无误后继续上传！", "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }

            }


            timer1.Interval = 2000;
            timer1.Start();
        }


        private void getOriginDetail()
        {
            //禁用按钮
            setButtonEnable(false);
            htmlDoc = this.webBrowser3.Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("请确认是否已启动浏览器？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            String batch = comboBox1.Text;
           
            carInfo.batch = batch;

            //抓取
            carInfo.single_state = "8";

            #region //勾选处理
            dfNoStr = "";
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if ((Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true)
                    && (Convert.ToString(dataGridView1.Rows[i].Cells[2].Value) == "7"))
                {
                    dfNoStr += "'" + dataGridView1.Rows[i].Cells[1].Value.ToString() + "',";
                }
                else
                    continue;
            }
            //有勾选项
            if (!dfNoStr.Equals(""))
            {
                dfNoStr = dfNoStr.Substring(0, dfNoStr.Length - 1);
                SingleInfo carinfo1 = new SingleInfo();
                carinfo1.batch = batch;
                carinfo1.certificate_origin_no = dfNoStr;
                carinfo1.single_state = carInfo.single_state;
                list = miBll.selectChecked(carinfo1);
            }
            else
            {
                list = miBll.GetList(carInfo);
            }
            #endregion
            if (list.Count == 0)
            {
                MessageBox.Show("该批次查询暂无数据！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            timer1.Start();
        }

        //查询条件设置
        private void inputSearchParam()
        {
            while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
            Thread.Sleep(600);
            detailIdlist = miBll.GetListByDfno(list[num]);
            SingleInfo detailInfo = detailIdlist[0];
            htmlDoc = webBrowser3.Document.Window.Frames[1].Document;
            //查询条件         
            Application.DoEvents();
            SetText("CERT_NO", "value", detailInfo.certificate_origin_no.ToString(),htmlDoc);
            Application.DoEvents();
            htmlDoc.GetElementById("searchBtn").InvokeMember("click");
            Application.DoEvents();           
        }

        /// <summary>
        /// 查询所需数据事件
        /// </summary>
        /// <param name="carInfo"></param>
        private void tempSave()
        {
            //禁用按钮
            setButtonEnable(false);
            htmlDoc = this.webBrowser3.Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("请确认是否已启动浏览器？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }            
            while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();

            Thread.Sleep(600);   
            String batch = comboBox1.Text;

            if (batch == "")
            {
                MessageBox.Show("请输入批次号！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            carInfo.batch = batch;
          
            if (flag == 1) //写入
            {
                carInfo.single_state = "7";
            }  
            else if (flag == 2) //抓取
            {
                carInfo.single_state = "8";
            }
            else if (flag == 3)//删除证明书
            {
                carInfo.single_state = "9";
            }
            else if (flag == 4) //提交
            {
                carInfo.single_state = "9";
            }

            #region //勾选处理
            dfNoStr = "";
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if ((Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true)
                    && (Convert.ToString(dataGridView1.Rows[i].Cells[2].Value)!="10"))
                {
                    dfNoStr += "'" + dataGridView1.Rows[i].Cells[1].Value.ToString() + "',";
                }
                else
                    continue;
            }
            //有勾选项
            if (!dfNoStr.Equals(""))
            {
                dfNoStr = dfNoStr.Substring(0, dfNoStr.Length - 1);
                SingleInfo carinfo1 = new SingleInfo();
                carinfo1.batch = batch;
                carinfo1.certificate_origin_no = dfNoStr;
                carinfo1.single_state = carInfo.single_state;
                list = miBll.selectChecked(carinfo1);
            }
            else
            {
                list = miBll.GetList(carInfo);
            }
            #endregion
            if (list.Count == 0)
            {
                MessageBox.Show("该批次查询暂无数据！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            else
            {
                if (flag == 1 && MessageBox.Show("当前将会帮您自动上传" + list.Count + "票原产地证的原产地证明书数据,请确认无误后继续上传！", "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }

            }
            timer1.Start();
        }

        bool isloading = true;
        /// <summary>
        /// 页面填充数据事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void timer1EventProcessor(object source, EventArgs e)
        {
            timer1.Stop();
            if (flag == 1)//写入数据
            {
                #region 写入数据
                while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
                if (num == list.Count)
                {
                    DateTime d2 = new DateTime();
                    TimeSpan d3 = d2.Subtract(startTime);
                    MessageBox.Show("数据写入完成！用时" + d3.Minutes.ToString() + " 分钟");
                    num = 0;
                    setButtonEnable(true);
                    search();
                    this.webBrowser3.Refresh();
                    return;
                }

                HtmlElementCollection dhl = htmlDoc.GetElementsByTagName("A");
                if (dhl.Count > 7)
                {
                    foreach (HtmlElement item in dhl)
                    {
                        if (item.InnerText == "原产地证据文件录入")
                        {
                            item.InvokeMember("click");
                            break;
                        }

                    }
                    timer2.Interval = 2000;
                    timer2.Start();
                }
                #endregion
            }
            else if (flag == 2)//查询数据
            {
                #region 查询数据  新网站暂时不用
                htmlDoc = webBrowser3.Document.Window.Frames[1].Document;
                while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
                if (num == list.Count)
                {
                    MessageBox.Show(messageShow);
                    num = 0;
                    foreach (var item in notList)
                    {
                        item.single_state = "0";
                        miBll.UpdateHeadState(item);
                    }
                    notList = new List<SingleInfo>();
                    messageShow = "数据校验完成！\n";
                    setButtonEnable(true);
                    search();
                    webBrowser3.Refresh();
                    return;
                }
                inputSearchParam();
                #endregion
            }
            else if (flag == 3)//抓取数据
            {
                #region 数据抓取
                while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
                if (num == list.Count)
                {
                    MessageBox.Show("数据抓取完成！");
                    num = 0;
                    setButtonEnable(true);
                    search();
                    webBrowser3.Refresh();
                    return;
                }
                HtmlElementCollection dhl = htmlDoc.GetElementsByTagName("A");
                if (dhl.Count > 7)
                {
                    foreach (HtmlElement item in dhl)
                    {
                        if (item.InnerText == "原产地证据文件查询")
                        {
                            item.InvokeMember("click");
                            break;
                        }

                    }
                    timer2.Interval = 2000;
                    timer2.Start();
                }
                #endregion
            }       
        }

        /// <summary>
        /// 数据写入成功事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void timer2EventProcessor(object source, EventArgs e)
        {
            try
            {
                timer2.Stop();
                //等待加载完毕
                while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
                if (flag == 1)//写入原产地证
                {
                    #region 数据写入   
                    AddCarinfo();
                    timer3.Interval = 1000;
                    timer3.Start();
                    #endregion
                }
                else if (flag == 2)//查询原产地证
                {
                    #region 数据查询 暂未使用
                    Application.DoEvents();
                    bool b = true;
                    foreach (HtmlElement item in htmlDoc.All)
                    {
                        if (item.Id != null && item.Id.Contains("mini-14$row"))
                        {
                            String ss = item.Id;
                            b = false;
                        }
                    }
                    Application.DoEvents();
                    if (b)
                    {
                        messageShow += list[num].certificate_origin_no + " 未录入申报系统！\n";
                        notList.Add(list[num]);
                    }
                    num++;
                    timer1.Start();
                    #endregion
                }
                else if (flag == 3)//抓取原产地证
                {
                    #region 数据抓取
                    while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();                    
                    if (bl)
                    {
                        //输入报关单号页面查询事件
                        inputSearchParam();
                        bl = false;
                        timer2.Start();
                    }
                    else
                    {
                        //点击明细
                        bool has = ClickItem();
                        if (has)
                        {                            
                            bl = true;                           
                            //等待加载完毕
                            while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
                            timer3.Interval = 2000;
                            timer3.Start();
                            return;
                        }
                        else
                        {
                            //当查询统一编号页面未及时加载出来 再次延迟timer2 查询加载
                            bl = true;
                            timer2.Interval = 8000;
                            timer2.Start();
                            return;
                        }
                    }                  
                    #endregion
                }                
            }
            catch (Exception ex)
            {
                timer2.Stop();
                if (MessageBox.Show("当前程序点击当前项出现异常，是否继续！" + ex.Message.ToString(), "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }
                timer1.Start();
            }
        }

        //点击查询结果
        private bool ClickItem()
        {
            htmlDoc = webBrowser3.Document.Window.Frames[1].Document;
            HtmlElement table = htmlDoc.GetElementById("nocListInfoTalbe");
            HtmlElement tbody = table.Children[1];
            int rows = tbody.Children.Count;
            if (rows != 1)
            {
                MessageBox.Show("该票原产地包含项数与系统数据不一致！单一窗口数量：" + rows + "系统查询数量" + 1);
                return false;
            }
            HtmlElement tr = tbody.Children[0];
            tr.Children[0].InvokeMember("click");
            while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();

            htmlDoc.GetElementById("showDetailBtn").InvokeMember("click");
            return true;
        }

        int delFlag = 0;
        int count = 0;
        int clickCount = 1;
        bool fisrt = true;
        #region //timer3 抓取 删除 提交
        /// <summary>
        /// timer3
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        /// 暂时未用到
        public void timer3EventProcessor(object source, EventArgs e)
        {
            timer3.Stop();
            while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
            try
            {               
                if (flag==1)
                {
                    #region 数据写入 写入第二部分项号数据
                    this.addCarInfoDetail();
                    CloseAddCarInfoPage();

                    timer1.Interval = 1000;
                    timer1.Start();
                    #endregion
                }
                if (flag == 3)
                {
                    Console.Write("当前第："+num);
                    #region 数据抓取
                    htmlDoc = webBrowser3.Document.Window.Frames[2].Document;
                    if (fisrt)
                    {
                        //第一次计算出多少页
                        detailIdlist = miBll.GetListDetail(list[num]);
                        count = detailIdlist.Count / 5;
                        if ((detailIdlist.Count % 5) > 0)
                        {
                            count++;
                        }                        
                        this.GetCarinfo();
                        fisrt = false;
                        timer3.Interval = 1000;
                        timer3.Start();
                    }
                    else
                    {
                        if (clickCount==count)
                        {
                            //最后一页抓完结束                            
                            delFlag = 0;
                            count = 0;
                            clickCount = 1;
                            fisrt = true;
                            num++;
                            this.CloseSeachCarInfoPageDetail();
                            this.CloseSeachCarInfoPage();
                            timer1.Interval = 1000;
                            timer1.Start();
                        }
                        else
                        {                          
                            HtmlElement decFrom = htmlDoc.GetElementById("nocListInfoTalbe");
                            HtmlElement doc = decFrom.Children[1];                            
                            while (webBrowser3.ReadyState < WebBrowserReadyState.Complete)
                                Application.DoEvents();                         
                            HtmlElementCollection pagination = htmlDoc.GetElementsByTagName("ul");
                            HtmlElement next = pagination[1].Children[7];
                            Thread.Sleep(600);
                            next.InvokeMember("click");
                            Application.DoEvents();
                            this.GetCarinfo();
                            clickCount++;
                            timer3.Interval = 1000;                           
                            timer3.Start();                         
                        }
                    }                    
                    #endregion
                }                    
            }
            catch (Exception ex)
            {
                delFlag = 0;
                count = 0;
                clickCount = 0;
                fisrt = true;
                if (MessageBox.Show("当前程序出现异常，是否继续！" + ex.Message.ToString(), "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    //解禁按钮
                    setButtonEnable(true);
                    this.stopTimers();
                    return;
                }
                timer1.Start();                
            }
        }
        #endregion




        #region //明细点击事件

        /// <summary>
        /// 商品列表item点击事件
        /// </summary>
        /// <param name="num"></param>
        /// <param name="decNum"></param>
        private int tableItemClick(int seq_no, int decNum)
        {
            number4++;

            htmlDoc = webBrowser3.Document.Window.Frames["mainFrame"].Document;

            HtmlElement table = htmlDoc.GetElementById("ID_entryData");
            if (table == null)
            {
                MessageBox.Show("没有获取到原产地证详情列表！");
                //解禁按钮
                setButtonEnable(true);
                return 1;
            }
            HtmlElement tbody = table.Children[1];

            int rows = tbody.Children.Count;

            tr = tbody.Children[seq_no - 1];
            if (tr.Children.Count < 2)
            {
                SendKeys.Send("{enter}");
                SendKeys.Send("{ESC}");
                return 3;
            }
            if (rows != getDetailCount)
            {
                MessageBox.Show("该票原产地证包含项数与单一窗口项数不一致！单一窗口数量：" + rows + "系统查询数量" + getDetailCount);
                return 2;
            }

            int seqNo = Convert.ToInt32(tr.Children[0].InnerText.ToString());
            decCount = Convert.ToInt32(tr.Children[5].InnerText.ToString());
            brand = tr.Children[2].InnerText.ToString();//获取品名
            model = tr.Children[3].InnerText.ToString();//获取申报要素
            if (seqNo != seq_no)
            {
                MessageBox.Show("该票原产地证商品项号与单一窗口商品项号不一致！单一窗口数量：" + seqNo + "系统查询数量" + seq_no);
                timer1.Stop();
                return 2;
            }
            if (decCount != decNum)
            {
                MessageBox.Show("该票原产地证申报数量与单一窗口申报数量不一致！单一窗口数量：" + decCount + "系统查询数量" + decNum);
                timer1.Stop();
                return 2;
            }
            SendKeys.Send("{enter}");
            SendKeys.Send("{enter}");
            //tr.InvokeMember("click");
            System.Diagnostics.Debug.WriteLine(DateTime.Now + "     操作一  tableItemClick事件执行第---" + number4 + "---次------------该项中未导入原产地剩余数量--" + vinlist.Count);
            return 0;
        }
        #endregion



        #region //导入原产地主表数据
        /// <summary>
        /// 自动录入证明书信息
        /// </summary>
        /// <param name="num"></param>
        /// <param name="decNum"></param>
        private void AddCarinfo()
        {
            while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
            Thread.Sleep(600);
            detailIdlist = miBll.GetListDetail(list[num]);
            SingleInfo detailInfo = detailIdlist[0];
            htmlDoc = webBrowser3.Document.Window.Frames[1].Document;

            //-----------------第一部分数据-------------------
            //原产地证号
            SetText("CERT_NO", "value", detailInfo.certificate_origin_no.ToString(),htmlDoc);
            while (webBrowser3.ReadyState < WebBrowserReadyState.Complete)Application.DoEvents();
            //优惠贸易协定代码
            ExecJS(htmlDoc, " function setAGREEMENT_ID(){  $('#agreementIdCode').attr('value', '02');$('#AGREEMENT_ID').attr('data-id', '02');$('#AGREEMENT_ID').attr('alt', '中国-东盟');$('#AGREEMENT_ID').val('中国-东盟'); }", "setAGREEMENT_ID");
            //原产国
            ExecJS(htmlDoc, " function setORIGIN_COUNTRY(){  $('#originCountryCode').attr('value', '136');$('#ORIGIN_COUNTRY').attr('data-id', '136');$('#ORIGIN_COUNTRY').attr('alt', '泰国');$('#ORIGIN_COUNTRY').val('泰国'); }", "setORIGIN_COUNTRY");
            //签证日期
            ExecJS(htmlDoc, "function setISSUE_DATE(){ $(\"#ISSUE_DATE\").val(\""+ DateTime.Parse(detailInfo.sign_date).ToString("yyyy-MM-dd") + "\"); }", "setISSUE_DATE");       
           
            //第三部分数据      
            while (webBrowser3.ReadyState < WebBrowserReadyState.Complete)
                Application.DoEvents();
            SetText("IS_TRANSFER_RADIO_N", "checked", "true", htmlDoc);
            SetText("IS_TRANSPORT_DOC_RADIO_Y", "checked", "true", htmlDoc);
            SetText("TRANSPORT_DOC_NO_TEXT", "value", detailInfo.bill_no, htmlDoc);
            Application.DoEvents();
            //ExecJS(htmlDoc, "function alert(){ } ", "alert");
            htmlDoc.GetElementById("saveBtn").InvokeMember("click");
            Application.DoEvents();            
        }
        #endregion

        #region //导入原产地子表数据
        /// <summary>
        /// 写入第二部分项号数据
        /// </summary>
        private void addCarInfoDetail()
        {
            for (int i = 0; i < detailIdlist.Count; i++)
            {
                while (webBrowser3.ReadyState < WebBrowserReadyState.Complete)
                    Application.DoEvents();
                SetText("NO", "value", (i + 1).ToString(), htmlDoc);
                SetText("CODE_TS", "value", detailIdlist[i].hs_code.ToString(), htmlDoc);
                SetText("QTY", "value", detailIdlist[i].single_count.ToString(), htmlDoc);
                ExecJS(htmlDoc, "function setUNIT(){ $('#unitCode').attr('value', '003');$('#UNIT').attr('data-id', '003');$('#UNIT').attr('alt', '辆');$('#UNIT').val('辆'); }", "setUNIT");
                SetText("ORIGIN_CRITERION", "value", detailIdlist[i].origin_standard.ToString(), htmlDoc);
                while (webBrowser3.ReadyState < WebBrowserReadyState.Complete)
                    Application.DoEvents();
                htmlDoc.GetElementById("listInfoAddBtn").InvokeMember("click");
            }

            htmlDoc = webBrowser3.Document.Window.Frames[1].Document;
            list[num].single_seq_no = htmlDoc.GetElementById("SEQ_NO").GetAttribute("value");
            list[num].single_state = "7";
            miBll.UpdateHeadStateAndNo(list[num]);
            num++;
        }
        #endregion

        //关闭重新写入
        public void CloseAddCarInfoPage()
        {
            //点击关闭
            htmlDoc = webBrowser3.Document;
            HtmlElement layer = htmlDoc.GetElementById("page-wrapper");
            if (layer != null)
            {
                HtmlElementCollection div = layer.GetElementsByTagName("A");


                foreach (HtmlElement item in div)
                {
                    if (item.InnerText != null && item.InnerText.ToString().Trim() == "原产地证据文件录入")
                    {

                        HtmlElementCollection dhl = item.GetElementsByTagName("i");
                        foreach (HtmlElement a in dhl)
                        {
                            if (a.InnerText == null)
                            {
                                a.InvokeMember("click");
                                break;
                            }

                        }
                        break;
                    }

                }
            }
        }

        public void CloseSeachCarInfoPage()
        {
            //点击关闭
            htmlDoc = webBrowser3.Document;
            HtmlElement layer = htmlDoc.GetElementById("page-wrapper");
            if (layer != null)
            {
                HtmlElementCollection div = layer.GetElementsByTagName("A");


                foreach (HtmlElement item in div)
                {
                    if (item.InnerText != null && item.InnerText.ToString().Trim() == "原产地证据文件查询")
                    {
                        HtmlElementCollection dhl = item.GetElementsByTagName("i");
                        foreach (HtmlElement a in dhl)
                        {
                            if (a.InnerText == null)
                            {
                                a.InvokeMember("click");
                                break;
                            }

                        }
                        break;
                    }

                }
            }
        }

        public void CloseSeachCarInfoPageDetail()
        {
            //点击关闭
            htmlDoc = webBrowser3.Document;
            HtmlElement layer = htmlDoc.GetElementById("page-wrapper");
            if (layer != null)
            {
                HtmlElementCollection div = layer.GetElementsByTagName("A");
                foreach (HtmlElement item in div)
                {
                    if (item.InnerText != null && item.InnerText.ToString().Trim() == "原产地证据文件明细")
                    {
                        //原产地证据文件明细 
                        HtmlElementCollection dhl = item.GetElementsByTagName("i");
                        foreach (HtmlElement a in dhl)
                        {
                            if (a.InnerText == null)
                            {
                                a.InvokeMember("click");
                                break;
                            }

                        }
                        break;
                    }

                }
            }
        }

        #region //抓取原产证表数据
        /// <summary>
        /// 抓取原产地证数据
        /// </summary>
        private void GetCarinfo()
        {
            HtmlElement decFrom = htmlDoc.GetElementById("nocListInfoTalbe");
            HtmlElement doc = decFrom.Children[1];
            HtmlElementCollection pagination = htmlDoc.GetElementsByTagName("ul");
            if (doc.Children.Count > 0)
            {
                foreach (HtmlElement item in doc.Children)
                {
                    list[num].single_seq_no= htmlDoc.GetElementById("SEQ_NO").GetAttribute("value");
                    //原产地证号
                    list[num].CERT_NO = htmlDoc.GetElementById("CERT_NO").GetAttribute("value");
                    //优惠贸易协定代码
                    list[num].AGREEMENT_ID = htmlDoc.GetElementById("AGREEMENT_ID").GetAttribute("value");
                    //原产国
                    list[num].ORIGIN_COUNTRY = htmlDoc.GetElementById("ORIGIN_COUNTRY").GetAttribute("value");
                    //签证日期
                    list[num].ISSUE_DATE = htmlDoc.GetElementById("ISSUE_DATE").GetAttribute("value");

                    //是否经过非协定成员方中转
                    list[num].IS_TRANSFER_RADIO = htmlDoc.GetElementById("IS_TRANSFER_RADIO").GetAttribute("value");
                    //是否有单份全程运输单证
                    list[num].IS_TRANSPORT_DOC_RADIO = htmlDoc.GetElementById("IS_TRANSPORT_DOC_RADIO").GetAttribute("value");
                    //单份全程运输单证号码
                    list[num].TRANSPORT_DOC_NO_TEXT = htmlDoc.GetElementById("TRANSPORT_DOC_NO_TEXT").GetAttribute("value");

                    #region 项号信息
                    list[num].order_no = item.Children[2].InnerText;
                    //项号
                    list[num].NO = item.Children[2].InnerText;
                    //HS编码
                    list[num].CODE_TS = item.Children[3].InnerText;
                    //数量
                    list[num].QTY = item.Children[4].InnerText;
                    //计量单位
                    list[num].UNIT = item.Children[5].InnerText;
                    //原产地标准
                    list[num].ORIGIN_CRITERION = item.Children[6].InnerText;

                    //更新状态
                    list[num].single_state = "8";
                    #endregion
                    miBll.UpdateCertificateDetail(list[num]);
                }

            }
        }
        #endregion

        //通过document设置text 文本框值
        private void SetText(String textName,String type,String value,HtmlDocument hd)
        {
            hd.GetElementById(textName).Focus();
            hd.GetElementById(textName).SetAttribute(type, value);
            Application.DoEvents();
            SendKeys.Send("{tab}");
            Application.DoEvents();
            Thread.Sleep(600);
        }

        //通过document设置ComBox 文本框值
        private void SetComBox(String textName, String type, String value)
        {
            webBrowser3.Document.GetElementById(textName).InvokeMember("click");
            webBrowser3.Document.GetElementById(textName).SetAttribute(type, value);
            Application.DoEvents();
            SendKeys.Send("{tab}");
        }

        //通过JavaScript设置text 文本框值
        public void ExecJS(HtmlDocument Doc, string JsFun, string FunNanme)
        {           
            HtmlElement ele = Doc.CreateElement("script");
            ele.SetAttribute("type", "text/javascript");
            ele.SetAttribute("text", JsFun);
            Doc.Body.AppendChild(ele);
            Doc.InvokeScript(FunNanme);
            SendKeys.Send("{tab}");
            Application.DoEvents();
        }    
      

        #region //删除原产地证明书
        /// <summary>
        /// 删除原产地证明书数据
        /// </summary>
        private void DeleteCarinfo()
        {
            SendKeys.Send("{enter}");
            timer3.Stop();

            htmlDoc = webBrowser3.Document.Window.Frames["mainFrame"].Document;
            if (htmlDoc==null)
            {
                MessageBox.Show("htmlDoc为空！");
                //SendKeys.Send("{ENTER}");
                timer3.Start();
                return;
            }
            HtmlElement table = htmlDoc.GetElementById("ID_tableData");
            if (table == null)
            {
                MessageBox.Show("table为空！");
                //SendKeys.Send("{ENTER}");
                timer3.Start();
                return;
            }
            HtmlElement tbody = table.Children[1];
            int rows = tbody.Children.Count;
            if (rows.Equals(0))
            {
                detailIdlist.Remove(detailIdlist[0]);
                
                timer2.Start();
                return;
            }

            HtmlElement tr = tbody.Children[0];
            if (tr == null)
            {
                detailIdlist.Remove(detailIdlist[0]);
                timer2.Start();
                return;
            }


            HtmlElement deletebtn = htmlDoc.GetElementById("delButtn");
            if (deletebtn != null && deletebtn.Enabled)
            {
                SendKeys.Send("{ENTER}");
                //deletebtn.InvokeMember("click");
                timer3.Start();
            }
            else {
                //Thread t = new Thread(TimerStart);//创建了线程还未开启
                //t.Start();//用来给函数传递参数，开启线程
                SendKeys.Send("{ENTER}");
                //tr.InvokeMember("click");
                timer3.Start();
            }
        }
        #endregion





        //thread开启线程要求：该方法参数只能有一个，且是object类型
		public void TimerStart(){
            Thread.Sleep(3000);
            timer3.Start();
		}





        #region //提交原产地证明书数据
        int commiteFlag = 0;
        /// <summary>
        /// 提交原产地证明书数据
        /// </summary>
        private void CommiteCarinfo()
        {
            timer3.Stop();
            htmlDoc = webBrowser3.Document.Window.Frames["mainFrame"].Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("htmlDoc为空！");
                timer3.Start();
                return;
            }
            HtmlElement table = htmlDoc.GetElementById("ID_tableData");
            if (table == null)
            {
                MessageBox.Show("table为空！");
                timer3.Start();
                return;
            }
            HtmlElement tbody = table.Children[1];
            int rows = tbody.Children.Count;
            if (rows.Equals(0))
            {
                detailIdlist.Remove(detailIdlist[0]);
                
                timer2.Start();
                return;
            }


            
           


            HtmlElement submitbtn = htmlDoc.GetElementById("applyButtn");
            HtmlElement copyButtn = htmlDoc.GetElementById("copyButtn");
            if (submitbtn != null && submitbtn.Enabled && copyButtn.Enabled)
            {
                SendKeys.Send("{enter}");
                SendKeys.Send("{ESC}");
                //submitbtn.InvokeMember("click");
                timer3.Start();
            }
            else
            {
                if (commiteFlag < rows)
                {
                    HtmlElement tr = tbody.Children[commiteFlag];
                    if (tr == null)
                    {
                        detailIdlist.Remove(detailIdlist[0]);
                       
                        timer2.Start();
                        return;
                    }
                    commiteFlag++;
                    SendKeys.Send("{enter}");
                    SendKeys.Send("{ESC}");
                    //tr.InvokeMember("click");
                    timer3.Start();
                }
                else {

                    HtmlElement nextpage = htmlDoc.GetElementById("hasNextHref");
                    if (nextpage == null || !nextpage.Enabled)
                    {
                        detailIdlist.Remove(detailIdlist[0]);
                        
                        HtmlElement firstpage = htmlDoc.GetElementById("fistHref");
                        if (firstpage != null)
                        {
                            //htmlDoc.All["fistHref"].InvokeMember("click");//一个bug  当一票里的两项申报数量一致时 第一项抓取后要点击首页
                        }
                        timer2.Start();
                    }
                    else
                    {
                        nextpage.InvokeMember("click");
                        timer3.Start();
                    }
                    commiteFlag = 0;
                }
               
            }

            
        }
        #endregion



        #region//阻止弹出新窗口


        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            mshtml.IHTMLDocument2 doc = (webBrowser3.ActiveXInstance as SHDocVw.WebBrowser).Document as mshtml.IHTMLDocument2;
            doc.parentWindow.execScript("window.alert=null", "javascript");
            doc.parentWindow.execScript("window.confirm=null", "javascript");
            doc.parentWindow.execScript("window.open=null", "javascript");
            doc.parentWindow.execScript("window.showModalDialog=null", "javascript");
            doc.parentWindow.execScript("window.close=null", "javascript");
        }



        private void InjectAlertBlocker()
        {
            //自动点击弹出确认或弹出提示
            IHTMLDocument2 vDocument = (IHTMLDocument2)webBrowser3.Document.DomDocument;
            vDocument.parentWindow.execScript("function confirm(str){return true;} ", "javascript"); //弹出确认
            vDocument.parentWindow.execScript("function alert(str){return true;} ", "javaScript");//弹出提示
        }

        //必须要在这个事件里面调用才能禁用，之前一直写在别的事件里面，浪费了2小时。
        private void webBrowser2_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            InjectAlertBlocker();

        }

        //阻止弹出新窗口
        private void webBrowser2_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

        }

        #endregion



        #region//启动浏览器
        /// <summary>
        /// 加载浏览器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.StartIE();
        }

        private void StartIE()
        {
            var appName = Process.GetCurrentProcess().ProcessName + ".exe";
            SetIE11KeyforWebBrowserControl(appName);
            this.webBrowser3.Url = new Uri("https://i.chinaport.gov.cn/deskserver/sw/deskIndex?menu_id=noc");

            webBrowser3.ScriptErrorsSuppressed = true;
            //等待加载完毕
            while (webBrowser3.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();

            htmlDoc = this.webBrowser3.Document;
            if (htmlDoc.All["password"] != null)
            {

                htmlDoc.All["password"].SetAttribute("value", "88888888");
                htmlDoc.All["loginbuttonCa"].InvokeMember("click");
            }
            //解禁按钮
            setButtonEnable(true);
        }

        private void SetIE11KeyforWebBrowserControl(string appName)
        {
            RegistryKey Regkey = null;
            try
            {
                // For 64 bit machine
                if (Environment.Is64BitOperatingSystem)
                    Regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Wow6432Node\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                else  //For 32 bit machine
                    Regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);

                // If the path is not correct or
                // if the user haven't priviledges to access the registry
                if (Regkey == null)
                {
                    MessageBox.Show("设置失败，没找到地址");
                    return;
                }

                string FindAppkey = Convert.ToString(Regkey.GetValue(appName));
                labelItem2.Text = "当前浏览器版本：" + FindAppkey;

                // Check if key is already present
                //if (FindAppkey == "11000")//11000
                //{
                //    //MessageBox.Show("目前版本已经设置好");
                //    Regkey.Close();
                //    return;
                //}

                string ieVersion = textBox1.Text;
                switch (ieVersion)
                {
                    case "7000":
                        Regkey.SetValue(appName, unchecked((int)0x1B58), RegistryValueKind.DWord);//ie7
                        break;
                    case "8000":
                        Regkey.SetValue(appName, unchecked((int)0x1F40), RegistryValueKind.DWord);//ie8
                        break;
                    case "9000":
                        Regkey.SetValue(appName, unchecked((int)0x2328), RegistryValueKind.DWord);//ie9
                        break;
                    case "10000":
                        Regkey.SetValue(appName, unchecked((int)0x2710), RegistryValueKind.DWord);//ie10
                        break;
                    case "11000":
                        Regkey.SetValue(appName, unchecked((int)0x2AF8), RegistryValueKind.DWord);//ie11
                        break;
                    case "8888":
                        Regkey.SetValue(appName, unchecked((int)0x22B8), RegistryValueKind.DWord);//ie88
                        break;
                    case "9999":
                        Regkey.SetValue(appName, unchecked((int)0x270F), RegistryValueKind.DWord);//ie99
                        break;
                    case "":
                        Regkey.SetValue(appName, unchecked((int)0x2AF8), RegistryValueKind.DWord);//ie99
                        ieVersion = "11000";
                        break;
                    default:
                        break;
                }
                //Regkey.SetValue(appName, unchecked((int)0x2AF8), RegistryValueKind.DWord);//ie11
                // Check for the key after adding
                FindAppkey = Convert.ToString(Regkey.GetValue(appName));

                if (FindAppkey == ieVersion)
                {
                    MessageBox.Show("本次设置成功");
                    labelItem2.Text = "当前浏览器版本：" + FindAppkey;
                }
                else
                {
                    MessageBox.Show("本次设置失败, Ref: " + FindAppkey);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("设置出现异常");
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close the Registry
                if (Regkey != null)
                    Regkey.Close();
            }
        }

        #endregion



        #region //上传原产地证
        /// <summary>
        /// 上传原产地证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            flag = 1;
            startTime = DateTime.Now;
            tempSave();
        }




        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {            
            flag = 2;
            getOrigionNo();           
        }

        /// <summary>
        /// 抓取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            flag = 3;            
            getOriginDetail();
        }





        /// <summary>
        /// 查询原产地证详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            search();
        }


        public void search()
        {
            SingleInfo carInfo = new SingleInfo();
            String batch = comboBox1.Text;
            carInfo.batch = batch;
            List<SingleInfo> dvlist = miBll.GetList(carInfo);
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = dvlist;
            if (dvlist.Count>0)
            {

                dataGridView1.Rows[0].Selected = false;
            }
        }

        /// <summary>
        /// 设置按钮可用
        /// </summary>
        /// <param name="bl"></param>
        public void setButtonEnable(bool bl)
        {

            if (bl)
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
            }
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要删除吗？", "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            flag = 3;            
            timer3.Interval = timer3Flash;
        }
        

        private void getSearchMenu()
        {
            
            HtmlElement htmlED = webBrowser3.Document.GetElementById("searchInfo");
            //htmlED.InvokeMember("click");
            foreach (HtmlElement item in htmlED.All)
            {
                if (item.InnerText== "原产地证据文件查询")
                {
                    item.InvokeMember("click");
                    break;
                }
            }
        }


        /// <summary>
        /// 格式化数据
        /// </summary>
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.Value != null && e.Value != "") //哪一列
            {
                if (Convert.ToInt32(e.Value) == 7)
                {
                    e.Value = "写入完成！";
                    e.CellStyle.ForeColor = Color.Green;
                }
                else if (Convert.ToInt32(e.Value) == 8)
                {
                    e.Value = "抓取完成！";
                    e.CellStyle.ForeColor = Color.Violet;
                }
                else if (Convert.ToInt32(e.Value) == 9)
                {
                    e.Value = "提交完成！";
                    e.CellStyle.ForeColor = Color.Green;
                }
                else if (Convert.ToInt32(e.Value) == 10)
                {
                    e.Value = "暂不处理！";
                    e.CellStyle.ForeColor = Color.Red;
                }
                else if (Convert.ToInt32(e.Value) == 11)
                {
                    e.Value = "证明书编号完成";
                    e.CellStyle.ForeColor = Color.Green;
                }
                else {
                    e.Value = "待写入";
                    e.CellStyle.ForeColor = Color.Red;
                }
            }

            if (e.ColumnIndex == 1 && e.Value != null && e.Value != "") //哪一列
            {
                if (ht_dfNo.ContainsKey(e.Value))
                {
                    e.CellStyle.ForeColor = Color.Blue;
                }
                
            }
        }


        /// <summary>
        /// 停止按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            this.stopTimers();
        }

        private void stopTimers()
        {
            if (timer1 != null)
            {
                timer1.Stop();
            }
            if (timer2 != null)
            {
                timer2.Stop();
            }
            if (timer3 != null)
            {
                timer3.Stop();
            }
            if (timer5 != null)
            {
                timer5.Stop();
            }
            if (timer6 != null)
            {
                timer6.Stop();
            }
            if (timer7 != null)
            {
                timer7.Stop();
            }
            if (timer8 != null)
            {
                timer8.Stop();
            }
            setButtonEnable(true);
            num = 0;
        }

        #endregion



        #region //检查提交相关
        private void button9_Click(object sender, EventArgs e)
        {
            htmlDoc = this.webBrowser3.Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("请确认是否已启动浏览器？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            htmlDoc = webBrowser3.Document.Window.Frames["leftFrame"].Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("没有获取到菜单项，请确认是否已登录？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            HtmlElementCollection dhl = htmlDoc.GetElementsByTagName("A");
            foreach (HtmlElement item in dhl)
            {
                if (item.InnerText == "批量提交原产地证明书签发申请")
                {
                    //item.InvokeMember("click");
                }

            }
            String batch = comboBox1.Text;

            if (batch == "")
            {
                MessageBox.Show("请输入批次号！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            carInfo.batch = batch;
            carInfo.single_state = "10";
            #region //勾选处理
            dfNoStr = "";
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if ((Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true))
                {
                    dfNoStr += "'" + dataGridView1.Rows[i].Cells[1].Value.ToString() + "',";
                }
                else
                    continue;
            }
            //有勾选项
            if (!dfNoStr.Equals(""))
            {
                dfNoStr = dfNoStr.Substring(0, dfNoStr.Length - 1);
                SingleInfo carinfo1 = new SingleInfo();
                carinfo1.batch = batch;
                carinfo1.certificate_origin_no = dfNoStr;
                carinfo1.single_state = "10";
                list = miBll.selectChecked(carinfo1);
            }
            else
            {
                list = miBll.GetList(carInfo);

            }
            #endregion
            if (list.Count == 0)
            {
                MessageBox.Show("该批次查询暂无数据！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            timer5 = new System.Windows.Forms.Timer();
            timer5.Interval = 2000;
            timer5.Enabled = true;
            timer5.Tick += new EventHandler(timer5EventProcessor);//添加事件

            timer6 = new System.Windows.Forms.Timer();
            timer6.Interval = 2000;
            timer6.Enabled = true;
            timer6.Tick += new EventHandler(timer6EventProcessor);//添加事件
            timer6.Stop();
        }


        /// <summary>
        /// 点击查询原产地证号
        /// </summary>
        /// <param name="dfNo"></param>
        private bool putInfoForSearch(String dfNo)
        {
            htmlDoc = webBrowser3.Document.Window.Frames["mainFrame"].Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("没有获取原产地证查询的frame，请确认是否已登录？");
                //解禁按钮
                setButtonEnable(true);
                return true;
            }
            htmlDoc.All["EntryId"].SetAttribute("value", dfNo);

            HtmlElementCollection inputs = htmlDoc.GetElementsByTagName("input");
            foreach (HtmlElement item in inputs)
            {
                if (item.GetAttribute("value") == "查询")
                {
                    SendKeys.Send("{ESC}");
                    //item.InvokeMember("click");
                }
            }
            return false;
        }


        bool batchFlag = true;
        /// <summary>
        /// timer3
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void timer5EventProcessor(object source, EventArgs e)
        {
            
            timer5.Stop();
            if (list.Count==0)
                {
                    timer5.Stop();
                    MessageBox.Show("检查提交完成！");
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }
                //输入原产地证号页面查询事件
                if (putInfoForSearch(list[0].certificate_origin_no))
                {
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }
                timer6.Start();

           
        }


        public void timer6EventProcessor(object source, EventArgs e)
        {
            SendKeys.Send("{ESC}");
            timer6.Stop();
            htmlDoc = webBrowser3.Document.Window.Frames["mainFrame"].Document;

            HtmlElement table = htmlDoc.GetElementById("tableDate");
            if (table == null)
            {
                MessageBox.Show("没有获取到原产地证详情列表！");
                //解禁按钮
                setButtonEnable(true);
            }
            HtmlElement tbody = table.Children[1];

            int rows = tbody.Children.Count;
            if (rows > 0)
            {
                tr = tbody.Children[0];
                if (tr.Children.Count > 2)
                {
                    SingleInfo carinfo1 = new SingleInfo();
                    carinfo1.batch = carInfo.batch;
                    carinfo1.single_state = "8";
                    carinfo1.certificate_origin_no = "'" + list[0].certificate_origin_no + "'";
                    miBll.UpdateHeadState1(carinfo1);
                }

            }


            list.Remove(list[0]);
            timer5.Start();
        }
        #endregion


        #region //复选框CheckBox相关
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if ((Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == false))
                    {
                        dataGridView1.Rows[i].Cells[0].Value = "True";
                    }
                    else
                        continue;
                }
            }
            else {

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if ((Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true))
                    {
                        dataGridView1.Rows[i].Cells[0].Value = "False";
                    }
                    else
                        continue;
                }
            }
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            String batch = comboBox1.Text;
            String state = comboBox2.SelectedValue.ToString();
            String dfNos ="";
            
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if ((Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true))
                {
                    dfNos += "'"+dataGridView1.Rows[i].Cells[1].Value.ToString()+"',";
                }
                else
                    continue;
            }
            if (!dfNos.Equals(""))
            {
                dfNos = dfNos.Substring(0, dfNos.Length - 1);
                SingleInfo carinfo1 = new SingleInfo();
                carinfo1.batch = batch;
                carinfo1.single_state = state;
                carinfo1.certificate_origin_no = dfNos;
                miBll.UpdateHeadState1(carinfo1);
            }
            search();
            checkBox1.Checked = false;
        }

        #endregion


        #region //批量提交相关
        private void button10_Click(object sender, EventArgs e)
        {
            htmlDoc = this.webBrowser3.Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("请确认是否已启动浏览器？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            htmlDoc = webBrowser3.Document.Window.Frames["leftFrame"].Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("没有获取到菜单项，请确认是否已登录？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            HtmlElementCollection dhl = htmlDoc.GetElementsByTagName("A");
            foreach (HtmlElement item in dhl)
            {
                if (item.InnerText == "批量提交原产地证明书签发申请")
                {
                    //item.InvokeMember("click");
                }

            }
            String batch = comboBox1.Text;

            if (batch == "")
            {
                MessageBox.Show("请输入批次号！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            carInfo.batch = batch;
            carInfo.single_state = "9";
            #region //勾选处理
            dfNoStr = "";
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if ((Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true))
                {
                    dfNoStr += "'" + dataGridView1.Rows[i].Cells[1].Value.ToString() + "',";
                }
                else
                    continue;
            }
            //有勾选项
            if (!dfNoStr.Equals(""))
            {
                dfNoStr = dfNoStr.Substring(0, dfNoStr.Length - 1);
                SingleInfo carinfo1 = new SingleInfo();
                carinfo1.batch = batch;
                carinfo1.certificate_origin_no = dfNoStr;
                carinfo1.single_state = "9";
                list = miBll.selectChecked(carinfo1);
            }
            else
            {
                list = miBll.GetList(carInfo);

            }
            #endregion
            if (list.Count == 0)
            {
                MessageBox.Show("该批次查询暂无数据！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            timer7 = new System.Windows.Forms.Timer();
            timer7.Interval = 2000;
            timer7.Enabled = true;
            timer7.Tick += new EventHandler(timer7EventProcessor);//添加事件

            timer8 = new System.Windows.Forms.Timer();
            timer8.Interval = 2000;
            timer8.Enabled = true;
            timer8.Tick += new EventHandler(timer8EventProcessor);//添加事件
            timer8.Stop();
        }





        /// <summary>
        /// timer3
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void timer7EventProcessor(object source, EventArgs e)
        {
            SendKeys.Send("{ESC}");
            timer7.Stop();

            if (!dfNoStr.Equals(""))
            {
                SingleInfo carinfo1 = new SingleInfo();
                carinfo1.batch = carInfo.batch;
                carinfo1.certificate_origin_no = dfNoStr;
                carinfo1.single_state = "9";
                list = miBll.selectChecked(carinfo1);
            }
            else
            {
                //多个客户端交替查询
                carInfo.single_state = "9";
                list = miBll.GetList(carInfo);

            }
            if (list.Count > 0)
            {
                list[0].single_state = "9";
                miBll.UpdateHeadState(list[0]);
            }
            if (list.Count == 0)
            {
                timer7.Stop();
                timer8.Stop();
                lblmessage.Text = "批量提交完成！";
                MessageBox.Show("批量提交完成！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            //输入原产地证号页面查询事件
            if (putInfoForSearch(list[0].certificate_origin_no))
            {
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            lblmessage.Text = list[0].certificate_origin_no;
            timer8.Interval = 2000;
            timer8.Start();
            pageFlag = "";
            sum = 1;
        }

        int sum = 1;
        bool batchcommit = true;
        string pageFlag = "";
        public void timer8EventProcessor(object source, EventArgs e)
        {

            SendKeys.Send("{ESC}");
            timer8.Stop();
            lblmessage.Text = "30秒结束：" + sum;

            htmlDoc = webBrowser3.Document.Window.Frames["mainFrame"].Document;
            
            HtmlElement table = htmlDoc.GetElementById("tableDate");
            if (table == null)
            {
                MessageBox.Show("没有获取到原产地证详情列表！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            HtmlElement tbody = table.Children[1];

            int rows = tbody.Children.Count;
            if (rows > 0)
            {

                tr = tbody.Children[0];
                if (tr.Children.Count > 2)
                {
                    HtmlElement ID_pagination = htmlDoc.GetElementById("ID_pagination");
                    if (ID_pagination == null)
                    {
                        lblmessage.Text = "page为空，timer7.Start提交下一票！";
                        timer8.Interval = 1000;
                        timer7.Start();
                        return;
                    }
                    HtmlElementCollection span = ID_pagination.GetElementsByTagName("SPAN");
                    string pageFlags = "";
                    foreach (HtmlElement item in span)
                    {
                        if (item.InnerText.Contains("共"))
                        {
                            pageFlags = item.InnerText;
                            int indexStart = pageFlags.IndexOf("共");
                            pageFlags = pageFlags.Substring(indexStart + 1, 2);
                            pageFlags = pageFlags.Replace("页", "");
                        }

                    }

                    if (pageFlags.Equals(pageFlag))
                    {

                        timer8.Interval = 5000;
                        timer8.Start();
                        sum++;
                        return;
                    }



                    if(batchcommit){
                        HtmlElement selectAll = htmlDoc.GetElementById("selectAll");
                        if (selectAll == null)
                        {
                            timer8.Interval = sum++ * 4000;
                            return;
                        }

                        //selectAll.InvokeMember("click");
                        batchcommit = false;
                        timer8.Interval = 2000;
                        timer8.Start();
                        return;
                    }

                    HtmlElementCollection dhl = htmlDoc.GetElementsByTagName("input");
                    foreach (HtmlElement item in dhl)
                    {
                        if (item.GetAttribute("value") == "批量提交")
                        {
                            //item.InvokeMember("click");
                            break;
                        }

                    }
                    pageFlag = pageFlags;
                    batchcommit = true; 
                    timer8.Interval = 30000;
                    lblmessage.Text = "点击提交完成，等待30秒！" + pageFlag;
                    timer8.Start();
                    return;
                }
                else
                {
                    lblmessage.Text = "timer7.Start提交下一票！";
                    timer7.Start();

                }

            }
            
           
            

        }
        #endregion

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            SolidBrush b = new SolidBrush(this.dataGridView1.RowHeadersDefaultCellStyle.ForeColor);
            e.Graphics.DrawString((e.RowIndex + 1).ToString(System.Globalization.CultureInfo.CurrentUICulture), this.dataGridView1.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
        }

        private void button11_Click(object sender, EventArgs e)
        {

            htmlDoc = this.webBrowser3.Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("请确认是否已启动浏览器？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            htmlDoc = webBrowser3.Document.Window.Frames["leftFrame"].Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("没有获取到菜单项，请确认是否已登录？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            HtmlElementCollection dhl = htmlDoc.GetElementsByTagName("A");
            foreach (HtmlElement item in dhl)
            {
                if (item.InnerText == "查询原产地证明书签发申请")
                {
                    //item.InvokeMember("click");
                }

            }
            String batch = comboBox1.Text;

            if (batch == "")
            {
                MessageBox.Show("请输入批次号！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            carInfo.batch = batch;
            carInfo.single_state = "10";
            #region //勾选处理
            dfNoStr = "";
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if ((Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true))
                {
                    dfNoStr += "'" + dataGridView1.Rows[i].Cells[1].Value.ToString() + "',";
                }
                else
                    continue;
            }
            //有勾选项
            if (!dfNoStr.Equals(""))
            {
                dfNoStr = dfNoStr.Substring(0, dfNoStr.Length - 1);
                SingleInfo carinfo1 = new SingleInfo();
                carinfo1.batch = batch;
                carinfo1.certificate_origin_no = dfNoStr;
                carinfo1.single_state = "10";
                list = miBll.selectChecked(carinfo1);
            }
            else
            {
                list = miBll.GetList(carInfo);

            }
            #endregion
            if (list.Count == 0)
            {
                MessageBox.Show("该批次查询暂无数据！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            timer9 = new System.Windows.Forms.Timer();
            timer9.Interval = 2000;
            timer9.Enabled = true;
            timer9.Tick += new EventHandler(timer9EventProcessor);//添加事件

            timer10 = new System.Windows.Forms.Timer();
            timer10.Interval = 2000;
            timer10.Enabled = true;
            timer10.Tick += new EventHandler(timer10EventProcessor);//添加事件
            timer10.Stop();
        }

        /// <summary>
        /// timer3
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void timer9EventProcessor(object source, EventArgs e)
        {
            timer9.Stop();

            if (!dfNoStr.Equals(""))
            {
                SingleInfo carinfo1 = new SingleInfo();
                carinfo1.batch = carInfo.batch;
                carinfo1.certificate_origin_no = dfNoStr;
                carinfo1.single_state = "11";
                list = miBll.selectChecked(carinfo1);
            }
            else
            {
                //多个客户端交替查询
                carInfo.single_state = "11";
                list = miBll.GetList(carInfo);

            }
            if (list.Count > 0)
            {
                list[0].single_state = "11";
                miBll.UpdateHeadState(list[0]);
            }
            if (list.Count == 0)
            {
                timer9.Stop();
                timer10.Stop();
                lblmessage.Text = "证明书编号抓取完成！";
                MessageBox.Show("证明书编号抓取完成！");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            //输入原产地证号页面查询事件
            if (putInfoForSearch(list[0].certificate_origin_no))
            {
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            lblmessage.Text = list[0].certificate_origin_no;
            timer10.Interval = 2000;
            timer10.Start();
        }


        /// <summary>
        /// timer3
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void timer10EventProcessor(object source, EventArgs e)
        {
            timer10.Stop();
            htmlDoc = webBrowser3.Document.Window.Frames["mainFrame"].Document;
            HtmlElement table = htmlDoc.GetElementById("tableDate");
            HtmlElement tbody = table.Children[1];
            int rows = tbody.Children.Count;
            if (rows.Equals(0))
            {
                if (MessageBox.Show("当前原产地证" + list[0].certificate_origin_no + "暂无数据是否继续抓取！", "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }
                timer9.Start();
                return;
            }
            for (int i = 0; i < rows; i++)
            {
                SingleInfo carinfo = new SingleInfo();
                HtmlElement tr = tbody.Children[i];
                if (tr != null)
                {
                    //carinfo.Barcode = tr.Children[9].InnerText == null ? "" : tr.Children[9].InnerText.ToString();
                    //carinfo.cer_no = tr.Children[5].InnerText == null ? "" : tr.Children[5].InnerText.ToString();
                    //carinfo.Statu = tr.Children[11].InnerText == null ? "" : tr.Children[11].InnerText.ToString();
                }
                carinfo.certificate_origin_no = list[0].certificate_origin_no;
                carinfo.batch = carInfo.batch;
                //carinfo.update_date = DateTime.Now;
                UpdateCerNoList.Add(carinfo);
            }

            if (UpdateCerNoList.Count > 0)
            {
                for (int i = 0; i < UpdateCerNoList.Count; i++)
                {
                    bool b = miBll.UpdateCerNo(UpdateCerNoList[i]);
                  }
                UpdateCerNoList.Clear();
                HtmlElement nextpage = htmlDoc.GetElementById("hasNextHref");
                if (nextpage == null)
                {
                    HtmlElement firstpage = htmlDoc.GetElementById("fistHref");
                    if (firstpage != null)
                    {
                        //htmlDoc.All["fistHref"].InvokeMember("click");//一个bug  当一票里的两项申报数量一致时 第一项抓取后要点击首页
                    }
                    timer9.Start();
                }
                else
                {
                   // htmlDoc.All["hasNextHref"].InvokeMember("click");
                    timer10.Start();
                }
            }
        }
    }
}
