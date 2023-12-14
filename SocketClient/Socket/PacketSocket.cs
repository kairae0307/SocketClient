using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace SocketClient
{
    class PacketSocket : TcpClientSocket
    {
        protected int mStreamBufferProcessPoint = 0;
        protected const int mHeaderSizeSize = 2;
        protected const int mHeaderTypeSize = 2;
        protected byte[] mProcessBuffer;
        public string Tachostr = "";

        public PacketSocket()
        {
            mProcessBuffer = new byte[mTempStreamBufferSize];
        }

        public PacketSocket(Socket o)
        {
            mClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mStreamBuffer = new byte[mStreamBufferSize];
            mProcessBuffer = new byte[mTempStreamBufferSize];
            mClient = o;
        }

        public int IsAbleToProcess()
        {
            int size1, size2, size;

            if ((mHeaderSizeSize + mHeaderTypeSize) <= mStreamBufferCurrentSize)
            {
                if (mStreamBufferSize <= mStreamBufferProcessPoint)
                {
                    size1 = mStreamBuffer[0];
                    size2 = mStreamBuffer[1];
                }
                else
                {
                    size1 = mStreamBuffer[mStreamBufferProcessPoint];

                    if (mStreamBufferSize <= (mStreamBufferProcessPoint + 1))
                        size2 = mStreamBuffer[0];
                    else
                        size2 = mStreamBuffer[mStreamBufferProcessPoint + 1];
                }
                
                size = (size2 * 256) + size1;

                if (size <= mStreamBufferCurrentSize)
                    return size;
                else
                    return size;
                  //  return -1;
            }

            return -1;
        }
        int testid = 0;
        public void RunProcess()
        {
            int i, size, type1, type2, type;
            
            size = IsAbleToProcess();
            if (-1 == size)
                return;

            //프로세스 버퍼의 청소
            for (i = 0; i < mProcessBuffer.Length; i++)
            {
                mProcessBuffer[i] = 0;
            }

            for(i = 0; i < size; i++)
            {
                if (mStreamBufferSize <= mStreamBufferProcessPoint)
                    mStreamBufferProcessPoint = 0;

                mProcessBuffer[i] = mStreamBuffer[mStreamBufferProcessPoint];
                mStreamBufferProcessPoint++;
            }
            mStreamBufferCurrentSize -= size;

            type1 = mProcessBuffer[mHeaderSizeSize];
            type2 = mProcessBuffer[mHeaderSizeSize + 1];

            type = (type2 * 256) + type1;

            
           
            /*
            //타입에 따른 패킷 처리를 해 준다.
            switch((ePt_Type)type)
            {
                case ePt_Type.PT_GENERAL_ACK:
                    PacketHandler.Handle_GeneralAck(mProcessBuffer, this);
                    break;
                case ePt_Type.PT_CHANGE_DOOR:
                    PacketHandler.Handle_ChangeDoor(mProcessBuffer, this);
                    break;
                case ePt_Type.PT_CHAT_MSG:
                

                 
                   
                  
                    PacketHandler.Handle_ChatMsg(mProcessBuffer, this);
                    break;
                case ePt_Type.PT_LOGIN:
                    PacketHandler.Handle_LogIn(mProcessBuffer, this);
                    break;
                default:
                    break;
            }*/
        }

        public void RunTachoSend()
        {

            byte[] mProcessBuffer = Encoding.Default.GetBytes(Tachostr);

          /*  mProcessBuffer[0] = 0x02;
            mProcessBuffer[1] = 0x01;
            mProcessBuffer[2] = 0x00;
            mProcessBuffer[3] = 0x41;*/
              
           //    PacketHandler.Handle_ChatMsg(mProcessBuffer, this);
             
        }

    }
}
