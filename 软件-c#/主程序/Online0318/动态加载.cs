using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Online0318
{
    public partial class Loading : Form
    {
        MainForm MF;
        /// <summary>
        /// //“动态加载”窗口进度条前进一个单位
        /// </summary>
        public void dkd() 
        {  
            progressBar1.PerformStep();
            if (progressBar1.Value == 100)
            {
                this.Close();
                //this.DialogResult = DialogResult.Yes;
            }
        }
        public Loading()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            this.StartPosition = FormStartPosition.CenterScreen;//设置窗体居中
        }

        private void Loading_Load(object sender, EventArgs e)
        {
            //设置进度条
            progressBar1.Maximum = 100;
            progressBar1.Minimum = 0;
            progressBar1.Step = 20;
            progressBar1.PerformStep();
        }

        private void Loading_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (progressBar1.Value != 100)
            {
                //控制是否断开或接受数据
                MF = (MainForm)this.Owner;
                MF.ISSaveData = false;
                MF.CanRecvData = false;
                MF.ISConnected = false;
                MF.Threada.Abort();
            }
            this.Dispose();
        }
    }
}
