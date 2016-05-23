using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Method;

namespace Online0318
{
    public partial class SystemSetting : Form
    {
        #region 字段
        CheckConnectState checkConnectState = new CheckConnectState();
        MainForm MF;
        public TcpClient client;//建立TCP连接
        NetworkStream ns;//网络流
        int newEqCount;//新的IP数

        char[] detectINT;//检验IP是否可连接
        char[] detectREC;//校验命令，存放反馈，char 

        Dictionary<string, List<short>>  Dict;//存放临时数据,用于传送数据

        Thread threadReceive;//线程
        int channelcount = 0;//通道个数
        private int flag = 0;//关键字定位
        private int flag1 = 0;
        private bool fla;//控制断开连接
        #endregion

        #region 属性    
        ///// <summary>
        ///// 控制属性
        ///// </summary>
        //public bool Propertbt
        //{
        //    set 
        //    { 
        //        groupBox1.Enabled = value;
        //        groupBox3.Enabled = value;
        //        groupBox4.Enabled = value;
        //        groupBox7.Enabled = value;
        //        groupBox8.Enabled = value;
        //        saveSetbt.Enabled = value;
        //        detectConnectbt.Enabled = value;

        //    }
        //}
        #endregion

        public SystemSetting()
        {
            InitializeComponent();
        }
        public SystemSetting(bool isconnect)
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            this.StartPosition = FormStartPosition.CenterScreen;//设置窗体居中

