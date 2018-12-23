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

        public const string PREFIX_LANG_SYSTEM_SIGNAL = "systemLang";
        public const string PREFIX_LANG_SYSTEM_UNIT = "systemUnit";
        public const string PREFIX_LANG_SYSTEM_ENUM = "systemEnum";
        public const string PREFIX_LANG_SYSTEM_GROUP = "systemGroup";

        public const string PREFIX_SIGNAL_SYSTEM_BASIC = "basic";
        public const string PREFIX_SIGNAL_SYSTEM_TEMP_HUM = "tempHum";

        private int[] systemLanguageTabIndex = { 4, 5, 6 };

        private NetMng netMng = new NetMng("127.0.0.1");
        private Simulator simulator;
        private List<DataGridView> dist2DataGridViewList;
        private List<Hashtable> dist2SignalDataItemHashTable;
        private Dictionary<String, DataGridView> prefix2SignalDataGridView;
        private Dictionary<String, DataGridView> prefix2LangDataGridView;
        private Dictionary<string, Dictionary<UInt16, LanguageResourceItem>> prefix2LangDictionary;

        private BPLibApi.BP_SigId2Val[] sysSigId2Val;
        private IntPtr sysSigId2ValIntPtr;
        private BP_WORD sysSigId2ValSize;
        private BPLibApi.BP_SigTable[] sysSigTable;
        private IntPtr sysSigTableIntPtr;
        private List<Byte[]> systemSignalEnableBits;
        private List<BPLibApi.BP_SysSigMap> bpSysSigMaps;
        private IntPtr bpSysSigMapsIntPtr;
        private BP_WORD bpSysSigMapSize;

        private BPLibApi.BP_SysCustomUnit[] bpSysCusUnitTable;
        private BP_WORD bpSysCusUnitTableNum;
        private IntPtr bpSysCusUnitTableIntPtr;

        private BPLibApi.BP_SigId2Val[] cusSigId2Val;
        private IntPtr cusSigId2ValIntPtr;
        private BP_WORD cusSigId2ValSize;
        private BPLibApi.BP_SigTable[] cusSigTable;
        private IntPtr cusSigTableIntPtr;
        private string[] cusSigNameLang;
        private IntPtr cusSigNameLangIntPtr;
        private BP_WORD cusSigNameLangSize;
        private IntPtr cusSigUnitLangIntPtr;
        private UInt64 cusSigUnitLangSize;
        private IntPtr cusSigGroupLangIntPtr;
        private UInt64 cusSigGroupLangSize;
        private IntPtr cusSigEnumLangIntPtr;
        private UInt64 cusSigEnumLangSize;
        private BPLibApi.BP_CusLangMap[] cusSigNameLangMap;
        private IntPtr cusSigNameLangMapIntPtr;
        private BP_WORD cusSigNameLangMapSize;
        private BPLibApi.BP_CusLangMap[] cusSigUnitLangMap;
        private IntPtr cusSigUnitLangMapIntPtr;
        private BP_WORD cusSigUnitLangMapSize;
        private BPLibApi.BP_CusLangMap[] cusSigGroupLangMap;
        private IntPtr cusSigGroupLangMapIntPtr;
        private BP_WORD cusSigGroupLangMapSize;
        private BPLibApi.BP_SigId2EnumSignalMap[] cusSigEnumLangMap;
        private IntPtr cusSigEnumLangMapIntPtr;
        private BP_WORD cusSigEnumLangMapSize;

        public BcTool()
        {
            InitializeComponent();
            myInit();
        }

        private void myInit()
        {
            this.WindowState = FormWindowState.Maximized;
            progressBar1.Value = 0;
            
            comboBoxCrcType.SelectedIndex = 0;
            comboBoxCrcType.Enabled = false;
            comboBoxEncryption.SelectedIndex = 0;
            comboBoxEncryption.Enabled = false;
            comboBoxPerformance.SelectedIndex = 0;
            comboBoxPerformance.Enabled = false;



            dist2DataGridViewList = new List<DataGridView>();
            dist2DataGridViewList.Add(this.systemBasicDataGridView);
            dist2SignalDataItemHashTable = new List<Hashtable>();
            for (int i = 0; i < BPLibApi.SYSTEM_SIGNAL_TABLE_NUM; i++)
            {
                dist2SignalDataItemHashTable.Add(new Hashtable());
            }
            prefix2SignalDataGridView = new Dictionary<string, DataGridView>();
            prefix2SignalDataGridView.Add(PREFIX_SIGNAL_SYSTEM_BASIC, systemBasicDataGridView);
            prefix2SignalDataGridView.Add(PREFIX_SIGNAL_SYSTEM_TEMP_HUM, systemTempHumDataGridView);

            prefix2LangDataGridView = new Dictionary<string, DataGridView>();
            prefix2LangDataGridView.Add(PREFIX_LANG_SYSTEM_SIGNAL, systemLangDataGridView);
            prefix2LangDataGridView.Add(PREFIX_LANG_SYSTEM_UNIT, systemUnitDataGridView);
            prefix2LangDataGridView.Add(PREFIX_LANG_SYSTEM_ENUM, systemEnumDataGridView);
            prefix2LangDataGridView.Add(PREFIX_LANG_SYSTEM_GROUP, systemGroupDataGridView);

            prefix2LangDictionary = new Dictionary<string, Dictionary<UInt16, LanguageResourceItem>>();
            prefix2LangDictionary.Add(PREFIX_LANG_SYSTEM_SIGNAL, new Dictionary<UInt16, LanguageResourceItem>());
            prefix2LangDictionary.Add(PREFIX_LANG_SYSTEM_UNIT, new Dictionary<UInt16, LanguageResourceItem>());
            prefix2LangDictionary.Add(PREFIX_LANG_SYSTEM_ENUM, new Dictionary<UInt16, LanguageResourceItem>());
            prefix2LangDictionary.Add(PREFIX_LANG_SYSTEM_GROUP, new Dictionary<UInt16, LanguageResourceItem>());



            loadReadOnlyDataGridView("sys_unit_language_resource.csv", this.systemUnitDataGridView, 2);
            loadReadOnlyDataGridView("sys_group_language_resource.csv", this.systemGroupDataGridView, 2);
            loadReadOnlyDataGridView("sys_enum_language_resource.csv", this.systemEnumDataGridView, 2);
            loadReadOnlyDataGridView("sys_sig_info_basic.csv", this.systemBasicDataGridView, 2);
            loadReadOnlyDataGridView("sys_sig_info_basic_language_resource.csv", this.systemLangDataGridView, 2);
            loadReadOnlyDataGridView("sys_sig_info_temp_humidity.csv", this.systemTempHumDataGridView, 2);
            loadReadOnlyDataGridView("sys_sig_info_temp_humidity_language_resource.csv", this.systemLangDataGridView, 2);


            sysSigId2ValIntPtr = IntPtr.Zero;
            sysSigTableIntPtr = IntPtr.Zero;
            cusSigId2ValIntPtr = IntPtr.Zero;
            cusSigNameLangIntPtr = IntPtr.Zero;
            cusSigUnitLangIntPtr = IntPtr.Zero;
            cusSigGroupLangIntPtr = IntPtr.Zero;
            cusSigEnumLangIntPtr = IntPtr.Zero;
            cusSigNameLangMapIntPtr = IntPtr.Zero;
            cusSigUnitLangMapIntPtr = IntPtr.Zero;
            cusSigGroupLangMapIntPtr = IntPtr.Zero;
            cusSigEnumLangMapIntPtr = IntPtr.Zero;

            systemSignalEnableBits = new List<byte[]>();
            for(int i = 0; i < BPLibApi.SYSTEM_SIGNAL_TABLE_NUM; i++)
            {
                systemSignalEnableBits.Add(new byte[64]);
            }

            bpSysSigMaps = new List<BPLibApi.BP_SysSigMap>();

    }

        private void loadReadOnlyDataGridView(string csvName, DataGridView dataGridView, int offsetLine)
        {

            // dataGridView.Rows.Clear();
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

                    int rowIndex = dataGridView.Rows.Count - 1;
                    while ((line = sr.ReadLine()) != null)
                    {
                        dataGridView.Rows.Add();
                        string[] stringArray = line.Split(',');
                        for(int i = 0; i < stringArray.Length; i++)
                        {
                            if(rowIndex % 2 == 0)
                            {
                                dataGridView.Rows[rowIndex].DefaultCellStyle.BackColor = SystemColors.ControlLightLight;
                            }
                            else
                            {
                                dataGridView.Rows[rowIndex].DefaultCellStyle.BackColor = SystemColors.Info;
                            }
                            
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
            if (null == simulator || simulator.IsDisposed)
            {
                simulator = new Simulator();
            }

            List<SignalDataItem> signalDataItemList = new List<SignalDataItem>();
            Dictionary<UInt16, Dictionary<char, Object>> systemSignalCustomInfo = new Dictionary<ushort, Dictionary<char, object>>();
            foreach(Dictionary<UInt16, LanguageResourceItem> value in prefix2LangDictionary.Values)
            {
                value.Clear();
            }
            Dictionary<UInt16, LanguageResourceItem> signalNameLanguageResourceTable = new Dictionary<ushort, LanguageResourceItem>();

            if(!simulator.Simulating)
            {
                Boolean ret = makeSimTable(ref signalDataItemList, ref prefix2LangDictionary);
                if (true == ret)
                {
                    try
                    {
                        int bpAlivePeriod = Convert.ToInt32(textBoxAliveTime.Text);
                        if (bpAlivePeriod > UInt16.MaxValue)
                        {
                            MessageBox.Show("Error: AliveTime too large(Max:" + UInt16.MaxValue + ")");
                            return;
                        }
                        int bpTimeout = Convert.ToInt32(textBoxTimeout.Text);
                        if (bpTimeout > 255)
                        {
                            MessageBox.Show("Error: Timeout too large(Max:" +255 + ")");
                            return;
                        }

                        loadSimData(signalDataItemList, systemSignalCustomInfo, null);
                        

                        simulator.setSignalDataItemList(signalDataItemList);
                        simulator.setSignalNameLangTable(prefix2LangDictionary[PREFIX_LANG_SYSTEM_SIGNAL]);
                        simulator.setLanguageKey(LanguageResourceItem.ENGLISH_KEY);
                        simulator.Show();
                        simulator.reloadSignalTable();
                        simulator.Sn = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX3";
                        simulator.Password = "3333333333333333333333333333333333333333333333333333333333333333";
                        simulator.BpAlivePeriod = (ushort)bpAlivePeriod;
                        simulator.BpTimeout = (ushort)bpTimeout;
                        simulator.startSim();
                    }
                    catch (System.OverflowException ex)
                    {
                        MessageBox.Show("Error: Invalid AliveTime/Timeout");
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("TODO: something wrong");
                    }
                }
                else
                {
                    MessageBox.Show("TODO: something wrong");
                }
            }
            else
            {
                MessageBox.Show("Running now");
            }

        }

        private void buttonStopSim_Click(object sender, EventArgs e)
        {

            if (null != simulator && !simulator.IsDisposed)
            {
                simulator.Dispose();
            }
        }

        private Boolean makeSimTable(ref List<SignalDataItem> signalDataItemList, ref Dictionary<string, Dictionary<UInt16, LanguageResourceItem> > prefix2LangTable)
        {
            Boolean ret = false;
            string err = "";
            try
            {
                foreach (string prefix in prefix2SignalDataGridView.Keys)
                {
                    DataGridView dataGridViewTmp = prefix2SignalDataGridView[prefix];
                    int size = dataGridViewTmp.Rows.Count;
                    for (int i = 0; i < size; i++)
                    {
                        err = "";
                        SignalDataItem tmp = SignalDataItem.parseSignalDataItem(dataGridViewTmp.Rows[i].Cells, prefix, true, ref err);
                        if (tmp != null)
                        {
                            err = "line " + i + ", ";
                            signalDataItemList.Add(tmp);
                        }
                        else if (!string.IsNullOrWhiteSpace(err))
                        {
                            err = "line " + i + ", " + err;
                            Console.WriteLine(err);
                        }

                    }
                }

                foreach (string prefix in prefix2LangDataGridView.Keys)
                {
                    DataGridView dataGridViewTmp = prefix2LangDataGridView[prefix];
                    int size = dataGridViewTmp.Rows.Count;
                    for (int i = 0; i < size; i++)
                    {
                        err = "";
                        LanguageResourceItem tmp = LanguageResourceItem.parseLanguageResourceItem(dataGridViewTmp.Rows[i].Cells, prefix, ref err);
                        if (tmp != null)
                        {
                            err = "line " + i + ", ";
                            prefix2LangTable[prefix].Add((UInt16)tmp.IndexId, tmp);
                        }
                        else if (!string.IsNullOrWhiteSpace(err))
                        {
                            err = "line " + i + ", ";
                            Console.WriteLine(err);
                        }
                        else
                        {
                            /* parse ok */
                            break;
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

        private void loadSimData(List<SignalDataItem> sysSignalDataItemList, Dictionary<UInt16, Dictionary<char, Object>> sysSignalCustomInfo, List<SignalDataItem> cusSignalDataItemList)
        {
            if(null == sysSignalDataItemList)
            {
                return;
            }
            if (null == cusSignalDataItemList)
            {
                return;
            }
            /* free old data */
            /* TODO: free the fields of IntPtr */
            Tools.freeIntPtr(sysSigId2ValIntPtr);
            Tools.freeIntPtr(sysSigTableIntPtr);
            Tools.freeIntPtr(cusSigId2ValIntPtr);
            Tools.freeIntPtr(cusSigNameLangIntPtr);
            Tools.freeIntPtr(cusSigUnitLangIntPtr);
            Tools.freeIntPtr(cusSigGroupLangIntPtr);
            Tools.freeIntPtr(cusSigEnumLangIntPtr);
            Tools.freeIntPtr(cusSigNameLangMapIntPtr);
            Tools.freeIntPtr(cusSigUnitLangMapIntPtr);
            Tools.freeIntPtr(cusSigGroupLangMapIntPtr);
            Tools.freeIntPtr(cusSigEnumLangMapIntPtr);

            /* system signal set new value */
            int sysSignlaSize = sysSignalDataItemList.Count;
            sysSigId2Val = new BPLibApi.BP_SigId2Val[sysSignlaSize];
            sysSigTable = new BPLibApi.BP_SigTable[sysSignlaSize];
            for (int i = 0; i < sysSignlaSize; i++)
            {
                SignalDataItem sdiTmp = sysSignalDataItemList[i];
                Tools.setDefaultValue(ref sysSigId2Val[i], sdiTmp);
                Tools.setSigTable(ref sysSigTable[i], sdiTmp);
                Tools.setSysSignalEnableBits(ref systemSignalEnableBits, (UInt16)(sdiTmp.SignalId & 0xFFFF));
            }

            sysSigId2ValIntPtr = Tools.mallocIntPtr(sysSigId2Val);
            sysSigId2ValSize = sysSignlaSize;

            bpSysSigMaps.Clear();
            for (int i = 0; i < BPLibApi.SYSTEM_SIGNAL_TABLE_NUM; i++)
            {
                Boolean signalEnabled = false;
                int sysSignalClass = 0;
                byte[] bits = systemSignalEnableBits[i];
                for (int j = BPLibApi.SYSTEM_SIGNAL_CLASS_END; j >= BPLibApi.SYSTEM_SIGNAL_CLASS_START; j--)
                {
                    int k1 = 1 << j;
                    int k0 = k1 / 2;
                    for(int k = k0; k < k1; k++)
                    {
                        if(bits[k] != 0)
                        {
                            signalEnabled = true;
                            sysSignalClass = j;
                            break;
                        }
                    }
                    if(signalEnabled)
                    {
                        break;
                    }

                }

                if(signalEnabled)
                {
                    BPLibApi.BP_SysSigMap bpSysSigMap = new BPLibApi.BP_SysSigMap();
                    bpSysSigMap.Dist = (char)0;
                    bpSysSigMap.Dist |= (char)(i << BPLibApi.SYSTEM_SIGNAL_CLASS_OFFSET);
                    bpSysSigMap.Dist |= (char)(sysSignalClass << BPLibApi.SYSTEM_SIGNAL_CLASS_OFFSET);
                    bpSysSigMap.SigMapSize = (char)(1 << sysSignalClass);
                    bpSysSigMap.SigMap = Tools.mallocIntPtr(bits, 0, bpSysSigMap.SigMapSize);
                    bpSysSigMaps.Add(bpSysSigMap);
                }
            }

            bpSysSigMapsIntPtr = Tools.mallocIntPtr(bpSysSigMaps);
            bpSysSigMapSize = bpSysSigMaps.Count;

            /* system signal custom info */
            // private BPLibApi.BP_SysCustomUnit bpSysCusUnitTable;
            // private BP_WORD bpSysCusUnitTableNum;
            // private IntPtr bpSysCusUnitTableIntPtr;
            bpSysCusUnitTable = new BPLibApi.BP_SysCustomUnit[1];
            bpSysCusUnitTable[0].SidId = 0xE000;
            bpSysCusUnitTable[0].CustomType = BPLibApi.SYS_SIG_CUSTOM_TYPE_DEF_VAL;
            bpSysCusUnitTable[0].CustomValue = Tools.mallocIntPtr("abc"); ; // IntPtr
            bpSysCusUnitTableNum = bpSysCusUnitTable.Count<BPLibApi.BP_SysCustomUnit>();

        /* custom signal set new value */
        int cusSignlaSize = cusSignalDataItemList.Count;
            cusSigId2Val = new BPLibApi.BP_SigId2Val[cusSignlaSize];
            cusSigTable = new BPLibApi.BP_SigTable[cusSignlaSize];
            for (int i = 0; i < sysSignlaSize; i++)
            {
                SignalDataItem sdiTmp = sysSignalDataItemList[i];
                Tools.setDefaultValue(ref sysSigId2Val[i], sdiTmp);
                Tools.setSigTable(ref sysSigTable[i], sdiTmp);
                Tools.setSysSignalEnableBits(ref systemSignalEnableBits, (UInt16)(sdiTmp.SignalId & 0xFFFF));
            }

            sysSigId2ValIntPtr = Tools.mallocIntPtr(sysSigId2Val);
            sysSigId2ValSize = sysSignlaSize;



            sysSigTable = new BPLibApi.BP_SigTable[2];
            sysSigTable[0].SigId = 0x5a5a;
            sysSigTable[0].SigType = 3;
            sysSigTable[0].IsDisplay = 1;
            sysSigTable[1].SigId = 0xa5a5;
            sysSigTable[1].SigType = 5;
            sysSigTable[1].IsDisplay = 0;
            sysSigTableIntPtr = Tools.mallocIntPtr(sysSigTable);

            // private IntPtr cusSigNameLangIntPtr;
            // private UInt64 cusSigNameLangSize;
            cusSigNameLang = new string[6];
            cusSigNameLang[0] = "";
            cusSigNameLang[1] = "";
            cusSigNameLang[2] = "";
            cusSigNameLang[3] = "";
            cusSigNameLang[4] = "light";
            cusSigNameLang[5] = "灯";
            cusSigNameLangIntPtr = Tools.mallocIntPtr(cusSigNameLang);
            cusSigNameLangSize = cusSigNameLang.Length;

            cusSigNameLangMap = new BPLibApi.BP_CusLangMap[2];
            cusSigNameLangMap[0].SigId = 0x0000;
            cusSigNameLangMap[0].LangId = 1;
            cusSigNameLangMap[1].SigId = 0x0001;
            cusSigNameLangMap[1].LangId = 2;
            cusSigNameLangMapIntPtr = Tools.mallocIntPtr(cusSigNameLangMap);

            // private BPLibApi.BP_CusLangMap[] cusSigEnumLangMap;
            // private IntPtr cusSigEnumLangMapIntPtr;
            // private BP_WORD cusSigEnumLangMapSize;
            cusSigEnumLangMap = new BPLibApi.BP_SigId2EnumSignalMap[2];
            cusSigEnumLangMap[0].SigId = 0x0001;
            BPLibApi.BP_EnumSignalMap tmp = new BPLibApi.BP_EnumSignalMap();
            tmp.Key = 0;tmp.Val = 1;
            tmp.Key = 1; tmp.Val = 2;
            cusSigEnumLangMap[0].EnumSignalMap = Tools.mallocIntPtr(tmp);
            cusSigEnumLangMap[0].EnumSignalMapNum = 2;
            cusSigEnumLangMap[1].SigId = 0x0002;
            tmp = new BPLibApi.BP_EnumSignalMap();
            tmp.Key = 0; tmp.Val = 2;
            tmp.Key = 1; tmp.Val = 3;
            cusSigEnumLangMap[1].EnumSignalMap = Tools.mallocIntPtr(tmp);
            cusSigEnumLangMap[1].EnumSignalMapNum = 3;
            cusSigEnumLangMapIntPtr = Tools.mallocIntPtr(cusSigNameLangMap);
            cusSigEnumLangMapSize = cusSigEnumLangMap.Count<BPLibApi.BP_SigId2EnumSignalMap>();
        }

        private void textBoxAliveTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if ((e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar)) || (String.IsNullOrWhiteSpace(t.Text.Trim()) && e.KeyChar == '0'))
            {
                e.Handled = true;
            }
        }

        private void textBoxAliveTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxAliveTime_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if(cb.Checked)
            {
                textBoxAliveTime.Text = "3600";
                textBoxAliveTime.ReadOnly = true;
            }
            else
            {
                textBoxAliveTime.ReadOnly = false;
            }
        }

        private void checkBoxTimeout_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked)
            {
                textBoxTimeout.Text = "10";
                textBoxTimeout.ReadOnly = true;
            }
            else
            {
                textBoxTimeout.ReadOnly = false;
            }
        }

        private void checkBoxPerformance_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked)
            {
                comboBoxPerformance.SelectedIndex = 0;
                comboBoxPerformance.Enabled = false;
            }
            else
            {
                comboBoxPerformance.Enabled = true;
            }
        }

        private void checkBoxEncryption_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked)
            {
                comboBoxEncryption.SelectedIndex = 0;
                comboBoxEncryption.Enabled = false;
            }
            else
            {
                comboBoxEncryption.Enabled = true;
            }
        }

        private void checkBoxCrcType_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked)
            {
                comboBoxCrcType.SelectedIndex = 0;
                comboBoxCrcType.Enabled = false;
            }
            else
            {
                comboBoxCrcType.Enabled = true;
            }
        }
    }
}
