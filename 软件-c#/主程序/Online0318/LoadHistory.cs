using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Online0318
{
    public partial class LoadHistory : Form
    {
        bool loadOralarm=true;
        public LoadHistory(bool tf)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;//设置窗体居中
            loadOralarm=tf;
        }

        private void LoadHistory_Load(object sender, EventArgs e)
        {
            FileStream ke=null;
            if (loadOralarm)
            {
                ke = new FileStream(@"e:\Program Files\OnlineMonitor\LoadHistory.bin",
                      FileMode.Open, FileAccess.Read);
            }
            else 
            {
                ke = new FileStream(@"e:\Program Files\OnlineMonitor\LimitAlarm.bin",
                         FileMode.Open, FileAccess.Read);
            }
            StreamReader br = new StreamReader(ke);
            string a = br.ReadLine();
            while (a != null)
            {
                richTextBox1.AppendText(a.ToString() + Environment.NewLine);
                a = br.ReadLine();

            }
            br.Close();
            ke.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
