using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace SocketClient
{
    public class TcpServerSocket
    {
       // private int mServerPort = 2001;
        private int mServerPort = 3333;
        private int mOnceListen = 10;
        private int mThreadJoinTime = 1000;
        private int mPollSeconds = 1;
        public bool TachoSend1 = false;
        public bool TachoSend2 = false;
        public bool TachoSend3 = false;
        public bool TachoSend4 = false;
      

        //비동기 모드 : true / false
        private bool mAsyncMode = false;                                                           

        public static int mStreamBufferSize = 1024 * 10;
        public static int mTempStreamBufferSize = 1024;
        public static int mStreamBufferCurrentPoint = 0;
        public static int mStreamBufferCurrentSize = 0;
        public string TachoStr = "";

        private Socket mFdes = null;

        protected byte[] mStreamBuffer;

        private TcpClientSocketManager TcpClientList;

        private Thread mAccpetThread = null;
        private bool mIsAcceptStop = false;
        public int ClientCnt = 0;

        public Form1 mainform = null;

        public TcpServerSocket(bool bAsyncMode_)
        {
            mAsyncMode = bAsyncMode_;
            mFdes = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mStreamBuffer = new byte[mStreamBufferSize];
            TcpClientList = (TcpClientSocketManager)new TcpClientSocketManager();
        }

        ~TcpServerSocket()
        {
        }

       public void SetMainForm(Form1 f)
        {
            mainform = f;
        }

        //비동기 용
        public void SetAsyncMode(bool o)
        {
            mAsyncMode = o;
        }

        //비동기 용
        public bool GetAsyncMode()
        {
            return mAsyncMode;
        }

       
        //비동기 용
        public void AcceptStart()
        {
            if (mAsyncMode == true)
            {
                BeginAcceptClientManager();
            }
            else
            {
                AcceptThreadStart();
            }
        }

        public void AcceptThreadStart()
        {
            if (null == mAccpetThread)
            {
                mAccpetThread = new Thread(new ThreadStart(AcceptThread));
                mAccpetThread.IsBackground = true;
                mAccpetThread.Start();
            }
        }

    
        //비동기 용
        public void BeginAcceptClientManager()
        {
            mFdes.BeginAccept(new AsyncCallback(EndAcceptClient), mFdes);
        }

     
        //비동기 용
        public void EndAcceptClient(IAsyncResult ar)
        {
            Socket listenSocket = (Socket)ar.AsyncState;
            Socket cltSocket = listenSocket.EndAccept(ar);
            TcpClientSocket tcpClientSocket = (TcpClientSocket)new PacketSocket(cltSocket);
            tcpClientSocket.SetClientManager(TcpClientList);
            tcpClientSocket.SetServerSocket(this);
            tcpClientSocket.GetSocket().BeginReceive(tcpClientSocket.GetTempStreamBuffer(), 0, TcpClientSocket.mTempStreamBufferSize, SocketFlags.None, new AsyncCallback(EndRecvClient), tcpClientSocket);
          
            TcpClientList.Add(tcpClientSocket);

            ClientCnt = TcpClientList.GetCount();

            mFdes.BeginAccept(new AsyncCallback(EndAcceptClient), mFdes);
        }

        public void AcceptThreadEnd()
        {
            if (null != mAccpetThread)
            {
                mAccpetThread.Join(mThreadJoinTime);
                if (mAccpetThread.IsAlive)
                    mAccpetThread.Abort();

                mAccpetThread = null;
            }
        }

        public void AcceptThread()
        {
            while (true != mIsAcceptStop)
            {
                Socket Sock = mFdes.Accept();

                TcpClientSocket Client = new PacketSocket(Sock);
                Client.SetClientManager(TcpClientList);
                Client.SetServerSocket(this);

                TcpClientList.Add(Client);
            }
        }

        public void Poll()
        {
            lock (TcpClientList)
            {
                PacketSocket Client = null;
                for (int i = 0; i < TcpClientList.GetCount(); i++)
                {

                    Client = (PacketSocket)TcpClientList.Get(i);
                    if (Client.GetSocket() != null)
                    {
                        if (true == Client.GetSocket().Poll(mPollSeconds, SelectMode.SelectRead))
                        {
                            try
                            {
                                Client.RecvToStreamBuffer();
                            }
                            catch (SocketException e)
                            {
                                //연결이 끊겼다. 여기서 연결 끊김 처리를 해 주면 된다.
                                TcpClientList.Remove(Client);
                                ClientCnt = TcpClientList.GetCount();

                                Console.WriteLine("From {0} {1}", Client.GetSocket().Handle, e.Message);

                                Client.CloseSocket();
                                Client = null;

                                return;
                            }
                        }
                    }
                }
            }
        }

     
        //비동기 용
        public void BeginRecvClientManager()
        {
            lock (TcpClientList)
            {
                PacketSocket Client = null;
                for (int i = 0; i < TcpClientList.GetCount(); i++)
                {
                    Client = (PacketSocket)TcpClientList.Get(i);
                    if (Client.GetSocket() != null)
                    {
                        if (Client.IsAbleToRecv() == true)
                        {
                            Client.GetSocket().BeginReceive(Client.GetTempStreamBuffer(), 0, TcpClientSocket.mTempStreamBufferSize, SocketFlags.None, new AsyncCallback(EndRecvClient), Client);
                        }
                    }
                }
            }
        }

      
        //비동기 용
        public void EndRecvClient(IAsyncResult ar)
        {
            PacketSocket Client = (PacketSocket)ar.AsyncState;

            try
            {
                int size = Client.GetSocket().EndReceive(ar);

                //원격에서 연결을 종료했다.
                if (size <= 0)
                {
                    Console.WriteLine("From {0} : 호스트 연결이 끊겼습니다.", Client.GetSocket().Handle);
                    Client.GetSocket().Shutdown(SocketShutdown.Both);
                    Client.GetSocket().Close();
                    TcpClientList.Remove(Client);
                    ClientCnt = TcpClientList.GetCount();
                    mFdes.BeginAccept(new AsyncCallback(EndAcceptClient), mFdes);
                }
                else
                {
                    Client.RecvToStreamBufferForAsync(size);
                    Client.GetSocket().BeginReceive(Client.GetTempStreamBuffer(), 0, TcpClientSocket.mTempStreamBufferSize, SocketFlags.None, new AsyncCallback(EndRecvClient), Client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("From {0} : {1}", Client.GetSocket().Handle, e.Message.ToString());
                Console.WriteLine("From {0} : 호스트 연결이 끊겼습니다.", Client.GetSocket().Handle);
                Client.GetSocket().Shutdown(SocketShutdown.Both);
                Client.GetSocket().Close();
                TcpClientList.Remove(Client);
                ClientCnt = TcpClientList.GetCount();
                mFdes.BeginAccept(new AsyncCallback(EndAcceptClient), mFdes);
            }
        }

        public void ProcessPacket()
        {
            lock (TcpClientList)
            {
                for (int i = 0; i < TcpClientList.GetCount(); i++)
                {
                    PacketSocket Client = (PacketSocket)TcpClientList.Get(i);
                    if (Client != null)
                        Client.RunProcess();
                }
            }
        }

        public void TachoPacket()
        {
           
            lock (TcpClientList)
            {
              //  for (int i = 0; i < TcpClientList.GetCount(); i++)
              //  {
                    PacketSocket Client = (PacketSocket)TcpClientList.Get(0);
                    if (Client != null)
                    {
                        Client.Tachostr = TachoStr;
                    //    Client.RunTachoSend();
                      //  BroadCast
                    }
              //  }
           }
            
          /*  byte[] mProcessBuffer = Encoding.Default.GetBytes(TachoStr);
            lock (TcpClientList)
            {
               
                    for (int i = 0; i < TcpClientList.GetCount(); i++)
                    {
                        TcpClientSocket Client = TcpClientList.Get(i);
                        Client.Send(mProcessBuffer, mProcessBuffer.Length);
                    }
              
            }
            */ 

           
        }

   
        //비동기 용
        public void Update()
        {
            if (mAsyncMode == true)
            {
                // ProcessPacket();

                if (ClientCnt != 0)
                {
                    if (TachoSend1 == true)
                    {
                        TachoSend1 = false;
                        TachoPacket();
                    }
                    if (TachoSend2 == true)
                    {
                        TachoSend2 = false;
                        TachoPacket();
                    }
                    if (TachoSend3 == true)
                    {
                        TachoSend3 = false;
                        TachoPacket();
                    }
                    if (TachoSend4 == true)
                    {
                        TachoSend4 = false;
                        TachoPacket();
                    }

                }
            }
            else
            {
                Poll();
                //  ProcessPacket();
                if (ClientCnt != 0)
                {
                    if (TachoSend1 == true)
                    {
                        TachoSend1 = false;
                        TachoPacket();
                    }
                    if (TachoSend2 == true)
                    {
                        TachoSend2 = false;
                        TachoPacket();
                    }
                    if (TachoSend3 == true)
                    {
                        TachoSend3 = false;
                        TachoPacket();
                    }
                    if (TachoSend4 == true)
                    {
                        TachoSend4 = false;
                        TachoPacket();
                    }
                }
            }
        }

        public void SetSocket(Socket o)
        {
            mFdes = o;
        }

        public Socket GetSocket()
        {
            return mFdes;
        }

        public bool Connect(string host, int port)
        {
            mFdes.Connect(host, port);

            if (mFdes.Connected)
                return true;

            return false;
        }

        public void Bind()
        {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, mServerPort);
            mFdes.Bind(serverEndPoint);
        }

        public void Listen()
        {
            mFdes.Listen(mOnceListen);
        }

        public int Send(byte[] o, int size)
        {
            return mFdes.Send(o, size, SocketFlags.None);
        }

        public int Recv(byte[] o, int size)
        {
            return mFdes.Receive(o, size, SocketFlags.None);
        }

        public void rcvClientMessage(byte[] mTempStreamBuffer,int size)
        {
            //mStreamBuffer
          //  MessageBox.Show("rev");
        //  mainform.revClientMsg(mTempStreamBuffer, size);
        }


        public void BroadCast(byte[] o, int size)
        {
            lock (TcpClientList)
            {
                if (size > 0)
                {
                    for (int i = 0; i < TcpClientList.GetCount(); i++)
                    {
                        TcpClientSocket Client = TcpClientList.Get(i);
                        Client.Send(o, size);
                    }
                }
            }
        }
    }
}
