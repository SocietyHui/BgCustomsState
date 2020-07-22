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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BgCustomsState
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //创建业务逻辑层对象
        CarInfoBll miBll = new CarInfoBll();
        private void Form1_Load(object sender, EventArgs e)
        {
            List<CarInfo> cblist = miBll.GetList();
            comboBox1.DataSource = cblist;
            comboBox1.DisplayMember = "Batch";
            comboBox1.ValueMember = "Batch";
        }

        private void LoadList()
        {
            //禁用列表的自动生成
            dgvList.AutoGenerateColumns = false;
            this.dgvList.DataSource = null;
            //调用方法获取数据，绑定到列表的数据源上
            dgvList.DataSource = lstr;
        }


        public static List<CarInfo> lstr = new List<CarInfo>();

        private void button1_Click(object sender, EventArgs e)
        {
            lstr.Clear();
            if (MessageBox.Show("请确认已选择正确的批次号？", "Confirm Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                return;
            }
            String batch = comboBox1.Text;
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
            folderBrowserDialog1.Description = "请选择文件夹";
            DialogResult d = folderBrowserDialog1.ShowDialog();
            if (d == DialogResult.OK)
            {

                //txtFilesName指的是界面一个文本框获取路径
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                GetFiles(new DirectoryInfo(folderBrowserDialog1.SelectedPath), "*.txt", batch);
            }
            else MessageBox.Show("请选择目录！");

            //加载列表
            LoadList();
        }



        private  void GetFiles(DirectoryInfo directory, string pattern, string batch)
        {
            if (directory.Exists || pattern.Trim() != string.Empty)
            {
                foreach (FileInfo info in directory.GetFiles(pattern))
                {

                    CarInfo carInfo = new CarInfo();
                    carInfo.Motor_no = info.Name;
                    carInfo.Model = info.FullName.ToString();
                    carInfo.Batch = batch;
                    GetFilesInfo(info.FullName.ToString(),carInfo);
                }
                //foreach (DirectoryInfo info in directory.GetDirectories())
                //{
                //    GetFiles(info, pattern);
                //}
            }


        }

        private void GetFilesInfo(String path, CarInfo carInfo)
        {
            StreamReader sr = new StreamReader(path);

            string strConten = string.Empty;
            int i = 0;
            while ((strConten = sr.ReadLine()) != null)
            {
                i++;
                if(i==1){
                    string cerNo = strConten.Replace(" ","").Trim();
                    carInfo.Df_no = cerNo;
                }
                if (i == 2 && strConten.Replace(" ", "").Trim().Length > 20)
                {
                    string vin = GetRealVinInfo(strConten);
                    carInfo.Barcode = vin;
                }
                if (i == 3 && strConten.Replace(" ", "").Trim().Length > 20)
                {
                    string vin = GetRealVinInfo(strConten);
                    carInfo.Barcode = vin;
                }
            }
            lstr.Add(carInfo);


            sr.Close();
        }


        private String GetRealVinInfo(String vinStr)
        {
            string newStr = "";
            string vin = vinStr.Replace(" ", "").Trim();
            vin = vin.Substring(vin.Length - 20, 20).Replace("〇", "0");
            foreach (char ch in vin)
            {
                if (ch >= 'A' && ch <= 'Z') {
                    newStr += ch.ToString();
                }
                if (Char.IsDigit(ch))
                {
                    newStr += ch.ToString();
                }
            }
            if (newStr.Length >= 17)
            {
                newStr = newStr.Substring(newStr.Length - 17, 17);
            }
           
            //长度超过17位为不正确的车架号
            if (newStr.Length!=17)
            {
                newStr = "";
            }
            return newStr;

        }




        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            lstr.Clear();
            //重新获取数据集合
            GetFiles(new DirectoryInfo(textBox1.Text), "*.txt", comboBox1.Text);
            
            bool b=false;
            foreach (var item in lstr)
            {
                b = miBll.UpdateCerInfo(item);
            }
             MessageBox.Show("保存成功！");
          
        }

        private void dgvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //判断双击的是否为标题
            if (e.RowIndex >= 0 && e.ColumnIndex>3)
            {
                string path = dgvList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                System.Diagnostics.Process.Start(path); //打开此文件。
                path = path.Replace(".txt",".pdf");
                System.Diagnostics.Process.Start(path); //打开此文件。

            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
