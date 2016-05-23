using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Method;

namespace Online0318
{
    public partial class UserForm : Form
    {
        string oldName, oldPassword;

        public UserForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;//设置窗体居中
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            oldName = SystemConfig.GetConfigData("UserID", string.Empty);
            oldPassword = SystemConfig.GetConfigData("Password", string.Empty);
           
            #region 判断输入是否有错
            if ((txtBoxOldname.Text.Trim().Equals(string.Empty)) | (oldName != txtBoxOldname.Text))
            {
                errorProvider1.SetError(txtBoxOldname, "用户名有误，请重新输入"); //显示错误的提示信息
                MessageBox.Show("用户名有误，请重新输入");
                txtBoxOldname.Text = null;
                txtBoxOldPassword.Text = null;
                txtBoxNewname.Text = null;
                txtBoxNewPassword.Text = null;
                txtBoxConfirm.Text = null;
                txtBoxOldname.Focus();
                return;
            }
            if ((txtBoxOldPassword.Text.Trim().Equals(string.Empty)) | (oldPassword != txtBoxOldPassword.Text))
            {
                errorProvider1.SetError(txtBoxOldPassword, "密码有误，请重新输入"); //显示错误的提示信息
                MessageBox.Show("密码有误，请重新输入");
                txtBoxOldPassword.Text = null;
                txtBoxNewname.Text = null;
                txtBoxNewPassword.Text = null;
                txtBoxConfirm.Text = null;
                txtBoxOldPassword.Focus();
                return;
            }
            if (txtBoxNewname.Text.Trim().Equals(string.Empty))
            {
                errorProvider1.SetError(txtBoxNewname, "用户名为空，请重新输入"); //显示错误的提示信息
                MessageBox.Show("用户名为空，请重新输入");
                txtBoxNewPassword.Text = null;
                txtBoxConfirm.Text = null;
                txtBoxNewname.Focus();
                return;
            }
            if (txtBoxNewPassword.Text.Trim().Equals(string.Empty))
            {
                errorProvider1.SetError(txtBoxNewPassword, "新密码为空，请重新输入"); //显示错误的提示信息
                MessageBox.Show("新密码为空，请重新输入");
                txtBoxConfirm.Text = null;
                txtBoxNewPassword.Focus();
                return;
            }
            if (txtBoxConfirm.Text.Trim().Equals(string.Empty))
            {
                errorProvider1.SetError(txtBoxConfirm, "不能为空，请重新输入"); //显示错误的提示信息
                MessageBox.Show("不能为空，请重新输入");
                txtBoxConfirm.Text = null;
                txtBoxConfirm.Focus();
                return;
            }
            if (txtBoxConfirm.Text != txtBoxNewPassword.Text)
            {
                errorProvider1.SetError(txtBoxNewPassword, "两次输入密码不一致"); //显示错误的提示信息
                MessageBox.Show("两次输入密码不一致");
                txtBoxNewPassword.Text = null;
                txtBoxConfirm.Text = null;
                txtBoxConfirm.Focus();
                return;
            } 
            #endregion

            SystemConfig.WriteConfigData("UserID",txtBoxNewname.Text.Trim());
            SystemConfig.WriteConfigData("Password",txtBoxNewPassword.Text.Trim());
            this.Close();
            GC.Collect();  
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            GC.Collect();
        }

    }
}
