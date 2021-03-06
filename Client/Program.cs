﻿using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace tcp
{
    class Message
    {
        public string type;
        public double value;
        public string id;
        public Message(string type, double value, string id)
        {
            this.type = type;
            this.id = id;
            this.value = value;
        }
         
    }

    class Result
    {
        public string id;
        public double value;
        public string type;
        public double answer;
    }

    class Program
    {
        static int port = 8005; // порт сервера
        static string address = "127.0.0.1"; // адрес сервера

        static public void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("Select Math orepation");
            Console.WriteLine();
            Console.WriteLine("1. Factorial");
            Console.WriteLine("2. SumOfRow");
            Console.WriteLine("3. Sqr");
            Console.WriteLine("4. To2");
            Console.WriteLine("5. Log");
            Console.WriteLine("6. GetResult");
        }
        static public double GetNumber(string title)
        {
            while (true)
            {
                Console.WriteLine(title);

                double Num;
                string str = Console.ReadLine();

                bool isNum = double.TryParse(str, out Num);

                if (isNum)

                {
                    return Num;

                }

            }

        }
        static public Message Menu()
        {
            string myid = "Client1";
            while (true)
            {
                DisplayMenu();
                Console.WriteLine();
                double answer = GetNumber("Select action");
                switch (answer)
                {
                    case 1:
                        Console.Clear();
                        double value = GetNumber("Select factorial value:");
                        return new Message("Factorial", value, myid);
                    case 2:
                        Console.Clear();
                        double value2 = GetNumber("Select SumOfRow value:");
                        return new Message("SumOfRow", value2, myid);
                    case 3:
                        Console.Clear();
                        double value3 = GetNumber("Select Sqr value:");
                        return new Message("Sqr", value3, myid);
                    case 4:
                        Console.Clear();
                        double value4 = GetNumber("Select To2 value:");
                        return new Message("To2", value4, myid);
                    case 5:
                        Console.Clear();
                        double value5 = GetNumber("Select Log value:");
                        return new Message("Log", value5, myid);
                    case 6:
                        Console.Clear();
                        return new Message("GetResult", 0, myid);
                    default:
                        break;
                }
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Press some button");
            Console.ReadLine();
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(address), port);
            try
            {
                while (true)
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(ipe);
                    if (!socket.Connected)
                    {
                        Console.WriteLine("Fail - Connection failed");
                        return;
                    }
                    Console.WriteLine("Connected!");
                    Message newMessage = Menu();
                    string json = JsonConvert.SerializeObject(newMessage);
                    byte[] data = Encoding.Unicode.GetBytes(json);
                    socket.Send(data);
 

                    // получаем ответ
                    data = new byte[256]; // буфер для ответа
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байт

                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);

                    if (newMessage.type == "GetResult")
                    {
                        var result = JsonConvert.DeserializeObject<List<Result>>(builder.ToString());
                        result.ForEach(el =>
                        {
                            Console.WriteLine();
                            Console.WriteLine("operation: " + el.type);
                            Console.WriteLine("value: " + el.value);
                            Console.WriteLine("answer: " + el.answer);
                        });
                    }
                    else
                    {
                        Console.WriteLine("server answer: " + builder.ToString());
                    }

                    // закрываем сокет
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    Console.Read();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
