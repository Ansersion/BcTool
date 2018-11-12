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

        public const string GRIDVIEW_SIGNAL_ID = "SignalID";
        public const string GRIDVIEW_TYPE = "Type";
        public const string GRIDVIEW_VALUE = "Value";
        public const string GRIDVIEW_ALARM = "Alarm";

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

        private int signalId;
        private Boolean enabled;
        private String macro;
        private Boolean alarm;
        private ValueType valueType;
        private int unitLangId;
        private BcPermission bcPermission;
        private Boolean display;
        private int accuracy;
        private Object minValue;
        private Object maxValue;
        private Object defaultValue;
        private int groupLangId;
        private Hashtable enumLangIdTable;
        private Boolean statistics;
        private int alarmClass;
        private int alarmBefDelay;
        private int alarmAftDelay;

        public int SignalId { get => signalId; set => signalId = value; }
        public bool Enabled { get => enabled; set => enabled = value; }
        public string Macro { get => macro; set => macro = value; }
        public bool Alarm { get => alarm; set => alarm = value; }
        internal ValueType ValueType1 { get => valueType; set => valueType = value; }
        public int UnitLangId { get => unitLangId; set => unitLangId = value; }
        internal BcPermission BcPermission1 { get => bcPermission; set => bcPermission = value; }
        public bool Display { get => display; set => display = value; }
        public int Accuracy { get => accuracy; set => accuracy = value; }
        public object MinValue { get => minValue; set => minValue = value; }
        public object MaxValue { get => maxValue; set => maxValue = value; }
        public object DefaultValue { get => defaultValue; set => defaultValue = value; }
        public int GroupLangId { get => groupLangId; set => groupLangId = value; }
        public Hashtable EnumLangIdTable { get => enumLangIdTable; set => enumLangIdTable = value; }
        public bool Statistics { get => statistics; set => statistics = value; }
        public int AlarmClass { get => alarmClass; set => alarmClass = value; }
        public int AlarmBefDelay { get => alarmBefDelay; set => alarmBefDelay = value; }
        public int AlarmAftDelay { get => alarmAftDelay; set => alarmAftDelay = value; }

        public SignalDataItem()
        {
        }

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
            this.AlarmAftDelay = alarmAftDelay;
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
