using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcTool
{
    class ColorMng
    {

        public static Color WARNING_COLOR = Color.Red;
        public static Color NULL_COLOR = Color.FromArgb(1, 2, 3, 4);

        class CellErrorInfo
        {
            public Color oldColor;
            public Color currentColor;
            public Boolean isError;

            public CellErrorInfo()
            {
                oldColor = NULL_COLOR;
                currentColor = NULL_COLOR;
                isError = false;
            }
        }


        // private Dictionary<int, Dictionary<int, Color>> gridOldColorMap;
        private Dictionary<int, Dictionary<int, CellErrorInfo>> cellErrorInfoMap;

        public ColorMng()
        {
            this.cellErrorInfoMap = new Dictionary<int, Dictionary<int, CellErrorInfo>>();
        }


        public Color clearErrorInfor(int rowIndex, int columnIndex)
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
    }
}
