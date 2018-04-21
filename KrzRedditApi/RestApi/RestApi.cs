using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RestApi
{
    public abstract class RESTApi
    {
        
        private TcpListener server;

        public RESTApi(int port)
        {
            server = new TcpListener(IPAddress.Parse("0.0.0.0"), port);
            Console.WriteLine("Starting server: " + server.LocalEndpoint);
            server.Start();
        }

        public void Start()
        {
            Console.WriteLine("Server listening");
            while (true)
            {
                HandleRequest(server.AcceptTcpClient());
            }
        }


        private void HandleRequest(TcpClient client)
        {
            new Thread(() =>
            {
                try
                {
                    StreamReader sr = new StreamReader(client.GetStream(), Encoding.Default);
                    string content = ReadAllLinesWithPeek(sr);

                    Request request = ParseRequest(client, content);

                    if (request != null)
                    {
                        string answer = HandleRequest(request);

                        int respcode = (JsonConvert.DeserializeObject(answer) as dynamic).code;

                        Answer(client, respcode, answer);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    client.Close();
                }
            }).Start();
        }


        private Request ParseRequest(TcpClient client, string content)
        {
            Request request = null;
            foreach (var a in content.Split('\n'))
            {
                if (a.StartsWith("GET"))
                {
                    request = new Request()
                    {
                        Pfad = a.Split(' ')[1],
                        Type = RequestType.GET
                    };
                }
                else if (a.StartsWith("POST"))
                {
                    request = new Request()
                    {
                        Pfad = a.Split(' ')[1],
                        Type = RequestType.POST
                    };
                }
                else if (a.StartsWith("PUT"))
                {
                    request = new Request()
                    {
                        Pfad = a.Split(' ')[1],
                        Type = RequestType.PUT
                    };

                    request.FileName = ExtractFileName(content);
                    request.File = ExtractFileData(request, SplitLines(content));
                }
                else if (a.StartsWith("DELETE"))
                {
                    request = new Request()
                    {
                        Pfad = a.Split(' ')[1],
                        Type = RequestType.DELETE
                    };
                }
                else if (a.StartsWith("OPTIONS"))
                {
                    StreamWriter writer2 = new StreamWriter(client.GetStream());
                    writer2.Write($"HTTP/1.0 200 OK");
                    writer2.Write(Environment.NewLine);
                    writer2.Write("Content-Type: text/plain; charset=UTF-8" + Environment.NewLine);
                    writer2.Write("Access-Control-Allow-Origin: *" + Environment.NewLine);
                    writer2.Write("Access-Control-Allow-Methods: GET, POST, PUT, DELETE" + Environment.NewLine);
                    writer2.Write("Access-Control-Allow-Headers: content-type");
                    writer2.Write("");
                    writer2.Flush();
                    return null;
                }
            }
            if (request == null)
                return null;

            request.Content = content;
            request.Data = GetJsonData(request.Pfad, request.Content);
            return request;
        }

        private string ExtractFileName(string content)
        {
            foreach (string line in content.Split('\n'))
            {
                if (line.Contains("filename="))
                {
                    return line.Split('\"')[3];
                }
            }
            return null;
        }

        private string ExtractFileData(Request request, string[] lines)
        {
            StringBuilder data = new StringBuilder();
            bool inData = false;
            int skip = 0;
            foreach (string line in lines)
            {
                if (line.StartsWith("Content-Type: image/"))
                {
                    inData = true;
                    skip = 1;
                }
                if (line.StartsWith("-----"))
                    inData = false;
                if (skip > 0)
                    skip--;
                else if (inData)
                    data.Append(line);
            }
            return data.ToString();
        }
        
        private string[] SplitLines(string content)
        {
            List<StringBuilder> data = new List<StringBuilder>();
            data.Add(new StringBuilder());

            bool inLineEnding = false;
            char[] chars = content.ToCharArray();
            foreach (char c in content.ToCharArray())
            {
                if(c.Equals('\r') || c.Equals('\n'))
                {
                    inLineEnding = true;
                }
                else if (inLineEnding)
                {
                    inLineEnding = false;
                    data.Add(new StringBuilder());
                }
                data.Last().Append(c);
            }

            string[] result = new string[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                result[i] = data[i].ToString();
            }
            return result;
        }

        protected abstract string HandleRequest(Request request);

        private void Answer(TcpClient client, int respcode, string answer)
        {
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.Write($"HTTP/1.0 {respcode} {(respcode == 200 ? "OK" : "ERR")}");
            writer.Write(Environment.NewLine);
            writer.Write("Content-Type: text/plain; charset=UTF-8" + Environment.NewLine);
            writer.Write("Access-Control-Allow-Origin: *" + Environment.NewLine);
            writer.Write("Access-Control-Allow-Methods: POST, GET, DELETE, PUT" + Environment.NewLine);
            writer.Write("Access-Control-Allow-Headers: content-type");
            writer.Write(Environment.NewLine);
            writer.Write(Environment.NewLine);
            writer.Write(answer);
            writer.Flush();
        }


        private Dictionary<string, string> GetJsonData(string pfad, string content)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            string jsondata = "";
            if (pfad.Contains("?"))
            {
                foreach (var b in pfad.Substring(pfad.IndexOf('?') + 1).Split('&'))
                {
                    data.Add(b.Split('=')[0], b.Split('=')[1]);
                }
            }
            jsondata = content.Split(new string[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.None)[1];

            try
            {
                Dictionary<string, string> c = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsondata);
                if (c != null)
                {
                    foreach (var a in c)
                        data.Add(a.Key, a.Value);
                }
            }
            catch { }

            return data;
        }

        static string ReadAllLinesWithPeek(StreamReader sr)
        {
            string input = "";
            while (sr.Peek() >= 0)
            {
                input += (char)sr.Read();
            }
            return input;
        }
    }
}
