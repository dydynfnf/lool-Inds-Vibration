using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Method;


namespace Online0318
{
    public partial class MainForm : Form
    {
        #region 字段
        int saveInterval1 = 0, saveInterval2 = 0, saveInterval3 = 0, saveInterval4 = 0;//计数存储时间
        int[] saveIntervals1 = new int[2],
            saveIntervals2 = new int[2],
            saveIntervals3 = new int[2],
            saveIntervals4 = new int[2];
        string[] savetime;
        bool canRecvData = true;//控制是否可以连接
        bool isSaveData = true;//控制是否存储数据
        bool isConnected = false;
        Thread threadDynamic;
        Loading loading;

        #region 数据传输
        Thread threadReceive1, threadReceive2, threadReceive3, threadReceive4;//线程
        int detectIP1 = 1;//控制线程同步
        Dictionary<string, List<short>> dataDict1, Mdict1, Mdict2, Mdict3, Mdict4, Mdict10, Mdict20, Mdict30, Mdict40;//Dictionary
        List<string> recMessage = new List<string>();//连接后反馈信息
        List<string> recMessage1 = new List<string>();//连接后反馈信息
        byte[] databyt1, databyt1fen, databyt2, databyt2fen, databyt3, databyt3fen, databyt4,databyt4fen;//数据格式："DATxx****_PSAI(_)"
        char[] datasaveC1, datasaveC2, datasaveC3, datasaveC4;  //接收数据     
        int dacount1 = 1, dacount2 = 1, dacount3 = 1, dacount4 = 1;//计数据包数
        int arraynum1, arraynum2, arraynum3, arraynum4;//包序号
        int recvc1 = 0, recvc2 = 0, recvc3 = 0, recvc4 = 0;//记录程序执行次数

        List<List<short>> datShowLists1 = new List<List<short>>();
        List<byte[]> datSaveListsB1 = new List<byte[]>();
        List<List<short>> datShowLists2 = new List<List<short>>();
        List<byte[]> datSaveListsB2 = new List<byte[]>();
        List<List<short>> datShowLists3 = new List<List<short>>();
        List<byte[]> datSaveListsB3 = new List<byte[]>();
        List<List<short>> datShowLists4 = new List<List<short>>();
        List<byte[]> datSaveListsB4 = new List<byte[]>();

        List<FileStream> fileStreams1 = new List<FileStream>();
        List<BinaryWriter> binaryWriters1 = new List<BinaryWriter>();
        List<FileStream> fileStreams2 = new List<FileStream>();
        List<BinaryWriter> binaryWriters2 = new List<BinaryWriter>();
        List<FileStream> fileStreams3 = new List<FileStream>();
        List<BinaryWriter> binaryWriters3 = new List<BinaryWriter>();
        List<FileStream> fileStreams4 = new List<FileStream>();
        List<BinaryWriter> binaryWriters4 = new List<BinaryWriter>();
        #endregion
        #endregion
        public delegate void MyDelegate(string limitAlarm);
        public event MyDelegate MyEvent;

        public MainForm()
        {
            InitializeComponent();
            //初始时禁用“系统配置”、“在线监测”、“历史查询”。

            // Edit by Kier
            toolStripButton2.Enabled = true;

            toolStripButton3.Enabled = false;
            toolStripButton4.Enabled = false;
            toolStripButton1.Enabled = false;
            注销登录ToolStripMenuItem.Enabled = false;
            toolStripStatusLabel1.Text = "登录状态：否";
            //this.WindowState = FormWindowState.Maximized;//最大化窗体
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        #region 属性
        /// <summary>
        /// 总缓存，将数据送到显示界面
        /// </summary>
        public Dictionary<string, List<short>> Datadict
        {
            get
            {
                dataDict1 = null;
                #region 检验是否有值
                if (Mdict10 == null)
                {
                    Mdict10 = new Dictionary<string, List<short>>(0);
                }
                if (Mdict20 == null)
                {
                    Mdict20 = new Dictionary<string, List<short>>(0);
                }
                if (Mdict30 == null)
                {
                    Mdict30 = new Dictionary<string, List<short>>(0);
                }
                if (Mdict40 == null)
                {
                    Mdict40 = new Dictionary<string, List<short>>(0);
                }
                #endregion

                //将“分缓存”数据汇总到dataDict1 
                dataDict1 = new List<Dictionary<string, List<short>>>() { Mdict10, Mdict20, Mdict30, Mdict40 }.
                    SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value);
              
                return dataDict1;//32个key,(x#装置(固定)、1—4#（固定）通道（通道名称）,1#转速，1#温度，1#温湿度（固定）)*4
            }
        }
        /// <summary>
        /// 接收“系统配置”传值，控制是否接受数据
        /// </summary>
        public bool CanRecvData
        {
            get
            {
                return canRecvData;
            }
            set
            {
                canRecvData = value;
            }
        }
        /// <summary>
        /// 接收“系统配置”传值，控制是否存储数据
        /// </summary>
        public bool ISSaveData
        {
            get
            {
                return isSaveData;
            }
            set
            {
                isSaveData = value;
            }
        }
        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool ISConnected
        {
            get
            {
                return isConnected;
            }
            set
            {
                if (!value)
                {
                    登录ToolStripMenuItem.Checked = false;
                    系统管理员身份登录ToolStripMenuItem.Checked = false;
                    if (recMessage.Count==4)
                    {
                        LoadHistorys();    
                    }  
                }
                isConnected = value;
            }
        }

        public Thread Threada 
        {
            get 
            {
                //“系统登录”下拉菜单逻辑控制
                系统管理员身份登录ToolStripMenuItem.Enabled = true;
                注销登录ToolStripMenuItem.Enabled = false;
                登录历史ToolStripMenuItem.Enabled = true;
                登录ToolStripMenuItem.Enabled = true;
                recMessage.Clear();
                return threadDynamic;
            }
        }
        #endregion

        #region 事件

        ///// <summary>
        ///// 登录
        ///// </summary>
        //private void LoginBt_Click(object sender, EventArgs e)
        //{
        //    if (txtUserID.Text.Trim().Equals(string.Empty))
        //    {
        //        errorProvider1.SetError(txtUserID, "用户名为空，请重新输入"); //显示错误的提示信息
        //        MessageBox.Show("用户名为空，请重新输入");
        //        txtUserID.Focus();

        //        return;
        //    }
        //    if (txtPassWord.Text.Trim().Equals(string.Empty))
        //    {
        //        errorProvider1.SetError(txtPassWord, "密码为空，请重新输入");
        //        MessageBox.Show("密码为空，请重新输入");
        //        txtPassWord.Focus();
        //        return;
        //    }
        //    //读取
        //    id = SystemConfig.GetConfigData("UserID", string.Empty);
        //    password = SystemConfig.GetConfigData("Password", string.Empty);
        //    if ((id == txtUserID.Text) && (password == txtPassWord.Text))
        //    {
        //        toolStripStatusLabel1.Text = "登录状态：是";
        //        #region 激活按钮
        //        txtUserID.Text = null;
        //        txtPassWord.Text = null;
        //        label2.Enabled = false;
        //        label3.Enabled = false;
        //        txtUserID.Enabled = false;
        //        txtPassWord.Enabled = false;
        //        LoginBt.Enabled = false;
        //        toolStripButton2.Enabled = true;
        //        toolStripButton3.Enabled = true;
        //        toolStripButton4.Enabled = true;
        //        errorProvider1.Clear();
        //        #endregion

        //        #region 连接状态检验
        //        Connect();
        //        string s = "网络连接状态：\n";
        //        for (int i = 0; i < recMessage.Count; i++)
        //        {
        //            s = s + recMessage[i];
        //        }
        //        MessageBox.Show(s, "网络连接反馈信息");
        //        recMessage1 = new List<string>(recMessage.ToArray());
        //        recMessage.Clear();
        //        #endregion
        //    }
        //    else if (id != txtUserID.Text)
        //    {
        //        errorProvider1.SetError(txtPassWord, "用户名不存在，请重新输入");
        //        MessageBox.Show("用户名不存在，请重新输入");
        //        txtUserID.Text = null;
        //        txtPassWord.Text = null;
        //        txtUserID.Focus();
        //    }
        //    else
        //    {
        //        errorProvider1.SetError(txtPassWord, "密码错误，请重新输入");
        //        MessageBox.Show("密码错误，请重新输入");
        //        txtPassWord.Text = null;
        //        txtPassWord.Focus();
        //    }
        //}
        ///// <summary>
        ///// 退出系统
        ///// </summary>
        //private void ExitBt_Click(object sender, EventArgs e)
        //{
        //    //if (SS != null)
        //    //{
        //    //    SS.Dispose();
        //    //}
        //    Application.Exit();
        //}
        ///// <summary>
        ///// 用户管理
        ///// </summary>
        //private void toolStripButton1_Click(object sender, EventArgs e)
        //{
        //    UserForm userForm = new UserForm();
        //    userForm.ShowDialog();
        //}
        /// <summary>
        /// 普通登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1000);//延时1秒
            loading = new Loading();//“动态加载”界面
            loading.Owner = this;
            loading.Show();
            threadDynamic = new Thread(DynamicConnect);//开启连接线程
            threadDynamic.Start();
            //“系统登录”下拉菜单逻辑控制
            系统管理员身份登录ToolStripMenuItem.Enabled = false;
            注销登录ToolStripMenuItem.Enabled = true;
            登录历史ToolStripMenuItem.Enabled = false;
            登录ToolStripMenuItem.Enabled = false;

            // Add by Kier
            系统管理员身份登录ToolStripMenuItem.Checked = true;

