using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BcTool
{
    public partial class Simulator : Form
    {

        enum MSG_RECV_STATE
        {
            WAIT_HEADER,
            WAIT_DATA,
            ERROR_RESTORE,
        }

        private const int LOG_LEVEL_TRACE = 4;
        private const int LOG_LEVEL_DEBUG = 3;
        private const int LOG_LEVEL_INFO = 2;
        private const int LOG_LEVEL_ERROR = 1;
        private const int NET_ERROR_RESTORE_WAIT_PERIOD = 1000; // ms

        public const int MSG_RECV_BUFFER_SIZE = 2048;
        public const long EVENT_TIMEOUT = 0x1;
        public const long EVENT_NET_MSSAGE_RECV = 0x2;
        public const long EVENT_NET_MSSAGE_SEND = 0x4;
        public const int TIMER_INTERVAL = 1000; // ms

        public static Boolean endLoop;
        public static Boolean endMsgRecv;
        private static Simulator currentSim;
        private static ManualResetEvent simSignal = new ManualResetEvent(false);
        private static TimeSpan simTimeSpan = new TimeSpan(0, 0, 1);
        private static long eventFlags = 0;
        private static readonly object eventFlagsLocker = new object();
        private static MSG_RECV_STATE msgRecvState;
        private static byte[] msgBuffer;
        private static NetMng netMng;
        private static string address;
        


        private int CURRENT_LOG_LEVEL = LOG_LEVEL_DEBUG;

        private List<SignalDataItem> signalDataItems;
        private Dictionary<UInt16, LanguageResourceItem> signalNameLangTable;
        private UInt16 languageKey;
        private Thread simThread;
        private Thread netRecvMsgThread;
        // private MSG_RECV_STATE msgRecvState = MSG_RECV_STATE.WAIT_HEADER;
        private Boolean msgRecvWaiting = false;
        private System.Timers.Timer timer;
        private readonly object packSendListLocker = new object();
        private List<byte[]> packSendList = new List<byte[]>();
        private List<string> packRecvList = new List<string>();
        private string sn;
        private string password;
        private IntPtr bufPtr;
        private UInt64 bufSize;
        private IntPtr bufName;
        private IntPtr bufPassword;
        private Boolean simulating;

        public BPLibApi.BPContext bPContext;
        // public BPLibApi.PackBuf packBuf;
        // public List<byte[]> PackSendList { get => packSendList; set => packSendList = value; }
        public List<string> PackRecvList { get => packRecvList; set => packRecvList = value; }
        public string Sn { get => sn; set => sn = value; }
        public string Password { get => password; set => password = value; }
        public IntPtr BufPtr { get => bufPtr; set => bufPtr = value; }
        public IntPtr BufName { get => bufName; set => bufName = value; }
        public IntPtr BufPassword { get => bufPassword; set => bufPassword = value; }
        public bool Simulating { get => simulating; set => simulating = value; }

        public Simulator()
        {
            InitializeComponent();
            init();
        }

        public void init()
        {
            simulating = false;
            msgRecvWaiting = false;
            msgRecvState = MSG_RECV_STATE.WAIT_HEADER;
            endLoop = false;
            endMsgRecv = false;
            simSignal.Reset();
            currentSim = this;
            address = "127.0.0.1";
            netMng = new NetMng(address);

            bPContext = new BPLibApi.BPContext();
            BPLibApi.PackBuf packBuf = new BPLibApi.PackBuf();
            bufPtr = Marshal.AllocHGlobal(2048);
            bufSize = 2048;
            bufName = Marshal.AllocHGlobal(128);
            bufPassword = Marshal.AllocHGlobal(128);
            BPLibApi.BP_Init2Default(ref bPContext);
            BPLibApi.BP_InitPackBuf(ref packBuf, bufPtr, ref bufSize);
            int s = Marshal.SizeOf(packBuf);
            IntPtr tmp = Marshal.AllocHGlobal(s);
            Marshal.StructureToPtr(packBuf, tmp, true);
            bPContext.packBuf = tmp;
            bPContext.name = bufName;
            bPContext.password = bufPassword;
            // BPLibApi.BP_InitEmbededContext();

            msgBuffer = new byte[MSG_RECV_BUFFER_SIZE];

            
        }

        static void timeout(object sender, System.Timers.ElapsedEventArgs e)
        {
            triggerSignal(EVENT_TIMEOUT);
        }

        public void setSignalDataItemList(List<SignalDataItem> list)
        {
            signalDataItems = list;
        }

        public void setSignalNameLangTable(Dictionary<UInt16, LanguageResourceItem> table)
        {
            signalNameLangTable = table;
        }

        public void setLanguageKey(UInt16 key)
        {
            languageKey = key;
        }

        public void reloadSignalTable()
        {
            int size = dataGridViewSignalTable.Rows.Count;

                dataGridViewSignalTable.Rows.Clear();
       
            if(signalDataItems != null)
            {
                foreach (SignalDataItem value in signalDataItems)
                {
                    UInt16 signalID = (UInt16)value.SignalId;
                    DataGridViewCellStyle dataGridViewCellStyleTmp;
                    DataGridViewCell dataGridViewCellTmp;
                    if (signalNameLangTable.ContainsKey(signalID))
                    {
                        dataGridViewSignalTable.Rows.Add();
                        dataGridViewSignalTable.Rows[dataGridViewSignalTable.Rows.Count - 1].Cells[SignalDataItem.GRIDVIEW_SIGNAL_ID].Value = signalNameLangTable[signalID].LanguageMap[languageKey];
                        dataGridViewSignalTable.Rows[dataGridViewSignalTable.Rows.Count - 1].Cells[SignalDataItem.GRIDVIEW_TYPE].Value = value.getValueTypeString();
                        dataGridViewSignalTable.Rows[dataGridViewSignalTable.Rows.Count - 1].Cells[SignalDataItem.GRIDVIEW_VALUE].Value = value.getDefaultString();
                        if(!value.Alarm)
                        {
                            dataGridViewCellTmp = dataGridViewSignalTable.Rows[dataGridViewSignalTable.Rows.Count - 1].Cells[SignalDataItem.GRIDVIEW_ALARM];
                            dataGridViewCellTmp.ReadOnly = true;
                            dataGridViewCellStyleTmp = dataGridViewCellTmp.Style;
                            dataGridViewCellStyleTmp.BackColor = SystemColors.Control;
                        }

                    }
                }
            }
            
        }

        private void dataGridViewSignalTable_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (false == dataGridViewSignalTable.CurrentCell is DataGridViewCheckBoxCell)
            {
                int column = e.ColumnIndex;
                int row = e.RowIndex;
                packSendList.Add(new byte[1]);
                triggerSignal(EVENT_NET_MSSAGE_SEND);
            }

        }

        /* for value changed of dataGridViewSignalTable checkbox */
        private void dataGridViewSignalTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewSignalTable.CurrentCell is DataGridViewCheckBoxCell)
            {
                MessageBox.Show(this.dataGridViewSignalTable[e.ColumnIndex, e.RowIndex].Value.ToString());

            }
        }

        /* for value changed of dataGridViewSignalTable checkbox */
        private void dataGridViewSignalTable_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewSignalTable.CurrentCell is DataGridViewCheckBoxCell)
            {
                dataGridViewSignalTable.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        public Boolean isSim()
        {
            return !endLoop;
        }

        public void startSim()
        {
            LogLn(LOG_LEVEL_INFO, "start simulate");
            simulating = true;
            timer = new System.Timers.Timer(TIMER_INTERVAL);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timeout);
            timer.Start();
            simThread = new Thread(simTask);
            simThread.Start();
            netRecvMsgThread = new Thread(netMsgRecvTask);
            netRecvMsgThread.Start();
        }

        public void stopSim()
        {
            LogLn(LOG_LEVEL_INFO, "stop simulate");
        }

        private void Log(int level, string msg)
        {
            if(level <= CURRENT_LOG_LEVEL)
            {
                richTextBoxSim.ScrollToCaret();
                switch(level)
                {
                    case LOG_LEVEL_TRACE:
                        richTextBoxSim.SelectionColor = Color.Green;
                        break;
                    case LOG_LEVEL_DEBUG:
                        richTextBoxSim.SelectionColor = Color.Blue;
                        break;
                    case LOG_LEVEL_INFO:
                        richTextBoxSim.SelectionColor = Color.Black;
                        break;
                    case LOG_LEVEL_ERROR:
                        richTextBoxSim.SelectionColor = Color.Red;
                        break;
                    default:
                        richTextBoxSim.SelectionColor = Color.Black;
                        break;
                }
                richTextBoxSim.AppendText(msg);
            }
        }

        public void LogLn(int level, string msg)
        {
            string msgTmp = msg + "\r\n";
            Log(level, msgTmp);
        }

        private void 中文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            languageKey = LanguageResourceItem.CHINESE_KEY;
            reloadSignalTable();
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            languageKey = LanguageResourceItem.ENGLISH_KEY;
            reloadSignalTable();
        }

        private void françaisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            languageKey = LanguageResourceItem.FRENCH_KEY;
            reloadSignalTable();
        }

        private void русскийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            languageKey = LanguageResourceItem.RUSSIAN_KEY;
            reloadSignalTable();
        }

        private void العربيةToolStripMenuItem_Click(object sender, EventArgs e)
        {
            languageKey = LanguageResourceItem.ARABIC_KEY;
            reloadSignalTable();
        }

        private void españolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            languageKey = LanguageResourceItem.SPANISH_KEY;
            reloadSignalTable();
        }

        static long getEventFlags()
        {
            long ret;
            lock (eventFlagsLocker)
            {
                ret = eventFlags;
            }
            return ret;
        }

        static void triggerSignal(long eventFlagsTmp)
        {
            lock (eventFlagsLocker)
            {
                eventFlags |= eventFlagsTmp;
                simSignal.Set();
            }
            
        }

        static void clearSignal(long eventFlagsTmp)
        {
            lock (eventFlagsLocker)
            {
                eventFlags &= ~eventFlagsTmp;
                simSignal.Reset();
            }
            
        }

        static void simTask()
        {
            long eventFlagsTmp;
            do
            {
                simSignal.WaitOne();
                eventFlagsTmp = getEventFlags();

                if ((eventFlagsTmp & EVENT_TIMEOUT) != 0)
                {
                    Console.WriteLine("event timeout");
                    clearSignal(EVENT_TIMEOUT);
                }
                if ((eventFlagsTmp & EVENT_NET_MSSAGE_RECV) != 0)
                {
                    Console.WriteLine("event net message recv");
                    clearSignal(EVENT_NET_MSSAGE_RECV);
                }
                if ((eventFlagsTmp & EVENT_NET_MSSAGE_SEND) != 0)
                {
                    Console.WriteLine("event net message send");

                    byte[] pack = null;
                    do
                    {
                        pack = currentSim.popSendPack();
                        if(pack != null)
                        {
                            netMng.write(pack);
                        }
                    } while (pack != null);

                clearSignal(EVENT_NET_MSSAGE_SEND);
                }

            } while (!endLoop);
            Console.WriteLine("simTask: End");
            // currentSim.destroy();
        }

        static void netMsgRecvTask()
        {
            bool errOccur = false;
            int recvNumTmp;
            int byteRead = 0;
            int remainLen = 0;

            IntPtr snPtr = Marshal.AllocHGlobal(currentSim.Sn.Length + 1);
            byte[] snBytes = Tools.addCStringEndFlag(System.Text.Encoding.ASCII.GetBytes(currentSim.Sn));
            Marshal.Copy(snBytes, 0, snPtr, snBytes.Length);

            IntPtr passwordPtr = Marshal.AllocHGlobal(currentSim.Password.Length + 1);
            byte[] passwordBytes = Tools.addCStringEndFlag(System.Text.Encoding.ASCII.GetBytes(currentSim.Password));
            Marshal.Copy(passwordBytes, 0, passwordPtr, passwordBytes.Length);

            IntPtr intPtrPack = BPLibApi.BP_PackConnect(ref currentSim.bPContext, snPtr, passwordPtr);
            BPLibApi.PackBuf packBufSend = (BPLibApi.PackBuf)Marshal.PtrToStructure(intPtrPack, typeof(BPLibApi.PackBuf));
            byte[] sendBytes = new byte[packBufSend.MsgSize];
            Marshal.Copy(packBufSend.PackStart, sendBytes, 0, sendBytes.Length);
            

            netMng.connect();
            netMng.write(sendBytes);
            do
            {
                try
                {
                    switch (msgRecvState)
                    {
                        case MSG_RECV_STATE.WAIT_HEADER:
                            {
                                recvNumTmp = netMng.read(msgBuffer, byteRead, BPLibApi.FIX_HEADER_SIZE - byteRead);
                                if(recvNumTmp <= 0 || byteRead > BPLibApi.FIX_HEADER_SIZE)
                                {
                                    msgRecvState = MSG_RECV_STATE.ERROR_RESTORE;
                                    break;
                                }
                                byteRead += recvNumTmp;
                                if(BPLibApi.FIX_HEADER_SIZE == byteRead)
                                {
                                    msgRecvState = MSG_RECV_STATE.WAIT_DATA;
                                    remainLen = (msgBuffer[1] & 0xFF) << 8;
                                    remainLen += msgBuffer[2] & 0xFF;

                                }
                                break;
                            }
                        case MSG_RECV_STATE.WAIT_DATA:
                            {
                                recvNumTmp = netMng.read(msgBuffer, byteRead, remainLen + BPLibApi.FIX_HEADER_SIZE - byteRead);
                                if (recvNumTmp < 0 || byteRead > remainLen + BPLibApi.FIX_HEADER_SIZE)
                                {
                                    msgRecvState = MSG_RECV_STATE.ERROR_RESTORE;
                                    break;
                                }
                                byteRead += recvNumTmp;
                                if (remainLen + BPLibApi.FIX_HEADER_SIZE == byteRead)
                                {
                                    msgRecvState = MSG_RECV_STATE.WAIT_HEADER;
                                    /* send signal */

                                }
                                break;
                            }
                        case MSG_RECV_STATE.ERROR_RESTORE:
                            {
                                byteRead = 0;
                                remainLen = 0;
                                msgRecvState = MSG_RECV_STATE.WAIT_HEADER;
                                Console.WriteLine("net error...");
                                netMng.destroy();
                                netMng = new NetMng(address);
                                netMng.connect();
                                Thread.Sleep(NET_ERROR_RESTORE_WAIT_PERIOD);
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    errOccur = true;
                    msgRecvState = MSG_RECV_STATE.ERROR_RESTORE;
                }
            } while (!endMsgRecv);
            Console.WriteLine("netMsgRecvTask: End");
            Marshal.FreeHGlobal(snPtr);
            Marshal.FreeHGlobal(passwordPtr);
        }

        public void doTimeout()
        {
            switch(msgRecvState)
            {
                case MSG_RECV_STATE.WAIT_HEADER:
                    {
                        if(!msgRecvWaiting)
                        {
                            msgRecvWaiting = true;
                        }
                        break;
                    }
                case MSG_RECV_STATE.WAIT_DATA:
                    {
                        break;
                    }
            }
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                /*
                if (bytesRead > 0)
                {
                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, state.BufferSize, 0,
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
                */
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void endTasks()
        {
            try
            {
                bpDisconn();
                endLoop = true;
                simThread.Join();
                timer.Stop();
                timer.Close();
                netMng.destroy();
                endMsgRecv = true;
                netRecvMsgThread.Join();
            }
            catch (Exception e)
            {

            }
        }

        private void destroy()
        {
            try
            {
                endTasks();
                // netMng.destroy();
                Marshal.FreeHGlobal(bufPtr);
                Marshal.FreeHGlobal(bufName);
                Marshal.FreeHGlobal(bufPassword);
                Marshal.FreeHGlobal(bPContext.packBuf);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private Boolean bpDisconn()
        {
            Boolean ret = false;
            try
            {
                IntPtr intPtrPack = BPLibApi.BP_PackDisconn(ref bPContext);
                BPLibApi.PackBuf packBufSend = (BPLibApi.PackBuf)Marshal.PtrToStructure(intPtrPack, typeof(BPLibApi.PackBuf));
                byte[] sendBytes = new byte[packBufSend.MsgSize];
                Marshal.Copy(packBufSend.PackStart, sendBytes, 0, sendBytes.Length);

                pushSendPack(sendBytes);
                ret = true;
            } 
            catch(Exception e)
            {
                Console.Write(e.Message);
                ret = false;
            }
            return ret;
        }

        private byte[] popSendPack()
        {
            byte[] ret = null;
            lock (packSendListLocker)
            {
                if(packSendList.Count > 0)
                {
                    ret = packSendList[0];
                    packSendList.RemoveAt(0);
                }
            }
            return ret;
        }

        private void pushSendPack(byte[] pack)
        {
            lock (packSendListLocker)
            {
                packSendList.Add(pack);
                triggerSignal(EVENT_NET_MSSAGE_SEND);
            }
        }


    }
}
