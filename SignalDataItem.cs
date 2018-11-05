using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcTool
{
    class SignalDataItem
    {
        public enum ValueType
        {
            U32, 
            U16,
            I32,
            I16,
            ENUM,
            FLOAT,
            STRING,
            BOOLEAN,
            MEM,
        }

        public enum BcPermission
        {
            RO,
            RW,
        }

        public int signalId;
        public Boolean enabled;
        public String macro;
        public Boolean alarm;
        public ValueType valueType;
        public int unitLangId;
        public BcPermission bcPermission;
        public Boolean display;
        public int accuracy;
        public Object minValue;
        public Object maxValue;
        public Object defaultValue;
        public int groupLangId;
        public Hashtable enumLangIdTable;
        public Boolean statistics;
        public int alarmClass;
        public int alarmBefDelay;
        public int alarmAftDelay;

        public SignalDataItem(int signalId, bool enabled, string macro, bool alarm, ValueType valueType, int unitLangId, BcPermission bcPermission, bool display, int accuracy, object minValue, object maxValue, object defaultValue, int groupLangId, Hashtable enumLangIdTable, bool statistics, int alarmClass, int alarmBefDelay, int alarmAftDelay)
        {
            this.signalId = signalId;
            this.enabled = enabled;
            this.macro = macro;
            this.alarm = alarm;
            this.valueType = valueType;
            this.unitLangId = unitLangId;
            this.bcPermission = bcPermission;
            this.display = display;
            this.accuracy = accuracy;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.defaultValue = defaultValue;
            this.groupLangId = groupLangId;
            this.enumLangIdTable = enumLangIdTable;
            this.statistics = statistics;
            this.alarmClass = alarmClass;
            this.alarmBefDelay = alarmBefDelay;
            this.alarmAftDelay = alarmAftDelay;
        }

        public String getSignalIdString()
        {
            return "" + signalId;
        }

        public String getValueTypeString()
        {
            String ret;
            switch(valueType)
            {
                case ValueType.U32:
                    ret = "U32";
                    break;
                case ValueType.U16:
                    ret = "U16";
                    break;
                case ValueType.I32:
                    ret = "I32";
                    break;
                case ValueType.I16:
                    ret = "I16";
                    break;
                case ValueType.ENUM:
                    ret = "ENUM";
                    break;
                case ValueType.FLOAT:
                    ret = "FLOAT";
                    break;
                case ValueType.STRING:
                    ret = "STRING";
                    break;
                case ValueType.BOOLEAN:
                    ret = "BOOLEAN";
                    break;
                case ValueType.MEM:
                    ret = "MEM";
                    break;
                default:
                    ret = "";
                    break;
            }
            return ret;
        }

        public String getDefaultString()
        {
            String ret = "";
            if(defaultValue != null)
            {
                ret = defaultValue.ToString();
            }
            return ret;
        }

    }
}
