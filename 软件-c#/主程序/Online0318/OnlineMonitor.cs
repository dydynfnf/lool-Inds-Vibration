using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyDllLibrary;
using Method;
using System.Threading;

using KPaintPanel;

namespace Online0318
{
    public partial class OnlineMonitor : Form
    {
        #region 字段
        bool boolBE = false;//ButtonsEnabled
        MainForm MF ;//实例化设置窗口
        List<string> canUse = new List<string>();
        List<string> sssss=new List<string>();
        Thread threadISconnect;
        #region 单点监测字段
        bool bool0 = true;//单点与多点切换
        string ent;//被选中的节点
        string Paent;//被选中的节点的父节点
        LinkedList<KTuple> list10 = new LinkedList<KTuple>();//实例化存放数据点的list
        LinkedList<KTuple> list11 = new LinkedList<KTuple>();
        LinkedList<KTuple> list101 = new LinkedList<KTuple>();//排序,找出最大值、最小值
        LinkedList<KTuple> list111 = new LinkedList<KTuple>();//排序,找出最大值、最小值
        List<string> nodes = new List<string>();//用于传值
        List<string> nodesc = new List<string>();//用于多项监测记录选中节点
        Dictionary<string, List<short>> data0 = new Dictionary<string, List<short>>();
        double[] frequList;//频率序列
        FFTs me = new FFTs();//提供傅里叶变换方法 
        List<short> singleList=new List<short>();//存值，傅里叶变换 
        List<short> singleList1 = new List<short>();//存值，单点监测显示 
        int tick1 = 0;//计错误数
        int showTrackBar = 100;//振动点显示个数
        #endregion

        #region 多点监测字段
        //LineItem myCurve20;
        List<LinkedList<KTuple>> pairLists = new List<LinkedList<KTuple>>();  // 修改多点检测的数据
        bool bool1 = false;//单点与多点切换
        int num1 = 0;//记录点的个数
        Dictionary<string, List<short>> data1 = new Dictionary<string, List<short>>();

        #endregion

        #region 多项监测字段
        bool bool2 = false;//单点与多点切换
        //LineItem myCurve30;
        Dictionary<string, List<short>> data2 = new Dictionary<string, List<short>>();
        #endregion

        #region 振动特征
        bool bool3 = false;//振动特征监测
        LineItem myCurve40;
        PointPairList list40;
        Dictionary<string, double> ditcSanDian = new Dictionary<string, double>();
        //List<PointPairList> pairLists4 = new List<PointPairList>();
        #endregion

        #region 三维
        bool bool4 = false;//三维特征监测

        //List<PointPairList> listpoint=new List<PointPairList>();
        //Define a graphic variable to graphic
        private Graphics gB;
        private Bitmap bitmap, bm;
        //Define instances of class
        private ChartStyle cs;
        private ChartStyle2D cs2d;
        private DataSeries ds;
        private DrawChart dc;
        private ChartFunctions cf;
        private ColorMap cm;

        private int[,] colortype;
        private float azimuth = 60;
        private float elevation = 30;
        private int Th = 45;
        private decimal cmin = 0, cmax = 100;
        #endregion
        #endregion

        Random rd = new Random();
        int timeInterval = 1000;
        int showTimeTnterval = 50;    // 用作图像显示，帧率应较高
        string[] seleTemNodes = { "MCU", "DSP", "网卡", "CPU电源", "ad-1", "ad-2" };
        string[] seleRHNodes = { "环境温度", "环境湿度" };

        private NetworkAdapter[] adapters;
        private NetworkMonitor monitor;

