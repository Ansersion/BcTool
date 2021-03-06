﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BcTool
{

    public class CellErrorInfo
    {
        public Color oldColor;
        public Color currentColor;
        public Boolean isError;
        public string errorInfo;

        public CellErrorInfo()
        {
            oldColor = ColorMng.NULL_COLOR;
            currentColor = ColorMng.NULL_COLOR;
            isError = false;
            errorInfo = "";
        }
    }

    class ColorMng
    {

        public static Color WARNING_COLOR = Color.Red;
        public static Color ODD_COLOR = SystemColors.Info;
        public static Color EVEN_COLOR = SystemColors.ControlLightLight;
        public static Color READ_ONLY_COLOR = System.Drawing.SystemColors.Control;
        public static Color NULL_COLOR = Color.FromArgb(1, 2, 3, 4);


        // private Dictionary<int, Dictionary<int, Color>> gridOldColorMap;
        private Dictionary<int, Dictionary<int, CellErrorInfo>> cellErrorInfoMap;

        public ColorMng()
        {
            this.cellErrorInfoMap = new Dictionary<int, Dictionary<int, CellErrorInfo>>();
        }


        public Color clearErrorInfor(ref DataGridView dataGridView, int rowIndex, int columnIndex)
        {
            Color ret = NULL_COLOR;

            if (cellErrorInfoMap.ContainsKey(rowIndex))
            {
                if(cellErrorInfoMap[rowIndex].ContainsKey(columnIndex))
                {
                    CellErrorInfo tmp = cellErrorInfoMap[rowIndex][columnIndex];
                    tmp.isError = false;
                    tmp.currentColor = tmp.oldColor;
                    ret = tmp.currentColor;
                    try
                    {
                        DataGridViewCell cell =
                            dataGridView.Rows[rowIndex].Cells[columnIndex];
                        cell.ToolTipText = "";
                    }
                    catch(Exception e)
                    {

                    }
                }
            }

            return ret;
        }

        public void putErrorInfo(int rowIndex, int columnIndex, Color oldColor, Color errorColor)
        {
    
            if (!cellErrorInfoMap.ContainsKey(rowIndex))
            {
                cellErrorInfoMap.Add(rowIndex, new Dictionary<int, CellErrorInfo>());
            }

            CellErrorInfo tmp;
            if (!cellErrorInfoMap[rowIndex].ContainsKey(columnIndex))
            {
                tmp = new CellErrorInfo();
                tmp.oldColor = oldColor;
                cellErrorInfoMap[rowIndex].Add(columnIndex, tmp);
            }
            tmp = cellErrorInfoMap[rowIndex][columnIndex];
            // tmp.oldColor = oldColor; old color only initialized when CellErrorInfo is created
            tmp.currentColor = errorColor;
            tmp.isError = true;
        }

        public void putColor(int rowIndex, int columnIndex, Color oldColor, Color newColor)
        {

            if (!cellErrorInfoMap.ContainsKey(rowIndex))
            {
                cellErrorInfoMap.Add(rowIndex, new Dictionary<int, CellErrorInfo>());
            }

            CellErrorInfo tmp;
            if (!cellErrorInfoMap[rowIndex].ContainsKey(columnIndex))
            {
                tmp = new CellErrorInfo();
                tmp.oldColor = oldColor;
                cellErrorInfoMap[rowIndex].Add(columnIndex, tmp);
            }
            tmp = cellErrorInfoMap[rowIndex][columnIndex];
            // tmp.oldColor = oldColor; old color only initialized when CellErrorInfo is created
            tmp.currentColor = newColor;
        }

        public Color getColor(int rowIndex, int columnIndex)
        {
            if (!cellErrorInfoMap.ContainsKey(rowIndex))
            {
                cellErrorInfoMap.Add(rowIndex, new Dictionary<int, CellErrorInfo>());
            }

            CellErrorInfo tmp;
            if (!cellErrorInfoMap[rowIndex].ContainsKey(columnIndex))
            {
                tmp = new CellErrorInfo();
                tmp.oldColor = ColorMng.NULL_COLOR;
                tmp.currentColor = ColorMng.NULL_COLOR;
                cellErrorInfoMap[rowIndex].Add(columnIndex, tmp);
            }
            tmp = cellErrorInfoMap[rowIndex][columnIndex];
            // tmp.oldColor = oldColor; old color only initialized when CellErrorInfo is created
            return tmp.currentColor;
        }

        public CellErrorInfo getErrorInfo(int rowIndex, int columnIndex)
        {
            CellErrorInfo ret = null;
            if (cellErrorInfoMap.ContainsKey(rowIndex))
            {
                if(cellErrorInfoMap[rowIndex].ContainsKey(columnIndex))
                {
                    ret = cellErrorInfoMap[rowIndex][columnIndex];
                }
            }
            return ret;
            
        }

        public static void setDataGridViewLineColor(ref DataGridView dataGridView, int rowIndex)
        {
            if(rowIndex < 0)
            {
                return;
            }
            if(rowIndex >= dataGridView.Rows.Count)
            {
                return;
            }

            if (rowIndex % 2 == 0)
            {
                dataGridView.Rows[rowIndex].DefaultCellStyle.BackColor = ColorMng.EVEN_COLOR;
            }
            else
            {
                dataGridView.Rows[rowIndex].DefaultCellStyle.BackColor = ColorMng.ODD_COLOR;
            }
        }

        public static void setDataGridViewColumnColor(ref DataGridView dataGridView, int columnIndex, Color color)
        {
            if (columnIndex < 0)
            {
                return;
            }
            if (columnIndex >= dataGridView.Columns.Count)
            {
                return;
            }

            if (columnIndex % 2 == 0)
            {
                dataGridView.Columns[columnIndex].DefaultCellStyle.BackColor = ColorMng.EVEN_COLOR;
            }
            else
            {
                dataGridView.Columns[columnIndex].DefaultCellStyle.BackColor = ColorMng.ODD_COLOR;
            }
        }

        public static void setDataGridViewColor(ref DataGridView dataGridView, int rowIndex, int columnIndex)
        {

        }
    }
}
