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
using BLL.CicAlone;
using System.Data.OleDb;

namespace BgCustomsState
{
    public partial class CisAloneForm : Form
    {
        public HtmlDocument htmlDoc;
        public List<CarInfo> list = new List<CarInfo>();
        public List<CarInfo> detailIdlist;
        public List<CarInfo> vinlist;
        public List<CarInfo> vinlistCopy;
        public List<CarInfo> getVinList = new List<CarInfo>();
        public List<CarInfo> UpdateCerNoList = new List<CarInfo>();
        public int controlStep = 0;
        public HtmlElement tr = null;
        public int detailnum = 0;
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

        public int decCount = 0;//报关单项数总计
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
        CarInfo carInfo = new CarInfo();
        public String model = "";
        public String brand = "";
        //创建业务逻辑层对象
        CarAloneBill miBll = new CarAloneBill();
        CicAloneBill ciBll = new CicAloneBill();
        public CisAloneForm()
        {
            InitializeComponent();
            //禁用按钮
            setButtonEnable(true);
        }


        private void initBatch()
        {
            List<CarInfo> cblist = miBll.GetList();
            comboBox1.DataSource = cblist;
            comboBox1.DisplayMember = "Batch";
            comboBox1.ValueMember = "Batch";

            comboBox4.DataSource = cblist;
            comboBox4.DisplayMember = "Batch";
            comboBox4.ValueMember = "Batch";
        }