        public OnlineMonitor()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            CheckForIllegalCrossThreadCalls = false;// 允许多线程调用
         
        }
        private void OnlineMonitor_Load(object sender, EventArgs e)
        {
            try
            {

                toolStripComboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;//ComboBox控件智能提示
                toolStripComboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;
                comboBox1.SelectedIndex = 0;

                ShowScrollbartoolStripBt.Enabled = false;
                //风机测点名称
                sssss.AddRange(SystemConfig.GetConfigData("风场及装置名称", string.Empty).Split('|'));

                MF = (MainForm)this.Owner;
               
                if (MF.ISConnected && MF.Datadict.Count > 0)
                {
                    //获取数据
                    data0 = MF.Datadict;//"1#装置""1#通道"..."1#转速""1#温度""1#温湿度"
                    List<string> ls = new List<string>(data0.Keys);
                    for (int i = 0; i < data0.Keys.Count / 8; i++)
                    {
                        canUse.Add(ls[8 * i]);//"1#装置""2#装置"".......
                    }
                    timer2.Interval = showTimeTnterval;
                    timer1.Interval = timeInterval;//无论单点还是多点，都开启time1
                    CreatTree();
                    //默认选中节点
                    treeView1.SelectedNode = treeView1.Nodes[0].Nodes[0].Nodes[0];
                    ent = treeView1.Nodes[0].Nodes[0].Nodes[0].Text;
                    Paent = treeView1.Nodes[0].Nodes[0].Text;
                    MyMasterPane1(ent, Paent);//初始化显示界面
                    Initial3D();//初始化瀑布图
                }
                else
                {
                    System.Threading.Thread.Sleep(1000);
                }
                //
                if (MF.Datadict.Count != 8 * SystemConfig.GetConfigData("选用装置", string.Empty).Split('|').Length)
                {
                    threadISconnect = new Thread(new ThreadStart(ThreadIsConnect));
                    threadISconnect.Start();
                }

                #region 网速显示
                monitor = new NetworkMonitor();
                this.adapters = monitor.Adapters;
                /* If the length of adapters is zero, then no instance  
                 * exists in the networking category of performance console.*/
                if (adapters.Length == 0)
                {
                    MessageBox.Show("No network adapters found on this computer.");
                    return;
                }
                monitor.StartMonitoring();
                #endregion

            }
            catch (OverflowException ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
        }
    
        #region 属性
        //显示控件
        public bool ButtonsEnabled
        {
            set
            {
                boolBE = value;
                接收ToolStripMenuItem.Enabled = !value;
                停止接收ToolStripMenuItem.Enabled = value;
                接收数据toolStripBt.Enabled = !value;
                停止接收toolStripBt.Enabled = value;
            }
            get
            {
                return boolBE;
            }
        } 
        #endregion
   
        #region 方法
        /// <summary>
        /// 生成监测点树结构，不同监测界面的树不同
        /// </summary>
        private void CreatTree()
        {
            treeView1.Nodes.Clear();
            treeView1.CheckBoxes = false;

            #region 单点监测树
            //XX风场                     //XX风场 DICT
            //1#装置（XX）               //1#装置
            //通道1—4（XX）             //1#(通道1—4)（XX）
            //1#转速，1#温度，1#温湿度   //1#转速，1#温度，1#温湿度
           
            TreeNode root1 = treeView1.Nodes.Add(sssss[0]);  //父节点
            int ode = 0;
            foreach(string st in canUse)
            {
                int stt = int.Parse(st.Substring(0,1));
                root1.Nodes.Add(sssss[5*stt-4]);
                root1.Nodes[ode].Nodes.Add(sssss[5 * stt - 3]);
                root1.Nodes[ode].Nodes.Add(sssss[5 * stt - 2]);
                root1.Nodes[ode].Nodes.Add(sssss[5 * stt - 1]);
                root1.Nodes[ode].Nodes.Add(sssss[5 * stt ]);
                ode++;
            }
   
            #endregion

            #region 多点监测
            if (bool1)
            {
                for (int s = 0; s < root1.Nodes.Count; s++)
                {
                    //root1.Nodes[s].Nodes.Add(canUse[s].Substring(0, 2) + "转速");
                    //root1.Nodes[s].Nodes.Add(canUse[s].Substring(0, 2) + "温度");
                    root1.Nodes[s].Nodes.Add("转速");
                    /*
                    root1.Nodes[s].Nodes.Add("温度");
                    for (int i = 0; i < 6;i++ )
                    {
                        root1.Nodes[s].Nodes[5].Nodes.Add(seleTemNodes[i]);
                    }
                   
                    //root1.Nodes[s].Nodes.Add(canUse[s].Substring(0, 2) + "温湿度");
                    root1.Nodes[s].Nodes.Add( "温湿度");
                    for (int i = 0; i < 2; i++)
                    {
                        root1.Nodes[s].Nodes[6].Nodes.Add(seleRHNodes[i]);
                    }
                    */
                }
                treeView1.CheckBoxes = true;
            } 
            #endregion

            #region 多项监测
            if (bool2)
            {
                List<string> tr = new List<string>();//存放一级树节点
                foreach (TreeNode tn in treeView1.Nodes[0].Nodes)
                {
                    tr.Add(tn.Text);
                }
                treeView1.Nodes.Clear();
                root1 = treeView1.Nodes.Add(sssss[0]);  //父节点
                for (int i = 0; i < tr.Count; i++)
                {
                    root1.Nodes.Add(tr[i]);
                    //root1.Nodes[i].Nodes.Add(canUse[i].Substring(0, 2) + "温度");
                    root1.Nodes[i].Nodes.Add( "温度");
                  
                    for (int a = 0; a < 6; a++)
                    {
                        root1.Nodes[i].Nodes[0].Nodes.Add(seleTemNodes[a]);
                    }
                  
                    //root1.Nodes[i].Nodes.Add(canUse[i].Substring(0, 2) + "温湿度");
                    root1.Nodes[i].Nodes.Add( "温湿度");
                    for (int a = 0; a < 2; a++)
                    {
                        root1.Nodes[i].Nodes[1].Nodes.Add(seleRHNodes[a]);
                    }
                }
                treeView1.CheckBoxes = true;
            } 
            #endregion

            #region 振动监测
            //if (bool3)
            //{
            //    treeView1.CheckBoxes = true;
            //} 
            #endregion

            #region 三维图
            //if (bool3)
            //{
            //    treeView1.CheckBoxes = true;
            //} 
            #endregion

            root1.ExpandAll();                        
        }
        /// <summary>
        /// 单曲线显示区
        /// </summary>
        /// <param name="childNode"></param>
        /// <param name="parentNode"></param>
        /// <param name="n">横坐标显示文字的最大间隔</param>
        int plainDefault = 200;
        private void MyMasterPane1(string childNode, string parentNode) // Added by Kier: 实际绘图的函数，其实是构建绘图的形式
        {
            kChartPanel1.Panel.Clear();
            kChartPanel1.PanelRow = 2;
            kChartPanel1.PanelCol = 1;
            //标题
            kChartPanel1.Title = parentNode + "状态在线监测";
            //添加显示面板
            for (int a = 0; a < 2; a++)
            {
                KPanelSetting ps = new KPanelSetting();
                kChartPanel1.Panel.Add(ps);
            }
            kChartPanel1.Panel[0].Title = childNode + "振动信号在线监测";
            kChartPanel1.Panel[0].YAxisTitle = "幅值";
            //MasterPan1[0].Chart.Border.Color = Color.White;
            kChartPanel1.Panel[1].Title = childNode + "振动信号频谱";
            kChartPanel1.Panel[1].XAxisTitle = "频率/HZ";
            kChartPanel1.Panel[1].YAxisTitle = "幅值";

            kChartPanel1.Panel[0].AxisXMarkTrankfer = x => (x * 1000 / nowSTotal).ToString() + "ms";  //1秒的延迟

            // 注意，横轴显示为时间
            //MasterPan1[0].XAxis.Scale.Format = "mm:ss";

            // 获取显示的大小
            int plain = plainDefault;
            try
            {
                plain = int.Parse(toolStripComboBox2.Text);
            }
            catch
            {

            }

            //为面板添加线
            kChartPanel1.Panel[0].Line = list10;
            kChartPanel1.Panel[1].Line = list11;
            //坐标设置
            kChartPanel1.Panel[0].YAxisMin = -plain;
            kChartPanel1.Panel[0].YAxisMax = plain;
            kChartPanel1.Panel[0].XAxisMin = -nowSTotal;//最小值 
            kChartPanel1.Panel[0].XAxisMax = 0;//最小值 

            kChartPanel1.Panel[1].XAxisMin = 0;
            kChartPanel1.Panel[1].XAxisMax = 360;//最小值

            // 设置自动属性
            //kChartPanel1.Panel[0].AutoX = true;
            //kChartPanel1.Panel[0].AutoY = true;
            //kChartPanel1.Panel[1].AutoX = true;
            kChartPanel1.Panel[1].AutoY = true;

            kChartPanel1.Refresh();
        }
        /// <summary>
        /// 多点显示区
        /// </summary>
        private void MyMasterPane2()
        {
            kChartPanel2.Panel.Clear();
            kChartPanel2.PanelRow = nodesc.Count;
            kChartPanel2.PanelCol = 1;
            kChartPanel2.Title = "多点状态在线监测";
            ColorSymbolRotator rotator = new ColorSymbolRotator();
            for (int a = 0; a < nodesc.Count; a++)
            {
                KPanelSetting ps = new KPanelSetting();
                kChartPanel2.Panel.Add(ps);


                int plain = plainDefault;
                try { plain = int.Parse(toolStripComboBox2.Text); }
                catch { }

                kChartPanel2.Panel[a].XAxisMin = 0;//最小值 
                if (nodesc[a].Substring(2, 2) == "温度")
                {
                    int ef = int.Parse(nodesc[a].Substring(4, 1));
                    kChartPanel2.Panel[a].Title = sssss[5 * int.Parse(nodesc[a].Substring(0, 1)) - 4] + seleTemNodes[ef] + "温度信号在线监测";
                    //MasterPan2[a].XAxis.Scale.MajorStep = data1[nodesc[a].Substring(0, 4)].Count / 6;//步长  
                    //myCurve20 = MasterPan2[a].AddCurve("单位：℃", pairLists[a], rotator.NextColor, SymbolType.None);
                    kChartPanel2.Panel[a].Line = pairLists[a];
                    kChartPanel2.Panel[a].YAxisMin = 0;//最小值 
                    kChartPanel2.Panel[a].YAxisMax = 100;//最da值 
                    kChartPanel2.Panel[a].XAxisMin = -300;
                    kChartPanel2.Panel[a].XAxisMax = 0;
                    kChartPanel2.Panel[a].AxisXMarkTrankfer = x => (x / 60).ToString() + "min";

                    //kChartPanel2.Panel[a].AutoX = true;
                }
                else if (nodesc[a].Substring(2, 2) == "温湿")
                {
                    int ef = int.Parse(nodesc[a].Substring(5, 1));
                    kChartPanel2.Panel[a].Title = sssss[5 * int.Parse(nodesc[a].Substring(0, 1)) - 4] + seleRHNodes[ef] + "信号在线监测";
                    //MasterPan2[a].XAxis.Scale.MajorStep = data1[nodesc[a].Substring(0, 5)].Count / 2;//步长  
                    if (ef==0)
                    {
                        //myCurve20 = MasterPan2[a].AddCurve("单位：℃", pairLists[a], rotator.NextColor, SymbolType.None); 
                        kChartPanel2.Panel[a].Line = pairLists[a];
                    }
                    if (ef == 1)
                    {
                        //myCurve20 = MasterPan2[a].AddCurve("单位：RH%", pairLists[a], rotator.NextColor, SymbolType.None);
                        kChartPanel2.Panel[a].Line = pairLists[a];
                    }
                    kChartPanel2.Panel[a].YAxisMin = 0;//最小值 
                    kChartPanel2.Panel[a].YAxisMax = 100;//最da值 
                    kChartPanel2.Panel[a].XAxisMin = -300;
                    kChartPanel2.Panel[a].XAxisMax = 0;
                    kChartPanel2.Panel[a].AxisXMarkTrankfer = x => (x / 60).ToString() + "min";

                    //kChartPanel2.Panel[a].AutoX = true;

                }
                else if (nodesc[a].Substring(2, 2) == "转速")
                {
                    kChartPanel2.Panel[a].Title = sssss[5 * int.Parse(nodesc[a].Substring(0, 1)) - 4] + "转速信号在线监测";
                    //myCurve20 = MasterPan2[a].AddCurve("单位：r/min", pairLists[a], rotator.NextColor, SymbolType.None);
                    kChartPanel2.Panel[a].Line = pairLists[a];
                    kChartPanel2.Panel[a].YAxisMin = 0;//最小值 
                    kChartPanel2.Panel[a].YAxisMax = 3000;//最da值
                    kChartPanel2.Panel[a].XAxisMin = -300;
                    kChartPanel2.Panel[a].XAxisMax = 0;

                    kChartPanel2.Panel[a].AxisXMarkTrankfer = x => (x / 60).ToString() + "min";

                    //kChartPanel2.Panel[a].AutoX = true;
                }
                else
                {
                    kChartPanel2.Panel[a].Title = sssss[5 * int.Parse(nodesc[a].Substring(0, 1)) - 4] +
                        sssss[5 * int.Parse(nodesc[a].Substring(0, 1)) - 4 + int.Parse(nodesc[a].Substring(4, 1))] + "振动信号在线监测";//
                    //MasterPan2[a].XAxis.IsVisible = false;
                    //myCurve20 = MasterPan2[a].AddCurve("幅值", pairLists[a], rotator.NextColor, SymbolType.None);
                    kChartPanel2.Panel[a].Line = pairLists[a];
                    kChartPanel2.Panel[a].YAxisMin = -plain;
                    kChartPanel2.Panel[a].YAxisMax = plain;
                    kChartPanel2.Panel[a].XAxisMin = -nowSTotal;//最小值 
                    kChartPanel2.Panel[a].XAxisMax = 0;

                    kChartPanel2.Panel[a].AxisXMarkTrankfer = x => (x * 1000 / nowSTotal).ToString() + "ms";  //1秒的延迟

                    //kChartPanel2.Panel[a].AutoX = true;
                }
                //myCurve20 = MasterPan2[a].AddCurve(nodesc[a], pairLists[a], GetColor(), SymbolType.None);
            }

            //myCurve20.Line.IsAntiAlias = true;//抗锯齿效果
            kChartPanel2.Refresh();
            //禁止缩放功能
            ////文字不缩放
            //zedGraphData.GraphPane.IsFontsScaled = false;
            //zedGraphElec.GraphPane.IsFontsScaled = false;

            ////是否自动绘制矩形，自己搞
            //zedControl2.MasterPane[0].Chart.IsRectAuto = false;
            //zedControl2.MasterPane[1].Chart.IsRectAuto = false;

            //if(nodesc.Count==1)
            //{
            //    zedControl2.Size = new Size(panel1.Width,panel1.Height);//==============================================菜单栏可以改
            //    //zedControl2.IsEnableHZoom = true;
            //    zedControl2.IsEnableVZoom = true;
            //    //zedControl2.IsEnableHPan = true;
            //    zedControl2.IsEnableVPan = true;
            //    zedControl2.IsEnableWheelZoom = true;
            //    MasterPan2.Legend.IsVisible = false;//中间条框
            //}
            //using (Graphics g = zedControl2.CreateGraphics())
            //{
            //    for (int j = 0; j < nodesc.Count; j++)
            //    {
            //        zedControl2.MasterPane[j].ReSize(g, 
            //            new RectangleF(0, 40, zedControl2.Width-100, zedControl2.Height / nodesc.Count));  
            //    }
            //    MasterPan2.SetLayout(g, PaneLayout.SingleColumn);
            //    MasterPan2.AxisChange(g);
            //    zedControl2.Refresh();
            //}
        }
        /// <summary>
        /// 多项显示区
        /// </summary>
        private void MyMasterPane3() 
        {
            kMultiPanel1.Clear();
            kMultiPanel1.Title = "多轴数据监测";
            kMultiPanel1.FixedYAxisMin = 0;//最小值 
            kMultiPanel1.FixedYAxisMax = 100;//最小值 
            kMultiPanel1.FixedXAxisMin = -3600;//最小值 
            kMultiPanel1.FixedXAxisMax = 0;//最小值
            kMultiPanel1.AxisXMarkTrankfer = x => (x / 60).ToString() + "min";
            string[] labels = { "5分钟前", "", "", "", "", "", "", "", "", "", "", "", 
                              "4分钟前", "", "", "", "", "", "", "", "", "", "", "", 
                              "3分钟前",  "", "", "", "", "", "", "", "", "", "", "", 
                              "2分钟前",  "", "", "", "", "", "", "", "", "", "", "", 
                              "1分钟前",  "", "", "", "", "", "", "", "", "", "", "", "当前" };
            //string[] labels = { "60min前", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 
            //                  "45min前", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
            //                  "30min前",  "", "", "", "", "", "", "", "", "", "", "", "", "", "", 
            //                  "15min前",  "", "", "", "", "", "", "", "", "", "", "", "", "", "",  "当前"};
            for (int i = 0; i < nodesc.Count; ++i)
            {

                if (!showLine.ContainsKey(nodesc[i])) showLine.Add(nodesc[i], new LinkedList<KTuple>());
                kMultiPanel1.AddLine(DistinguishNode(nodesc[i]), showLine[nodesc[i]]);
                //pairLists[i].AddLast(new KTuple(1, data1[nodesc[i].Substring(0, 2) + "温度"][int.Parse(nodesc[i].Substring(4, 1))] / 10));
            }

            kMultiPanel1.Refresh();
        
        }
        /// <summary>
        /// 振动特征
        /// </summary>
        private void MyMasterPane4()
        {
            kChartPanel4.Panel.Clear();
            kChartPanel4.PanelRow = 2;
            kChartPanel4.PanelCol = 3;
            kChartPanel4.Title = Paent + ent + "振动信号固有特征在线监测";
          
            #region 特征参数监测
            if (!checkBox1.Checked)
            {
                // Initialize a color and symbol type rotator
                ColorSymbolRotator rotator = new ColorSymbolRotator();
                string[] sda = { "方差", "均方根", "峭度", "峰值指标", "脉冲指标" };
                pairLists.Clear();
                // Create some new GraphPanes
                // 监测要显示的数据
                for (int j = 0; j < 5; j++)
                {
                    KPanelSetting ps = new KPanelSetting();

                    ps.Title = sda[j];
                    ps.XAxisTitle = "";
                    ps.YAxisTitle = "";
                    ps.AutoY = true;
                    ps.XAxisMin = -1800;
                    ps.XAxisMax = 0;
                    ps.AxisXMarkTrankfer = x => (x / 60).ToString() + "min";

                    LinkedList<KTuple> list = new LinkedList<KTuple>();

                    ps.Line = list;
                    pairLists.Add(list);                           

                    kChartPanel4.Panel.Add(ps);
                }
            } 
            #endregion

            #region 散点图
            if (checkBox1.Checked)
            {
                /*
                GraphPane gp = new GraphPane();
                MasterPan4.Add(gp);
                gp.Fill = new Fill(Color.White, Color.Tomato, 45.0F);
                MasterPan4.Legend.IsVisible = false;
                gp.Legend.IsVisible = false;
                gp.YAxis.MajorGrid.IsZeroLine = false;
                gp.XAxis.Title.Text = "转速  r/s";
                gp.Title.Text = "特征轨迹图";
                //gp.YAxis.Scale.Min = StaticData.LS[comboBox1.Text].X;
                //gp.YAxis.Scale.Max = StaticData.LS[comboBox1.Text].Y;
                #region 加窗
                //// Draw a box item to highlight a value range
                //BoxObj box = new BoxObj(0, StaticData.LS[comboBox1.Text].Z, 25,
                //    StaticData.LS[comboBox1.Text].T, Color.Empty,Color.FromArgb(150, Color.Red));
                //box.Fill = new Fill(Color.White, Color.FromArgb(50, Color.Red), 45.0F);
                //// Use the BehindGrid zorder to draw the highlight beneath the grid lines
                //box.ZOrder = ZOrder.F_BehindGrid;
                //gp.GraphObjList.Add(box);
                #endregion
                list40 = new PointPairList();
                gp.YAxis.Title.Text = comboBox1.Text;
                myCurve40 = gp.AddCurve(comboBox1.Text,
                       list40, Color.Blue, SymbolType.Diamond);
                myCurve40.Symbol.Fill = new Fill(Color.Green, Color.Red);//填满实心方块
                //myCurve40.Symbol.Fill.Type = FillType.GradientByZ;
                myCurve40.Symbol.Border.IsVisible = false;
                myCurve40.Symbol.Fill.SecondaryValueGradientColor = Color.Empty;
                myCurve40.Symbol.Fill.RangeMin = 2.000;
                myCurve40.Symbol.Fill.AlignH = AlignH.Center;
                myCurve40.Symbol.Fill.RangeMax = 2.2;
                //myCurve40.Line.IsVisible = false;
                myCurve40.Line.IsSmooth = true;//曲线是否不规则
                myCurve40.Line.SmoothTension = 0.6F;
                myCurve40.Line.Width = 5;
                //zedGraphControl1.MasterPane[0].LineType = LineType.Stack;

                using (Graphics g = zedControl4.CreateGraphics())
                {
                    MasterPan4.SetLayout(g, PaneLayout.SingleColumn);
                    MasterPan4.AxisChange(g);
                }
                */
            }
            #endregion

            kChartPanel4.Refresh();
        }
        /// <summary>
        /// 区别选中的是温度节点还是湿度节点，配合MyMasterPane3（）
        /// </summary>
        /// <param name="dnode"></param>
        /// <returns></returns>
        private string DistinguishNode(string dnode) 
        {
            string dnode1 = "";
            if (dnode.Substring(2, 2)=="温度")
            {
                dnode1=seleTemNodes[int.Parse(dnode.Substring(4,1))]+"温度";
            }
            if (dnode.Substring(2, 2) == "温湿")
            {
                dnode1 = seleRHNodes[int.Parse(dnode.Substring(5, 1))];
            }
            dnode1 = sssss[5 * int.Parse(dnode.Substring(0, 1)) - 4] + dnode1;
            return dnode1;
        }
        /// <summary>
        /// 傅里叶变换
        /// </summary>
        private void FFt() 
        {
            #region 1
            double[] inin = new double[singleList.Count];
            double[] outout = new double[singleList.Count];
            //MasterPan1[1].XAxis.Scale.Min = 0;//最小值
            //MasterPan1[1].XAxis.Scale.Max = 1000;//最da值
            for (int j = 0; j < singleList.Count; j++)
            {
                inin[j] = (double)singleList[j];
                outout[j] = (double)0;
            }
            int a = me.upFFT(inin, outout, 1);//FFT计算，1，输出幅角；0输出实部虚部
            inin[0] = 0;
            list11.Clear();
            frequList = new double[a];
            for (int i = 0; i < 500; i++)//只显示前500个点
            {

                frequList[i] = (double)93750 / (StaticData.DSDS + 1) * (double)i / (double)a;//频率序列赋值
                list11.AddLast(new KTuple(frequList[i], inin[i]));
            }
            //kChartPanel1.Refresh();
            list111 = new LinkedList<KTuple>(list11);//排序,找出最大值、最小值

            // 注意，这里有一个链表的排序
            //list111.Sort(s);
            // 按照Y值增序，X增序排列
            KUtil.SortList(list111, (t1, t2) => t1.Y < t2.Y ? true : (t1.Y == t2.Y && t1.X < t2.X ? true : false));

            textBox2.Text = String.Format("{0:F}", list111.Last.Value.X);
            textBox4.Text = String.Format("{0:F}", list111.Last.Previous.Value.X);
            textBox5.Text = String.Format("{0:F}", list111.Last.Previous.Previous.Value.X);
            textBox6.Text = String.Format("{0:F}", list111.Last.Previous.Previous.Previous.Value.X);
            singleList.Clear(); 
            #endregion
        }
        /// <summary>
        /// 多项和多点显示
        /// </summary>

        int second1 = 0;
        private void LineShow() 
        {
            int nnnnnTick = System.Environment.TickCount;
            num1++;
            if (nnnnnTick - controlTick >= 1000)
            {
                second1++;
                controlTick += 1000;
                num1 = 0;
            }

            #region 2
            if (bool1)
            {
                //DateTime dt = DateTime.Now;
               // double x0 = (double)new XDate(dt);

                int sTotal = nowSTotal;

                int pointNeedToBeShowCount = sTotal * showTicks2 / 1000; // 这个是要显示的点的个数，从showIndex开始的这么多点

                bool needTobeRest = false;
               
                for (int i = 0; i < nodesc.Count; i++)
                {
                    if (nodesc[i].Substring(2, 2) == "转速")//1s显示一次，共显示5分钟
                    {
                        if (num1 != 1) continue;    // 表示1s啊，一秒里就一次num1=1啊，
                        pairLists[i].AddLast(new KTuple(1, (ushort)data1[nodesc[i]][0]/4));
                        if (pairLists[i].Count >= 300) //showTrackBar
                        {
                            pairLists[i].RemoveFirst();
                        }
                        KUtil.ListMap(pairLists[i], t => new KTuple(t.X - 1, t.Y));
                    }
                    else if (nodesc[i].Substring(2, 2) == "温度")//5s显示一次，共显示5分钟
                    {
                        if (second1 >= 5)
                        {
                            pairLists[i].AddLast(new KTuple(1, data1[nodesc[i].Substring(0, 2) + "温度"][int.Parse(nodesc[i].Substring(4, 1))] / 10));
                            if (pairLists[i].Count >= 60) //showTrackBar
                            {
                                pairLists[i].RemoveFirst();
                            }
                            KUtil.ListMap(pairLists[i], t => new KTuple(t.X - 1, t.Y));
                        }
                    }
                    else if (nodesc[i].Substring(2, 2) == "温湿" )//5s显示一次，共显示5分钟
                    {
                        if (second1 >= 5)
                        {
                            //string[] ss=data1.Keys.ToArray();
                            
                            //string[] ss1=nodesc.ToArray();
                            //string saasda="",ssasab="";
                            //for (int h = 0; h < ss.Length;h++ )
                            //{
                            //    saasda = saasda +" " +ss[h];
                            //}
                            //for (int h = 0; h < ss1.Length; h++)
                            //{
                            //    ssasab = ssasab + " " + ss1[h];
                            //}
                            //LimitAlarm(saasda+"\n"+ssasab);

                            pairLists[i].AddLast(new KTuple(1, data1[nodesc[i].Substring(0, 2) + "温湿度"][int.Parse(nodesc[i].Substring(5, 1))]));
                            if (pairLists[i].Count >= 60) //showTrackBar
                            {
                                pairLists[i].RemoveFirst();
                            }
                            KUtil.ListMap(pairLists[i], t => new KTuple(t.X - 1, t.Y));
                        }
                    }
                    else //振动
                    {
                        if (!multiList.ContainsKey(nodesc[i]) || multiList[nodesc[i]].Count == 0)
                        {
                            needTobeRest = true;
                            continue;
                        }

                        int ic = 0; // ic表示加入表中的点个数
                        for (int k = 0; k < pointNeedToBeShowCount; k += showTrackBar)    // ic
                        {
                            int index = (showIndex2 + k) / showTrackBar * showTrackBar;
                            if (index >= multiList[nodesc[i]].Count) continue;
                            if (pairLists[i].Count >= sTotal / showTrackBar) pairLists[i].RemoveFirst();
                            pairLists[i].AddLast(new KTuple((++ic) * showTrackBar, multiList[nodesc[i]][index]));
                        }
                        KUtil.ListMap(pairLists[i], t => new KTuple(t.X - showTrackBar * ic, t.Y));

                        if (showIndex2 + pointNeedToBeShowCount >= sTotal)
                        {
                            multiList[nodesc[i]].RemoveRange(0, sTotal);
                        }

                        //kChartPanel2.Refresh();
                    }
                }

                if (needTobeRest)
                {
                    lastTick2 = System.Environment.TickCount;   // 设置计数
                    showIndex2 = 0;
                }
                else
                {
                    showIndex2 += pointNeedToBeShowCount;

                    bool oneSecondPass = showIndex2 >= sTotal;

                    if (oneSecondPass)
                    {
                        showIndex2 -= sTotal;
                    }
                }


                kChartPanel2.Refresh();
                if (second1 >= 5)
                    second1 = 0;
            }
            else
            {
                lastTick2 = System.Environment.TickCount;   // 设置计数
                showIndex2 = 0;
            }
            #endregion

            #region 3
            if (bool2 && second1 >= 5)//num1用来控制时间num1=n，代表n秒添加一个点
            {
                // DateTime dt = DateTime.Now;
                // 这里更新状态不在这儿了，在获得数据的时候就会更新点
                /*
                double x0 = (double)new XDate();
               
                for (int i = 0; i < nodesc.Count; i++)
                {
                    if (nodesc[i].Substring(2, 2) == "温度")
                    {
                        pairLists[i].AddLast(new KTuple(1, data1[nodesc[i].Substring(0, 2) + "温度"][int.Parse(nodesc[i].Substring(4, 1))] / 10));
                        //pairLists[i].Add(x0, int.Parse(nodesc[i].Substring(4, 1)));
                        if (pairLists[i].Count >= 60) //showTrackBar
                        {
                            pairLists[i].RemoveFirst();
                        }
                        KUtil.ListMap(pairLists[i], t => new KTuple(t.X - 1, t.Y));
                    }
                    else if (nodesc[i].Substring(2, 2) == "温湿")
                    {
                        pairLists[i].AddLast(new KTuple(1, data1[nodesc[i].Substring(0, 2) + "温湿度"][int.Parse(nodesc[i].Substring(5, 1))]));
                        if (pairLists[i].Count >= 60) //showTrackBar
                        {
                            pairLists[i].RemoveFirst();
                        }
                        KUtil.ListMap(pairLists[i], t => new KTuple(t.X - 1, t.Y));
                    }

                }
                kMultiPanel1.Refresh();
                */
                second1 = 0;
            } 
            #endregion

            #region 4
            if (bool3 && second1 >= 1)
            {
                try
                {
                    if (singleList3.Count == 0)
                    {
                        //MessageBox.Show("hhh");
                        return;
                    }
                    List<short> kj = new List<short>(singleList3);
                    double[] bList = Array.ConvertAll<short, double>(kj.ToArray(), x => x);//short[]转换成double[]
                    double avg = Maths.Avg(bList);//均值
                    double max = Maths.Max(bList);//峰值
                    double pro = Maths.PRO(bList);//平均幅值
                    double rms = Maths.RMS(bList);//均方根
                    double var = Maths.Var(bList, avg);//方差
                    double kurtosis = Maths.Kurtosis(bList, avg);//峭度 Maths.Kurtosis(bList, avg)
                    double fzzb = max / rms;//峰值指标
                    double mczb = max / pro;//脉冲指标
                    textBox7.Text = String.Format("{0:F}", var);
                    textBox8.Text = String.Format("{0:F}", rms);
                    textBox9.Text = String.Format("{0:F}", kurtosis);
                    textBox10.Text = String.Format("{0:F}", fzzb);
                    textBox11.Text = String.Format("{0:F}", mczb);
                    DateTime dt = DateTime.Now;
                    double x0 = (double)new XDate(dt);
                    if (!checkBox1.Checked)
                    {
                        for (int i = 0; i < 5; ++i)
                        {
                            pairLists[i].Clear();
                            KUtil.ListAddRange(pairLists[i], showingLineState[i]);
                        }
                        // 下面是旧的显示数据
                        /*
                        pairLists[0].AddLast(new KTuple(1, var));
                        KUtil.ListMap(pairLists[0], t => new KTuple(t.X - 1, t.Y));
                        pairLists[1].AddLast(new KTuple(1, rms));
                        KUtil.ListMap(pairLists[1], t => new KTuple(t.X - 1, t.Y));
                        pairLists[2].AddLast(new KTuple(1, kurtosis));
                        KUtil.ListMap(pairLists[2], t => new KTuple(t.X - 1, t.Y));
                        pairLists[3].AddLast(new KTuple(1, fzzb));
                        KUtil.ListMap(pairLists[3], t => new KTuple(t.X - 1, t.Y));
                        pairLists[4].AddLast(new KTuple(1, mczb));
                        KUtil.ListMap(pairLists[4], t => new KTuple(t.X - 1, t.Y));
                        if (pairLists[1].Count > 1800)
                        {
                            pairLists[0].RemoveFirst();
                            pairLists[1].RemoveFirst();
                            pairLists[2].RemoveFirst();
                            pairLists[3].RemoveFirst();
                            pairLists[4].RemoveFirst();
                        }
                        */
                    }
                    if (checkBox1.Checked)
                    {
                        ditcSanDian = new Dictionary<string, double>();
                        ditcSanDian.Add("方差", var);
                        ditcSanDian.Add("均方根", rms);
                        ditcSanDian.Add("峭度", kurtosis);
                        ditcSanDian.Add("峰值指标", fzzb);
                        ditcSanDian.Add("脉冲指标", mczb);
                        double dsd = rd.Next(0, 25);
                        list40.Add(dsd, ditcSanDian[comboBox1.Text]);
                        if (list40.Count > 10)
                        {
                            list40.RemoveAt(0);
                        }

                    }
                    kChartPanel4.Refresh();
                    singleList3.Clear();
                    second1 = 0;
                }
                catch (Exception eee)
                {
                    MessageBox.Show(eee.ToString());
                }
            }
            #endregion
           
        }
        /// <summary>  
        /// 随机成一个颜色  
        /// </summary>  
        /// <returns></returns>  
        private Color GetColor()
        {
            Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            //  对于C#的随机数，没什么好说的   
            System.Threading.Thread.Sleep(RandomNum_First.Next(100));
            Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);
            //  为了在白色背景上显示，尽量生成深色   
            int int_Red = RandomNum_First.Next(256);
            int int_Green = RandomNum_Sencond.Next(256);
            int int_Blue = (int_Red + int_Green > 400) ? 0 : 400 - int_Red - int_Green;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;
            return Color.FromArgb(int_Red, int_Green, int_Blue);
        }
        /// <summary>
        /// 线程，联网不成功，一直检测连接状态
        /// </summary>
        private void ThreadIsConnect()
        {
            while (true)
            {
                try
                {
                    MF = (MainForm)this.Owner;
                    if (MF.ISConnected && 
                        MF.Datadict.Count == 8 * SystemConfig.GetConfigData("选用装置", string.Empty).Split('|').Length)
                    {
                        //MessageBox.Show("检测到连接状态");
                        timer2.Stop();
                        timer1.Stop();
                        canUse = new List<string>();
                        data0 = MF.Datadict;
                        List<string> ls = new List<string>(data0.Keys);
                        for (int i = 0; i < data0.Keys.Count / 8; i++)
                        {
                            canUse.Add(ls[8 * i]);
                        }
                        //canUse.Add(data0.Keys);
                        CreatTree();
                        treeView1.SelectedNode = treeView1.Nodes[0].Nodes[0].Nodes[0];
                        ent = treeView1.Nodes[0].Nodes[0].Nodes[0].Text;
                        Paent = treeView1.Nodes[0].Nodes[0].Text;
                        MyMasterPane1(ent, Paent);
                        Initial3D();
                        timer2.Interval = showTimeTnterval;
                        timer2.Start();
                        timer1.Interval = timeInterval;//无论单点还是多点，都开启time1
                        timer1.Start();

                        this.Refresh();
                        break;
                    }
                }
                catch (Exception)
                {
                    
                    break;
                }
            }
            threadISconnect.Abort();
        }
        /// <summary>
        /// 多项监测时获取树子节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private TreeNode GetNode(TreeNode node)
        {
            //node:"1#温度1"、"1#温湿度1"、"1#转速"、"1#通道1"
            if (node.Nodes.Count == 0)
            {
                
                if (node.Checked)
                {
                    
                    if (node.Parent.Text.Substring(0, 2) == "温度")
                    {
                        //nodesc.Add(node.Parent.Text.Substring(0, 2) +"温度"+ node.Index.ToString());
                        nodesc.Add(canUse[node.Parent.Parent.Index].Substring(0, 2) + "温度" + node.Index.ToString());
                    }
                    else if (node.Parent.Text.Substring(0, 2) == "温湿")
                    {
                        nodesc.Add(canUse[node.Parent.Parent.Index].Substring(0, 2) + "温湿度" + node.Index.ToString());
                    }
                    else if (node.Text.Substring(0, 2) == "转速")
                    {
                        nodesc.Add(canUse[node.Parent.Index].Substring(0, 2) + "转速");
                    }
                    else 
                    {
                        nodesc.Add(canUse[node.Parent.Index].Substring(0, 2) +"通道"+(node.Index+1).ToString()); 
	                }
                    //if (node.Text.Length >= 4)
                    //{
                    //    nodesc.Add(node.Text);
                    //}
                    //else 
                    //{
                    //   // MessageBox.Show(node.Parent.Index.ToString());
                    //    nodesc.Add(canUse[node.Parent.Index].Substring(0,2)+node.Text); 
                    //}
                }
                return new TreeNode(node.Text);
            }
            TreeNode ns = new TreeNode(node.Text);
            foreach (TreeNode item in node.Nodes)
            {
                TreeNode n = GetNode(item);

                ns.Nodes.Add(n);
            }
            return ns;
        }
        /// <summary>
        ///  取消节点选中状态之后，取消所有父节点的选中状态
        /// </summary>
        /// <param name="currNode"></param>
        /// <param name="state"></param>
        private void setParentNodeCheckedState(TreeNode currNode, bool state)
        {
            TreeNode parentNode = currNode.Parent;

            parentNode.Checked = state;
            if (currNode.Parent.Parent != null)
            {
                setParentNodeCheckedState(currNode.Parent, state);
            }
        }
        /// <summary>
        /// 选中节点之后，选中节点的所有子节点
        /// </summary>
        /// <param name="currNode"></param>
        /// <param name="state"></param>
        private void setChildNodeCheckedState(TreeNode currNode, bool state)
        {
            TreeNodeCollection nodes = currNode.Nodes;
            if (nodes.Count > 0)
                foreach (TreeNode tn in nodes)
                {

                    tn.Checked = state;
                    setChildNodeCheckedState(tn, state);
                }
        }
        /// <summary>
        /// tabControlChange使2,3tabpage直接显示
        /// </summary>
        private void tabControlChangeTreeNode()
        {
            timer2.Stop();
            num1 = 0;
            pairLists.Clear();
            nodesc.Clear();

            foreach (TreeNode item in treeView1.Nodes)//遍历Treeview的所有节点
            {
                TreeNode node = GetNode(item);//遍历子节点，返回值其实没用
            }
            for (int i = 0; i < nodesc.Count; i++)
            {
                LinkedList<KTuple> pointPairList = new LinkedList<KTuple>();
                pairLists.Add(pointPairList);
            }
            if (nodesc.Count != 0 && bool1)
            {
                MyMasterPane2();
            }
            if (nodesc.Count != 0 && bool2)
            {
                MyMasterPane3();
            }
            timer2.Start();
        }
        /// <summary>
        /// 越限报警
        /// </summary>
        /// <param name="dictDeliver"></param>
        public void LimitAlarm(string limitAlarm)
        {
            lock (this)
            {
                listBox1.Items.Insert(0, DateTime.Now.ToString() + " <=> " +limitAlarm);
                //最近两次错误如果相同。则不再重复显示
                //if (listBox1.Items.Count>1&&listBox1.Items[listBox1.Items.Count - 1].ToString().Substring(0, 10)
                //    == listBox1.Items[listBox1.Items.Count - 2].ToString().Substring(0, 10))
                //{
                //    listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                //}
                //控制个数
                if (listBox1.Items.Count > 30)
                {
                    listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                }
                foreach(TreeNode tn in treeView1.Nodes)
                {
                    foreach (TreeNode tn1 in tn.Nodes)
                    {
                        if(tn1.Text==limitAlarm.Substring(0,4))
                        {
                            tn1.BackColor = Color.Red;//若有故障，则变红
                            break;
                        }
                    }
                }
            }
        }
        #endregion
       
