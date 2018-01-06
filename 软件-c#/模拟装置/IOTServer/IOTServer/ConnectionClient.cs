using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace IOTServer
{        
    
    /// <summary>
    /// 与客户端的 连接通信类(包含了一个 与客户端 通信的 套接字，和线程)
    /// </summary>
    /// 
    public delegate void DGShowMsg(string strMsg);
    public class ConnectionClient
    {
        public delegate void DGBagsNum(double bagsNum);
        DGBagsNum BagsNum;
        Socket sokMsg;
        DGShowMsg dgShowMsg;//负责 向主窗体文本框显示消息的方法委托
        Thread threadMsg, threadReadData;

        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sokMsg">通信套接字</param>
        /// <param name="dgShowMsg">向主窗体文本框显示消息的方法委托</param>
        public ConnectionClient(Socket sokMsg, DGShowMsg dgShowMsg, DGBagsNum bagsNum)
        {
            this.sokMsg = sokMsg;
            this.dgShowMsg = dgShowMsg;//实例化委托，此后dgShowMsg(前)拥有了dgShowMsg(后)的特性
            this.BagsNum = bagsNum;
            this.threadMsg = new Thread(RecMsg);
            this.threadMsg.IsBackground = true;
            this.threadMsg.Start();
        }
        #endregion

       
        #region 监听客户端发送来的请求
        bool isRec = true;
        int preHz = 0;//预分频，用于计算1秒发多少数据包。
        int[] divHz = new int[4];//通道分频
        int bagsSendNum = 0;
        void RecMsg()
        {
            while (isRec)
            {
                try
                {
                    byte[] byteMsg = new byte[8];
                    char[] charMsg = new char[8];
                    char[] datamlrec = new char[8];//反馈 
                    datamlrec[0] = 'A'; datamlrec[1] = 'C'; datamlrec[2] = 'K';
                    datamlrec[3] = datamlrec[4] = datamlrec[5] = datamlrec[6] = datamlrec[7] = '0';//默认
                    //接收 对应 客户端发来的消息
                    int length = sokMsg.Receive(byteMsg);
                    charMsg = Encoding.UTF8.GetChars(byteMsg);

                    #region INT
                    if ((charMsg[0] == 'I') && (charMsg[1] == 'N') && (charMsg[2] == 'T'))
                    {
                        dgShowMsg("初始化.....");
                        datamlrec[4] = (char)1;
                        datamlrec[5] = (char)6;
                        datamlrec[6] = (char)1;
                        datamlrec[7] = (char)4;
                        Send(datamlrec);
                    } 
                    #endregion
                   
                    #region PRE
                    if ((charMsg[0] == 'P') && (charMsg[1] == 'R') && (charMsg[2] == 'E'))
                    {
                        preHz = Convert.ToInt16(charMsg[7]);//处理总分频，用于计算发多少个包。93.75/500(a+1)
                        dgShowMsg("总分频.....每秒发" + (93750 / (500 * (preHz + 1))).ToString() + "个数据包");
                        Send(datamlrec);
                    } 
                    #endregion
                  
                    #region DIV
                    if ((charMsg[0] == 'D') && (charMsg[1] == 'I') && (charMsg[2] == 'V'))
                    {
                        #region MyRegion
                        switch (charMsg[6])//通道号不能大于通道数4
                        {
                            case (char)1:
                                {
                                    divHz[0] = 500 / (1 + Convert.ToInt16(charMsg[7]));//Convert.ToInt16(dataml[7])通道分频，用于计算每个个通道占得包长。500/(DIV+1)
                                    dgShowMsg("1#通道分频....."+(2*divHz[0]).ToString()+"个字节");
                                } break;
                            case (char)2:
                                {
                                    divHz[1] = 500 / (1 + Convert.ToInt16(charMsg[7]));//Convert.ToInt16(dataml[7])通道分频，用于计算每个个通道占得包长。500/(DIV+1)
                                    dgShowMsg("2#通道分频....." + (2 * divHz[1]).ToString() + "个字节");
                                } break;
                            case (char)3:
                                {
                                    divHz[2] = 500 / (1 + Convert.ToInt16(charMsg[7]));//Convert.ToInt16(dataml[7])通道分频，用于计算每个个通道占得包长。500/(DIV+1)
                                    dgShowMsg("3#通道分频....." + (2 * divHz[2]).ToString() + "个字节");
                                } break;
                            case (char)4:
                                {
                                    divHz[3] = 500 / (1 + Convert.ToInt16(charMsg[7]));//Convert.ToInt16(dataml[7])通道分频，用于计算每个个通道占得包长。500/(DIV+1)
                                    dgShowMsg("4#通道分频....." + (2 * divHz[3]).ToString() + "个字节");
                                } break;
                        }
                        #endregion
                        Send(datamlrec);
                    } 
                    #endregion

                    #region STA
                    if ((charMsg[0] == 'S') && (charMsg[1] == 'T') && (charMsg[2] == 'A'))
                    {
                        threadReadData = new Thread(WriteData);
                        threadReadData.IsBackground = true;
                        threadReadData.Start();
                        dgShowMsg("发送数据包....."); 
                    }
                    #endregion
                  
                    #region END
                    if ((charMsg[0] == 'E') && (charMsg[1] == 'N') && (charMsg[2] == 'D'))
                    {
                        sokMsg.Disconnect(true);
                        dgShowMsg("断开连接.....");
                        Send(datamlrec);
                    }
                    #endregion

                    continue;
                }
                catch (Exception ex)
                {
                    isRec = false;
                    //从主窗体中 移除 下拉框中对应的客户端选择项，同时 移除 集合中对应的 ConnectionClient对象
                    //dgRemoveConnection(sokMsg.RemoteEndPoint.ToString());
                }
            }
        }
        #endregion

        #region 向数据包写入数据
        int count = 1;//计算数据包已发的个数
        Random rand = new Random();
        void WriteData()
        {
            try
            {
                int hehha = 2 * (divHz[0] + divHz[1] + divHz[2] + divHz[3]) + 26;
                if (hehha % 2 == 1)
                {
                    hehha = hehha + 1;
                }
                char[] charData = new char[hehha];
                byte[] byteData = new byte[hehha];
                short st;
                int sinso = 0;
                int cosso = 0;
                int zhengbili = 0;
                bool saatru = true;
                while (isRec)
                {
                    //Stopwatch sw = new Stopwatch();
                    //sw.Start();
                   
                    #region 写入数据
                    charData[0] = 'D';
                    charData[1] = 'A';
                    charData[2] = 'T';
                    charData[3] = (char)(count >> 8);
                    charData[4] = (char)(count);
                    int zhende = 0;
                    for (int i = 0; i < divHz[0]; i++)//1号通道
                    {
                        st = (short)rand.Next(100);
                        charData[5 + 2 * i] = (Char)(st >> 8);
                        charData[6 + 2 * i] = (Char)(st);
                    }
                    zhende = 2 * divHz[0];
                    for (int i = 0; i < divHz[1]; i++)//2号通道
                    {
                        st = (short)cosso;
                        //st = Convert.ToInt16(Math.Abs(Math.Sin(Math.PI * cosso / Class1.TT)) * 100);
                        if (saatru)
                        {
                            cosso++;
                            if (cosso >= 100)
                            {
                                cosso = 0;
                            }
                        }
                        else 
                        {
                            cosso--;
                            if (cosso <= 0)
                            {
                                cosso = 100;
                            }
                        }
                        charData[5 + zhende + 2 * i] = (Char)(st >> 8);
                        charData[6 + zhende + 2 * i] = (Char)(st);
                    }
                    zhende = 2 * divHz[0] + 2 * divHz[1];
                    for (int i = 0; i < divHz[2]; i++)//3号通道
                    {
                        //st = (short)rand.Next(100);
                        st = Convert.ToInt16(Math.Abs(Math.Sin(Math.PI * sinso / Class1.TT)) * 20);
                        if(Class1.lkls)
                        {
                            st = (short)rand.Next(80,100);
                           
                        }
                        sinso++;
                        if(sinso==Class1.TT)
                        {
                            sinso = 0;
                        }
                        charData[5 + zhende + 2 * i] = (Char)(st >> 8);
                        charData[6 + zhende + 2 * i] = (Char)(st);
                    }
                    //if (Class1.lkls)
                    //{
                    //    Class1.lkls = false;
                    //}
                    zhende = 2 * divHz[0] + 2 * divHz[1] + 2 * divHz[2];
                    for (int i = 0; i < divHz[3]; i++)//4号通道
                    {
                        st = (short)zhengbili;
                        zhengbili++;
                        if (zhengbili==100)
                        {
                            zhengbili = 0;
                        }
                        charData[5 + zhende + 2 * i] = (Char)(st >> 8);
                        charData[6 + zhende + 2 * i] = (Char)(st);
                    }
                    zhende = 2 * divHz[0] + 2 * divHz[1] + 2 * divHz[2] + 2 * divHz[3];
                    st = (short)rand.Next(100);
                    charData[5 + zhende] = (Char)(st >> 8);
                    charData[6 + zhende] = (Char)(st);
                    for (int j = 0; j < 6; j++)
                    {
              
                        charData[7 + zhende + 2 * j] = (char)rand.Next(20,60);
                        charData[8 + zhende + 2 * j] = (char)rand.Next(0,9);
                    }
                    charData[19 + zhende] = (char)rand.Next(90);
                    charData[20 + zhende] = (char)rand.Next(90);
                    charData[21 + zhende] = '_';
                    charData[22 + zhende] = 'P';
                    charData[23 + zhende] = 'S';
                    charData[24 + zhende] = 'A';
                    charData[25 + zhende] = 'I';
                    #endregion
                    if (saatru)
                    {
                        saatru = false;
                    }
                    else 
                    {
                        saatru = true;
                    }
                    if (bagsSendNum==343)
                    {
                        int sdd = 0;
                    }
                    Send(charData);
                    bagsSendNum++;
                    BagsNum(bagsSendNum);
                  
                    count++;
                    if (count == ((93750 / (500 * (preHz + 1))) + 1))
                    {
                        count = 1;
                        //sw.Stop();
                        //TimeSpan ts2 = sw.Elapsed;
                        //BagsNum(ts2.TotalMilliseconds);
                        Delay(850);
                    }

                }
            }
            catch (Exception ex)
            {
                dgShowMsg(ex.ToString());
                isRec = false;
                threadReadData.Abort();
            }
        } 
        #endregion

        #region 向客户端发送消息
        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="strMsg"></param>
        public void Send(char[] strMsg)
        {
            byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg);
            sokMsg.Send(arrMsg, arrMsg.Length, SocketFlags.None);
        }
        #endregion

        private void Delay(int mm) 
        {
            DateTime cu = DateTime.Now;
            int aa = 0;
            while(cu.AddMilliseconds(mm)>DateTime.Now)
            {
                aa++;
            }
            return;
        }
    }
}
