using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace simpleServer
{
    class Program
    {
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static int maxSize = 1024 * 1024 * 2; //2M
        static Encoding encoding = Encoding.GetEncoding("utf-8");
        static Dictionary<string, string> mimeMapping;
        static string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        static int port = 8081;

        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
                port = Convert.ToInt32(args[0]);
            mimeMapping = File.ReadAllText(Path.Combine(baseDir,"mimeMapping.ini"))
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(row => row.Split(':', StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(t => t[0].Trim().Trim('"'), t => t[1].Trim().Trim('"'), StringComparer.OrdinalIgnoreCase);
            //绑定监听
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(10);
            Console.WriteLine($"http://localhost:{port}");
            while (true)
            {
                var s = socket.Accept();
                new Thread(() => ProcessRequest(s)).Start();
            }
        }
        static void ProcessRequest(Socket s)
        {
            try
            {
                var revBuffer = new byte[maxSize];  
                var realLength = s.Receive(revBuffer);
                var requestContent = encoding.GetString(revBuffer, 0, realLength);
                var request = new HttpRequest(requestContent);
                var filePath = request.Url.TrimStart('/').Replace("/", "\\");
                if (!File.Exists(filePath))
                {
                    s.Send(new HttpResponse(404, encoding, "404 Not Found").GetResponse());
                    s.Close();
                    return;
                }
                var mineType = "application/octet-stream";
                mimeMapping.TryGetValue(Path.GetExtension(filePath), out mineType);
                var msg = new HttpResponse(200, mineType, encoding, File.ReadAllBytes(filePath)).GetResponse();
                s.Send(msg);
                s.Close();
            }
            catch
            {
                s.Send(new HttpResponse(404, encoding, "500 Server Error").GetResponse());
                s.Close();
            }

        }
    }
}
