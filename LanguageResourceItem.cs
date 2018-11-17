using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcTool
{
    public class LanguageResourceItem
    {
        public const int CHINESE_KEY = 7;
        public const int ENGLISH_KEY = 6;
        public const int FRENCH_KEY = 5;
        public const int RUSSIAN_KEY = 4;
        public const int ARABIC_KEY = 3;
        public const int SPANISH_KEY = 2;

        private UInt16 indexId;
        private Dictionary<UInt16, string> languageMap;

        public static LanguageResourceItem parseLanguageResourceItem(System.Windows.Forms.DataGridViewCellCollection row, ref string err)
        {
            LanguageResourceItem languageResourceItemRet = null;
            if (null == row)
            {
                return languageResourceItemRet;
            }

            try
            {
                String tmp;

                tmp = row["LanguageID"].Value.ToString().Trim();
                int languageId = -1;
                try
                {
                    languageId = Convert.ToInt32(tmp, 16);
                }
                catch (Exception e)
                {
                    languageId = -1;
                }
                if (languageId < 0)
                {
                    if (null != err)
                    {
                        err = "<LanguageID> error: \"" + tmp + "\"";
                    }
                    return languageResourceItemRet;
                }

                tmp = row["Chinese"].Value.ToString().Trim();
                /*
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

                string macro = row["Macro"].Value.ToString().Trim();
                if (string.IsNullOrWhiteSpace(macro))
                {
                    if (null != err)
                    {
                        err = "<Macro> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }


                tmp = row["IsAlarm"].Value.ToString().Trim();
                if (!yesOrNoTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<IsAlarm> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                Boolean isAlarm = yesOrNoTable[tmp];

                tmp = row["ValueType"].Value.ToString().Trim();
                if (!valueTypeTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<ValueType> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                ValueType valueType = valueTypeTable[tmp];

                tmp = row["UnitID"].Value.ToString().Trim();
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

                tmp = row["Permission"].Value.ToString().Trim();
                if (!permissionTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<Permission> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                BcPermission permission = permissionTable[tmp];

                tmp = row["IsDisplay"].Value.ToString().Trim();
                if (!yesOrNoTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<IsDisplay> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                Boolean isDisplay = yesOrNoTable[tmp];

                tmp = row["Accuracy"].Value.ToString().Trim();
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


                tmp = row["Min"].Value.ToString().Trim();
                object minValue = parseValue(valueType, tmp);

                if (null == minValue)
                {
                    if (null != err)
                    {
                        err = "<Min> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }

                tmp = row["Max"].Value.ToString().Trim();
                object maxValue = parseValue(valueType, tmp);

                if (null == maxValue)
                {
                    if (null != err)
                    {
                        err = "<Max> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }

                tmp = row["Default"].Value.ToString().Trim();
                object defValue = parseValue(valueType, tmp);

                if (null == defValue)
                {
                    if (null != err)
                    {
                        err = "<Default> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }

                tmp = row["GroupID"].Value.ToString().Trim();
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

                tmp = row["EnumID"].Value.ToString().Trim();
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
                        if (null == mat || mat.Groups.Count < 3)
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

                tmp = row["EnableStatistics"].Value.ToString().Trim();
                if (!yesOrNoTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<EnableStatistics> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                Boolean enStatistics = yesOrNoTable[tmp];


                tmp = row["AlarmClass"].Value.ToString().Trim();
                if (!alarmClassTable.ContainsKey(tmp))
                {
                    if (null != err)
                    {
                        err = "<AlarmClass> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }
                BcAlarmClass alarmClass = alarmClassTable[tmp];

                tmp = row["DBA"].Value.ToString().Trim();
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
                if (dba < 0)
                {
                    if (null != err)
                    {
                        err = "<DBA> error: \"" + tmp + "\"";
                    }
                    return signalDataItemRet;
                }

                tmp = row["DAA"].Value.ToString().Trim();
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
                */
            }
            catch (Exception e)
            {
                languageResourceItemRet = null;
                if (err != null)
                {
                    err += "(Catch exception)";
                }
            }

            return languageResourceItemRet;
        }


    }
}
