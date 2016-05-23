using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MyDllLibrary;
using System.Threading;
using Method;

namespace Online0318
{
    public partial class SearchHistory : Form
    {
        #region 字段
        LineItem myCurve;
        PointPairList list10 = new PointPairList();
        List<short> ashj = new List<short>();
        List<double> dddd = new List<double>();
        string dd = "";//文件内的时间处理
        string searchpath = "";
        DateTime dt1, dt2;
        string[] tepName = {"MCU","DSP","网卡","CPU-电源","ad-1","ad-2"};
        string[] tepshiName = { "环境温度", "环境湿度" };
        string[] s4;
        char[] fenpian;
        #endregion
        public MasterPane MasterPan1 { get { return zedGraphControl1.MasterPane; } }
        public SearchHistory()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;//设置窗体居中
            this.WindowState = FormWindowState.Maximized;
            label6.Visible = false;
            progressBar1.Visible = false;
            comboBox2.SelectedIndex = 0;
        }
        private void SearchHistory_Load(object sender, EventArgs e)
        {
            searchpath = Application.StartupPath + "\\Data";
            openFileDialog1.InitialDirectory = Application.StartupPath + "\\Data";//指定初始路径
        }
      
        #region 事件
        /// <summary>
        /// 清除树节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearBt_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            list10.Clear();
            zedGraphControl1.Refresh();
            radioButton1.Checked = true;
        }
        /// <summary>
        /// 分析数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalysisBt_Click(object sender, EventArgs e)
        {
            if (treeView1.Nodes.Count == 0 || treeView1.SelectedNode == null || treeView1.SelectedNode.Nodes.Count > 0)
            {
                MessageBox.Show("尚未选择左侧数据节点");
                return;
            }
            DatasDo(searchpath);          
        }
        /// <summary>
        /// 选中节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count == 0)//选中子节点
            {
                if (radioButton1.Checked)
                {
                    searchpath = e.Node.Parent.Text + "\\" + e.Node.Text;
                }
                if (radioButton2.Checked)
                {
                    searchpath = e.Node.Parent.Text + e.Node.Text;
                }
            }
            else
            {
                e.Node.Expand();
            }
        }
        /// <summary>
        /// 查询文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBt_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                dt1 = dateTimePicker2.Value;
                dt2 = dateTimePicker3.Value;
                int dSubtract = (int)dt2.Subtract(dt1).TotalMinutes + 1;
                if (dSubtract <= 0)
                {
                    MessageBox.Show("时间格式不对或者该时间段内没有满足条件的数据文件，请重新选择。");
                    dateTimePicker2.Focus();
                    return;
                }
                treeView1.Nodes.Clear();
                GetAllFile(Application.StartupPath + "\\Data");
            }
            if(radioButton2.Checked)
            {
                if (!Directory.Exists(Application.StartupPath + "\\Data"))
                {
                    //MessageBox.Show("本地路径: "+Application.StartupPath + "\\Data "+"没有数据！");
                    openFileDialog1.InitialDirectory = Application.StartupPath;
                }
                #region MyRegion
                openFileDialog1.Filter = "bin文件(*.bin)|*.bin";//筛选文件
                openFileDialog1.Multiselect = true;//允许选中多个文件

                if (openFileDialog1.ShowDialog() == DialogResult.OK)//判断是否选中文件
                {
                    if (openFileDialog1.FileNames.Length <= 0)
                    {
                        return;
                    }
                    int zssdwd = 0;
                A:  string filepath = openFileDialog1.FileNames[zssdwd];
                    FileInfo fi = new FileInfo(filepath);
                    string path = filepath.Substring(0,filepath.Length - fi.Name.Length);

                    if (path.Substring(0, 2) == "温度" || path.Substring(0, 2) == "温湿" || path.Substring(0, 2) == "转速")
                    {
                        zssdwd++;
                        goto A;
                    }
                    //判断路径是否已存在
                    if (!treeView1.Nodes.ContainsKey(path))
                    {    //将文件夹路径添加到树节点中
                        treeView1.Nodes.Add(path, path);//其中第一个path是Key,为了使用ContainsKey()才如此添加
                        foreach (string filename in openFileDialog1.SafeFileNames)
                        {
                            if (filename.Substring(0, 2) == "温度" || filename.Substring(0, 2) == "温湿" || filename.Substring(0, 2) == "转速")
                                continue;
                            if (!treeView1.Nodes[treeView1.Nodes.Count - 1].Nodes.ContainsKey(filename))
                                treeView1.Nodes[treeView1.Nodes.Count - 1].Nodes.Add(filename, filename);
                        }
                    }
                    else 
                    {
                        //TreeNode tnn=new TreeNode();
                        //tnn.Text = path;
                        int meichon = treeView1.Nodes.IndexOfKey(path);
                        foreach (string filename in openFileDialog1.SafeFileNames)
                        {
                            if (filename.Substring(0, 2) == "温度" || filename.Substring(0, 2) == "温湿" || filename.Substring(0, 2) == "转速")
                                continue;
                            if (!treeView1.Nodes[meichon].Nodes.ContainsKey(filename))
                                treeView1.Nodes[meichon].Nodes.Add(filename, filename);
                        }
                    }
                } 
                #endregion
            }
            treeView1.ExpandAll();
        }
        /// <summary>
        /// 查询方式切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == radioButton1 && rb.Checked)
            {
                groupBox2.Enabled = true;
            }
            if (rb == radioButton2 && rb.Checked)
            {
                groupBox2.Enabled = false;
            }
            treeView1.Nodes.Clear();
        }
        /// <summary>
        /// 去掉右键菜单中的某个项
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
        #endregion

        #region 方法
        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="spath"></param>
        private void DatasDo(string spath)
        {
            ashj.Clear();
            dddd.Clear();
            dd = "";
            label6.Visible = true;
            progressBar1.Visible = true;
     
            try
            {
                #region 读取数据
                FileStream ke = new FileStream(spath, FileMode.Open, FileAccess.Read);
                BinaryReader br1 = new BinaryReader(ke);
                //string selectNode = treeView1.SelectedNode.Parent.Parent.Text.Substring(2,2);
                int a = 300;//读取a秒
                int datas = 0;
                int datasAndtime = 0;

                //数据范围 
                datas = 93000;
                datasAndtime = 93023;

                byte[] s = new byte[datasAndtime * a];
                List<byte> s11 = new List<byte>();
                fenpian = new char[datas];//一个振动测点，不分频时187个包，一个包放1000byte
                for (int j = 0; j < a; j++)
                {
                    #region MyRegion
                    //读取时间
                    int recv = br1.Read(s, datasAndtime * j, 23);
                    if (recv <= 0)
                    {
                        break;
                    }
                    for (int i = 0; i < recv; i++)
                    {

                        dd = dd + Convert.ToChar(s[i + datasAndtime * j]).ToString();
                    }
                    ////读取1秒数据,纯的数据
                    int recv1 = br1.Read(s, datasAndtime * j + 23, datas);

                    for (int i = 0; i < recv1; i++)
                    {
                        fenpian[i] = Convert.ToChar(s[i + datasAndtime * j + 23]);
                    }

                    for (int i = 0; i < datas / 2; i++)
                    {
                        ashj.Add(((short)((fenpian[2 * i] << 8) | (fenpian[2 * i + 1]))));
                    }

                    if (j / 100 == 0)
                    {
                        progressBar1.PerformStep();
                    }
                    #endregion
                }

                br1.Close();
                ke.Close();
                 #endregion

                #region 处理时间
                string s1 = dd.Remove(dd.Length - 2, 2);
                string s2 = s1.Remove(0, 2);
                string s3 = s2.Replace("@@@@", "%");
                s4 = s3.Split('%');
                int del = 0;
                for (int i = 0; i < a; i++)
                {
                    if (i < s4.Length)
                    {
                        if (s4[i].Length != 19)
                        {
                            del++;
                            continue;
                        }
                        DateTime dt = DateTime.ParseExact(s4[i], "yyyy-MM-dd-HH-mm-ss", null);
                        dddd.Add((double)new XDate(dt));
                        if ((i - del) > 0 && dddd[i - del] == dddd[i - 1 - del])//处理文件中“相邻的时间相同”的问题
                        {
                            dddd.Remove(dddd[i - del]);
                            dddd.Add((double)new XDate(dt.AddSeconds(1)));
                        }
                    }
                    else
                    {
                        break;
                    }

                }
                #endregion
                list10.Clear();

                DataShow();
                label6.Visible = false;
                progressBar1.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //MessageBox.Show("未能打开文件，清查看文件是否为历史数据文件！");
                return;          
            }
           
        }
        /// <summary>
        /// 显示数据
        /// </summary>
        private void DataShow()
        {
            MasterPan1.PaneList.Clear();
            MasterPan1.Title.FontSpec.Size = 18;
            //MasterPan1.Title.FontSpec.IsDropShadow = true;
            MasterPan1.Title.FontSpec.FontColor = Color.Blue;
            MasterPan1.Title.IsVisible = true;

            MasterPan1.Fill = new Fill(Color.White, Color.LightSkyBlue, 45.0F);
            MasterPan1.Margin.All = 2;    //图框到边框的距离
            MasterPan1.InnerPaneGap = 2; //图框之间的距离
            //zedGraphControl1.IsEnableHZoom = false;
            //zedGraphControl1.IsEnableVZoom = true;

            GraphPane gr = new GraphPane();
            gr.Chart.Fill = new Fill(Color.SkyBlue, Color.WhiteSmoke, 255F);
            MasterPan1.Add(gr);

            string[] heh = searchpath.Split('\\');
            MasterPan1[0].Title.Text = heh[heh.Length - 2]+"—"+treeView1.SelectedNode.Text;
            MasterPan1[0].Legend.Border.IsVisible = false;
            MasterPan1[0].Legend.IsShowLegendSymbols = false;
            MasterPan1[0].Legend.FontSpec.Size = 14;
            MasterPan1[0].Legend.IsVisible = true;
            MasterPan1[0].Legend.Position = LegendPos.TopFlushLeft;
            MasterPan1[0].YAxis.Title.IsVisible = true ;

            MasterPan1[0].Title.FontSpec.Size = 18;//设置字体
            MasterPan1[0].XAxis.Title.FontSpec.Size = 18;
            MasterPan1[0].XAxis.Title.IsVisible = true;
            
            myCurve = MasterPan1[0].AddCurve(null,
                list10, Color.DarkGreen, SymbolType.None);

            if (comboBox1.SelectedIndex==0)
            {
                MasterPan1[0].YAxis.Scale.Min = -255;
                MasterPan1[0].YAxis.Scale.Max = 255;
                MasterPan1[0].XAxis.Scale.Min = 0;//最小值 
                MasterPan1[0].XAxis.Scale.Max = 935;//最小值 
                MasterPan1[0].XAxis.Scale.MinorStep = 187;
                MasterPan1[0].XAxis.Scale.MajorStep = 467.5;
                MasterPan1[0].XAxis.Type = AxisType.DateAsOrdinal;
                MasterPan1[0].XAxis.Scale.Format = "mm:ss";
                MasterPan1[0].XAxis.Title.Text = "时间";
                int sasa = 467 * (dddd.Count-2);
                for (int i = 0; i < sasa; i++)
                {
                    list10.Add(dddd[i / 467], ashj[i * 100]);
                } 
            }
            else
            {
                MasterPan1[0].XAxis.Type = AxisType.Linear;
                MasterPan1[0].XAxis.Scale.Min = 0;//最小值 
                MasterPan1[0].XAxis.Scale.Max = 2000;//最小值 
                MasterPan1[0].YAxis.Scale.MaxAuto=true;
                MasterPan1[0].YAxis.Scale.MinAuto = true;
                MasterPan1[0].XAxis.Title.Text = "频率/Hz";
                double[] inin = new double[32768];
                inin = FFT(ashj);
                double[] frequList = new double[32768];
                for (int i = 0; i < 2000; i++)//只显示前500个点
                {
                    frequList[i] = (double)(46875 * (double)i / 32768);//频率序列赋值
                    list10.Add(frequList[i], inin[i]);
                } 
            }

            myCurve.Line.IsAntiAlias = true;//抗锯齿效果
            zedGraphControl1.IsAutoScrollRange = true;
            zedGraphControl1.AutoScroll = true;
            zedGraphControl1.IsShowHScrollBar = true;
            zedGraphControl1.IsShowVScrollBar = true;
            zedGraphControl1.IsEnableHZoom = true;//禁止放大缩小
            using (Graphics g = zedGraphControl1.CreateGraphics())
            {
                MasterPan1.SetLayout(g, PaneLayout.SingleColumn);
                MasterPan1.AxisChange(g);
                zedGraphControl1.Refresh();
            }
            zedGraphControl1.AxisChange();//加上此句后，滚动条才会固定大小
            list10.RemoveAt(0);
            zedGraphControl1.Refresh();
        }
        public void GetAllFile(string directory)
        {
            if (!Directory.Exists(directory))
            {
                MessageBox.Show("本地路径: " + Application.StartupPath + "\\Data " + "没有数据！");
                return;
            }
            string[] directorys = Directory.GetDirectories(directory);
            if (directorys.Length <= 0) //如果该目录总没有其他文件夹
            {
                MessageBox.Show("本地路径: " + Application.StartupPath + "\\Data " + "没有数据！");
                return;
            }
            for (int i = 0; i < directorys.Length; i++)
            {
                treeView1.Nodes.Add(directorys[i]);
                string[] path = Directory.GetFiles(directorys[i]);

                if (path.Length <= 0) //没有文件，返回
                    continue;
                for (int j = 0; j < path.Length-1; j++)
                {
                    string heh = path[j].Split('\\').Last();
                    if (heh.Split('@', '.')[0].Length < 3)
                        continue;//筛选振动传感器
                    DateTime dt = DateTime.ParseExact(heh.Split('@', '.')[1], "yyyy-MM-dd-HH-mm-ss", null);
                    if ((int)dt2.Subtract(dt).TotalMinutes >= 0 &&
                        (int)dt.Subtract(dt1).TotalMinutes >= 0 &&
                        (heh.Split('@', '.')[0]).Substring(0,3) == comboBox2.Text)
                    {
                        treeView1.Nodes[i].Nodes.Add(heh);
                    }

                }
               
            }
            int sdsad = 0;
            for (int k = 0; k < treeView1.Nodes.Count; k++)
            {
                sdsad = sdsad + treeView1.Nodes[k].GetNodeCount(true);
            }
            if(sdsad<=0)
            {
                MessageBox.Show("找不到文件，请查看本地路径是否有数据或时间段是否正确。");
            }
           
        }
        private double[] FFT(List<short> ls) 
        {
            int acac = 32768;
            double[] inin = new double[acac];
            double[] outout = new double[acac];
            for (int j = 0; j < acac; j++)
            {
                inin[j] = (double)ls[j];
                outout[j] = (double)0;
            }
            FFTs fft = new FFTs();
            int a = fft.upFFT(inin, outout, 1);
            
            return inin;
        }
        #endregion       
       
    }
}
