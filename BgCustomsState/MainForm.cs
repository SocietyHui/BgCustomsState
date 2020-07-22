using BLL;
using MODEL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mshtml;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;

namespace BgCustomsState
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }



        private void MainForm_Load(object sender, EventArgs e)
        {




            CisForm frm = new CisForm();
            frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            frm.TopLevel = false;
            frm.Dock = DockStyle.Fill;
            this.tabPage1.Controls.Add(frm);
            //frm.WindowState = FormWindowState.Maximized;//如果windowState设置为最大化，添加到tabPage中时，winform不会显示出来
            frm.Show();

            this.tabPage7.Controls.Clear();
            OriginForm ogisFrm = new OriginForm();
            ogisFrm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            ogisFrm.TopLevel = false;
            ogisFrm.Dock = DockStyle.Fill;
            this.tabPage7.Controls.Add(ogisFrm);
            //frm.WindowState = FormWindowState.Maximized;//如果windowState设置为最大化，添加到tabPage中时，winform不会显示出来
            ogisFrm.Show();


            this.tabPage3.Controls.Clear();
            SingleForm frm3 = new SingleForm();
            frm3.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            frm3.TopLevel = false;
            frm3.Dock = DockStyle.Fill;
            this.tabPage3.Controls.Add(frm3);
            //frm.WindowState = FormWindowState.Maximized;//如果windowState设置为最大化，添加到tabPage中时，winform不会显示出来
            frm3.Show();


            this.tabPage2.Controls.Clear();
            Form2 frm2 = new Form2();
            frm2.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            frm2.TopLevel = false;
            frm2.Dock = DockStyle.Fill;
            this.tabPage2.Controls.Add(frm2);
            //frm2.WindowState = FormWindowState.Maximized;//如果windowState设置为最大化，添加到tabPage中时，winform不会显示出来
            frm2.Show();





            this.tabPage4.Controls.Clear();
            ShunFeng frm4 = new ShunFeng();
            frm4.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            frm4.TopLevel = false;
            frm4.Dock = DockStyle.Fill;
            this.tabPage4.Controls.Add(frm4);
            //frm4.WindowState = FormWindowState.Maximized;//如果windowState设置为最大化，添加到tabPage中时，winform不会显示出来
            frm4.Show();


            this.tabPage5.Controls.Clear();
            Form1 frm5 = new Form1();
            frm5.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            frm5.TopLevel = false;
            frm5.Dock = DockStyle.Fill;
            this.tabPage5.Controls.Add(frm5);
            //frm4.WindowState = FormWindowState.Maximized;//如果windowState设置为最大化，添加到tabPage中时，winform不会显示出来
            frm5.Show();

            this.tabPage6.Controls.Clear();
            InfinitiUploadForm frm6 = new InfinitiUploadForm();
            frm6.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            frm6.TopLevel = false;
            frm6.Dock = DockStyle.Fill;
            this.tabPage6.Controls.Add(frm6);
            //frm4.WindowState = FormWindowState.Maximized;//如果windowState设置为最大化，添加到tabPage中时，winform不会显示出来
            frm6.Show();

            this.tabPage8.Controls.Clear();
            CisAloneForm frm8 = new CisAloneForm();
            frm8.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            frm8.TopLevel = false;
            frm8.Dock = DockStyle.Fill;
            this.tabPage8.Controls.Add(frm8);
            //frm4.WindowState = FormWindowState.Maximized;//如果windowState设置为最大化，添加到tabPage中时，winform不会显示出来
            frm8.Show();

        }

    }
}