        /// <summary>
        /// 初始化登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CisAloneForm_Load(object sender, EventArgs e)
        {
            //設置當前文檔為百度
            //this.webBrowser4.Url = new Uri("http://www.haiguan.info/onlinesearch/gateway/Gatewaystate.aspx");

            //wb.Navigate("http://cic.chinaport.gov.cn/cic/pageframe/mainFrame.action");
            //webBrowser4.ScriptErrorsSuppressed = true;
            initBatch();

            IList<Info> infoList = new List<Info>();
            Info info1 = new Info() { Id = "10", Name = "暂不处理" };
            Info info2 = new Info() { Id = "0", Name = "待写入" };
            Info info3 = new Info() { Id = "7", Name = "写入完成" };
            Info info4 = new Info() { Id = "8", Name = "抓取完成" };
            Info info5 = new Info() { Id = "9", Name = "提交完成" };
            infoList.Add(info1);
            infoList.Add(info2);
            infoList.Add(info3);
            infoList.Add(info4);
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

        public class Info
        {
            public string Id { get; set; }
            public string Name { get; set; }

        }







        /// <summary>
        /// 点击进入查询界面 开启timer
        /// </summary>
        /// <param name="carInfo"></param>
        private void  loginStep()
        {
            //禁用按钮
            setButtonEnable(false);


            htmlDoc = this.webBrowser4.Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("请确认是否已启动浏览器？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }

            htmlDoc = webBrowser4.Document.Window.Frames["leftFrame"].Document;
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
                if (item.InnerText == "录入车辆证明书签发申请")
                {
                    item.InvokeMember("click");
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
            carInfo.Batch = batch;
            carInfo.Df_no = "";
            if (dataGridView1.SelectedRows.Count>0)
            {
                carInfo.Df_no = dataGridView1.SelectedCells[1].Value.ToString();
            }
            if (flag == 1&&carInfo.Df_no == "")
            {
                carInfo.SingleState = "7";
            }
            if (flag == 2 && carInfo.Df_no == "")
            {
                carInfo.SingleState = "8";
            }
            if (flag == 3)//删除证明书
            {
                carInfo.SingleState = "9";
            }
            if (flag == 4)
            {
                carInfo.SingleState = "9";
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
                CarInfo carinfo1 = new CarInfo();
                carinfo1.Batch = batch;
                carinfo1.Df_no = dfNoStr;
                carinfo1.SingleState = carInfo.SingleState;
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
                if (flag == 1 && MessageBox.Show("当前将会帮您自动上传" + list.Count + "票报关单的车辆证明书数据,请确认无误后继续上传！", "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }

            }
            timer1.Interval = 2000;
            timer1.Start();
        }






        /// <summary>
        /// timer事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void timer1EventProcessor(object source, EventArgs e)
        {
            if (textBox1.Text!="")
            {
                int speed = Convert.ToInt32(textBox1.Text);
                timer2Flash = speed * 1000;
                timer3Flash = speed * 1000;
                labelItem2.Text = "--速度：" + textBox1.Text + " 秒";
            }
            timer1.Stop();
            //抓取
            if (flag == 2)
            {
                if (!dfNoStr.Equals(""))
                {
                    CarInfo carinfo1 = new CarInfo();
                    carinfo1.Batch = carInfo.Batch;
                    carinfo1.Df_no = dfNoStr;
                    carinfo1.SingleState = "8";
                    list = miBll.selectChecked(carinfo1);
                }
                else
                {
                    //多个客户端交替抓取
                    carInfo.SingleState = "8";
                    list = miBll.GetList(carInfo);

                }

                if (list.Count > 0)
                {
                    list[0].SingleState = "8";
                    miBll.UpdateHeadState(list[0]);
                }
            }
            if (flag == 4)
            {
                if (!dfNoStr.Equals(""))
                {
                    CarInfo carinfo1 = new CarInfo();
                    carinfo1.Batch = carInfo.Batch;
                    carinfo1.Df_no = dfNoStr;
                    carinfo1.SingleState = "9";
                    list = miBll.selectChecked(carinfo1);
                }
                else
                {
                    //多个客户端交替查询
                    carInfo.SingleState = "9";
                    list = miBll.GetList(carInfo);

                }

                if (list.Count > 0)
                {
                    list[0].SingleState = "9";
                    miBll.UpdateHeadState(list[0]);
                }
            }
            if (flag == 1 && carInfo.Df_no == "")
            {
                if (!dfNoStr.Equals(""))
                {
                    CarInfo carinfo1 = new CarInfo();
                    carinfo1.Batch = carInfo.Batch;
                    carinfo1.Df_no = dfNoStr;
                    carinfo1.SingleState = "7";
                    list = miBll.selectChecked(carinfo1);
                }
                else
                {
                    carInfo.SingleState = "7";
                    list = miBll.GetList(carInfo);

                }
                
                if (list.Count > 0)
                {
                    list[0].SingleState = "7";
                    miBll.UpdateHeadState(list[0]);
                }
            }
            //等待加载完毕
            while (webBrowser4.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
            number1++;
            System.Diagnostics.Debug.WriteLine(DateTime.Now + "   timer1EventProcessor*******************************************************第" + number1 + "票报关单===");
            if (list.Count > 0)
            {
                //当报关单号是N/A时停止
                if (list[0].Df_no.Length<10)
                {
                    lblmessage.Text = "导入完成！请抓取数据进行比对！";
                    timer1.Stop();
                    timer2.Stop();
                    timer3.Stop();
                    MessageBox.Show("恭喜你！该批次车辆证明书数据导入完成！");
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }
                //根据报关单号获取detail id
                detailIdlist = miBll.GetListByDfno(list[0]);
                //记录系统报关单数据一票共包含几项
                getDetailCount = detailIdlist.Count;

                if (detailIdlist.Count > 0)
                {
                    //输入报关单号页面查询事件
                    if (putDfnoInfoForSearch(list[0].Df_no))
                    {
                        //解禁按钮
                        setButtonEnable(true);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("根据报关单号没有查询到对应的detail项数详情信息！");
                    return;
                }


                bl = true;
                timer2.Start();

            }
            else
            {
                dfNoStr = "";

                if (flag == 1)
                {
                    lblmessage.Text = "导入完成！请抓取数据进行比对！";
                    System.Diagnostics.Debug.WriteLine(DateTime.Now + "   timer1EventProcessor*******************************************************共" + list.Count + "票报关单数据导入finish完成====");
                    timer1.Stop();
                    timer2.Stop();
                    timer3.Stop();
                    if (repeatFlag)
                    {
                        flag = 2;
                        timer1.Start();
                        repeatFlag = false;
                        return;
                    }
                    MessageBox.Show("恭喜你！该批次车辆证明书数据导入完成！");
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }
                if (flag == 2)
                {
                    System.Diagnostics.Debug.WriteLine(DateTime.Now + "   timer1EventProcessor*******************************************************共" + list.Count + "票报关单数据抓取finish完成====");
                    timer1.Stop();
                    timer2.Stop();
                    timer3.Stop();
                    MessageBox.Show("恭喜你！该批次车辆证明书数据抓取完成！");
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }

                if (flag == 3)
                {
                    System.Diagnostics.Debug.WriteLine(DateTime.Now + "   timer1EventProcessor*******************************************************共" + list.Count + "票报关单数据删除finish完成====");
                    timer1.Stop();
                    timer2.Stop();
                    timer3.Stop();
                    MessageBox.Show("恭喜你！该批次车辆证明书数据删除完成！");
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }

                if (flag == 4)
                {
                    System.Diagnostics.Debug.WriteLine(DateTime.Now + "   timer1EventProcessor*******************************************************共" + list.Count + "票报关单数据提交finish完成====");
                    timer1.Stop();
                    timer2.Stop();
                    timer3.Stop();
                    MessageBox.Show("恭喜你！该批次车辆证明书数据提交完成！");
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }
            }


        }




        /// <summary>
        /// 点击查询报关单号
        /// </summary>
        /// <param name="dfNo"></param>
        private bool putDfnoInfoForSearch(String dfNo)
        {
            htmlDoc = webBrowser4.Document.Window.Frames["mainFrame"].Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("没有获取报关单查询的frame，请确认是否已登录？");
                //解禁按钮
                setButtonEnable(true);
                return true;
            }
            htmlDoc.All["ID_entryId"].SetAttribute("value", dfNo);
            htmlDoc.All["entryIdButtn"].InvokeMember("click");
            return false;
        }



        #region //timer2 获取数据校验
        bool bl = true;
        public void timer2EventProcessor(object source, EventArgs e)
        {
            
            try{
            SendKeys.Send("{enter}");
            //SendKeys.Send("{ESC}");
            number2++;
            timer2.Stop();
            //等待加载完毕
            while (webBrowser4.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();

            if (detailIdlist.Count > 0)
            {
                //timer2重复开启 只允许获取一次vinlist
                if (bl)
                {
                    number5++;
                    //根据detail id 获取dec_list数据
                    vinlist = miBll.GetListByDetailId(detailIdlist[0]);
                    vinlistCopy = new List<CarInfo>();
                    vinlistCopy.AddRange(vinlist);
                    detailnum = vinlist.Count;
                    if (vinlist.Count == 0)
                    {
                        //解禁按钮
                        setButtonEnable(true);
                        MessageBox.Show("当前报关单列表明细没有查询到对应的车辆证明书信息,批量导入中断！");
                        lblmessage.Text = "当前报关单列表明细没有查询到对应的车辆证明书信息,批量导入中断！";
                        return;
                    }
                    bl = false;
                    System.Diagnostics.Debug.WriteLine(DateTime.Now + "    timer2EventProcessor**********************************************第" + number5 + "项********************************" + vinlist.Count);
                }

                //校验单一窗口与系统数据是否对应 每票共有几项 每项detail包含数量是否一致 
                if (vinlist.Count > 0)
                {
                    seqno = Convert.ToInt32(vinlist[0].Seq_no);
                    //依次点击报关单列表明细事件
                    int itemFlag=tableItemClick(seqno, detailnum);
                    if (itemFlag == 1 || itemFlag == 3)
                    {
                        //解禁按钮
                        setButtonEnable(true);
                        bl = true;
                        //detailIdlist.Remove(detailIdlist[0]);
                        list[0].SingleState = "7";
                        miBll.UpdateHeadState(list[0]);
                        timer1.Interval = 2 * timer1.Interval;
                        timer1.Start();
                        lblmessage.Text = "明细列表查询未显示，正在再次查询！timer1:" + timer1.Interval;
                        return;
                    }
                    if (itemFlag ==2)
                    {
                        lblmessage.Text = "该票报关单商品项号及数量与系统不一致！程序停止！";
                        return;
                    }
                    timer1.Interval=2000;
                    timer3.Start();
                   
                }
                else
                {
                    curVin = 0;//当前条数计数器
                    bl = true;
                    detailIdlist.Remove(detailIdlist[0]);
                    timer2.Start();
                }
                System.Diagnostics.Debug.WriteLine(DateTime.Now + "   操作二  timer2EventProcessor事件执行第---" + number2 + "---次------------该项中未导入车辆剩余数量--" + vinlist.Count);


            }
            else
            {
                if (flag == 1)//导入证明书
                {
                    list[0].SingleState = "7";
                }
                else if (flag == 2)//抓取证明书
                {
                    list[0].SingleState = "8";
                }
                else if (flag == 3)//删除证明书
                {
                    list[0].SingleState = "0";
                }
                else if (flag == 4)//提交证明书
                {
                    list[0].SingleState = "9";
                }
                miBll.UpdateHeadState(list[0]);
                list.Remove(list[0]);
                bl = true;
                timer1.Start();
                timer2.Stop();
                timer3.Stop();

                number2 = 0;
                number3 = 0;
                number4 = 0;
                number5 = 0;
            }

            }
            catch (Exception ex)
            {
                timer2.Stop();
                timer3.Stop();
                if (MessageBox.Show("当前程序点击当前项出现异常，是否继续！" + ex.Message.ToString(), "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }
                timer1.Start();
               
                bl = true;
            }
        }
        #endregion


        int delFlag = 0;
        #region //timer3 导入 抓取 删除 提交
        /// <summary>
        /// timer3
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void timer3EventProcessor(object source, EventArgs e)
        {
            try { 
            
            timer3.Stop();
                //等待加载完毕
                while (webBrowser4.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
            number3++;


            if (flag == 1)
            {
                SendKeys.Send("{ESC}");
                lblmessage.Text = "正在写入！当前报关单第 ："+seqno+" 项";
                if (delFlag > 0)
                {


                    HtmlElement table = htmlDoc.GetElementById("ID_tableData");
                    HtmlElement tbody = table.Children[1];
                    int rows = tbody.Children.Count;
                    for (int i = 0; i < rows; i++)
                    {
                        HtmlElement tr = tbody.Children[i];
                        String number = tr.Children[0].InnerText.ToString();

                        if (rows - 1 == i)
                        {
                            if (int.Parse(number) == delFlag - 1)
                            {
                                delFlag = 0;
                            }
                        }

                        if (delFlag != 0)
                        {
                            tr.InvokeMember("click");
                            HtmlElement deletebtn = htmlDoc.GetElementById("delButtn");
                            if (deletebtn != null && deletebtn.Enabled)
                            {
                                
                                SendKeys.Send("{ENTER}");
                                deletebtn.InvokeMember("click");
                                SendKeys.Send("{ESC}");
                                timer3.Start();
                                return;
                            }

                        }
                        
                    }

                    bl = true;
                    delFlag = 0;
                    //进入末页
                    HtmlElement fistHref = htmlDoc.GetElementById("fistHref");
                    if (fistHref != null)
                    {
                        fistHref.InvokeMember("click");//一个bug  当一票里的两项申报数量一致时 第一项抓取后要点击首页
                    }
                    timer2.Start();
                    return;
                }

                //支持续传
                if (vinlist.Count==vinlistCopy.Count)
                {
                    //移除
                    HtmlElement nextpage = htmlDoc.GetElementById("hasNextHref");
                    HtmlElement table = htmlDoc.GetElementById("ID_tableData");
                    HtmlElement tbody = table.Children[1];
                    //只有第一页
                    if (nextpage == null)
                    {
                        int rows = tbody.Children.Count;
                        if (rows>0)
                        {
                            for (int i = 0; i < rows; i++)
                            {
                                HtmlElement tr = tbody.Children[i];
                                String number = tr.Children[0].InnerText.ToString();
                                String barcode = tr.Children[9].InnerText.ToString();
                                if (vinlist[0].Row_no.Equals(number) && vinlist[0].Barcode.Equals(barcode))
                                {
                                    vinlist.Remove(vinlist[0]);
                                    vinlistCopy.Remove(vinlistCopy[0]);
                                }
                                else {
                                    delFlag = int.Parse(vinlist[0].Row_no.ToString());
                                    timer3.Start();
                                    return;
                                }
                            
                            }
                        }
                    
                    
                    }
                    else
                    {
                        //存在下一页
                        for (int i = 0; i < 10; i++)
                        {
                            HtmlElement tr = tbody.Children[i];
                            String number = tr.Children[0].InnerText.ToString();
                            String barcode = tr.Children[9].InnerText.ToString();
                            if (vinlist[0].Row_no.Equals(number) && vinlist[0].Barcode.Equals(barcode))
                            {
                                vinlist.Remove(vinlist[0]);
                                vinlistCopy.Remove(vinlistCopy[0]);
                            }
                            else
                            {
                                delFlag = int.Parse(vinlist[0].Row_no.ToString());
                                timer3.Start();
                                return;
                            }
                        

                        }

                        htmlDoc.All["hasNextHref"].InvokeMember("click");
                        timer3.Start();
                        return;
                    }

                }
                else {
                    //进入末页
                    HtmlElement endHref = htmlDoc.GetElementById("endHref");
                    if (endHref != null)
                    {
                        endHref.InvokeMember("click");//一个bug  当一票里的两项申报数量一致时 第一项抓取后要点击首页

                    }

                    htmlDoc = webBrowser4.Document.Window.Frames["mainFrame"].Document;
                    HtmlElement table1 = htmlDoc.GetElementById("ID_tableData");
                    if (table1 != null)
                    {
                        HtmlElement tbody = table1.Children[1];
                        if (tbody.Children.Count > 0)
                        {
                            int rows = tbody.Children.Count;
                           
                            for (int i = 0; i < rows; i++)
                            {
                              //比对上一行
                                if (i == rows - 1)
                                {
                                    HtmlElement tr = tbody.Children[i];
                                    String number = tr.Children[0].InnerText.ToString();
                                    String barcode = tr.Children[9].InnerText.ToString();
                                    //此处判断上一条是否保存成功正确
                                    if (vinlistCopy[0].Row_no.Equals(number) && vinlistCopy[0].Barcode.Equals(barcode))
                                    {
                                        //正确的话 移除上一条
                                        vinlistCopy.Remove(vinlistCopy[0]);
                                    }
                                    else
                                    {
                                        //不等删除最后一条
                                        delFlag = int.Parse(number);
                                        timer3.Start();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }


               

               

                
               

                #region 原始导入检查逻辑
                


                //从一开始校验一次该列表明细是否已导入过车辆证明书  没有导入过为正常 decCount为申报数量
                //if (vinlist.Count == decCount)
                //{
                //    htmlDoc = webBrowser4.Document.Window.Frames["mainFrame"].Document;
                //    HtmlElement table = htmlDoc.GetElementById("ID_tableData");
                //    HtmlElement tbody = table.Children[1];
                //    int rows = tbody.Children.Count;
                //    if (!rows.Equals(0))
                //    {
                        
                //        HtmlElement nextpage = htmlDoc.GetElementById("hasNextHref");
                //        //只有第一页
                //        if (nextpage == null)
                //        {

                //            for (int i = 0; i < rows;i++ )
                //            {
                //                HtmlElement tr = tbody.Children[i];
                //                String number = tr.Children[0].InnerText.ToString();
                //                String barcode = tr.Children[9].InnerText.ToString();
                //                if (vinlist[0].Row_no.Equals(number) && vinlist[0].Barcode.Equals(barcode))
                //                {
                //                    vinlist.Remove(vinlist[0]);
                //                }
                //                else {

                //                    delFlag = rows - i ;
                //                    timer3.Start();
                //                    return;
                //                }
                //            }
                //            //保存完点击回到首页
                //            HtmlElement firstpage = htmlDoc.GetElementById("fistHref");
                //            if (firstpage != null)
                //            {
                //                firstpage.InvokeMember("click");//一个bug  当一票里的两项申报数量一致时 第一项抓取后要点击首页
                //            }
                //        }
                //        else
                //        {
                //            //存在下一页
                //            for (int i = 0; i < 10; i++)
                //            {
                //                HtmlElement tr = tbody.Children[i];
                //                String number = tr.Children[0].InnerText.ToString();
                //                String barcode = tr.Children[9].InnerText.ToString();
                //                if (vinlist[0].Row_no.Equals(number) && vinlist[0].Barcode.Equals(barcode))
                //                {
                //                    vinlist.Remove(vinlist[0]);
                //                    decCount = decCount - 1;
                //                }
                //                else
                //                {

                //                    delFlag = 10 - i;
                //                    timer3.Start();
                //                    return;
                //                }
                                
                //            }
                //            htmlDoc.All["hasNextHref"].InvokeMember("click");
                //            timer3.Start();
                //            return;
                //        }
                        

                //    }


                //}
                #endregion
                SendKeys.Send("{ENTER}");


                    if (vinlist.Count > 0)
                {
                    curVin++;
                    lblmessage.Text = "正在录入报关单号为" + list[0].Df_no + "的第" + seqno + "项 ，第" + curVin + "条数据！";
                    //int itemFlag = tableItemClick(seqno, detailnum);
                    SendKeys.Send("{ESC}");
                    tr.InvokeMember("click");
                    AddCarinfo(vinlist[0]);
                    vinlist.Remove(vinlist[0]);
                    
                }

                timer2.Start();
            }
            else if (flag == 2)
            {
                lblmessage.Text = "正在抓取！当前报关单" + list[0].Df_no + "第 ：" + seqno + " 项";
                GetCarinfo(detailIdlist[0]);
            }
            else if (flag == 3)
            {
                SendKeys.Send("{enter}");
                lblmessage.Text = "正在删除！当前报关单第 ：" + seqno + " 项";
                DeleteCarinfo();
            }
            else if (flag == 4)
            {
                lblmessage.Text = "正在提交！当前报关单第 ：" + seqno + " 项";
                CommiteCarinfo();
            }
            else
            {
                MessageBox.Show("暂时无任何操作，定时循环停止！");
                return;
            }

            }
            catch (Exception ex)
            {
                if (MessageBox.Show("当前程序出现异常，是否继续！" + ex.Message.ToString(), "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    //解禁按钮
                    setButtonEnable(true);
                    return;
                }
                timer2.Start();
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

            htmlDoc = webBrowser4.Document.Window.Frames["mainFrame"].Document;

            HtmlElement table = htmlDoc.GetElementById("ID_entryData");
            if (table == null)
            {
                MessageBox.Show("没有获取到报关单详情列表！");
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
                MessageBox.Show("该票报关单包含项数与单一窗口项数不一致！单一窗口数量：" + rows + "系统查询数量" + getDetailCount);
                return 2;
            }
           
            int seqNo = Convert.ToInt32(tr.Children[0].InnerText.ToString());
            decCount = Convert.ToInt32(tr.Children[5].InnerText.ToString());
            brand = tr.Children[2].InnerText.ToString();//获取品名
            model = tr.Children[3].InnerText.ToString();//获取申报要素
            if (seqNo != seq_no)
            {
                MessageBox.Show("该票报关单商品项号与单一窗口商品项号不一致！单一窗口数量：" + seqNo + "系统查询数量" + seq_no);
                timer1.Stop();
                return 2;
            }
            if (decCount != decNum)
            {
                MessageBox.Show("该票报关单申报数量与单一窗口申报数量不一致！单一窗口数量：" + decCount + "系统查询数量" + decNum);
                timer1.Stop();
                return 2;
            }
            SendKeys.Send("{enter}");
            SendKeys.Send("{enter}");
            tr.InvokeMember("click");
            System.Diagnostics.Debug.WriteLine(DateTime.Now + "     操作一  tableItemClick事件执行第---" + number4 + "---次------------该项中未导入车辆剩余数量--" + vinlist.Count);
            return 0;
        }
        #endregion


        #region //导入证明书操作
        /// <summary>
        /// 自动录入证明书信息
        /// </summary>
        /// <param name="num"></param>
        /// <param name="decNum"></param>
        private void AddCarinfo(CarInfo carinfo)
        {

            htmlDoc = webBrowser4.Document.Window.Frames["mainFrame"].Document;
            HtmlElement el = htmlDoc.GetElementById("CIC_energyType");
            switch (carinfo.Spec)
            {
                case "汽柴油型":
                    el.SetAttribute("selectedIndex", "1");
                    break;
                case "汽油型":
                    el.SetAttribute("selectedIndex", "1");
                    break;
                case "混合动力型":
                    el.SetAttribute("selectedIndex", "2");
                    break;

                case "纯电动型":
                    el.SetAttribute("selectedIndex", "3");
                    carinfo.Engine_type = "";
                    break;
                case "其他":
                    el.SetAttribute("selectedIndex", "4");
                    break;
            }
            if (carinfo.Production_date == null)
            {
                MessageBox.Show("出厂日期不能为空！导入停止！");
                //解禁按钮
                setButtonEnable(true);
                timer1.Stop();
                timer2.Stop();
                timer3.Stop();
                return;
            }
            String year = carinfo.Production_date.Substring(0, 4);
            String month = carinfo.Production_date.Substring(4, 2);
            String colour = carinfo.Color.Replace("/","");
            int colorLenth = colour.Length;
            if (colorLenth == 1)
            {
                setColor("CIC_color1", colour);
            }
            else if (colorLenth == 2)
                {
                    String color1 = colour.Substring(0, 1);
                    String color2 = colour.Substring(1, 1);
                    setColor("CIC_color1", color1);
                    setColor("CIC_color2", color2);
                }
                else if (colorLenth == 3)
                    {
                        String color1 = colour.Substring(0, 1);
                        String color2 = colour.Substring(1, 1);
                        String color3 = colour.Substring(2, 1);
                        setColor("CIC_color1", color1);
                        setColor("CIC_color2", color2);
                        setColor("CIC_color3", color3);
                    }
                    else
                    {
                        MessageBox.Show("填入颜色时截取字段出现异常！" + carinfo.Color);
                        timer1.Stop();
                        return;
                    }

            htmlDoc.All["CIC_engineNo"].SetAttribute("value", carinfo.Engine_type);
            htmlDoc.All["CIC_qtyExhaust"].SetAttribute("value", carinfo.Displacement);
            htmlDoc.All["CIC_elecMotorNo"].SetAttribute("value", carinfo.Motor_no);
            htmlDoc.All["CIC_elecMotorPower"].SetAttribute("value", carinfo.Motor_power);

            htmlDoc.All["CIC_year"].SetAttribute("value", year);
            htmlDoc.All["CIC_month"].SetAttribute("value", month);
            htmlDoc.All["CIC_carCoverNo"].SetAttribute("value", carinfo.Barcode);

            SendKeys.Send("{ESC}");
            htmlDoc.All["saveButtn"].InvokeMember("click");



            String line = DateTime.Now + "     操作四  AddCarinfo事件执行第---" + number3 + "---次------------该项中未导入车辆剩余数量--" + vinlist.Count +
                "/n/r" + carinfo.Barcode + "  " + carinfo.Engine_type + "  " + carinfo.Color + "  " + carinfo.Production_date + "  " + carinfo.Displacement + "  " + carinfo.Motor_power + "  " + carinfo.Motor_no;
            System.Diagnostics.Debug.WriteLine(line);

            System.Diagnostics.Debug.WriteLine("导入后当前剩余数量---------" + vinlist.Count);
        }





        /// <summary>
        /// 填充颜色封装方法
        /// </summary>
        /// <param name="colorID"></param>
        /// <param name="color"></param>
        private void setColor(String colorID, String color)
        {
            HtmlElement el2 = htmlDoc.GetElementById(colorID);
            switch (color)
            {
                case "白":
                    el2.SetAttribute("selectedIndex", "1");
                    break;
                case "灰":
                    el2.SetAttribute("selectedIndex", "2");
                    break;

                case "黄":
                    el2.SetAttribute("selectedIndex", "3");
                    break;
                case "粉":
                    el2.SetAttribute("selectedIndex", "4");
                    break;
                case "红":
                    el2.SetAttribute("selectedIndex", "5");
                    break;
                case "紫":
                    el2.SetAttribute("selectedIndex", "6");
                    break;

                case "绿":
                    el2.SetAttribute("selectedIndex", "7");
                    break;
                case "蓝":
                    el2.SetAttribute("selectedIndex", "8");
                    break;
                case "棕":
                    el2.SetAttribute("selectedIndex", "9");
                    break;
                case "黑":
                    el2.SetAttribute("selectedIndex", "10");
                    break;


            }
        }

        #endregion


        #region //抓取证明书数据
        /// <summary>
        /// 抓取单一窗口车辆证明书数据
        /// </summary>
        /// <param name="carinfo"></param>
        private void GetCarinfo(CarInfo carInfo)
        {
            timer3.Stop();
            //等待加载完毕
            while (webBrowser4.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
            htmlDoc = webBrowser4.Document.Window.Frames["mainFrame"].Document;
            HtmlElement table = htmlDoc.GetElementById("ID_tableData");
            HtmlElement tbody = table.Children[1];
            int rows = tbody.Children.Count;
            if (rows.Equals(0))
            {
                //if (MessageBox.Show("当前报关单" + list[0].Df_no + " 第" + seqno + "项暂无数据是否继续抓取！", "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                //{
                //    //解禁按钮
                //    setButtonEnable(true);
                //    return;
                //}
                ht_dfNo.Add(list[0].Df_no, "");//第一次没有写入证明书的报关单号存入map记录
                lblmessage.Text = "当前报关单" + list[0].Df_no + " 第" + seqno + "项暂无数据！";
                if (list.Count > 0)
                {
                    list[0].SingleState = "7";
                    miBll.UpdateHeadState(list[0]);
                }
                bl = true;
                timer2.Start();
                repeatFlag = true;
                flag = 1;
                return;
            }
            for (int i = 0; i < rows; i++)
            {
                CarInfo carinfo = new CarInfo();
                HtmlElement tr = tbody.Children[i];
                carinfo.Seq_no = (i + 1) + "";
                if (tr != null)
                {
                    carinfo.Seq_no = tr.Children[0].InnerText == null ? "" : tr.Children[0].InnerText.ToString();
                    carinfo.CopNo = tr.Children[1].InnerText == null ? "" : tr.Children[1].InnerText.ToString();
                    carinfo.CarType = tr.Children[2].InnerText == null ? "" : tr.Children[2].InnerText.ToString();
                    carinfo.Spec = tr.Children[3].InnerText == null ? "" : tr.Children[3].InnerText.ToString();
                    carinfo.Engine_type = tr.Children[4].InnerText == null ? "" : tr.Children[4].InnerText.ToString();
                    carinfo.Displacement = tr.Children[5].InnerText == null ? "" : tr.Children[5].InnerText.ToString();
                    carinfo.Motor_no = tr.Children[6].InnerText == null ? "" : tr.Children[6].InnerText.ToString();
                    carinfo.Motor_power = tr.Children[7].InnerText == null ? "" : tr.Children[7].InnerText.ToString();
                    carinfo.Color = tr.Children[8].InnerText == null ? "" : tr.Children[8].InnerText.ToString();
                    carinfo.Barcode = tr.Children[9].InnerText == null ? "" : tr.Children[9].InnerText.ToString();
                    carinfo.Production_date = tr.Children[10] == null ? "" : tr.Children[10].InnerText.ToString();
                    carinfo.Statu = tr.Children[11].InnerText == null ? "" : tr.Children[11].InnerText.ToString();
                }
                carinfo.Model = model;
                carinfo.Brand = brand;
                carinfo.Df_no = list[0].Df_no;
                carinfo.detailSum = vinlist.Count;
                carinfo.create_date = DateTime.Now;
                carinfo.update_date = DateTime.Now;
                carinfo.Batch = carInfo.Batch;
                carinfo.Detail_id = carInfo.Detail_id;
                carinfo.Cust_dec_head_id = carInfo.Cust_dec_head_id;
                getVinList.Add(carinfo);
            }
            getVinCount += rows;
            //页面抓取车架号数量大于等于该项包含总数
            if (getVinCount >= vinlist.Count)
            {
                try
                {
                    for (int i = 0; i < getVinList.Count; i++)
                    {
                        bool b = miBll.UpdateCarInfo(getVinList[i]);
                    }
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show("当前程序出现异常，是否继续！" + ex.Message.ToString(), "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        //解禁按钮
                        setButtonEnable(true);
                        return;
                    }
                    timer2.Start();
                }
                detailIdlist.Remove(detailIdlist[0]);
                getVinCount = 0;
                getVinList.Clear();
                bl = true;
                //htmlDoc.All["entryIdButtn"].InvokeMember("click");
                HtmlElement firstpage = htmlDoc.GetElementById("fistHref");
                if (firstpage != null)
                {
                    htmlDoc.All["fistHref"].InvokeMember("click");//一个bug  当一票里的两项申报数量一致时 第一项抓取后要点击首页
                }
                timer2.Start();
            }
            else
            {
                HtmlElement nextpage = htmlDoc.GetElementById("hasNextHref");
                if (nextpage == null)
                {
                    //if (MessageBox.Show("当前报关单" + list[0].Df_no + " 第" + seqno + "项数据不全，需导入补全，是否跳过继续抓取！", "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    //{
                    //    //解禁按钮
                    //    setButtonEnable(true);
                    //    return;
                    //}
                    lblmessage.Text = "当前报关单" + list[0].Df_no + " 第" + seqno + "项数据不全！";
                    if (!ht_dfNo.ContainsKey(list[0].Df_no))
                    {
                    ht_dfNo.Add(list[0].Df_no,"");
                    }
                    if (list.Count > 0)
                    {
                        list[0].SingleState = "7";
                        miBll.UpdateHeadState(list[0]);
                    }
                    bl = true;
                    timer2.Start();
                    repeatFlag = true;
                    flag = 1;
                    return;

                    //detailIdlist.Remove(detailIdlist[0]);
                    //getVinCount = 0;
                    //getVinList.Clear();
                    //bl = true;
                    ////htmlDoc.All["entryIdButtn"].InvokeMember("click");
                    //timer2.Start();
                }
                else
                {
                    htmlDoc.All["hasNextHref"].InvokeMember("click");
                    timer3.Start();
                }

            }

        }
        #endregion


        #region //删除车辆证明书
        /// <summary>
        /// 删除车辆证明书数据
        /// </summary>
        private void DeleteCarinfo()
        {
            SendKeys.Send("{enter}");
            timer3.Stop();

            htmlDoc = webBrowser4.Document.Window.Frames["mainFrame"].Document;
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
                bl = true;
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
                deletebtn.InvokeMember("click");
                timer3.Start();
            }
            else {
                //Thread t = new Thread(TimerStart);//创建了线程还未开启
                //t.Start();//用来给函数传递参数，开启线程
                SendKeys.Send("{ENTER}");
                tr.InvokeMember("click");
                timer3.Start();
            }
        }
        #endregion





        //thread开启线程要求：该方法参数只能有一个，且是object类型
		public void TimerStart(){
            Thread.Sleep(3000);
            timer3.Start();
		}





        #region //提交车辆证明书数据
        int commiteFlag = 0;
        /// <summary>
        /// 提交车辆证明书数据
        /// </summary>
        private void CommiteCarinfo()
        {
            timer3.Stop();
            htmlDoc = webBrowser4.Document.Window.Frames["mainFrame"].Document;
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
                bl = true;
                timer2.Start();
                return;
            }


            
           


            HtmlElement submitbtn = htmlDoc.GetElementById("applyButtn");
            HtmlElement copyButtn = htmlDoc.GetElementById("copyButtn");
            if (submitbtn != null && submitbtn.Enabled && copyButtn.Enabled)
            {
                SendKeys.Send("{enter}");
                SendKeys.Send("{ESC}");
                submitbtn.InvokeMember("click");
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
                        bl = true;
                        timer2.Start();
                        return;
                    }
                    commiteFlag++;
                    SendKeys.Send("{enter}");
                    SendKeys.Send("{ESC}");
                    tr.InvokeMember("click");
                    timer3.Start();
                }
                else {

                    HtmlElement nextpage = htmlDoc.GetElementById("hasNextHref");
                    if (nextpage == null || !nextpage.Enabled)
                    {
                        detailIdlist.Remove(detailIdlist[0]);
                        bl = true;
                        HtmlElement firstpage = htmlDoc.GetElementById("fistHref");
                        if (firstpage != null)
                        {
                            htmlDoc.All["fistHref"].InvokeMember("click");//一个bug  当一票里的两项申报数量一致时 第一项抓取后要点击首页
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


        private void webBrowser4_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            mshtml.IHTMLDocument2 doc = (webBrowser4.ActiveXInstance as SHDocVw.WebBrowser).Document as mshtml.IHTMLDocument2;
            doc.parentWindow.execScript("window.alert=null", "javascript");
            doc.parentWindow.execScript("window.confirm=null", "javascript");
            doc.parentWindow.execScript("window.open=null", "javascript");
            doc.parentWindow.execScript("window.showModalDialog=null", "javascript");
            doc.parentWindow.execScript("window.close=null", "javascript");

        }



        private void InjectAlertBlocker()
        {
            //自动点击弹出确认或弹出提示
            IHTMLDocument2 vDocument = (IHTMLDocument2)webBrowser4.Document.DomDocument;
            vDocument.parentWindow.execScript("function confirm(str){return true;} ", "javascript"); //弹出确认
            vDocument.parentWindow.execScript("function alert(str){return true;} ", "javaScript");//弹出提示
        }

        //必须要在这个事件里面调用才能禁用，之前一直写在别的事件里面，浪费了2小时。
        private void webBrowser4_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            InjectAlertBlocker();

        }

        //阻止弹出新窗口
        private void webBrowser4_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

        }

        #endregion

        public enum ShowCommands : int
        {
            SW_HIDE = 0,
            SW_SHOWNOrmAL = 1,
            SW_NOrmAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }

        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, ShowCommands nShowCmd);

        #region//启动浏览器
        /// <summary>
        /// 加载浏览器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            //清除IE临时文件
            ShellExecute(IntPtr.Zero, "open", "rundll32.exe", " InetCpl.cpl,ClearMyTracksByProcess 8", "", ShowCommands.SW_HIDE);
            Application.DoEvents();

            var appName = Process.GetCurrentProcess().ProcessName + ".exe";
            SetIE7KeyforWebBrowserControl(appName);
            this.webBrowser4.Url = new Uri("http://cic.chinaport.gov.cn/cic/pageframe/mainFrame.action");
            webBrowser4.ScriptErrorsSuppressed = true;
            //等待加载完毕
            while (webBrowser4.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();

            htmlDoc = this.webBrowser4.Document;
            if (htmlDoc.All["password"] != null)
            {
                htmlDoc.All["password"].SetAttribute("value", "88888888");
                htmlDoc.All["loginbuttonCa"].InvokeMember("click");
            }
            

            //解禁按钮
            setButtonEnable(true);
        }

        private void SetIE7KeyforWebBrowserControl(string appName)
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
                //if (FindAppkey == "7000")//11000
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
                        Regkey.SetValue(appName, unchecked((int)0x1B58), RegistryValueKind.DWord);//ie99
                        ieVersion = "7000";
                        break;
                    default:
                        break;
                }
                // If a key is not present add the key, Key value 8000 (decimal)
               
                //Regkey.SetValue(appName, unchecked((int)0x1B58), RegistryValueKind.DWord);//ie7
                //Regkey.SetValue(appName, unchecked((int)0x2AF8), RegistryValueKind.DWord);//ie11
                // Check for the key after adding
                FindAppkey = Convert.ToString(Regkey.GetValue(appName));

                if (FindAppkey == ieVersion)
                {
                    MessageBox.Show("本次设置成功");
                    labelItem2.Text = "当前浏览器版本：" + FindAppkey + "--速度：" + timer2Flash/1000;
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



        #region //点击事件相关
        /// <summary>
        /// 点击开始事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            flag = 1;
            loginStep();

        }




        /// <summary>
        /// 抓取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

            ht_dfNo.Clear();
            flag = 2;
            loginStep();
            timer3.Interval = timer3Flash;
        }






        /// <summary>
        /// 查询报关单详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            search();
        }


        public void search()
        {
            CarInfo carInfo = new CarInfo();
            String batch = comboBox1.Text;
            carInfo.Batch = batch;
            List<CarInfo> dvlist = miBll.GetList(carInfo);
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
            loginStep();
            timer3.Interval = timer3Flash;
        }


        /// <summary>
        /// 提交数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要提交吗？", "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            flag = 4;
            loginStep();
            timer3.Interval = timer3Flash;
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
            if (timer1!=null)
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
        }

        #endregion



        #region //检查提交相关
        private void button9_Click(object sender, EventArgs e)
        {
            htmlDoc = this.webBrowser4.Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("请确认是否已启动浏览器？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            htmlDoc = webBrowser4.Document.Window.Frames["leftFrame"].Document;
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
                if (item.InnerText == "批量提交车辆证明书签发申请")
                {
                    item.InvokeMember("click");
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
            carInfo.Batch = batch;
            carInfo.SingleState = "10";
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
                CarInfo carinfo1 = new CarInfo();
                carinfo1.Batch = batch;
                carinfo1.Df_no = dfNoStr;
                carinfo1.SingleState = "10";
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
        /// 点击查询报关单号
        /// </summary>
        /// <param name="dfNo"></param>
        private bool putInfoForSearch(String dfNo)
        {
            htmlDoc = webBrowser4.Document.Window.Frames["mainFrame"].Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("没有获取报关单查询的frame，请确认是否已登录？");
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
                    item.InvokeMember("click");
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
                //输入报关单号页面查询事件
                if (putInfoForSearch(list[0].Df_no))
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
            htmlDoc = webBrowser4.Document.Window.Frames["mainFrame"].Document;

            HtmlElement table = htmlDoc.GetElementById("tableDate");
            if (table == null)
            {
                MessageBox.Show("没有获取到报关单详情列表！");
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
                    CarInfo carinfo1 = new CarInfo();
                    carinfo1.Batch = carInfo.Batch;
                    carinfo1.SingleState = "8";
                    carinfo1.Df_no = "'" + list[0].Df_no + "'";
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
                CarInfo carinfo1 = new CarInfo();
                carinfo1.Batch = batch;
                carinfo1.SingleState = state;
                carinfo1.Df_no = dfNos;
                miBll.UpdateHeadState1(carinfo1);
            }
            search();
            checkBox1.Checked = false;
        }

        #endregion


        #region //批量提交相关
        private void button10_Click(object sender, EventArgs e)
        {
            htmlDoc = this.webBrowser4.Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("请确认是否已启动浏览器？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            htmlDoc = webBrowser4.Document.Window.Frames["leftFrame"].Document;
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
                if (item.InnerText == "批量提交车辆证明书签发申请")
                {
                    item.InvokeMember("click");
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
            carInfo.Batch = batch;
            carInfo.SingleState = "9";
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
                CarInfo carinfo1 = new CarInfo();
                carinfo1.Batch = batch;
                carinfo1.Df_no = dfNoStr;
                carinfo1.SingleState = "9";
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
                CarInfo carinfo1 = new CarInfo();
                carinfo1.Batch = carInfo.Batch;
                carinfo1.Df_no = dfNoStr;
                carinfo1.SingleState = "9";
                list = miBll.selectChecked(carinfo1);
            }
            else
            {
                //多个客户端交替查询
                carInfo.SingleState = "9";
                list = miBll.GetList(carInfo);

            }
            if (list.Count > 0)
            {
                list[0].SingleState = "9";
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
            //输入报关单号页面查询事件
            if (putInfoForSearch(list[0].Df_no))
            {
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            lblmessage.Text = list[0].Df_no;
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

            htmlDoc = webBrowser4.Document.Window.Frames["mainFrame"].Document;
            
            HtmlElement table = htmlDoc.GetElementById("tableDate");
            if (table == null)
            {
                MessageBox.Show("没有获取到报关单详情列表！");
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

                        selectAll.InvokeMember("click");
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
                            item.InvokeMember("click");
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

            htmlDoc = this.webBrowser4.Document;
            if (htmlDoc == null)
            {
                MessageBox.Show("请确认是否已启动浏览器？");
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            htmlDoc = webBrowser4.Document.Window.Frames["leftFrame"].Document;
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
                if (item.InnerText == "查询车辆证明书签发申请")
                {
                    item.InvokeMember("click");
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
            carInfo.Batch = batch;
            carInfo.SingleState = "10";
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
                CarInfo carinfo1 = new CarInfo();
                carinfo1.Batch = batch;
                carinfo1.Df_no = dfNoStr;
                carinfo1.SingleState = "10";
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
                CarInfo carinfo1 = new CarInfo();
                carinfo1.Batch = carInfo.Batch;
                carinfo1.Df_no = dfNoStr;
                carinfo1.SingleState = "11";
                list = miBll.selectChecked(carinfo1);
            }
            else
            {
                //多个客户端交替查询
                carInfo.SingleState = "11";
                list = miBll.GetList(carInfo);

            }
            if (list.Count > 0)
            {
                list[0].SingleState = "11";
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
            //输入报关单号页面查询事件
            if (putInfoForSearch(list[0].Df_no))
            {
                //解禁按钮
                setButtonEnable(true);
                return;
            }
            lblmessage.Text = list[0].Df_no;
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
            htmlDoc = webBrowser4.Document.Window.Frames["mainFrame"].Document;
            HtmlElement table = htmlDoc.GetElementById("tableDate");
            HtmlElement tbody = table.Children[1];
            int rows = tbody.Children.Count;
            if (rows.Equals(0))
            {
                if (MessageBox.Show("当前报关单" + list[0].Df_no + "暂无数据是否继续抓取！", "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
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
                CarInfo carinfo = new CarInfo();
                HtmlElement tr = tbody.Children[i];
                if (tr != null)
                {
                    carinfo.Barcode = tr.Children[9].InnerText == null ? "" : tr.Children[9].InnerText.ToString();
                    carinfo.cer_no = tr.Children[5].InnerText == null ? "" : tr.Children[5].InnerText.ToString();
                    carinfo.Statu = tr.Children[11].InnerText == null ? "" : tr.Children[11].InnerText.ToString();
                }
                carinfo.Df_no = list[0].Df_no;
                carinfo.Batch = carInfo.Batch;
                carinfo.update_date = DateTime.Now;
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
                        htmlDoc.All["fistHref"].InvokeMember("click");//一个bug  当一票里的两项申报数量一致时 第一项抓取后要点击首页
                    }
                    timer9.Start();
                }
                else
                {
                    htmlDoc.All["hasNextHref"].InvokeMember("click");
                    timer10.Start();
                }
            }
        }

        //excel导入
        private void button12_Click(object sender, EventArgs e)
        {
            if (this.comboBox4.Text==null || "".Equals(this.comboBox4.Text.ToString().Trim()))
            {
                MessageBox.Show("请输入批次号！");
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "表格文件 (*.xls,*.xlsx)|*.xls;*.xlsx";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
               DataTable dt= ReadExcelToTable(openFileDialog.FileName);
                if (dt!=null)
                {
                    String enterid = dt.Rows[0][3].ToString();
                    List<CarInfo> listCars = new List<CarInfo>();
                    if ("EnterId".Equals(dt.Rows[0][3].ToString())
                        && "CarCoverNo".Equals(dt.Rows[0][7].ToString())
                        && dt.Columns.Count == 21)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (i == 0) continue;
                            if (dt.Rows[i][7] == null || dt.Rows[i][7].ToString().Trim().Equals(""))
                            {
                                continue;
                            }
                            CarInfo car = new CarInfo();
                            car.Batch = this.comboBox4.Text;
                            if (dt.Rows[i][3] == null || dt.Rows[i][3].ToString().Trim().Equals(""))
                            {
                                MessageBox.Show("excel第" + i + 1 + "行第4列数据为空,请检查！");
                                return;
                            }
                            if (dt.Rows[i][4] == null || dt.Rows[i][4].ToString().Trim().Equals(""))
                            {
                                MessageBox.Show("excel第" + i+1 + "行第5列数据为空,请检查！");
                                return;
                            }
                            if (dt.Rows[i][5] == null || dt.Rows[i][5].ToString().Trim().Equals(""))
                            {
                                MessageBox.Show("excel第" + i + 1 + "行第6列数据为空,请检查！");
                                return;
                            }
                            if (dt.Rows[i][6] == null || dt.Rows[i][6].ToString().Trim().Equals(""))
                            {
                                MessageBox.Show("excel第" + i + 1 + "行第8列数据为空,请检查！");
                                return;
                            }
                            if (dt.Rows[i][8] == null || dt.Rows[i][8].ToString().Trim().Equals(""))
                            {
                                MessageBox.Show("excel第" + i + 1 + "行第8列数据为空,请检查！");
                                return;
                            }
                            if (dt.Rows[i][9] == null || dt.Rows[i][9].ToString().Trim().Equals(""))
                            {
                                MessageBox.Show("excel第" + i + 1 + "行第10列数据为空,请检查！");
                                return;
                            }
                            if (dt.Rows[i][10] == null || dt.Rows[i][10].ToString().Trim().Equals(""))
                            {
                                MessageBox.Show("excel第" + i + 1 + "行第11列数据为空,请检查！");
                                return;
                            }
                            if (dt.Rows[i][13] == null || dt.Rows[i][13].ToString().Trim().Equals(""))
                            {
                                MessageBox.Show("excel第" + i + 1 + "行第14列数据为空,请检查！");
                                return;
                            }
                            car.Df_no = dt.Rows[i][3].ToString();
                            car.Detail_id = dt.Rows[i][4].ToString();
                            car.Seq_no = dt.Rows[i][5].ToString();
                            car.Color = this.setColorValue(dt.Rows[i][6].ToString());
                            car.Barcode = dt.Rows[i][7].ToString();
                            car.Spec = setCarType(dt.Rows[i][8].ToString());
                            car.Engine_type = dt.Rows[i][9].ToString();
                            car.Displacement = dt.Rows[i][10].ToString();
                            car.Production_date = dt.Rows[i][13].ToString().Replace(".", "");
                            car.create_date = new DateTime();
                            car.update_date = new DateTime();
                            listCars.Add(car);
                        }

                        foreach (CarInfo car in listCars)
                        {
                            miBll.InsertCarInfo(car);
                        }
                        MessageBox.Show("导入成功！");
                        CarInfo carInfo = new CarInfo();
                        carInfo.Batch = this.comboBox4.Text;
                        this.seach(carInfo);
                    }
                    else
                    {
                        MessageBox.Show("excel模板不正确,请检查！");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("excel 数据不能为空！");
                    return;
                }
            }
        }

        private String setCarType(String carType)
        {
            if (carType.Contains("汽柴油型"))
            {
                return "汽柴油型";
            }
            else if (carType.Contains("汽油型"))
            {
                return "汽油型";
            }
            else if (carType.Contains("混合动力型"))
            {
                return "混合动力型";
            }
            else if (carType.Contains("纯电动型"))
            {
                return "纯电动型";
            }
            else
            {
                return "其他";
            }
        }
        private String setColorValue(String color)
        {
            color=color.Replace("无","");
            String value = color;
            if (color.Length>1)
            {
                foreach (char item in color)
                {
                    value += item + "/";
                }
                if (value.Length>1)
                {
                    value = value.Substring(0, value.Length - 1);
                }
            }
            return color;
        }

        /// <summary>
        /// excel 导入
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DataTable ReadExcelToTable(string path)//excel存放的路径
        {
            try
            {

                //连接字符串
                string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';"; // Office 07及以上版本 不能出现多余的空格 而且分号注意
                //string connstring = Provider=Microsoft.JET.OLEDB.4.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';"; //Office 07以下版本 
                using (OleDbConnection conn = new OleDbConnection(connstring))
                {
                    conn.Open();
                    DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" }); //得到所有sheet的名字
                    string firstSheetName = sheetsName.Rows[0][2].ToString(); //得到第一个sheet的名字
                    string sql = string.Format("SELECT * FROM [{0}]", firstSheetName); //查询字符串                    //string sql = string.Format("SELECT * FROM [{0}] WHERE [日期] is not null", firstSheetName); //查询字符串
                    OleDbDataAdapter ada = new OleDbDataAdapter(sql, connstring);
                    DataSet set = new DataSet();
                    ada.Fill(set);
                    return set.Tables[0];
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button21_Click(object sender, EventArgs e)
        {
            CarInfo carInfo = new CarInfo();
            String batch = comboBox4.Text;
            carInfo.Batch = batch;

            this.seach(carInfo);
        }

        /// <summary>
        /// 导入查询
        /// </summary>
        /// <param name="carInfo"></param>
        private void seach(CarInfo carInfo)
        {           
            List<CarInfo> dvlist = miBll.GetListAll(carInfo);
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.DataSource = dvlist;
            if (dvlist.Count > 0)
            {
                dataGridView2.Rows[0].Selected = false;
            }
        }


        /// <summary>
        /// 批次删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            String batch = comboBox4.Text;
            if (batch!=null && batch.Trim().Length>0)
            {
                miBll.DeleteCicData(batch);
                MessageBox.Show("删除成功！");
                initBatch();
                this.comboBox4.Text = "";
                this.comboBox1.Text = "";

                CarInfo carInfo = new CarInfo();
                carInfo.Batch = "LL99LL99";
                this.seach(carInfo);
            }
        }

        /// <summary>
        /// tab切换时加载下拉框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            this.initBatch();
        }

        /// <summary>
        /// 数据比对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            String batch = comboBox4.Text;
            if (batch != null && batch.Trim().Length > 0)
            {
                carInfo.Batch = batch;
                List<CarInfo> dvlist = miBll.Compare(carInfo);
                Compare(dvlist);
                if (dvlist!=null && dvlist.Count>0)
                {
                    MessageBox.Show("导入数据与车辆证明书系统数据不一致,请检查！");
                }
                else
                {
                    MessageBox.Show("导入数据与车辆证明书系统数据一致！");
                }
                dataGridView2.AutoGenerateColumns = false;
                dataGridView2.DataSource = dvlist;
                if (dvlist.Count > 0)
                {
                    dataGridView2.Rows[0].Selected = false;
                }
            }
        }

        /// <summary>
        /// 抓取数据比对
        /// </summary>
        /// <param name="dvlist"></param>
        /// <returns></returns>
        private List<CarInfo> Compare(List<CarInfo> dvlist)
        {
            foreach (CarInfo car in dvlist)
            {
                if (!car.Spec.Equals(car.spec_cic))
                {
                    car.Spec += " 不一致";
                }
                if (!car.Barcode.Equals(car.barcode_cic))
                {
                    car.Barcode += " 不一致";
                }
                if (!car.Color.Equals(car.color_cic))
                {
                    car.Color += " 不一致";
                }
                if (!car.Displacement.Equals(car.displacement_cic))
                {
                    car.Displacement += " 不一致";
                }
                if (!car.Engine_type.Equals(car.engine_type_cic))
                {
                    car.Engine_type += " 不一致";
                }
                if (!car.Motor_no.Equals(car.motor_no_cic))
                {
                    car.Motor_no += " 不一致";
                }
                if (!car.Motor_power.Equals(car.motor_power_cic))
                {
                    car.Motor_power += " 不一致";
                }
                if (!car.Production_date.Equals(car.production_date_cic))
                {
                    car.Production_date += " 不一致";
                }
            }
            return dvlist;
        }

        /// <summary>
        /// 颜色标注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e != null && e.Value!=null)
            {
                if (e.Value.ToString().Contains("不一致"))
                {
                    e.CellStyle.ForeColor = Color.Red;
                }
            }
        }
    }
}
