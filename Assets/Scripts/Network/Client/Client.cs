using System;
using System.Net.Sockets;
using UnityEngine;

namespace Kirara.Network
{
    public static class Client
    {
        public static readonly NetMsgProcessor processor = new();

        public static Session Connect(string host, int port, Action connectionRefused = null)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.ConnectAsync(host, port);
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionRefused)
            {
                Debug.LogWarning("连接被拒绝");
                connectionRefused?.Invoke();
                socket.Dispose();
                return null;
            }

            var session = new Session(socket, processor);
            _ = session.ReceiveAsync();
            return session;
        }
    }
}