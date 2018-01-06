using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DSDLL;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
//引用内存映射文件命名空间  
using System.IO.MemoryMappedFiles;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //const int INVALID_HANDLE_VALUE = -1;
        //const int PAGE_READWRITE = 0x04;

        //[DllImport("User32.dll")]
        //private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        //[DllImport("User32.dll")]
        //private static extern bool SetForegroundWindow(IntPtr hWnd);

        ////共享内存
        //[DllImport("Kernel32.dll", EntryPoint = "CreateFileMapping")]
        //private static extern IntPtr CreateFileMapping(IntPtr hFile, //HANDLE hFile,
        // UInt32 lpAttributes,//LPSECURITY_ATTRIBUTES lpAttributes,  //0
        // UInt32 flProtect,//DWORD flProtect
        // UInt32 dwMaximumSizeHigh,//DWORD dwMaximumSizeHigh,
        // UInt32 dwMaximumSizeLow,//DWORD dwMaximumSizeLow,
        // string lpName//LPCTSTR lpName
        // );

        //[DllImport("Kernel32.dll", EntryPoint = "OpenFileMapping")]
        //private static extern IntPtr OpenFileMapping(
        // UInt32 dwDesiredAccess,//DWORD dwDesiredAccess,
        // int bInheritHandle,//BOOL bInheritHandle,
        // string lpName//LPCTSTR lpName
        // );

        //const int FILE_MAP_ALL_ACCESS = 0x0002;
        //const int FILE_MAP_WRITE = 0x0002;

        //[DllImport("Kernel32.dll", EntryPoint = "MapViewOfFile")]
        //private static extern IntPtr MapViewOfFile(
        // IntPtr hFileMappingObject,//HANDLE hFileMappingObject,
        // UInt32 dwDesiredAccess,//DWORD dwDesiredAccess
        // UInt32 dwFileOffsetHight,//DWORD dwFileOffsetHigh,
        // UInt32 dwFileOffsetLow,//DWORD dwFileOffsetLow,
        // UInt32 dwNumberOfBytesToMap//SIZE_T dwNumberOfBytesToMap
        // );

        //[DllImport("Kernel32.dll", EntryPoint = "UnmapViewOfFile")]
        //private static extern int UnmapViewOfFile(IntPtr lpBaseAddress);

        //[DllImport("Kernel32.dll", EntryPoint = "CloseHandle")]
        //private static extern int CloseHandle(IntPtr hObject);

        //private Semaphore m_Write;  //可写的信号
        //private Semaphore m_Read;  //可读的信号
        //private IntPtr handle;     //文件句柄
        //private IntPtr addr;       //共享内存地址
        //uint mapLength;            //共享内存长

        ////线程用来读取数据       
        //Thread threadRed;
        public Form1()
        {
            InitializeComponent();
            //init();
        }
        ///<summary>
        /// 初始化共享内存数据 创建一个共享内存
        ///</summary>
        private void init()
        {
            //m_Write = new Semaphore(1, 1, "WriteMap");//开始的时候有一个可以写
            //m_Read = new Semaphore(0, 1, "ReadMap");//没有数据可读
            //mapLength = 32768;
            //IntPtr hFile = new IntPtr(INVALID_HANDLE_VALUE);
            //handle = CreateFileMapping(hFile, 0, PAGE_READWRITE, 0, mapLength, "shareMemory");
            //addr = MapViewOfFile(handle, FILE_MAP_ALL_ACCESS, 0, 0, 0);

            //threadRed = new Thread(new ThreadStart(ThreadReceive));
            //threadRed.IsBackground = true;
            //threadRed.Start();
        }
        Class1 cs;
        private void button1_Click(object sender, EventArgs e)
        {
            cs = new Class1();
            cs.savePath = Application.StartupPath;
            if (cs.Connect(textBox1.Text))
            {
                label1.Text = "连接成功....";

            };
            comboBox1.SelectedIndex = 2;
            //Thread td2 = new Thread(Crosscontrol);//读取控件
            //td2.IsBackground = true; //设定为后台线程 
            //td2.Start();//启动线程 
            Thread td21 = new Thread(Crosscontrol1);//读取控件
            td21.IsBackground = true; //设定为后台线程 
            td21.Start();//启动线程 

            button2_Click(sender, e);
            button3_Click( sender,  e);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (cs.WriteOrder())
            {
                label1.Text = "命令发送成功....";
            };

        }

        #region 选择通道
        void Crosscontrol()
        {
            while (true)
            {
                control();
                Thread.Sleep(1000);
            }
        }
        void Crosscontrol1()
        {
            while (true)
            {
                control1();
                Thread.Sleep(1000);
            }
        }
        int channel = 1;
        int selecomindex = 1;
        private delegate void DelegateFunction();//代理
        void control()
        {
            try
            {
                //if (this.trackBar1.InvokeRequired)//等待异步
                //{
                //    DelegateFunction df = new DelegateFunction(control);
                //    this.Invoke(df);//invoke送主线程
                //}
                //else
                //{
                //    channel = trackBar1.Value;
                //}
            }
            catch (Exception)
            {
            }
        }
        void control1()
        {
            try
            {
                if (this.comboBox1.InvokeRequired)//等待异步
                {
                    DelegateFunction df = new DelegateFunction(control1);
                    this.Invoke(df);//invoke送主线程
                }
                else
                {
                    channel = comboBox1.SelectedIndex+1;
                    //switch (comboBox1.SelectedIndex)
                    //{
                    //    case 0: 
                    //        {
                    //            selecomindex = 1;
                    //        }
                    //        break;
                    //    case 1:
                    //        {
                    //            selecomindex = 20;
                    //        }
                    //        break;
                    //    case 2:
                    //        {
                    //            selecomindex = 50;
                    //        }
                    //        break;
                    //    case 3:
                    //        {
                    //            selecomindex = 100;
                    //        }
                    //        break;
                    //}
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            //try
            //{
            Thread threadRed = new Thread(new ThreadStart(readDataThread1));
            threadRed.IsBackground = true;
            threadRed.Start();
            //}
            //catch (Exception)
            //{ 
            //    MessageBox.Show("aaa ");
            //}

        }
        /// <summary>
        /// 线程启动从共享内存中获取数据信息 
        /// </summary>
        private void ThreadReceive()
        {
            Graphics g = pictureBox1.CreateGraphics();
            Pen pn1 = new Pen(Color.White);
            Pen pn2 = new Pen(Color.RoyalBlue);
            List<Point> points1a = new List<Point>();//存直线连接的点
            List<Point> points1b = new List<Point>();//存直线连接的点
            points1a.Add(new Point(1, 1)); points1a.Add(new Point(1, 1));//初始化存直线连接的点
            points1b.Add(new Point(1, 1)); points1b.Add(new Point(1, 1));//初始化存直线连接的点
            int x = 0;

            //myDelegate myI = new myDelegate(changeTxt);
            while (true)
            {
                try
                {

                    ////读取共享内存中的数据：
                    ////是否有数据写过来
                    //m_Read.WaitOne();
                    ////IntPtr m_Sender = MapViewOfFile(handle, FILE_MAP_ALL_ACCESS, 0, 0, 0);
                    //byte[] byteStr = new byte[374418];
                    //int[] klda = new int[4 * 500 * 93];
                    //byteCopy(byteStr, addr);
                    //int ksakl = 93;
                    //for (int i = 0; i < ksakl; i++)
                    //{
                    //    for (int j = 0; j < 2000; j++)
                    //    {
                    //        klda[i * 2000 + j] = Convert.ToInt16((Convert.ToChar(byteStr[i * 4026 + j * 2 + 5]) << 8)
                    //            | (Convert.ToChar(byteStr[i * 4026 + j * 2 + 6])));
                    //    }
                    //}
                    //for (int j = 0; j < 93; j++)
                    //{
                    //    for (int i = 0; i < 500; i++)
                    //    {
                    //        x++;
                    //        if (x < 800)
                    //        {
                    //            points1a.Add(new Point(x, 150 + klda[i + 2000 * j + 500 * (channel - 1)]));
                    //        }
                    //        else if (x == 800)
                    //        {
                    //            g.DrawLines(pn1, points1b.ToArray());//points1b填白清空
                    //            points1b.Clear();
                    //            g.DrawLines(pn2, points1a.ToArray());//points1a画黑线
                    //        }
                    //        else if (x < 1600)
                    //        {
                    //            points1b.Add(new Point(x - 800, 150 + klda[i + 2000 * j + 500 * (channel - 1)]));
                    //        }
                    //        if (x == 1600)
                    //        {
                    //            g.DrawLines(pn1, points1a.ToArray());//points1a填白清空
                    //            points1a.Clear();
                    //            g.DrawLines(pn2, points1b.ToArray());//points1b画黑线
                    //            x = 0;
                    //        }
                    //    }
                    //}
                    ////string str = Encoding.Default.GetString(byteStr, 0, byteStr.Length);
                    ////this.BeginInvoke((Action)delegate
                    ////{
                    ////    listBox1.Items.Add(str);
                    ////    if (listBox1.Items.Count > 20)
                    ////    {
                    ////        listBox1.Items.RemoveAt(0);
                    ////    }
                    ////});

                    ///////调用数据处理方法 处理读取到的数据
                    //m_Write.Release();
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    continue;
                    //Thread.Sleep(0);
                }

            }

        }
        //不安全的代码在项目生成的选项中选中允许不安全代码
        static unsafe void byteCopy(byte[] dst, IntPtr src)
        {
            fixed (byte* pDst = dst)
            {
                byte* pdst = pDst;
                byte* psrc = (byte*)src;
                while ((*pdst++ = *psrc++) != '\0')
                    ;
            }

        }
        private void readDataThread1()
        {
            try
            {
                long capacity = 1 << 10 << 10;

                using (var mmf = MemoryMappedFile.OpenExisting("testMmf"))
                {
                    MemoryMappedViewAccessor viewAccessor = mmf.CreateViewAccessor(0, capacity);
                    Graphics g = pictureBox1.CreateGraphics();
                    Pen pn1 = new Pen(Color.White);
                    Pen pn2 = new Pen(Color.RoyalBlue);
                    List<Point> points1a = new List<Point>();//存直线连接的点
                    List<Point> points1b = new List<Point>();//存直线连接的点
                    List<int> pointc = new List<int>();
                    points1a.Add(new Point(1, 1)); points1a.Add(new Point(1, 1));//初始化存直线连接的点
                    points1b.Add(new Point(1, 1)); points1b.Add(new Point(1, 1));//初始化存直线连接的点
                    int x = 0;
                    int sa = 0;
                    while (true)
                    {
                        #region MyRegion
                        //读取字符长度  
                        int strLength = viewAccessor.ReadInt32(0);
                        //if(strLength==0)
                        //{
                        //    MessageBox.Show("cuole");
                           
                        //}
                        int[] charsInMMf = new int[strLength];
                        //读取字符  
                        viewAccessor.ReadArray<int>(4, charsInMMf, 0, strLength);
                        if (strLength == 0)
                        {
                            MessageBox.Show("内存数据为空");
                            continue;
                        }
                        for (int i = 0; i < 500; i++)
                        {
                            sa++;
                            if (sa % selecomindex == 0)
                            {


                                if (points1a.Count > 2)
                                {
                                    g.DrawLines(pn1, points1b.ToArray());//points1b填白清空
                                    points1a.Clear();
                                    points1b.Clear();
                                }
                                if (pointc.Count >= 800)
                                {
                                    pointc.RemoveAt(0);
                                }
                                pointc.Add(150 + charsInMMf[i + 500 * (channel - 1)]);
                                for (int jk = 0; jk < pointc.Count; jk++)
                                {
                                    points1a.Add(new Point(jk, pointc[jk]));
                                    points1b.Add(new Point(jk, pointc[jk]));
                                }
                                g.DrawLines(pn2, points1a.ToArray());//points1a画黑线

                            }

                            //if (x < 800)
                            //{
                            //    points1a.Add(new Point(x, 150 + charsInMMf[i  + 500 * (channel - 1)]));
                            //}
                            //else if (x == 800)
                            //{
                            //    g.DrawLines(pn1, points1b.ToArray());//points1b填白清空
                            //    points1b.Clear();
                            //    g.DrawLines(pn2, points1a.ToArray());//points1a画黑线
                            //}
                            //else if (x < 1600)
                            //{
                            //    points1b.Add(new Point(x - 800, 150 + charsInMMf[i +  500 * (channel - 1)]));
                            //}
                            //if (x == 1600)
                            //{
                            //    g.DrawLines(pn1, points1a.ToArray());//points1a填白清空
                            //    points1a.Clear();
                            //    g.DrawLines(pn2, points1b.ToArray());//points1b画黑线
                            //    x = 0;
                            //}
                            //}
                            if (sa > 1000)
                            {
                                sa = 0;
                            }
                        }

                        #endregion

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void readDataThread()
        {
            try
            {

                Graphics g = pictureBox1.CreateGraphics();
                Pen pn1 = new Pen(Color.White);
                Pen pn2 = new Pen(Color.RoyalBlue);
                List<Point> points1a = new List<Point>();//存直线连接的点
                List<Point> points1b = new List<Point>();//存直线连接的点
                points1a.Add(new Point(1, 1)); points1a.Add(new Point(1, 1));//初始化存直线连接的点
                points1b.Add(new Point(1, 1)); points1b.Add(new Point(1, 1));//初始化存直线连接的点
                int x = 0;
                int sa = 0;
                string lkaskd = "";
                while (true)
                {
                    int[] lklks = cs.ReadData();
                    //lkaskd = cs.ReadData1();
                    if (lklks[0] == 1111)
                    {
                        //label2.Text = "11";
                        continue;
                    }

                    //label2.Text = "22";
                    for (int j = 0; j < 93; j++)
                    {
                        for (int i = 0; i < 500; i++)
                        {
                            sa++;
                            if (sa % 10 == 0)
                            {
                                x++;
                                if (x < 800)
                                {
                                    points1a.Add(new Point(x, 150 + lklks[i + 2000 * j + 500 * (channel - 1)]));
                                }
                                else if (x == 800)
                                {
                                    g.DrawLines(pn1, points1b.ToArray());//points1b填白清空
                                    points1b.Clear();
                                    g.DrawLines(pn2, points1a.ToArray());//points1a画黑线
                                }
                                else if (x < 1600)
                                {
                                    points1b.Add(new Point(x - 800, 150 + lklks[i + 2000 * j + 500 * (channel - 1)]));
                                }
                                if (x == 1600)
                                {
                                    g.DrawLines(pn1, points1a.ToArray());//points1a填白清空
                                    points1a.Clear();
                                    g.DrawLines(pn2, points1b.ToArray());//points1b画黑线
                                    x = 0;
                                }
                            }
                            if (sa > 1000)
                            {
                                sa = 0;
                            }
                        }
                        //if (sa>=2)
                        //{
                        //    break;
                        //    //break; 
                        //}
                    }
                    //break;

                }
                MessageBox.Show(lkaskd);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
