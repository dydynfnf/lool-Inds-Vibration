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
            if (IPCheck(textBox1.Text) && IPCheck(textBox2.Text) &&
                IPCheck(textBox3.Text) && IPCheck(textBox4.Text))
            {
                Connect(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
            }
            else 
            {
                MessageBox.Show("哥们,输入有误！");
                textBox1.Focus();
            }
        }

        private void Connect(string oldIP,string newip,string ziwangyanma,string wangguan) 
        {
            try
            {
                TcpClient client1 = TcpClientConnector.Connect(oldIP, 3840 + int.Parse(oldIP.Split('.')[3]), 2000);

                List<short> hehe = new List<short>();
                for (int i = 0; i < 4; i++)
                {
                    hehe.Add(short.Parse(newip.Split('.')[i]));
                }
                for (int i = 0; i < 4; i++)
                {
                    hehe.Add(short.Parse(ziwangyanma.Split('.')[i]));
                }
                for (int i = 0; i < 4; i++)
                {
                    hehe.Add(short.Parse(wangguan.Split('.')[i]));
                }

                NetworkStream ns1 = client1.GetStream();
                ns1.ReadTimeout = 5000;
                byte[] detOrdRecvByt1 = new byte[16];
                byte[] detOrdRecvByta = new byte[16];
                //char[] detectINT1 = new char[16];
                char[] detectREC1 = new char[16];
                //detectINT1[0] = 'I'; detectINT1[1] = 'P'; detectINT1[2] = 'C';
                //for (int i = 0; i < 12; i++)
                //{
                //    //detectINT1[3 + i] = (char)hehe[i];
                //    detectINT1[3 + i] = Convert.ToChar(00000011);
                //}
                //detectINT1[15] = '0';

                detOrdRecvByta[0] = Convert.ToByte('I');
                detOrdRecvByta[1] = Convert.ToByte('P');
                detOrdRecvByta[2] = Convert.ToByte('C');
                for (int i = 0; i < 12; i++)
                {
                    detOrdRecvByta[3 + i] = (byte)hehe[i];
                }
                detOrdRecvByta[15] = 0;
                //ns1.Write(Encoding.ASCII.GetBytes(detectINT1), 0, Encoding.ASCII.GetBytes(detectINT1).Length);
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
