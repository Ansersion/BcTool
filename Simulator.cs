using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BcTool
{
    public partial class Simulator : Form
    {
        private const int LOG_LEVEL_TRACE = 4;
        private const int LOG_LEVEL_DEBUG = 3;
        private const int LOG_LEVEL_INFO = 2;
        private const int LOG_LEVEL_ERROR = 1;

        private int CURRENT_LOG_LEVEL = LOG_LEVEL_DEBUG;

        private List<SignalDataItem> signalDataItems;

        public Simulator()
        {
            InitializeComponent();
        }

        public void setSignalDataItemList(List<SignalDataItem> list)
        {
            signalDataItems = list;
        }

        public void reloadSignalTable()
        {
            int size = dataGridViewSignalTable.Rows.Count;

                dataGridViewSignalTable.Rows.Clear();
       
            if(signalDataItems != null)
            {
                foreach (SignalDataItem value in signalDataItems)
                {
                    dataGridViewSignalTable.Rows.Add();
                    dataGridViewSignalTable.Rows[dataGridViewSignalTable.Rows.Count - 1].Cells[SignalDataItem.GRIDVIEW_SIGNAL_ID].Value = value.getSignalIdString();
                    dataGridViewSignalTable.Rows[dataGridViewSignalTable.Rows.Count - 1].Cells[SignalDataItem.GRIDVIEW_TYPE].Value = value.getValueTypeString();
                    dataGridViewSignalTable.Rows[dataGridViewSignalTable.Rows.Count - 1].Cells[SignalDataItem.GRIDVIEW_VALUE].Value = value.getDefaultString();
                }
            }
            
        }

        private void dataGridViewSignalTable_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (false == dataGridViewSignalTable.CurrentCell is DataGridViewCheckBoxCell)
            {
                int column = e.ColumnIndex;
                int row = e.RowIndex;

                MessageBox.Show("(" + row + "," + column + ")=" + dataGridViewSignalTable.Rows[row].Cells[column].Value.ToString());

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

        public void startSim()
        {
            LogLn(LOG_LEVEL_INFO, "start simulate");
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
    }
}
