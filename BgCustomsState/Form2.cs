using BLL;
using MODEL;
using mshtml;
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

namespace BgCustomsState
{
    public partial class Form2 : Form
    {
        List<UserInfo> list = new List<UserInfo>();
        HtmlDocument htmlDoc = null;
        public int recycle = 0;
        public Form2()
        {
            InitializeComponent();
        }
        //创建业务逻辑层对象
        UserInfoBll miBll = new UserInfoBll();

        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
        private void Form2_Load(object sender, EventArgs e)
        {
            //設置當前文檔為百度

            this.webBrowser1.Url = new Uri("http://www.haiguan.info/onlinesearch/gateway/Gatewaystate.aspx");

            //等待加载完毕
            while (webBrowser1.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
            htmlDoc = this.webBrowser1.Document;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (htmlDoc==null)
            {
                MessageBox.Show("没有获取到网页内容！");
                this.webBrowser1.Url = new Uri("http://www.haiguan.info/onlinesearch/gateway/Gatewaystate.aspx");

                //等待加载完毕
                while (webBrowser1.ReadyState < WebBrowserReadyState.Complete) Application.DoEvents();
                htmlDoc = this.webBrowser1.Document;
                return;
            }
            try
            {
            list = miBll.GetList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接异常！"+ex);;
            }
            button1.Enabled = false;
            timer1.Interval = 2000;
            timer1.Enabled = true;
            timer1.Tick += new EventHandler(timer1EventProcessor);//添加事件

            timer2.Interval = 2000;
            timer2.Enabled = true;
            timer2.Tick += new EventHandler(timer2EventProcessor);//添加事件
            timer2.Stop();
        }

        public void timer1EventProcessor(object source, EventArgs e)
        {
            timer1.Stop();
           
            if (list.Count > 0)
            {
                getClickInfo(list[0]);
            }
            else {
                MessageBox.Show("本次数据抓取完成！");
                timer1.Interval = 3600000;
                timer1.Start();
                return;
            }
            
        }

        public void timer2EventProcessor(object source, EventArgs e)
        {
            timer2.Stop();
            if (list.Count > 0)
            {
                UserInfo info = getInfo(list[0]);

                if (info != null && info.dfNo.Equals(list[0].dfNo))
                {
                    updateState(info);
                    list.Remove(list[0]);
                    timer1.Start();
                }
            }
            else
            {
                MessageBox.Show("本次数据抓取完成！");
                timer1.Interval = 3600000;
                return;
            }

        }

        private void getClickInfo(UserInfo userInfo)
        {
            //將我們在WinForm窗體的文本框中輸入的值同步到百度，kw是百度關鍵字輸入框的ID
            //htmlDoc.All["kw"].SetAttribute("value", this.txtKeyword.Text);
            HtmlElement inputpwd = htmlDoc.GetElementById("ctl00_MainContent_txtCode");
            if (inputpwd==null)
            {
                timer1.Stop();
                timer2.Stop();
                button1.Enabled = true;
                return;
            }
            htmlDoc.All["ctl00_MainContent_txtCode"].SetAttribute("value", userInfo.dfNo);
            htmlDoc.All["ctl00_MainContent_HistroyBtn"].InvokeMember("click");
            timer2.Start();
        }
        private UserInfo getInfo(UserInfo userInfo)
        { 
         
            
            
            HtmlDocument htmlDoc1 = this.webBrowser1.Document;
            IHTMLDocument2 document = htmlDoc1.DomDocument as IHTMLDocument2;
            IHTMLElementCollection tables = (IHTMLElementCollection)document.all.tags("TABLE");
             //MessageBox.Show("查询成功"+tables.length);
            if (tables.length > 1)
            {

                int i = 0;
                foreach (IHTMLTable table in tables)
                {
                    i++;
                    if (i > 1)
                    {
                        IHTMLElementCollection tts = table.rows;
                        foreach (IHTMLElement th in tts)
                        {
                            //跳过第一行
                            if (i > 2)
                            {
                                String a = th.innerText.ToString();
                                string[] sArray = a.Split(' ');
                                if (i == 3)
                                {
                                    userInfo.dfNo = sArray[0].ToString();
                                    userInfo.electronicDeclaration = sArray[1].ToString();
                                    userInfo.electronicDeclarationTime = sArray[2].ToString();
                                }
                                if (i == 4)
                                {
                                    userInfo.computerReview = sArray[0].ToString();
                                    userInfo.computerReviewTime = sArray[1].ToString();
                                }
                                if (i == 5)
                                {
                                    userInfo.sceneReceipt = sArray[0].ToString();
                                    userInfo.sceneReceiptTime = sArray[1].ToString();
                                }
                                if (i == 6)
                                {
                                    userInfo.documentaryRelease = sArray[0].ToString();
                                    userInfo.documentaryReleaseTime = sArray[1].ToString();
                                }
                                if (i == 7)
                                {
                                    userInfo.releaseOfGoods = sArray[0].ToString();
                                    userInfo.releaseOfGoodsTime = sArray[1].ToString();
                                }
                            }
                            i++;
                        }
                    }
                }
                i = 0;
            }
            else {
                userInfo = null;
            };
           
            return userInfo;
        }

        private void updateState(UserInfo u)
        {
            u.ctime = System.DateTime.Now.ToString();
            u.utime = System.DateTime.Now.ToString();
            
            miBll.Edit(u);

           // MessageBox.Show("查询成功");
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //得到當前文檔
            htmlDoc = this.webBrowser1.Document;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer2.Stop();
            button1.Enabled = true;
            MessageBox.Show("抓取停止！");
        }

    }
}
