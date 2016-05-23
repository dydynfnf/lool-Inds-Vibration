using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Method;

namespace Online0318
{
    public partial class Password : Form
    {
        public Password()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;//设置窗体居中v
        }

        private void Password_Load(object sender, EventArgs e)
        {
            
        }
        /// <summary>
        /// 重新输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox1.Focus();
        }
        /// <summary>
        /// 输入完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string password = SystemConfig.GetConfigData("Password", string.Empty);
            if(password!=textBox1.Text.Trim())
            {
                MessageBox.Show("口令不对，请重新输入！");
                textBox1.Text = "";
                textBox1.Focus();
                return;
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}
