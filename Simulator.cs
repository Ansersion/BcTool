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
        private Hashtable signalDataItemTable;

        public Simulator()
        {
            InitializeComponent();
        }

        public void setSignalDataItemTable(Hashtable hashtable)
        {
            signalDataItemTable = hashtable;
        }

        public void reloadSignalTable()
        {
            int size = dataGridViewSignalTable.Rows.Count;
            for(int i = 0; i < size; i++)
            {
                dataGridViewSignalTable.Rows.RemoveAt(i);
            }
            if(signalDataItemTable != null)
            {
                foreach (SignalDataItem value in signalDataItemTable.Values)
                {
                    dataGridViewSignalTable.Rows.Add();
                    dataGridViewSignalTable.Rows[dataGridViewSignalTable.Rows.Count - 1].Cells[0].Value = value.getSignalIdString();
                    dataGridViewSignalTable.Rows[dataGridViewSignalTable.Rows.Count - 1].Cells[1].Value = value.getValueTypeString();
                    dataGridViewSignalTable.Rows[dataGridViewSignalTable.Rows.Count - 1].Cells[2].Value = value.getDefaultString();
                }
            }
            
        }
    }
}
