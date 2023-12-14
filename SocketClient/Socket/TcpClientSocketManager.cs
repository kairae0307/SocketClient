using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace SocketClient
{
    class TcpClientSocketManager
    {
        private int mClientInitailNum = 1000;

        List<TcpClientSocket> ClientList;

        public TcpClientSocketManager()
        {
            ClientList = new List<TcpClientSocket>(mClientInitailNum);
        }

        public void Add(TcpClientSocket o)
        {
            lock (ClientList)
            {
                ClientList.Add(o);
            }
        }

        public void Remove(TcpClientSocket o)
        {
            lock (ClientList)
            {
                ClientList.Remove(o);
            }
        }

        public TcpClientSocket Get(int o)
        {
            lock (ClientList)
            {
                return (TcpClientSocket)ClientList[o];
            }
        }

        public int GetCount()
        {
            lock (ClientList)
            {
                return ClientList.Count;
            }
        }

    }
}
