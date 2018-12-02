using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BcTool
{
    public class StateObject
    {
        public Socket workSocket = null;
        public int offset;
        public int len;
        public byte[] buffer;
        public AsyncCallback asyncCallback;
    }

    public class NetMng
    {
        public static int BC_SERVER_PORT = 8025;
        public static UInt16 DEFAULT_RECV_BUFFER_SIZE = 2048;
     
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static Queue<object> recvPackQueue = new Queue<object>();
        private static readonly object recvPackQueuelocker = new object();

        private Socket client;
        private string address;
        private int port;
        // private AsyncCallback connectCallBack;
        private AsyncCallback sendAsyncCallBack;
        private AsyncCallback receiveAsyncCallBack;
        private byte[] recvBuffer;

        public string Address { get => address; set => address = value; }
        public int Port { get => port; set => port = value; }
        public AsyncCallback SendAsyncCallBack { get => sendAsyncCallBack; set => sendAsyncCallBack = value; }
        public AsyncCallback ReceiveAsyncCallBack { get => receiveAsyncCallBack; set => receiveAsyncCallBack = value; }

        // public AsyncCallback ConnectCallBack { get => connectCallBack; set => connectCallBack = value; }

        public NetMng(string servAddr)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            address = servAddr;
            port = BC_SERVER_PORT;
            initRecvBuffer(DEFAULT_RECV_BUFFER_SIZE);
        }

        public void initRecvBuffer(UInt16 size)
        {
            recvBuffer = new byte[size];
        }

        public bool connect()
        {
            bool ret = false;
            IPAddress ip = IPAddress.Parse(address);
            IPEndPoint remoteEP = new IPEndPoint(ip, port);

            try
            {
                client.Connect(remoteEP);
                ret = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ret = false;
            }

            return ret;
        }

        public bool write(byte[] msg)
        {
            bool ret = false;
            try
            {
                if(client.Send(msg) <= 0)
                {
                    ret = false;
                }
                else
                {
                    ret = true;
                }
            } 
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                ret = false;
            }
            return ret;
        }

        public int read(byte[] buffer, int offset, int size)
        {
            int ret = -1;
            try
            {
                ret = client.Receive(buffer, offset, size, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ret = -1;
            }
            return ret;
        }

        public bool writeAync(byte[] msg, int offset, int len, AsyncCallback asyncCallback)
        {
            bool ret = false;
            if(null == msg || 0 == msg.Length)
            {
                return ret;
            }
            try
            {
                client.BeginSend(msg, offset, len, 0,
                    asyncCallback, client);
                ret = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ret = false;
            }
            return ret;
        }

        public Boolean recvAsync(StateObject state)
        {
            Boolean ret = false;
            if(null == state)
            {
                return ret;
            }
            try
            {
                client.BeginReceive(state.buffer, state.offset, state.len, 0,
                    new AsyncCallback(state.asyncCallback), state);
                ret = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ret = false;
            }
            return ret;
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                // sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            /*
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            
            */
            /*
            lock(recvPackQueuelocker)
            {
                recvPackQueue.Enqueue();
            }
            receiveDone.Set();
            */
        }

        /*
         *        static void Work()
        {
            while (true)
            {
                string work = null;
                lock (_locker)
                {
                    if (_tasks.Count > 0)
                    {
                        work = _tasks.Dequeue(); // 有任务时，出列任务
                        
                        if (work == null)  // 退出机制：当遇见一个null任务时，代表任务结束
                            return;
                    }
                }

                if (work != null)
                    SaveData(work);  // 任务不为null时，处理并保存数据
                else
                    _wh.WaitOne();   // 没有任务了，等待信号
            }
        }
         */

        public void destroy()
        {
            try
            {
                client.Close();
                client.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
