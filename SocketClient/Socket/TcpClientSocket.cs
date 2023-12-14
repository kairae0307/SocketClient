using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketClient
{
    class TcpClientSocket
    {
        public static int mStreamBufferSize = 1024 * 10;
        public static int mTempStreamBufferSize = 1024;
   
        protected int mStreamBufferCurrentPoint = 0;
        protected int mStreamBufferCurrentSize = 0;

        protected Socket mClient = null;
        protected TcpServerSocket mServer = null;
        protected TcpClientSocketManager mClientMgr = null;

        protected byte[] mStreamBuffer = null;
        public byte[] mTempStreamBuffer = null;
        Form1 form1;

        public TcpClientSocket()
        {
            mClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mStreamBuffer = new byte[mStreamBufferSize];
            mTempStreamBuffer = new byte[mTempStreamBufferSize];
        //    form1 = f;
        }

        public void setMainform(Form1 f)
        {
            form1 = f;
        }

        public TcpClientSocket(Socket o)
        {
            mClient = o;
            mStreamBuffer = new byte[mStreamBufferSize];
            mTempStreamBuffer = new byte[mTempStreamBufferSize];
        }

        ~TcpClientSocket()
        {
        }

        public void SetClientManager(TcpClientSocketManager o)
        {
            mClientMgr = o;
        }

        public TcpClientSocketManager GetClientManager()
        {
            return mClientMgr;
        }

        public void SetSocket(Socket o)
        {
            mClient = o;
        }

        public TcpServerSocket GetServerSocket()
        {
            return mServer;
        }

        public void SetServerSocket(TcpServerSocket o)
        {
            mServer = o;
        }

        public Socket GetSocket()
        {
            return mClient;
        }

        public void SetTempStreamBuffer(byte[] o)
        {
            mTempStreamBuffer = o;
        }

        public byte[] GetTempStreamBuffer()
        {
            return mTempStreamBuffer;
        }

        public bool Connect(string host, int port)
        {
            if(mClient ==null)
            {
                mClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mStreamBuffer = new byte[mStreamBufferSize];
                mTempStreamBuffer = new byte[mTempStreamBufferSize];
            }
            mClient.Connect(host, port);

            if (mClient.Connected)
                return true;

            return false;
        }

        public void CloseSocket()
        {
            try
            {
                mClient.Close();
                mClient = null;
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
        }

        public int Send(byte[] o, int size)
        {
            try
            {
                return mClient.Send(o, size, SocketFlags.None);
            }
            catch (Exception e)
            {
                Console.WriteLine("From {0} : {1}", mClient.Handle, e.Message.ToString());
                return -1;
            }
        }

        public IAsyncResult BeginSendClient(byte[] o, int size)
        {
            try
            {
                return mClient.BeginSend(o, 0, size, SocketFlags.None, new AsyncCallback(EndSendClient), this);
            }
            catch (Exception e)
            {
                Console.WriteLine("From {0} : {1}", mClient.Handle, e.Message.ToString());
                return null;
            }
        }

        public void EndSendClient(IAsyncResult ar)
        {
        }

        public void StartReceive()
        {
            mClient.BeginReceive(mStreamBuffer, 0, mStreamBuffer.Length, SocketFlags.None, new AsyncCallback(CallBack_ReceiveMsg), mStreamBuffer);
        }
        public void CallBack_ReceiveMsg(IAsyncResult ar)
        {
            try
            {
                if (mClient != null)
                {

                    if (mClient.Connected == true)
                    {
                       int intSize = mClient.EndReceive(ar);
                        string strReceiveMsg = string.Empty;
                        strReceiveMsg = Encoding.Default.GetString(mStreamBuffer, 0, intSize);
                        mClient.BeginReceive(mStreamBuffer, 0, mStreamBuffer.Length, SocketFlags.None, new AsyncCallback(CallBack_ReceiveMsg), mStreamBuffer);
                     
                        form1.receiveMessage(strReceiveMsg);




                    }


                   
                }





            }
            catch (Exception err)
            {
                string strReceiveMsg = "서버가 종료 되었습니다.\n";
                CloseSocket();
                form1.receiveMessage(strReceiveMsg);
            }
        }


         public int Recv(byte[] o, int size)
        {
            return mClient.Receive(o, size, SocketFlags.None);
        }

        public void BroadCast(byte[] o, int size)
        {
            mServer.BroadCast(o, size);
        }

        public bool IsAbleToRecv()
        {
            if (mTempStreamBufferSize <= (mStreamBufferSize - mStreamBufferCurrentSize))
                return true;
          
            return false;
        }

        public int RecvToStreamBuffer()
        {
            if (mClient.Connected)
            {
                // mTempStreamBufferSize만큼의 여유 공간이 있을때만 Receive해 준다.
                if (IsAbleToRecv() == true)
                {
                    int nSize = mClient.Receive(mTempStreamBuffer, 0, mTempStreamBufferSize, SocketFlags.None);
                    for (int i = 0; i < nSize; i++)
                    {
                        mStreamBuffer[mStreamBufferCurrentPoint] = mTempStreamBuffer[i];
                        mStreamBufferCurrentPoint++;

                        if (mStreamBufferSize <= mStreamBufferCurrentPoint)
                            mStreamBufferCurrentPoint = 0;
                    }
                    mStreamBufferCurrentPoint = 0;
                    mStreamBufferCurrentSize += nSize;
                    return nSize;
                }
            }

            return -1;
        }

       
        //비동기 용
        public int RecvToStreamBufferForAsync(int nSize)
        {
            if (mClient.Connected)
            {
                for (int i = 0; i < nSize; i++)
                {
                    mStreamBuffer[mStreamBufferCurrentPoint] = mTempStreamBuffer[i];
                    mStreamBufferCurrentPoint++;

                    if (mStreamBufferSize <= mStreamBufferCurrentPoint)
                        mStreamBufferCurrentPoint = 0;
                }
                mStreamBufferCurrentPoint = 0;
                mStreamBufferCurrentSize += nSize;


              
                mServer.rcvClientMessage(mTempStreamBuffer,mStreamBufferCurrentSize);
                return nSize;
            }

            return -1;
        }
    }
}
    
