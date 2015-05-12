using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mobile_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MqttDeal.SetClientID("0");
            MqttDeal.Subscribe_Messages("chaofen");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(this.textBox.Text !="")
            {
                String filename = this.textBox.Text;
                String[] rs = filename.Split(new char[]{'\\'});
                String topicfile = rs[rs.Length - 1];
                rs = topicfile.Split(new char[]{'.'});
                String topic = rs[0];
                String saveName = FileName(this.textBox.Text);
                String fileServer = Mobile_Project.Properties.Settings.Default.FileUrlAfter;
                if(FileUpDown.Upload_Request(fileServer, this.textBox.Text.ToString(), saveName))
                {
                    MessageBox.Show("上传成功");
                    //发送mqtt消息
                    MqttDeal.PublishMessage(saveName, topic);
                }
                    
            }

            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "打开文件";//对话框标题
            openFileDialog.Filter = "文件（.png）|*.png|所有文件|*.*";//文件扩展名
            if ((bool)openFileDialog.ShowDialog().GetValueOrDefault())//打开
            {
                //成功后的处理
                String path = openFileDialog.FileName;
                this.textBox.Text = path;

            }
            
        }


        private String FileName(String path)
        {
            System.DateTime time = DateTime.Now;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1990, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;
            String[] results = path.Split(new char[] { '.' });
            int length = results.Length;
            String savename = t + "." + results[length - 1];
            return savename;
        }
    }
}
