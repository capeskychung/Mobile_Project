using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Session;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt;
using System.Windows;
using System.Threading;

namespace Mobile_Project
{
    class MqttDeal
    {
        private static MQTTCom client = null;
        static String url = "";
        public static void SetClientID(String id)
        {
            if (client == null)
                client = new MQTTCom();
            client.ClientId = id;
            url = Mobile_Project.Properties.Settings.Default.MqttUrl;
        }


        public static void Subscribe_Messages(String topic)
        {
            if (client == null)
                client = new MQTTCom();
            client.MqttMsgSubscribed += mqttClient_MqttMsgSubscribed;
            client.MqttMsgPublishReceived += mqttClient_MqttMsgPublishReceived;
            topic = "nercms/schedule/" + topic;
            try
            {
                client.Subscribe("202.114.66.77", 1883, new String[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            }
            catch(Exception e)
            {
                MessageBox.Show("Subscribe Failed");
            }
        }

        public static void mqttClient_MqttMsgPublishReceived(object sender,MqttMsgPublishEventArgs e)
        {
            var msg = System.Text.Encoding.Default.GetString(e.Message);
            var obj = sender.ToString();
            DealMsg(msg);
        }

        public static bool PublishMessage(String msg,String topic)
        {
            if (client == null)
                client = new MQTTCom();
            client.MqttMsgPublished += myclient_MqttMsgPublished;
            client.Publish("202.114.66.77", 1883, "mobile_project/" + topic, System.Text.Encoding.UTF8.GetBytes(msg), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            return true;
        }

        static void myclient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {

        }

        public static void mqttClient_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
           
        }

        public static void DealMsg(String msg)
        {
            if(MessageBox.Show("有文件到达，是否下载","下载提示",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    //String url = Mobile_Project.Properties.Settings.Default.FileUrlBefore;
                    Microsoft.Win32.SaveFileDialog sfdFile = new Microsoft.Win32.SaveFileDialog();
                    if (msg == null)
                        sfdFile.FileName = "";
                    else
                    {
                        sfdFile.FileName = msg;


                        sfdFile.Filter = "所有文件|*.*";
                        sfdFile.ShowDialog();
                        String file = sfdFile.FileName;
                        FileUpDown.DownFile(msg, file);

                    }
                    System.Windows.Threading.Dispatcher.Run();
                }));

                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();
            }
            
        }
    
    }
}
