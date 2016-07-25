namespace Online0318
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.登录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.系统管理员身份登录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.登录历史ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.注销登录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出系统ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton1,
            this.toolStripButton4});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(70, 601);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.AutoToolTip = false;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.登录ToolStripMenuItem,
            this.系统管理员身份登录ToolStripMenuItem,
            this.登录历史ToolStripMenuItem,
            this.注销登录ToolStripMenuItem,
            this.退出系统ToolStripMenuItem});
            this.toolStripDropDownButton1.Image = global::Online0318.Properties.Resources.mian_icon3232;
            this.toolStripDropDownButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(67, 53);
            this.toolStripDropDownButton1.Text = "系统登录";
            this.toolStripDropDownButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // 登录ToolStripMenuItem
            // 
            this.登录ToolStripMenuItem.CheckOnClick = true;
            this.登录ToolStripMenuItem.Name = "登录ToolStripMenuItem";
            this.登录ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.登录ToolStripMenuItem.Text = "登录";
            this.登录ToolStripMenuItem.Click += new System.EventHandler(this.登录ToolStripMenuItem_Click);
            // 
            // 系统管理员身份登录ToolStripMenuItem
            // 
            this.系统管理员身份登录ToolStripMenuItem.Checked = true;
            this.系统管理员身份登录ToolStripMenuItem.CheckOnClick = true;
            this.系统管理员身份登录ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.系统管理员身份登录ToolStripMenuItem.Name = "系统管理员身份登录ToolStripMenuItem";
            this.系统管理员身份登录ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.系统管理员身份登录ToolStripMenuItem.Text = "系统管理员身份登录";
            this.系统管理员身份登录ToolStripMenuItem.Visible = false;
            this.系统管理员身份登录ToolStripMenuItem.Click += new System.EventHandler(this.系统管理员身份登录ToolStripMenuItem_Click);
            // 
            // 登录历史ToolStripMenuItem
            // 
            this.登录历史ToolStripMenuItem.Name = "登录历史ToolStripMenuItem";
            this.登录历史ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.登录历史ToolStripMenuItem.Text = "登录历史";
            this.登录历史ToolStripMenuItem.Click += new System.EventHandler(this.登录历史ToolStripMenuItem_Click);
            // 
            // 注销登录ToolStripMenuItem
            // 
            this.注销登录ToolStripMenuItem.Name = "注销登录ToolStripMenuItem";
            this.注销登录ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.注销登录ToolStripMenuItem.Text = "注销登录";
            this.注销登录ToolStripMenuItem.Click += new System.EventHandler(this.注销登录ToolStripMenuItem_Click);
            // 
            // 退出系统ToolStripMenuItem
            // 
            this.退出系统ToolStripMenuItem.Name = "退出系统ToolStripMenuItem";
            this.退出系统ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.退出系统ToolStripMenuItem.Text = "退出系统";
            this.退出系统ToolStripMenuItem.Click += new System.EventHandler(this.退出系统ToolStripMenuItem_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::Online0318.Properties.Resources.Setting3232;
            this.toolStripButton2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(67, 53);
            this.toolStripButton2.Text = "系统配置";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = global::Online0318.Properties.Resources.Mono3232;
            this.toolStripButton3.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(67, 53);
            this.toolStripButton3.Text = "在线监测";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::Online0318.Properties.Resources.PICIcon;
            this.toolStripButton1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolStripButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(67, 53);
            this.toolStripButton1.Text = "PCI采集卡";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Image = global::Online0318.Properties.Resources.Search3232;
            this.toolStripButton4.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(67, 53);
            this.toolStripButton4.Text = "历史查询";
            this.toolStripButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton4.Visible = false;
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 579);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(701, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // panel1
            // 
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(70, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(701, 601);
            this.panel1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::Online0318.Properties.Resources.desktop;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(701, 579);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(771, 601);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(100, 100);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "振动监测系统";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem 登录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 系统管理员身份登录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 登录历史ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 注销登录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出系统ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

