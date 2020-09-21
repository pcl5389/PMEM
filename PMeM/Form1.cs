using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PMeM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Scaler.MemShare.Check(textBox1.Text, textBox2.Text).ToString());
        }
        private void Button4_Click(object sender, EventArgs e)
        {
            textBox4.Text = Scaler.MemShare.toString();
        }
        public string chek(object i)
        {
            Scaler.MemShare.Check(i + "-ddddd", "ffffffffffffffffffff");
            Interlocked.Decrement(ref iwork);
            return "";
        }
        public string sign(object stu_id)
        {
            string url = string.Format("http://localhost:50322/student/test.aspx?b_id=5&s_id={0}&l_id=213&d=fri", stu_id);
            Scaler.Http http = new Scaler.Http();
            string content= http.GetHTML(url, "", "", "", "GET");
            Scaler.Win.WriteLog(stu_id + "\t" + content);
            Interlocked.Decrement(ref iwork);
            return "";
        }
        public string getClient(object obj)
        {
            //Scaler.NewClient ct = Scaler.MemShare.GetClient();
            //Scaler.MemShare.ReleaseClient(ct);
            Interlocked.Decrement(ref iwork);
            return "";
        }

        int iwork = 0;
        private void Button5_Click(object sender, EventArgs e)
        {
            /*
            DateTime dt = DateTime.Now;
            for (int j = 0; j < 10000; j++)
            {
                Scaler.NewClient ct = Scaler.MemShare.GetClient();
                Scaler.MemShare.ReleaseClient(ct);
            }
            MessageBox.Show((DateTime.Now - dt).TotalMilliseconds.ToString());
            //return;
            */
            //* memshare 测试
            for (int i = 0; i < 105375; i++)
            {
                Interlocked.Increment(ref iwork);
                Task<string>.Factory.StartNew(new Func<object, string>(chek), i);
            }
            WaitFinished();
            //*/
        }



        private void WaitFinished()
        {
            while (iwork > 0)
            {
                label1.Text = string.Format("{0}个进程正在执行-{1}", iwork, DateTime.Now.ToString("HH:mm:ss:fff"));
                Application.DoEvents();
                Thread.Sleep(500);
            }
            label1.Text = string.Format("{0}个进程正在执行-{1}", iwork, DateTime.Now.ToString("HH:mm:ss:fff"));
            //MessageBox.Show("已完成");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int.TryParse(null, out int id);
            MessageBox.Show(id.ToString());
            return;
            Stopwatch watch = new Stopwatch();
            //開始计时
            watch.Start();

            for (int i = 0; i < 10000; i++)
            {
                method1();
            }
            watch.Stop();

            //获取当前实例測量得出的总执行时间（以毫秒为单位）
            string time = watch.ElapsedMilliseconds.ToString();
            MessageBox.Show(time);
        }

        public void method1()
        {
            FileInfo fi = new FileInfo("Index_index.html");
            using (MemoryStream fs = new MemoryStream())
            {
                using (FileStream iStream = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    iStream.CopyTo(fs);
                    /*
                long dataLengthToRead = iStream.Length;//获取下载的文件总大小 


                while (dataLengthToRead > 0 && HttpContext.Current.Response.IsClientConnected)
                {
                    int lengthRead = iStream.Read(buffer, 0, CHUNK_SIZE);//读取的大小 
                    HttpContext.Current.Response.OutputStream.Write(buffer, 0, lengthRead);
                    HttpContext.Current.Response.Flush();
                    dataLengthToRead = dataLengthToRead - lengthRead;
                }*/
                }
                string content = Encoding.UTF8.GetString(fs.GetBuffer(), 0, (int)fi.Length);
            }
        }
        public void method2()
        {
            FileInfo fi = new FileInfo("Index_index.html");
            using (MemoryStream fs = new MemoryStream())
            {
                using (FileStream iStream = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    iStream.CopyTo(fs);
                    /*
                long dataLengthToRead = iStream.Length;//获取下载的文件总大小 


                while (dataLengthToRead > 0 && HttpContext.Current.Response.IsClientConnected)
                {
                    int lengthRead = iStream.Read(buffer, 0, CHUNK_SIZE);//读取的大小 
                    HttpContext.Current.Response.OutputStream.Write(buffer, 0, lengthRead);
                    HttpContext.Current.Response.Flush();
                    dataLengthToRead = dataLengthToRead - lengthRead;
                }*/
                }
                string content = Encoding.UTF8.GetString(fs.ToArray());
            }
        }
        const int CHUNK_SIZE = 102400;
        public void method22()
        {
            FileInfo fi = new FileInfo("Index_index.html");

            StringBuilder sb = new StringBuilder();
            byte[] buffer = new byte[CHUNK_SIZE];
            using (FileStream iStream = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long dataLengthToRead = iStream.Length;//获取下载的文件总大小 

                while (dataLengthToRead > 0)
                {
                    int lengthRead = iStream.Read(buffer, 0, CHUNK_SIZE);//读取的大小 
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, lengthRead));
                    dataLengthToRead = dataLengthToRead - lengthRead;
                }
                string content = sb.ToString();

            }

        }

        public void method3()
        {
            FileInfo fi = new FileInfo("Index_index.html");

            using (MemoryStream fs = new MemoryStream())
            {
                byte[] buffer = new byte[CHUNK_SIZE];
                using (FileStream iStream = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    long dataLengthToRead = iStream.Length;//获取下载的文件总大小 


                    while (dataLengthToRead > 0)
                    {
                        int lengthRead = iStream.Read(buffer, 0, CHUNK_SIZE);//读取的大小 
                        fs.Write(buffer, 0, lengthRead);
                        dataLengthToRead = dataLengthToRead - lengthRead;
                    }
                    string content = (Encoding.UTF8.GetString(fs.ToArray()));

                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {

            MessageBox.Show(Scaler.Common.md5(Scaler.Common.md5(Scaler.Common.md5("sdhuijiumanager" + "216a590a1b6848dc2f02ea0dca21ba15") + "216a590a1b6848dc2f02ea0dca21ba15") + "216a590a1b6848dc2f02ea0dca21ba15"));
            return;




            Stopwatch watch = new Stopwatch();
            //開始计时
            watch.Start();

            for (int i = 0; i < 10000; i++)
            {
                method2();
            }
            watch.Stop();

            //获取当前实例測量得出的总执行时间（以毫秒为单位）
            string time = watch.ElapsedMilliseconds.ToString();
            MessageBox.Show(time);
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            Stopwatch watch = new Stopwatch();
            //開始计时
            watch.Start();

            for (int i = 0; i < 10000; i++)
            {
                method3();
            }
            watch.Stop();

            //获取当前实例測量得出的总执行时间（以毫秒为单位）
            string time = watch.ElapsedMilliseconds.ToString();
            MessageBox.Show(time);
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            object obj = Scaler.C.Get("CurTime");
            if (obj == null)
            {
                Scaler.C.Add("CurTime", obj = DateTime.Now.ToString(), "CurTime.txt");
            }
            MessageBox.Show(obj.ToString());
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            Scaler.MemShare.Dispose();
            MessageBox.Show("已关闭");
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(300, 100);
            Graphics g= Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.DrawString("交易趋势分析至少两个月！", new Font("微软雅黑", 18f), Brushes.Red, new PointF(0,0));
            g.Dispose();

            pictureBox1.Image = bitmap;
        }

        string AccessToken = string.Empty;
        DateTime dtExpire = DateTime.Now.AddHours(-1);
        string app_id = "18213796";
        string app_key = "p7mFr7hOlsWoVfrv8Bcun28i";
        string secret_key = "69pXY6gOxXWFf8kmL4fVI4U3Uk73rS7l";

        public string getAccessToken()
        {
            if (!string.IsNullOrEmpty(AccessToken) && DateTime.Now < dtExpire)
            {
                return AccessToken;
            }
            string url = string.Format("https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials&client_id={0}&client_secret={1}", app_key, secret_key);
            Scaler.Http http = new Scaler.Http();
            string content = http.GetHTML(url, "", "", "", "GET");
            Newtonsoft.Json.Linq.JObject obj;
            try
            {
                obj = Newtonsoft.Json.Linq.JObject.Parse(content);
            }
            catch (Exception err)
            {
                Scaler.Win.WriteLog("获取Access失败！|" + content + "|" + err.Message);
                return string.Empty;
            }
            /*{
                "access_token": "1.a6b7dbd428f731035f771b8d********.86400.1292922000-2346678-124328",
                "expires_in": 2592000,
                "refresh_token": "2.385d55f8615fdfd9edb7c4b********.604800.1293440400-2346678-124328",
                "scope": "public audio_tts_post ...",
                "session_key": "ANXxSNjwQDugf8615Onqeik********CdlLxn",
                "session_secret": "248APxvxjCZ0VEC********aK4oZExMB",
            }*/
            if (!obj.TryGetValue("access_token", out Newtonsoft.Json.Linq.JToken access_token))
            {
                Scaler.Win.WriteLog("获取Access失败！无access_token|" + content);
                return string.Empty;
            }
            AccessToken = access_token.ToString();
            if (obj.TryGetValue("expires_in", out access_token))
            {
                if (int.TryParse(access_token.ToString(), out int expires))
                {
                    dtExpire = DateTime.Now.AddHours(-1).AddSeconds(expires);
                }
            }
            return AccessToken;
        }

        public string Text2Audio(string text)
        {
            string access_token = getAccessToken();
            if (string.IsNullOrEmpty(access_token))
            {
                return "0获取AcessToken失败！";
            }
            string url = string.Format("http://tsn.baidu.com/text2audio?lan=zh&ctp=1&cuid=abcdxxx&tok={0}&tex={1}&vol=9&per=0&spd=5&pit=5&aue=3", access_token, text);
            return "1" + url;
        }
        static readonly Regex regHead = new Regex(@"<head(>|\s.*?>)",RegexOptions.Multiline | RegexOptions.Compiled);
        Regex regHead2 = new Regex(@"<head(>|\s.*?>)", RegexOptions.IgnoreCase);
        private void Button10_Click(object sender, EventArgs e)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < 100000; i++)
            {
                List<byte> ls = new List<byte>();
                ls.AddRange(BitConverter.GetBytes((uint)1));
                ls.AddRange(BitConverter.GetBytes((uint)5000));
                ls.AddRange(BitConverter.GetBytes((uint)1000));
                var b = ls.ToArray();
            }
            timer.Stop();
            MessageBox.Show(timer.ElapsedMilliseconds.ToString());

            timer.Restart();
            for (int i = 0; i < 100000; i++)
            {
                var c = BitConverter.GetBytes((uint)1)
                    .Concat(BitConverter.GetBytes((uint)5000))
                    .Concat(BitConverter.GetBytes((uint)1000))
                    .ToArray();
            }
                timer.Stop();
                MessageBox.Show(timer.ElapsedMilliseconds.ToString());

                return;



                string text = File.ReadAllText("main.html").ToLowerInvariant();
            for (int i = 0; i < 100000; i++)
            {
                Match m = regHead2.Match(text);
                if (m.Success) { }
            }
            timer.Stop();
            MessageBox.Show(timer.ElapsedMilliseconds.ToString());

            timer.Restart();
            text = File.ReadAllText("main.html").ToLowerInvariant();
            for (int i = 0; i < 100000; i++)
            {
                Match m = regHead.Match(text);
                if (m.Success) { }
            }
            timer.Stop();
            MessageBox.Show(timer.ElapsedMilliseconds.ToString());

            return;


            bool.TryParse(null, out bool bval);

            string url = Text2Audio("1号桌订餐红烧茄子").TrimStart(new char[] { '1' });

            axWindowsMediaPlayer1.stretchToFit = true; // 自动缩放。
            axWindowsMediaPlayer1.currentPlaylist.clear();//清除播放的内容
            axWindowsMediaPlayer1.currentPlaylist.appendItem(axWindowsMediaPlayer1.newMedia(url));//添加播放的地址
            axWindowsMediaPlayer1.Ctlcontrols.play();//立即播放


            //Mp3.Play(url);
            textBox4.Text = url;

        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            Scaler.MemShare.Set(textBox1.Text, textBox2.Text);
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            Scaler.MemShare.Remove(textBox1.Text);
        }

        private void Button12_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Scaler.MemShare.Get(textBox1.Text));
        }

        private void Button13_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(Scaler.MemShare.ClientCount.ToString());
            MessageBox.Show(Scaler.MemShare.ClinetsDump());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Scaler.MemShare.Dispose();
        }
    }
}
