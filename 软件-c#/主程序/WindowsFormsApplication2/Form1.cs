using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        [DllImport("PlxApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RETURN_CODE PlxSdkVersion(ref byte versionmajor, ref byte versionminor, ref byte versionrevision);

        [DllImport("PlxApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RETURN_CODE PlxPciDeviceOpen(ref DEVICE_LOCATION device_location, ref IntPtr handle);

        [DllImport("PlxApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RETURN_CODE PlxBusIopWrite(IntPtr handle, IOP_SPACE IopSpace, UInt32 address, Boolean bRemap,
                                                                ref byte Buffer, UInt32 ByteCount, ACCESS_TYPE AccessType);

        [DllImport("PlxApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RETURN_CODE PlxDmaSglChannelOpen(IntPtr handle, DMA_CHANNEL dmachannel, ref DMA_CHANNEL_DESC dma_channel_desc);

        [DllImport("PlxApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RETURN_CODE PlxIntrStatusGet(IntPtr handle, ref PLX_INTR plx_intr);

        [DllImport("PlxApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RETURN_CODE PlxIntrAttach(IntPtr handle, PLX_INTR plx_intr, ref IntPtr eventhandle);

        [DllImport("PlxApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RETURN_CODE PlxIntrEnable(IntPtr handle, ref PLX_INTR plx_intr);

        [DllImport("PlxApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RETURN_CODE PlxIntrWait(IntPtr handle, IntPtr hEvent, UInt32 timeout);

        [DllImport("PlxApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RETURN_CODE PlxDmaSglTransfer(IntPtr handle, DMA_CHANNEL dmachannel, ref DMA_TRANSFER_ELEMENT dma_transfer_element,
                                                           Boolean returnImmediate);

        public Form1()
        {
            InitializeComponent();
        }


        private Thread td1 = null;
        private Thread td2 = null;
        private void button1_Click(object sender, EventArgs e)//打开pci设备
        {
            pci_open();
            channel_open();


            td1 = new Thread(draw);//作图
            td1.IsBackground = true; //设定为后台线程 
            td1.Start();//启动线程 

            td2 = new Thread(Crosscontrol);//读取控件
            td2.IsBackground = true; //设定为后台线程 
            td2.Start();//启动线程 
        }

        RETURN_CODE rc = 0;
        IntPtr hDevice = new IntPtr();
        DEVICE_LOCATION device_location;
        private void pci_open()
        {
            byte a = 0, b = 0, c = 0;
            device_location.SerialNumber = new byte[12];
            device_location.BusNumber = 0xff;
            device_location.SlotNumber = 0xff;
            device_location.VendorId = 0xffff;
            device_location.DeviceId = 0xffff;

            rc = PlxSdkVersion(ref a, ref b, ref c);

            rc = PlxPciDeviceOpen(ref device_location, ref hDevice);
        }

        DMA_CHANNEL_DESC DmaDesc;
        PLX_INTR PlxInterrupt;
        IntPtr hInterruptEvent;
        DMA_TRANSFER_ELEMENT DmaData;
        byte ReceiveFlag;
        public byte[] buffer = new byte[500000];
        private void channel_open()//打开dma通道
        {
            DmaDesc.EnableReadyInput = 1;
            DmaDesc.EnableBTERMInput = 0;
            DmaDesc.EnableIopBurst = 1;
            DmaDesc.EnableWriteInvalidMode = 0;
            DmaDesc.EnableDmaEOTPin = 0;
            DmaDesc.DmaStopTransferMode = 0;
            DmaDesc.HoldIopAddrConst = 1;
            DmaDesc.DemandMode = 0;
            DmaDesc.EnableTransferCountClear = 0;
            DmaDesc.WaitStates = 0;
            DmaDesc.IopBusWidth = 2;   // 32-bit
            DmaDesc.DmaChannelPriority = DMA_CHANNEL_PRIORITY.Rotational;

            rc = PlxDmaSglChannelOpen(hDevice, DMA_CHANNEL.PrimaryPciChannel0, ref DmaDesc);

            rc = PlxIntrStatusGet(hDevice, ref PlxInterrupt);
            PlxInterrupt.PciDmaChannel0 = 1;
            PlxInterrupt.PciMainInt = 1;
            PlxInterrupt.IopToPciInt = 1;

            rc = PlxIntrAttach(hDevice, PlxInterrupt, ref hInterruptEvent);
            rc = PlxIntrEnable(hDevice, ref PlxInterrupt);

            ReceiveFlag = 0xf0;
            PlxBusIopWrite(hDevice, IOP_SPACE.IopSpace1, 0x00001000, true, ref ReceiveFlag, 1, ACCESS_TYPE.BitSize8);


            DmaData.UserVa = getaddress(buffer);
            DmaData.LocalAddr = 0x00008000;
            DmaData.TransferCount = 500000;
            DmaData.LocalToPciDma = 1;// Local to PCI
            DmaData.TerminalCountIntr = 0;
        }

        void Crosscontrol()
        {
            while (true)
            {
                control();
                Thread.Sleep(100);
            }
        }
        private delegate void DelegateFunction();//代理
        void control()
        {
            if (this.trackBar1.InvokeRequired)//等待异步
            {
                DelegateFunction df = new DelegateFunction(control);
                this.Invoke(df);//invoke送主线程
            }
            else
            {
                div = this.trackBar1.Value;//分频
                channel = (int)(this.numericUpDown1.Value);//通道
                textBox1.Clear();
                textBox1.AppendText(datarate.ToString());//数据率
                textBox2.Clear();
                textBox2.AppendText(samplerate.ToString());//采样率
            }
        }

        Int32 count = 0;
        int channel;
        int skip, div;
        float datarate, samplerate;
        void draw()
        {
            Graphics g = pictureBox1.CreateGraphics();
            Pen pn1 = new Pen(Color.White);
            Pen pn2 = new Pen(Color.RoyalBlue);
            List<Point> points1a = new List<Point>();//存直线连接的点
            List<Point> points1b = new List<Point>();//存直线连接的点
            points1a.Add(new Point(1, 1)); points1a.Add(new Point(1, 1));//初始化存直线连接的点
            points1b.Add(new Point(1, 1)); points1b.Add(new Point(1, 1));//初始化存直线连接的点
            int x = 0;
            int i = 0;


            while (true)
            {
                int startTime = System.Environment.TickCount;
                int temp = 0;
                int data_count = 0;

                for (count = 0; count <= 100; count++)
                {
                    DmaData.UserVa = getaddress(buffer);
                    rc = PlxIntrWait(hDevice, hInterruptEvent, 1000);
                    if (rc == RETURN_CODE.ApiSuccess)
                    {
                        rc = PlxDmaSglTransfer(hDevice, DMA_CHANNEL.PrimaryPciChannel0, ref DmaData, false);
                    }
                    i = 0;
                    temp = 0;
                    foreach (byte y in buffer)
                    {
                        if (i < 3)//前三字节 表示数据
                        {
                            temp |= (y << (8 * i));
                        }
                        else if (i == 3)
                        {
                            if ((temp & 0x00800000) != 0)
                            {
                                temp = ~temp;
                                temp = temp + 1;
                                temp = temp & 0x00ffffff;//取绝对值

                                temp = temp >> 8;
                                temp = temp & 0x0000ffff;
                                temp = temp / 2;
                                temp = 150 - temp;
                            }
                            else
                            {
                                temp = temp >> 8;
                                temp = temp & 0x0000ffff;
                                temp = temp / 2;
                                temp = 150 + temp;
                            }
                            if (y == channel)//第四字节 表示通道号
                            {
                                data_count++;
                                skip++;
                                if (skip == div)//隔div个点取一次数据
                                {
                                    skip = 0;
                                    x++;
                                    if (x < 800)
                                    {
                                        points1a.Add(new Point(x, temp));
                                    }
                                    else if (x == 800)
                                    {
                                        g.DrawLines(pn1, points1b.ToArray());//points1b填白清空
                                        points1b.Clear();
                                        g.DrawLines(pn2, points1a.ToArray());//points1a画黑线
                                    }
                                    else if (x < 1600)
                                    {
                                        points1b.Add(new Point(x - 800, temp));
                                    }
                                    if (x == 1600)
                                    {
                                        g.DrawLines(pn1, points1a.ToArray());//points1a填白清空
                                        points1a.Clear();
                                        g.DrawLines(pn2, points1b.ToArray());//points1b画黑线
                                        x = 0;
                                    }
                                }
                            }
                        }
                        i++;
                        if (i == 4)
                        {
                            i = 0;
                            temp = 0;
                        }
                    }
                }
                int endTime = System.Environment.TickCount;
                int runTime = endTime - startTime;
                datarate = 500 * 100 / (float)runTime;
                samplerate = data_count / (float)runTime;
                data_count = 0;
            }
        }

        unsafe static UInt32 getaddress(byte[] b)
        {
            fixed (byte* p = b)
            {
                return ((UInt32)p);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (td1 != null)
            {
                td1.Abort();
                td1.Join();
            }
            if (td2 != null)
            {
                td2.Abort();
                td2.Join();
            }
        }
    }
}
