﻿using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace tcp
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 58900);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(ipe);

            socket.Listen(1);

            Console.WriteLine("Done - Server started");
            Console.WriteLine(ipe);


            while (true)
            {
                Socket socket_ = socket.Accept();
                int bytes = 0;
                byte[] data = new byte[256];
                do
                {
                    bytes = socket_.Receive(data);
                    Console.WriteLine($"Info - Received mess from {((IPEndPoint)(socket_.RemoteEndPoint)).Address.ToString()}:" + Encoding.ASCII.GetString(data, 0, bytes));
                } while (bytes > 0);

                socket_.Shutdown(SocketShutdown.Both);
                socket_.Close();
            }
        }
    }
}