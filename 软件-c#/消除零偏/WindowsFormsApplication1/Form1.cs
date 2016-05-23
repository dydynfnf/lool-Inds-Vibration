using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IPCheck(textBox1.Text))
            {
                Connect(textBox1.Text);
            }
            else 
            {
                MessageBox.Show("哥们,输入有误！");
                textBox1.Focus();
            }
        }

        private void Connect(string oldIP) 
        {
            try
            {
                TcpClient client1 = TcpClientConnector.Connect(oldIP, 3840 + int.Parse(oldIP.Split('.')[3]), 2000);

                NetworkStream ns1 = client1.GetStream();
                ns1.ReadTimeout = 5000;
                byte[] detOrdRecvByt1 = new byte[8];
                byte[] detOrdRecvByta = new byte[8];
                char[] detectREC1 = new char[8];
                
                detOrdRecvByta[0] = Convert.ToByte('C');
                detOrdRecvByta[1] = Convert.ToByte('A');
                detOrdRecvByta[2] = Convert.ToByte('L');
                for (int i = 0; i < 5; i++)
                {
                    detOrdRecvByta[3 + i] = 0;
                }
                ns1.Write(detOrdRecvByta, 0, detOrdRecvByta.Length);
                ns1.Flush();
                int recv = ns1.Read(detOrdRecvByt1, 0, detOrdRecvByt1.Length);
                if (recv == 0)
                {
                    MessageBox.Show("没收到反馈的命令");
                }
                detectREC1 = Encoding.UTF8.GetChars(detOrdRecvByt1, 0, detOrdRecvByt1.Length);
                if (detectREC1[0] == 'A' && detectREC1[1] == 'C' && detectREC1[2] == 'K')
                {
                    this.Close();
                }
                MessageBox.Show("反馈信息不正确。请及时保存数据，您的电脑将重启 100 次！！！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 验证IP是否正确
        /// </summary>
        /// <param name="IP">IP地址字符串</param>
        /// <returns>方法返回布尔值</returns>
        public bool IPCheck(string IP)
        {
            string num = "(25[0-5]|2[0-4]\\d|[0-1]\\d{2}|[1-9]?\\d)";//创建正则表达式字符串
            return Regex.IsMatch(IP,//使用正则表达式判断是否匹配
                ("^" + num + "\\." + num + "\\." + num + "\\." + num + "$"));
        }

    }
}
