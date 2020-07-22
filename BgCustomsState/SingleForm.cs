using BLL;
using Microsoft.Win32;
using MODEL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BgCustomsState
{
    public partial class SingleForm : Form
    {
        public HtmlDocument htmlDoc;
        public List<SingleWindow> list;
        public List<SingleWindow> detailIdlist;
        public List<SingleWindow> vinlist;
        public List<SingleGoodslimit> goodslimitList = new List<SingleGoodslimit>();
        public List<SingleGoodslimitVin> goodslimitListVin = new List<SingleGoodslimitVin>();
        public List<SingleContainer> ContainerList = new List<SingleContainer>();
        
        public int controlStep = 0;
        public HtmlElement tr = null;
        public int detailnum = 0;
        System.Windows.Forms.Timer timer1 = null;
        System.Windows.Forms.Timer timer2 = null;
        System.Windows.Forms.Timer timer3 = null;
        System.Windows.Forms.Timer timer4 = null;
        System.Windows.Forms.Timer timer5 = null;
        System.Windows.Forms.Timer timer6 = null;

        System.Windows.Forms.Timer originCertificate = null;

        public int number1 = 0;
        public int number2 = 0;
        public int number3 = 0;
        public int number4 = 0;
        public int number5 = 0;

        public int decCount = 0;
        public int getVinCount = 0;
        public int DetailCount = -1;
        public int flag = 0;
        public int detail_gno = 0;
        public String dfNoStr = "";

        public SingleDetail singleDetail = null;
        public int timer3_flag = 0;
        public int timer3_interval = 0;
        //创建业务逻辑层对象
        SingleWindowBll miBll = new SingleWindowBll();
        CarInfoBll carinfoBll = new CarInfoBll();
        public SingleForm()
        {
            InitializeComponent();
            list = new List<SingleWindow>();

            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void SingleForm_Load(object sender, EventArgs e)
        {
            List<CarInfo> cblist = carinfoBll.GetList();
            comboBox1.DataSource = cblist;
            comboBox1.DisplayMember = "Batch";
            comboBox1.ValueMember = "Batch";


            IList<Info> infoList = new List<Info>();
            Info info1 = new Info() { Id = "2", Name = "暂不处理" };
            Info info2 = new Info() { Id = "0", Name = "待抓取" };
            Info info3 = new Info() { Id = "1", Name = "抓取完成" };
            infoList.Add(info1);
            infoList.Add(info2);
            infoList.Add(info3);
            comboBox2.DataSource = infoList;
            comboBox2.ValueMember = "Id";
            comboBox2.DisplayMember = "Name";

            timer2 = new System.Windows.Forms.Timer();
            timer2.Interval = 2000;
            timer2.Enabled = true;
            timer2.Tick += new EventHandler(timer2EventProcessor);//添加事件
            timer2.Stop();

            timer3 = new System.Windows.Forms.Timer();
            timer3.Interval = 4000;
            timer3.Enabled = true;
            timer3.Tick += new EventHandler(timer3EventProcessor);//添加事件
            timer3.Stop();

            timer4 = new System.Windows.Forms.Timer();
            timer4.Interval = 2000;
            timer4.Enabled = true;
            timer4.Tick += new EventHandler(timer4EventProcessor);//添加事件
            timer4.Stop();

        }

        public class Info
        {
            public string Id { get; set; }
            public string Name { get; set; }

        }



        #region // 登录事件timer1
        /// <summary>
        /// timer登录事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void timer1EventProcessor(object source, EventArgs e)
        {

            controlStep++;
            timer1.Stop();

            try
            {

                if (controlStep == 1)
                {
                    labmessage.Text = "正在登录，请稍等！";
                    //等待加载完毕
                    while (webBrowser1.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
                    htmlDoc = this.webBrowser1.Document;
                    HtmlElement pwdInput = htmlDoc.GetElementById("password");
                    HtmlElement loginBtn = htmlDoc.GetElementById("loginbutton");
                    if (pwdInput != null && loginBtn != null)
                    {
                        htmlDoc.All["password"].SetAttribute("value", "88888888");
                        htmlDoc.All["loginbutton"].InvokeMember("click");
                        timer1.Interval = 10000;
                        timer1.Start();
                    }
                    else
                    {
                        MessageBox.Show("请确认当前界面是否为卡介质登录页面，重新启动浏览器！");
                        //controlStep = 0;
                        timer1.Interval = 5000;
                        timer1.Start();
                    }

                }

                //该步骤出现异常 就需要重新登录
                if (controlStep == 2)
                {
                    labmessage.Text = "正在跳转！";
                    //等待加载完毕
                    while (webBrowser1.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
                    htmlDoc = this.webBrowser1.Document;

                    HtmlElementCollection dhl = htmlDoc.GetElementsByTagName("A");
                    if (dhl.Count > 50)
                    {
                        foreach (HtmlElement item in dhl)
                        {
                            if (item.InnerText == "报关数据查询")
                            {
                                item.InvokeMember("click");
                                break;
                            }

                        }

                        //button1.Enabled = true;
                        //button2.Enabled = true;
                        timer1.Start();
                    }
                    else
                    {
                        if (MessageBox.Show("当前未进入查询页面是否继续自动登录？", "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        {
                            this.webBrowser1.Url = new Uri("https://swapp.singlewindow.cn/deskserver/sw/deskIndex?menu_id=dec001");
                            MessageBox.Show("当前自动登录停止！请手动完成登录！");
                            button1.Enabled = true;
                            button2.Enabled = true;
                            return;
                        }
                        this.webBrowser1.Url = new Uri("https://swapp.singlewindow.cn/deskserver/sw/deskIndex?menu_id=dec001");
                        controlStep = 1;
                        timer1.Start();
                    }

                }


                if (controlStep >= 3)
                {
                    
                    HtmlWindowCollection frame = webBrowser1.Document.Window.Frames;

                    htmlDoc = this.webBrowser1.Document;
                    HtmlElement layer = htmlDoc.GetElementById("page-wrapper");
                    if (layer==null)
                    {
                        controlStep = 0;
                        this.webBrowser1.Url = new Uri("https://app.singlewindow.cn/cas/login?service=https%3A%2F%2Fswapp.singlewindow.cn%2Fdeskserver%2Fj_spring_cas_security_check&logoutFlag=1&_swCardF=1");
                        MessageBox.Show("自动登录失败！");
                        return;
                    }
                    HtmlElementCollection dhl = layer.GetElementsByTagName("A");
                    foreach (HtmlElement item in dhl)
                    {
                        if (item.InnerText == "报关数据查询 " && frame.Count > 1)
                        {
                            MessageBox.Show("自动登录成功！");
                            labmessage.Text = "浏览器启动成功！";
                            button1.Enabled = true;
                            button2.Enabled = true;
                            timer1.Stop();
                            return;
                        }

                    }
                    MessageBox.Show("自动登录失败！");
                    labmessage.Text = "";
                    button1.Enabled = true;
                    button2.Enabled = true;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("登录异常！" + ex);

            }
        }

        #endregion





        SingleWindow singleWindow = new SingleWindow();
        #region//点击开始抓取
        /// <summary>
        /// 开始抓取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            #region //勾选处理

            flag = 1;
            singleWindow.ClientSeqNo = flag+"";
            String batch = comboBox1.Text;

            singleWindow.Batch = batch;
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
                if (batch == "")
                {
                    MessageBox.Show("请输入批次号！");
                    return;
                }
                singleWindow.SeqNo = dfNoStr;
                list = miBll.selectChecked(singleWindow);
            }
            else
            {
                list = miBll.GetList(singleWindow);

            }
            #endregion
            timer2.Interval = 2000;
            timer2.Start();


        }
        #endregion







        #region //启动第一个抓取timer

        bool bl = true;

        /// <summary>
        /// 查询统一编号
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void timer2EventProcessor(object source, EventArgs e)
        {
            if (bl && flag != 3 && flag != 4)
            {
                if (!dfNoStr.Equals(""))
                {
                    list = miBll.selectChecked(singleWindow);
                }
                else
                {
                    //多个客户端交替查询
                    list = miBll.GetList(singleWindow);

                }

                
            }
            timer2.Stop();
            number1++;
            System.Diagnostics.Debug.WriteLine(DateTime.Now + "   timer1EventProcessor*******************************************************第" + number1 + "票报关单===");


            if (list.Count > 0)
            {
                label5.Text = "当前抓取数据！统一编号为：" + list[0].SeqNo + "； 剩余： " + (list.Count - 1) + "票";
                labmessage.Text = "开始抓取数据！统一编号为：" + list[0].SeqNo + "  剩余： " + list.Count + "票";
                if (bl)
                {

                    //输入报关单号页面查询事件
                    putDfnoInfoForSearch(list[0].SeqNo);
                    bl = false;
                    timer2.Start();
                }
                else
                {

                    //点击明细
                    bool has = ClickItem(list[0].SeqNo);
                    if (has)
                    {
                        bhead = true;
                        userflag = 0;
                        bl = true;
                        timer2.Interval = 2000;
                        //等待加载完毕
                        while (webBrowser1.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
                        timer3.Start();
                        
                        //更新抓取状态
                        SingleWindow singleWindow = new SingleWindow();
                        singleWindow.ClientSeqNo = flag + "";
                        singleWindow.SeqNo = "'" + list[0].SeqNo + "'";
                        singleWindow.Batch = list[0].Batch;
                        miBll.UpdateHeadState(singleWindow);
                    }
                    else
                    {
                        labmessage.Text = "该统一编号没有查询到任何数据！";
                        //当查询统一编号页面未及时加载出来 再次延迟timer2 查询加载
                        bl = true;
                        timer2.Interval = 8000;
                        timer2.Start();
                        return;
                    }

                }


            }
            else
            {
                if (flag == 1)
                {
                    labmessage.Text = "本次抓取数据结束！";
                    //SingleWindow singleWindow = new SingleWindow();
                    //singleWindow.Batch = "";
                    //singleWindow.ClientSeqNo = "1";
                    //list = miBll.GetList(singleWindow);
                    if (list.Count == 0)
                    {
                        //labmessage.Text = "本次抓取数据结束！下次自动抓取时间为：" + DateTime.Now.AddHours(1).ToLongTimeString();
                        //System.Diagnostics.Debug.WriteLine(DateTime.Now + "   timer1EventProcessor*******************************************************共" + list.Count + "票报关单数据抓取finish完成====");
                        //timer2.Interval = 3600000;
                        MessageBox.Show("恭喜你， 数据抓取完成！");
                        //label2.Text=DateTime.Now.AddHours(1).ToLongTimeString();
                    }
                    //timer2.Start();
                    bl = true;
                }

                if (flag == 3)
                {
                    MessageBox.Show("恭喜你， 导入关联号完成！");
                }
                if (flag == 4)
                {
                    MessageBox.Show("恭喜你， 抓取委托书编号完成！");
                }
            }

        }
        #endregion




        #region //点击列表事件
        private void putDfnoInfoForSearch(String seqNo)
        {
            htmlDoc = webBrowser1.Document.Window.Frames[1].Document;
            htmlDoc.All["cusCiqNo"].SetAttribute("value", seqNo);
            htmlDoc.All["tableFlagName"].SetAttribute("value", "否");

            htmlDoc.All["decQuery"].InvokeMember("click");

        }



        private bool ClickItem(String seqNo)
        {
            number4++;

            htmlDoc = webBrowser1.Document.Window.Frames[1].Document;

            HtmlElement table = htmlDoc.GetElementById("declareTable");
            HtmlElement tbody = table.Children[1];

            int rows = tbody.Children.Count;


            if (rows != 1)
            {
                MessageBox.Show("该票报关单包含项数与单一窗口项数不一致！单一窗口数量：" + rows + "系统查询数量" + 1);
                return false;
            }
            tr = tbody.Children[0];
            String seqNo1 = "";
            if (tr.Children.Count > 1)
            {
                seqNo1 = tr.Children[1].InnerText == null ? "" : tr.Children[1].InnerText;
            }
           

            if (seqNo != seqNo1)
            {
                MessageBox.Show("该票报关单统一编号与单一窗口统一编号不一致！单一窗口统一编号：" + seqNo1 + "系统统一编号：" + seqNo);
                return false;
            }
            HtmlElement td = tr.Children[1].GetElementsByTagName("A")[0];

            td.InvokeMember("click");
            return true;

        }



        private void ClickItemForDetail()
        {
            //项数计数器
            detail_gno++;
            label5.Text = "当前抓取数据！统一编号为：" + list[0].SeqNo + "；  第" + detail_gno + "项；  剩余： " + (list.Count - 1) + "票";
            labmessage.Text = "当前抓取表体信息，第" + detail_gno + "项";
            htmlDoc = webBrowser1.Document.Window.Frames[2].Document;

            HtmlElement table = htmlDoc.GetElementById("decListTable");
            HtmlElement tbody = table.Children[1];

            DetailCount = tbody.Children.Count;

            HtmlElement tr = tbody.Children[detail_gno - 1];

            String seqNo = tr.Children[1].InnerText.ToString();

            if (seqNo != detail_gno.ToString())
            {
                MessageBox.Show("该票报关单表体项号出错！单一窗口表体项号：" + seqNo);
                return;
            }

            tr.Children[0].InvokeMember("click");

        }
        #endregion




        #region //启动第二个抓取timer 抓取head信息
        bool bhead = true;
        bool container = false;
        int userflag = 0;
        int timer3_step = 0;
        bool HeadShow = true;
        int rowsC = 0;
        public void timer3EventProcessor(object source, EventArgs e)
        {
            timer3.Stop();
            timer3_step++;
            if (webBrowser1.Document.Window.Frames.Count > 2)
            {
                timer3.Stop();
                htmlDoc = webBrowser1.Document.Window.Frames[2].Document;
                //等待加载完毕
                while (webBrowser1.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();

                if (bhead)
                {
                    #region//118关联号
                    //118关联号
                    if (flag == 3)
                    {
                        try
                        {
                            HtmlElement decHeadShow = htmlDoc.GetElementById("decHeadShow");
                            if (decHeadShow != null)
                            {
                                if (HeadShow)
                                {
                                    decHeadShow.InvokeMember("click");
                                    timer3.Start();
                                    HeadShow = false;
                                    return;
                                }
                            }
                            else
                            {
                                ClickItem(list[0].SeqNo);
                                timer3.Interval = 8000;
                                timer3.Start();
                                return;
                            }



                            //未加载成功 延长加载时间
                            if (addCiqNo())
                            {
                                ClickItem(list[0].SeqNo);
                                timer3.Interval = 8000;
                                timer3.Start();
                                return;
                            };
                            //关闭frame2 待定
                            CloseFrame();
                            timer2.Start();
                            timer3.Interval = 3000;
                            list.Remove(list[0]);
                            timer3_step = 0;
                            HeadShow = true;
                            return;
                        }
                        catch (Exception)
                        {
                            bl = false;
                            timer3.Interval = 8000;
                            timer2.Start();
                            return;
                        }
                    }
                    #endregion

                    #region//随附单据 
                    if (flag == 4)
                    {
                        try
                        {
                            //未加载成功 延长加载时间
                            if (getDecDocBtn())
                            {
                                ClickItem(list[0].SeqNo);
                                timer3.Interval = 8000;
                                timer3.Start();
                                return;
                            };
                            flag = 5;
                            timer3.Start();
                            return;
                        }
                        catch (Exception)
                        {
                            bl = false;
                            timer3.Interval = 8000;
                            timer2.Start();
                            return;
                        }
                    }
                    if (flag == 5)
                    {
                        //直到能获取到使用人 才能往下执行获取表头
                        HtmlElement table = htmlDoc.GetElementById("decDocTable");
                        HtmlElement tbody = table.Children[1];

                        int rows = tbody.Children.Count;

                        foreach (HtmlElement item in tbody.Children)
                        {
                            if (item.Children[0].InnerText.Equals("代理报关委托协议（电子）"))
                            {
                                list[0].ProDfNo = item.Children[2].InnerText.ToString();
                                break;
                            }

                        }


                        CloseDecDocContent();


                        //未加载成功 延长加载时间
                        if (list[0].ProDfNo != null && list[0].ProDfNo != "")
                        {
                            //保存数据到head
                            miBll.UpadteHeadInfo(list[0]);
                        };
                        //关闭frame2 待定
                        flag = 4;
                        CloseFrame();
                        timer2.Start();
                        timer3.Interval = 3000;
                        list.Remove(list[0]);
                        return;
                    }
                    #endregion

                    labmessage.Text = "正在抓取表头数据！";

                    #region //使用人信息
                    //点击使用人
                    if (userflag == 0)
                    {
                        //使用人按钮非空校验
                        HtmlElement userInfo = htmlDoc.GetElementById("userInfoBtn");
                        if (userInfo == null)
                        {
                            userflag = 0;
                            timer3.Start();
                            return;
                        }

                        //点击使用人加载信息
                        htmlDoc.All["userInfoBtn"].InvokeMember("click");
                        userflag = 1;
                        timer3.Interval = 3000;
                        timer3.Start();
                        return;
                    }
                    //抓取使用人
                    if (userflag == 1)
                    {
                        //直到能获取到使用人 才能往下执行获取表头
                        HtmlElement table = htmlDoc.GetElementById("userTable");
                        try
                        {
                            HtmlElement tbody = table.Children[1];

                            int rows = tbody.Children.Count;

                            tr = tbody.Children[0];
                            if (tr.Children.Count < 2)
                            {
                                timer3_interval++;
                                labmessage.Text = "使用人信息为空！";
                                userflag = 0;//没有获取到 继续点击
                                CloseUserInfo();
                                ClickItem(list[0].SeqNo);

                                timer3.Interval = timer3_interval * 6000;
                                timer3.Start();
                                return;
                            }
                        }
                        catch (Exception)
                        {
                            bl = false;
                            timer3.Interval = 8000;
                            timer2.Start();
                            return;
                        }
                        list[0].UseOrgPersonCode = tr.Children[2].InnerText.ToString();
                        list[0].UseOrgPersonTel = tr.Children[3].InnerText.ToString();

                        timer3_interval = 0;
                        CloseUserInfo();
                        userflag = 2;
                    }
                    #endregion


                    #region  //其他包装信息
                    //点击其他包装
                    if (userflag == 2)
                    {
                        HtmlElement packageInfo = htmlDoc.GetElementById("packageInfoBtn");
                        if (packageInfo == null)
                        {
                            userflag = 2;
                            timer3.Start();
                            return;
                        }

                        //点击使用人加载信息
                        htmlDoc.All["packageInfoBtn"].InvokeMember("click");
                        userflag = 3;
                        timer3.Interval = 3000;
                        timer3.Start();
                        return;
                    }
                    //抓取其他包装
                    if (userflag == 3)
                    {
                        //直到能获取到使用人 才能往下执行获取表头
                        HtmlElement table = htmlDoc.GetElementById("decPackageTable");
                        HtmlElement tbody = table.Children[1];

                        int rows = tbody.Children.Count;
                        foreach (HtmlElement item in tbody.Children)
                        {
                            if (item.FirstChild.InnerHtml.Contains("checked"))
                            {
                                tr = item;
                                break;
                            }
                        }
                        if (rows < 2)
                        {
                            timer3_interval++;
                            labmessage.Text = "其他包装信息为空！";
                            userflag = 2;//没有获取到 继续点击
                            ClosepackageInfo();
                            ClickItem(list[0].SeqNo);

                            timer3.Interval = timer3_interval * 6000;
                            timer3.Start();
                            return;
                        }

                        list[0].PackType = tr.Children[2].InnerText.ToString();//包装材料代码
                        list[0].PackTypeName = tr.Children[3].InnerText.ToString();//包装材料名称

                        timer3_interval = 0;
                        ClosepackageInfo();
                        userflag = 4;
                    }
                    #endregion


                    #region  //检验检疫签证申报要素信息
                    //点击检验检疫签证申报要素
                    if (userflag == 4)
                    {
                        HtmlElement docReqBtn = htmlDoc.GetElementById("docReqBtn");
                        if (docReqBtn == null)
                        {
                            userflag = 2;
                            timer3.Start();
                            return;
                        }

                        //点击检验检疫签证申报要素
                        htmlDoc.All["docReqBtn"].InvokeMember("click");
                        userflag = 5;
                        timer3.Interval = 3000;
                        timer3.Start();
                        return;
                    }
                    //抓取检验检疫签证申报要素
                    if (userflag == 5)
                    {
                        //直到能获取到检验检疫签证申报要素 才能往下执行获取表头
                        HtmlElement table = htmlDoc.GetElementById("docReqTable");
                        HtmlElement tbody = table.Children[1];

                        int rows = tbody.Children.Count;

                        tr = tbody.Children[17];
                        if (rows < 2)
                        {
                            timer3_interval++;
                            labmessage.Text = "检验检疫签证申报要素信息为空！";
                            userflag = 2;//没有获取到 继续点击
                            CloseDocReqBtn();
                            ClickItem(list[0].SeqNo);

                            timer3.Interval = timer3_interval * 6000;
                            timer3.Start();
                            return;
                        }

                        list[0].AppCertCode = tr.Children[2].InnerText.ToString();//申请单证代码
                        list[0].AppCertName = tr.Children[3].InnerText.ToString();//申请单证名称
                        list[0].ApplOri = tr.Children[4].Children[0].GetAttribute("value");//申请单证正本数
                        list[0].ApplCopyQuan = tr.Children[5].Children[0].GetAttribute("value");//申请单证副本数

                        list[0].DomesticConsigneeEname = htmlDoc.GetElementById("consigneeEname").GetAttribute("value");//境内收发货人名称(外文)
                        list[0].OverseasConsignorCname = htmlDoc.GetElementById("consignorCname").GetAttribute("value");//境外收发货人名称(中文)	
                        list[0].OverseasConsignorAddr = htmlDoc.GetElementById("consignorAddr").GetAttribute("value");//境外发货人地址
                        list[0].CmplDschrgDt = htmlDoc.GetElementById("cmplDschrgDt").GetAttribute("value");//卸毕日期	


                        timer3_interval = 0;
                        CloseDocReqBtn();
                        //userflag = 0;
                        //CloseDocReqBtn();
                        userflag = 6;
                        //timer3.Interval = 3000;
                        //timer3.Start();
                    }
                    #endregion

                    #region 原产地
                    //打开原产地证页面
                    if (userflag == 6)
                    {
                        //是否有选产地证，有就选中打开页面
                        int selectStatus = selectOrigin();
                        if (selectStatus == 1) 
                        {
                            HtmlElement docReqBtn = htmlDoc.GetElementById("decCiqRelAddBtn");
                            if (docReqBtn == null)
                            {
                                userflag = 2;
                                timer3.Start();
                                return;
                            }
                            //点击原产地证
                            htmlDoc.All["decCiqRelAddBtn"].InvokeMember("click");
                            userflag = 7;
                            timer3.Start();
                            return;
                        }
                        else if (selectStatus==1)//有原产地证页面没有打开（可能浏览器加载原因），递归再进行一次打开
                        {
                            timer3.Start();
                            return;
                        }
                        else
                        {
                            //无原产地证时跳过                           
                            timer3_interval = 0;                         
                            userflag = 0;
                        }
                    }

                    //原产地证项号抓取
                    if (userflag == 7)
                    {
                        HtmlElement decFrom = htmlDoc.GetElementById("cusCiqRelTable");
                        HtmlElement doc = decFrom.Children[1];
                        if (doc.Children.Count > 0)
                        {
                            String dgno = "";
                            String egno = "";
                            foreach (HtmlElement item in doc.Children)
                            {
                                dgno += item.Children[1].InnerText + ",";
                                egno += item.Children[2].InnerText + ",";
                            }
                            list[0].DecGNo = dgno;
                            list[0].EcoGNo = egno;
                            if (!dgno.Equals(""))
                            {
                                list[0].DecGNo = dgno.Substring(0, dgno.Length - 1);
                            }
                            if (!egno.Equals(""))
                            {
                                list[0].EcoGNo = egno.Substring(0, egno.Length - 1);
                            }
                        }
                        timer3_interval = 0;
                        CloseOriginC();
                        userflag = 0;
                    }
                    #endregion

                    #region  //许可证信息
                    HtmlElement table1 = htmlDoc.GetElementById("decLicenseTable");
                    HtmlElement tbody1 = table1.Children[1];
                    int rows1 = tbody1.Children.Count;
                    tr = tbody1.Children[0];

                    if (tr.Children.Count > 1)
                    {
                        list[0].DocTypeCode1 = tr.Children[1].InnerText.ToString();//单证代码
                        list[0].DocNo1 = tr.Children[2].InnerText.ToString();//单证编号 还没有更新到数据库
                        list[0].LicenceNo = tr.Children[2].InnerText.ToString();//许可证号
                    }
                    #endregion

                    #region  //集装箱信息
                    HtmlElement tableC = htmlDoc.GetElementById("decContainerTable");
                    HtmlElement tbodyC = tableC.Children[1];
                    rowsC = tbodyC.Children.Count;
                    tr = tbodyC.Children[0];
                    if (rowsC >= 1 && tr.Children.Count > 2)
                    {
                        ContainerList.Clear();
                        container = true;
                        timer3.Interval = 1000;
                    }


                    #endregion    
                    //获取表头信息
                    Application.DoEvents();
                    GetHeadInfo(list[0]);
                    labmessage.Text = "表头数据抓取完成！";


                }
                if (container)
                {
                    if (rowsC > ContainerList.Count)
                    {
                        //获取信息
                        GetContainerInfo(list[0]);
                    }
                    else
                    {
                        container = false;
                    }
                    bhead = false;
                    timer3.Start();
                    return;
                }
                if (detail_gno == DetailCount)
                {
                    labmessage.Text = "该票表体数据全部抓取完成！";
                    detail_gno = 0;
                    bhead = true;
                    timer2.Start();
                    //关闭frame2 待定
                    CloseFrame();
                    list.Remove(list[0]);
                }
                else
                {
                    bhead = false;
                    ClickItemForDetail();
                    singleDetail = new SingleDetail();
                    detail_saved = true;
                    timer4.Start();
                }
            }
            else if (timer3_step > 4)
            {
                MessageBox.Show("没获取到内容，抓取数据停止！");
            }
            else
            {
                timer2.Start();
            }


        }
        #endregion

        #region
        private void GetContainerInfo(SingleWindow singleWindow)
        {
            SingleContainer singleContainer = new SingleContainer();
            htmlDoc = webBrowser1.Document.Window.Frames[2].Document;

            singleContainer.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            singleContainer.UpdateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            singleContainer.Batch = singleWindow.Batch;
            singleContainer.CustDecHeadId = singleWindow.Id;
            singleContainer.ContainerId = htmlDoc.GetElementById("containerNo").GetAttribute("value");//集装箱号
            singleContainer.ContainerMd = htmlDoc.GetElementById("containerMdCode").GetAttribute("value");//集装箱规格
            singleContainer.ContainerWt = htmlDoc.GetElementById("containerWt").GetAttribute("value");//自重(KG)
            singleContainer.LclFlag = htmlDoc.GetElementById("lclFlag").GetAttribute("value");//拼箱标识
            singleContainer.GoodsNo = htmlDoc.GetElementById("goodsNo").GetAttribute("value");//商品项号关系

            if (!singleContainer.ContainerId.Equals("") && !singleContainer.ContainerMd.Equals("") && !singleContainer.ContainerWt.Equals(""))
            {
                ContainerList.Add(singleContainer);
                //查询是否存在 存在更新  不存在插入
                List<SingleContainer> exsitList = miBll.GetSingleContainer(singleContainer);
                if (exsitList.Count > 0)
                {
                    miBll.UpdateSingleContainer(singleContainer);
                }
                else
                {
                    miBll.InsertSingleContainer(singleContainer);

                }
            }

            HtmlElement tableC = htmlDoc.GetElementById("decContainerTable");
            HtmlElement tbodyC = tableC.Children[1];
            int rowsC = tbodyC.Children.Count;
            if (rowsC > ContainerList.Count)
            {
                HtmlElement tr = tbodyC.Children[ContainerList.Count];
                tr.Children[0].InvokeMember("click");
            }
            
        }
        #endregion

        #region //启动第三个抓取timer 抓取goodslimit详情
        bool detail_saved = true;
        bool limit_saved = false;
        bool limitVin_saved = false;
        bool existVin = true;
        public void timer4EventProcessor(object source, EventArgs e)
        {
            timer4.Interval = 1000;
            timer4.Stop();

            //while (webBrowser1.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();

            if (detail_saved)
            {
                GetDtailInfo(singleDetail);
                labmessage.Text = "表体数据抓取完成！";
                detail_saved = false;
                limit_flag = true;
            }
            if (limit_flag)
            {
                ClickGoodslimit();
                labmessage.Text = "正在抓取许可证数据！";
                limit_flag = false;
                limit_saved = true;
                timer4.Start();
                return;

            }
            if (limit_saved)
            {
                GetGoodslimitInfo(singleDetail);
                labmessage.Text = "许可证数据抓取完成！";
                limit_saved = false;
                if (detail_gno == 1 && existVin)
                {
                    labmessage.Text = "正在抓取许可证VIN 408数据！";
                    //点击弹出vin信息
                    ClickGoodslimitVin();
                    timer4.Start();
                    limitVin_saved = true;
                    return;
                }

                CloseLimitLayer();
            }
            //第一项才包含vin信息
            if (limitVin_saved && detail_gno == 1)
            {
                GetGoodslimitVinInfo(singleDetail);
                limitVin_saved = false;
                labmessage.Text = "许可证VIN 408数据，全部抓取完成！";
            }

            //点击下一条
            timer3.Start();
        }

        #endregion


      

        #region//点击产品资质 点击弹出vin信息
        bool limit_flag = true;
        /// <summary>
        /// 点击产品资质
        /// </summary>
        /// 
        public void ClickGoodslimit()
        {
            htmlDoc = webBrowser1.Document.Window.Frames[2].Document;
            //点击使用人加载信息
            htmlDoc.All["goodsLicenceBtn"].InvokeMember("click");

        }


        /// <summary>
        /// 点击弹出vin信息
        /// </summary>
        /// 
        public void ClickGoodslimitVin()
        {
            htmlDoc = webBrowser1.Document.Window.Frames[2].Document;

            //点击弹出vin信息
            htmlDoc.All["licenceVinBtn"].InvokeMember("click");
        }

        #endregion

        
        #region //关闭页面弹出框事件


        public void CloseDecDocContent()
        {
            //点击关闭弹窗使用人加载信息
            for (int i = 0; i < 100; i++)
            {
                //点击关闭
                HtmlElement layer = htmlDoc.GetElementById("layui-layer" + i);
                if (layer != null)
                {
                    HtmlElementCollection div = layer.GetElementsByTagName("div");


                    foreach (HtmlElement item in div)
                    {
                        if (item.InnerText == "随附单据编辑")
                        {

                            HtmlElementCollection dhl = item.Parent.GetElementsByTagName("A");
                            foreach (HtmlElement a in dhl)
                            {
                                if (a.InnerText == null)
                                {
                                    a.InvokeMember("click");
                                    return;
                                }

                            }
                        }

                    }
                }

            }
        }


        //原产地对应关系录入
        public void CloseOriginC()
        {
            //点击关闭弹窗使用人加载信息
            for (int i = 0; i < 100; i++)
            {
                //点击关闭
                HtmlElement layer = htmlDoc.GetElementById("layui-layer" + i);
                if (layer != null)
                {
                    HtmlElementCollection div = layer.GetElementsByTagName("div");


                    foreach (HtmlElement item in div)
                    {
                        if (item.InnerText == "原产地对应关系录入")
                        {

                            HtmlElementCollection dhl = item.Parent.GetElementsByTagName("A");
                            foreach (HtmlElement a in dhl)
                            {
                                if (a.InnerText == null)
                                {
                                    a.InvokeMember("click");
                                    return;
                                }

                            }
                        }

                    }
                }

            }
        }

        public void CloseUserInfo()
        {
            //点击关闭弹窗使用人加载信息
            for (int i = 0; i < 100; i++)
            {
                //点击关闭
                HtmlElement layer = htmlDoc.GetElementById("layui-layer" + i);
                if (layer != null)
                {
                    HtmlElementCollection div = layer.GetElementsByTagName("div");


                    foreach (HtmlElement item in div)
                    {
                        if (item.InnerText == "编辑使用人信息")
                        {

                            HtmlElementCollection dhl = item.Parent.GetElementsByTagName("A");
                            foreach (HtmlElement a in dhl)
                            {
                                if (a.InnerText == null)
                                {
                                    a.InvokeMember("click");
                                    return;
                                }

                            }
                        }

                    }
                }

            }
        }


        public void ClosepackageInfo()
        {
            //点击关闭弹窗使用人加载信息
            for (int i = 0; i < 100; i++)
            {
                //点击关闭
                HtmlElement layer = htmlDoc.GetElementById("layui-layer" + i);
                if (layer != null)
                {
                    HtmlElementCollection div = layer.GetElementsByTagName("div");


                    foreach (HtmlElement item in div)
                    {
                        if (item.InnerText == "编辑其他包装信息")
                        {

                            HtmlElementCollection dhl = item.Parent.GetElementsByTagName("A");
                            foreach (HtmlElement a in dhl)
                            {
                                if (a.InnerText == null)
                                {
                                    a.InvokeMember("click");
                                    return;
                                }

                            }
                        }

                    }
                }

            }
        }

        public void CloseDocReqBtn()
        {
            //点击关闭弹窗使用人加载信息
            for (int i = 0; i < 100; i++)
            {
                //点击关闭
                HtmlElement layer = htmlDoc.GetElementById("layui-layer" + i);
                if (layer != null)
                {
                    HtmlElementCollection div = layer.GetElementsByTagName("div");


                    foreach (HtmlElement item in div)
                    {
                        if (item.InnerText == "检验检疫签证申报要素")
                        {

                            HtmlElementCollection dhl = item.Parent.GetElementsByTagName("A");
                            foreach (HtmlElement a in dhl)
                            {
                                if (a.InnerText == null)
                                {
                                    a.InvokeMember("click");
                                    return;
                                }

                            }
                        }

                    }
                }

            }
        }

        #endregion




        #region//保存表头信息
        /// <summary>
        /// 保存表头信息
        /// </summary>
        /// <param name="singleWindow"></param>
        public void GetHeadInfo(SingleWindow singleWindow)
        {
            htmlDoc = webBrowser1.Document.Window.Frames[2].Document;

            #region 原产地证项号
            
            #endregion


            singleWindow.PortOfIeCode = htmlDoc.GetElementById("customMaster").GetAttribute("value");//申报地海关代码
            singleWindow.PortOfIeName = htmlDoc.GetElementById("customMasterName").GetAttribute("value");//申报地海关

            singleWindow.CustomsStatus = htmlDoc.GetElementById("customMaster").GetAttribute("value");//申报状态代码
            singleWindow.CustomsStatusDetail = htmlDoc.GetElementById("cusDecStatusName").GetAttribute("value");//申报状态

            String SeqNo = htmlDoc.GetElementById("cusCiqNo").GetAttribute("value");//统一编号
            if (singleWindow.SeqNo != SeqNo)
            {
                MessageBox.Show("该票报关单统一编号不匹配！系统：" + singleWindow.SeqNo + " 单一窗口：" + SeqNo);
                return;
            }
            singleWindow.SeqNo = htmlDoc.GetElementById("cusCiqNo").GetAttribute("value");//统一编号

            singleWindow.PreEntryNo = htmlDoc.GetElementById("preEntryId").GetAttribute("value");//预录入编号

            singleWindow.DfNo = htmlDoc.GetElementById("entryId").GetAttribute("value");//海关编号

            singleWindow.IePort = htmlDoc.GetElementById("iEPort").GetAttribute("value");//进境关别代码
            singleWindow.IePortName = htmlDoc.GetElementById("iEPortName").GetAttribute("value");//进境关别

            singleWindow.ContrNo = htmlDoc.GetElementById("contrNo").GetAttribute("value");//合同协议号

            singleWindow.DateOfIe = htmlDoc.GetElementById("iEDate").GetAttribute("value");//进口日期
            singleWindow.DateOfDeclaration = htmlDoc.GetElementById("dDate").GetAttribute("value");//申报日期

            singleWindow.TradeCoScc = htmlDoc.GetElementById("rcvgdTradeScc").GetAttribute("value");//境内收发货人91310000717883402X

            singleWindow.ProprietorCompanyCode = htmlDoc.GetElementById("rcvgdTradeCode").GetAttribute("value");//3114942023

            singleWindow.TradeCiqCode = htmlDoc.GetElementById("consigneeCode").GetAttribute("value");//3100623765

            singleWindow.ProprietorCompanyName = htmlDoc.GetElementById("consigneeCname").GetAttribute("value");//沃尔沃汽车销售(上海)有限公司



            singleWindow.OverseasConsignorCode = htmlDoc.GetElementById("consignorCode").GetAttribute("value");//境外收发货人SE5560743089
            singleWindow.OverseasConsignorEname = htmlDoc.GetElementById("consignorEname").GetAttribute("value");//VOLVO CAR CORPORATION

            singleWindow.OwnerCodeScc = htmlDoc.GetElementById("ownerScc").GetAttribute("value");//消费使用单位
            singleWindow.OwnerCode = htmlDoc.GetElementById("ownerCode").GetAttribute("value");//10位海关代码3114942023

            singleWindow.OwnerCiqCode = htmlDoc.GetElementById("ownerCiqCode").GetAttribute("value");//10位检验检疫编码3100623765
            singleWindow.OwnerName = htmlDoc.GetElementById("ownerName").GetAttribute("value");//企业名称 沃尔沃汽车销售(上海)有限公司

            singleWindow.AgentCodeScc = htmlDoc.GetElementById("agentScc").GetAttribute("value");//申报单位913101155587707842
            singleWindow.AgentCode = htmlDoc.GetElementById("agentCode").GetAttribute("value");//10位海关代码3116680002

            singleWindow.DeclCiqCode = htmlDoc.GetElementById("declRegNo").GetAttribute("value");//10位检验检疫编码3100910343
            singleWindow.AgentName = htmlDoc.GetElementById("agentName").GetAttribute("value");//企业名称 上海东泽国际物流有限公司

            singleWindow.TrafModeStd = htmlDoc.GetElementById("cusTrafMode").GetAttribute("value");//运输方式 2
            singleWindow.TrafModeStdName = htmlDoc.GetElementById("cusTrafModeName").GetAttribute("value");//水路运输

            singleWindow.TrafName = htmlDoc.GetElementById("trafName").GetAttribute("value");//运输工具名称
            singleWindow.VoyageNo = htmlDoc.GetElementById("cusVoyageNo").GetAttribute("value");//航次号

            singleWindow.BillNo = htmlDoc.GetElementById("billNo").GetAttribute("value");//提运单号
            singleWindow.TradeMethodCode = htmlDoc.GetElementById("supvModeCdde").GetAttribute("value");//监管方式0110

            singleWindow.TradeMethodName = htmlDoc.GetElementById("supvModeCddeName").GetAttribute("value");//    一般贸易
            singleWindow.NcCode = htmlDoc.GetElementById("cutMode").GetAttribute("value");//征免性质101

            singleWindow.NcName = htmlDoc.GetElementById("cutModeName").GetAttribute("value");//一般征税
            //singleWindow.LicenceNo = htmlDoc.GetElementById("licenseNo").GetAttribute("value");//许可证号

            singleWindow.TradeCountryStd = htmlDoc.GetElementById("cusTradeCountry").GetAttribute("value");//启运国(地区) SWE
            singleWindow.TradeCountryStdName = htmlDoc.GetElementById("cusTradeCountryName").GetAttribute("value");// 瑞典

            singleWindow.DistinatePortCode = htmlDoc.GetElementById("distinatePort").GetAttribute("value");//经停港 SWE300
            singleWindow.DistinatePortName = htmlDoc.GetElementById("distinatePortName").GetAttribute("value");//  瓦尔港（瑞典）

            singleWindow.TermsOfDeliveryCode = htmlDoc.GetElementById("transMode").GetAttribute("value");//成交方式 1
            singleWindow.TermsOfDeliveryName = htmlDoc.GetElementById("transModeName").GetAttribute("value");//  CIF

            singleWindow.TotalPiece = htmlDoc.GetElementById("packNo").GetAttribute("value");//件数 35
            singleWindow.WrapTypeStd = htmlDoc.GetElementById("wrapType").GetAttribute("value");//包装种类 00

            singleWindow.WrapTypeStdName = htmlDoc.GetElementById("wrapTypeName").GetAttribute("value");//散装

            singleWindow.TotalGrossWeight = htmlDoc.GetElementById("grossWt").GetAttribute("value");//毛重(KG) 82446

            singleWindow.TotalNetWeight = htmlDoc.GetElementById("netWt").GetAttribute("value");//净重(KG) 82446
            singleWindow.TradeCountryCode = htmlDoc.GetElementById("cusTradeNationCode").GetAttribute("value");//贸易国别(地区) SWE

            singleWindow.TradeCountryName = htmlDoc.GetElementById("cusTradeNationCodeName").GetAttribute("value");//  瑞典
            singleWindow.DocTypeCode1 = htmlDoc.GetElementById("attaDocuCdstr").GetAttribute("value");//随附单证 O

            singleWindow.EntyPortCode = htmlDoc.GetElementById("ciqEntyPortCode").GetAttribute("value");//入境口岸 310701
            singleWindow.EntyPortName = htmlDoc.GetElementById("ciqEntyPortCodeName").GetAttribute("value");// 外高桥

            singleWindow.GoodsPlace = htmlDoc.GetElementById("goodsPlace").GetAttribute("value");//货物存放地点 海通
            singleWindow.DespPortCode = htmlDoc.GetElementById("despPortCode").GetAttribute("value");//启运港 SWE300

            singleWindow.DespPortName = htmlDoc.GetElementById("despPortCodeName").GetAttribute("value");//  瓦尔港（瑞典）
            singleWindow.DecType = htmlDoc.GetElementById("entryType").GetAttribute("value");//报关单类型 M

            singleWindow.DecTypeName = htmlDoc.GetElementById("entryTypeName").GetAttribute("value");//通关无纸化
            singleWindow.LabelRemark = htmlDoc.GetElementById("noteS").GetAttribute("value");//备注

            singleWindow.MarkNo = htmlDoc.GetElementById("markNo").GetAttribute("value");//标记唛码
            singleWindow.OrgCode = htmlDoc.GetElementById("orgCode").GetAttribute("value");//检验检疫受理机关 310700

            singleWindow.OrgName = htmlDoc.GetElementById("orgCodeName").GetAttribute("value");//  上海外高桥机关本部
            singleWindow.DeclarationMaterialCode = htmlDoc.GetElementById("entQualifTypeCodeS").GetAttribute("value");//企业资质 101040

            singleWindow.EntQualifNo = htmlDoc.GetElementById("entQualifTypeCodeSName").GetAttribute("value");//  合格保证
            singleWindow.VsaOrgCode = htmlDoc.GetElementById("vsaOrgCode").GetAttribute("value");//领证机关 310700

            singleWindow.VsaOrgName = htmlDoc.GetElementById("vsaOrgCodeName").GetAttribute("value");//  上海外高桥机关本部
            singleWindow.InspOrgCode = htmlDoc.GetElementById("inspOrgCode").GetAttribute("value");//口岸检验检疫机关 310700

            singleWindow.InspOrgName = htmlDoc.GetElementById("inspOrgCodeName").GetAttribute("value");//  上海外高桥机关本部
            singleWindow.DespDate = htmlDoc.GetElementById("despDate").GetAttribute("value");//启运日期 2018-11-18

            singleWindow.BlLineNo = htmlDoc.GetElementById("ciqBillNo").GetAttribute("value");//B/L号 EUKOSECH1573917
            singleWindow.PurpOrgCode = htmlDoc.GetElementById("purpOrgCode").GetAttribute("value");//目的地检验检疫机关  310100

            singleWindow.PurpOrgName = htmlDoc.GetElementById("purpOrgCodeName").GetAttribute("value");//  上海浦江机关本部
            singleWindow.CorrelationNo = htmlDoc.GetElementById("correlationDeclNo").GetAttribute("value");//关联号码及理由 118000009163984

            singleWindow.CorrelationReasonFlag = htmlDoc.GetElementById("correlationReasonFlagName").GetAttribute("value");//关联理由



            //singleWindow.PortOfIeName = htmlDoc.GetElementById("preDecUserList").GetAttribute("value");//使用人[{"createUser":"2000040004242","updateTime":"2018-12-25 13:38:15","consumerUsrCode":"3100623765","userName":"沃尔沃汽车销售(上海)有限公司","consumerUsrSeqNo":"1","cusCiqNo":"I20180000154291887","useOrgPersonTel":"02169018529","useOrgPersonCode":"黄健","indbTime":"2018-12-25 13:38:15","updateUser":"2000040004242","state":true}]

            singleWindow.FileType = htmlDoc.GetElementById("appCertCode").GetAttribute("value");//  所需单证 98
            singleWindow.FileTypeName = htmlDoc.GetElementById("appCertName").GetAttribute("value");//  其他单
            singleWindow.RelationFlag = htmlDoc.GetElementById("promiseItem1").GetAttribute("value");//特殊关系确认 1 是

            singleWindow.PriceFlag = htmlDoc.GetElementById("promiseItem2").GetAttribute("value");//价格影响确认 0 否
            singleWindow.RoyaltyFlag = htmlDoc.GetElementById("promiseItem3").GetAttribute("value");//与货物有关的特许权使用费支付确认 0 否
            if (singleWindow.RelationFlag == "1" && singleWindow.PriceFlag == "0" && singleWindow.RoyaltyFlag == "0")
            {
                singleWindow.RprFlag = "是否否";
            }
            singleWindow.UpdateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            
            miBll.Edit(singleWindow);
        }

        #endregion


        #region // 保存表体detail
        /// <summary>
        /// 保存表体信息
        /// </summary>
        /// <param name="singleDetail"></param>
        public void GetDtailInfo(SingleDetail singleDetail)
        {
            htmlDoc = webBrowser1.Document.Window.Frames[2].Document;

            singleDetail.GNo = htmlDoc.GetElementById("gNo").GetAttribute("value");//项号
            //singleDetail.ContrItem = htmlDoc.GetElementById("contrItem").GetAttribute("value");//备案序号
            String hscode = htmlDoc.GetElementById("codeTs").GetAttribute("value");//商品编号
            singleDetail.CustomCode = hscode.Substring(0, 8);
            singleDetail.AppendCode = hscode.Substring(8, 2);
            singleDetail.CiqCode = htmlDoc.GetElementById("ciqCode").GetAttribute("value");//检验检疫名称 999

            singleDetail.CiqName = htmlDoc.GetElementById("ciqName").GetAttribute("value");//检验检疫名称
            singleDetail.GoodsName = htmlDoc.GetElementById("gName").GetAttribute("value");//商品名称

            singleDetail.Spec = htmlDoc.GetElementById("gModel").GetAttribute("value");//规格型号
            singleDetail.ValuationQty = htmlDoc.GetElementById("gQty").GetAttribute("value");//成交数量 申报数量

            singleDetail.ValuationUnitCode = htmlDoc.GetElementById("gUnit").GetAttribute("value");//成交计量单位 003
            singleDetail.ValuationUnitName = htmlDoc.GetElementById("gUnitName").GetAttribute("value");// 辆

            singleDetail.UnitPrice = htmlDoc.GetElementById("declPrice").GetAttribute("value");//单价
            singleDetail.UnitTotalPrice = htmlDoc.GetElementById("declTotal").GetAttribute("value");//总价

            singleDetail.TradeCurrStd = htmlDoc.GetElementById("tradeCurr").GetAttribute("value");//币制 CNY
            singleDetail.CurrencyCode = htmlDoc.GetElementById("tradeCurr").GetAttribute("value");//币制 CNY
            singleDetail.CurrencyName = htmlDoc.GetElementById("tradeCurrName").GetAttribute("value");// 人民币

            singleDetail.LegalQty1 = htmlDoc.GetElementById("qty1").GetAttribute("value") == "" ? "0" : htmlDoc.GetElementById("qty1").GetAttribute("value");//法定第一数量
            singleDetail.LegalUnit1Code = htmlDoc.GetElementById("unit1").GetAttribute("value");//法定第一计量单位 003

            singleDetail.LegalUnit1Name = htmlDoc.GetElementById("unit1Name").GetAttribute("value");//  辆

            singleDetail.DestinationCountryStd = htmlDoc.GetElementById("destinationCountry").GetAttribute("value");//最终目的国(地区) CHN  
            singleDetail.DestinationCountryStdName = htmlDoc.GetElementById("destinationCountryName").GetAttribute("value");//中国  

            singleDetail.LegalQty2 = htmlDoc.GetElementById("qty2").GetAttribute("value") == "" ? "0" : htmlDoc.GetElementById("qty2").GetAttribute("value");//法定第二数量
            singleDetail.LegalUnit2Code = htmlDoc.GetElementById("unit2").GetAttribute("value");//法定第二计量单位 003

            singleDetail.GrossWeight = htmlDoc.GetElementById("qty2").GetAttribute("value") == "" ? "0" : htmlDoc.GetElementById("qty2").GetAttribute("value");//毛重
            singleDetail.NetWeight = htmlDoc.GetElementById("qty2").GetAttribute("value") == "" ? "0" : htmlDoc.GetElementById("qty2").GetAttribute("value");//净重

            singleDetail.LegalUnit2Name = htmlDoc.GetElementById("unit2Name").GetAttribute("value");//  辆
            singleDetail.OriginCountryStd = htmlDoc.GetElementById("cusOriginCountry").GetAttribute("value");//原产国(地区) SWE

            singleDetail.OriginCountryStdName = htmlDoc.GetElementById("cusOriginCountryName").GetAttribute("value");//  瑞典

            singleDetail.DistrictCode = htmlDoc.GetElementById("districtCode").GetAttribute("value");//境内目的地 31149
            singleDetail.DistrictName = htmlDoc.GetElementById("districtCodeName").GetAttribute("value");//嘉定

            singleDetail.DestCode = htmlDoc.GetElementById("ciqDestCode").GetAttribute("value");//境内目的地 310114
            singleDetail.DestName = htmlDoc.GetElementById("ciqDestCodeName").GetAttribute("value");//上海市嘉定区

            singleDetail.NcDetailCode = htmlDoc.GetElementById("dutyMode").GetAttribute("value");//征免方式 1
            singleDetail.NcDetailName = htmlDoc.GetElementById("dutyModeName").GetAttribute("value");//照章征税

            String model = htmlDoc.GetElementById("goodsTargetInput").GetAttribute("value");
            if (model!="")
            {
                singleDetail.GoodsSpec = model.Split(';')[0].Trim();//检验检疫货物规格
                singleDetail.GoodsBrand = model.Split(';')[1];//检验检疫货物规格
                singleDetail.ProdBatchNo = model.Split(';')[2];//检验检疫货物规格
            }
            //singleDetail.GoodsSpec = htmlDoc.GetElementById("goodsTargetInput").GetAttribute("value");//检验检疫货物规格
            singleDetail.GoodsAttr = htmlDoc.GetElementById("goodsAttr").GetAttribute("value");//货物属性

            singleDetail.GoodsAttrName = htmlDoc.GetElementById("goodsAttrName").GetAttribute("value");//3C目录内
            singleDetail.Purpose = htmlDoc.GetElementById("purpose").GetAttribute("value");//用途 99

            singleDetail.PurposeName = htmlDoc.GetElementById("purposeName").GetAttribute("value");// 其他

            singleDetail.CountryOfOriginCode = htmlDoc.GetElementById("cusOriginCountry").GetAttribute("value");//原产国(地区) SWE

            singleDetail.CountryOfOriginName = htmlDoc.GetElementById("cusOriginCountryName").GetAttribute("value");//  瑞典

            singleDetail.Batch = list[0].Batch;

            singleDetail.CustDecHeadId = list[0].Id;
            singleDetail.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            singleDetail.UpdateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                //查询是否存在 存在更新  不存在插入
                List<SingleDetail> exsitList = miBll.GetDetailList(singleDetail);
                if (exsitList.Count > 0)
                {
                    miBll.UpdateDetail(singleDetail);
                    singleDetail.Id = exsitList[0].Id;
                }
                else
                {
                    singleDetail.Id = miBll.InsertSingleDetail(singleDetail);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存detail出现异常！" + ex);
                stopProcess();
            }



        }


        #endregion




        #region //保存许可证信息
        /// <summary>
        /// 保存许可证信息
        /// </summary>
        /// <param name="singleDetail"></param>
        public void GetGoodslimitInfo(SingleDetail singleDetail)
        {
            htmlDoc = webBrowser1.Document.Window.Frames[2].Document;
            SingleGoodslimit singleGoodslimit = new SingleGoodslimit();
            //获取隐藏域的值json
            //String goodslimit_json = htmlDoc.GetElementById("preDecCiqGoodsLimit").GetAttribute("value");//[{"licenceNo":"2018011101112349","gNo":"2","createUser":"2000040004242","goodsLimitSeqNo":"1","updateTime":"2018-12-25 13:38:15","licTypeName":"强制性产品认证（CCC认证）证书","preDecCiqGoodsLimitVinVo":"[]","licTypeCodeName":"强制性产品认证（CCC认证）证书","cusCiqNo":"I20180000154291887","licTypeCode":"411","indbTime":"2018-12-25 13:38:15","updateUser":"2000040004242","state":true}]

            HtmlElement table = htmlDoc.GetElementById("decListLicenceTable");
            HtmlElement tbody = table.Children[1];

            int rows = tbody.Children.Count;
            if (rows == 1)
            {
                existVin = false;
            }
            else {
                existVin = true;
            }
            //保存3CCC证明书编号
            singleDetail.CarCccNo = tbody.Children[0].Children[4].InnerText;
            miBll.UpdateDetail(singleDetail);
            //tr = tbody.Children[0];
            foreach (HtmlElement tr in tbody.Children)
            {
                if (tr.Children[2].InnerText.ToString() == "408")
                {
                    tr.Children[0].InvokeMember("click");
                }
                singleGoodslimit.GoodsNo = singleDetail.GNo;
                singleGoodslimit.LicTypeCode = tr.Children[2].InnerText.ToString();
                singleGoodslimit.LicTypeName = tr.Children[3].InnerText.ToString();
                singleGoodslimit.LicenceNo = tr.Children[4].InnerText.ToString();

                singleGoodslimit.CustDecHeadId = singleDetail.CustDecHeadId;
                singleGoodslimit.CustDecDetailId = singleDetail.Id;
                singleGoodslimit.Batch = singleDetail.Batch;
                singleGoodslimit.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                singleGoodslimit.UpdateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                try
                {
                    //查询是否存在 存在更新  不存在插入
                    List<SingleGoodslimit> exsitList = miBll.GetGoodslimitList(singleGoodslimit);
                    if (exsitList.Count > 0)
                    {
                        miBll.UpdateSingleGoodslimit(singleGoodslimit);
                    }
                    else
                    {
                        miBll.InsertSingleGoodslimit(singleGoodslimit);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存许可证信息出现异常！" + ex);
                    stopProcess();
                }
            }

        }

        #endregion




        #region //保存许可证VIN信息

        /// <summary>
        /// 保存许可证VIN
        /// </summary>
        /// <param name="singleDetail"></param>
        public void GetGoodslimitVinInfo(SingleDetail singleDetail)
        {
            SingleGoodslimitVin singleGoodslimitVin = new SingleGoodslimitVin();
            //获取隐藏域的值json
            //String goodslimit_json = htmlDoc.GetElementById("preDecCiqGoodsLimit").GetAttribute("value");//[{"licenceNo":"2018011101112349","gNo":"2","createUser":"2000040004242","goodsLimitSeqNo":"1","updateTime":"2018-12-25 13:38:15","licTypeName":"强制性产品认证（CCC认证）证书","preDecCiqGoodsLimitVinVo":"[]","licTypeCodeName":"强制性产品认证（CCC认证）证书","cusCiqNo":"I20180000154291887","licTypeCode":"411","indbTime":"2018-12-25 13:38:15","updateUser":"2000040004242","state":true}]
            //HtmlElement licenceVinContent = htmlDoc.GetElementById("licenceVinContent");
            singleGoodslimitVin.LicenceNo = htmlDoc.GetElementById("licenceNo").GetAttribute("value");
            singleGoodslimitVin.LicTypeName = htmlDoc.GetElementById("licTypeCodeVinName").GetAttribute("value");
            singleGoodslimitVin.LicTypeCode = "408";
            HtmlElement table = htmlDoc.GetElementById("licenceVinTable");
            HtmlElement tbody = table.Children[1];

            int rows = tbody.Children.Count;

            //tr = tbody.Children[0];
            foreach (HtmlElement tr in tbody.Children)
            {
                
                singleGoodslimitVin.VinNo = tr.Children[1].InnerText.ToString();
                singleGoodslimitVin.BillLadDate = tr.Children[2].InnerText.ToString();
                singleGoodslimitVin.QualityQgp = tr.Children[3].InnerText.ToString();
                singleGoodslimitVin.MotorNo = tr.Children[4].InnerText.ToString();

                singleGoodslimitVin.VinCode = tr.Children[5].InnerText.ToString();
                singleGoodslimitVin.ChassisNo = tr.Children[6].InnerText.ToString();
                singleGoodslimitVin.InvoiceNo = tr.Children[7].InnerText.ToString();
                singleGoodslimitVin.InvoiceNum = tr.Children[8].InnerText.ToString();

                singleGoodslimitVin.ProdCnnm = tr.Children[9].InnerText.ToString();
                singleGoodslimitVin.ProdEnnm = tr.Children[10].InnerText.ToString();
                singleGoodslimitVin.ModelEn = tr.Children[11].InnerText.ToString();//型号
                singleGoodslimitVin.PricePerUnit = tr.Children[12].InnerText.ToString();


                singleGoodslimitVin.CustDecHeadId = singleDetail.CustDecHeadId;
                singleGoodslimitVin.CustDecDetailId = singleDetail.Id;
                singleGoodslimitVin.Batch = singleDetail.Batch;
                singleGoodslimitVin.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                singleGoodslimitVin.UpdateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                try
                {
                    //查询是否存在 存在更新  不存在插入
                    List<SingleGoodslimitVin> exsitList = miBll.GetGoodslimitListVin(singleGoodslimitVin);
                    if (exsitList.Count > 0)
                    {
                        miBll.UpdateSingleGoodslimitVin(singleGoodslimitVin);
                    }
                    else
                    {
                        miBll.InsertSingleGoodslimitVin(singleGoodslimitVin);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存许可证VIN信息出现异常！" + ex);
                    stopProcess();
                }

            }

            CloseVinLayer();
            CloseLimitLayer();
        }

        #endregion





        #region //关闭弹窗
        /// <summary>
        /// 关闭许可证vin弹窗
        /// </summary>
        public void CloseVinLayer()
        {
            //点击关闭弹窗使用人加载信息
            for (int i = 0; i < 30; i++)
            {
                //点击关闭
                HtmlElement layer = htmlDoc.GetElementById("layui-layer" + i);
                if (layer != null)
                {
                    HtmlElementCollection div = layer.GetElementsByTagName("div");


                    foreach (HtmlElement item in div)
                    {
                        if (item.InnerText == "编辑许可证VIN")
                        {

                            HtmlElementCollection dhl = item.Parent.GetElementsByTagName("A");
                            foreach (HtmlElement a in dhl)
                            {
                                if (a.InnerText == null)
                                {
                                    a.InvokeMember("click");
                                    i = 31;
                                    break;
                                }

                            }
                            break;
                        }

                    }
                }

            }
        }



        /// <summary>
        /// 关闭许可证弹窗
        /// </summary>
        public void CloseLimitLayer()
        {
            //点击关闭弹窗使用人加载信息
            for (int i = 0; i < 30; i++)
            {
                //点击关闭
                HtmlElement layer = htmlDoc.GetElementById("layui-layer" + i);
                if (layer != null)
                {
                    HtmlElementCollection div = layer.GetElementsByTagName("div");


                    foreach (HtmlElement item in div)
                    {
                        if (item.InnerText == "编辑产品许可证/审批/备案信息")
                        {

                            HtmlElementCollection dhl = item.Parent.GetElementsByTagName("A");
                            foreach (HtmlElement a in dhl)
                            {
                                if (a.InnerText == null)
                                {
                                    a.InvokeMember("click");
                                    i = 31;
                                    break;
                                }

                            }
                            break;
                        }

                    }
                }

            }
        }



        /// <summary>
        /// 关闭弹出frame
        /// </summary>
        public void CloseFrame()
        {
            //点击关闭
            htmlDoc = webBrowser1.Document;
            HtmlElement layer = htmlDoc.GetElementById("page-wrapper");
            if (layer != null)
            {
                HtmlElementCollection div = layer.GetElementsByTagName("A");


                foreach (HtmlElement item in div)
                {
                    if (item.InnerText != null && item.InnerText.ToString().Trim() == "进口报关单整合申报" + list[0].SeqNo)
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

        #endregion




        #region  //停止抓取
        private void button2_Click(object sender, EventArgs e)
        {
            stopProcess();
        }
        #endregion




        #region //加载浏览器
        private void button3_Click(object sender, EventArgs e)
        {
            var appName = Process.GetCurrentProcess().ProcessName + ".exe";
            SetIE11KeyforWebBrowserControl(appName);
            webBrowser1.ScriptErrorsSuppressed = true;
            //进入主界面
            this.webBrowser1.Url = new Uri("https://app.singlewindow.cn/cas/login?service=https%3A%2F%2Fswapp.singlewindow.cn%2Fdeskserver%2Fj_spring_cas_security_check&logoutFlag=1&_swCardF=1");
            //等待加载完毕
            while (webBrowser1.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();

            htmlDoc = this.webBrowser1.Document;
            //HtmlElementCollection dhl = htmlDoc.GetElementsByTagName("A");
            //foreach (HtmlElement item in dhl)
            //{
            //    if (item.InnerText == "卡介质")
            //    {
            //        item.InvokeMember("click");
            //    }

            //}

            if (timer1 != null)
            {
                timer1.Dispose();
            }
            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 4000;
            timer1.Enabled = true;
            timer1.Tick += new EventHandler(timer1EventProcessor);//添加事件
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
                label3.Text = "当前浏览器版本：" + FindAppkey;

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
                    label3.Text = "当前浏览器版本：" + FindAppkey;
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



       
        #region  //查询
        private void button4_Click(object sender, EventArgs e)
        {
            search();
        }

        public void search() {
            SingleWindow singleWindow = new SingleWindow();
            String batch = comboBox1.Text;
            singleWindow.Batch = batch;
            List<SingleWindow> dvlist = miBll.GetList(singleWindow);
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = dvlist;
        }
        #endregion


        /// <summary>
        /// 格式化数据
        /// </summary>
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.Value != null && e.Value != "") //哪一列
            {
                if (Convert.ToInt32(e.Value) == 1)
                {
                    e.Value = "抓取完成！";
                    e.CellStyle.ForeColor = Color.Green;
                }
                else if (Convert.ToInt32(e.Value) == 0)
                {
                    e.Value = "待抓取";
                    e.CellStyle.ForeColor = Color.Red;
                }
                else {
                    e.Value = "暂不处理";
                    e.CellStyle.ForeColor = Color.Red;
                }
            }
        }

        public void stopProcess()
        {

            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            timer4.Stop();
        }


        /// <summary>
        /// 查询原产地证号 开启写入报检号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {

            SingleWindow singleWindow = new SingleWindow();
            String batch = comboBox1.Text;
            if (batch == "")
            {
                MessageBox.Show("请输入批次号！");
                return;
            }
            singleWindow.Batch = batch;
            list = miBll.GetCiqList(singleWindow);
            flag = 3;
            timer2.Start();
        }


        //选择原产地证
        public int selectOrigin()
        {
            HtmlElement table1 = htmlDoc.GetElementById("decLicenseTable");
            HtmlElement tbody1 = table1.Children[1];
            HtmlElement trr = null;
            //判断是否有原产地证
            if (tbody1.Children.Count > 1)
            {
                trr = tbody1.Children[1];
            }
            //原产地证
            if (trr != null)
            {
                list[0].DocTypeCode2 = trr.Children[1].InnerText.ToString(); //单证代码
                list[0].DocNo2 = trr.Children[2].InnerText.ToString();//原产地证编号
                //原产地证项号
                HtmlElement decTable = htmlDoc.GetElementById("decLicenseTable");
                //判断是否有原产地证
                if (decTable.Children.Count > 1)
                {
                    HtmlElement doc = decTable.Children[1];
                    HtmlElement docDetail = doc.Children[1];
                    foreach (HtmlElement item in docDetail.Children)
                    {
                        if (item.InnerText == "Y")
                        {
                            //点击原产地证
                            item.InvokeMember("click");
                            return 1;
                        }
                    }
                }
                else
                {
                    return 0;
                }
            }
            return 2;
        }

        public bool addCiqNo()
        {
            HtmlElement DeclNo = htmlDoc.GetElementById("correlationDeclNo");
            HtmlElement addHeadBtn = htmlDoc.GetElementById("addHeadBtn");
            if (DeclNo != null && addHeadBtn != null )
            {
                DeclNo.SetAttribute("value", list[0].CiqDeclNo);
                addHeadBtn.InvokeMember("click");
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool getDecDocBtn()
        {
            HtmlElement DeclNo = htmlDoc.GetElementById("decDocBtn");
            if (DeclNo != null)
            {
                DeclNo.InvokeMember("click");
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// 获取报关单电子委托书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            SingleWindow singleWindow = new SingleWindow();
            String batch = comboBox1.Text;
            if (batch == "")
            {
                MessageBox.Show("请输入批次号！");
                return;
            }
            singleWindow.Batch = batch;
            list = miBll.GetDecDocList(singleWindow);
            flag = 4;
            timer2.Start();
        }




        private void button8_Click(object sender, EventArgs e)
        {
            String batch = comboBox1.Text;
            String state = comboBox2.SelectedValue.ToString();
            String dfNos = "";

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if ((Convert.ToBoolean(dataGridView1.Rows[i].Cells[0].Value) == true))
                {
                    dfNos += "'" + dataGridView1.Rows[i].Cells[1].Value.ToString() + "',";
                }
                else
                    continue;
            }
            if (!dfNos.Equals(""))
            {
                dfNos = dfNos.Substring(0, dfNos.Length - 1);
                SingleWindow singleWindow = new SingleWindow();
                singleWindow.Batch = batch;
                singleWindow.ClientSeqNo = state;
                singleWindow.SeqNo = dfNos;
                miBll.UpdateHeadState(singleWindow);
            }
            search();
            checkBox1.Checked = false;

        }

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
            else
            {

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

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            SolidBrush b = new SolidBrush(this.dataGridView1.RowHeadersDefaultCellStyle.ForeColor);
            e.Graphics.DrawString((e.RowIndex + 1).ToString(System.Globalization.CultureInfo.CurrentUICulture), this.dataGridView1.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
        }

    }
}
