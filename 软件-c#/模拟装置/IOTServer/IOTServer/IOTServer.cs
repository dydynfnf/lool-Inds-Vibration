using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Net;//Endpoint
using System.Net.Sockets;//包含套接字
using System.Threading;
using System.IO;

namespace IOTServer
{
    public partial class IOTServer : Form
    {
        Socket sokWatch = null;//负责监听 客户端段 连接请求的  套接字
        Thread threadWatch = null;//负责 调用套接字， 执行 监听请求的线程


        public IOTServer()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        private void start_Click(object sender, EventArgs e)
        {
            Class1.TT =int.Parse(textBox1.Text);
            //实例化 套接字 （ip4寻址协议，流式传输，TCP协议）
            sokWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //创建 ip对象
            IPAddress address = IPAddress.Parse(txtIP.Text.Trim());
            //创建网络节点对象 包含 ip和port
            IPEndPoint endpoint = new IPEndPoint(address, int.Parse(txtPort.Text.Trim()));
            //将 监听套接字  绑定到 对应的IP和端口
            sokWatch.Bind(endpoint);
            //设置 监听队列 长度为10(同时能够处理 10个连接请求)
            sokWatch.Listen(10);
            threadWatch = new Thread(StartWatch);
            threadWatch.IsBackground = true;
            threadWatch.Start();
            ShowMsg("启动服务器成功......\r\n");
        }

        bool isWatch = true;

        #region 1.被线程调用 监听连接端口
        /// <summary>
        /// 被线程调用 监听连接端口
        /// </summary>
        void StartWatch()
        {
            while (isWatch)
            {
                Socket sokMsg = sokWatch.Accept();//监听到请求，立即创建负责与该客户端套接字通信的套接字
                ShowBagsNum(0);
                ConnectionClient connection = new ConnectionClient(sokMsg, ShowMsg,ShowBagsNum);
                txtInput.Text = sokMsg.RemoteEndPoint.ToString();
                ShowMsg("接收连接成功......");
            }
        }
        #endregion

        /// <summary>
        /// 向文本框显示消息
        /// </summary>
        /// <param name="msgStr">消息</param>
        public void ShowMsg(string msgStr)
        {
            txtShow.AppendText(msgStr + "\r\n");
        }

        public void ShowBagsNum(double msgStr)
        {
            this.Invoke(new ThreadStart(delegate
            {
                label4.Text = "已发送数据包个数：" + msgStr.ToString();
            }));
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Class1.lkls)
            {
                Class1.lkls = false;
            }
            else 
            {
                Class1.lkls = true;
            }

        }

      
    }
}