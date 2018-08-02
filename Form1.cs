using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BcTool
{
    public partial class BcTool : Form
    {
        private const string MESSAGE_TAG_INVALID_OPERATION = "Invalid operation:";
        private const string MESSAGE_TAG_SPECIAL_CHARACTER = "Special character(\",\\n) error:";
        private const string MESSAGE_TAG_INFO = "Info:";

        private int[] systemLanguageTabIndex = { 4, 5, 6 };
        private int x = 0;
        // private const int tabPageNum = 10;
        private Dictionary<TabPage, int> tabPage2IndexDic = new Dictionary<TabPage, int>();
        private Dictionary<int, DataGridView> index2DataGridViewDic = new Dictionary<int, DataGridView>();
        private Dictionary<int, bool> index2isCustomDic = new Dictionary<int, bool>();
        private Dictionary<int, string> index2FileNameDic = new Dictionary<int, string>();


        public BcTool()
        {
            InitializeComponent();
            myInit();
        }

        private void myInit()
        {
            /* load system signal info */
            loadReadOnlyDataGridView("sys_signal_language_resource.csv", this.systemLangDataGridView, 1);
            loadReadOnlyDataGridView("sys_unit_language_resource.csv", this.systemUnitDataGridView, 1);
            loadReadOnlyDataGridView("sys_enum_language_resource.csv", this.systemEnumDataGridView, 2);
            loadReadOnlyDataGridView("sys_sig_info_basic.csv", this.systemBasicDataGridView, 2);

            /* initialize progress bar */
            progressBar1.Value = 0;

            /* initialize the TabPage to index dictionary */
            tabPage2IndexDic.Add(customSignalLangTabPage, 0);
            tabPage2IndexDic.Add(customEnumTabPage, 1);
            tabPage2IndexDic.Add(customUnitTabPage, 2);
            tabPage2IndexDic.Add(customGroupTabPage, 3);
            tabPage2IndexDic.Add(customTabPage, 4);
            tabPage2IndexDic.Add(systemLangTabPage, 5);
            tabPage2IndexDic.Add(systemEnumTabPage, 6);
            tabPage2IndexDic.Add(systemUnitTabPage, 7);
            tabPage2IndexDic.Add(systemGroupTabPage, 8);
            tabPage2IndexDic.Add(basicTabPage, 9);

            /* initialize the index to which is custom tab */
            index2isCustomDic.Add(0, true);
            index2isCustomDic.Add(1, true);
            index2isCustomDic.Add(2, true);
            index2isCustomDic.Add(3, true);
            index2isCustomDic.Add(4, true);
            index2isCustomDic.Add(5, false);
            index2isCustomDic.Add(6, false);
            index2isCustomDic.Add(7, false);
            index2isCustomDic.Add(8, false);
            index2isCustomDic.Add(9, false);

            /* initialize the index to DataGridView dictionary */
            index2DataGridViewDic.Add(0, customLangDataGridView);
            index2DataGridViewDic.Add(1, customEnumDataGridView);
            index2DataGridViewDic.Add(2, customUnitDataGridView);
            index2DataGridViewDic.Add(3, customGroupDataGridView);
            index2DataGridViewDic.Add(4, customDataGridView);
            index2DataGridViewDic.Add(5, systemLangDataGridView);
            index2DataGridViewDic.Add(6, systemEnumDataGridView);
            index2DataGridViewDic.Add(7, systemUnitDataGridView);
            index2DataGridViewDic.Add(8, systemGroupDataGridView);
            index2DataGridViewDic.Add(9, systemBasicDataGridView);

            /* initialize the index to csv file name */
            index2FileNameDic.Add(0, "customLang.csv");
            index2FileNameDic.Add(1, "customEnum.csv");
            index2FileNameDic.Add(2, "customUnit.csv");
            index2FileNameDic.Add(3, "customGroup.csv");
            index2FileNameDic.Add(4, "custom.csv");
            index2FileNameDic.Add(5, "systemLang.csv");
            index2FileNameDic.Add(6, "systemEnum.csv");
            index2FileNameDic.Add(7, "systemUnit.csv");
            index2FileNameDic.Add(8, "systemGroup.csv");
            index2FileNameDic.Add(9, "systemBasic.csv");
        }

        private void appendMessage(string tag, string msg)
        {
            messageBox.AppendText("[" + DateTime.Now.ToString("T") + "]" + tag + ":" + msg + "\r\n");
        }

        private bool loadReadOnlyDataGridView(string csvName, DataGridView dataGridView, int offsetLine)
        {
            bool ret = true;
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
                        if(line.Contains("\""))
                        {
                            appendMessage(MESSAGE_TAG_SPECIAL_CHARACTER, csvName + ",line " + rowIndex + 1);
                            return false;
                        }
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
                ret = false;
            }

            return ret;
        }

        private bool saveAll(string directoryName)
        {
            bool ret = true;

            try
            {
                foreach (int key in index2FileNameDic.Keys)
                {
                    if(!saveDataGridView2File(index2DataGridViewDic[key], directoryName + "\\" + index2FileNameDic[key]))
                    {
                        ret = false;
                        break;
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


        private bool loadAll(string directoryName)
        {
            bool ret = true;

            try
            {
                foreach (int key in index2FileNameDic.Keys)
                {
                    if (!loadReadOnlyDataGridView(directoryName + "\\" + index2FileNameDic[key], index2DataGridViewDic[key], 1))
                    {
                        ret = false;
                        break;
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

        private bool saveDataGridView2File(DataGridView dataGridView, string fileName)
        {
            bool ret = true;
            UTF8Encoding uTF8Encoding = new System.Text.UTF8Encoding(true);
            string line = "";
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName, false, uTF8Encoding))
                {
                    for (int i = 0; i < dataGridView.ColumnCount; i++)
                    {
                        if (i > 0)
                        {
                            line += ",";
                        }
                        line += dataGridView.Columns[i].HeaderText;
                    }
                    sw.WriteLine(line);
                    for (int j = 0; j < dataGridView.Rows.Count; j++)
                    {
                        line = "";
                        int colCount = dataGridView.Columns.Count;
                        for (int k = 0; k < colCount; k++)
                        {
                            if (k > 0 && k < colCount)
                                line += ",";
                            if (dataGridView.Rows[j].Cells[k].Value == null)
                                line += "";
                            else
                            {
                                string cell = dataGridView.Rows[j].Cells[k].Value.ToString().Trim();
                                cell = cell.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                                if (cell.Contains(',') || cell.Contains('"')
                                    || cell.Contains('\r') || cell.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                                {
                                    cell = string.Format("\"{0}\"", cell);
                                }

                                line += cell;
                            }
                        }
                        sw.WriteLine(line);
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

        private bool loadFile2DataGridView(string fileName, DataGridView dataGridView)
        {
            bool ret = true;
            UTF8Encoding uTF8Encoding = new System.Text.UTF8Encoding(true);
            string line = "";
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName, false, uTF8Encoding))
                {
                    for (int i = 0; i < dataGridView.ColumnCount; i++)
                    {
                        if (i > 0)
                        {
                            line += ",";
                        }
                        line += dataGridView.Columns[i].HeaderText;
                    }
                    sw.WriteLine(line);
                    for (int j = 0; j < dataGridView.Rows.Count; j++)
                    {
                        line = "";
                        int colCount = dataGridView.Columns.Count;
                        for (int k = 0; k < colCount; k++)
                        {
                            if (k > 0 && k < colCount)
                                line += ",";
                            if (dataGridView.Rows[j].Cells[k].Value == null)
                                line += "";
                            else
                            {
                                string cell = dataGridView.Rows[j].Cells[k].Value.ToString().Trim();
                                cell = cell.Replace("\"", "\"\"");
                                cell = "\"" + cell + "\"";
                                line += cell;
                            }
                        }
                        sw.WriteLine(line);
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

        private void dataGridView9_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            try
            {
                if(index2isCustomDic[e.TabPageIndex])
                {
                    addLineButton.Enabled = true;
                    deleteLineButton.Enabled = true;
                }
                else
                {
                    addLineButton.Enabled = false;
                    deleteLineButton.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void addLineButton_Click(object sender, EventArgs e)
        {
            try
            {
                int newRowIndex = index2DataGridViewDic[tabControl.SelectedIndex].Rows.Count;
                if(newRowIndex > 0xFFFF)
                {
                    appendMessage(MESSAGE_TAG_INVALID_OPERATION, "Max==0xFFFF");
                    return;
                }
                index2DataGridViewDic[tabControl.SelectedIndex].Rows.Add();
                index2DataGridViewDic[tabControl.SelectedIndex].Rows[newRowIndex].Cells[0].Value = string.Format("{0:X4}", newRowIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void deleteLineButton_Click(object sender, EventArgs e)
        {
            try
            {

                int lastRowIndex = index2DataGridViewDic[tabControl.SelectedIndex].Rows.Count - 1;
                if (lastRowIndex < 0)
                {
                    appendMessage(MESSAGE_TAG_INVALID_OPERATION, "No rows");
                    return;
                }
                index2DataGridViewDic[tabControl.SelectedIndex].Rows.RemoveAt(lastRowIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                if (loadAll(folderBrowserDialog.SelectedPath) == true)
                {
                    this.Text = folderBrowserDialog.SelectedPath;
                    MessageBox.Show("加载成功");
                }
                else
                {
                    foreach (int key in index2FileNameDic.Keys)
                    {
                        index2DataGridViewDic[key].Rows.Clear();
                    }
                    loadReadOnlyDataGridView("sys_signal_language_resource.csv", this.systemLangDataGridView, 1);
                    loadReadOnlyDataGridView("sys_unit_language_resource.csv", this.systemUnitDataGridView, 1);
                    loadReadOnlyDataGridView("sys_enum_language_resource.csv", this.systemEnumDataGridView, 2);
                    loadReadOnlyDataGridView("sys_sig_info_basic.csv", this.systemBasicDataGridView, 2);
                    MessageBox.Show("加载失败");
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveAll(folderBrowserDialog.SelectedPath) == true)
                {
                    MessageBox.Show("保存成功");
                }
                else
                {
                    MessageBox.Show("保存失败");
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if(this.Text == "BcTool")
            {
                buttonSaveAs_Click(sender, e);
            }
            else
            {
                if(saveAll(this.Text))
                {
                    appendMessage(MESSAGE_TAG_INFO, "Save OK");
                }
                else
                {
                    appendMessage(MESSAGE_TAG_INVALID_OPERATION, "Save Error");
                }
            }
        }
    }
}
