using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SocketClient
{
    public partial class Form1 : Form
    {
        private Socket client = null;
        public string ServerIP = "192.168.0.17";
        public int intPortNum = 3333;
        private byte[] byteReceiveMsg = new byte[1024];
        private byte[] byteSendMsg = new byte[1024];
        private bool bConnectServer = false;
        private int intSize = 0;
        private string strReceiveMsg = "";
        delegate void textCallbak(String txt);
         TcpClientSocket clientSocket;
        public int test = 0;

        public int test234 = 0;


        public Form1()
        {
            InitializeComponent();
            clientSocket = new TcpClientSocket();
            clientSocket.setMainform(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {

         

             ServerIP = textBox1.Text;
            intPortNum = Int32.Parse(textBox2.Text);

            try
            {
                if (clientSocket.Connect(ServerIP, intPortNum))
                {
                    AppendText(string.Format("[SYSTEM] : 서버 접속 성공!\n"));
                    //  Add_Log(strLog);
                    bConnectServer = true;
                    clientSocket.StartReceive();


                    //  clientSocket.BeginSendClient(byteSendMsg, 0);

                    //  byteSendMsg[0] = 1;

                    // 접속 메세지 전송
                    //  client.BeginSend(byteSendMsg, 0, byteSendMsg.Length, SocketFlags.None, new AsyncCallback(CallBack_SendMsg), client);
                    //이제 메시지를 전송받는다.
                    // client.BeginReceive(byteReceiveMsg, 0, byteReceiveMsg.Length, SocketFlags.None, new AsyncCallback(CallBack_ReceiveMsg), byteReceiveMsg);
                    //ReceiveStart();
                    // break;
                }
            }
            catch(Exception ex)
            {
                // MessageBox.Show(ex.Message);
                AppendText(ex.Message + "\n");
            }
     



           /* if ( clientSocket.Connect(ServerIP,intPortNum))
            {
                AppendText(string.Format("[SYSTEM] : 서버 접속 성공!\n"));
            }
           else
            {
                AppendText(string.Format("[SYSTEM] : 서버 접속 실패!\n"));
            }*/
            /*
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            IPAddress ipServer = IPAddress.Parse(ServerIP);


            while (true)
            {
                try
                {
                    client.Connect(new IPEndPoint(ipServer, intPortNum));


                    if (client.Connected)
                    {
                        AppendText(string.Format("[SYSTEM] : 서버 접속 성공!\n"));
                        //  Add_Log(strLog);
                        bConnectServer = true;

                       


                        byteSendMsg[0] = 1;

                        // 접속 메세지 전송
                      //  client.BeginSend(byteSendMsg, 0, byteSendMsg.Length, SocketFlags.None, new AsyncCallback(CallBack_SendMsg), client);
                        //이제 메시지를 전송받는다.
                        client.BeginReceive(byteReceiveMsg, 0, byteReceiveMsg.Length, SocketFlags.None, new AsyncCallback(CallBack_ReceiveMsg), byteReceiveMsg);
                        //ReceiveStart();
                        break;
                    }
                }
                catch (Exception err)
                {
                    AppendText(string.Format("[SYSTEM] : {0}", err.Message));
                    //   Add_Log(strErr);
                    //   MessageBox.Show(err.Message);
                    bConnectServer = false;
                }
            }
         */
        }
        private void CallBack_SendMsg(IAsyncResult ar)
        {
           /* client = (Socket)ar.AsyncState;

            try
            {
                intSize = client.EndSend(ar);
                if (intSize == 0)
                {
                    return;
                    Disconnet();
                }
                else
                {
                    AppendText("[보낸 메세지] :" + Encoding.Default.GetString(byteSendMsg, 0, intSize)+"\n");
                    client.BeginReceive(byteReceiveMsg, 0, byteReceiveMsg.Length, SocketFlags.None, new AsyncCallback(CallBack_ReceiveMsg), byteReceiveMsg);
                }
            }
            catch (Exception err)
            {
                AppendText( string.Format("[SYSTEM] : {0}", err.Message));
                //   MessageBox.Show(err.Message);
                //    Add_Log(strErr);
                Disconnet();
            }*/
        }



      //public void CallBack_ReceiveMsg(IAsyncResult ar)
      //{
            /*byte[] bytes = (byte[])ar.AsyncState;
            intSize = clientSocket.RecvToStreamBuffer();
            if (intSize > 0)
            {
                byteReceiveMsg = clientSocket.mTempStreamBuffer;
                strReceiveMsg = Encoding.Default.GetString(byteReceiveMsg, 0, intSize);
                AppendText(strReceiveMsg);
            }*/

            

                
                 
                


             //     AppendText(strReceiveMsg);


             


            

            /*
            try
            {
                if (client != null)
                {

                    if (client.Connected == true)
                    {
                        intSize = client.EndReceive(ar);
                        strReceiveMsg = "";
                        strReceiveMsg = Encoding.Default.GetString(bytes, 0, intSize);
                        client.BeginReceive(byteReceiveMsg, 0, byteReceiveMsg.Length, SocketFlags.None, new AsyncCallback(CallBack_ReceiveMsg), byteReceiveMsg);


                        AppendText(strReceiveMsg);


                    }


                    Array.Clear(bytes, 0, bytes.Length);
                }





            }
            catch (Exception err)
            {
                AppendText(string.Format("[SYSTEM] : {0}", err.Message));

            }
            */
     // }


        private void Disconnet()
        {
            if (client != null)
            {
                AppendText("서버에 재접속중...\n");
                client.Close();
                client = null;
                bConnectServer = false;
              

            }
        }

        [STAThread]
        private void AppendText(String str)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                textCallbak d = new textCallbak(AppendText);
                this.Invoke(d, new object[] { str });
            }
            else
            {
                this.richTextBox1.Text += str;
            }
        }

        public void receiveMessage(string str)
        {
            AppendText(str +"\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                clientSocket.CloseSocket();
                AppendText("접속 종료" + "\n");
            }
            catch(Exception ex)
            {
                AppendText(ex.Message+"\n");
            }


           /* if (client != null)
            {
                client.Close();
            }
            client = null;
            bConnectServer = false;*/


           
        }

        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            byteSendMsg = Encoding.Default.GetBytes(txtSendMsg.Text);
            int num = 0;
            num = clientSocket.Send(byteSendMsg, byteSendMsg.Length);

            if(num <0)
            {
                AppendText(string.Format("[SYSTEM] : 전송 실패\n"));
            }
            else
            {
                AppendText(string.Format("[CLIENT] : {0}", txtSendMsg.Text) + "\n");
            }


            /*
            if (!bConnectServer)
            {
                AppendText(string.Format("[SYSTEM] : 서버와의 접속이 되지 않았습니다.\n"));
                //     Add_Log(strErr);
                txtSendMsg.Text = "";
                txtSendMsg.Focus();
                return;
            }

            if (txtSendMsg.Text == "") return;

            byteSendMsg = Encoding.Default.GetBytes(txtSendMsg.Text);
         //   AppendText(string.Format("[CLIENT] : {0}", txtSendMsg.Text)+"\n");
            // Add_Log(strLog);
            client.BeginSend(byteSendMsg, 0, byteSendMsg.Length, SocketFlags.None, new AsyncCallback(CallBack_SendMsg), client);
          //  txtSendMsg.Text = "";
          //  txtSendMsg.Focus();
            */
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {


                intSize = clientSocket.RecvToStreamBuffer();
                if (intSize > 0)
                {
                    //  clientSocket.Recv(byteReceiveMsg, intSize);
                    // strReceiveMsg = "";
                    byteReceiveMsg = clientSocket.mTempStreamBuffer;
                     strReceiveMsg = Encoding.Default.GetString(byteReceiveMsg, 0, intSize);
                    AppendText(strReceiveMsg);
                }

            }
            catch (Exception ee)
            {

            }
        }
    }
}
