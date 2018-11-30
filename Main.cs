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
                    simulator.setSignalDataItemList(signalDataItemList);
                    simulator.setSignalNameLangTable(prefix2LangDictionary[PREFIX_LANG_SYSTEM_SIGNAL]);
                    simulator.setLanguageKey(LanguageResourceItem.ENGLISH_KEY);
                    simulator.Show();
                    simulator.reloadSignalTable();
                    simulator.Sn = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX3";
                    simulator.Password = "3333333333333333333333333333333333333333333333333333333333333333";
                    simulator.startSim();
                }
                else
                {
                    MessageBox.Show("TODO: something wrong");
                }
            }

        }

        private void buttonStopSim_Click(object sender, EventArgs e)
        {

            if (null != simulator && !simulator.IsDisposed)
            {
                simulator.stopSim();
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
    }
}
