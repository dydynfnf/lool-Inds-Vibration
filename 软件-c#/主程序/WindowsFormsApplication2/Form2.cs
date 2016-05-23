using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging; 

namespace WindowsFormsApplication2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public byte[] data = new byte[3568];
        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics; //创建画板,这里的画板是由Form提供的.
            List<Point> points = new List<Point>();//存直线连接的点
            int x=0;
            int i = 0;
            foreach (byte y in data)
            {
                if (i == 0)
                {
                    points.Add(new Point(x, y));
                    x++;
                }
                i++;
                if (i >= 4)
                {
                    i = 0;
                }
            }
            g.DrawLines(new Pen(new SolidBrush(Color.Black)), points.ToArray());
        }
    }
}
