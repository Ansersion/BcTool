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

        public enum BPValueType
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
            NONE,
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

        public const UInt32 CUSTOM_INFO_PARSE_ERROR = 0xFFFFFFFF;


        public static Dictionary<string, Boolean> yesOrNoTable = new Dictionary<string, Boolean>()
        {
            {"", false},
            {"YES", true},
            {"NO", false}
        };

        public static Dictionary<string, BPValueType> valueTypeTable = new Dictionary<string, BPValueType>()
        {
            {"UINT32", BPValueType.U32},
            {"UINT16", BPValueType.U16},
            {"INT32", BPValueType.I32},
            {"INT16", BPValueType.I16},
            {"ENUM", BPValueType.ENUM},
            {"FLOAT", BPValueType.FLOAT},
            {"STRING", BPValueType.STRING},
            {"BOOLEAN", BPValueType.BOOLEAN},
            {"MEM", BPValueType.MEM}
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
            { BPValueType.U32, 0xFFFFFFFF },
            { BPValueType.U16, 0xFFFF },
            { BPValueType.I32, 0x7FFFFFFF },
            { BPValueType.I16, 0x7FFF },
            { BPValueType.ENUM, 0xFFFF },
            { BPValueType.FLOAT, 0x7FFFFFFF },
            { BPValueType.STRING, "" },
            { BPValueType.BOOLEAN, false },
            { BPValueType.MEM, null },
        };

        public static UInt32 parseCustomInfoMask(int offset, ref SignalDataItem signalDataItem, string newValue)
        {
            UInt32 ret = CUSTOM_INFO_PARSE_ERROR;
            try
            {
               
                switch (offset)
                {
                    /*
                    case 0:
                        return SignalId;
                    case 1:
                        return Enabled;
                    case 2:
                        return Macro;
                        */
                    case 3:
                        if (SignalDataItem.yesOrNoTable.ContainsKey(newValue))
                        {
                            ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_IS_ALARM);
                            signalDataItem.Alarm = SignalDataItem.yesOrNoTable[newValue];
                        }

                        break;
                    /*
                case 4:
                    return ValueType1;
                    */
                    case 5:
                        {
                            int unitId = -1;
                            try
                            {
                                
                                if (string.IsNullOrWhiteSpace(newValue))
                                {
                                    unitId = DEFAULT_UNIT_ID;
                                }
                                else
                                {
                                    Match mat = unitIDRegex.Match(newValue);

                                    if (null != mat && mat.Groups.Count >= 2)
                                    {
                                        unitId = Convert.ToInt32(mat.Groups[1].Value);
                                    }
                                }
                            }
                            catch(Exception e)
                            {
                                unitId = -1;
                            }

                            if(unitId >= 0)
                            {
                                ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_UNIT_LANG);
                                signalDataItem.UnitLangId = unitId;
                            }
                            break;
                        }
                    case 6:
                        
                        if (permissionTable.ContainsKey(newValue))
                        {
                            ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_PERMISSION);
                            signalDataItem.BcPermission1 = permissionTable[newValue];
                        }
                        break;
                    case 7:
                        if (yesOrNoTable.ContainsKey(newValue))
                        {
                            ret = (1 << BPLibApi.SYS_SIG_CUSTOM_IS_DISPLAY);
                            signalDataItem.Display = yesOrNoTable[newValue];
                        }
                        break;
                    case 8:
                        int accuracy = -1;
                        try
                        {
                            accuracy = Convert.ToInt32(newValue);
                            if (accuracy >= 0)
                            {
                                ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_ACCURACY);
                                signalDataItem.Accuracy = accuracy;
                            }
                        }
                        catch (Exception e)
                        {
                            accuracy = -1;
                        }
                        
                        break;
                    case 9:
                        if(Tools.setSigValue(ref signalDataItem.minValue, signalDataItem.ValueType1, newValue))
                        {
                            ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_MIN_VAL);
                        }
                        break;
                    case 10:
                        if (Tools.setSigValue(ref signalDataItem.maxValue, signalDataItem.ValueType1, newValue))
                        {
                            ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_MAX_VAL);
                        }
                        break;
                    case 11:
                        if (Tools.setSigValue(ref signalDataItem.defaultValue, signalDataItem.ValueType1, newValue))
                        {
                            ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_DEF_VAL);
                        }
                        break;
                    case 12:
                        {
                            int groupId = -1;
                            try
                            {
                                if (string.IsNullOrWhiteSpace(newValue))
                                {
                                    groupId = DEFAULT_GROUP_ID;
                                }
                                else
                                {
                                    Match mat = groupIDRegex.Match(newValue);
                                    if (null != mat && mat.Groups.Count >= 2)
                                    {
                                        groupId = Convert.ToInt32(mat.Groups[1].Value);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                groupId = -1;
                            }
                            if (groupId >= 0)
                            {
                                ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_GROUP_LANG);
                                signalDataItem.GroupLangId = groupId;
                            }
                            break;
                        }
                    case 13:
                        Dictionary<UInt16, UInt32> enumMap = new Dictionary<ushort, uint>();
                        if (!string.IsNullOrWhiteSpace(newValue))
                        {
                            string[] enumIDArray = newValue.Split('/');
                            if (null != enumIDArray && 0 != enumIDArray.Length)
                            {
                                foreach (string enumIDEntry in enumIDArray)
                                {
                                    Match mat = enumIDRegex.Match(enumIDEntry);
                                    if (null == mat || mat.Groups.Count < 3)
                                    {
                                        enumMap = null;
                                        break;
                                    }

                                    UInt16 key = Convert.ToUInt16(mat.Groups[1].Value);
                                    UInt32 val = Convert.ToUInt16(mat.Groups[2].Value);
                                    if (enumMap.ContainsKey(key))
                                    {
                                        enumMap = null;
                                        break;
                                    }
                                    enumMap[key] = val;

                                }
                            }
                            else
                            {
                                enumMap = null;
                            }
                        }
                        if(null != enumMap)
                        {
                            signalDataItem.EnumLangIdTable = enumMap;
                            ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_ENUM_LANG);
                        }
                   
                        break;
                    case 14:
                        if (yesOrNoTable.ContainsKey(newValue))
                        {
                            ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_EN_STATISTICS);
                            signalDataItem.Statistics = yesOrNoTable[newValue];
                        }
                        break;
                    case 15:
                        {
                            if (alarmClassTable.ContainsKey(newValue))
                            {
                                ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_ALM_CLASS);
                                signalDataItem.AlarmClass = alarmClassTable[newValue];
                            }
                            break;
                        }
                    case 16:
                        {
                            int dba = -1;
                            try
                            {
                                if (string.IsNullOrWhiteSpace(newValue))
                                {
                                    dba = DEFAULT_DBA; ;
                                }
                                else
                                {
                                    dba = Convert.ToUInt16(newValue);
                                }
                            }
                            catch (Exception e)
                            {
                                dba = -1;
                            }
                            if (dba >= 0)
                            {
                                ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_ALM_DLY_BEFORE);
                                signalDataItem.AlarmBefDelay = dba;
                            }

                            break;
                        }
                    case 17:
                        {
                            int daa = -1;
                            try
                            {
                                if (string.IsNullOrWhiteSpace(newValue))
                                {
                                    daa = DEFAULT_DBA; ;
                                }
                                else
                                {
                                    daa = Convert.ToUInt16(newValue);
                                }
                            }
                            catch (Exception e)
                            {
                                daa = -1;
                            }
                            if (daa >= 0)
                            {
                                ret = (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_ALM_DLY_AFTER);
                                signalDataItem.AlarmBefDelay = daa;
                            }
                            break;
                        }
                    default:
                        ret = CUSTOM_INFO_PARSE_ERROR;
                        break;
                }
            }
            catch (Exception e)
            {
                ret = CUSTOM_INFO_PARSE_ERROR;
            }

            return ret;
        }

        public static Boolean judgeEqual(SignalDataItem signalDataItemVal, ref SignalDataItem signalDataItem, int index)
        {
            Boolean ret = false;

            if (null == signalDataItemVal)
            {
                return ret;
            }
            if (null == signalDataItem[index])
            {
                return ret;
            }
            
            try
            {
                object value = signalDataItemVal[index];
                if (13 == index && signalDataItem.ValueType1 == BPValueType.ENUM)
                {
                    Dictionary<UInt16, UInt32> enumMap = (Dictionary<ushort, uint>)value;

                    if (signalDataItem.EnumLangIdTable.Count == enumMap.Count)
                    {
                        ret = true;
                        foreach(UInt16 key in signalDataItem.EnumLangIdTable.Keys)
                        {
                            if(!enumMap.ContainsKey(key))
                            {
                                ret = false;
                                break;
                            }
                            if(enumMap[key] != signalDataItem.EnumLangIdTable[key])
                            {
                                ret = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    ret = value.Equals(signalDataItem[index]);
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                ret = false;
            }

            return ret;
        }

        private int signalId;
        private Boolean enabled;
        private String macro;
        private Boolean alarm;
        private BPValueType valueType;
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
        private UInt32 customInfo;

        public static Regex enumIDRegex = new Regex(ENUM_ID_REGEX_PATTERN);
        public static Regex unitIDRegex = new Regex(UNIT_ID_REGEX_PATTERN);
        public static Regex groupIDRegex = new Regex(GROUP_ID_REGEX_PATTERN);

        public int SignalId { get => signalId; set => signalId = value; }
        public bool Enabled { get => enabled; set => enabled = value; }
        public string Macro { get => macro; set => macro = value; }
        public bool Alarm { get => alarm; set => alarm = value; }
        internal BPValueType ValueType1 { get => valueType; set => valueType = value; }
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
        public uint CustomInfo { get => customInfo; set => customInfo = value; }

        public object this[int index]
        {
            get
            {
                switch(index)
                {
                    case 0:
                        return SignalId;
                    case 1:
                        return Enabled;
                    case 2:
                        return Macro;
                    case 3:
                        return Alarm;
                    case 4:
                        return ValueType1;
                    case 5:
                        return UnitLangId;
                    case 6:
                        return BcPermission1;
                    case 7:
                        return Display;
                    case 8:
                        return Accuracy;
                    case 9:
                        return MinValue;
                    case 10:
                        return MaxValue;
                    case 11:
                        return DefaultValue;
                    case 12:
                        return GroupLangId;
                    case 13:
                        return EnumLangIdTable;
                    case 14:
                        return Statistics;
                    case 15:
                        return AlarmClass;
                    case 16:
                        return AlarmBefDelay;
                    case 17:
                        return AlarmAftDelay;
                    default:
                        return null;
                            
                }
               
            }
        }

        public static object getDefault(BPValueType valueType)
        {
            object ret = null;

            try
            {
                switch (valueType)
                {
                    case BPValueType.U32:
                        ret = Convert.ToUInt32("0");
                        break;
                    case BPValueType.U16:
                        ret = Convert.ToUInt16("0");
                        break;
                    case BPValueType.I32:
                        ret = Convert.ToInt32("0");
                        break;
                    case BPValueType.I16:
                        ret = Convert.ToInt16("0");
                        break;
                    case BPValueType.ENUM:
                        ret = Convert.ToUInt16("0");
                        break;
                    case BPValueType.FLOAT:
                        ret = Convert.ToDouble("0");
                        break;
                    case BPValueType.STRING:
                        ret = DEFAULT_VALUE;
                        break;
                    case BPValueType.BOOLEAN:
                        ret = float.Parse("0");
                        break;
                    case BPValueType.MEM:
                        ret = DEFAULT_VALUE;
                        break;
                    default:
                        ret = null;
                        break;

                }
            }
            catch (Exception e)
            {
                ret = null;
            }
            return ret;
        }

        public static object parseValue(BPValueType valueType, string str)
        {
            object ret = null;
            if(null == str)
            {
                return ret;
            }
            if(string.IsNullOrWhiteSpace(str))
            {
                ret = getDefault(valueType);
                return ret;
            }

            try
            {
                switch(valueType)
                {
                    case BPValueType.U32:
                        ret = Convert.ToUInt32(str);
                        break;
                    case BPValueType.U16:
                        ret = Convert.ToUInt16(str);
                        break;
                    case BPValueType.I32:
                        ret = Convert.ToInt32(str);
                        break;
                    case BPValueType.I16:
                        ret = Convert.ToInt16(str);
                        break;
                    case BPValueType.ENUM:
                        ret = Convert.ToUInt16(str);
                        break;
                    case BPValueType.FLOAT:
                        ret = float.Parse(str);
                        break;
                    case BPValueType.STRING:
                        ret = str;
                        break;
                    case BPValueType.BOOLEAN:
                        ret = Convert.ToBoolean(str);
                        break;
                    case BPValueType.MEM:
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
                BPValueType valueType = valueTypeTable[tmp];

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

        public SignalDataItem(int signalId, bool enabled, string macro, bool alarm, BPValueType valueType, int unitLangId, BcPermission bcPermission, bool display, int accuracy, object minValue, object maxValue, object defaultValue, int groupLangId, Dictionary<ushort, uint> enumLangIdTable, bool statistics, BcAlarmClass alarmClass, int alarmBefDelay, int alarmAftDelay)
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

        public SignalDataItem(SignalDataItem signalDataItem)
        {
            this.signalId = signalDataItem.SignalId;
            this.enabled = signalDataItem.Enabled;
            this.macro = signalDataItem.Macro;
            this.alarm = signalDataItem.Alarm;
            this.valueType = signalDataItem.ValueType1;
            this.unitLangId = signalDataItem.UnitLangId;
            this.bcPermission = signalDataItem.BcPermission1;
            this.display = signalDataItem.Display;
            this.accuracy = signalDataItem.Accuracy;

            // TODO: maybe realloc mem
            this.minValue = signalDataItem.MinValue;
            this.maxValue = signalDataItem.MaxValue;
            this.defaultValue = signalDataItem.DefaultValue;
            this.groupLangId = signalDataItem.GroupLangId;
            // TODO: maybe realloc mem
            this.enumLangIdTable = signalDataItem.EnumLangIdTable;
            this.statistics = signalDataItem.Statistics;
            this.alarmClass = signalDataItem.AlarmClass;
            this.alarmBefDelay = signalDataItem.AlarmBefDelay;
            this.alarmAftDelay = signalDataItem.AlarmAftDelay;
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
                case BPValueType.U32:
                    ret = "U32";
                    break;
                case BPValueType.U16:
                    ret = "U16";
                    break;
                case BPValueType.I32:
                    ret = "I32";
                    break;
                case BPValueType.I16:
                    ret = "I16";
                    break;
                case BPValueType.ENUM:
                    ret = "ENUM";
                    break;
                case BPValueType.FLOAT:
                    ret = "FLOAT";
                    break;
                case BPValueType.STRING:
                    ret = "STRING";
                    break;
                case BPValueType.BOOLEAN:
                    ret = "BOOLEAN";
                    break;
                case BPValueType.MEM:
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