            fla = isconnect;//接收MainForm连接状态
        }
        private void SystemSetting_Load(object sender, EventArgs e)
        {
            if (fla)//如果处于连接状态
            {
                groupBox2.Enabled = false;
            }
            else 
            {
                groupBox2.Enabled = true;
                EQcounts();
                //detectConnectbt.Enabled = false;
            }
            label40.Visible = false;
            progressBar1.Visible = false;//隐藏进度条 
            //检验IP进度条设置
            progressBar1.Maximum = 100;
            progressBar1.PerformStep();
            setBtnStatus();//设置连接按钮状态
            Dict = new Dictionary<string, List<short>>();//存放临时数据 
           
            #region 存储时间置初值
            List<string> savetime = new List<string>();//装置名称
            savetime.AddRange(SystemConfig.GetConfigData("SaveTime", string.Empty).Split('|'));
            WindFieldName.Text = SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|')[0];
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                if (comboBox1.Items[i].ToString() == savetime[0])
                {
                    comboBox1.SelectedIndex = i;
                    break;
                }
            }
            for (int i = 0; i < comboBox2.Items.Count; i++)
            {
                if (comboBox2.Items[i].ToString() == savetime[1])
                {
                    comboBox2.SelectedIndex = i;
                    break;
                }
            } 
            #endregion


        }

        #region 方法
        /// <summary>
        /// 读取接入的装置数
        /// </summary>
        private void EQcounts()
        {
            string[] ew = SystemConfig.GetConfigData("期望装置", string.Empty).Split('|');
            for (int i = 0; i <ew.Length; i++)
            {
                richTextBox1.AppendText(ew[i] + " " + SystemConfig.GetConfigData(ew[i], string.Empty) +
                    System.Environment.NewLine);
            }
            //int EqCount = int.Parse(SystemConfig.GetConfigData("EqCounts", string.Empty));
            //for (int i = 1; i <= EqCount; i++)
            //{
            //    richTextBox1.AppendText(i.ToString() + "#装置 " + SystemConfig.GetConfigData(i.ToString() + "#装置", string.Empty) +
            //        System.Environment.NewLine);
            //}
        }

        /// <summary>
        /// 验证长度及格式是否正确
        /// </summary>
        /// <param name="length">字符串</param>
        /// <returns>方法返回布尔值</returns>
        public bool lenthCheck(string length)
        {
            bool ii = Regex.IsMatch(length,//使用正则表达式判断是否匹配
                @"^.*[#\.\0-9\u4e00-\u9fa5]{14}$");
            return ii;
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
        /// <summary>
        /// 关键字加颜色
        /// </summary>
        /// <param name="chars"></param>
        private void KeyWordBrush(string chars)
        {
            if ("▲" == chars)
            {
                if ((flag = richTextBox1.Text.IndexOf(chars, flag)) == -1)//当文件中不存在要搜索的关键字时
                {
                    //MessageBox.Show("没有要查找的结果", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);//弹出对应的信息提示
                    //keyWord.Clear();//清空文本框中的内容
                    flag = 0;//重新为flag赋值
                }
                else//当在文件中存在对应的关键字时
                {
                    richTextBox1.Select(flag, chars.Length);//在RichTextBox控件中搜索关键字
                    flag = flag + chars.Length;//递增标识查询关键字的初始长度
                    richTextBox1.SelectionColor = Color.Green;//设定关键字为红色
                }
            }
            else
            {
                if ((flag1 = richTextBox1.Text.IndexOf(chars, flag1)) == -1)//当文件中不存在要搜索的关键字时
                {
                    flag1 = 0;//重新为flag赋值
                }
                else//当在文件中存在对应的关键字时
                {
                    richTextBox1.Select(flag1, chars.Length);//在RichTextBox控件中搜索关键字
                    flag1 = flag1 + chars.Length;//递增标识查询关键字的初始长度
                    richTextBox1.SelectionColor = Color.Red;//设定关键字为红色
                }
            }
        }
        /// <summary>
        /// 设置Connectbt状态
        /// </summary>
        private void setBtnStatus()
        {
            int con = (fla==false) ? 0 : 1;
            string[] constr = { "连接", "断开连接" };
            bool[] btnEnabled = { false, true };
            Color[] cl = {Color.Green,Color.Red };
            Connectbt.Text = constr[con];
            Connectbt.ForeColor=cl[con];
        }

        /// <summary>
        /// 线程，检验采集卡是否能连接
        /// </summary>
        private void Order()
        {
            #region 检验IP
            while (true)
            {
                try
                {
                    byte[] detOrdRecvByt = new byte[8];
                    int recv = ns.Read(detOrdRecvByt, 0, detOrdRecvByt.Length);
                    if (recv == 0)
                    {
                        MessageBox.Show("未接受到命令反馈！");
                        continue;
                    }
                    detectREC = Encoding.UTF8.GetChars(detOrdRecvByt, 0, detOrdRecvByt.Length);
                    if ((detectREC[0] == 'A') && (detectREC[1] == 'C') && (detectREC[2] == 'K'))
                    {
                        #region 可用
                        if ((detectINT[0] == 'I') && (detectREC[7] != (char)0))
                        {
                            List<short> zwsList = new List<short>();
                            zwsList.Add(Convert.ToInt16(detectREC[4]));//转速个数
                            zwsList.Add(Convert.ToInt16(detectREC[5]));//温度个数
                            zwsList.Add(Convert.ToInt16(detectREC[6]));//湿度个数
                            zwsList.Add(Convert.ToInt16(detectREC[7]));//通道个数
                            Dict.Add(channelcount.ToString() + "#装置", zwsList);
                            zwsList = null;

                            richTextBox1.AppendText(channelcount.ToString() + "#装置 " +
                                                SystemConfig.GetConfigData(channelcount.ToString() + "#装置", string.Empty) +
                                                " ▲" + System.Environment.NewLine);
                            KeyWordBrush("▲");
                            // conNodechanel.Add(channelcount.ToString() + "#装置", Convert.ToInt16(detectREC[7])); ---------------------------------------
                            listBox1.Items.Add(channelcount.ToString() + "#装置 " + Convert.ToInt16(detectREC[7]).ToString() + "个通道可用");
                            //// ns.Close(); 
                            //client.Client.Close();
                            ////ns.Dispose();
                            //client.Client.Dispose();
                        }
                        #endregion

                        #region 不可用
                        if ((detectINT[0] == 'I') && (detectREC[7] == (char)0))//
                        {

                            richTextBox1.AppendText(channelcount.ToString() + "#装置 " +
                                                    SystemConfig.GetConfigData(channelcount.ToString() + "#装置", string.Empty) +
                                                    " ▼" + System.Environment.NewLine);
                            KeyWordBrush("▼");
                            // conNodechanel.Add(channelcount.ToString() + "#装置", Convert.ToInt16(detectREC[7]));
                            listBox1.Items.Add(channelcount.ToString() + "#装置 没有通道可用");
                            //// ns.Close();
                            //client.Client.Close();
                            ////ns.Dispose();
                            //client.Client.Dispose();
                        }
                        #endregion
                        detectREC = null;
                        break;
                    }
                }
                catch (SocketException)
                {
                    throw;
                }
                catch (IOException)
                {
                }
                //continue;
            }
            #endregion
            threadReceive.Abort();
            client.Close();
            client = null;
        }

        #endregion

        #region 事件
        /// <summary>
        /// 保存接入装置的IP
        /// </summary>
        private void saveSetbt_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.SelectionLength = 0;
            richTextBox1.Text=richTextBox1.Text.Trim();
            richTextBox1.Text = Regex.Replace(richTextBox1.Text, @"(?s)\n\s*\n", "\n");//去除空行
            #region 检验IP格式是否正确
            newEqCount = richTextBox1.Lines.Length;
            for (int i = 0; i <newEqCount; i++)
            {
                string ss = richTextBox1.Lines[i];//字符串
                if (richTextBox1.Lines[i].Length >= 6)
                {
                    string ip = ss.Substring(5);
                    if ((!lenthCheck(ss)) | (!IPCheck(ip)))
                    {
                        MessageBox.Show("长度或输入格式有误或IP地址格式有误，请修改");
                        int sss = 0;
                        for (int j = 0; j<i; j++)
                        {
                            sss = sss + richTextBox1.Lines[j].Length;
                        }
                        richTextBox1.Select(sss+i, richTextBox1.Lines[i].Length);//选中第i行
                        richTextBox1.SelectionColor = Color.Red;
                        richTextBox1.SelectionLength = 0;
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("长度或输入格式有误，请修改");
                    int sss = 0;
                    for (int j = 0; j < i; j++)
                    {
                        sss = sss + richTextBox1.Lines[j].Length;
                    }
                    richTextBox1.Select(sss + i, richTextBox1.Lines[i].Length);//选中第i行
                    richTextBox1.SelectionColor = Color.Red;
                    richTextBox1.SelectionLength = 0;
                    return;
                }
            }
            #endregion

            string we = richTextBox1.Lines[0].Substring(0, 1) + "#装置";    
            for (int j = 0; j < newEqCount; j++)
            {
                SystemConfig.WriteConfigData(richTextBox1.Lines[j].Substring(0,1)+ "#装置", richTextBox1.Lines[j].Substring(5));
                if (j < newEqCount-1)
                {
                    we = we + "|" + richTextBox1.Lines[j + 1].Substring(0, 1) + "#装置"; 
                }
            }
            SystemConfig.WriteConfigData("期望装置", we);//用于生成树

            detectConnectbt.Enabled = true;
            saveSetbt.Enabled = false;
            richTextBox1.ReadOnly = true;

           
        }
        /// <summary>
        /// 检测连接状态按钮
        /// </summary>
        private void detectConnectbt_Click(object sender, EventArgs e)
        {
            //threadConnect = new Thread(ThreadConnect);
            //threadConnect.IsBackground = true;
            //threadConnect.SetApartmentState(ApartmentState.STA);
            //threadConnect.Start();
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.SelectionLength = 0;
            richTextBox1.Text = richTextBox1.Text.Trim();
            richTextBox1.Text = Regex.Replace(richTextBox1.Text, @"(?s)\n\s*\n", "\n");//去除空行
            newEqCount = richTextBox1.Lines.Length;

            progressBar1.Visible = true;//显示进度条
            label40.Visible = true;
            progressBar1.PerformStep();//增加进度条进度
           
            ThreadConnect();
            System.Threading.Thread.Sleep(2000);

        }     
        /// <summary>
        /// 连接装置
        /// </summary>
        private void Connectbt_Click(object sender, EventArgs e)
        {
           if(!fla)
           {
               System.Threading.Thread.Sleep(2000);
               if (Dict.Keys.Count!=0)
               {
                   DeviceSetSave(); 
               }
               MF = (MainForm)this.Owner;
               MF.CanRecvData = true;
               MF.ISSaveData = true;
               this.DialogResult = DialogResult.OK;
           }
           if(fla)
           {
                MF=(MainForm)this.Owner;
                fla = false;
                MF.CanRecvData = fla;//断开MainForm网络连接
                MF.ISSaveData = fla;
                MF.ISConnected = false;
               
                EQcounts();
                System.Threading.Thread.Sleep(3000);
                groupBox2.Enabled = true;
                //detectConnectbt.Enabled = true;
           }
           setBtnStatus();
        }
        /// <summary>
        /// 关闭事件
        /// </summary>
        private void SystemSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        /// <summary>
        /// 检测是否断网
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!checkConnectState.IsConnected())
            {
                listBox1.Items.Add("网络断开了");
            }
        }
        /// <summary>
        /// 选中ToolStripItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChildToolStripItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem ts = sender as ToolStripMenuItem;
            if (!ts.Checked)
            {
                ts.CheckState = CheckState.Checked;
            }
            else
            {
                ts.CheckState = CheckState.Unchecked;
            }
        }
        /// <summary>
        /// 修改名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            ToolStripTextBox tstb = sender as ToolStripTextBox;
            tstb.OwnerItem.Text = tstb.Text;
        }
        /// <summary>
        /// 修改风机及采集卡采样率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string heh="";
            #region 遍历第一层
            foreach (var item in ChannelsBt.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    #region 遍历第二层
                    foreach (var k in (item as ToolStripMenuItem).DropDownItems)
                    {
                        if (k is ToolStripMenuItem)
                        {
                            #region 遍历第三层
                            foreach (var m in (k as ToolStripMenuItem).DropDownItems)
                            {
                                if (m is ToolStripComboBox)
                                {
                                    (m as ToolStripComboBox).Items.Clear();
                                    (m as ToolStripComboBox).Items.AddRange(new object[] {
                                    "93.75 kHz",
                                    "46.88 kHz",
                                    "31.25 kHz",
                                    "23.44 kHz",
                                    "18.75 kHz",
                                    "15.63 kHz",
                                    "13.39 kHz",
                                    "11.72 kHz",
                                    "10.42 kHz",
                                    " 9.38 kHz"});
                                    (m as ToolStripComboBox).Text = ChannelsBt.DropDownItems[1].Text;
                                    int sss = (m as ToolStripComboBox).SelectedIndex;
                                    for (int i = 0; i < sss; i++)
                                    {
                                        (m as ToolStripComboBox).Items.RemoveAt(0);
                                    }
                                    heh = (m as ToolStripComboBox).Text;
                                }
                            }
                            #endregion

                        }
                    }
                    #endregion

                }
            }
            #endregion
            List<string> sadas = new List<string>();
            sadas.AddRange(new string[] {
                                    "93.75 kHz",
                                    "46.88 kHz",
                                    "31.25 kHz",
                                    "23.44 kHz",
                                    "18.75 kHz",
                                    "15.63 kHz",
                                    "13.39 kHz",
                                    "11.72 kHz",
                                    "10.42 kHz",
                                    " 9.38 kHz"});
            StaticData.DSDS = sadas.IndexOf(heh);
        }
        /// <summary>
        /// 存储时间设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 8)
            {
                comboBox2.Enabled = false;
            }
            else
            {
                comboBox2.Enabled = true;
            }
        }
        /// <summary>
        /// 存储时间设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != 8)
            {
                if (int.Parse(comboBox1.Text) < int.Parse(comboBox2.Text) / 60)
                {
                    MessageBox.Show("存储时间超出了存储间隔，请重新选择");
                    comboBox2.SelectedIndex = 0;
                }
            }
        }
        /// <summary>
        /// 复位按钮，获取初始配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, EventArgs e)
        {
            Password ps = new Password();
            if (ps.ShowDialog(this) == DialogResult.OK)
            {
                MF = (MainForm)this.Owner;
                fla = false;
                MF.CanRecvData = fla;//断开MainForm网络连接
                MF.ISSaveData = fla;
                MF.ISConnected = false;
                groupBox2.Enabled = true;
                saveSetbt.Enabled = true;
                richTextBox1.Clear();
                listBox1.Items.Clear();
                ChannelsBt.DropDownItems.Clear();
                setBtnStatus();

                #region 复位
                SystemConfig.WriteConfigData("UserID", "1");
                SystemConfig.WriteConfigData("Password", "1");
                //SystemConfig.WriteConfigData("EqCounts", "4");
                SystemConfig.WriteConfigData("SaveTime", "∞|10");
                SystemConfig.WriteConfigData("1#装置", "192.168.1.14");
                SystemConfig.WriteConfigData("2#装置", "192.168.1.15");
                SystemConfig.WriteConfigData("3#装置", "192.168.1.16");
                SystemConfig.WriteConfigData("4#装置", "192.168.1.17");
                SystemConfig.WriteConfigData("5#装置", "192.168.1.18");
                SystemConfig.WriteConfigData("期望装置", "1#装置|2#装置|3#装置|4#装置");
                SystemConfig.WriteConfigData("选用装置", "1#装置|2#装置|3#装置|4#装置");
                SystemConfig.WriteConfigData("1#装置通道分频数", "0|0|0|0|0");
                SystemConfig.WriteConfigData("2#装置通道分频数", "0|0|0|0|0");
                SystemConfig.WriteConfigData("3#装置通道分频数", "0|0|0|0|0");
                SystemConfig.WriteConfigData("4#装置通道分频数", "0|0|0|0|0");
                SystemConfig.WriteConfigData("5#装置通道分频数", "0|0|0|0|0");
                SystemConfig.WriteConfigData("风场及装置名称","龙源风场|11#风机01|齿轮箱01|齿轮箱02|齿轮箱03|齿轮箱04|11#风机02|发电机01|发电机02|低速轴01|低速轴02|20#风机01|齿轮箱01|齿轮箱02|齿轮箱03|齿轮箱04|20#风机02|发电机01|发电机02|低速轴01|低速轴02|5#装置|通道1|通道2|通道3|通道4");
                #endregion

                EQcounts();
                detectConnectbt.Enabled = true;
            }
        }
        #endregion

        /// <summary>
        /// 初始化风机名称和采集卡通道采样率
        /// </summary>
        private void DeviceINT() 
        {
            List<string> deviceName=new List<string>();//固定的。1#装置，2#装置。。。
            foreach(string st in Dict.Keys)
            {
                deviceName.Add(st);
            }

            #region 一级菜单项
            ToolStripLabel tl = new ToolStripLabel();
            tl.Text = "总采样率：";
            ChannelsBt.DropDownItems.Add(tl);

            ToolStripComboBox tb1 = new ToolStripComboBox();//总采样率下拉菜单
            tb1.ToolTipText = "装置总采样率";
            tb1.SelectedIndexChanged += new EventHandler(toolStripComboBox_SelectedIndexChanged);
            tb1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;//智能提示
            tb1.AutoCompleteSource = AutoCompleteSource.ListItems;
            tb1.Items.AddRange(new object[] {
                                    "93.75 kHz",
                                    "46.88 kHz",
                                    "31.25 kHz",
                                    "23.44 kHz",
                                    "18.75 kHz",
                                    "15.63 kHz",
                                    "13.39 kHz",
                                    "11.72 kHz",
                                    "10.42 kHz",
                                    " 9.38 kHz"});
            tb1.Size = new System.Drawing.Size(121, 25);
            tb1.Text = "93.75 kHz";
            tb1.SelectedIndex = int.Parse(SystemConfig.GetConfigData(deviceName[0].Substring(0, 1) +
                "#装置通道分频数", string.Empty).Split('|')[0]);//读取 总采样频率 的记录
            ChannelsBt.DropDownItems.Add(tb1);

            ToolStripSeparator ts = new ToolStripSeparator();//分割线
            ChannelsBt.DropDownItems.Add(ts);

            List<ToolStripMenuItem> ListTS1 = new List<ToolStripMenuItem>();//存一级菜单中的ToolStripMenuItem，以备添加二级子菜单
            for (int i = 0; i < deviceName.Count; i++)
            {
                ToolStripMenuItem rr = new ToolStripMenuItem(SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|')[5 * int.Parse(deviceName[i].Substring(0, 1)) - 4]);
                rr.Checked = true;
                rr.Click += new EventHandler(ChildToolStripItem_Click);
                ChannelsBt.DropDownItems.Add(rr);
                ListTS1.Add(rr);
            } 
            #endregion

            #region 二三级子菜单
            for (int i = 0; i < ListTS1.Count; i++)
            {
                ToolStripTextBox tt2 = new ToolStripTextBox();
                tt2.Text = ListTS1[i].Text;
                tt2.ToolTipText = "装置名称";
                tt2.TextChanged += new EventHandler(toolStripTextBox_TextChanged);
                ListTS1[i].DropDownItems.Add(tt2);

                ToolStripSeparator tss = new ToolStripSeparator();
                ListTS1[i].DropDownItems.Add(tss);

                for (int j = 1; j <= 4; j++)
                {
                    ToolStripMenuItem rr = new ToolStripMenuItem(SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|')[5 * int.Parse(deviceName[i].Substring(0, 1)) - 4+j]);
                    ListTS1[i].DropDownItems.Add(rr);

                    #region 三级子菜单
                    ToolStripTextBox tt3 = new ToolStripTextBox();
                    tt3.ToolTipText = "通道名称";
                    tt3.Text = rr.Text;
                    tt3.TextChanged += new EventHandler(toolStripTextBox_TextChanged);
                    rr.DropDownItems.Add(tt3);

                    ToolStripComboBox tb3 = new ToolStripComboBox();
                    tb3.AutoCompleteMode = AutoCompleteMode.SuggestAppend;//智能提示
                    tb3.AutoCompleteSource = AutoCompleteSource.ListItems;
                    tb3.Items.AddRange(new object[] {
                                    "93.75 kHz",
                                    "46.88 kHz",
                                    "31.25 kHz",
                                    "23.44 kHz",
                                    "18.75 kHz",
                                    "15.63 kHz",
                                    "13.39 kHz",
                                    "11.72 kHz",
                                    "10.42 kHz",
                                    " 9.38 kHz"});
                    tb3.Size = new System.Drawing.Size(121, 25);
                    tb3.Text = "93.75 kHz";
                    tb3.ToolTipText = "通道采样率";
                    rr.DropDownItems.Add(tb3);
                    int sssasad = tb1.SelectedIndex;
                    for (int b = 0; b < sssasad; b++)
                    {
                        tb3.Items.RemoveAt(0);
                    }
                    tb3.SelectedIndex = int.Parse(SystemConfig.GetConfigData(deviceName[i].Substring(0, 1) +
                        "#装置通道分频数", string.Empty).Split('|')[j]);
                    #endregion

                }

            }

            #endregion

        }
        /// <summary>
        /// 保存配置
        /// </summary>
        private void DeviceSetSave() 
        {
            if (groupBox2.Enabled && (!detectConnectbt.Enabled) && (!saveSetbt.Enabled))
            {
                List<string> deviceName = new List<string>();//固定的。1#装置，2#装置。。。
                foreach (string st in Dict.Keys)
                {
                    deviceName.Add(st);
                }

                List<string> freq = new List<string>();//多个装置频率
                List<string> name = new List<string>();//装置名称
                List<string> selectDevice = new List<string>();//选用装置名称
               
                name.AddRange(SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|'));
                int hfhg = 0;
                name[0] = toolStrip1.Items[0].Text;
                #region 遍历第一层
                foreach (var item in ChannelsBt.DropDownItems)
                {
                    if (item is ToolStripMenuItem)
                    {

                        if ((item as ToolStripMenuItem).Checked)
                        {
                            selectDevice.Add(deviceName[hfhg]);
                        }
                        freq.AddRange(SystemConfig.GetConfigData(deviceName[hfhg].Substring(0, 1) + "#装置通道分频数", string.Empty).Split('|'));
                        freq[0] = (ChannelsBt.DropDownItems[1] as ToolStripComboBox).SelectedIndex.ToString();//总频率

                        name[5 * int.Parse(deviceName[hfhg].Substring(0, 1)) - 4] = (item as ToolStripMenuItem).Text;//装置名称

                        #region 遍历第二层
                        int ddds = 1;
                        foreach (var k in (item as ToolStripMenuItem).DropDownItems)
                        {
                            if (k is ToolStripMenuItem)
                            {
                                name[5 * int.Parse(deviceName[hfhg].Substring(0, 1)) - 4 + ddds] = (k as ToolStripMenuItem).Text;//通道名称

                                #region 遍历第三层
                                foreach (var m in (k as ToolStripMenuItem).DropDownItems)
                                {
                                    if (m is ToolStripComboBox)
                                    {
                                        freq[ddds] = (m as ToolStripComboBox).SelectedIndex.ToString();//分频
                                    }
                                }
                                #endregion
                                ddds++;
                            }
                        }
                        #endregion

                        #region 写入读好的频率
                        string en = freq[0];
                        for (int w = 1; w < freq.Count; w++)
                        {
                            en = en + "|" + freq[w];
                        }
                        SystemConfig.WriteConfigData(deviceName[hfhg].Substring(0, 1) + "#装置通道分频数", en);
                        freq.Clear();
                        #endregion

                        hfhg++;
                    }
                }
                string we = selectDevice[0];
                for (int t = 1; t < selectDevice.Count; t++)
                {
                    we = we + "|" + selectDevice[t];
                }
                SystemConfig.WriteConfigData("选用装置", "");//清空
                SystemConfig.WriteConfigData("选用装置", we);//用于生成树

                string name1 = name[0];
                for (int t = 1; t < name.Count; t++)
                {
                    name1 = name1 + "|" + name[t];
                }
                SystemConfig.WriteConfigData("风场及装置名称", name1);
                #endregion

                SystemConfig.WriteConfigData("SaveTime", comboBox1.Text + "|" + comboBox2.Text);//存储时间配置
            }
            
        }
        /// <summary>
        /// 检验连接状态
        /// </summary>
        private void ThreadConnect() 
        {
            string[] sa = richTextBox1.Lines;
            richTextBox1.Clear();

          
            #region 检验IP
            for (int i = 1; i <= newEqCount; i++)
            {
                try
                {
                    string ssas = SystemConfig.GetConfigData(sa[i - 1].Substring(0, 1) + "#装置", string.Empty);
                    TcpClient client = TcpClientConnector.Connect(ssas, 3840 + int.Parse(ssas.Split('.')[3]), 2000);
                    //client = new TcpClient(ssas, 3840 + int.Parse(ssas.Split('.')[3]));
                    // client = new TcpClient(ssas, 3840); 
                    detectREC = new char[8];
                    ns = client.GetStream();
                    ns.ReadTimeout = 2000;
                    detectINT = new char[8];
                    detectINT[0] = 'I'; detectINT[1] = 'N'; detectINT[2] = 'T';
                    detectINT[3] = detectINT[4] = detectINT[5] = detectINT[6] = (char)0;
                    //detectINT[7] = i.ToString().ToCharArray(0,1)[0];
                    detectINT[7] = (char)0;
                    ns.Write(Encoding.ASCII.GetBytes(detectINT), 0, Encoding.ASCII.GetBytes(detectINT).Length);
                    ns.Flush();
                    channelcount = int.Parse(sa[i - 1].Substring(0, 1));

                    //开始线程接收命令反馈
                    threadReceive = new Thread(new ThreadStart(Order));
                    threadReceive.IsBackground = true;
                    threadReceive.Start();
                    threadReceive.Join();

                    progressBar1.PerformStep();//增加进度条进度
                }
                catch (SocketException)
                {
                    richTextBox1.AppendText(sa[i - 1].Substring(0, 1) + "#装置 " +
                        SystemConfig.GetConfigData(sa[i - 1].Substring(0, 1) + "#装置", string.Empty) +
                        " ▼" + System.Environment.NewLine);
                    KeyWordBrush("▼");
                    listBox1.Items.Add(sa[i - 1].Substring(0, 1) + "#装置 连接不可用");
                }
                catch (IOException e1)
                {
                    MessageBox.Show(e1.ToString());
                }
            }
            #endregion
            progressBar1.Visible = false;//隐藏进度条
            label40.Visible = false;
            detectConnectbt.Enabled = false;
            if (Dict.Keys.Count != 0)
            {
                DeviceINT();
            }
           // threadConnect.Abort();
           
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
    }
}
