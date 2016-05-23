using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyDllLibrary;
using Method;
using System.Threading;

namespace Online0318
{
    class StaticData
    {
        
        #region MyRegion
        //静态数据成员    
        private static Dictionary<string, PointPair4> ls;
        //静态构造函数        
        static StaticData()
        {
            ls = new Dictionary<string, PointPair4>();
            PointPair4 p0 = new PointPair4(0, 2000,1800,1600);//x下限.y上限.zbox起点纵坐标.tbox高
            ls.Add("方差",p0);
            PointPair4 p1 = new PointPair4(0,100,90,80);
            ls.Add("均方根", p1);
            PointPair4 p2 = new PointPair4(0, 20,18,16);
            ls.Add("峭度", p2);
            PointPair4 p3 = new PointPair4(0, 20, 18, 16);
            ls.Add("峰值指标", p3);
            PointPair4 p4 = new PointPair4(0, 20, 18, 16);
            ls.Add("脉冲指标", p4);
            PointPairList list = new PointPairList();
            for(int i = 0; i < 500;i++ )
            {
                list.Add(0,0);
            }
            listpoint = new List<PointPairList>();
            for (int i = 0; i < 10;i++ )
            {
                listpoint.Add(list);
            }
        }
        //静态属性    
        public static Dictionary<string, PointPair4> LS
        {
            get { return ls; }
        }  
        #endregion       
        
        private static List<PointPairList> listpoint;

        //静态属性    
        public static List<PointPairList> ListPoint
        {
            set { listpoint=value; }
            get { return listpoint; }
        }


        private static int dsds;

        //静态属性    
        public static int DSDS
        {
            set { dsds = value; }
            get { return dsds; }
        }
    }
}
