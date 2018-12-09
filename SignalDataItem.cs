using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BcTool
{
    public class SignalDataItem
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

        public enum BcAlarmClass
        {
            EMERGENCY,
            SERIOUS,
            WARNING,
            NOTICE,
            INFO,
        }

        public const string GRIDVIEW_SIGNAL_ID = "SignalID";
        public const string GRIDVIEW_TYPE = "Type";
        public const string GRIDVIEW_VALUE = "Value";
        public const string GRIDVIEW_ALARM = "Alarm";
        public const string BLANK_VALUE = "";
        public const string DEFAULT_VALUE = "DEFAULT_VALUE";
        public const string ENUM_ID_REGEX_PATTERN = @"\s*\[\s*(\d+)\s*=\s*ELR(\d+)\s*\]\s*";
        public const string UNIT_ID_REGEX_PATTERN = @"ULR(\d+)\s*";
        public const string GROUP_ID_REGEX_PATTERN = @"GLR(\d+)\s*";
        public const int DEFAULT_UNIT_ID = 0;
        public const int DEFAULT_GROUP_ID = 0;
        public const BcPermission DEFAULT_PERMISSION = BcPermission.RO;
        public const BcAlarmClass DEFAULT_ALARM_CLASS = BcAlarmClass.SERIOUS;
        public const int DEFAULT_DBA = 5;
        public const int DEFAULT_DAA = 5;


        public static Dictionary<string, Boolean> yesOrNoTable = new Dictionary<string, Boolean>()
        {
            {"", false},
            {"YES", true},
            {"NO", false}
        };

        public static Dictionary<string, ValueType> valueTypeTable = new Dictionary<string, ValueType>()
        {
            {"UINT32", ValueType.U32},
            {"UINT16", ValueType.U16},
            {"INT32", ValueType.I32},
            {"INT16", ValueType.I16},
            {"ENUM", ValueType.ENUM},
            {"FLOAT", ValueType.FLOAT},
            {"STRING", ValueType.STRING},
            {"BOOLEAN", ValueType.BOOLEAN},
            {"MEM", ValueType.MEM}
        };

        public static Dictionary<string, BcPermission> permissionTable = new Dictionary<string, BcPermission>()
        {
            {"", DEFAULT_PERMISSION},
            {"RO", BcPermission.RO},
            {"RW", BcPermission.RW}
        };

        public static Dictionary<string, BcAlarmClass> alarmClassTable = new Dictionary<string, BcAlarmClass>()
        {
            {"", DEFAULT_ALARM_CLASS},
            {"EMERGENCY", BcAlarmClass.EMERGENCY},
            {"SERIOUS", BcAlarmClass.SERIOUS},
            {"WARNING", BcAlarmClass.WARNING},
            {"NOTICE", BcAlarmClass.NOTICE},
            {"INFO", BcAlarmClass.INFO}
        };

        public static Hashtable defaultSignalValues = new Hashtable()
        {
            { ValueType.U32, 0xFFFFFFFF },
            { ValueType.U16, 0xFFFF },
            { ValueType.I32, 0x7FFFFFFF },
            { ValueType.I16, 0x7FFF },
            { ValueType.ENUM, 0xFFFF },
            { ValueType.FLOAT, 0x7FFFFFFF },
            { ValueType.STRING, "" },
            { ValueType.BOOLEAN, false },
            { ValueType.MEM, null },
        };

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
        private Dictionary<UInt16, UInt32> enumLangIdTable;
        private Boolean statistics;
        private BcAlarmClass alarmClass;
        private int alarmBefDelay;
        private int alarmAftDelay;

        private Boolean hasCustomInfo;

        private static Regex enumIDRegex = new Regex(ENUM_ID_REGEX_PATTERN);
        private static Regex unitIDRegex = new Regex(UNIT_ID_REGEX_PATTERN);
        private static Regex groupIDRegex = new Regex(GROUP_ID_REGEX_PATTERN);

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
        public Dictionary<ushort, uint> EnumLangIdTable { get => enumLangIdTable; set => enumLangIdTable = value; }
        public bool Statistics { get => statistics; set => statistics = value; }
        internal BcAlarmClass AlarmClass { get => alarmClass; set => alarmClass = value; }
        public int AlarmBefDelay { get => alarmBefDelay; set => alarmBefDelay = value; }
        public int AlarmAftDelay { get => alarmAftDelay; set => alarmAftDelay = value; }
        public bool HasCustomInfo { get => hasCustomInfo; set => hasCustomInfo = value; }

        public static object parseValue(ValueType valueType, string str)
        {
            object ret = null;
            if(null == str)
            {
                return ret;
            }
            if(string.IsNullOrWhiteSpace(str))
            {
                ret = DEFAULT_VALUE;
                return ret;
            }

            try
            {
                switch(valueType)
                {
                    case ValueType.U32:
                        ret = Convert.ToUInt32(str);
                        break;
                    case ValueType.U16:
                        ret = Convert.ToUInt16(str);
                        break;
                    case ValueType.I32:
                        ret = Convert.ToInt32(str);
                        break;
                    case ValueType.I16:
                        ret = Convert.ToInt16(str);
                        break;
                    case ValueType.ENUM:
                        ret = Convert.ToUInt16(str);
                        break;
                    case ValueType.FLOAT:
                        ret = Convert.ToDouble(str);
                        break;
                    case ValueType.STRING:
                        ret = str;
                        break;
                    case ValueType.BOOLEAN:
                        ret = Convert.ToBoolean(str);
                        break;
                    case ValueType.MEM:
                        ret = DEFAULT_VALUE;
                        break;
                    default:
                        ret = null;
                        break;

                }
            }
            catch(Exception e)
            {
                ret = null;
            }
            return ret;
        }

        public static SignalDataItem parseSignalDataItem(System.Windows.Forms.DataGridViewCellCollection row, string prefix, Boolean enabledOnly, ref string err)
        {
            SignalDataItem signalDataItemRet = null;
            if (null == row)
            {
                return signalDataItemRet;
            }

            try
            {
                String tmp;

                tmp = row[prefix + "SignalID"].Value.ToString().Trim();
                int signalId = -1;
                try
                {
                    signalId = Convert.ToInt32(tmp, 16);
                }
                catch (Exception e)
                {
                    signalId = -1;
                }
                if(signalId < 0)
                {
                    if (null != err)
                    {
                        err = "<SignalID> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }

                tmp = row[prefix + "Enabled"].Value.ToString().Trim();
                if (!yesOrNoTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<Enabled> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                Boolean enabled = yesOrNoTable[tmp];

                if (enabledOnly)
                {
                    if (!enabled)
                    {
                        return signalDataItemRet;
                    }
                }

                string macro = row[prefix + "Macro"].Value.ToString().Trim();
                if(string.IsNullOrWhiteSpace(macro))
                {
                    if (null != err)
                    {
                        err = "<Macro> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }


                tmp = row[prefix + "IsAlarm"].Value.ToString().Trim();
                if (!yesOrNoTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<IsAlarm> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                Boolean isAlarm = yesOrNoTable[tmp];

                tmp = row[prefix + "ValueType"].Value.ToString().Trim();
                if (!valueTypeTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<ValueType> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                ValueType valueType = valueTypeTable[tmp];

                tmp = row[prefix + "UnitID"].Value.ToString().Trim();
                int unitId = -1;
                try
                {
                    if (string.IsNullOrWhiteSpace(tmp))
                    {
                        unitId = DEFAULT_UNIT_ID;
                    }
                    else
                    {
                        Match mat = unitIDRegex.Match(tmp);

                        if (null == mat || mat.Groups.Count < 2)
                        {
                            if (null != err)
                            {
                                err = "<UnitID> error: \"" + tmp + "\"";
                            }
                            return signalDataItemRet;
                        }
                        unitId = Convert.ToInt32(mat.Groups[1].Value);
                    }

                    
                }
                catch (Exception e)
                {
                    unitId = -1;
                }
                if (unitId < 0)
                {
                    if (null != err)
                    {
                        err = "<UnitID> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }

                tmp = row[prefix + "Permission"].Value.ToString().Trim();
                if (!permissionTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<Permission> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                BcPermission permission = permissionTable[tmp];

                tmp = row[prefix + "IsDisplay"].Value.ToString().Trim();
                if (!yesOrNoTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<IsDisplay> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                Boolean isDisplay = yesOrNoTable[tmp];

                tmp = row[prefix + "Accuracy"].Value.ToString().Trim();
                int accuracy = -1;
                try
                {
                    accuracy = Convert.ToInt32(tmp);
                }
                catch (Exception e)
                {
                    accuracy = -1;
                }
                if (accuracy < 0)
                {
                    if (null != err)
                    {
                        err = "<Accuracy> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }


                tmp = row[prefix + "Min"].Value.ToString().Trim();
                object minValue = parseValue(valueType, tmp);

                if (null == minValue)
                {
                    if (null != err)
                    {
                        err = "<Min> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }

                tmp = row[prefix + "Max"].Value.ToString().Trim();
                object maxValue = parseValue(valueType, tmp);

                if (null == maxValue)
                {
                    if (null != err)
                    {
                        err = "<Max> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }

                tmp = row[prefix + "Default"].Value.ToString().Trim();
                object defValue = parseValue(valueType, tmp);

                if (null == defValue)
                {
                    if (null != err)
                    {
                        err = "<Default> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }

                tmp = row[prefix + "GroupID"].Value.ToString().Trim();
                int groupId = -1;
                try
                {
                    if (string.IsNullOrWhiteSpace(tmp))
                    {
                        groupId = DEFAULT_GROUP_ID;
                    }
                    else
                    {

                        Match mat = groupIDRegex.Match(tmp);
                        if (null == mat || mat.Groups.Count < 2)
                        {
                            if (null != err)
                            {
                                err = "<GroupID> error: \"" + tmp + "\"";
                            }
                            return signalDataItemRet;
                        }
                        groupId = Convert.ToInt32(mat.Groups[1].Value);
                    }

                    
                }
                catch (Exception e)
                {
                    groupId = -1;
                }
                if (groupId < 0)
                {
                    if (null != err)
                    {
                        err = "<groupID> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }

                tmp = row[prefix + "EnumID"].Value.ToString().Trim();
                Dictionary<UInt16, UInt32> enumMap = null;
                if (!string.IsNullOrWhiteSpace(tmp))
                {
                    string[] enumIDArray = tmp.Split('/');
                    if (null == enumIDArray || 0 == enumIDArray.Length)
                    {
                        if (null != err)
                        {
                            err = "<EnumID> error: \"" + tmp + "\"";
                        }
                        return signalDataItemRet;
                    }
                    enumMap = new Dictionary<UInt16, UInt32>();
                    foreach (string enumIDEntry in enumIDArray)
                    {
                        Match mat = enumIDRegex.Match(enumIDEntry);
                        if(null == mat || mat.Groups.Count < 3)
                        {
                            if (null != err)
                            {
                                err = "<EnumID> error: \"" + tmp + "\"";
                            }
                            return signalDataItemRet;
                        }

                        UInt16 key = Convert.ToUInt16(mat.Groups[1].Value);
                        UInt32 val = Convert.ToUInt16(mat.Groups[2].Value);
                        if (enumMap.ContainsKey(key))
                        {
                            if (null != err)
                            {
                                err = "<EnumID> error: \"" + tmp + "\"";
                            }
                            return signalDataItemRet;
                        }
                        enumMap[key] = val;

                    }
                }

                tmp = row[prefix + "EnableStatistics"].Value.ToString().Trim();
                if (!yesOrNoTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<EnableStatistics> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                Boolean enStatistics = yesOrNoTable[tmp];


                tmp = row[prefix + "AlarmClass"].Value.ToString().Trim();
                if (!alarmClassTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<AlarmClass> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                BcAlarmClass alarmClass = alarmClassTable[tmp];

                tmp = row[prefix + "DBA"].Value.ToString().Trim();
                int dba = -1;
                try
                {
                    if (string.IsNullOrWhiteSpace(tmp))
                    {
                        dba = DEFAULT_DBA;
                    }
                    else
                    {
                        dba = Convert.ToUInt16(tmp);
                    }
                }
                catch (Exception e)
                {
                    dba = -1;
                }
                if(dba < 0)
                {
                    if (null != err)
                    {
                        err = "<DBA> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }

                tmp = row[prefix + "DAA"].Value.ToString().Trim();
                int daa = -1;
                try
                {
                    if (string.IsNullOrWhiteSpace(tmp))
                    {
                        daa = DEFAULT_DAA;
                    }
                    else
                    {
                        daa = Convert.ToUInt16(tmp);
                    }
                }
                catch (Exception e)
                {
                    daa = -1;
                }
                if (daa < 0)
                {
                    if (null != err)
                    {
                        err = "<DAA> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }

                signalDataItemRet = new SignalDataItem(signalId, enabled, macro, isAlarm, valueType, unitId, permission, isDisplay, accuracy, minValue, maxValue, defValue, groupId, enumMap, enStatistics, alarmClass, dba, daa);
            }
            catch (Exception e)
            {
                signalDataItemRet = null;
                if (err != null)
                {
                    err += "(Catch exception)";
                }
            }

            return signalDataItemRet;
        }

        public SignalDataItem(int signalId, bool enabled, string macro, bool alarm, ValueType valueType, int unitLangId, BcPermission bcPermission, bool display, int accuracy, object minValue, object maxValue, object defaultValue, int groupLangId, Dictionary<ushort, uint> enumLangIdTable, bool statistics, BcAlarmClass alarmClass, int alarmBefDelay, int alarmAftDelay)
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
            switch (valueType)
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
            if (defaultValue != null)
            {
                ret = defaultValue.ToString();
            }
            return ret;
        }

    }
}
