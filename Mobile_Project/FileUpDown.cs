using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Windows;
namespace Mobile_Project
{
    class FileUpDown
    {
        /// 将本地文件上传到指定的服务器(HttpWebRequest方法)   
        /// </summary>   
        /// <param name="address">文件上传到的服务器</param>   
        /// <param name="fileNamePath">要上传的本地文件（全路径）</param>   
        /// <param name="saveName">文件上传后的名称</param>    
        /// <returns>成功返回1，失败返回0</returns>   
        static public bool Upload_Request(String address, String fileNamePath, String saveName)
        {
            // 要上传的文件   
            FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);

            //时间戳   
            String strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");

            //请求头部信息   
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(strBoundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append("file");
            sb.Append("\"; filename=\"");
            sb.Append(saveName);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append("application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");
            String strPostHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);


            StringBuilder sb1 = new StringBuilder();
            sb1.Append("--");
            sb1.Append(strBoundary);
            sb1.Append("\r\n");
            sb1.Append("Content-Disposition: form-data; name=\"");
            sb1.Append("file");
            sb1.Append("\"; filename=\"");
            sb1.Append("\"");
            sb1.Append("\r\n");
            sb1.Append("Content-Type: ");
            sb1.Append("application/octet-stream");
            sb1.Append("\r\n");
            sb1.Append("\r\n");
            String strPostHeader1 = sb1.ToString();
            byte[] postHeaderBytes1 = Encoding.UTF8.GetBytes(strPostHeader1);

            // 根据uri创建HttpWebRequest对象   
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(address));
            httpReq.Method = "POST";

            //对发送的数据不使用缓存   
            httpReq.AllowWriteStreamBuffering = false;

            //设置获得响应的超时时间（300秒）   
            httpReq.Timeout = 300000;
            httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;           
            long length = fs.Length + postHeaderBytes.Length + postHeaderBytes1.Length + boundaryBytes.Length;            
            long fileLength = fs.Length;
            httpReq.ContentLength = length;
            try
            {

                //每次上传4k   
                int bufferLength = 4096;
                byte[] buffer = new byte[bufferLength];

                //已上传的字节数   
                long offset = 0;

                //开始上传时间   
                DateTime startTime = DateTime.Now;
                int size = r.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();

                //发送请求头部消息   
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;
                    size = r.Read(buffer, 0, bufferLength);
                }
                postStream.Write(postHeaderBytes1, 0, postHeaderBytes1.Length);
                //添加尾部的时间戳   
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();

                //获取服务器端的响应   
                WebResponse webRespon = httpReq.GetResponse();
                Stream s = webRespon.GetResponseStream();
                StreamReader sr = new StreamReader(s);

                //读取服务器端返回的消息   
                String sReturnString = sr.ReadLine();
                s.Close();
                sr.Close();

            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                fs.Close();
                r.Close();
            }

            return true;
        }


        static public void DownFile(String url, String savepath)
        {
            String address = Mobile_Project .Properties.Settings.Default.FileUrlBefore+ "/" + url;
            WebClient dc = new WebClient();
            dc.DownloadFile(address, savepath);
        }


        static public bool Upload(String address, String fileNamePath, String saveName)

        {
            String path = address + saveName; 
            WebClient webclient = new WebClient();
           // webclient.Credentials = CredentialCache.DefaultCredentials;
            webclient.UploadFile(path, fileNamePath);  // 上传文件
            //FileStream filestream = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
           /// BinaryReader br = new BinaryReader(filestream);
           // byte[] uploadArray = br.ReadBytes((int)filestream.Length);
            
            //try
            //{
                            
            //    Stream uploadStream = webclient.OpenWrite(path);
            //    if (uploadStream.CanWrite)
            //    {
            //        uploadStream.Write(uploadArray, 0, uploadArray.Length);
                    
            //    }
            //    uploadStream.Close();
            //    return true;
                
            //}
            //catch(Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}
            return false;
        }
    }
}
