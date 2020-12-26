using System;
using System.IO;
using System.Text;
using System.Threading;
using StackExchange.Redis;

namespace Pub
{
    class Program
    {
        
        static void Main()
        {
            string path = Environment.CurrentDirectory + @"\Log\log" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            //创建连接
            ISubscriber sub = null;
            IDatabase db = null;
            ConfigurationOptions configurationOptions = new ConfigurationOptions { EndPoints = { "r-bp1o34m0xnofw53evopd.redis.rds.aliyuncs.com", "6379" }, KeepAlive = 60, Password = "K4ZGj5Qvsw5xWCOY3k3lcUai86", AllowAdmin = true };
            try
            {
                
                using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect("localhost:6379"))
                {
                    //ConnectionMultiplexer conn = ConnectionMultiplexer.Connect("localhost:6379");
                     sub = conn.GetSubscriber();
                     db = conn.GetDatabase();

                    
                    /*conn.ConfigurationChanged += (object sender, EndPointEventArgs e) =>
                    {
                        Console.WriteLine("配置更改");
                        string[] logmsg = { $"{conn.ClientName}配置更改[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ff")}]" + "Configuration changed: " + e.EndPoint };
                        //db.ExecuteAsync("sadd", $"LogServer{conn.ClientName}:ConfigurationChanged", logmsg);
                        File.AppendAllLines(path, logmsg, Encoding.Default);
                    };*/
                    conn.ConfigurationChangedBroadcast += (object sender, EndPointEventArgs e) =>
                    {
                        Console.WriteLine("通过发布订阅更新配置");
                        string[] logmsg = { $"{conn.ClientName}通过发布订阅更新配置[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ff")}]" + e.EndPoint };
                        //db.ExecuteAsync("sadd", $"LogServer{conn.ClientName}:ConfigurationChangedBroadcast", logmsg);
                        File.AppendAllLines(path, logmsg, Encoding.Default);


                    };
                    conn.ConnectionFailed += (object sender, ConnectionFailedEventArgs e) =>
                    {
                        Console.WriteLine("连接失败 ， 如果重新连接成功你将不会收到这个通知");
                        string[] logmsg = { $"{conn.ClientName}连接失败[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ff")}]" + "Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)) };

                        File.AppendAllLines(path, logmsg, Encoding.Default);

                        
                        //db.ExecuteAsync("sadd", $"LogServer{conn.ClientName}:ConnectionFailed", logmsg);

                    };
                    conn.ConnectionRestored += (object sender, ConnectionFailedEventArgs e) =>
                    {
                        Console.WriteLine("重连成功");
                        string[] logmsg = { $"{conn.ClientName}重连成功,重新建立连接之前的错误[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ff")}]" + "Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)) };
                        //db.ExecuteAsync("sadd", $"LogServer{conn.ClientName}:ConnectionRestored", logmsg);
                        File.AppendAllLines(path, logmsg, Encoding.Default);
                    };
                    conn.ErrorMessage += (object sender, RedisErrorEventArgs e) =>
                    {
                        Console.WriteLine("发生错误");
                        string[] logmsg = { $"{conn.ClientName}发生错误[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ff")}]" + "ErrorMessage: " + e.Message };
                        //db.ExecuteAsync("sadd", $"LogServer{conn.ClientName}:ErrorMessage", logmsg);
                        File.AppendAllLines(path, logmsg, Encoding.Default);
                    };
                    conn.HashSlotMoved += (object sender, HashSlotMovedEventArgs e) =>
                    {
                        Console.WriteLine("更改集群");
                        string[] logmsg = { $"{conn.ClientName}更改集群[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ff")}]" + "HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint };
                        //db.ExecuteAsync("sadd", $"LogServer{conn.ClientName}:HashSlotMoved", logmsg);
                        File.AppendAllLines(path, logmsg, Encoding.Default);
                    };
                    conn.InternalError += (object sender, InternalErrorEventArgs e) =>
                    {
                        Console.WriteLine("redis类库错误");
                        string[] logmsg = { $"{conn.ClientName}redis类库错误[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ff")}]" + "InternalError:Message" + e.Exception.Message };
                        //db.ExecuteAsync("sadd", $"LogServer{conn.ClientName}:InternalError", logmsg);
                        File.AppendAllLines(path, logmsg, Encoding.Default);
                        Main();
                    };
                    //Console.WriteLine("请输入任意字符，输入exit退出");

                    //string input;

                    //do
                    //{
                    // input = Console.ReadLine();
                    // sub.Publish("messages", input);
                    //} while (input != "exit");
                    Thread.Sleep(50);
                    while (true)
                    {
                        try
                        {
                            sub.Publish("messages", "111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111");
                        }
                        catch (Exception e)
                        {

                            Console.WriteLine("未知错误");


                            string[] logmsg = { $"{conn.ClientName}未知错误[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ff")}]{e.Message}" };
                            File.AppendAllLines(path, logmsg, Encoding.Default);
                            

                            Main();



                        }
                    }
                }
            }
            catch (Exception)
            {
                //string[] logmsg = { $"无法连接[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ff")}]" };

                //Console.WriteLine(""+logmsg[0]);
                //File.AppendAllLines(path, logmsg, Encoding.Default);
                Main();
            }
        }
		}
    }
