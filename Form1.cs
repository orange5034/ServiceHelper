using System;
using System.Collections;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;

namespace ServiceHelper
{
    public partial class form1 : Form
    {
        string servicePath = string.Empty;
        string serviceName = string.Empty;

        public form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void InstallServiceButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists(servicePath))
            {
                MessageBox.Show("安装文件错误");
                return;
            }
            if (this.IsServiceExisted(serviceName))
            {
                this.UninstallService(servicePath);
            }
            this.InstallService(servicePath);
            this.StartService(serviceName);
            this.ServiceStatusLabel.Text = "已启动";
            MessageBox.Show("安装成功");
        }

        private void StartServiceButton_Click(object sender, EventArgs e)
        {
            if (this.IsServiceExisted(serviceName))
            {
                this.StartService(serviceName);
                this.ServiceStatusLabel.Text = "已启动";
                MessageBox.Show("服务已启动");
            }
            else
            {
                MessageBox.Show("服务启动失败");
            }

        }

        private void StopServiceButton_Click(object sender, EventArgs e)
        {
            if (this.IsServiceExisted(serviceName))
            {
                this.StopService(serviceName);
                this.ServiceStatusLabel.Text = "已停止";
                MessageBox.Show("服务已停止");
            }
            else
            {
                MessageBox.Show("服务停止失败");
            }
        }

        private void UnInstallServiceButton_Click(object sender, EventArgs e) 
        {
            if (this.IsServiceExisted(serviceName))
            {
                this.StopService(serviceName);
                this.UninstallService(servicePath);
                this.ServiceStatusLabel.Text = "未安装";
                MessageBox.Show("卸载成功");
            }
            else
            {
                MessageBox.Show("服务未安装");
            }
        }

        private bool IsServiceExisted(string name)
        {
            var services = ServiceController.GetServices();
            foreach (var s in services)
            {
                if (s.ServiceName.ToLower() == name.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        private void InstallService(string path)
        {
            using (var install = new AssemblyInstaller())
            {
                install.UseNewContext = true;
                install.Path = path;
                var saveState = new Hashtable();
                install.Install(saveState);
                install.Commit(saveState);

            }
        }

        private void UninstallService(string path)
        {
            using (var install = new AssemblyInstaller())
            {
                install.UseNewContext = true;
                install.Path = path;
                install.Uninstall(null);
            }
        }

        private void StartService(string name)
        {
            using (var controller = new ServiceController(name))
            {
                if (controller.Status == ServiceControllerStatus.Stopped)
                {
                    controller.Start();
                }
            }
        }

        private void StopService(string name)
        {
            using (var controller = new ServiceController(name))
            {
                if (controller.Status == ServiceControllerStatus.Running)
                {
                    controller.Stop();
                }
            }
        }

        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var savePath = this.openFileDialog.FileName;
                this.SelectFilePathRichTextBox.Text = savePath;
                this.servicePath = savePath;
                this.serviceName = this.openFileDialog.SafeFileName.Substring(0,this.openFileDialog.SafeFileName.Length-4);
                using(var controller=new ServiceController(this.serviceName))
                {
                    if (!this.IsServiceExisted(this.serviceName))
                    {
                        this.ServiceStatusLabel.Text = "未安装"; 
                    }
                    else
                    {
                        switch (controller.Status)
                        {
                            case ServiceControllerStatus.Running:
                                this.ServiceStatusLabel.Text = "已启动";
                                return;
                            case ServiceControllerStatus.Stopped:
                                this.ServiceStatusLabel.Text = "已停止";
                                return;
                        }
                    }

                }
            }
        }

    }
}
