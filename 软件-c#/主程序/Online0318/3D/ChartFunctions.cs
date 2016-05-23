using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Online0318
{
    /// <summary>
    /// A class that provides some data series for testing 
    /// 取数据送往显示。为测试提供数据源，可以由系统自行产生，也可以从数据文件中读取数据
    /// </summary>
    /// 
    public class ChartFunctions
    {
        #region Constructor

        public ChartFunctions()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// 创建用于绘制三维折线图的数据
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="cs"></param>
        public void Line3D(DataSeries ds, ChartStyle cs)
        {
            cs.XMin = -1f;
            cs.XMax = 1f;
            cs.YMin = -1f;
            cs.YMax = 1f;
            cs.ZMin = 0;
            cs.ZMax = 30;
            cs.XTick = 0.5f;
            cs.YTick = 0.5f;
            cs.ZTick = 5;


            ds.XDataMin = cs.XMin;
            ds.YDataMin = cs.YMin;
            ds.XSpacing = 0.3f;
            ds.YSpacing = 0.3f;
            ds.XNumber = Convert.ToInt16((cs.XMax - cs.XMin) / ds.XSpacing) + 1;
            ds.YNumber = Convert.ToInt16((cs.YMax - cs.YMin) / ds.YSpacing) + 1;
            ds.PointList.Clear();

            for (int i = 0; i < 300; i++)
            {
                float t = 0.1f * i;
                float x = (float)Math.Exp(-t / 30) *
                    (float)Math.Cos(t);
                float y = (float)Math.Exp(-t / 30) *
                    (float)Math.Sin(t);
                float z = t;
                ds.AddPoint(new Point3(x, y, z, 1));
            }
            //float x, y, z;
            //for (int i = 0; i < 1; i++)
            //{
            //    x = 0.1f * i;
            //    for (int j = 0; j < 10; j++)
            //    {
            //        y = 0.1f * j;
            //        z = 20 * y;
            //        ds.AddPoint(new Point3(x, y, z, 1));
            //    }
            //}
        }
        /// <summary>
        /// 测试网格图
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="cs"></param>
        public void Peak3D(DataSeries ds, ChartStyle cs)
        {
            cs.XMin = -3;
            cs.XMax = 3;
            cs.YMin = -3;
            cs.YMax = 3;
            cs.ZMin = -8;
            cs.ZMax = 8;
            cs.XTick = 1;
            cs.YTick = 1;
            cs.ZTick = 4;

            ds.XDataMin = cs.XMin;
            ds.YDataMin = cs.YMin;
            ds.XSpacing = 0.3f;
            ds.YSpacing = 0.3f;
            ds.XNumber = Convert.ToInt16((cs.XMax - cs.XMin) / ds.XSpacing) + 1;
            ds.YNumber = Convert.ToInt16((cs.YMax - cs.YMin) / ds.YSpacing) + 1;

            Point3[,] pts = new Point3[ds.XNumber, ds.YNumber];
            for (int i = 0; i < ds.XNumber; i++)
            {
                for (int j = 0; j < ds.YNumber; j++)
                {
                    float x = ds.XDataMin + i * ds.XSpacing;
                    float y = ds.YDataMin + j * ds.YSpacing;
                    double zz = 3 * Math.Pow((1 - x), 2) * Math.Exp(-x * x -
                        (y + 1) * (y + 1)) - 10 * (0.2 * x - Math.Pow(x, 3) -
                        Math.Pow(y, 5)) * Math.Exp(-x * x - y * y) -
                        1 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y);
                    float z = (float)zz;
                    pts[i, j] = new Point3(x, y, z, 1);
                }
            }
            ds.PointArray = pts;
        }
        /// <summary>
        /// 曲面图和轮廓图
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="cs"></param>
        public void SinROverR3D(DataSeries ds, ChartStyle cs)
        {
            cs.XMin = -8;
            cs.XMax = 8;
            cs.YMin = -8;
            cs.YMax = 8;
            cs.ZMin = -0.5f;
            cs.ZMax = 1;
            cs.XTick = 4;
            cs.YTick = 4;
            cs.ZTick = 0.5f;

            ds.XDataMin = cs.XMin;
            ds.YDataMin = cs.YMin;
            ds.XSpacing = 0.5f;
            ds.YSpacing = 0.5f;
           
            ds.XNumber = Convert.ToInt16((cs.XMax - cs.XMin) / ds.XSpacing) + 1;
            ds.YNumber = Convert.ToInt16((cs.YMax - cs.YMin) / ds.YSpacing) + 1;

            Point3[,] pts = new Point3[ds.XNumber, ds.YNumber];
            for (int i = 0; i < ds.XNumber; i++)
            {
                for (int j = 0; j < ds.YNumber; j++)
                {
                    float x = ds.XDataMin + i * ds.XSpacing;
                    float y = ds.YDataMin + j * ds.YSpacing;
                    float r = (float)Math.Sqrt(x * x + y * y) + 0.000001f;
                    float z = (float)Math.Sin(r) / r;
                    pts[i, j] = new Point3(x, y, z, 1);
                }
            }
            ds.PointArray = pts;
        }
        float randdata = 0;
        /// <summary>
        /// 随机数据
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="cs"></param>
        public void RandData(DataSeries ds, ChartStyle cs)
        {
            randdata++;
            cs.XMin = -8;
            cs.XMax = 8;
            cs.YMin = -randdata;
            cs.YMax = -randdata+16;
            cs.ZMin = -1f;
            cs.ZMax = 16;
            cs.XTick = 4;
            cs.YTick = 4;
            cs.ZTick = 4f;

            ds.XDataMin = cs.XMin;
            ds.YDataMin = cs.YMin;
            ds.XSpacing = 1f;
            ds.YSpacing = 1f;

            ds.XNumber = Convert.ToInt16((cs.XMax - cs.XMin) / ds.XSpacing) + 1;
            ds.YNumber = Convert.ToInt16((cs.YMax - cs.YMin) / ds.YSpacing) + 1;
            Random rd = new Random();
            Point3[,] pts = new Point3[ds.XNumber, ds.YNumber];
            for (int i = 0; i < ds.XNumber; i++)
            {
                for (int j = 0; j < ds.YNumber; j++)
                {
                    float x = ds.XDataMin + i * ds.XSpacing ;
                    float y = ds.YDataMin + j * ds.YSpacing;//时间轴
                    //float z = 2 * x + randdata / 10+(float)j/5;
                    float z = rd.Next(16);
                    pts[i, j] = new Point3(x, y, z, 1);
                }
            }
            ds.PointArray = pts;
            if(randdata>=10)
            {
                randdata = 0;
            }
        }
        /// <summary>
        /// 测试切片图的标量量数据
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="cs"></param>
        public void Exp4D(DataSeries ds, ChartStyle cs)
        {
            cs.XMin = -2;
            cs.XMax = 2;
            cs.YMin = -2;
            cs.YMax = 2;
            cs.ZMin = -2;
            cs.ZMax = 2;
            cs.XTick = 1;
            cs.YTick = 1;
            cs.ZTick = 1;

            ds.XDataMin = cs.XMin;
            ds.YDataMin = cs.YMin;
            ds.ZZDataMin = cs.ZMin;
            ds.XSpacing = 0.1f;
            ds.YSpacing = 0.1f;
            ds.ZSpacing = 0.1f;
            ds.XNumber = Convert.ToInt16((cs.XMax - cs.XMin) / ds.XSpacing) + 1;
            ds.YNumber = Convert.ToInt16((cs.YMax - cs.YMin) / ds.YSpacing) + 1;
            ds.ZNumber = Convert.ToInt16((cs.ZMax - cs.ZMin) / ds.ZSpacing) + 1;

            Point4[, ,] pts = new Point4[ds.XNumber, ds.YNumber, ds.ZNumber];
            for (int i = 0; i < ds.XNumber; i++)
            {
                for (int j = 0; j < ds.YNumber; j++)
                {
                    for (int k = 0; k < ds.ZNumber; k++)
                    {
                        float x = ds.XDataMin + i * ds.XSpacing;
                        float y = ds.YDataMin + j * ds.YSpacing;
                        float z = cs.ZMin + k * ds.ZSpacing;
                        float v = z * (float)Math.Exp(-x * x - y * y - z * z);
                        pts[i, j, k] = new Point4(new Point3(x, y, z, 1), v);
                    }
                }
            }
            ds.Point4Array = pts;
        }
        /// <summary>
        /// 读取外部文件数据
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="cs"></param>
        public void ReadDataFromFile(DataSeries ds, ChartStyle cs)
        {
            string inputdata = "";
            int count = 0;
            int n = 0;
            string[] readArray = new string[2];
            float[] xvalue = new float[2], yvalue = new float[2], zvalue = new float[2];
            cs.XMin = -3;
            cs.XMax = 3;
            cs.YMin = -3;
            cs.YMax = 3;
            cs.ZMin = -8;
            cs.ZMax = 8;
            cs.XTick = 1;
            cs.YTick = 1;
            cs.ZTick = 4;
            //Read data from file
            string filename = System.AppDomain.CurrentDomain.BaseDirectory + "testingdata.txt";
            StreamReader sr = new StreamReader(filename);
            char[] cSplitter = { ' ', ',', ':', '\t' };
            inputdata = sr.ReadLine();
            readArray = inputdata.Split(cSplitter);
            xvalue = new float[readArray.Length - 1];
            yvalue = new float[readArray.Length - 1];
            zvalue = new float[xvalue.Length * yvalue.Length];
            for (int i = 0; i < xvalue.Length; i++)
            {
                xvalue[i] = float.Parse(readArray[i + 1]);
            }
            while(inputdata!=null)
            {
                inputdata = sr.ReadLine();
                if (inputdata == null)
                {
                    break;
                }
                readArray = inputdata.Split(cSplitter);
                yvalue[n] = float.Parse(readArray[0]);
                for (int i = 0; i < yvalue.Length; i++)
                {
                    zvalue[count] = float.Parse(readArray[i + 1]);
                    count++;
                }
                n++;
            }

            Point3[,] pts = new Point3[xvalue.Length, yvalue.Length];

            float x, y, z;
            for (int i = 0; i < xvalue.Length; i++)
            {
                x = xvalue[i];
                for (int j = 0; j < yvalue.Length; j++)
                {
                    y = yvalue[j];
                    z = zvalue[j + i * yvalue.Length];
                    //pts[i - 1, j - 1] = new Point3(x, y, z, 1);
                    pts[i, j] = new Point3(x, y, z, 1);
                }
            }
            ds.PointArray = pts;//取出文件里的数据——显示

            ds.XDataMin = pts[0, 0].X;
            ds.YDataMin = pts[0, 0].Y;
            ds.XSpacing = pts[1, 0].X - pts[0, 0].X;
            ds.YSpacing = pts[0, 1].Y - pts[0, 0].Y;
            ds.XNumber = pts.GetLength(0);
            ds.YNumber = pts.GetLength(1);

        }

        /// <summary>
        /// 随机数据
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="cs"></param>
        public void Show3DData(DataSeries ds, ChartStyle cs)
        {
            cs.XMin = 0;
            cs.XMax = 700;
            cs.YMin = 0;
            cs.YMax = 10;
            cs.ZMin = 0f;
            cs.ZMax = 1000000;
            cs.XTick = 100;//大tick
            cs.YTick = 2;
            cs.ZTick = 250000f;

            ds.XDataMin = cs.XMin;
            ds.YDataMin = cs.YMin;
            ds.XSpacing = 1f;//小tick
            ds.YSpacing = 1f;
            //ds.ZSpacing = 1f;
            ds.XNumber = 500;
            //ds.XNumber = Convert.ToInt16((cs.XMax - cs.XMin) / ds.XSpacing) ;
            ds.YNumber = Convert.ToInt16((cs.YMax - cs.YMin) / ds.YSpacing);//==================多减了个1
            Random rd = new Random();
            Point3[,] pts = new Point3[ds.XNumber, ds.YNumber];
            for (int i = 0; i < ds.XNumber; i++)
            {
                for (int j = 0; j < ds.YNumber; j++)
                {
                    float x = (float)StaticData.ListPoint[j][i].X;
                    float y = j;
                    float z=(float)StaticData.ListPoint[j][i].Y;;
                    if (z>1000000)
                    {
                        z = 1000000; 
                    }
                    pts[i, j] = new Point3(x, y, z, 1);
                }
            }
            ds.PointArray = pts;
            
        }
        #endregion

    }
}
