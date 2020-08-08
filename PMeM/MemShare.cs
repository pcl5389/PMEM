using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PMeM
{
    public static class MemShare
    {
        const int BUFFER_LEN = 256, MAX_FAIL_COUNT = 3;
        static TcpListener server;
        static ConcurrentDictionary<string, string> Data = new ConcurrentDictionary<string, string>();
        static int is_client = -1;
        static FileStream objFileStream = null;
        static bool bClose = false;
        static NewClient[] clients = new NewClient[50];

        public static int MemSharePort = 848;
        public static IPAddress MemShareIP;
        static MemShare()
        {
            int.TryParse(GetStringSetting("MemSharePort", MemSharePort.ToString()), out MemSharePort);
            if (!IPAddress.TryParse(GetStringSetting("MemShareIP", "127.0.0.1"), out MemShareIP))
                MemShareIP = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(new IPEndPoint(IPAddress.Any, MemSharePort));
        }
        public static int ClientCount
        {
            get
            {
                for (int i = 0; i < clients.Length; i++)
                {
                    if (clients[i] == null)
                        return i;
                }
                return clients.Length;
            }
        }
        public static string Dump()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < clients.Length; i++)
            {
                if (clients[i] != null)
                    sb.AppendLine(i.ToString() + ":" + clients[i].iUsering);
            }
            return sb.ToString();
        }
        public static string Get(string _key, int ifail = 0)
        {
            if (ifail > MAX_FAIL_COUNT) return string.Empty;
            if (!IsClient)
            {
                Data.TryGetValue(_key.Trim(), out string _value);
                return _value;
            }
            else
            {
                int iconn = GetClient();
                NewClient ct = clients[iconn];
                try
                {
                    if (ct.tcp == null || !ct.tcp.Connected)
                    {
                        ct.tcp = new TcpClient();
                        ct.tcp.Connect(MemShareIP, MemSharePort);
                    }
                }
                catch
                {
                    is_client = -1;
                    ReleaseClient(iconn);
                }
                if (is_client == -1) //连接失败需要重新初始化
                {
                    Init();
                    return Get(_key, ++ifail);
                }
                byte[] buffer = new byte[BUFFER_LEN];
                ct.tcp.Client.ReceiveTimeout = 100;
                int icount = 0;
                try
                {
                    ct.tcp.Client.Send(Encoding.UTF8.GetBytes(string.Format("{0}&{1}&{2}", MyEncode(_key), "", "G")));
                    icount = ct.tcp.Client.Receive(buffer, 0, BUFFER_LEN, SocketFlags.None);
                }
                catch
                {
                    closeConn(ref ct.tcp);
                    is_client = -1;
                }
                finally
                {
                    ReleaseClient(iconn);
                }
                if (is_client == -1) //通讯失败需要重新初始化
                {
                    Init();
                    return Get(_key, ++ifail);
                }
                if (icount > 0)
                    return Encoding.UTF8.GetString(buffer, 0, icount);
                return string.Empty;
            }
        }
        public static bool Set(string _key, string _value, int ifail = 0)
        {
            if (ifail > MAX_FAIL_COUNT) return false;
            if (!IsClient)
            {
                Data[_key.Trim()] = _value;
                return true;
            }
            else
            {
                int iconn = GetClient();
                NewClient ct = clients[iconn];
                try
                {
                    if (ct.tcp == null || !ct.tcp.Connected)
                    {
                        ct.tcp = new TcpClient();
                        ct.tcp.Connect(MemShareIP, MemSharePort);
                    }
                }
                catch
                {
                    is_client = -1;
                    ReleaseClient(iconn);
                }
                if (is_client == -1) //连接失败需要重新初始化
                {
                    Init();
                    return Set(_key, _value, ++ifail);
                }
                byte[] buffer = new byte[BUFFER_LEN];
                ct.tcp.Client.ReceiveTimeout = 100;
                int icount = 0;
                try
                {
                    ct.tcp.Client.Send(Encoding.UTF8.GetBytes(string.Format("{0}&{1}&{2}", MyEncode(_key), MyEncode(_value), "S")));
                    icount = ct.tcp.Client.Receive(buffer, 0, BUFFER_LEN, SocketFlags.None);
                }
                catch
                {
                    closeConn(ref ct.tcp);
                    is_client = -1;
                }
                finally
                {
                    ReleaseClient(iconn);
                }
                if (is_client == -1) //通讯失败需要重新初始化
                {
                    Init();
                    return Set(_key, _value, ++ifail);
                }
                if (icount > 0)
                    return Encoding.UTF8.GetString(buffer, 0, icount) == "true";
                return false;
            }
        }
        public static bool Remove(string _key, int ifail = 0)
        {
            if (ifail > MAX_FAIL_COUNT) return false;
            if (!IsClient)
            {
                return Data.TryRemove(_key, out _);
            }
            else
            {
                int iconn = GetClient();
                NewClient ct = clients[iconn];
                try
                {
                    if (ct.tcp == null || !ct.tcp.Connected)
                    {
                        ct.tcp = new TcpClient();
                        ct.tcp.Connect(MemShareIP, MemSharePort);
                    }
                }
                catch
                {
                    is_client = -1;
                    ReleaseClient(iconn);
                }
                if (is_client == -1) //连接失败需要重新初始化
                {
                    Init();
                    return Remove(_key, ++ifail);
                }
                byte[] buffer = new byte[BUFFER_LEN];
                ct.tcp.Client.ReceiveTimeout = 100;
                int icount = 0;
                try
                {
                    ct.tcp.Client.Send(Encoding.UTF8.GetBytes(string.Format("{0}&{1}&{2}", MyEncode(_key), "", "R")));
                    icount = ct.tcp.Client.Receive(buffer, 0, BUFFER_LEN, SocketFlags.None);
                }
                catch
                {
                    closeConn(ref ct.tcp);
                    is_client = -1;
                }
                finally
                {
                    ReleaseClient(iconn);
                }
                if (is_client == -1) //通讯失败需要重新初始化
                {
                    Init();
                    return Remove(_key, ++ifail);
                }
                if (icount > 0)
                    return Encoding.UTF8.GetString(buffer, 0, icount) == "true";
                return false;
            }
        }
        public static bool Check(string _key, string _value, int ifail = 0)
        {
            if (ifail > MAX_FAIL_COUNT)
                return true;
            if (!IsClient)
            {
                Data.TryGetValue(_key.Trim(), out string value);
                if (string.IsNullOrEmpty(value))
                {
                    Data[_key.Trim()] = _value;
                    return true;
                }
                return _value == value;
            }
            else
            {
                int iconn = GetClient();
                NewClient ct = clients[iconn];
                try
                {
                    if (ct.tcp == null || !ct.tcp.Connected)
                    {
                        ct.tcp = new TcpClient();
                        ct.tcp.Connect(MemShareIP, MemSharePort);
                    }
                }
                catch
                {
                    is_client = -1;
                    ReleaseClient(iconn);
                }
                if (is_client == -1) //连接失败需要重新初始化
                {
                    Init();
                    return Check(_key, _value, ++ifail);
                }
                byte[] buffer = new byte[BUFFER_LEN];
                ct.tcp.Client.ReceiveTimeout = 1000;
                int icount = 0;
                try
                {
                    ct.tcp.Client.Send(Encoding.UTF8.GetBytes(string.Format("{0}&{1}&{2}", MyEncode(_key), MyEncode(_value), "C")));
                    icount = ct.tcp.Client.Receive(buffer, 0, BUFFER_LEN, SocketFlags.None);
                }
                catch
                {
                    closeConn(ref ct.tcp);
                    is_client = -1;
                }
                finally
                {
                    ReleaseClient(iconn);
                }
                if (is_client == -1) //通讯失败需要重新初始化
                {
                    Init();
                    return Check(_key, _value, ++ifail);
                }
                if (icount > 0)
                    return Encoding.UTF8.GetString(buffer, 0, icount) == "true";
                return false;
            }
        }
        static void closeConn(ref TcpClient ct)
        {
            if (ct == null)
                return;
            if (ct.Connected)
            {
                try
                {
                    ct.Client.Shutdown(SocketShutdown.Both);
                }
                catch { }
            }
            try
            {
                ct.Client.Close();
            }
            catch { }
            ct = null;
        }
        static int intconn = 0;
        static int GetClient()
        {
            if (intconn > 0)
            {
                for (int i = 0; i < intconn; i++)
                {
                    if (Interlocked.CompareExchange(ref clients[i].iUsering, 1, 0).Equals(0))
                    {
                        return i;
                    }
                }
            }
            if (intconn >= clients.Length)
            {
                Thread.Sleep(1);
                return GetClient();
            }
            int iUse = Interlocked.Increment(ref intconn);
            iUse = iUse - 1;
            clients[iUse] = new NewClient();
            return iUse;
        }
        static void ReleaseClient(int i)
        {
            Interlocked.Exchange(ref clients[i].iUsering, 0);
        }
        public new static string ToString()
        {
            StringBuilder sb = new StringBuilder();

            string[] all = new string[Data.Keys.Count + 512];
            Data.Keys.CopyTo(all, 0);

            foreach (string key in all)
            {
                if (key == null || string.IsNullOrEmpty(key.Trim()))
                    continue;
                sb.AppendFormat("{0}\t{1}", key, Data[key.Trim()]).AppendLine();
            }
            return sb.ToString();
        }


        static void Deal(IAsyncResult ir)
        {
            if (ir.AsyncState is NewClient ct)
            {
                try
                {
                    int icount = ct.tcp.GetStream().EndRead(ir);
                    if (icount > 0)
                    {
                        string rsv = Encoding.UTF8.GetString(ct.buffer, 0, icount);
                        string[] tmp = rsv.Split('&');
                        string ret = string.Empty;
                        if (tmp.Length > 2)
                        {
                            switch (tmp[2])
                            {
                                case "G":
                                    ret = Get(MyDecode(tmp[0]));
                                    break;
                                case "S":
                                    ret = Set(MyDecode(tmp[0]), MyDecode(tmp[1])).ToString().ToLowerInvariant();
                                    break;
                                case "R":
                                    ret = Remove(MyDecode(tmp[0])).ToString().ToLowerInvariant();
                                    break;
                                case "C":
                                    ret = Check(MyDecode(tmp[0]), MyDecode(tmp[1])).ToString().ToLowerInvariant();
                                    break;
                            }
                        }
                        if (!string.IsNullOrEmpty(ret))
                        {
                            byte[] rets = Encoding.UTF8.GetBytes(ret);
                            ct.tcp.GetStream().Write(rets, 0, rets.Length);
                            ct.tcp.GetStream().BeginRead(ct.buffer, 0, ct.buffer.Length, new AsyncCallback(Deal), ct);
                        }
                        else
                        {
                            throw new Exception("无效数据格式");
                        }
                    }
                }
                catch
                {
                    closeConn(ref ct.tcp);
                    ct.buffer = null;
                }
            }
        }
        public static void Dispose()
        {
            if (!IsClient)
            {
                bClose = true;
                Thread.Sleep(5);
                server.Stop();
                server = null;
            }
            else
            {
                foreach (NewClient client in clients)
                {
                    closeConn(ref client.tcp);
                    client.buffer = null;
                }
            }
        }
        static bool IsClient
        {
            get
            {
                if (is_client == -1)
                    Init();
                return is_client == 1;
            }
        }
        static void Init()
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"App_Data\Cache\"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"App_Data\Cache\");
            }
            bool bInit = true;
            if (is_client == -1)
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\Cache\MeMShare.txt";
                try
                {
                    objFileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                }
                catch { }
                if (objFileStream != null) //启动监听
                {
                    try
                    {
                        server.Start();
                        is_client = 0;
                        bInit = false;
                    }
                    catch
                    {
                        if (HttpContext.Current != null)
                        {
                            HttpContext.Current.Response.Write("<center style='color:red'>共享内存模块启动失败！检查端口号 " + MemSharePort.ToString() + " 是否被占用！<br>可修改web.config 配置文件参数 <add key=\"MemSharePort\" value=\"" + MemSharePort.ToString() + "\"/> </center>");
                            HttpContext.Current.Response.End();
                        }
                        else
                        {
                            throw new Exception("<center style='color:red'>共享内存模块启动失败！检查端口号 " + MemSharePort.ToString() + " 是否被占用！<br>可修改web.config 配置文件参数 <add key=\"MemSharePort\" value=\"" + MemSharePort.ToString() + "\"/> </center>");
                        }
                    }
                }
                else
                {
                    is_client = 1;
                }
            }
            if (!bInit)
            {
                new Task(() =>
                {
                    while (!bClose)
                    {
                        TcpClient ct = server.AcceptTcpClient();
                        ct.GetStream().ReadTimeout = 2000;
                        NewClient _ct = new NewClient(ct);
                        ct.GetStream().BeginRead(_ct.buffer, 0, _ct.buffer.Length, new AsyncCallback(Deal), _ct);
                    }
                }).Start();
            }
        }
        static string GetStringSetting(string name, string val)
        {
            return ConfigurationManager.AppSettings[name] == null ? val : ConfigurationManager.AppSettings[name].Trim();
        }
        static string MyEncode(string str)
        {
            return HttpUtility.UrlEncode(str, Encoding.UTF8);
        }
        static string MyDecode(string str)
        {
            return HttpUtility.UrlDecode(str, Encoding.UTF8);
        }
    }
    public class NewClient
    {
        public TcpClient tcp = null;
        public byte[] buffer = new byte[256];
        public int iUsering = 1;
        public NewClient() { }
        public NewClient(TcpClient _tcp)
        {
            tcp = _tcp;
        }
    }
}