using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Online0318
{
    class Maths
    {
        public enum paramvalue
        {
            one,
            two,
            three
        }
        /// <summary>
        /// 最大值，最小值，峰峰值
        /// </summary>
        /// <param name="v"></param>
        /// <param name="maxOrmin">one求最大值，two求最小值，three求峰峰值</param>
        /// <returns></returns>
        public static double MaxOrMin(double[] v, paramvalue maxOrmin)
        {
            double max = v.Max();
            double min = v.Min();
            double maxmin = max - min;
            double ba=0;
            switch (maxOrmin)
            {
                case paramvalue.one: 
                    {
                        ba= max;
                    } break;
                case paramvalue.two:
                    {
                        ba = min;
                    } break;
                case paramvalue.three:
                    {
                        ba = maxmin;
                    } break;
                default: break;
            }
            return ba;
        }
        /// <summary>
        /// 峰值
        /// </summary>
        public static double Max(double[] v)
        {
            double max = v.Max(x => Math.Abs(x));
            return max;
        }
        /// <summary>
        /// 均值
        /// </summary>
        public static double Avg(double[] v)
        {
            double avg = v.Average();
            return avg;
        }

        /// <summary>
        /// 平均幅值(绝对值平均)
        /// </summary>
        public static double PRO(double[] v)
        {
            double pro = v.Sum(x => Math.Abs(x)) / v.Length;
            return pro;
        }
        
         /// <summary>
         /// 方差
         /// </summary>
        public static double Var(double[] v)
         {
             double avg = Avg(v);
             double var1 = (v.Sum(x => (x - avg) * (x - avg))) / (v.Length-1);
             return var1;
         }
        /// <summary>
        /// 方差
        /// </summary>
        /// <param name="v">输入序列</param>
        /// <param name="avg">输入序列的平均值</param>
        /// <returns></returns>
        public static double Var(double[] v,double avg)
        {
            double var1 = (v.Sum(x => (x - avg) * (x - avg))) / (v.Length - 1);
            return var1;
        }
        /// <summary>
         /// 标准差
         /// </summary>
        public static double Std(double[] v)
        {
            return System.Math.Sqrt(Var(v) * (v.Length - 1)/v.Length);//此处标准差与方差不是直接平方的关系
        }

        /// <summary>
        /// 均方幅值（有效值或均方根）
        /// </summary>
        public static double RMS(double[] v)
        {
            double var1 = v.Sum(x => (x * x)) / v.Length;
            return System.Math.Sqrt(var1);
        }

        /// <summary>
        /// 均方值
        /// </summary>
        public static double MeanSquare(double[] v)
        {
            double var1 = v.Sum(x => (x * x)) / v.Length;
            return var1;
        }

        /// <summary>
        /// 峭度
        /// </summary>
        public static double Kurtosis(double[] v)
        {
            double avg = Avg(v);//平均值
            double std = Std(v);//标准差
            double kurtosis = v.Sum(x => (Math.Pow((x - avg) / std, 4))) / v.Length;
            return kurtosis;
        }
        /// <summary>
        /// 峭度
        /// </summary>
        public static double Kurtosis(double[] v,double avg)
        {
            //double avg = Avg(v);//平均值
            double std = Std(v);//标准差
            double kurtosis = v.Sum(x => (Math.Pow((x - avg) / std, 4))) / v.Length;
            return kurtosis;
        }
        /// <summary>
        /// 偏度（歪度）
        /// </summary>
        public static double Skewness(double[] v)
        {
            double avg = Avg(v);//平均值
            double std = Std(v);//标准差
            double skewness = v.Sum(x => (Math.Pow((x - avg) / std, 3))) / v.Length;
            return skewness;
        }

        /// <summary>
        /// 方根幅值
        /// </summary>
        public static double MSA(double[] v)
        { 
            double msa = v.Sum(x => Math.Sqrt(Math.Abs(x))) / v.Length;
            msa = msa*msa;
            return msa;
        }
    }
}
