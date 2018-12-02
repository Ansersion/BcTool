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

        private int[] systemLanguageTabIndex = { 4, 5, 6 };

        private NetMng netMng = new NetMng("127.0.0.1");
        private Simulator simulator;
        private List<DataGridView> dist2DataGridViewList;
        private List<Hashtable> dist2SignalDataItemHashTable;
        private Dictionary<String, DataGridView> prefix2SignalDataGridView;
        private Dictionary<String, DataGridView> prefix2LangDataGridView;
        private Dictionary<string, Dictionary<UInt16, LanguageResourceItem>> prefix2LangDictionary;

        private BPLibApi.BP_SigId2Val[] sysSigId2Val;
        private UInt64 sysSigId2ValSize;
        private BPLibApi.BP_SigTable[] sysSigTable;

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

                        /* TODO: set signal table */
                        sysSigId2Val = new BPLibApi.BP_SigId2Val[2];
                        sysSigId2Val[0].SigId = 0x5a5a;
                        sysSigId2Val[0].SigVal.t_u32 = 0x5a5a5a5a;
                        sysSigId2Val[1].SigId = 0xa5a5;
                        sysSigId2Val[1].SigVal.t_u32 = 0xa5a5a5a5;
                        Tools.mallocIntPtr(sysSigId2Val);
                        sysSigId2ValSize = 2;
                        /* BP_SetSysSigId2ValTable */

                        sysSigTable = new BPLibApi.BP_SigTable[2];
                        sysSigTable[0].SigId = 0x5a5a;
                        sysSigTable[0].SigType = 3;
                        sysSigTable[0].IsDisplay = 1;
                        sysSigTable[1].SigId = 0xa5a5;
                        sysSigTable[1].SigType = 5;
                        sysSigTable[1].IsDisplay = 0;
                        Tools.mallocIntPtr(sysSigTable);

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
