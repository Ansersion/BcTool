﻿using System;
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

        public const int ENABLE_COLUMN_INDEX = 1;

        private int[] systemLanguageTabIndex = { 4, 5, 6 };

        private NetMng netMng = new NetMng("127.0.0.1");
        private Simulator simulator;
        private List<string> prefixLists;
        private List<DataGridView> dist2DataGridViewList;
        private List<Hashtable> dist2SignalDataItemHashTable;
        private Dictionary<String, DataGridView> prefix2SignalDataGridView;
        private Dictionary<String, Tools.SignalTableMetaData> prefix2SystemTableMetaData;
        private Dictionary<String, DataGridView> prefix2LangDataGridView;
        private Dictionary<string, Dictionary<UInt16, LanguageResourceItem>> prefix2LangDictionary;

        private Dictionary<String, List<SignalDataItem>> prefix2systemSignalDataItemConstList;

        /* system signal info */
        private BPLibApi.BP_SigId2Val[] sysSigId2Val;
        private IntPtr sysSigId2ValIntPtr;
        private BP_WORD sysSigId2ValSize;

        private BPLibApi.BP_SigTable[] sysSigTable;
        private IntPtr sysSigTableIntPtr;

        private List<Byte[]> systemSignalEnableBits;
        private List<BPLibApi.BP_SysSigMap> bpSysSigMaps;
        private IntPtr bpSysSigMapsIntPtr;
        private BP_WORD bpSysSigMapSize;

        /* custom system signal info */
        private BPLibApi.BP_SysCustomUnit[] bpSysCusUnitTable;
        private BP_WORD bpSysCusUnitTableNum;
        private IntPtr bpSysCusUnitTableIntPtr;

        /* custom signal info */
        private BPLibApi.BP_SigId2Val[] cusSigId2Val;
        private IntPtr cusSigId2ValIntPtr;
        private BP_WORD cusSigId2ValSize;

        private BPLibApi.BP_SigTable[] cusSigTable;
        private IntPtr cusSigTableIntPtr;

        private string[] cusSigNameLang;
        private string[] cusSigUnitLang;
        private string[] cusSigGroupLang;
        private string[] cusSigEnumLang;

        private IntPtr cusSigNameLangIntPtr;
        private BP_WORD cusSigNameLangSize;
        private IntPtr cusSigUnitLangIntPtr;
        private BP_WORD cusSigUnitLangSize;
        private IntPtr cusSigGroupLangIntPtr;
        private BP_WORD cusSigGroupLangSize;
        private IntPtr cusSigEnumLangIntPtr;
        private BP_WORD cusSigEnumLangSize;

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

        /* */
        private string gererateDirectory = @".\";

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

            prefixLists = new List<string>();
            prefixLists.Add(PREFIX_SIGNAL_SYSTEM_BASIC);
            prefixLists.Add(PREFIX_SIGNAL_SYSTEM_TEMP_HUM);

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

            prefix2SystemTableMetaData = new Dictionary<string, Tools.SignalTableMetaData>();
            prefix2SystemTableMetaData.Add(PREFIX_SIGNAL_SYSTEM_BASIC, new Tools.SignalTableMetaData());
            prefix2SystemTableMetaData.Add(PREFIX_SIGNAL_SYSTEM_TEMP_HUM, new Tools.SignalTableMetaData());

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

            Tools.SignalTableMetaData signalTableMetaDataTmp = new Tools.SignalTableMetaData();
            loadReadOnlyDataGridView("sys_unit_language_resource.csv", this.systemUnitDataGridView, ref signalTableMetaDataTmp);
            loadReadOnlyDataGridView("sys_group_language_resource.csv", this.systemGroupDataGridView, ref signalTableMetaDataTmp);
            loadReadOnlyDataGridView("sys_enum_language_resource.csv", this.systemEnumDataGridView, ref signalTableMetaDataTmp);
            loadReadOnlyDataGridView("sys_sig_info_basic.csv", this.systemBasicDataGridView, ref signalTableMetaDataTmp);
            prefix2SystemTableMetaData[PREFIX_SIGNAL_SYSTEM_BASIC] = signalTableMetaDataTmp;
            signalTableMetaDataTmp.recordNum = 0;
            loadReadOnlyDataGridView("sys_sig_info_basic_language_resource.csv", this.systemLangDataGridView, ref signalTableMetaDataTmp);
            loadReadOnlyDataGridView("sys_sig_info_temp_humidity.csv", this.systemTempHumDataGridView, ref signalTableMetaDataTmp);
            prefix2SystemTableMetaData[PREFIX_SIGNAL_SYSTEM_TEMP_HUM] = signalTableMetaDataTmp;
            signalTableMetaDataTmp.recordNum = 0;
            loadReadOnlyDataGridView("sys_sig_info_temp_humidity_language_resource.csv", this.systemLangDataGridView, ref signalTableMetaDataTmp);

            if (!loadSystemSignalInfo())
            {
                MessageBox.Show("Error: System signal table error");
            }

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

            cusSigNameLang = new string[0];
            cusSigUnitLang = new string[0];
            cusSigGroupLang = new string[0];
            cusSigEnumLang = new string[0];

            bpSysSigMaps = new List<BPLibApi.BP_SysSigMap>();

            /* system signal info */
            systemSignalEnableBits = new List<byte[]>();
            for (int i = 0; i < BPLibApi.SYSTEM_SIGNAL_TABLE_NUM; i++)
            {
                systemSignalEnableBits.Add(new byte[64]);
            }
            /* 0xE000(SerialNumber) and 0xE001(CommunicationState) enabled as default */
            systemSignalEnableBits[0][0] = 0x03;

            foreach (DataGridView value in prefix2SignalDataGridView.Values)
            {
                value.AllowUserToAddRows = false;
            }

            foreach (DataGridView value in prefix2LangDataGridView.Values)
            {
                value.AllowUserToAddRows = false;
            }

            /* set default generate path to current directory */
            generatePathTextBox.Text = System.IO.Directory.GetCurrentDirectory();

            initSignalBlockTools();

        }

        private void loadReadOnlyDataGridView(string csvName, DataGridView dataGridView, ref Tools.SignalTableMetaData signalTableMetaData)
        {

            // dataGridView.Rows.Clear();
            try
            {
                UTF8Encoding uTF8Encoding = new System.Text.UTF8Encoding(true);
                using (StreamReader sr = new StreamReader(csvName, uTF8Encoding))
                {
                    string line;

                    string tmp = sr.ReadLine();
                    signalTableMetaData.version = Tools.signalTableInfoParser("Version", tmp);
                    try
                    {
                        signalTableMetaData.recordNum = int.Parse(Tools.signalTableInfoParser("RecordNum", tmp));
                    }
                    catch (Exception e)
                    {

                    }
                    tmp = sr.ReadLine(); // skip the title line

                    int recordNum = signalTableMetaData.recordNum;
                    int rowIndex = dataGridView.Rows.Count - 1;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (signalTableMetaData.recordNum > 0)
                        {
                            if (recordNum-- <= 0)
                            {
                                break;
                            }
                        }
                        dataGridView.Rows.Add();
                        string[] stringArray = line.Split(',');
                        for (int i = 0; i < stringArray.Length; i++)
                        {
                            if (rowIndex % 2 == 0)
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

        private void systemSignalCellValueChangedCallback(string prefix, object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }
            if (!prefix2SignalDataGridView.ContainsKey(prefix))
            {
                return;
            }
            if (null == prefix2systemSignalDataItemConstList || !prefix2systemSignalDataItemConstList.ContainsKey(prefix))
            {
                return;
            }

            DataGridView dataGridView = prefix2SignalDataGridView[prefix];
            if (e.RowIndex >= dataGridView.Rows.Count || e.ColumnIndex >= dataGridView.Columns.Count)
            {
                return;
            }
            List<SignalDataItem> signalDataItems = prefix2systemSignalDataItemConstList[prefix];

            if (null == signalDataItems || signalDataItems.Count <= e.RowIndex)
            {
                return;
            }
            SignalDataItem signalDataItem = signalDataItems[e.RowIndex];

            try
            {
                
                if(ENABLE_COLUMN_INDEX == e.ColumnIndex)
                {
                    /* update enable flag */
                    string tmp = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Trim();
                    if (!SignalDataItem.yesOrNoTable.ContainsKey(tmp))
                    {
                        /* TODO: error */
                        // return signalDataItemRet;
                        return;
                    }
                    signalDataItem.Enabled = SignalDataItem.yesOrNoTable[tmp];
                    Tools.setSysSignalEnableBits(ref systemSignalEnableBits, (UInt16)(signalDataItem.SignalId & 0xFFFF), signalDataItem.Enabled);
                }
                else
                {
                    object originalValue = signalDataItem[e.ColumnIndex];
                    if (null == originalValue || !dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Trim().Equals(originalValue.ToString()))
                    {
                        /* set system signal custom info mask */
                        signalDataItem.CustomInfo |= SignalDataItem.parseCustomInfoMask(e.ColumnIndex);
                    }
                }

                Console.WriteLine("(" + e.RowIndex + "," + e.ColumnIndex + ")=" + dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + "(" + signalDataItem[e.ColumnIndex] + ")");
            }
            catch (Exception ex)
            {

            }
        }


        private Boolean loadSystemSignalInfo()
        {
            prefix2systemSignalDataItemConstList = new Dictionary<string, List<SignalDataItem>>();
            prefix2systemSignalDataItemConstList.Clear();
            Boolean ret = false;
            string err = "";
            try
            {
                foreach (string prefix in prefix2SignalDataGridView.Keys)
                {
                    List<SignalDataItem> systemSignalDataItemConstListTmp = new List<SignalDataItem>();
                    DataGridView dataGridViewTmp = prefix2SignalDataGridView[prefix];
                    int size = dataGridViewTmp.Rows.Count;
                    if (prefix2SystemTableMetaData.ContainsKey(prefix))
                    {
                        size = Math.Min(size, prefix2SystemTableMetaData[prefix].recordNum);
                    }
                      
                    for (int i = 0; i < size; i++)
                    {
                        err = "";
                        SignalDataItem tmp = SignalDataItem.parseSignalDataItem(dataGridViewTmp.Rows[i].Cells, prefix, false, ref err);
                        if (tmp != null)
                        {
                            err = "line " + i + ", ";
                            systemSignalDataItemConstListTmp.Add(tmp);
                        }
                        else if (!string.IsNullOrWhiteSpace(err))
                        {
                            err = "line " + i + ", " + err;
                            Console.WriteLine(err);
                            return ret;
                        }

                    }
                    prefix2systemSignalDataItemConstList[prefix] = systemSignalDataItemConstListTmp;
                }
                ret = true;
            } 
            catch(Exception e)
            {
                ret = false;
            }
            return ret;
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

                        loadSimData(signalDataItemList, systemSignalCustomInfo, new List<SignalDataItem>());
                        // loadSimData(signalDataItemList, systemSignalCustomInfo, null);


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
                        SignalDataItem tmp = SignalDataItem.parseSignalDataItem(dataGridViewTmp.Rows[i].Cells, prefix, false, ref err);
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
            /* free system signal info */
            Tools.freeIntPtr(sysSigId2ValIntPtr);

            if(null != sysSigTable)
            {
                for (int i = 0; i < sysSigTable.Count<BPLibApi.BP_SigTable>(); i++)
                {
                    Tools.freeIntPtr(sysSigTable[i].MinVal);
                    Tools.freeIntPtr(sysSigTable[i].MaxVal);
                    Tools.freeIntPtr(sysSigTable[i].DefVal);
                }
            }
            Tools.freeIntPtr(sysSigTableIntPtr);

            if(null != bpSysSigMaps)
            {
                for (int i = 0; i < bpSysSigMaps.Count; i++)
                {
                    Tools.freeIntPtr(bpSysSigMaps[i].SigMap);
                }
            }
            Tools.freeIntPtr(bpSysSigMapsIntPtr);

            /* free custom system signal info */
            if(null != bpSysCusUnitTable)
            {
                for (int i = 0; i < bpSysCusUnitTable.Count<BPLibApi.BP_SysCustomUnit>(); i++)
                {
                    Tools.freeIntPtr(bpSysCusUnitTable[i].CustomValue);
                }
            }
            Tools.freeIntPtr(bpSysCusUnitTableIntPtr);

            /* free custom signal info */
            Tools.freeIntPtr(cusSigId2ValIntPtr);

            if(null != cusSigTable)
            {
                for (int i = 0; i < cusSigTable.Count<BPLibApi.BP_SigTable>(); i++)
                {
                    Tools.freeIntPtr(cusSigTable[i].MinVal);
                    Tools.freeIntPtr(cusSigTable[i].MaxVal);
                    Tools.freeIntPtr(cusSigTable[i].DefVal);
                }
            }
            Tools.freeIntPtr(cusSigTableIntPtr);

            Tools.freeLangIntPtr(cusSigNameLangIntPtr, cusSigNameLangSize);
            Tools.freeLangIntPtr(cusSigUnitLangIntPtr, cusSigUnitLangSize);
            Tools.freeLangIntPtr(cusSigGroupLangIntPtr, cusSigGroupLangSize);
            Tools.freeLangIntPtr(cusSigEnumLangIntPtr, cusSigEnumLangSize);

            Tools.freeIntPtr(cusSigNameLangMapIntPtr);
            Tools.freeIntPtr(cusSigUnitLangMapIntPtr);
            Tools.freeIntPtr(cusSigGroupLangMapIntPtr);

            if(null != cusSigEnumLangMap)
            {
                for (BP_WORD i = 0; i < cusSigEnumLangMapSize; i++)
                {
                    Tools.freeIntPtr(cusSigEnumLangMap[i].EnumSignalMap);
                }
            }
            Tools.freeIntPtr(cusSigEnumLangMapIntPtr);

            /* system signal info */
            systemSignalEnableBits = new List<byte[]>();
            for (int i = 0; i < BPLibApi.SYSTEM_SIGNAL_TABLE_NUM; i++)
            {
                systemSignalEnableBits.Add(new byte[64]);
            }
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
            sysSigTableIntPtr = Tools.mallocIntPtr(sysSigTable);

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
            List<BPLibApi.BP_SysCustomUnit> bpSysCusUnitTableList = new List<BPLibApi.BP_SysCustomUnit>();
            int sizeTmp = sysSignalCustomInfo.Count;
            bpSysCusUnitTableNum = 0;
            foreach (UInt16 key in sysSignalCustomInfo.Keys)
            {
                Dictionary<char, Object> dicTmp = sysSignalCustomInfo[key];
                foreach(char key2 in dicTmp.Keys)
                {
                    object customValueTmp = dicTmp[key2];
                    BPLibApi.BP_SysCustomUnit bpSysCusUnitTableTmp = new BPLibApi.BP_SysCustomUnit();
                    bpSysCusUnitTableTmp.SidId = key;
                    bpSysCusUnitTableTmp.CustomType = key2;
                    bpSysCusUnitTableTmp.CustomValue = Tools.mallocIntPtr(key, key2, customValueTmp); // IntPtr
                    bpSysCusUnitTableNum++;
                    bpSysCusUnitTableList.Add(bpSysCusUnitTableTmp);
                }
            }
            bpSysCusUnitTable = bpSysCusUnitTableList.ToArray();


            /* custom signal set new value */
            int cusSignlaSize = cusSignalDataItemList.Count;
            cusSigId2Val = new BPLibApi.BP_SigId2Val[cusSignlaSize];
            cusSigTable = new BPLibApi.BP_SigTable[cusSignlaSize];
            for (int i = 0; i < cusSignlaSize; i++)
            {
                SignalDataItem sdiTmp = cusSignalDataItemList[i];
                Tools.setDefaultValue(ref cusSigId2Val[i], sdiTmp);
                Tools.setSigTable(ref cusSigTable[i], sdiTmp);
            }

            cusSigId2ValIntPtr = Tools.mallocIntPtr(cusSigId2Val);
            cusSigId2ValSize = cusSignlaSize;
            cusSigTableIntPtr = Tools.mallocIntPtr(cusSigTable);

            // private IntPtr cusSigNameLangIntPtr;
            // private UInt64 cusSigNameLangSize;
            /*
            cusSigNameLang = new string[6];
            cusSigNameLang[0] = "";
            cusSigNameLang[1] = "";
            cusSigNameLang[2] = "";
            cusSigNameLang[3] = "";
            cusSigNameLang[4] = "light";
            cusSigNameLang[5] = "灯";
            */
            cusSigNameLangIntPtr = Tools.mallocIntPtr(cusSigNameLang);
            cusSigNameLangSize = cusSigNameLang.Length;
            cusSigUnitLangIntPtr = Tools.mallocIntPtr(cusSigUnitLang);
            cusSigUnitLangSize = cusSigUnitLang.Length;
            cusSigGroupLangIntPtr = Tools.mallocIntPtr(cusSigGroupLang);
            cusSigGroupLangSize = cusSigGroupLang.Length;
            cusSigEnumLangIntPtr = Tools.mallocIntPtr(cusSigEnumLang);
            cusSigEnumLangSize = cusSigEnumLang.Length;

            /*
            cusSigNameLangMap = new BPLibApi.BP_CusLangMap[2];
            cusSigNameLangMap[0].SigId = 0x0000;
            cusSigNameLangMap[0].LangId = 1;
            cusSigNameLangMap[1].SigId = 0x0001;
            cusSigNameLangMap[1].LangId = 2;
            */
            cusSigNameLangMapIntPtr = Tools.mallocIntPtr(cusSigNameLangMap);
            if(IntPtr.Zero == cusSigNameLangMapIntPtr)
            {
                cusSigNameLangMapSize = 0;
            }
            else
            {
                cusSigNameLangMapSize = cusSigNameLangMap.Count<BPLibApi.BP_CusLangMap>();
            }
            
            cusSigUnitLangMapIntPtr = Tools.mallocIntPtr(cusSigUnitLangMap);
            if (IntPtr.Zero == cusSigUnitLangMapIntPtr)
            {
                cusSigUnitLangMapSize = 0;
            }
            else
            {
                cusSigUnitLangMapSize = cusSigUnitLangMap.Count<BPLibApi.BP_CusLangMap>();
            }

            cusSigGroupLangMapIntPtr = Tools.mallocIntPtr(cusSigGroupLangMap);
            if (IntPtr.Zero == cusSigGroupLangMapIntPtr)
            {
                cusSigGroupLangMapSize = 0;
            }
            else
            {
                cusSigGroupLangMapSize = cusSigGroupLangMap.Count<BPLibApi.BP_CusLangMap>();
            }
            

            // private BPLibApi.BP_CusLangMap[] cusSigEnumLangMap;
            // private IntPtr cusSigEnumLangMapIntPtr;
            // private BP_WORD cusSigEnumLangMapSize;
            /*
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
            */

            cusSigEnumLangMapIntPtr = Tools.mallocIntPtr(cusSigEnumLangMap);
            if (IntPtr.Zero == cusSigEnumLangMapIntPtr)
            {
                cusSigEnumLangMapSize = 0;
            }
            else
            {
                cusSigEnumLangMapSize = cusSigEnumLangMap.Count<BPLibApi.BP_SigId2EnumSignalMap>();
            }
            
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

        private void systemBasicDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            systemSignalCellValueChangedCallback(PREFIX_SIGNAL_SYSTEM_BASIC, sender, e);
        }

        private void systemTempHumDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            systemSignalCellValueChangedCallback(PREFIX_SIGNAL_SYSTEM_TEMP_HUM, sender, e);
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            string gererateDirectory = generatePathTextBox.Text;
            if(!Directory.Exists(gererateDirectory))
            {
                MessageBox.Show("Invalid generate path");
                return;
            }

            Dictionary<string, string> moduleFile2SrcFileDictionary = new Dictionary<string, string>();
            moduleFile2SrcFileDictionary.Add("bp_sig_table_h.mod", "bp_sig_table.h");
            moduleFile2SrcFileDictionary.Add("bp_sig_table_c.mod", "bp_sig_table.c");

            foreach(string moduleFile in moduleFile2SrcFileDictionary.Keys)
            {
                try
                {
                    UTF8Encoding uTF8Encoding = new System.Text.UTF8Encoding(true);
                    using (StreamReader sr = new StreamReader(moduleFile, uTF8Encoding))
                    {
                        using (StreamWriter sw = new StreamWriter(gererateDirectory + @"\" + moduleFile2SrcFileDictionary[moduleFile], false, uTF8Encoding))
                        {
                            string line;
                            string signalCodeBlock;
                            string blockTag;

                            while ((line = sr.ReadLine()) != null)
                            {
                                line += "\r\n";
                                Match mat = SignalTableUtil.REGEX_SIGNAL_TABLE_BLOCK_START.Match(line);
                                if (null == mat || mat.Groups.Count < 2)
                                {
                                    sw.Write(line);
                                }
                                else
                                {
                                    blockTag = mat.Groups[1].Value;
                                    Regex blockEndRegex = SignalTableUtil.makeSignalTableBlockEndRegex(blockTag);
                                    signalCodeBlock = "";
                                    while ((line = sr.ReadLine()) != null)
                                    {
                                        if (blockEndRegex.IsMatch(line))
                                        {
                                            break;
                                        }
                                        line += "\r\n";
                                        signalCodeBlock += line;
                                    }
                                    signalCodeBlock = constructCodeBlock(blockTag, signalCodeBlock);
                                    sw.Write(signalCodeBlock);

                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            /*
            try
            {
                UTF8Encoding uTF8Encoding = new System.Text.UTF8Encoding(true);
                using (StreamReader sr = new StreamReader("bp_sig_table_h.mod", uTF8Encoding))
                {
                    using (StreamWriter sw = new StreamWriter(gererateDirectory + @"\bp_sig_table.h", false, uTF8Encoding))
                    {
                        string line;
                        string signalCodeBlock;
                        string blockTag;

                        while ((line = sr.ReadLine()) != null)
                        {
                            line += "\r\n";
                            Match mat = SignalTableUtil.REGEX_SIGNAL_TABLE_BLOCK_START.Match(line);
                            if (null == mat || mat.Groups.Count < 2)
                            {
                                sw.Write(line);
                            }
                            else
                            {
                                blockTag = mat.Groups[1].Value;
                                Regex blockEndRegex = SignalTableUtil.makeSignalTableBlockEndRegex(blockTag);
                                signalCodeBlock = "";
                                while ((line = sr.ReadLine()) != null)
                                {
                                    if(blockEndRegex.IsMatch(line))
                                    {
                                        break;
                                    }
                                    line += "\r\n";
                                    signalCodeBlock += line;
                                }
                                signalCodeBlock = constructCodeBlock(blockTag, signalCodeBlock);
                                sw.Write(signalCodeBlock);
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            
            try
            {
                UTF8Encoding uTF8Encoding = new System.Text.UTF8Encoding(true);
                using (StreamReader sr = new StreamReader("bp_sig_table_c.mod", uTF8Encoding))
                {
                    using (StreamWriter sw = new StreamWriter(gererateDirectory + @"\bp_sig_table.c", false, uTF8Encoding))
                    {
                        string line;
                        string signalCodeBlock;
                        string blockTag;

                        while ((line = sr.ReadLine()) != null)
                        {
                            line += "\r\n";
                            Match mat = SignalTableUtil.REGEX_SIGNAL_TABLE_BLOCK_START.Match(line);
                            if (null == mat || mat.Groups.Count < 2)
                            {
                                sw.Write(line);
                            }
                            else
                            {
                                blockTag = mat.Groups[1].Value;
                                Regex blockEndRegex = SignalTableUtil.makeSignalTableBlockEndRegex(blockTag);
                                signalCodeBlock = "";
                                while ((line = sr.ReadLine()) != null)
                                {
                                    if (blockEndRegex.IsMatch(line))
                                    {
                                        break;
                                    }
                                    line += "\r\n";
                                    signalCodeBlock += line;
                                }
                                signalCodeBlock = constructCodeBlock(blockTag, signalCodeBlock);
                                sw.Write(signalCodeBlock);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            */

            MessageBox.Show("Generation Done");
        }

        private void generatePathTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void generatePathBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = @"Target directory";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                string sPath = folder.SelectedPath;
                generatePathTextBox.Text = sPath;
                MessageBox.Show(sPath);
            }
        }
    }
}