            canRecvData = true;
            isSaveData = false; 
            //LoadHistorys();
        }
        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 系统管理员身份登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1000);//延时
            Password ps = new Password();//输入密码窗口
            // ps.ShowDialog();
            if (ps.ShowDialog(this) == DialogResult.OK)//密码正确
            {
                loading = new Loading();//“动态加载”界面
                loading.Owner = this;
                loading.Show();
                threadDynamic = new Thread(DynamicConnect);//开启连接线程
                threadDynamic.Start();
                //“系统登录”下拉菜单逻辑控制
                注销登录ToolStripMenuItem.Enabled = true;
                登录历史ToolStripMenuItem.Enabled = false;
                登录ToolStripMenuItem.Enabled = false;
                系统管理员身份登录ToolStripMenuItem.Enabled = false;
                canRecvData = true;
                isSaveData = false;
                //LoadHistorys();
            }
            else
            {
                系统管理员身份登录ToolStripMenuItem.Checked = false;
            }

        }
        private void 注销登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CanRecvData = false;//断开连接
            ISSaveData = false;
            ISConnected = false;
            LoadHistorys();

            #region 初始化
            //系统管理员身份登录ToolStripMenuItem.Checked = false;
            系统管理员身份登录ToolStripMenuItem.Enabled = true;
            登录ToolStripMenuItem.Enabled = true;
            登录历史ToolStripMenuItem.Enabled = true;
            
            // Edit by Kier
            //toolStripButton2.Enabled = false;
            
            toolStripButton3.Enabled = false;
            toolStripButton4.Enabled = false;
            toolStripButton1.Enabled = false;
            注销登录ToolStripMenuItem.Enabled = false;
            toolStripStatusLabel1.Text = "登录状态：否";
            detectIP1 = 1;

            recMessage.Clear();//连接后反馈信息
            recMessage1.Clear();//连接后反馈信息  

            #endregion
        }
        private void 登录历史ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadHistory lh = new LoadHistory(true);//查看登录历史
            lh.ShowDialog();
        }
        private void 退出系统ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm_FormClosing(sender, e as FormClosingEventArgs);
            //if (canRecvData)
            //{
            //    CanRecvData = false;//断开连接
            //    ISSaveData = false;
            //    LoadHistorys();             
            //}
            //#region 保存登录历史
            //string APPpath = Application.StartupPath;
            //if (Directory.Exists(APPpath) == false) //判断目录是否存在
            //    //创建目录
            //    Directory.CreateDirectory(APPpath);
            //if (File.Exists(APPpath + "\\LoadHistory.bin"))
            //{
            //    File.Delete(APPpath + "\\LoadHistory.bin");
            //}
            //string destPath = Path.Combine(APPpath, Path.GetFileName(@"e:\Program Files\OnlineMonitor\LoadHistory.bin"));
            //System.IO.File.Copy(@"e:\Program Files\OnlineMonitor\LoadHistory.bin", destPath);
            //#endregion
            //Application.Exit();
        }
        /// <summary>
        /// 系统配置
        /// </summary>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //if(!ISConnected)
            //{
            //    canRecvData = true;
            //}
            SystemSetting SS = new SystemSetting(ISConnected);//“系统登录”界面
            SS.Owner = this;
            try
            {
                SS.ShowDialog();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
            if (ISSaveData)//保存配置,并开启连接
            {
                系统管理员身份登录ToolStripMenuItem.Checked = true;
                loading = new Loading();
                loading.Owner = this;
                loading.Show();
                threadDynamic = new Thread(DynamicConnect);
                threadDynamic.Start();
            }
            //通过连接状态来控制逻辑
            if (ISConnected == true)
            {
                注销登录ToolStripMenuItem.Enabled = true;
                登录历史ToolStripMenuItem.Enabled = false;
                系统管理员身份登录ToolStripMenuItem.Enabled = false;
                登录ToolStripMenuItem.Enabled = false;
            }
            if (ISConnected == false)
            {
                注销登录ToolStripMenuItem.Enabled = false;
                登录历史ToolStripMenuItem.Enabled = true;
                系统管理员身份登录ToolStripMenuItem.Enabled = true;
                登录ToolStripMenuItem.Enabled = true;
            }
            
        }
        /// <summary>
        /// 在线监测
        /// </summary>
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            OnlineMonitor OM = new OnlineMonitor();//在线监测界面
            OM.Owner = this;
            OM.FormClosed += new FormClosedEventHandler(OnlineMonitor_FormClosed);
            MyEvent += new MyDelegate(OM.LimitAlarm);//注册报警事件。MyEvent(string)即能将报警信息传至“监测界面”
            OM.Show();
            this.Hide();
        }
        /// <summary>
        /// 历史查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SearchHistory sh = new SearchHistory();//历史查询界面
            sh.Owner = this;
            sh.FormClosed += new FormClosedEventHandler(SearchHistory_FormClosed);
            sh.Show();
            this.Hide();
        }
        /// <summary>
        /// 窗体关闭前保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ISConnected)
            {
                CanRecvData = false;//断开连接
                ISSaveData = false;
                ISConnected = false;
                //LoadHistorys();
            }

            #region 保存登录历史
            string APPpath = Application.StartupPath;
            if (File.Exists(APPpath + "\\LoadHistory.bin"))
            {
                File.Delete(APPpath + "\\LoadHistory.bin");
            }
            string destPath = Path.Combine(APPpath, Path.GetFileName(@"e:\Program Files\OnlineMonitor\LoadHistory.bin"));
            if (File.Exists(@"e:\Program Files\OnlineMonitor\LoadHistory.bin"))
            {
                System.IO.File.Copy(@"e:\Program Files\OnlineMonitor\LoadHistory.bin", destPath); 
            }
            #endregion

            #region 保存报警数据
            if (File.Exists(APPpath + "\\LimitAlarm.bin"))
            {
                File.Delete(APPpath + "\\LimitAlarm.bin");
            }
            string destPath1 = Path.Combine(APPpath, Path.GetFileName(@"e:\Program Files\OnlineMonitor\LimitAlarm.bin"));
            if (File.Exists(@"e:\Program Files\OnlineMonitor\LimitAlarm.bin"))
            {
                System.IO.File.Copy(@"e:\Program Files\OnlineMonitor\LimitAlarm.bin", destPath1);
            }
            #endregion

            #region 保存配置
            if (Directory.Exists(@"e:\Program Files\OnlineMonitor") == false) //判断目录是否存在
                //创建目录
                Directory.CreateDirectory(@"e:\Program Files\OnlineMonitor");
            if (File.Exists(@"e:\Program Files\OnlineMonitor\SystemConfig.xml"))
            {
                File.Delete(@"e:\Program Files\OnlineMonitor\SystemConfig.xml");
            }
            System.IO.File.Copy(APPpath + "\\SystemConfig.xml", @"e:\Program Files\OnlineMonitor\SystemConfig.xml"); 
            #endregion

            this.Dispose();
            Application.Exit();
        }
        /// <summary>
        /// 关闭子窗体，显示主窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlineMonitor_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyEvent = null;

            this.Show();//显示主窗体
            this.Refresh();
        }
        private void SearchHistory_FormClosed(object sender, FormClosedEventArgs e) 
        {
            this.Show();//显示主窗体
            this.Refresh();
        }
        #endregion

        #region 数据处理
        /// <summary>
        /// 连接网络
        /// </summary>
        private void Connect()
        {
            try
            {

                #region 依次开启4个连接线程
                if (SystemConfig.GetConfigData("选用装置", string.Empty).Split('|').Contains("1#装置"))
                {
                    threadReceive1 = new Thread(new ThreadStart(Order1));
                    threadReceive1.IsBackground = true;
                    threadReceive1.Start();
                }
                else
                {
                    loading.dkd();
                    detectIP1 = 2;
                }
                while (detectIP1 == 1) { };//等待线程1开启
                detectIP1 = 1;
                if (SystemConfig.GetConfigData("选用装置", string.Empty).Split('|').Contains("2#装置"))
                {
                    threadReceive2 = new Thread(new ThreadStart(Order2));
                    threadReceive2.IsBackground = true;
                    threadReceive2.Start();
                }
                else
                {
                    loading.dkd();
                    detectIP1 = 2;
                }
                while (detectIP1 == 1) { };//等待线程2开启
                detectIP1 = 1;
                if (SystemConfig.GetConfigData("选用装置", string.Empty).Split('|').Contains("3#装置"))
                {
                    threadReceive3 = new Thread(new ThreadStart(Order3));
                    threadReceive3.IsBackground = true;
                    threadReceive3.Start();
                }
                else
                {
                    loading.dkd();
                    detectIP1 = 2;
                }
                while (detectIP1 == 1) { };//等待线程3开启
                detectIP1 = 1;
                if (SystemConfig.GetConfigData("选用装置", string.Empty).Split('|').Contains("4#装置"))
                {
                    threadReceive4 = new Thread(new ThreadStart(Order4));
                    threadReceive4.IsBackground = true;
                    threadReceive4.Start();
                }
                else
                {
                    loading.dkd();
                    detectIP1 = 2;
                }
                while (detectIP1 == 1) { };//等待线程4开启
                detectIP1 = 1;
                #endregion
                // System.Thr
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }// eading.Thread.Sleep(1000);
        }
        /// <summary>
        /// 线程方法
        /// </summary>
        private void Order1()
        {
            List<string> DeviceName = new List<string>();//读取采集卡名称
            DeviceName.AddRange(SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|'));
           
            try
            {
                //开启TCP连接。方法属于TcpClientConnector.cs。参数依次为IP、端口号、连接时长（本例为2000毫秒，如果2000毫秒内不能连接则退出）
                TcpClient client1 = TcpClientConnector.Connect(SystemConfig.GetConfigData("1#装置", string.Empty),
                   3840 + int.Parse(SystemConfig.GetConfigData("1#装置", string.Empty).Split('.')[3]), 2000);
                //TcpClient client1 = new TcpClient(SystemConfig.GetConfigData("1#装置", string.Empty),
                //    3840 + int.Parse(SystemConfig.GetConfigData("1#装置", string.Empty).Split('.')[3]));
                // TcpClient client1 = new TcpClient("127.0.0.1",3840);
                recMessage.Add(DeviceName[1] + "连接状态：已连接！\n");//连接状态反馈信息
                lock (this)
                {
                    ISConnected = true;
                }
                loading.dkd();
                List<string> fenpin = new List<string>();//通道分频
                fenpin.AddRange(SystemConfig.GetConfigData("1#装置通道分频数", string.Empty).Split('|'));

                #region INT_ _ _ _ X ； ACK_ _ _ _ _
                NetworkStream ns1 = client1.GetStream();//获取TCP连接的数据流
                ns1.ReadTimeout = 5000;//5000毫秒内收不到数据则断开连接
                byte[] detOrdRecvByt1 = new byte[8];//接收下位机反馈
                char[] detectINT1 = new char[8];//字符指令
                char[] detectREC1 = new char[8];//下位机反馈字符
                detectINT1[0] = 'I'; detectINT1[1] = 'N'; detectINT1[2] = 'T';
                detectINT1[3] = detectINT1[4] = detectINT1[5] = detectINT1[6] = '0';
                detectINT1[7] = '0';
                //发送指令
                ns1.Write(Encoding.ASCII.GetBytes(detectINT1), 0, Encoding.ASCII.GetBytes(detectINT1).Length);
                //刷新通信通道，为确保数据已发送
                ns1.Flush();
                //接收下位机反馈
                int recv = ns1.Read(detOrdRecvByt1, 0, detOrdRecvByt1.Length);
                if (recv == 0)
                {
                    MessageBox.Show(DeviceName[1] + "没收到INT反馈的命令");
                }
                //将"下位机反馈"转换成字符
                detectREC1 = Encoding.UTF8.GetChars(detOrdRecvByt1, 0, detOrdRecvByt1.Length);

                //传感器可用
                List<short> zwsList = new List<short>();//存储传感器个数
                if ((detectINT1[0] == 'I') && (detectREC1[7] != (char)0))
                {
                    //1个转速传感器，2个字节；6个温度传感器，12个字节；
                    //1个温湿度传感器，2个字节（包括一个温度，一个湿度，各占一个字节）
                    zwsList.Add(Convert.ToInt16(detectREC1[4]));//转速个数
                    zwsList.Add(Convert.ToInt16(detectREC1[5]));//温度个数
                    zwsList.Add(Convert.ToInt16(detectREC1[6]));//湿度个数
                    zwsList.Add(Convert.ToInt16(detectREC1[7]));//振动通道个数
                    Mdict1 = new Dictionary<string, List<short>>();
                    Mdict1.Add("1#装置", zwsList);
                    //zwsList = null;
                    recMessage.Add(DeviceName[1] + "  " + Convert.ToInt16(detectREC1[7]).ToString() + "个通道可用 \n");
                }
                //没有传感器可用
                if ((detectINT1[0] == 'I') && (detectREC1[7] == (char)0))//
                {
                    recMessage.Add(DeviceName[1] + "没有通道可用 \n");
                }
                detectIP1 = 2;//此线程已连接，跳出第542行的while循环.数值可以任意设置，只要不是1就行
                #endregion

                #region PRE_ _ _ _ X ； ACK_ _ _ _ _
                ns1 = client1.GetStream();
                detOrdRecvByt1 = new byte[8];
                detectINT1 = new char[8];
                detectREC1 = new char[8];
                detectINT1[0] = 'P'; detectINT1[1] = 'R'; detectINT1[2] = 'E';
                detectINT1[3] = detectINT1[4] = detectINT1[5] = detectINT1[6] = '0';
                detectINT1[7] = (char)int.Parse(fenpin[0]);
                ns1.Write(Encoding.ASCII.GetBytes(detectINT1), 0, Encoding.ASCII.GetBytes(detectINT1).Length);
                ns1.Flush();
                recv = ns1.Read(detOrdRecvByt1, 0, detOrdRecvByt1.Length);
                if (recv == 0)
                {
                    MessageBox.Show(DeviceName[1] + "没收到PRE反馈的命令");
                }
                #endregion

                #region DIV_ _ _ XY ； ACK_ _ _ _ _
                ns1 = client1.GetStream();
                detectREC1 = new char[8];
                detectINT1 = new char[8];
                detOrdRecvByt1 = new byte[8];
                detectINT1[0] = 'D'; detectINT1[1] = 'I'; detectINT1[2] = 'V';
                detectINT1[3] = detectINT1[4] = detectINT1[5] = '0';
                for (int i = 1; i <= Mdict1["1#装置"][3]; i++)
                {
                    detectINT1[6] = (char)i;
                    detectINT1[7] = (char)int.Parse(fenpin[i]);
                    ns1.Write(Encoding.ASCII.GetBytes(detectINT1), 0, Encoding.ASCII.GetBytes(detectINT1).Length);
                    ns1.Flush();
                    recv = ns1.Read(detOrdRecvByt1, 0, detOrdRecvByt1.Length);
                    if (recv == 0)
                    {
                        MessageBox.Show(DeviceName[1] + "没收到DIV反馈的命令");
                    }
                }
                #endregion

                #region STA_ _ _ _ _ ； ACK_ _ _ _ _
                ns1 = client1.GetStream();
                detectREC1 = new char[8];
                detectINT1 = new char[8];
                detOrdRecvByt1 = new byte[8];
                detectINT1[0] = 'S'; detectINT1[1] = 'T'; detectINT1[2] = 'A';
                detectINT1[3] = detectINT1[4] = detectINT1[5] = detectINT1[6] = detectINT1[7] = '0';
                ns1.Write(Encoding.ASCII.GetBytes(detectINT1), 0, Encoding.ASCII.GetBytes(detectINT1).Length);
                ns1.Flush();
                #endregion

                #region MyRegion
                List<char> fenpian = new List<char>();//TCP分片会导致接收不全，作为分片缓存
                List<byte> databyt11 = new List<byte>();//同上，分片导致存的数据不全
                while (CanRecvData)
                {
                   
                    try
                    {
                        int dataCounts = 0;
                        //判断采集卡是否处于连接状态，否，退出
                        if (client1.Connected == false) { break; }
                        int da = 0;//振动点数
                        Mdict1.Clear();
                        Mdict1.Add("1#装置", zwsList);
                        //计算一个数据包内的字节个数
                        for (int i = 1; i <= Mdict1["1#装置"][3]; i++)
                        {
                            da = 500 / (int.Parse(fenpin[i]) + 1) + da;
                        }
                        dataCounts = 11 + 2 * da + Mdict1["1#装置"][0] +
                            2 * Mdict1["1#装置"][1] + 2 * Mdict1["1#装置"][2];
                        databyt1 = new byte[dataCounts];
                        datasaveC1 = new char[dataCounts];//用于显示
                        databyt1fen = new byte[dataCounts];//用于存储
                        //接收数据包
                        recv = ns1.Read(databyt1, 0, databyt1.Length);
                        
                        if (!(recv > 0))
                        {
                            MessageBox.Show(DeviceName[1] + "没有收到数据,连接将断开，请重新连接。");
                            break;
                        }

                        //TCP分片接收判断
                        //接收到的数据先放到fenpian中，直至满了一个数据包
                        for (int i = 0; i < recv; i++)
                        {
                            fenpian.Add(Convert.ToChar(databyt1[i]));
                            databyt11.Add(databyt1[i]);

                        }
                        recv = 0;
                        //存满了一个数据包即取出
                        if (fenpian.Count >= dataCounts)
                        {
                            for (int a = 0; a < dataCounts; a++)
                            {
                                datasaveC1[a] = fenpian[a];
                            }
                            databyt1fen = databyt11.GetRange(0, dataCounts).ToArray();
                        }
                        else
                        {
                            continue;
                        }
                        fenpian.RemoveRange(0, dataCounts);
                        databyt11.RemoveRange(0, dataCounts);
                        //判断包头包尾
                        if ((datasaveC1[0] == 'D') && (datasaveC1[1] == 'A') && (datasaveC1[2] == 'T') &&
                            (datasaveC1[dataCounts - 5] == '_') && (datasaveC1[dataCounts - 4] == 'P') &&
                            (datasaveC1[dataCounts - 3] == 'S') && (datasaveC1[dataCounts - 2] == 'A') &&
                            (datasaveC1[dataCounts - 1] == 'I'))
                        {
                            //判断包序号
                            arraynum1 = Convert.ToInt16((datasaveC1[3] << 8) | (datasaveC1[4]));
                            if (arraynum1 == dacount1)
                            {
                                SaveData1();
                                dacount1++;
                            }
                            else
                            {
                                dacount1 = 1;
                                if (MyEvent != null)
                                {
                                    MyEvent(DeviceName[1] + "数据包包序号不对" + DateTime.Now.ToString());
                                }
                            }
                            //判断一组数据是否发完，是，则重新计数
                            if (dacount1 == (1 + 93750 / (500 * (1 + int.Parse(fenpin[0])))))
                            {
                                dacount1 = 1;
                            }
                        }
                        else
                        {
                        
                            if (MyEvent != null)
                            {
                                MyEvent(DeviceName[1] + "数据包头包尾不对" + DateTime.Now.ToString());
                            }
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        //MessageBox.Show(DeviceName[1] + "无法收到数据,请检查连接状态" + DateTime.Now.ToString());
                        if (MyEvent != null)
                        {
                            MyEvent(DeviceName[1] + "无法收到数据" + DateTime.Now.ToString());
                        }
                        GC.Collect();
                        break;
                    }
                }
                recvc1 = 0;
                lock (this)
                {
                    recMessage.Clear();
                }
                ns1.Close();//关闭网络流
                client1.Close();//关闭网络连接
                ns1.Dispose();
                client1.Client.Dispose();
                Mdict10 = null;
                
                threadReceive1.Abort();//退出线程

                #endregion
            }
            catch (SocketException)
            {
                recMessage.Add(DeviceName[1] + "连接状态：无法连接！\n");
                loading.dkd();
                detectIP1 = 2;
            }
            catch (IOException e1)
            {
                MessageBox.Show(DeviceName[1] + e1.ToString());
            }
        }
        private void Order2()
        {
            List<string> DeviceName = new List<string>();
            DeviceName.AddRange(SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|'));
           
            try
            {
                TcpClient client2 = TcpClientConnector.Connect(SystemConfig.GetConfigData("2#装置", string.Empty),
                   3840 + int.Parse(SystemConfig.GetConfigData("2#装置", string.Empty).Split('.')[3]), 2000);
                //TcpClient client2 = new TcpClient(SystemConfig.GetConfigData("2#装置", string.Empty),
                //    3840 + int.Parse(SystemConfig.GetConfigData("2#装置", string.Empty).Split('.')[3]));
                // TcpClient client2 = new TcpClient("127.0.0.2", 3840);
                recMessage.Add(DeviceName[6]+"连接状态：已连接！\n");
                lock (this)
                {
                    ISConnected = true;
                }
                loading.dkd();
                List<string> fenpin = new List<string>();
                fenpin.AddRange(SystemConfig.GetConfigData("2#装置通道分频数", string.Empty).Split('|'));

                #region INT_ _ _ _ X ； ACK_ _ _ _ _
                NetworkStream ns2 = client2.GetStream();
                ns2.ReadTimeout = 5000;
                byte[] detOrdRecvByt2 = new byte[8];
                char[] detectINT2 = new char[8];
                char[] detectREC2 = new char[8];
                detectINT2[0] = 'I'; detectINT2[1] = 'N'; detectINT2[2] = 'T';
                detectINT2[3] = detectINT2[4] = detectINT2[5] = detectINT2[6] = '0';
                detectINT2[7] = '0';
                ns2.Write(Encoding.ASCII.GetBytes(detectINT2), 0, Encoding.ASCII.GetBytes(detectINT2).Length);
                ns2.Flush();
                int recv = ns2.Read(detOrdRecvByt2, 0, detOrdRecvByt2.Length);
                if (recv == 0)
                {
                    MessageBox.Show(DeviceName[6] + "没收到INT反馈的命令");
                }
                detectREC2 = Encoding.UTF8.GetChars(detOrdRecvByt2, 0, detOrdRecvByt2.Length);

                //可用
                List<short> zwsList = new List<short>();
                if ((detectINT2[0] == 'I') && (detectREC2[7] != (char)0))
                {

                    zwsList.Add(Convert.ToInt16(detectREC2[4]));//转速个数
                    zwsList.Add(Convert.ToInt16(detectREC2[5]));//温度个数
                    zwsList.Add(Convert.ToInt16(detectREC2[6]));//湿度个数
                    zwsList.Add(Convert.ToInt16(detectREC2[7]));//通道个数
                    Mdict2 = new Dictionary<string, List<short>>();
                    Mdict2.Add("2#装置", zwsList);
                    //zwsList = null;
                    recMessage.Add(DeviceName[6] + "  " + Convert.ToInt16(detectREC2[7]).ToString() + "个通道可用 \n");
                }
                //不可用
                if ((detectINT2[0] == 'I') && (detectREC2[7] == (char)0))//
                {
                    recMessage.Add(DeviceName[6] + "没有通道可用 \n");
                }
                detectIP1 = 2;
                #endregion

                #region PRE_ _ _ _ X ； ACK_ _ _ _ _
                ns2 = client2.GetStream();
                detOrdRecvByt2 = new byte[8];
                detectINT2 = new char[8];
                detectREC2 = new char[8];
                detectINT2[0] = 'P'; detectINT2[1] = 'R'; detectINT2[2] = 'E';
                detectINT2[3] = detectINT2[4] = detectINT2[5] = detectINT2[6] = '0';
                detectINT2[7] = (char)int.Parse(fenpin[0]);
                ns2.Write(Encoding.ASCII.GetBytes(detectINT2), 0, Encoding.ASCII.GetBytes(detectINT2).Length);
                ns2.Flush();
                recv = ns2.Read(detOrdRecvByt2, 0, detOrdRecvByt2.Length);
                if (recv == 0)
                {
                    MessageBox.Show(DeviceName[6] + "没收到PRE反馈的命令");
                }
                #endregion

                #region DIV_ _ _ XY ； ACK_ _ _ _ _
                ns2 = client2.GetStream();
                detectREC2 = new char[8];
                detectINT2 = new char[8];
                detOrdRecvByt2 = new byte[8];
                detectINT2[0] = 'D'; detectINT2[1] = 'I'; detectINT2[2] = 'V';
                detectINT2[3] = detectINT2[4] = detectINT2[5] = '0';
                for (int i = 1; i <= Mdict2["2#装置"][3]; i++)
                {
                    detectINT2[6] = (char)i;
                    detectINT2[7] = (char)int.Parse(fenpin[i]);
                    ns2.Write(Encoding.ASCII.GetBytes(detectINT2), 0, Encoding.ASCII.GetBytes(detectINT2).Length);
                    ns2.Flush();
                    recv = ns2.Read(detOrdRecvByt2, 0, detOrdRecvByt2.Length);
                    if (recv == 0)
                    {
                        MessageBox.Show(DeviceName[6] + "没收到DIV反馈的命令");
                    }
                }
                #endregion

                #region STA_ _ _ _ _ ； ACK_ _ _ _ _
                ns2 = client2.GetStream();
                detectREC2 = new char[8];
                detectINT2 = new char[8];
                detOrdRecvByt2 = new byte[8];
                detectINT2[0] = 'S'; detectINT2[1] = 'T'; detectINT2[2] = 'A';
                detectINT2[3] = detectINT2[4] = detectINT2[5] = detectINT2[6] = detectINT2[7] = '0';
                ns2.Write(Encoding.ASCII.GetBytes(detectINT2), 0, Encoding.ASCII.GetBytes(detectINT2).Length);
                ns2.Flush();
                #endregion

                #region MyRegion
                List<char> fenpian = new List<char>();//TCP分片会导致接收不全
                List<byte> databyt22 = new List<byte>();//同上，分片导致存的数据不全
                while (CanRecvData)
                {
                    int dataCounts = 0;
                    try
                    {
                        if (client2.Connected == false) { break; }
                        int da = 0;//振动点数
                        Mdict2.Clear();
                        Mdict2.Add("2#装置", zwsList);
                        for (int i = 1; i <= Mdict2["2#装置"][3]; i++)
                        {
                            da = 500 / (int.Parse(fenpin[i]) + 1) + da;
                        }
                        dataCounts = 11 + 2 * da + Mdict2["2#装置"][0] +
                           2 * Mdict2["2#装置"][1] + 2 * Mdict2["2#装置"][2];
                        databyt2 = new byte[dataCounts];//未知数据量
                        datasaveC2 = new char[dataCounts];
                        databyt2fen = new byte[dataCounts];
                        recv = ns2.Read(databyt2, 0, databyt2.Length);
                        if (recv <= 0)
                        {
                            MessageBox.Show(DeviceName[6] + "没有收到数据,连接将断开，请重新连接。");
                            break;
                        }


                        //TCP分片接收判断
                        //fenpian = new List<char>();
                        for (int i = 0; i < recv; i++)
                        {
                            fenpian.Add(Convert.ToChar(databyt2[i]));
                            databyt22.Add(databyt2[i]);
                        }
                        recv = 0;
                        if (fenpian.Count >= dataCounts)
                        {
                            for (int a = 0; a < dataCounts; a++)
                            {
                                datasaveC2[a] = fenpian[a];
                            }
                            databyt2fen = databyt22.GetRange(0, dataCounts).ToArray();
                        }
                        else
                        {
                            continue;
                        }
                        fenpian.RemoveRange(0, dataCounts);
                        databyt22.RemoveRange(0, dataCounts);
                        if ((datasaveC2[0] == 'D') && (datasaveC2[1] == 'A') && (datasaveC2[2] == 'T') &&
                            (datasaveC2[dataCounts - 5] == '_') && (datasaveC2[dataCounts - 4] == 'P') &&
                            (datasaveC2[dataCounts - 3] == 'S') && (datasaveC2[dataCounts - 2] == 'A') &&
                            (datasaveC2[dataCounts - 1] == 'I'))
                        {
                            arraynum2 = Convert.ToInt16((datasaveC2[3] << 8) | (datasaveC2[4]));
                            if (arraynum2 == dacount2)
                            {
                                SaveData2();
                                dacount2++;
                            }
                            else
                            {
                                dacount2 = 1;
                                if (MyEvent != null)
                                {
                                    MyEvent(DeviceName[6] + "数据包包序号不对" + DateTime.Now.ToString());
                                }
                            }

                            if (dacount2 == (1 + 93750 / (500 * (1 + int.Parse(fenpin[0])))))
                            {
                                dacount2 = 1;
                            }
                        }
                        else
                        {
                            //MessageBox.Show("2#装置数据包头包尾不对");
                            if (MyEvent != null)
                            {
                                MyEvent(DeviceName[6] + "数据包头包尾不对" + DateTime.Now.ToString());
                            }
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        //MessageBox.Show(DeviceName[6] + "无法收到数据,请检查连接状态" + DateTime.Now.ToString());
                        if (MyEvent != null)
                        {
                            MyEvent(DeviceName[6] + "无法收到数据" + DateTime.Now.ToString());
                        }
                        GC.Collect();
                        break;
                    }
                }
                recvc2 = 0;
                lock (this)
                {
                    recMessage.Clear();
                }
                ns2.Close();
                client2.Close();
                ns2.Dispose();
                client2.Client.Dispose();
                Mdict20 = null;
                threadReceive2.Abort();

                #endregion
            }
            catch (SocketException)
            {
                recMessage.Add(DeviceName[6] + "连接状态：无法连接！\n");
                loading.dkd();
                detectIP1 = 2;
            }
            catch (IOException e1)
            {
                MessageBox.Show(DeviceName[6] +e1.ToString());
            }
        }
        private void Order3()
        {
            List<string> DeviceName = new List<string>();
            DeviceName.AddRange(SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|'));
           
            try
            {
                TcpClient client3 = TcpClientConnector.Connect(SystemConfig.GetConfigData("3#装置", string.Empty),
                   3840 + int.Parse(SystemConfig.GetConfigData("3#装置", string.Empty).Split('.')[3]), 2000);
                //TcpClient client3 = new TcpClient(SystemConfig.GetConfigData("3#装置", string.Empty),
                //    3840 + int.Parse(SystemConfig.GetConfigData("3#装置", string.Empty).Split('.')[3]));
                //TcpClient client3 = new TcpClient("127.0.1.3", 3840);
                recMessage.Add(DeviceName[11]+"连接状态：已连接！\n");
                lock (this)
                {
                    ISConnected = true;
                }
                loading.dkd();
                List<string> fenpin = new List<string>();
                fenpin.AddRange(SystemConfig.GetConfigData("3#装置通道分频数", string.Empty).Split('|'));

                #region INT_ _ _ _ X ； ACK_ _ _ _ _
                NetworkStream ns3 = client3.GetStream();
                ns3.ReadTimeout = 5000;
                byte[] detOrdRecvByt3 = new byte[8];
                char[] detectINT3 = new char[8];
                char[] detectREC3 = new char[8];
                detectINT3[0] = 'I'; detectINT3[1] = 'N'; detectINT3[2] = 'T';
                detectINT3[3] = detectINT3[4] = detectINT3[5] = detectINT3[6] = '0';
                detectINT3[7] = '0';
                ns3.Write(Encoding.ASCII.GetBytes(detectINT3), 0, Encoding.ASCII.GetBytes(detectINT3).Length);
                ns3.Flush();
                int recv = ns3.Read(detOrdRecvByt3, 0, detOrdRecvByt3.Length);
                if (recv == 0)
                {
                    MessageBox.Show(DeviceName[11]+"没收到INT反馈的命令");
                }
                detectREC3 = Encoding.UTF8.GetChars(detOrdRecvByt3, 0, detOrdRecvByt3.Length);

                //可用
                List<short> zwsList = new List<short>();
                if ((detectINT3[0] == 'I') && (detectREC3[7] != (char)0))
                {

                    zwsList.Add(Convert.ToInt16(detectREC3[4]));//转速个数
                    zwsList.Add(Convert.ToInt16(detectREC3[5]));//温度个数
                    zwsList.Add(Convert.ToInt16(detectREC3[6]));//湿度个数
                    zwsList.Add(Convert.ToInt16(detectREC3[7]));//通道个数
                    Mdict3 = new Dictionary<string, List<short>>();
                    Mdict3.Add("3#装置", zwsList);
                    //zwsList = null;
                    recMessage.Add(DeviceName[11] + "  "+Convert.ToInt16(detectREC3[7]).ToString() + "个通道可用 \n");
                }
                //不可用
                if ((detectINT3[0] == 'I') && (detectREC3[7] == (char)0))//
                {
                    recMessage.Add(DeviceName[11] + " 没有通道可用 \n");
                }
                detectIP1 = 2;
                #endregion

                #region PRE_ _ _ _ X ； ACK_ _ _ _ _
                ns3 = client3.GetStream();
                detOrdRecvByt3 = new byte[8];
                detectINT3 = new char[8];
                detectREC3 = new char[8];
                detectINT3[0] = 'P'; detectINT3[1] = 'R'; detectINT3[2] = 'E';
                detectINT3[3] = detectINT3[4] = detectINT3[5] = detectINT3[6] = '0';
                detectINT3[7] = (char)int.Parse(fenpin[0]);
                ns3.Write(Encoding.ASCII.GetBytes(detectINT3), 0, Encoding.ASCII.GetBytes(detectINT3).Length);
                ns3.Flush();
                recv = ns3.Read(detOrdRecvByt3, 0, detOrdRecvByt3.Length);
                if (recv == 0)
                {
                    MessageBox.Show(DeviceName[11] + "没收到PRE反馈的命令");
                }
                #endregion

                #region DIV_ _ _ XY ； ACK_ _ _ _ _
                ns3 = client3.GetStream();
                detectREC3 = new char[8];
                detectINT3 = new char[8];
                detOrdRecvByt3 = new byte[8];
                detectINT3[0] = 'D'; detectINT3[1] = 'I'; detectINT3[2] = 'V';
                detectINT3[3] = detectINT3[4] = detectINT3[5] = '0';
                for (int i = 1; i <= Mdict3["3#装置"][3]; i++)
                {
                    detectINT3[6] = (char)i;
                    detectINT3[7] = (char)int.Parse(fenpin[i]);
                    ns3.Write(Encoding.ASCII.GetBytes(detectINT3), 0, Encoding.ASCII.GetBytes(detectINT3).Length);
                    ns3.Flush();
                    recv = ns3.Read(detOrdRecvByt3, 0, detOrdRecvByt3.Length);
                    if (recv == 0)
                    {
                        MessageBox.Show(DeviceName[11] + "没收到DIV反馈的命令");
                    }
                }
                #endregion

                #region STA_ _ _ _ _ ； ACK_ _ _ _ _
                ns3 = client3.GetStream();
                detectREC3 = new char[8];
                detectINT3 = new char[8];
                detOrdRecvByt3 = new byte[8];
                detectINT3[0] = 'S'; detectINT3[1] = 'T'; detectINT3[2] = 'A';
                detectINT3[3] = detectINT3[4] = detectINT3[5] = detectINT3[6] = detectINT3[7] = '0';
                ns3.Write(Encoding.ASCII.GetBytes(detectINT3), 0, Encoding.ASCII.GetBytes(detectINT3).Length);
                ns3.Flush();
                #endregion

                #region MyRegion
                List<char> fenpian = new List<char>();//TCP分片会导致接收不全
                List<byte> databyt33 = new List<byte>();//同上，分片导致存的数据不全
                while (CanRecvData)
                {
                    int dataCounts = 0;
                    try
                    {
                        if (client3.Connected == false) { break; }
                        int da = 0;//振动点数
                        Mdict3.Clear();
                        Mdict3.Add("3#装置", zwsList);
                        for (int i = 1; i <= Mdict3["3#装置"][3]; i++)
                        {
                            da = 500 / (int.Parse(fenpin[i]) + 1) + da;
                        }
                        dataCounts = 11 + 2 * da + Mdict3["3#装置"][0] +
                           2 * Mdict3["3#装置"][1] + 2 * Mdict3["3#装置"][2];
                        databyt3 = new byte[dataCounts];//未知数据量
                        datasaveC3 = new char[dataCounts];
                        databyt3fen = new byte[dataCounts];
                        recv = ns3.Read(databyt3, 0, databyt3.Length);
                        if (recv <= 0)
                        {
                            MessageBox.Show(DeviceName[11] + "没有收到数据,连接将断开，请重新连接。");
                            break;
                        }


                        //TCP分片接收判断
                        //fenpian = new List<char>();
                        for (int i = 0; i < recv; i++)
                        {
                            fenpian.Add(Convert.ToChar(databyt3[i]));
                            databyt33.Add(databyt3[i]);
                        }
                        recv = 0;
                        if (fenpian.Count >= dataCounts)
                        {
                            for (int a = 0; a < dataCounts; a++)
                            {
                                datasaveC3[a] = fenpian[a];
                            }
                            databyt3fen = databyt33.GetRange(0, dataCounts).ToArray();
                        }
                        else
                        {
                            continue;
                        }
                        fenpian.RemoveRange(0, dataCounts);
                        databyt33.RemoveRange(0, dataCounts);
                        if ((datasaveC3[0] == 'D') && (datasaveC3[1] == 'A') && (datasaveC3[2] == 'T') &&
                            (datasaveC3[dataCounts - 5] == '_') && (datasaveC3[dataCounts - 4] == 'P') &&
                            (datasaveC3[dataCounts - 3] == 'S') && (datasaveC3[dataCounts - 2] == 'A') &&
                            (datasaveC3[dataCounts - 1] == 'I'))
                        {
                            arraynum3 = Convert.ToInt16((datasaveC3[3] << 8) | (datasaveC3[4]));
                            if (arraynum3 == dacount3)
                            {
                                SaveData3();
                                dacount3++;
                            }
                            else
                            {
                                dacount3 = 1;
                                if (MyEvent != null)
                                {
                                    MyEvent(DeviceName[11] + "数据包包序号不对" + DateTime.Now.ToString());
                                }
                            }

                            if (dacount3 == (1 + 93750 / (500 * (1 + int.Parse(fenpin[0])))))
                            {
                                dacount3 = 1;
                            }
                        }
                        else
                        {
                            //MessageBox.Show("3#装置数据包头包尾不对");
                            if (MyEvent != null)
                            {
                                MyEvent(DeviceName[11] + "数据包头包尾不对" + DateTime.Now.ToString());
                            }
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        //MessageBox.Show(DeviceName[11] + "无法收到数据,请检查连接状态" + DateTime.Now.ToString());
                        if (MyEvent != null)
                        {
                            MyEvent(DeviceName[11] + "无法收到数据" + DateTime.Now.ToString());
                        }
                        GC.Collect();
                        break;
                    }
                }
                recvc3 = 0;
                lock (this)
                {
                    recMessage.Clear();
                }
                ns3.Close();
                client3.Close();
                ns3.Dispose();
                client3.Client.Dispose();
                Mdict30 = null;
                threadReceive3.Abort();

                #endregion
            }
            catch (SocketException)
            {
                recMessage.Add(DeviceName[11] + "连接状态：无法连接！\n");
                loading.dkd();
                detectIP1 = 2;
            }
            catch (IOException e1)
            {
                MessageBox.Show(DeviceName[11] +e1.ToString());
            }
        }
        private void Order4()
        {
            List<string> DeviceName = new List<string>();
            DeviceName.AddRange(SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|'));
           
            try
            {
                TcpClient client4 = TcpClientConnector.Connect(SystemConfig.GetConfigData("4#装置", string.Empty),
                   3840 + int.Parse(SystemConfig.GetConfigData("4#装置", string.Empty).Split('.')[3]), 2000);
                //TcpClient client4 = new TcpClient(SystemConfig.GetConfigData("4#装置", string.Empty),
                //    3840 + int.Parse(SystemConfig.GetConfigData("4#装置", string.Empty).Split('.')[3]));
                //TcpClient client4 = new TcpClient("127.0.1.2", 3840);
                recMessage.Add(DeviceName[16]+"连接状态：已连接！\n");
                lock (this)
                {
                    ISConnected = true;
                }
                loading.dkd();
                List<string> fenpin = new List<string>();
                fenpin.AddRange(SystemConfig.GetConfigData("4#装置通道分频数", string.Empty).Split('|'));

                #region INT_ _ _ _ X ； ACK_ _ _ _ _
                NetworkStream ns4 = client4.GetStream();
                ns4.ReadTimeout = 5000;
                byte[] detOrdRecvByt4 = new byte[8];
                char[] detectINT4 = new char[8];
                char[] detectREC4 = new char[8];
                detectINT4[0] = 'I'; detectINT4[1] = 'N'; detectINT4[2] = 'T';
                detectINT4[3] = detectINT4[4] = detectINT4[5] = detectINT4[6] = '0';
                detectINT4[7] = '0';
                ns4.Write(Encoding.ASCII.GetBytes(detectINT4), 0, Encoding.ASCII.GetBytes(detectINT4).Length);
                ns4.Flush();
                int recv = ns4.Read(detOrdRecvByt4, 0, detOrdRecvByt4.Length);
                if (recv == 0)
                {
                    MessageBox.Show(DeviceName[16] + "没收到INT反馈的命令");
                }
                detectREC4 = Encoding.UTF8.GetChars(detOrdRecvByt4, 0, detOrdRecvByt4.Length);

                //可用
                List<short> zwsList = new List<short>();
                if ((detectINT4[0] == 'I') && (detectREC4[7] != (char)0))
                {

                    zwsList.Add(Convert.ToInt16(detectREC4[4]));//转速个数
                    zwsList.Add(Convert.ToInt16(detectREC4[5]));//温度个数
                    zwsList.Add(Convert.ToInt16(detectREC4[6]));//湿度个数
                    zwsList.Add(Convert.ToInt16(detectREC4[7]));//通道个数
                    Mdict4 = new Dictionary<string, List<short>>();
                    Mdict4.Add("4#装置", zwsList);
                    //zwsList = null;
                    recMessage.Add(DeviceName[16] + "  " + Convert.ToInt16(detectREC4[7]).ToString() + "个通道可用 \n");
                }
                //不可用
                if ((detectINT4[0] == 'I') && (detectREC4[7] == (char)0))//
                {
                    recMessage.Add(DeviceName[16] + "没有通道可用 \n");
                }
                detectIP1 = 2;
                #endregion

                #region PRE_ _ _ _ X ； ACK_ _ _ _ _
                ns4 = client4.GetStream();
                detOrdRecvByt4 = new byte[8];
                detectINT4 = new char[8];
                detectREC4 = new char[8];
                detectINT4[0] = 'P'; detectINT4[1] = 'R'; detectINT4[2] = 'E';
                detectINT4[3] = detectINT4[4] = detectINT4[5] = detectINT4[6] = '0';
                detectINT4[7] = (char)int.Parse(fenpin[0]);
                ns4.Write(Encoding.ASCII.GetBytes(detectINT4), 0, Encoding.ASCII.GetBytes(detectINT4).Length);
                ns4.Flush();
                recv = ns4.Read(detOrdRecvByt4, 0, detOrdRecvByt4.Length);
                if (recv == 0)
                {
                    MessageBox.Show(DeviceName[16] + "没收到PRE反馈的命令");
                }
                #endregion

                #region DIV_ _ _ XY ； ACK_ _ _ _ _
                ns4 = client4.GetStream();
                detectREC4 = new char[8];
                detectINT4 = new char[8];
                detOrdRecvByt4 = new byte[8];
                detectINT4[0] = 'D'; detectINT4[1] = 'I'; detectINT4[2] = 'V';
                detectINT4[3] = detectINT4[4] = detectINT4[5] = '0';
                for (int i = 1; i <= Mdict4["4#装置"][3]; i++)
                {
                    detectINT4[6] = (char)i;
                    detectINT4[7] = (char)int.Parse(fenpin[i]);
                    ns4.Write(Encoding.ASCII.GetBytes(detectINT4), 0, Encoding.ASCII.GetBytes(detectINT4).Length);
                    ns4.Flush();
                    recv = ns4.Read(detOrdRecvByt4, 0, detOrdRecvByt4.Length);
                    if (recv == 0)
                    {
                        MessageBox.Show(DeviceName[16] + "没收到DIV反馈的命令");
                    }
                }
                #endregion

                #region STA_ _ _ _ _ ； ACK_ _ _ _ _
                ns4 = client4.GetStream();
                detectREC4 = new char[8];
                detectINT4 = new char[8];
                detOrdRecvByt4 = new byte[8];
                detectINT4[0] = 'S'; detectINT4[1] = 'T'; detectINT4[2] = 'A';
                detectINT4[3] = detectINT4[4] = detectINT4[5] = detectINT4[6] = detectINT4[7] = '0';
                ns4.Write(Encoding.ASCII.GetBytes(detectINT4), 0, Encoding.ASCII.GetBytes(detectINT4).Length);
                ns4.Flush();
                #endregion

                #region MyRegion
                List<char> fenpian = new List<char>();//TCP分片会导致接收不全
                List<byte> databyt44 = new List<byte>();//同上，分片导致存的数据不全
                while (CanRecvData)
                {
                    int dataCounts = 0;
                    try
                    {
                        if (client4.Connected == false) { break; }
                        int da = 0;//振动点数
                        Mdict4.Clear();
                        Mdict4.Add("4#装置", zwsList);
                        for (int i = 1; i <= Mdict4["4#装置"][3]; i++)
                        {
                            da = 500 / (int.Parse(fenpin[i]) + 1) + da;
                        }
                        dataCounts = 11 + 2 * da + Mdict4["4#装置"][0] +
                           2 * Mdict4["4#装置"][1] + 2 * Mdict4["4#装置"][2];
                        databyt4 = new byte[dataCounts];//未知数据量
                        datasaveC4 = new char[dataCounts];
                        databyt4fen = new byte[dataCounts];
                        recv = ns4.Read(databyt4, 0, databyt4.Length);
                        if (recv <= 0)
                        {
                            MessageBox.Show(DeviceName[16] + "没有收到数据,连接将断开，请重新连接。");
                            break;
                        }


                        //TCP分片接收判断
                        //fenpian = new List<char>();
                        for (int i = 0; i < recv; i++)
                        {
                            fenpian.Add(Convert.ToChar(databyt4[i]));
                            databyt44.Add(databyt4[i]);
                        }
                        recv = 0;
                        if (fenpian.Count >= dataCounts)
                        {
                            for (int a = 0; a < dataCounts; a++)
                            {
                                datasaveC4[a] = fenpian[a];
                            }
                            databyt4fen = databyt44.GetRange(0, dataCounts).ToArray();
                        }
                        else
                        {
                            continue;
                        }
                        fenpian.RemoveRange(0, dataCounts);
                        databyt44.RemoveRange(0, dataCounts);
                        if ((datasaveC4[0] == 'D') && (datasaveC4[1] == 'A') && (datasaveC4[2] == 'T') &&
                            (datasaveC4[dataCounts - 5] == '_') && (datasaveC4[dataCounts - 4] == 'P') &&
                            (datasaveC4[dataCounts - 3] == 'S') && (datasaveC4[dataCounts - 2] == 'A') &&
                            (datasaveC4[dataCounts - 1] == 'I'))
                        {
                            arraynum4 = Convert.ToInt16((datasaveC4[3] << 8) | (datasaveC4[4]));
                            if (arraynum4 == dacount4)
                            {
                                SaveData4();
                                dacount4++;
                            }
                            else
                            {
                                dacount4 = 1;
                                if (MyEvent != null)
                                {
                                    MyEvent(DeviceName[16] + "数据包包序号不对" + DateTime.Now.ToString());
                                }
                            }

                            if (dacount4 == (1 + 93750 / (500 * (1 + int.Parse(fenpin[0])))))
                            {
                                dacount4 = 1;
                            }
                        }
                        else
                        {
                            //MessageBox.Show("4#装置数据包头包尾不对");
                            if (MyEvent != null)
                            {
                                MyEvent(DeviceName[16] + "数据包头包尾不对" + DateTime.Now.ToString());
                            }
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        //MessageBox.Show(DeviceName[16] + "无法收到数据,请检查连接状态" + DateTime.Now.ToString());
                        if (MyEvent != null)
                        {
                            MyEvent(DeviceName[16] + "无法收到数据" + DateTime.Now.ToString());
                        }
                        GC.Collect();
                        break;
                    }
                }
                recvc4 = 0;
                lock (this)
                {
                    recMessage.Clear();
                }
                ns4.Close();
                client4.Close();
                ns4.Dispose();
                client4.Client.Dispose();
                Mdict40 = null;
                threadReceive4.Abort();
                #endregion
            }
            catch (SocketException)
            {
                recMessage.Add(DeviceName[16] + "连接状态：无法连接！\n");
                loading.dkd();
                detectIP1 = 2;
            }
            catch (IOException e1)
            {
                MessageBox.Show(DeviceName[16] +e1.ToString());
            }
        }
        /// <summary>
        /// 线程内方法，用于存储数据
        /// </summary>
        private void SaveData1()
        {

            byte[] datatime1 = new byte[23];
            string dt = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");//现在的时间

            List<string> fenpin = new List<string>();
            fenpin.AddRange(SystemConfig.GetConfigData("1#装置通道分频数", string.Empty).Split('|'));
            List<string> DeviceName = new List<string>();
            DeviceName.AddRange(SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|'));

            int daaa = 93750 / (500 * (int.Parse(fenpin[0]) + 1));//一组数据中的数据包个数

            if (arraynum1 == 1)//第一个数据包，声明7个临时缓存，放入datShowLists1(显示)，datSaveListsB1（存储）
            {
                datShowLists1.Clear();
                datSaveListsB1.Clear();
                for (int i = 0; i < Mdict1["1#装置"][3] + 3; i++)
                {
                    List<short> listShow = new List<short>();
                    datShowLists1.Add(listShow);
                    #region 实例化
                    if (i < 4)
                    {
                        byte[] databyts = new byte[1000 * daaa / (int.Parse(fenpin[i + 1]) + 1)];
                        datSaveListsB1.Add(databyts);
                    }
                    if (i == 4)
                    {
                        byte[] databyts = new byte[2*daaa];//===============================================
                        datSaveListsB1.Add(databyts);
                    }
                    if (i == 5)
                    {
                        byte[] databyts = new byte[12 * daaa];
                        datSaveListsB1.Add(databyts);
                    }
                    if (i == 6)
                    {
                        byte[] databyts = new byte[2 * daaa];
                        datSaveListsB1.Add(databyts);
                    }
                    #endregion
                }
            }

            #region 实例化存储路径
            if (recvc1 == 0 && isSaveData)//bin文件中数据已存满，重新新建存储文件
            {
                string APPpath = Application.StartupPath + "//Data//" + DeviceName[1];
                if (Directory.Exists(APPpath) == false) //判断目录是否存在
                    //创建目录
                    Directory.CreateDirectory(APPpath);
                List<string> filePaths = new List<string>();
                for (int j = 0; j < Mdict1["1#装置"][3] + 3; j++)
                {
                    #region 路径实例化
                    if (j < 4)
                    {
                        string pat = APPpath + "\\" +  DeviceName[j + 2] + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    if (j == 4)
                    {
                        string pat = APPpath + "\\" + "转速" + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    if (j == 5)
                    {
                        string pat = APPpath + "\\" + "温度" + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    if (j == 6)
                    {
                        string pat = APPpath + "\\" + "温湿度" + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    #endregion
                }
                //创建文件，供存储数据

                if (!File.Exists(filePaths[0]))
                {
                    fileStreams1.Clear();
                    binaryWriters1.Clear();
                    for (int k = 0; k < Mdict1["1#装置"][3] + 3; k++)
                    {
                        //新建文件流，用于像文件存储数据
                        FileStream fS = new FileStream(filePaths[k], FileMode.CreateNew);
                        fileStreams1.Add(fS);
                        BinaryWriter bW = new BinaryWriter(fS);
                        binaryWriters1.Add(bW);
                    }
                }
            }
            #endregion

            #region 数据读取
            List<int> cou = new List<int>();//一个数据包中，每个振动传感器包含的数据点个数
            cou.Add(500 / (int.Parse(fenpin[1]) + 1));
            cou.Add(500 / (int.Parse(fenpin[2]) + 1));
            cou.Add(500 / (int.Parse(fenpin[3]) + 1));
            cou.Add(500 / (int.Parse(fenpin[4]) + 1));
            int dadd = 5;
            //振动数据
            for (int m = 0; m < Mdict1["1#装置"][3]; m++)//========================================================
            {
                //将“每个包”的缓存数据 搞到 “每组数据”的缓存中（十进制，用来显示）
                for (int i = 0; i < cou[m]; i++)
                {
                    datShowLists1[m].Add((short)((datasaveC1[2 * i + dadd] << 8) | (datasaveC1[2 * i + dadd + 1])));
                }
                //将“每个包”的缓存数据 复制到 “每组数据”的缓存中（二进制，用来存储）
                Buffer.BlockCopy(databyt1fen, dadd, datSaveListsB1[m], (dacount1 - 1) * 2 * cou[m], 2 * cou[m]);
                //直接存数据
                //Buffer.BlockCopy(databyt1, dadd, datSaveListsB1[m], (dacount1 - 1) * 2 * cou[m], 2 * cou[m]);
                dadd = 2 * cou[m] + dadd;//记录缓存中的位置
            }
            //转速数据
            datShowLists1[4].Add((short)((datasaveC1[dadd] << 8) | (datasaveC1[dadd + 1])));
            Buffer.BlockCopy(databyt1fen, dadd, datSaveListsB1[4], 2 * (dacount1 - 1), 2);
            //温度数据
            for (int i = 0; i < 6; i++)
            {
                datShowLists1[5].Add((short)(((short)(datasaveC1[2 * i + dadd + 2])) * 10 + (short)(datasaveC1[2 * i + dadd + 3])));
            }
            Buffer.BlockCopy(databyt1fen, dadd + 2, datSaveListsB1[5], 12 * (dacount1 - 1), 12);
            //温湿度传感器中的温度
            datShowLists1[6].Add((short)datasaveC1[dadd + 14]);
            //温湿度传感器中的湿度
            datShowLists1[6].Add((short)datasaveC1[dadd + 15]);
            Buffer.BlockCopy(databyt1fen, dadd + 14, datSaveListsB1[6], 2 * (dacount1 - 1), 2);
            #endregion

            //传送数据显示
            //将接受的“一组数据”写入内存
            if (dacount1 == daaa)
            {

                if (saveInterval1 >= saveIntervals1[0])
                {
                    saveInterval1 = 0;
                }
                #region 数据写入bin文件
                datatime1 = Encoding.ASCII.GetBytes("@@" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "@@");
                for (int p = 0; p < Mdict1["1#装置"][3] + 3; p++)
                {
                    //存入缓存，用于显示
                    if (p < 4)
                    {
                        Mdict1.Add("1#通道" + (p+1).ToString(), datShowLists1[p]);
                    }
                    if (p == 4)
                    {
                        Mdict1.Add("1#转速", datShowLists1[4]);
                    }
                    if (p == 5)
                    {
                        Mdict1.Add("1#温度", datShowLists1[5]);
                    }
                    if (p == 6)
                    {
                        Mdict1.Add("1#温湿度", datShowLists1[6]);
                    }
                    //写入bin文件存储
                    if (isSaveData && (saveInterval1 < saveIntervals1[1]))
                    {
                        binaryWriters1[p].Write(datatime1);
                        binaryWriters1[p].Flush();
                        binaryWriters1[p].Write(datSaveListsB1[p]);
                        binaryWriters1[p].Flush();
                    }
                    //string ssssd="";
                    //for (int jdakldl = 0; jdakldl < datShowLists1[0].Count;jdakldl++ )
                    //{
                    //    ssssd = ssssd + datShowLists1[0][jdakldl].ToString();
                    //}
                    //MessageBox.Show(ssssd.Substring(0,100));
                }
                Mdict10 = new Dictionary<string, List<short>>(Mdict1);
                datShowLists1.Clear();
                datSaveListsB1.Clear();
                datatime1 = null;
                #endregion

                saveInterval1++;
            }
            recvc1++;
            //判断一个数据文件是否存满300秒的数据，是，则下次存储时新建存储文件
            if (recvc1 >= (300*saveIntervals1[0]*daaa/saveIntervals1[1]) && isSaveData)
            {
                if (saveIntervals1[0] == 0)
                {
                    if (recvc1 < 300 * daaa)
                    {
                        return;
                    }
                }
                for (int q = 0; q < Mdict1["1#装置"][3] + 3; q++)
                {
                    binaryWriters1[q].Close();
                    fileStreams1[q].Close();
                }
                recvc1 = 0;
            }
        }
        private void SaveData2()
        {
            byte[] datatime1 = new byte[23];
            string dt = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            List<string> DeviceName = new List<string>();
            DeviceName.AddRange(SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|'));
            List<string> fenpin = new List<string>();
            fenpin.AddRange(SystemConfig.GetConfigData("2#装置通道分频数", string.Empty).Split('|'));

            int daaa = 93750 / (500 * (int.Parse(fenpin[0]) + 1));//数据包数

            if (arraynum2 == 1)
            {
                datShowLists2.Clear();
                datSaveListsB2.Clear();
                for (int i = 0; i < Mdict2["2#装置"][3] + 3; i++)
                {
                    List<short> listShow = new List<short>();
                    datShowLists2.Add(listShow);
                    #region 实例化
                    if (i < 4)
                    {
                        byte[] databyts = new byte[1000 * daaa / (int.Parse(fenpin[i + 1]) + 1)];
                        datSaveListsB2.Add(databyts);
                    }
                    if (i == 4)
                    {
                        byte[] databyts = new byte[2*daaa];
                        datSaveListsB2.Add(databyts);
                    }
                    if (i == 5)
                    {
                        byte[] databyts = new byte[12 * daaa];
                        datSaveListsB2.Add(databyts);
                    }
                    if (i == 6)
                    {
                        byte[] databyts = new byte[2 * daaa];
                        datSaveListsB2.Add(databyts);
                    }
                    #endregion
                }
            }

            #region 实例化存储路径
            if (recvc2 == 0 && isSaveData)
            {
                string APPpath = Application.StartupPath + "//Data//" + DeviceName[6];
                if (Directory.Exists(APPpath) == false) //判断目录是否存在
                    //创建目录
                    Directory.CreateDirectory(APPpath);
                List<string> filePaths = new List<string>();
                for (int j = 0; j < Mdict2["2#装置"][3] + 3; j++)
                {
                    #region 路径实例化
                    if (j < 4)
                    {
                        string pat = APPpath + "\\" +  DeviceName[j + 7] + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    if (j == 4)
                    {
                        string pat = APPpath + "\\" + "转速" + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    if (j == 5)
                    {
                        string pat = APPpath + "\\" + "温度" + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    if (j == 6)
                    {
                        string pat = APPpath + "\\" + "温湿度" + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    #endregion
                }
                //创建文件，供存储数据

                if (!File.Exists(filePaths[0]))
                {
                    fileStreams2.Clear();
                    binaryWriters2.Clear();
                    for (int k = 0; k < Mdict2["2#装置"][3] + 3; k++)
                    {
                        FileStream fS = new FileStream(filePaths[k], FileMode.CreateNew);
                        fileStreams2.Add(fS);
                        BinaryWriter bW = new BinaryWriter(fS);
                        binaryWriters2.Add(bW);
                    }
                }
            }
            #endregion

            #region 数据读取
            List<int> cou = new List<int>();
            cou.Add(500 / (int.Parse(fenpin[1]) + 1));
            cou.Add(500 / (int.Parse(fenpin[2]) + 1));
            cou.Add(500 / (int.Parse(fenpin[3]) + 1));
            cou.Add(500 / (int.Parse(fenpin[4]) + 1));
            int dadd = 5;
            for (int m = 0; m < Mdict2["2#装置"][3]; m++)
            {
                for (int i = 0; i < cou[m]; i++)
                {
                    datShowLists2[m].Add((short)((datasaveC2[2 * i + dadd] << 8) | (datasaveC2[2 * i + dadd + 1])));
                }
                Buffer.BlockCopy(databyt2fen, dadd, datSaveListsB2[m], (dacount2 - 1) * 2 * cou[m], 2 * cou[m]);
                dadd = 2 * cou[m] + dadd;
            }

            datShowLists2[4].Add((short)((datasaveC2[dadd] << 8) | (datasaveC2[dadd + 1])));
            Buffer.BlockCopy(databyt2fen, dadd, datSaveListsB2[4], 2 * (dacount2 - 1), 2);

            for (int i = 0; i < 6; i++)
            {
                //datShowLists2[5].Add((short)((datasaveC2[2 * i + dadd + 1] << 8) | (datasaveC2[2 * i + dadd + 2])));
                datShowLists2[5].Add((short)(((short)(datasaveC2[2 * i + dadd + 2])) * 10 + (short)(datasaveC2[2 * i + dadd + 3])));
            }
            Buffer.BlockCopy(databyt2fen, dadd + 2, datSaveListsB2[5], 12 * (dacount2 - 1), 12);

            datShowLists2[6].Add((short)datasaveC2[dadd + 14]);
            datShowLists2[6].Add((short)datasaveC2[dadd + 15]);
            Buffer.BlockCopy(databyt2fen, dadd + 14, datSaveListsB2[6], 2 * (dacount2 - 1), 2);
            #endregion

            //传送数据显示
            //将接受的数据写入内存
            if (dacount2 == daaa)
            {
                if (saveInterval2 >= saveIntervals2[0])
                {
                    saveInterval2 = 0;
                }
                #region 数据写入
                datatime1 = Encoding.ASCII.GetBytes("@@" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "@@");
                for (int p = 0; p < Mdict2["2#装置"][3] + 3; p++)
                {
                    if (isSaveData && (saveInterval2 < saveIntervals2[1]))
                    {
                        binaryWriters2[p].Write(datatime1);
                        binaryWriters2[p].Flush();
                        //Array.ConvertAll<short, byte>(datShowLists2[p].ToArray(), Convert.ToByte);
                        //binaryWriters2[p].Write(Array.ConvertAll<short, byte>(datShowLists2[p].ToArray(), Convert.ToByte));
                        
                        binaryWriters2[p].Write(datSaveListsB2[p]);
                        binaryWriters2[p].Flush();
                    }
                    if (p < 4)
                    {
                        Mdict2.Add("2#通道" + (p + 1).ToString(), datShowLists2[p]);
                    }
                    if (p == 4)
                    {
                        Mdict2.Add("2#转速", datShowLists2[4]);
                    }
                    if (p == 5)
                    {
                        Mdict2.Add("2#温度", datShowLists2[5]);
                    }
                    if (p == 6)
                    {
                        Mdict2.Add("2#温湿度", datShowLists2[6]);
                    }
                }

                Mdict20 = new Dictionary<string, List<short>>(Mdict2);
                //  Mdict1 = null;
                datShowLists2.Clear();
                datSaveListsB2.Clear();
                datatime1 = null;
                #endregion

                saveInterval2++;
            }
            recvc2++;
            if (recvc2 >= (300 * saveIntervals2[0]*daaa / saveIntervals2[1]) && isSaveData)
            {
                if (saveIntervals2[0] == 0)
                {
                    if (recvc2 < 300 * daaa)
                    {
                        return;
                    }
                }
                for (int q = 0; q < Mdict2["2#装置"][3] + 3; q++)
                {
                    binaryWriters2[q].Close();
                    fileStreams2[q].Close();
                }
                recvc2 = 0;
            }
        }
        private void SaveData3()
        {
            byte[] datatime1 = new byte[23];
            string dt = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            List<string> DeviceName = new List<string>();
            DeviceName.AddRange(SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|'));
            List<string> fenpin = new List<string>();
            fenpin.AddRange(SystemConfig.GetConfigData("3#装置通道分频数", string.Empty).Split('|'));

            int daaa = 93750 / (500 * (int.Parse(fenpin[0]) + 1));//数据包数

            if (arraynum3 == 1)
            {
                datShowLists3.Clear();
                datSaveListsB3.Clear();
                for (int i = 0; i < Mdict3["3#装置"][3] + 3; i++)
                {
                    List<short> listShow = new List<short>();
                    datShowLists3.Add(listShow);
                    #region 实例化
                    if (i < 4)
                    {
                        byte[] databyts = new byte[1000 * daaa / (int.Parse(fenpin[i + 1]) + 1)];
                        datSaveListsB3.Add(databyts);
                    }
                    if (i == 4)
                    {
                        byte[] databyts = new byte[2*daaa];
                        datSaveListsB3.Add(databyts);
                    }
                    if (i == 5)
                    {
                        byte[] databyts = new byte[12 * daaa];
                        datSaveListsB3.Add(databyts);
                    }
                    if (i == 6)
                    {
                        byte[] databyts = new byte[2 * daaa];
                        datSaveListsB3.Add(databyts);
                    }
                    #endregion
                }
            }

            #region 实例化存储路径
            if (recvc3 == 0 && isSaveData)
            {
                string APPpath = Application.StartupPath + "//Data//"+DeviceName[11];
                if (Directory.Exists(APPpath) == false) //判断目录是否存在
                    //创建目录
                    Directory.CreateDirectory(APPpath);
                List<string> filePaths = new List<string>();
                for (int j = 0; j < Mdict3["3#装置"][3] + 3; j++)
                {
                    #region 路径实例化
                    if (j < 4)
                    {
                        string pat = APPpath + "\\" + DeviceName[j + 12] + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    if (j == 4)
                    {
                        string pat = APPpath + "\\" + "转速" + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    if (j == 5)
                    {
                        string pat = APPpath + "\\" + "温度" + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    if (j == 6)
                    {
                        string pat = APPpath + "\\" + "温湿度" + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    #endregion
                }
                //创建文件，供存储数据

                if (!File.Exists(filePaths[0]))
                {
                    fileStreams3.Clear();
                    binaryWriters3.Clear();
                    for (int k = 0; k < Mdict3["3#装置"][3] + 3; k++)
                    {
                        FileStream fS = new FileStream(filePaths[k], FileMode.CreateNew);
                        fileStreams3.Add(fS);
                        BinaryWriter bW = new BinaryWriter(fS);
                        binaryWriters3.Add(bW);
                    }
                }
            }
            #endregion

            #region 数据读取
            List<int> cou = new List<int>();
            cou.Add(500 / (int.Parse(fenpin[1]) + 1));
            cou.Add(500 / (int.Parse(fenpin[2]) + 1));
            cou.Add(500 / (int.Parse(fenpin[3]) + 1));
            cou.Add(500 / (int.Parse(fenpin[4]) + 1));
            int dadd = 5;
            for (int m = 0; m < Mdict3["3#装置"][3]; m++)
            {
                for (int i = 0; i < cou[m]; i++)
                {
                    datShowLists3[m].Add((short)((datasaveC3[2 * i + dadd] << 8) | (datasaveC3[2 * i + dadd + 1])));
                }
                Buffer.BlockCopy(databyt3fen, dadd, datSaveListsB3[m], (dacount3 - 1) * 2 * cou[m], 2 * cou[m]);
                dadd = 2 * cou[m] + dadd;
            }

            datShowLists3[4].Add((short)((datasaveC3[dadd] << 8) | (datasaveC3[dadd + 1])));
            Buffer.BlockCopy(databyt3fen, dadd, datSaveListsB3[4], 2 * (dacount3 - 1), 2);

            for (int i = 0; i < 6; i++)
            {
                //datShowLists3[5].Add((short)((datasaveC3[2 * i + dadd + 1] << 8) | (datasaveC3[2 * i + dadd + 2])));
                datShowLists3[5].Add((short)(((short)(datasaveC3[2 * i + dadd + 2])) * 10 + (short)(datasaveC3[2 * i + dadd + 3])));
            }
            Buffer.BlockCopy(databyt3fen, dadd + 2, datSaveListsB3[5], 12 * (dacount3 - 1), 12);

            datShowLists3[6].Add((short)datasaveC3[dadd + 14]);
            datShowLists3[6].Add((short)datasaveC3[dadd + 15]);
            Buffer.BlockCopy(databyt3fen, dadd + 14, datSaveListsB3[6], 2 * (dacount3 - 1), 2);
            #endregion

            //传送数据显示
            //将接受的数据写入内存
            if (dacount3 == daaa)
            {
                if (saveInterval3 >= saveIntervals3[0])
                {
                    saveInterval3 = 0;
                }
                #region 数据写入
                datatime1 = Encoding.ASCII.GetBytes("@@" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "@@");
                for (int p = 0; p < Mdict3["3#装置"][3] + 3; p++)
                {
                    if (isSaveData && (saveInterval3 < saveIntervals3[1]))
                    {
                        binaryWriters3[p].Write(datatime1);
                        binaryWriters3[p].Flush();
                        binaryWriters3[p].Write(datSaveListsB3[p]);
                        binaryWriters3[p].Flush();
                    }
                    if (p < 4)
                    {
                        Mdict3.Add("3#通道" + (p + 1).ToString(), datShowLists3[p]);
                    }
                    if (p == 4)
                    {
                        Mdict3.Add("3#转速", datShowLists3[4]);
                    }
                    if (p == 5)
                    {
                        Mdict3.Add("3#温度", datShowLists3[5]);
                    }
                    if (p == 6)
                    {
                        Mdict3.Add("3#温湿度", datShowLists3[6]);
                    }
                }

                Mdict30 = new Dictionary<string, List<short>>(Mdict3);
                //  Mdict1 = null;
                datShowLists3.Clear();
                datSaveListsB3.Clear();
                datatime1 = null;
                #endregion

                saveInterval3++;
            }
            recvc3++;
            if (recvc3 >= (300 * saveIntervals3[0]*daaa / saveIntervals3[1]) && isSaveData)
            {
                if (saveIntervals3[0] == 0 )
                {
                    if(recvc3<300*daaa)
                    {
                        return;
                    }
                }
                for (int q = 0; q < Mdict3["3#装置"][3] + 3; q++)
                {
                    binaryWriters3[q].Close();
                    fileStreams3[q].Close();
                }
                recvc3 = 0;
            }
        }
        private void SaveData4()
        {
            byte[] datatime1 = new byte[23];
            string dt = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            List<string> DeviceName = new List<string>();
            DeviceName.AddRange(SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|'));
            List<string> fenpin = new List<string>();
            fenpin.AddRange(SystemConfig.GetConfigData("4#装置通道分频数", string.Empty).Split('|'));

            int daaa = 93750 / (500 * (int.Parse(fenpin[0]) + 1));//数据包数

            if (arraynum4 == 1)
            {
                datShowLists4.Clear();
                datSaveListsB4.Clear();
                for (int i = 0; i < Mdict4["4#装置"][3] + 3; i++)
                {
                    List<short> listShow = new List<short>();
                    datShowLists4.Add(listShow);
                    #region 实例化
                    if (i < 4)
                    {
                        byte[] databyts = new byte[1000 * daaa / (int.Parse(fenpin[i + 1]) + 1)];
                        datSaveListsB4.Add(databyts);
                    }
                    if (i == 4)
                    {
                        byte[] databyts = new byte[2*daaa];
                        datSaveListsB4.Add(databyts);
                    }
                    if (i == 5)
                    {
                        byte[] databyts = new byte[12 * daaa];
                        datSaveListsB4.Add(databyts);
                    }
                    if (i == 6)
                    {
                        byte[] databyts = new byte[2 * daaa];
                        datSaveListsB4.Add(databyts);//====================================================================
                    }
                    #endregion
                }
            }

            #region 实例化存储路径
            if (recvc4 == 0 && isSaveData)
            {
                string APPpath = Application.StartupPath + "//Data//" + DeviceName[16];
                if (Directory.Exists(APPpath) == false) //判断目录是否存在
                    //创建目录
                    Directory.CreateDirectory(APPpath);
                List<string> filePaths = new List<string>();
                for (int j = 0; j < Mdict4["4#装置"][3] + 3; j++)
                {
                    #region 路径实例化
                    if (j < 4)
                    {
                        string pat = APPpath + "\\" + DeviceName[j + 17] + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    if (j == 4)
                    {
                        string pat = APPpath + "\\" + "转速" + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    if (j == 5)
                    {
                        string pat = APPpath + "\\" + "温度" + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    if (j == 6)
                    {
                        string pat = APPpath + "\\" + "温湿度" + "@" + dt + ".bin";
                        filePaths.Add(pat);
                    }
                    #endregion
                }
                //创建文件，供存储数据

                if (!File.Exists(filePaths[0]))
                {
                    fileStreams4.Clear();
                    binaryWriters4.Clear();
                    for (int k = 0; k < Mdict4["4#装置"][3] + 3; k++)
                    {
                        FileStream fS = new FileStream(filePaths[k], FileMode.CreateNew);
                        fileStreams4.Add(fS);
                        BinaryWriter bW = new BinaryWriter(fS);
                        binaryWriters4.Add(bW);
                    }
                }
            }
            #endregion

            #region 数据读取
            List<int> cou = new List<int>();
            cou.Add(500 / (int.Parse(fenpin[1]) + 1));
            cou.Add(500 / (int.Parse(fenpin[2]) + 1));
            cou.Add(500 / (int.Parse(fenpin[3]) + 1));
            cou.Add(500 / (int.Parse(fenpin[4]) + 1));
            int dadd = 5;
            for (int m = 0; m < Mdict4["4#装置"][3]; m++)
            {
                for (int i = 0; i < cou[m]; i++)
                {
                    datShowLists4[m].Add((short)((datasaveC4[2 * i + dadd] << 8) | (datasaveC4[2 * i + dadd + 1])));
                }
                Buffer.BlockCopy(databyt4fen, dadd, datSaveListsB4[m], (dacount4 - 1) * 2 * cou[m], 2 * cou[m]);

                dadd = 2 * cou[m] + dadd;
            }

            datShowLists4[4].Add((short)((datasaveC4[dadd] << 8) | (datasaveC4[dadd+1])));
            Buffer.BlockCopy(databyt4fen, dadd, datSaveListsB4[4], 2 * (dacount4 - 1), 2);

            for (int i = 0; i < 6; i++)
            {
     
                datShowLists4[5].Add((short)(((short)(datasaveC4[2 * i + dadd + 2])) * 10 + (short)(datasaveC4[2 * i + dadd + 3])));
            }
            Buffer.BlockCopy(databyt4fen, dadd + 2, datSaveListsB4[5], 12 * (dacount4 - 1), 12);

            datShowLists4[6].Add((short)datasaveC4[dadd + 14]);
            datShowLists4[6].Add((short)datasaveC4[dadd + 15]);
            Buffer.BlockCopy(databyt4fen, dadd + 14, datSaveListsB4[6], 2 * (dacount4 - 1), 2);
            #endregion

            
            //传送数据显示
            //将接受的数据写入内存
            if (dacount4 == daaa)   // Add by Kier: 控制数据，等够了1秒的才打包，额……
            {
                if (saveInterval4 >= saveIntervals4[0])
                {
                    saveInterval4 = 0;
                }
                #region 数据写入
                datatime1 = Encoding.ASCII.GetBytes("@@" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "@@");
                for (int p = 0; p < Mdict4["4#装置"][3] + 3; p++)
                {
                    if (isSaveData && (saveInterval4 < saveIntervals4[1]))
                    {
                        //List<char> s = new List<char>();
                        //binaryWriters4[p].Write(s.ToArray());
                        binaryWriters4[p].Write(datatime1);
                        binaryWriters4[p].Flush();
                        binaryWriters4[p].Write(datSaveListsB4[p]);
                        binaryWriters4[p].Flush();
                    }
                    if (p < 4)
                    {
                        Mdict4.Add("4#通道" + (p + 1).ToString(), datShowLists4[p]);
                    }
                    if (p == 4)
                    {
                        Mdict4.Add("4#转速", datShowLists4[4]);
                    }
                    if (p == 5)
                    {
                        Mdict4.Add("4#温度", datShowLists4[5]);
                    }
                    if (p == 6)
                    {
                        Mdict4.Add("4#温湿度", datShowLists4[6]);
                    }
                }

                Mdict40 = new Dictionary<string, List<short>>(Mdict4);
                //  Mdict1 = null;
                datShowLists4.Clear();
                datSaveListsB4.Clear();
                datatime1 = null;
                #endregion

                saveInterval4++;
            }
            recvc4++;
            if (recvc4 >= (300 * saveIntervals4[0]*daaa / saveIntervals4[1]) && isSaveData)
            {
                if (saveIntervals4[0] == 0)
                {
                    if (recvc4 < 300 * daaa)
                    {
                        return;
                    }
                }
                for (int q = 0; q < Mdict4["4#装置"][3] + 3; q++)
                {
                    binaryWriters4[q].Close();
                    fileStreams4[q].Close();
                }
                recvc4 = 0;
            }
        }
        #endregion

        /// <summary>
        /// 登录线程
        /// </summary>
        private void DynamicConnect()
        {
            recMessage.Clear();
            saveInterval1 = 0; saveInterval2 = 0; saveInterval3 = 0; saveInterval4 = 0;
            savetime = SystemConfig.GetConfigData("SaveTime", string.Empty).Split('|');
            if (savetime[0] == "∞")//控制存储时间控制
            {
                ISSaveData = false;
                saveIntervals1[0] = 0; saveIntervals1[1] = 1;
                saveIntervals2[0] = 0; saveIntervals2[1] = 1;
                saveIntervals3[0] = 0; saveIntervals3[1] = 1;
                saveIntervals4[0] = 0; saveIntervals4[1] = 1;
            }
            else
            {
                ISSaveData = true;
                saveIntervals1[0] = 60 * int.Parse(savetime[0]);//存储间隔
                saveIntervals2[0] = 60 * int.Parse(savetime[0]);//存储间隔
                saveIntervals3[0] = 60 * int.Parse(savetime[0]);//存储间隔
                saveIntervals4[0] = 60 * int.Parse(savetime[0]);//存储间隔
                saveIntervals1[1] = int.Parse(savetime[1]);//存储时间
                saveIntervals2[1] = int.Parse(savetime[1]);//存储时间
                saveIntervals3[1] = int.Parse(savetime[1]);//存储时间
                saveIntervals4[1] = int.Parse(savetime[1]);//存储时间

            }
            Connect();//连接
            //System.Threading.Thread.Sleep(3000);
            if (系统管理员身份登录ToolStripMenuItem.Checked)
            {
                toolStripButton2.Enabled = true;//设置全局变量bool指示是否为系统登录
                toolStripButton3.Enabled = true;
                toolStripButton4.Enabled = true;
                toolStripButton1.Enabled = true;
            }
            if (登录ToolStripMenuItem.Checked)
            {
                toolStripButton3.Enabled = true;
                toolStripButton4.Enabled = true;
                toolStripButton1.Enabled = true;
            }
            if (ISConnected == false)
            {
                注销登录ToolStripMenuItem.Enabled = false;
                登录历史ToolStripMenuItem.Enabled = true;
                系统管理员身份登录ToolStripMenuItem.Enabled = true;
                登录ToolStripMenuItem.Enabled = true;
                //toolStripButton2.Enabled = false;
                //toolStripButton3.Enabled = true;
                //toolStripButton4.Enabled = true;
            }
            if (ISConnected)
            {
                //注销登录ToolStripMenuItem.Enabled = true;
                //登录历史ToolStripMenuItem.Enabled = false;
                //系统管理员身份登录ToolStripMenuItem.Enabled = false;
                //登录ToolStripMenuItem.Enabled = false;
                //系统管理员身份登录ToolStripMenuItem.Checked = true;//setting启动，断开，再连接时，显示登录
                LoadHistorys();
            }

            //反馈信息
            string s = "网络连接状态：\n";
            for (int i = 0; i < recMessage.Count; i++)
            {
                s = s + recMessage[i];
            }
            MessageBox.Show(s, "网络连接反馈信息");
            recMessage1 = new List<string>(recMessage.ToArray());

        }
        /// <summary>
        /// 登录历史记录
        /// </summary>
        private void LoadHistorys()
        {
            string APPpath = @"e:\Program Files\OnlineMonitor\";
            if (Directory.Exists(APPpath) == false) //判断目录是否存在
                //创建目录
                Directory.CreateDirectory(APPpath);
            FileStream ke;
            if (!File.Exists(@"e:\Program Files\OnlineMonitor\LoadHistory.bin"))
            {
                ke = File.Create(@"e:\Program Files\OnlineMonitor\LoadHistory.bin");
                ke.Close();
            }
            else //判断登录天数，超过60天，清除记录
            {
                FileInfo f = new FileInfo(@"e:\Program Files\OnlineMonitor\LoadHistory.bin");
                DateTime creatTime = f.CreationTime;
                DateTime nowTime = DateTime.Now;
                int days = nowTime.Subtract(creatTime).Days;
                if (days >= 60)
                {
                    File.Delete(@"e:\Program Files\OnlineMonitor\LoadHistory.bin");
                    ke = File.Create(@"e:\Program Files\OnlineMonitor\LoadHistory.bin");//创建新的
                    ke.Close();
                }
            }
            ke = new FileStream(@"e:\Program Files\OnlineMonitor\LoadHistory.bin", FileMode.Append, FileAccess.Write);

            StreamWriter bw = new StreamWriter(ke);
            string info = "";
            if (登录ToolStripMenuItem.Checked || 系统管理员身份登录ToolStripMenuItem.Checked)
            {
                info = DateTime.Now.ToString() + "  登录";
            }
            else
            {
                info = DateTime.Now.ToString() + "  退出登录";
            }
            if (系统管理员身份登录ToolStripMenuItem.Checked)
            {
                info = info + "        系统管理员";
            }
            bw.WriteLine(info);
            bw.Close();
            ke.Close();
        }

        protected override void CreateHandle()
        {
            if (!IsHandleCreated)
            {
                try
                {
                    base.CreateHandle();
                }
                catch { }
                finally
                {
                    if (!IsHandleCreated)
                    {
                        base.RecreateHandle();
                    }
                }
            }
        }

        private WindowsFormsApplication2.Form1 PCIForm = null;
        private void PCIForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show();//显示主窗体
            this.Refresh();
        }
        private void PCIForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //PCIForm.Hide();
            //this.Show();//显示主窗体
            //this.Refresh();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            PCIForm = new WindowsFormsApplication2.Form1();//在线监测界面
            PCIForm.Owner = this;
            PCIForm.FormClosed += new FormClosedEventHandler(PCIForm_FormClosed);
            PCIForm.FormClosing += new FormClosingEventHandler(PCIForm_FormClosing);

            PCIForm.Show();
            this.Hide();
        }
    }
}
