using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BcTool
{
    public partial class BcTool : Form
    {
        private int[] systemLanguageTabIndex = { 4, 5, 6 };

        private NetMng netMng = new NetMng();
        private Simulator simulator;
        private List<DataGridView> dist2DataGridViewList;
        private List<Hashtable> dist2SignalDataItemHashTable;

        public BcTool()
        {
            InitializeComponent();
            myInit();
        }

        private void myInit()
        {
            progressBar1.Value = 0;
            dist2DataGridViewList = new List<DataGridView>();
            dist2DataGridViewList.Add(this.systemBasicDataGridView);
            dist2SignalDataItemHashTable = new List<Hashtable>();
            for (int i = 0; i < BPLibApi.SYSTEM_SIGNAL_TABLE_NUM; i++)
            {
                dist2SignalDataItemHashTable.Add(new Hashtable());
            }
            loadReadOnlyDataGridView("sys_signal_language_resource.csv", this.systemLangDataGridView, 1);
            loadReadOnlyDataGridView("sys_unit_language_resource.csv", this.systemUnitDataGridView, 1);
            loadReadOnlyDataGridView("sys_enum_language_resource.csv", this.systemEnumDataGridView, 2);
            loadReadOnlyDataGridView("sys_sig_info_basic.csv", this.systemBasicDataGridView, 2);
        }

        private void loadReadOnlyDataGridView(string csvName, DataGridView dataGridView, int offsetLine)
        {

            dataGridView.Rows.Clear();
            try
            {
                UTF8Encoding uTF8Encoding = new System.Text.UTF8Encoding(true);
                using (StreamReader sr = new StreamReader(csvName, uTF8Encoding))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    for (int i = 0; i < offsetLine; i++)
                    {
                        sr.ReadLine();
                    }

                    int rowIndex = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        dataGridView.Rows.Add();
                        string[] stringArray = line.Split(',');
                        for(int i = 0; i < stringArray.Length; i++)
                        {
                            dataGridView.Rows[rowIndex].Cells[i].Value = stringArray[i];
                        }
                        rowIndex++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private bool exportCsv(DataGridView dataGridView, string exportCsv)
        {
            bool ret = true;
            
            UTF8Encoding uTF8Encoding = new System.Text.UTF8Encoding(true);

            try
            {
                using (StreamWriter sw = new StreamWriter(exportCsv, false, uTF8Encoding))
                {
                    string line = "SignalID,中文,English,Français,русский,العربية,Español\r\n";
                    sw.Write(line);

                    for (int i = 0; i < dataGridView.Rows.Count; i++)
                    {
                        line = dataGridView.Rows[i].Cells[0].Value.ToString();
                        for(int j = 1; j < dataGridView.Rows[i].Cells.Count; j++)
                        {
                            line += "," + dataGridView.Rows[i].Cells[j].Value.ToString();
                        }
                        line += "\r\n";
                        sw.Write(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = false;
            }
            
            return ret;
        }

        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            SolidBrush solidBrushSystemLanguage = new SolidBrush(Color.Red);
            foreach (int i in systemLanguageTabIndex)
            {
                Rectangle tabRec = tabControl.GetTabRect(i);
                e.Graphics.FillRectangle(solidBrushSystemLanguage, tabRec);
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            exportCsv(this.systemLangDataGridView, "export.csv");
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            // messageBox.AppendText("" + x++ + "\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            netMng.setAddress("127.0.0.1");
            netMng.setPort(NetMng.BC_SERVER_PORT);
            netMng.connect();

            BPLibApi.BPContext bPContext = new BPLibApi.BPContext();
            BPLibApi.PackBuf packBuf = new BPLibApi.PackBuf();
            // byte[] buf = new byte[64];
            IntPtr bufPtr = Marshal.AllocHGlobal(64);
            IntPtr bufName = Marshal.AllocHGlobal(128);
            IntPtr bufPassword = Marshal.AllocHGlobal(128);
            BPLibApi.BP_Init2Default(ref bPContext);
            BPLibApi.BP_InitPackBuf(ref packBuf, bufPtr, 64);
            int s = Marshal.SizeOf(packBuf);
            IntPtr tmp = Marshal.AllocHGlobal(s);
            Marshal.StructureToPtr(packBuf, tmp, true);
            bPContext.packBuf = tmp;
            bPContext.name = bufName;
            bPContext.password = bufPassword;
            // BPLibApi.BP_InitEmbededContext();
            string sn = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX3";
            string password = "3333333333333333333333333333333333333333333333333333333333333333";

            IntPtr snPtr = Marshal.AllocHGlobal(sn.Length + 1);
            byte[] snBytes = Tools.addCStringEndFlag(System.Text.Encoding.ASCII.GetBytes(sn));
            Marshal.Copy(snBytes, 0, snPtr, snBytes.Length);

            IntPtr passwordPtr = Marshal.AllocHGlobal(password.Length + 1);
            byte[] passwordBytes = Tools.addCStringEndFlag(System.Text.Encoding.ASCII.GetBytes(password));
            Marshal.Copy(passwordBytes, 0, passwordPtr, passwordBytes.Length);

            IntPtr intPtrPack = BPLibApi.BP_PackConnect(ref bPContext, snPtr, passwordPtr);
            BPLibApi.PackBuf packBufSend = (BPLibApi.PackBuf)Marshal.PtrToStructure(intPtrPack, typeof(BPLibApi.PackBuf));
            byte[] sendBytes = new byte[packBufSend.MsgSize];
            Marshal.Copy(packBufSend.PackStart, sendBytes, 0, sendBytes.Length);
            netMng.write(sendBytes);
            */

            if (null == simulator || simulator.IsDisposed)
            {
                simulator = new Simulator();
            }

            Hashtable hashtable = new Hashtable();

            /*
            Random rd = new Random();
            int signalId = rd.Next(1, 0xFFFF);
            SignalDataItem tmp = new SignalDataItem(signalId, true, "ABC", false, SignalDataItem.ValueType.STRING, 1, SignalDataItem.BcPermission.RO, true, 0, 1, 1, 1, 1, null, true, 0x7F, 5, 5);
            hashtable.Add(1, tmp);
            signalId = rd.Next(1, 0xFFFF);
            tmp = new SignalDataItem(signalId, true, "ABC", false, SignalDataItem.ValueType.STRING, 1, SignalDataItem.BcPermission.RO, true, 0, 1, 1, 1, 1, null, true, 0x7F, 5, 5);
            hashtable.Add(2, tmp);
            signalId = rd.Next(1, 0xFFFF);
            tmp = new SignalDataItem(signalId, true, "ABC", false, SignalDataItem.ValueType.STRING, 1, SignalDataItem.BcPermission.RO, true, 0, 1, 1, 1, 1, null, true, 0x7F, 5, 5);
            hashtable.Add(3, tmp);
            */

            simulator.setSignalDataItemTable(hashtable);
            simulator.Show();
            simulator.reloadSignalTable();
            simulator.startSim();
        }

        private void buttonStopSim_Click(object sender, EventArgs e)
        {
            /*
            if(simulator.IsDisposed)
            {
                simulator = new Simulator();
            }
            
            Hashtable hashtable = new Hashtable();
            
            Random rd = new Random();
            int signalId = rd.Next(1, 0xFFFF);
            SignalDataItem tmp = new SignalDataItem(signalId, true, "ABC", false, SignalDataItem.ValueType.STRING, 1, SignalDataItem.BcPermission.RO, true, 0, 1, 1, 1, 1, null, true, 0x7F, 5, 5);
            hashtable.Add(1, tmp);
            signalId = rd.Next(1, 0xFFFF);
            tmp = new SignalDataItem(signalId, true, "ABC", false, SignalDataItem.ValueType.STRING, 1, SignalDataItem.BcPermission.RO, true, 0, 1, 1, 1, 1, null, true, 0x7F, 5, 5);
            hashtable.Add(2, tmp);
            signalId = rd.Next(1, 0xFFFF);
            tmp = new SignalDataItem(signalId, true, "ABC", false, SignalDataItem.ValueType.STRING, 1, SignalDataItem.BcPermission.RO, true, 0, 1, 1, 1, 1, null, true, 0x7F, 5, 5);
            hashtable.Add(3, tmp);
            
            simulator.setSignalDataItemTable(hashtable);
            simulator.Show();
            simulator.reloadSignalTable();
            */

            if (null != simulator && !simulator.IsDisposed)
            {
                simulator.stopSim();
            }
        }

        private Boolean makeSimTable(ref Hashtable hashtable)
        {
            Boolean ret = false;
            try
            {
                foreach (DataGridView dataGridView in dist2DataGridViewList)
                {
                    for(int i = 0; i < dataGridView.Rows.Count; i++)
                    {
                        if(dataGridView.Rows[i].Cells["Enabled"].Value.ToString().Equals("YES"))
                        {

                        }
                    }
                    
                }
                
                ret = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ret;
        }
    }
}