        #region 一般事件
        /// <summary>
        /// 选中树节点复选框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (e.Action == TreeViewAction.ByMouse)
                {
                    //tabControlChangeTreeNode(sender, e);
                    #region MyRegion
                    timer2.Stop();//停止显示
                    num1 = 0;
                    pairLists.Clear();
                    nodesc.Clear();
                    if (e.Node.Checked)
                    {
                        //节点选中状态之后，所有子节点的选中状态
                        setChildNodeCheckedState(e.Node, true);
                    }
                    else
                    {
                        //取消节点选中状态之后，取消所有子节点的选中状态
                        setChildNodeCheckedState(e.Node, false);
                        //如果节点存在父节点，取消父节点的选中状态
                        if (e.Node.Parent != null)
                        {
                            setParentNodeCheckedState(e.Node, false);
                        }
                    }
                    foreach (TreeNode item in treeView1.Nodes)//遍历Treeview的所有节点
                    {
                        TreeNode node = GetNode(item);//遍历子节点，返回值其实没用
                    }
                    for (int i = 0; i < nodesc.Count; i++)
                    {
                        LinkedList<KTuple> pointPairList = new LinkedList<KTuple>();
                        pairLists.Add(pointPairList);
                    }
                    if (nodesc.Count != 0 && bool1)
                    {
                        MyMasterPane2();
                    }
                    if (nodesc.Count != 0 && bool2)
                    {
                        MyMasterPane3();
                    }
                    if (nodesc.Count != 0 && bool3)
                    {
                        MyMasterPane4();
                    }
                    timer2.Start();
                    #endregion

                }
            }
            catch (OverflowException ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
           

        }
        /// <summary>
        /// 选中节点
        /// </summary>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                #region MyRegion
                if (e.Node.Nodes.Count == 0)//选中子节点
                {
                    string realname = canUse[e.Node.Parent.Index].Substring(0, 2) + "通道" + (e.Node.Index + 1).ToString();
                    if (tabControl2.SelectedTab.Text == "单点监测")
                    {
                        singleList.Clear();//FFT用
                        singleList1.Clear();//显示用
                        list10.Clear();
                        list11.Clear();
                        if (!data0.ContainsKey(realname))
                        {
                            MessageBox.Show("此传感器没有数据，请检测连接状态或选择其他传感器");
                            e.Node.BackColor = Color.Red;
                            e.Node.Parent.BackColor = Color.Red;
                            kChartPanel1.Refresh();
                            return;
                        }
                        e.Node.BackColor = Color.Empty;
                        e.Node.Parent.BackColor = Color.Empty;
                        ent = e.Node.Text;
                        Paent = e.Node.Parent.Text;
                        MyMasterPane1(ent, Paent);
                    }
                    if (tabControl2.SelectedTab.Text == "振动特征")
                    {
                        if (!data0.ContainsKey(realname))
                        {
                            MessageBox.Show("此传感器没有数据，请检测连接状态或选择其他传感器");
                            return;
                        }
                        ent = e.Node.Text;
                        Paent = e.Node.Parent.Text;
                        singleList.Clear();

                        MyMasterPane4();
                    }
                    if (tabControl2.SelectedTab.Text == "瀑布图")
                    {
                        if (!data0.ContainsKey(realname))
                        {
                            MessageBox.Show("此传感器没有数据，请检测连接状态或选择其他传感器");
                            return;
                        }
                        singleList.Clear();
                        ent = e.Node.Text;
                        Paent = e.Node.Parent.Text;
                        Initial3D();
                    } 
                
                }
                else
                {
                    e.Node.Expand();
                }
                #endregion
            }
            catch (OverflowException ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
        }
        /// <summary>
       /// 接收服务器数据
        /// </summary>
        Dictionary<string, LinkedList<KTuple>> showLine = new Dictionary<string, LinkedList<KTuple>>();
        Dictionary<string, List<LinkedList<KTuple>>> showLineState = new Dictionary<string,List<LinkedList<KTuple>>>(); // 每个通道要记录5组数据，平均值，什么的巴拉拉
        List<LinkedList<KTuple>> showingLineState;
        Dictionary<string, List<short>> multiList = new Dictionary<string, List<short>>();
        List<short> singleList3 = new List<short>();
        List<short> singleList4 = new List<short>();
        int nowSTotal = 93500;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                #region MyRegion
                statusStrip1.Items[0].Text = "";
                statusStrip1.Items[0].Text = "当前日期及时间：" + DateTime.Now.ToString();//状态栏显示系统时间

                //#region 网速
                //double www = 0;
                //double wwww = 0;
                //NetworkAdapter adapter;
                //for (int i = 0; i < adapters.Length; i++)
                //{
                //    adapter = this.adapters[i];
                //    www = adapter.DownloadSpeedKbps + www;
                //    wwww = adapter.UploadSpeedKbps + wwww;
                //}
                //LableDownloadValue.Text = String.Format("{0:n} kbps", www);
                //LabelUploadValue.Text = String.Format("{0:n} kbps", wwww);

                //#endregion

                MF = (MainForm)this.Owner;
                if (MF.ISConnected == true)
                {
                    data0 = MF.Datadict;//获取数据
                    data1 = MF.Datadict;//获取数据
                    data2 = MF.Datadict;//获取数据

                    //listBox1.Items.Add("Timer1");

                    if ((treeView1.SelectedNode != null) && (treeView1.SelectedNode.Nodes.Count == 0) &&
                        (bool0 || bool3 || bool4) && (ButtonsEnabled))
                    {
                        //对应实际通道和名称
                        List<string> realname = data0.Keys.ToList<string>();
                        if(realname.Count<=0)
                        {
                            tick1++;
                            if (tick1<=10)
                            {
                                return; 
                            }
                        }
                        int loc = 8 * treeView1.SelectedNode.Parent.Index + treeView1.SelectedNode.Index + 1;
                        string asas = realname[loc];

                        #region MyRegion
                        if (bool0)
                        {
                            singleList.AddRange(data0[asas]);
                            singleList1.AddRange(data0[asas]);  // 单项检测时，使用的缓存数组
                            // 记录1s中有多少个点
                            if (nowSTotal != data0[asas].Count) nowSTotal = data0[asas].Count;
                            lock (this)
                            {
                                FFt();  //傅里叶变换
                            }
                        }
                        // 这里使用带有记忆的震动状态
                        if (!showLineState.ContainsKey(asas))
                        {
                            List<LinkedList<KTuple>> lineState = new List<LinkedList<KTuple>>();
                            lineState.Add(new LinkedList<KTuple>());
                            lineState.Add(new LinkedList<KTuple>());
                            lineState.Add(new LinkedList<KTuple>());
                            lineState.Add(new LinkedList<KTuple>());
                            lineState.Add(new LinkedList<KTuple>());
                            showLineState.Add(asas, lineState);
                        }
                        showingLineState = showLineState[asas];
                        // 原先的显示数据
                        if (bool3) 
                            singleList3.AddRange(data0[asas]);
                        if (bool4)
                        {
                            singleList.AddRange(data0[asas]);

                            double[] inin = new double[singleList.Count];
                            double[] outout = new double[singleList.Count];
                            for (int j = 0; j < singleList.Count; j++)
                            {
                                inin[j] = (double)singleList[j];
                                outout[j] = (double)0;
                            }

                            int a = me.upFFT(inin, outout, 1);
                            frequList = new double[a];
                            if (StaticData.ListPoint.Count >= 10)
                            {
                                StaticData.ListPoint.RemoveAt(0);
                            }
                            PointPairList list50 = new PointPairList();

                            for (int i = 0; i < 500; i++)//只显示前500个点
                            {
                                frequList[i] = (double)93750 / (StaticData.DSDS + 1) * (double)i / (double)a;//频率序列赋值

                                list50.Add(frequList[i], inin[i]);
                            }
                            StaticData.ListPoint.Add(list50);//将FFT计算结果加入缓存，便于瀑布图显示
                            singleList.Clear();
                            //richTextBox1.AppendText("1"+"\n");
                            timer2_Tick(sender, e);
                        }
                        #endregion

                    }
                    else if ((treeView1.SelectedNode != null) && (treeView1.SelectedNode.Nodes.Count == 0) &&
                        (bool1 || bool2 || bool3) && (ButtonsEnabled))  // 这里要保存震动数据保持连续动画的值
                    {
                        Dictionary<string, List<short>> newMultiList = new Dictionary<string, List<short>>();
                        for (int i = 0; i < nodesc.Count; i++)
                        {
                            if (nodesc[i].Substring(2, 2) != "转速" &&
                                nodesc[i].Substring(2, 2) != "温度" &&
                                nodesc[i].Substring(2, 2) != "温湿")
                            {
                                if (multiList.ContainsKey(nodesc[i]))
                                    newMultiList.Add(nodesc[i], multiList[nodesc[i]]);
                                else
                                    newMultiList.Add(nodesc[i], new List<short>());
                                newMultiList[nodesc[i]].AddRange(data1[nodesc[i]]); // 多项检测时，使用的数组
                                // 记录1s中有多少个点
                                if (nowSTotal != data1[nodesc[i]].Count) nowSTotal = data1[nodesc[i]].Count;
                            }
                        }
                        multiList = newMultiList;
                    }
                    else
                    {
                        multiList.Clear();
                    }

                    if (ButtonsEnabled)
                    {
                        // 这里要保存所有要显示的数据，恩
                        List<string> dataKeys = data0.Keys.ToList<string>();
                        Dictionary<string, List<short>>.Enumerator dataEnumerator = data0.GetEnumerator();
                        while (dataEnumerator.MoveNext())
                        {
                            // 更新一次图像，好吧，防止等待时间太长了额~
                            timer2_Tick(sender, e);

                            if (dataEnumerator.Current.Key.Substring(2, 2) == "转速")
                            {
                                // do nothing
                            }
                            else if (dataEnumerator.Current.Key.Substring(2, 2) == "温度")
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    string lineKey = dataEnumerator.Current.Key + i.ToString();
                                    if (!showLine.ContainsKey(lineKey)) showLine.Add(lineKey, new LinkedList<KTuple>());
                                    showLine[lineKey].AddLast(new KTuple(1, dataEnumerator.Current.Value[i] / 10));
                                    if (showLine[lineKey].Count > 3600)
                                    {
                                        showLine[lineKey].RemoveFirst();
                                    }
                                    KUtil.ListMap(showLine[lineKey], t => new KTuple(t.X - 1, t.Y));
                                }
                            }
                            else if (dataEnumerator.Current.Key.Substring(2, 2) == "温湿")
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    string lineKey = dataEnumerator.Current.Key + i.ToString();
                                    if (!showLine.ContainsKey(lineKey)) showLine.Add(lineKey, new LinkedList<KTuple>());
                                    showLine[lineKey].AddLast(new KTuple(1, dataEnumerator.Current.Value[i]));
                                    if (showLine[lineKey].Count > 3600)
                                    {
                                        showLine[lineKey].RemoveFirst();
                                    }
                                    KUtil.ListMap(showLine[lineKey], t => new KTuple(t.X - 1, t.Y));
                                }
                            }
                            else if (dataEnumerator.Current.Key.Substring(2, 2) == "通道")
                            {
                                try
                                {
                                    string lineKey = dataEnumerator.Current.Key;
                                    if (!showLineState.ContainsKey(lineKey))
                                    {
                                        List<LinkedList<KTuple>> lineState = new List<LinkedList<KTuple>>();
                                        lineState.Add(new LinkedList<KTuple>());
                                        lineState.Add(new LinkedList<KTuple>());
                                        lineState.Add(new LinkedList<KTuple>());
                                        lineState.Add(new LinkedList<KTuple>());
                                        lineState.Add(new LinkedList<KTuple>());
                                        showLineState.Add(lineKey, lineState);
                                    }
                                    double[] bList = Array.ConvertAll<short, double>(dataEnumerator.Current.Value.ToArray(), x => x);//short[]转换成double[]

                                    double avg = Maths.Avg(bList);//均值
                                    double max = Maths.Max(bList);//峰值
                                    double pro = Maths.PRO(bList);//平均幅值
                                    double rms = Maths.RMS(bList);//均方根
                                    double var = Maths.Var(bList, avg);//方差
                                    double kurtosis = Maths.Kurtosis(bList, avg);//峭度Maths.Kurtosis(bList, avg)
                                    double fzzb = max / rms;//峰值指标
                                    double mczb = max / pro;//脉冲指标

                                    showLineState[lineKey][0].AddLast(new KTuple(1, var));
                                    KUtil.ListMap(showLineState[lineKey][0], t => new KTuple(t.X - 1, t.Y));
                                    showLineState[lineKey][1].AddLast(new KTuple(1, rms));
                                    KUtil.ListMap(showLineState[lineKey][1], t => new KTuple(t.X - 1, t.Y));
                                    showLineState[lineKey][2].AddLast(new KTuple(1, kurtosis));
                                    KUtil.ListMap(showLineState[lineKey][2], t => new KTuple(t.X - 1, t.Y));
                                    showLineState[lineKey][3].AddLast(new KTuple(1, fzzb));
                                    KUtil.ListMap(showLineState[lineKey][3], t => new KTuple(t.X - 1, t.Y));
                                    showLineState[lineKey][4].AddLast(new KTuple(1, mczb));
                                    KUtil.ListMap(showLineState[lineKey][4], t => new KTuple(t.X - 1, t.Y));

                                    if (showLineState[lineKey][1].Count > 1800)
                                    {
                                        showLineState[lineKey][0].RemoveFirst();
                                        showLineState[lineKey][1].RemoveFirst();
                                        showLineState[lineKey][2].RemoveFirst();
                                        showLineState[lineKey][3].RemoveFirst();
                                        showLineState[lineKey][4].RemoveFirst();
                                    }
                                    // xianmian shi baojing de caozuo 
                                    int varLimit = -400;
                                    int rmsLimit = -400;
                                    int kurtosisLimit = -400;
                                    int fzzbLimit = -400;
                                    int mczbLimit = -400;
                                    try
                                    {
                                        varLimit = int.Parse(textBox12.Text);
                                        rmsLimit = int.Parse(textBox13.Text);
                                        kurtosisLimit = int.Parse(textBox14.Text);
                                        fzzbLimit = int.Parse(textBox15.Text);
                                        mczbLimit = int.Parse(textBox16.Text);
                                    }
                                    catch
                                    {

                                    }
                                    if (varLimit <= 0) varLimit = 400;
                                    if (rmsLimit <= 0) rmsLimit = 400;
                                    if (kurtosisLimit <= 0) kurtosisLimit = 400;
                                    if (fzzbLimit <= 0) fzzbLimit = 400;
                                    if (mczbLimit <= 0) mczbLimit = 400;
                                    //if (var > varLimit)
                                    //{
                                    //    LimitAlarm(lineKey + "的方差大于" + varLimit + "了");
                                    //}
                                    //if (rms > rmsLimit)
                                    //{
                                    //    LimitAlarm(lineKey + "的均方根大于" + rmsLimit + "了");
                                    //}
                                    if (kurtosis > kurtosisLimit)
                                    {
                                        LimitAlarm(lineKey + "的峭度大于" + kurtosisLimit + "了");
                                    }
                                    if (fzzb > fzzbLimit)
                                    {
                                        LimitAlarm(lineKey + "的峰值指标大于" + fzzbLimit + "了");
                                    }
                                    if (mczb > mczbLimit)
                                    {
                                        LimitAlarm(lineKey + "的脉冲指标大于" + mczbLimit + "了");
                                    }
                                }
                                catch (Exception eee)
                                {
                                    MessageBox.Show(eee.ToString());
                                }
                                // yishang, buxiang yao nage zhushi nage !
                            }
                        }

                        if (bool2)
                            kMultiPanel1.Refresh();
                    }
                }
                #endregion
            }
            catch (OverflowException ex)
            {
                //MessageBox.Show(ex.ToString());
                LimitAlarm(ex.ToString());
                //throw;
            }
        }
        /// <summary>
        /// 单点、多点、多项监测数据
        /// </summary>
        private int lastTick;
        private int showIndex;
        private int lastTick2;
        private int showIndex2;
        private int showTicks2;
        private int controlTick;
        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                #region 单振动点监测数据显示
                if ((ButtonsEnabled) && (bool0))
                {
                    if (singleList1.Count != 0)
                    {

                        int sTotal = nowSTotal;

                        int nowTicks = System.Environment.TickCount;
                        int showTicks = nowTicks - lastTick;    // 距离上一次计时的时间
                        lastTick = nowTicks;

                        int pointNeedToBeShowCount = sTotal * showTicks / 1000; // 这个是要显示的点的个数，从showIndex开始的这么多点

                        int ic = 0; // ic表示加入表中的点个数
                        for (int k = 0; k < pointNeedToBeShowCount; k += showTrackBar)    // ic
                        {
                            int index = (showIndex + k) / showTrackBar * showTrackBar;
                            if (index >= singleList1.Count) continue;
                            if (list10.Count >= sTotal / showTrackBar) list10.RemoveFirst();
                            list10.AddLast(new KTuple((++ic) * showTrackBar, singleList1[index]));
                        }
                        KUtil.ListMap(list10, t => new KTuple(t.X - showTrackBar * ic, t.Y));

                        showIndex += pointNeedToBeShowCount;

                        bool oneSecondPass = showIndex >= sTotal;

                        if (oneSecondPass)
                        {
                            showIndex -= sTotal;
                            singleList1.RemoveRange(0, sTotal);
                        }

                        kChartPanel1.Refresh();

                        //DateTime dt = DateTime.Now;
                        //double x0 = (double)new XDate(dt);
                        //long x1 = (dt.Hour * 60 + dt.Minute) * 60 + dt.Second;
                        //int tt = singleList1.Count / (10 * showTrackBar);
                        //for (int i = 0; i < 10 * tt; i++) //93500总点数/隔点抽取
                        //{

                        //    list10.AddLast(new KTuple(lallalala++, singleList1[showTrackBar * i]));//隔showTrackBar个点取一个
                        //    if (list10.Count >= 10 * tt + 10) //showTrackBar
                        //    {
                        //        list10.RemoveFirst();
                        //        //if (i % trackBar1.Value == 0)
                        //        if ((i + 1) % tt == 0)
                        //        {
                        //            kChartPanel1.Refresh();
                        //        }
                        //    }
                        //}
                        #region 获取最值
                        if (oneSecondPass)
                        {
                            list101 = new LinkedList<KTuple>(list10);
                            // 注意，这里有一个排序
                            //list101.Sort(s);
                            // 按照Y值增序，X增序排列
                            KUtil.SortList(list101, (t1, t2) => t1.Y < t2.Y ? true : (t1.Y == t2.Y && t1.X < t2.X ? true : false));
                            textBox1.Text = String.Format("{0:F}", list101.Last.Value.Y);
                            textBox3.Text = String.Format("{0:F}", list101.First.Value.Y);
                        }
                        #endregion

                        //singleList1.Clear();
                        //singleList1.RemoveRange(0,singleList1.Count-50);//================貌似也不是这====53s================点数不连续
                    }
                    else
                    {
                        lastTick = System.Environment.TickCount;   // 设置计数
                        showIndex = 0;

                        //list10.Clear();
                        //list11.Clear();
                        singleList.Clear();
                        singleList1.Clear();
                        //treeView1.SelectedNode.BackColor = Color.Red;
                        //treeView1.SelectedNode.Parent.BackColor = Color.Red;
                        kChartPanel1.Refresh();
                    }
                }
                else
                {
                    lastTick = System.Environment.TickCount;   // 设置计数
                    showIndex = 0;
                }
                #endregion

                // 多振动点监测数据显示
                if ((ButtonsEnabled) && (bool1 || bool2 || bool3))
                {
                    int nowTicks2 = System.Environment.TickCount;
                    showTicks2 = nowTicks2 - lastTick2;    // 距离上一次计时的时间
                    lastTick2 = nowTicks2;

                    //listBox1.Items.Add("Timer2 " + showTicks2);

                    LineShow();
                }
                else
                {
                    lastTick2 = System.Environment.TickCount;
                    showIndex2 = 0;

                    controlTick = System.Environment.TickCount;
                }

                if (ButtonsEnabled && bool4)
                {
                    RefreshBackground();
                }
                else
                {
                }

            }
            catch (OverflowException ex)
            {
                //MessageBox.Show(ex.ToString());
                LimitAlarm(ex.ToString());
               // throw;
            }
        }
        /// <summary>
        /// 监测切换
        /// </summary>
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                MF = (MainForm)this.Owner;
                if (!MF.ISConnected)
                {
                    return;
                }
                timer1_Tick(sender, e);
                timer2.Stop();
                num1 = 0;

                string tpp2 = tabControl2.SelectedTab.Text;//selectedTab代表tabpage
                #region MyRegion
                #region MyRegion
                if (tpp2 == "单点监测")
                {
                    ShowScrollbartoolStripBt.Enabled = false;
                    toolStripComboBox1.Enabled = true;
                    bool3 = false;
                    bool1 = false;
                    bool2 = false;
                    bool4 = false;
                    bool0 = true;
                    CreatTree();
                    treeView1.SelectedNode = treeView1.Nodes[0].Nodes[0].Nodes[0];
                    ent = treeView1.Nodes[0].Nodes[0].Nodes[0].Text;
                    Paent = treeView1.Nodes[0].Nodes[0].Text;
                    MyMasterPane1(ent, Paent);
                    timer2.Interval = showTimeTnterval;
                    timer2.Start();
                    //ButtonsEnabled = true;
                }
                if (tpp2 == "多点监测")
                {
                    ShowScrollbartoolStripBt.Enabled = true;
                    toolStripComboBox1.Enabled = false;
                    bool0 = false;
                    bool3 = false;
                    bool2 = false;
                    bool4 = false;
                    bool1 = true;
                    CreatTree();
                    treeView1.Nodes[0].Nodes[0].Nodes[0].Checked = true;
                    tabControlChangeTreeNode();
                }
                #endregion

                #region MyRegion
                // Reedit by Kier
                if (tpp2 == "温湿度监测")
                {
                    ShowScrollbartoolStripBt.Enabled = false;
                    toolStripComboBox1.Enabled = false;
                    bool0 = false;
                    bool1 = false;
                    bool4 = false;
                    bool3 = false;
                    bool2 = true;
                    // MyMasterPane3();
                    CreatTree();
                    treeView1.Nodes[0].Nodes[0].Nodes[0].Nodes[0].Checked = true;
                    tabControlChangeTreeNode();

                }
                if (tpp2 == "振动特征")
                {
                    ShowScrollbartoolStripBt.Enabled = false;
                    toolStripComboBox1.Enabled = false;
                    bool0 = false;
                    bool1 = false;
                    bool2 = false;
                    bool4 = false;
                    bool3 = true;
                    // MyMasterPane3();
                    CreatTree();
                    treeView1.SelectedNode = treeView1.Nodes[0].Nodes[0].Nodes[0];
                    ent = treeView1.Nodes[0].Nodes[0].Nodes[0].Text;
                    Paent = treeView1.Nodes[0].Nodes[0].Text;
                    MyMasterPane4();
                    timer2.Interval = showTimeTnterval;
                    timer2.Start();
                    //ButtonsEnabled = true;

                }
                #endregion

                #region MyRegion
                // Redit by Kier
                if (tpp2 == "瀑布图")
                {
                    ShowScrollbartoolStripBt.Enabled = false;
                    toolStripComboBox1.Enabled = false;
                    bool0 = false;
                    bool1 = false;
                    bool2 = false;
                    bool3 = false;
                    bool4 = true;
                    CreatTree();

                    treeView1.SelectedNode = treeView1.Nodes[0].Nodes[0].Nodes[0];
                    ent = treeView1.Nodes[0].Nodes[0].Nodes[0].Text;
                    Paent = treeView1.Nodes[0].Nodes[0].Text;
                    InitialBitmap();
                    Color bckColor = this.PlotPanel.BackColor;
                    gB.Clear(bckColor);
                    cmbcolortype.SelectedIndex = 2;//SelectedChange事件去掉了刷新
                } 
                #endregion
                #endregion
            }
            catch (OverflowException ex)
            {
                MessageBox.Show(ex.ToString());
               // throw;
            }
        }
        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlineMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                #region 保存报警数据
                string APPpath = @"e:\Program Files\OnlineMonitor\";
                if (Directory.Exists(APPpath) == false) //判断目录是否存在
                    //创建目录
                    Directory.CreateDirectory(APPpath);
                FileStream ke;
                if (!File.Exists(@"e:\Program Files\OnlineMonitor\LimitAlarm.bin"))
                {
                    ke = File.Create(@"e:\Program Files\OnlineMonitor\LimitAlarm.bin");
                    ke.Close();
                }
                ke = new FileStream(@"e:\Program Files\OnlineMonitor\LimitAlarm.bin", FileMode.Append, FileAccess.Write);
                StreamWriter bw = new StreamWriter(ke);
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    bw.WriteLine(listBox1.Items[i].ToString());
                }
                bw.Close();
                ke.Close();

                #endregion
                if (threadISconnect != null && threadISconnect.IsAlive)
                {
                    threadISconnect.Abort();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
               // throw;
            }
        }
        /// <summary>
        /// 去掉显示面板的右键菜单中的某个项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="menuStrip"></param>
        /// <param name="mousePt"></param>
        /// <param name="objState"></param>
        private void zedControl1_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                if ((string)item.Tag == "set_default")// “恢复默认大小”菜单项
                {
                    menuStrip.Items.Remove(item);//移除菜单项

                    item.Visible = false; //不显示

                    break;
                }
            }

        }
        /// <summary>
        /// 特征参数轨迹监测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked)
                {
                    groupBox3.Enabled = true;
                    MyMasterPane4();
                }
                if (!checkBox1.Checked)
                {
                    groupBox3.Enabled = false;
                    MyMasterPane4();
                }
            }
            catch (OverflowException ex)
            {
                MessageBox.Show(ex.ToString());
               // throw;
            }
        }
        /// <summary>
        /// 轨迹图特征参数选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RestoreScaletoolStripBt_Click(sender, e);
        }
        /// <summary>
        /// splitContainerEx收缩事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitContainerEx1_CollapseClick(object sender, EventArgs e)
        {
            //zedControl2.Size = new Size(panel1.Width - 20, (nodesc.Count) * (panel1.Height) / 2);
        }
        #endregion

        #region 工具栏按钮事件
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavetoolStripBt_Click(object sender, EventArgs e)
        {
            string tpp2 = tabControl2.SelectedTab.Text;//selectedTab代表tabpage
            if (tpp2 == "单点监测")
            {
                // 注意，这里有一个另存为
                //zedControl1.SaveAs();
            }
            if (tpp2 == "多点监测")
            {
                //zedControl2.SaveAs();
            }
            if (tpp2 == "温湿度监测")
            {
                //zedControl3.SaveAs();
            }
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrinttoolStripBt_Click(object sender, EventArgs e)
        {
            string tpp2 = tabControl2.SelectedTab.Text;//selectedTab代表tabpage
            if (tpp2 == "单点监测")
            {
                // 注意，这里有一个保存操作
                //zedControl1.DoPrint();
            }
            if (tpp2 == "多点监测")
            {
                //zedControl2.DoPrint();
            }
            if (tpp2 == "温湿度监测")
            {
                //zedControl3.DoPrint();
            }
        }
        /// <summary>
        /// 打印设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageSetuptoolStripBt_Click(object sender, EventArgs e)
        {
            string tpp2 = tabControl2.SelectedTab.Text;//selectedTab代表tabpage
            if (tpp2 == "单点监测")
            {
                // 注意，这里有一个setup……干嘛的……
                //zedControl1.DoPageSetup();
            }
            if (tpp2 == "多点监测")
            {
                //zedControl2.DoPageSetup();
            }
            if (tpp2 == "温湿度监测")
            {
                //zedControl3.DoPageSetup();
            }
        }
        /// <summary>
        /// 打印预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintPreviewtoolStripBt_Click(object sender, EventArgs e)
        {
            string tpp2 = tabControl2.SelectedTab.Text;//selectedTab代表tabpage
            if (tpp2 == "单点监测")
            {
                // 注意，这里有一个打印预览
                //zedControl1.DoPrintPreview();
            }
            if (tpp2 == "多点监测")
            {
                //zedControl2.DoPrintPreview();
            }
            if (tpp2 == "温湿度监测")
            {
                //zedControl3.DoPrintPreview();
            }
        }
        /// <summary>
        /// 曲线显示值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowValuetoolStripBt_Click(object sender, EventArgs e)
        {
            string tpp2 = tabControl2.SelectedTab.Text;//selectedTab代表tabpage
            ToolStripButton tb = sender as ToolStripButton;
            if (tpp2 == "单点监测")
            {
                // 注意，这里有一个显示值，干嘛的？
                //zedControl1.ShowValue(tb.Checked);
            }
            if (tpp2 == "多点监测")
            {
                //zedControl2.ShowValue(tb.Checked);
            }
            if (tpp2 == "温湿度监测")
            {
                //zedControl3.ShowValue(tb.Checked);
            }
        }
        /// <summary>
        /// 撤销滚轮操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomouttoolStripBt_Click(object sender, EventArgs e)
        {
            string tpp2 = tabControl2.SelectedTab.Text;//selectedTab代表tabpage
            if (tpp2 == "单点监测")
            {
                // 注意，
                //zedControl1.Zoomout();
            }
            if (tpp2 == "多点监测")
            {
                //zedControl2.Zoomout();
            }
            if (tpp2 == "温湿度监测")
            {
                //zedControl3.Zoomout();
            }
        }
        /// <summary>
        /// 撤销所有滚轮操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomoutAlltoolStripBt_Click(object sender, EventArgs e)
        {
            string tpp2 = tabControl2.SelectedTab.Text;//selectedTab代表tabpage
            if (tpp2 == "单点监测")
            {
                // 注意，
                //zedControl1.ZoomoutAll();
            }
            if (tpp2 == "多点监测")
            {
                //zedControl2.ZoomoutAll();
            }
            if (tpp2 == "温湿度监测")
            {
                //zedControl3.ZoomoutAll();
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 接收数据toolStripBt_Click(object sender, EventArgs e)
        {
            接收ToolStripMenuItem_Click(sender, e);
        }
        /// <summary>
        /// 停止接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 停止接收toolStripBt_Click(object sender, EventArgs e)
        {
            停止接收ToolStripMenuItem_Click(sender, e);
        }
        /// <summary>
        /// 清屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cleartoolStripBt_Click(object sender, EventArgs e)
        {
            清除ToolStripMenuItem_Click(sender, e);
        }
        /// <summary>
        /// 显示数据点数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            list10.Clear();
            showTrackBar = int.Parse(toolStripComboBox1.SelectedItem.ToString().Trim());
        }
        /// <summary>
        /// 刷新坐标轴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestoreScaletoolStripBt_Click(object sender, EventArgs e)
        {
            try
            {
                if (bool0)
                {
                    // 注意，恢复尺度，这里不用
                    //zedControl1.RestoreScales();
                    if (ent != null && Paent != null)
                    {
                        MyMasterPane1(ent, Paent);
                    }
                    else
                    {
                        MyMasterPane1("", "");
                    }
                }
                if (bool1)
                {
                    //zedControl2.RestoreScales();
                    MyMasterPane2();
                }
                if (bool2)
                {
                    //zedControl3.RestoreScales();
                    MyMasterPane3();
                }
                if (bool3)
                {
                    //zedControl4.RestoreScales();

                    MyMasterPane4();
                }
            }
            catch (OverflowException ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
        }
        /// <summary>
        /// 多点显示滚动条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowScrollbartoolStripBt_Click(object sender, EventArgs e)
        {
            ToolStripButton tb = sender as ToolStripButton;
            if (tb.Checked)
            {
                //zedControl2.Dock = DockStyle.None;
                panel1.HorizontalScroll.Enabled = false;
            }
            else
            {
                //zedControl2.Dock = DockStyle.Fill;
                panel1.HorizontalScroll.Enabled = false;
            }
        }

        #endregion

        #region 菜单栏事件
        /// <summary>
        /// 接收数据
        /// </summary>
        private void 接收ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region MyRegion

            timer1.Interval = timeInterval;//无论单点还是多点，都开启time1
            timer1.Start();
            timer1_Tick(sender, e);

            if (!bool4)
            {
                timer2.Start(); 
            }
            //System.Threading.Thread.Sleep(1000);
            ButtonsEnabled = true;
            #endregion
        }
        /// <summary>
        /// 停止接收数据
        /// </summary>
        private void 停止接收ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ButtonsEnabled = false;
            timer1.Stop();
        }
        /// <summary>
        /// 清除曲线
        /// </summary>
        private void 清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            list10.Clear();
            list11.Clear();
            if (pairLists.Count!=0)
            {
                for (int i = 0; i < pairLists.Count; i++)
                {
                    pairLists[i].Clear();
                } 
            }

            kChartPanel1.Refresh();
            kChartPanel2.Refresh();
            //zedControl3.AxisChange();
            kMultiPanel1.Refresh();
        }
        /// <summary>
        /// 退出程序
        /// </summary>
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void 显示报警信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!显示报警信息ToolStripMenuItem.Checked)
            //{
            //    显示报警信息ToolStripMenuItem.CheckState = CheckState.Checked;
            //    splitContainerEx1.Panel2Collapsed = false;
            //}
            //else
            //{
            //    显示报警信息ToolStripMenuItem.CheckState = CheckState.Unchecked;
            //    splitContainerEx1.Panel2Collapsed = true;
            //}
            LoadHistory lh = new LoadHistory(false);
            lh.ShowDialog();
        }
        private void 图像保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavetoolStripBt_Click(sender, e);
        }
        private void PrinttoolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrinttoolStripBt_Click(sender, e);
        }
        private void PageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PageSetuptoolStripBt_Click(sender, e);
        }
        private void PrintPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintPreviewtoolStripBt_Click(sender, e);
        }
        #endregion

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            list10.Clear();
        }
        /// <summary>
        /// 字体设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // fontDialog1.ShowDialog();//弹出字体选择对话框
            //zedControl1.Font = fontDialog1.Font;
          //  MasterPan1.Fill = new Fill(fontDialog1.Color);
          //  zedControl1.Refresh();
        }
        private void BackColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 注意，这里选择字体当然不管用啦啦~
            colorDialog1.ShowDialog();//弹出字体选择对话框
            kChartPanel1.Refresh();
            ToolStripMenuItem ts = sender as ToolStripMenuItem;
           
        }

        #region 3D
        private void Initial3D() 
        {

            #region 3D初始化
            #region 双缓存技术
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this.SetStyle(ControlStyles.UserPaint, true);//When this flag is set to true,
            //the control paints itself and is not painted by the system operation

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.SetStyle(ControlStyles.DoubleBuffer, true);//when set Control.DoubleBuffered to true, 
            //it will set the ControlStyles.AllPaintingInWmPaint and ControlStyles.OptimizedDoubleBuffer to true           


            #endregion
            cs = new ChartStyle(this);
            cs2d = new ChartStyle2D(this);
            ds = new DataSeries();
            dc = new DrawChart(this);
            cf = new ChartFunctions();
            cm = new ColorMap();

            cs.GridStyle.LineColor = Color.LightGray;
            cs.GridStyle.Pattern = DashStyle.Dash;
            //cs.Title = "No Title";
            cs.Title = Paent + ent + "时间谱图";
            cs.XLabel = "频率/Hz";
            cs.YLabel = "时间/mm:ss";
            cs.ZLabel = "幅值";
            cs.Elevation = elevation;
            cs.Azimuth = azimuth;

            cs2d.ChartBackColor = Color.White;
            cs2d.ChartBorderColor = Color.Black;


            //ds.LineStyle.IsVisible = false;//曲面上是否显示实线
            ds.LineStyle.IsVisible = true;//同时控制着三维折线能否显示

            //dc.ChartType = DrawChart.ChartTypeEnum.Mesh;
            //dc.IsHiddenLine = true;//是否填充mesh，否
            //dc.IsHiddenLine = false;
            //dc.ChartType = DrawChart.ChartTypeEnum.MeshZ;
            dc.ChartType = DrawChart.ChartTypeEnum.Waterfall;
            //dc.ChartType = DrawChart.ChartTypeEnum.Surface;
            //dc.IsInterp = true;//Surface是由大块组成还是细密小块组成
            //dc.NumberInterp = 6;//Surface控制块的大小
            //dc.ChartType = DrawChart.ChartTypeEnum.Line;

            cs.IsColorBar = true;//颜色条
            //dc.IsColorMap = false;//与IsColorBar同时控制颜色条
            //dc.IsColorMap = true;

            //dc.CMap = cm.Rainbow(Th);// colortype之一Rainbow颜色控制

            #endregion
        }
        /// <summary>
        /// 3D取数据，画图
        /// </summary>
        /// <param name="colortype"></param>
        private void DrawPlot(int[,] colortype)
        {
            gB.SmoothingMode = SmoothingMode.AntiAlias;
            dc.CMap = colortype;
            cf.Show3DData(ds, cs);
            cs.AddChartStyle(gB); //画坐标轴，刻度
            if (ButtonsEnabled&&bool4)
            {
                dc.AddChart(gB, ds, cs, cs2d); //显示数据 
            }
        }
        /// <summary>
        /// 3D在缓存中定义一个与控件空间相同的位图，用于复制到控件中来显示
        /// </summary>
        private void InitialBitmap()
        {
            //Define bitmap
            Size size = new Size(PlotPanel.Width, PlotPanel.Height);
            bitmap = new Bitmap(size.Width, size.Height);
            gB = Graphics.FromImage(bitmap);
        }
        /// <summary>
        /// 3D将缓存中的位图拷到控件的背景上，以完成图形的显示
        /// </summary>
        private void RefreshBackground()
        {
            InitialBitmap();
            gB.Clear(this.PlotPanel.BackColor);
            DrawPlot(colortype);
          
            Size sz = this.PlotPanel.Size;
            Rectangle rt = new Rectangle(0, 0, sz.Width, sz.Height);
            bm = bitmap.Clone(rt, bitmap.PixelFormat);
            PlotPanel.BackgroundImage = bm;
        }
        /// <summary>
        /// 3D颜色选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbcolortype_SelectedIndexChanged(object sender, EventArgs e)
        {
            string color = cmbcolortype.Text;

            switch (color)
            {
                case "Autumn":
                    colortype = cm.Autumn();
                    break;
                case "Cool":
                    colortype = cm.Cool();
                    break;
                case "Gray":
                    colortype = cm.Gray();
                    break;
                case "Hot":
                    colortype = cm.Hot();
                    break;
                case "Jet":
                    colortype = cm.Jet();
                    break;
                case "Spring":
                    colortype = cm.Spring();
                    break;
                case "Summer":
                    colortype = cm.Summer();
                    break;
                case "Winter":
                    colortype = cm.Winter();
                    break;
                case "Mix1":
                    colortype = cm.Mix1();
                    break;
                case "Mix2":
                    colortype = cm.Mix2();
                    break;
                case "Mix3":
                    colortype = cm.Mix3();
                    break;
                case "Mix4":
                    colortype = cm.Mix4();
                    break;
                case "Rainbow":
                    colortype = cm.Rainbow(Th);
                    break;
                case "FallGrYl":
                    colortype = cm.FallGrYl();
                    break;
                case "FallRdGr":
                    colortype = cm.FallRdGr();
                    break;
                case "Cool1":
                    colortype = cm.Cool1(255);
                    break;
                case "Cool11":
                    colortype = cm.Cool11(255);
                    break;
                case "Cool2":
                    colortype = cm.Cool2(255);
                    break;
                case "Cool22":
                    colortype = cm.Cool22(255);
                    break;
                case "DeltaRdGr":
                    colortype = cm.DeltaRdGr(cmax, cmin);
                    break;
                case "DeltaRdBl":
                    colortype = cm.DeltaRdBl(cmax, cmin);
                    break;
                case "DeltaGrBl":
                    colortype = cm.DeltaGrBl(cmax, cmin);
                    break;
                case "DeltaGrRd":
                    colortype = cm.DeltaGrRd(cmax, cmin);
                    break;
                case "Hot1":
                    colortype = cm.Hot1();
                    break;
                case "Hot2":
                    colortype = cm.Hot2();
                    break;
                default:
                    break;
            }

            //RefreshBackground();

        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            InitialBitmap();
            RefreshBackground();
        }
        /// <summary>
        /// 方位角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownBeta_ValueChanged(object sender, EventArgs e)
        {
            azimuth = (float)numericUpDownBeta.Value;
            cs.Azimuth = azimuth;
            RefreshBackground();
        }
        /// <summary>
        /// 视角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownAlpha_ValueChanged(object sender, EventArgs e)
        {
            elevation = (float)numericUpDownAlpha.Value;
            cs.Elevation = elevation;
            RefreshBackground();
        }
        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnrotate_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            int i;
            for (i = -180; i <= 180; i += 2)
            {
                Thread.Sleep(50);
                azimuth = (float)i;
                numericUpDownBeta.Value = i;
                numericUpDownBeta.Refresh();

                PlotPanel.Refresh();
            }

            Cursor = Cursors.Default;
        }
        
        #endregion

        private void textBox12_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox12_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (textBox12.Text.Length == 0)
                textBox12.Text = "400";
        }

        private void toolStripComboBox2_TextChanged(object sender, EventArgs e)
        {
            int plain = plainDefault;
            try { plain = int.Parse(toolStripComboBox2.Text); }
            catch { }
            if (bool0)
            {
                kChartPanel1.Panel[0].YAxisMax = plain;
                kChartPanel1.Panel[0].YAxisMin = -plain;
            }
            if (bool1)
            {
                for (int i = 0; i < nodesc.Count; i++)
                {
                    if (nodesc[i].Substring(2, 2) == "通道")
                    {
                        kChartPanel2.Panel[i].YAxisMax = plain;
                        kChartPanel2.Panel[i].YAxisMin = -plain;
                    }
                }
            }
        }
    }
}
