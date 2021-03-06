﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static BcTool.SignalDataItem;

namespace BcTool
{
    public partial class BcTool
    {
        /*
         * #define ENABLE_ALARM            1
#define DISABLE_ALARM           0
#define ENABLE_STATISTICS       1
#define DISABLE_STATISTICS      0
#define ENABLE_DISPLAY          1
#define DISABLE_DISPLAY         0
#define HAS_CUSTOM_INFO         1
#define NO_CUSTOM_INFO          0
#define RESERVED_FIELD          0

#define ALARM_CLASS_NONE            0x7F
#define ALARM_CLASS_INFO            4    
#define ALARM_CLASS_ATTENTION       3
#define ALARM_CLASS_WARNING         2
#define ALARM_CLASS_SERIOUS         1
#define ALARM_CLASS_EMERGENCY       0

#define ALARM_DELAY_DEFAULT            5

#define SYS_SIG_CUSTOM_TYPE_ALM_DLY_AFTER       10
#define SYS_SIG_CUSTOM_TYPE_ALM_DLY_BEFORE      9
#define SYS_SIG_CUSTOM_TYPE_ALM_CLASS           8
#define SYS_SIG_CUSTOM_TYPE_IS_ALARM            7
#define SYS_SIG_CUSTOM_TYPE_DEF_VAL             6
#define SYS_SIG_CUSTOM_TYPE_MAX_VAL             5
#define SYS_SIG_CUSTOM_TYPE_MIN_VAL             4
#define SYS_SIG_CUSTOM_TYPE_ACCURACY            3
#define SYS_SIG_CUSTOM_TYPE_GROUP_LANG          2
#define SYS_SIG_CUSTOM_TYPE_ENUM_LANG           1
#define SYS_SIG_CUSTOM_TYPE_EN_STATISTICS       0

#define SIG_INDEX_INVALID           0xFFFF

#define DIST_END_FLAG_MSK   0x01

#define DIST_BASIC_MSK          0x00
#define DIST_TEMP_HUMIDITY_MSK  0x10

#define DIST_CLASS_MSK      0x0E    
#define DIST_CLASS_1_MSK    0x02    
#define DIST_CLASS_2_MSK    0x04    
#define DIST_CLASS_3_MSK    0x06    
#define DIST_CLASS_4_MSK    0x08    
#define DIST_CLASS_5_MSK    0x0A    
#define DIST_CLASS_6_MSK    0x0C    


         * */
        private static string BP_CODE_ENABLE_ALARM = "ENABLE_ALARM";
        private static string BP_CODE_DISABLE_ALARM = "DISABLE_ALARM";
        private static string BP_CODE_ENABLE_STATISTICS = "ENABLE_STATISTICS";
        private static string BP_CODE_DISABLE_STATISTICS = "DISABLE_STATISTICS";
        private static string BP_CODE_ENABLE_DISPLAY = "ENABLE_DISPLAY";
        private static string BP_CODE_DISABLE_DISPLAY = "DISABLE_DISPLAY";
        private static string BP_CODE_HAS_CUSTOM_INFO = "HAS_CUSTOM_INFO";
        private static string BP_CODE_NO_CUSTOM_INFO = "NO_CUSTOM_INFO";
        private static string BP_CODE_ALARM_CLASS_NONE = "ALARM_CLASS_NONE";
        private static string BP_CODE_ALARM_CLASS_INFO = "ALARM_CLASS_INFO";
        private static string BP_CODE_ALARM_CLASS_ATTENTION = "ALARM_CLASS_ATTENTION";
        private static string BP_CODE_ALARM_CLASS_WARNING = "ALARM_CLASS_WARNING";
        private static string BP_CODE_ALARM_CLASS_SERIOUS = "ALARM_CLASS_SERIOUS";
        private static string BP_CODE_ALARM_CLASS_EMERGENCY = "ALARM_CLASS_EMERGENCY";
        private static string BP_CODE_PERMISSION_RO = "SIG_PERM_RO";
        private static string BP_CODE_PERMISSION_RW = "SIG_PERM_RW";

        public static string ValueTypeToCode(BPValueType vt)
        {
            string ret = "";
            switch (vt)
            {
                case BPValueType.U32:
                    ret = "t_u32";
                    break;
                case BPValueType.U16:
                    ret = "t_u16";
                    break;
                case BPValueType.I32:
                    ret = "t_i32";
                    break;
                case BPValueType.I16:
                    ret = "t_i16";
                    break;
                case BPValueType.ENUM:
                    ret = "t_enm";
                    break;
                case BPValueType.FLOAT:
                    ret = "t_flt";
                    break;
                case BPValueType.STRING:
                    ret = "t_str";
                    break;
                case BPValueType.BOOLEAN:
                    ret = "t_bool";
                    break;
                case BPValueType.MEM:
                    ret = "t_u32"; // TODO
                    break;
            }

            return ret;
        }

        public static string ValueTypeToEnumCode(BPValueType vt)
        {
            string ret = "";
            switch (vt)
            {
                case BPValueType.U32:
                    ret = "SIG_TYPE_U32";
                    break;
                case BPValueType.U16:
                    ret = "SIG_TYPE_U16";
                    break;
                case BPValueType.I32:
                    ret = "SIG_TYPE_I32";
                    break;
                case BPValueType.I16:
                    ret = "SIG_TYPE_I16";
                    break;
                case BPValueType.ENUM:
                    ret = "SIG_TYPE_ENM";
                    break;
                case BPValueType.FLOAT:
                    ret = "SIG_TYPE_FLT";
                    break;
                case BPValueType.STRING:
                    ret = "SIG_TYPE_STR";
                    break;
                case BPValueType.BOOLEAN:
                    ret = "SIG_TYPE_BOOLEAN";
                    break;
                case BPValueType.MEM:
                    ret = "SIG_TYPE_U32"; // TODO
                    break;
            }

            return ret;
        }

        public static string BcPermissionToCode(BcPermission perm)
        {
            string ret = "";
            switch (perm)
            {
                case BcPermission.RO:
                    ret = "SIG_PERM_RO";
                    break;
                case BcPermission.RW:
                    ret = "SIG_PERM_RW";
                    break;
            }

            return ret;
        }

        public static string BcAlarmClassToCode(BcAlarmClass bcAlarmClass)
        {

            string ret = "";
            switch (bcAlarmClass)
            {
                case BcAlarmClass.EMERGENCY:
                    ret = BP_CODE_ALARM_CLASS_EMERGENCY;
                    break;
                case BcAlarmClass.SERIOUS:
                    ret = BP_CODE_ALARM_CLASS_SERIOUS;
                    break;
                case BcAlarmClass.WARNING:
                    ret = BP_CODE_ALARM_CLASS_WARNING;
                    break;
                case BcAlarmClass.NOTICE:
                    ret = BP_CODE_ALARM_CLASS_ATTENTION;
                    break;
                case BcAlarmClass.INFO:
                    ret = BP_CODE_ALARM_CLASS_INFO;
                    break;
                case BcAlarmClass.NONE:
                    ret = BP_CODE_ALARM_CLASS_NONE;
                    break;
            }

            return ret;
        }

        private static string BLOCK_CHILD_TAG_DIST_N = @"<DIST_N>";
        private static string BLOCK_CHILD_TAG_ENABLE_LIST = @"<ENABLE_LIST>";

        private static string BLOCK_CHILD_TAG_DIST_N_MAP = @"<DIST_N_MAP>";
        private static string BLOCK_CHILD_TAG_DIST_CLASS = @"<DIST_CLASS>";
        private static string BLOCK_CHILD_TAG_DIST_END_FLAG = @"<DIST_END_FLAG>";

        private static string BLOCK_CHILD_TAG_MACRO = @"<MACRO>";
        private static string BLOCK_CHILD_TAG_TYPE_DEFINED = @"<TYPE_DEFINED>";
        private static string BLOCK_CHILD_TAG_STATISTICS = @"<STATISTICS>";
        private static string BLOCK_CHILD_TAG_DISPLAY = @"<DISPLAY>";
        private static string BLOCK_CHILD_TAG_ACCURACY = @"<ACCURACY>";
        private static string BLOCK_CHILD_TAG_ALARM = @"<ALARM>";
        private static string BLOCK_CHILD_TAG_PERMISSION = @"<PERMISSION>";
        private static string BLOCK_CHILD_TAG_ALARM_CLASS = @"<ALARM_CLASS>";
        private static string BLOCK_CHILD_TAG_CUSTOM_INFO = @"<CUSTOM_INFO>";
        private static string BLOCK_CHILD_TAG_MIN = @"<MIN>";
        private static string BLOCK_CHILD_TAG_MAX = @"<MAX>";
        private static string BLOCK_CHILD_TAG_DEF = @"<DEF>";
        private static string BLOCK_CHILD_TAG_ALARM_DELAY_BEF = @"<ALARM_DELAY_BEF>";
        private static string BLOCK_CHILD_TAG_ALARM_DELAY_AFT = @"<ALARM_DELAY_AFT>";
        private static string BLOCK_CHILD_TAG_UNIT_ID = @"<UNIT_ID>";
        private static string BLOCK_CHILD_TAG_GROUP_ID = @"<GROUP_ID>";
        private static string BLOCK_CHILD_TAG_KEY_2_VALUE = @"<KEY_2_VALUE>";
        private static string BLOCK_CHILD_TAG_CUSTOM_ENUM_LANG_SIZE = @"<CUSTOM_ENUM_LANG_SIZE>";
        private static string REPLACE_VALUE_CUSTOM_ENUM_LANG_SIZE = "const BP_SysCusEnumUnit <MACRO>_ENUM_LANG = {sizeof(<MACRO>_ENUM_MAP) / sizeof(BP_EnumSignalMap), <MACRO>_ENUM_MAP}";
        private static string BLOCK_CHILD_TAG_CUSTOM_MIN_VAL = @"<CUSTOM_MIN_VAL>";
        private static string BLOCK_CHILD_TAG_CUSTOM_MAX_VAL = @"<CUSTOM_MAX_VAL>";
        private static string BLOCK_CHILD_TAG_CUSTOM_DEF_VAL = @"<CUSTOM_DEF_VAL>";

        /* custom signal code block tag */
        private static string BLOCK_CHILD_TAG_CUSTOM_SIGNAL_STD_LANG_NUM = @"<STD_LANG_NUM>";
        private static string BLOCK_CHILD_TAG_CUSTOM_SIGNAL_STD_LANG_MASK = @"<STD_LANG_MASK>";
        // private static string BLOCK_CHILD_TAG_CUSTOM_SIGNAL_MACRO = @"<STD_LANG_MASK>";

        private Regex REGEX_CODE_BLOCK_MACRO_DEFINED = new Regex(@"(\s*#define\s+)(<MACRO>)(\s+)(<SIGNAL_ID>)");
        private Regex REGEX_CODE_BLOCK_SIGNAL_MIN = new Regex(@"(.+)(<MACRO>)(.+)(<TYPE>)(.+)(<MIN>)(.+)");
        private Regex REGEX_CODE_BLOCK_SIGNAL_MAX = new Regex(@"(.+)(<MACRO>)(.+)(<TYPE>)(.+)(<MAX>)(.+)");
        private Regex REGEX_CODE_BLOCK_SIGNAL_DEF = new Regex(@"(.+)(<MACRO>)(.+)(<TYPE>)(.+)(<DEF>)(.+)");
        private Regex REGEX_CODE_BLOCK_SIGNAL_ID_2_VAL = new Regex(@"(<MACRO>)");
        // private Regex REGEX_CODE_BLOCK_SIGNAL_ID_2_VAL = new Regex(@"(<MACRO>)");

        private Regex REGEX_CODE_BLOCK_DIST_N = new Regex(@"(<DIST_N>)");
        private static string BLOCK_TAG_MACRO_DEFINED = "MACRO_DEFINED";
        private static string BLOCK_TAG_SYSTEM_SIGNAL_MAP_DIST = "SYSTEM_SIGNAL_MAP_DIST";
        private static string BLOCK_TAG_SIGNAL_MIN_MAX_DEF = "SIGNAL_MIN_MAX_DEF";
        private static string BLOCK_TAG_SIGNAL_ID_2_VAL = "SIGNAL_ID_2_VAL";
        private static string BLOCK_TAG_SYSTEM_SIGNAL_TABLE = "SYSTEM_SIGNAL_TABLE";
        private static string BLOCK_TAG_SYSTEM_SIGNAL_ENABLE_DIST = "SYSTEM_SIGNAL_ENABLE_DIST";
        private static string BLOCK_TAG_SYSTEM_SIGNAL_ENABLE_DIST_UNIT = "SYSTEM_SIGNAL_ENABLE_DIST_UNIT";
        private static string BLOCK_TAG_SYSTEM_SIGNAL_CUSTOM_VALUE = "SYSTEM_SIGNAL_CUSTOM_VALUE";

        /* custom signal table code block tag */
        private static string BLOCK_TAG_CUSTOM_LANGUAGE_SUPPORTED_INFO = "LANGUAGE_SUPPORTED_INFO";
        private static string BLOCK_TAG_CUSTOM_SIGNAL_MACRO_DEFINED = "CUSTOM_SIGNAL_MACRO_DEFINED";
        private static string BLOCK_TAG_CUSTOM_SIGNAL_MIN_MAX_DEF_VAL = "CUSTOM_SIGNAL_MIN_MAX_DEF_VAL";
        private static string BLOCK_TAG_CUSTOM_SIGNAL_ID_2_VAL = "CUSTOM_SIGNAL_ID_2_VAL";
        private static string BLOCK_TAG_CUSTOM_SIGNAL_TABLE = "CUSTOM_SIGNAL_TABLE";
        private static string BLOCK_TAG_SIGNAL_NAME_LANGUAGE = "SIGNAL_NAME_LANGUAGE";
        private static string BLOCK_TAG_SIGNAL_UNIT_LANGUAGE = "SIGNAL_UNIT_LANGUAGE";
        private static string BLOCK_TAG_SIGNAL_GROUP_LANGUAGE = "SIGNAL_GROUP_LANGUAGE";
        private static string BLOCK_TAG_SIGNAL_ENUM_LANGUAGE = "SIGNAL_ENUM_LANGUAGE";
        private static string BLOCK_TAG_SIGNAL_UNIT_LANGUAGE_UNIT = "SIGNAL_UNIT_LANGUAGE_UNIT";
        private static string BLOCK_TAG_SIGNAL_GROUP_LANGUAGE_UNIT = "SIGNAL_GROUP_LANGUAGE_UNIT";
        private static string BLOCK_TAG_SIGNAL_ENUM_LANGUAGE_MAP = "SIGNAL_ENUM_LANGUAGE_MAP";
        private static string BLOCK_TAG_SIGNAL_ENUM_LANGUAGE_MAP_UNIT = "SIGNAL_ENUM_LANGUAGE_MAP_UNIT";


        private static string BLOCK_CHILD_TAG_SPANISH = @"<SPANISH>";
        private static string BLOCK_CHILD_TAG_ARABIC = @"<ARABIC>";
        private static string BLOCK_CHILD_TAG_RUSSIAN = @"<RUSSIAN>";
        private static string BLOCK_CHILD_TAG_FRENCH = @"<FRENCH>";
        private static string BLOCK_CHILD_TAG_ENGLISH = @"<ENGLISH>";
        private static string BLOCK_CHILD_TAG_CHINESE = @"<CHINESE>";
        private static string BLOCK_CHILD_TAG_SIGNAL_UNIT_LANGUAGE_INDEX = @"<SIGNAL_UNIT_LANGUAGE_INDEX>";
        private static string BLOCK_CHILD_TAG_SIGNAL_GROUP_LANGUAGE_INDEX = @"<SIGNAL_GROUP_LANGUAGE_INDEX>";
        private static string BLOCK_CHILD_TAG_SIGNAL_ENUM_LANGUAGE_UNIT = @"<SIGNAL_ENUM_LANGUAGE_UNIT>";

        private delegate string DlgConstructCodeBlock(string codeBlock);

        private delegate string DlgConstructSystemCustomCodeBlock(string codeBlock, SignalDataItem signalDataItem);

        private Dictionary<string, DlgConstructCodeBlock> codeBlockTag2Dlg;
        private Dictionary<string, DlgConstructSystemCustomCodeBlock> childCodeBlockTag2systemCustomDlg;
        private Dictionary<string, DlgConstructSystemCustomCodeBlock> childCodeBlockTag2customSignalDlg;



        private void initSignalBlockTools()
        {
            codeBlockTag2Dlg = new Dictionary<string, DlgConstructCodeBlock>();
            codeBlockTag2Dlg.Add(BLOCK_TAG_MACRO_DEFINED, new DlgConstructCodeBlock(constructCodeBlock_MACRO_DEFINED));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SYSTEM_SIGNAL_MAP_DIST, new DlgConstructCodeBlock(constructCodeBlock_SYSTEM_SIGNAL_MAP_DIST));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SIGNAL_MIN_MAX_DEF, new DlgConstructCodeBlock(constructCodeBlock_SIGNAL_MIN_MAX_DEF));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SIGNAL_ID_2_VAL, new DlgConstructCodeBlock(constructCodeBlock_SIGNAL_ID_2_VAL));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SYSTEM_SIGNAL_TABLE, new DlgConstructCodeBlock(constructCodeBlock_SYSTEM_SIGNAL_TABLE));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SYSTEM_SIGNAL_ENABLE_DIST, new DlgConstructCodeBlock(constructCodeBlock_SYSTEM_SIGNAL_ENABLE_DIST));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SYSTEM_SIGNAL_ENABLE_DIST_UNIT, new DlgConstructCodeBlock(constructCodeBlock_SYSTEM_SIGNAL_ENABLE_DIST_UNIT));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SYSTEM_SIGNAL_CUSTOM_VALUE, new DlgConstructCodeBlock(constructCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE));

            codeBlockTag2Dlg.Add(BLOCK_TAG_CUSTOM_LANGUAGE_SUPPORTED_INFO, new DlgConstructCodeBlock(constructCodeBlock_LANGUAGE_SUPPORTED_INFO));
            codeBlockTag2Dlg.Add(BLOCK_TAG_CUSTOM_SIGNAL_MACRO_DEFINED, new DlgConstructCodeBlock(constructCodeBlock_CUSTOM_SIGNAL_MACRO_DEFINED));
            codeBlockTag2Dlg.Add(BLOCK_TAG_CUSTOM_SIGNAL_MIN_MAX_DEF_VAL, new DlgConstructCodeBlock(constructCodeBlock_CUSTOM_SIGNAL_MIN_MAX_DEF_VAL));
            codeBlockTag2Dlg.Add(BLOCK_TAG_CUSTOM_SIGNAL_ID_2_VAL, new DlgConstructCodeBlock(constructCodeBlock_CUSTOM_SIGNAL_ID_2_VAL));
            codeBlockTag2Dlg.Add(BLOCK_TAG_CUSTOM_SIGNAL_TABLE, new DlgConstructCodeBlock(constructCodeBlock_CUSTOM_SIGNAL_TABLE));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SIGNAL_NAME_LANGUAGE, new DlgConstructCodeBlock(constructCodeBlock_SIGNAL_NAME_LANGUAGE));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SIGNAL_UNIT_LANGUAGE, new DlgConstructCodeBlock(constructCodeBlock_SIGNAL_UNIT_LANGUAGE));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SIGNAL_GROUP_LANGUAGE, new DlgConstructCodeBlock(constructCodeBlock_SIGNAL_GROUP_LANGUAGE));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SIGNAL_ENUM_LANGUAGE, new DlgConstructCodeBlock(constructCodeBlock_SIGNAL_ENUM_LANGUAGE));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SIGNAL_UNIT_LANGUAGE_UNIT, new DlgConstructCodeBlock(constructCodeBlock_SIGNAL_UNIT_LANGUAGE_UNIT));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SIGNAL_GROUP_LANGUAGE_UNIT, new DlgConstructCodeBlock(constructCodeBlock_SIGNAL_GROUP_LANGUAGE_UNIT));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SIGNAL_ENUM_LANGUAGE_MAP, new DlgConstructCodeBlock(constructCodeBlock_SIGNAL_ENUM_LANGUAGE_MAP));
            codeBlockTag2Dlg.Add(BLOCK_TAG_SIGNAL_ENUM_LANGUAGE_MAP_UNIT, new DlgConstructCodeBlock(constructCodeBlock_SIGNAL_ENUM_LANGUAGE_MAP_UNIT));

            childCodeBlockTag2systemCustomDlg = new Dictionary<string, DlgConstructSystemCustomCodeBlock>();
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_ALARM, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_ENABLE_ALARM));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_UNIT_ID, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_UNIT_ID));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_PERMISSION, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_PERMISSION));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_DISPLAY, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_DISPLAY));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_ACCURACY, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_ACCURACY));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_CUSTOM_MIN_VAL, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_CUSTOM_MIN_VAL));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_CUSTOM_MAX_VAL, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_CUSTOM_MAX_VAL));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_CUSTOM_DEF_VAL, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_CUSTOM_DEF_VAL));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_GROUP_ID, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_GROUP_ID));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_KEY_2_VALUE, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_KEY_2_VALUE));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_STATISTICS, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_STATISTICS));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_ALARM_CLASS, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_ALARM_CLASS));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_ALARM_DELAY_BEF, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_ALARM_DELAY_BEF));
            childCodeBlockTag2systemCustomDlg.Add(BLOCK_CHILD_TAG_ALARM_DELAY_AFT, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_ALARM_DELAY_AFT));

            childCodeBlockTag2customSignalDlg = new Dictionary<string, DlgConstructSystemCustomCodeBlock>();
            // childCodeBlockTag2customSignalDlg.Add(BLOCK_CHILD_TAG_CUSTOM_SIGNAL_STD_LANG_NUM, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_CUSTOM_SIGNAL_STD_LANG_NUM));
            // childCodeBlockTag2customSignalDlg.Add(BLOCK_CHILD_TAG_CUSTOM_SIGNAL_STD_LANG_MASK, new DlgConstructSystemCustomCodeBlock(constructSystemCustomCodeBlock_CUSTOM_SIGNAL_STD_LANG_MASK));
            
        }

        private string constructCodeBlock_MACRO_DEFINED(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2systemSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2systemSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        /* there must be " 0x{0:X000}" not "0x{:X000}", otherwise "$3" will be "$30" */
                        string tmp = REGEX_CODE_BLOCK_MACRO_DEFINED.Replace(codeBlock, "$1" + signalDataItem.Macro + "$3" + string.Format(" 0x{0:X4}", signalDataItem.SignalId));
                        ret += tmp;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_SYSTEM_SIGNAL_MAP_DIST(string codeBlock)
        {
            string ret = "";
            try
            {
                //  ret = REGEX_CODE_BLOCK_MACRO_DEFINED.Replace(codeBlock, "$1" + signalDataItem.Macro + "$3" + string.Format(" 0x{0:X000}", signalDataItem.SignalId));
                for(int i = 0; i < systemSignalEnableBits.Count && i < BPLibApi.SYSTEM_SIGNAL_TABLE_NUM; i++)
                {
                    byte[] bits = systemSignalEnableBits[i];
                    for (int j = 0; j < bits.Length; j++)
                    {
                        if(bits[j] != 0)
                        {
                            ret += REGEX_CODE_BLOCK_DIST_N.Replace(codeBlock, i.ToString());
                            break;
                        }
                    }

                   
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_SIGNAL_MIN_MAX_DEF(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2systemSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2systemSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        string tmp = codeBlock;
                        tmp = REGEX_CODE_BLOCK_SIGNAL_MIN.Replace(tmp, "${1}" + signalDataItem.Macro + "${3}" + ValueTypeToCode(signalDataItem.ValueType1) + "${5}" + signalDataItem.MinValue.ToString() + "${7}");
                        tmp = REGEX_CODE_BLOCK_SIGNAL_MAX.Replace(tmp, "${1}" + signalDataItem.Macro + "${3}" + ValueTypeToCode(signalDataItem.ValueType1) + "${5}" + signalDataItem.MaxValue.ToString() + "${7}");
                        tmp = REGEX_CODE_BLOCK_SIGNAL_DEF.Replace(tmp, "${1}" + signalDataItem.Macro + "${3}" + ValueTypeToCode(signalDataItem.ValueType1) + "${5}" + signalDataItem.DefaultValue.ToString() + "${7}");
                        ret += tmp;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }


        private string constructCodeBlock_SIGNAL_ID_2_VAL(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2systemSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2systemSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        string tmp = REGEX_CODE_BLOCK_SIGNAL_ID_2_VAL.Replace(codeBlock, signalDataItem.Macro);
                        ret += tmp;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }
        
        private string constructCodeBlock_SYSTEM_SIGNAL_TABLE(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2systemSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2systemSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        string tmp = codeBlock;
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_MACRO, signalDataItem.Macro);
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_TYPE_DEFINED, ValueTypeToEnumCode(signalDataItem.ValueType1));
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_STATISTICS, signalDataItem.Statistics ? BP_CODE_ENABLE_STATISTICS : BP_CODE_DISABLE_STATISTICS);
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_DISPLAY, signalDataItem.Display ? BP_CODE_ENABLE_DISPLAY : BP_CODE_DISABLE_DISPLAY);
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_ACCURACY, signalDataItem.Accuracy.ToString());
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_ALARM, signalDataItem.Alarm ? BP_CODE_ENABLE_ALARM : BP_CODE_DISABLE_ALARM); 
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_PERMISSION, BcPermissionToCode(signalDataItem.BcPermission1));
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_ALARM_CLASS, BcAlarmClassToCode(signalDataItem.AlarmClass));
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_CUSTOM_INFO, 0 == signalDataItem.CustomInfo ? BP_CODE_NO_CUSTOM_INFO : BP_CODE_HAS_CUSTOM_INFO);
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_MIN, signalDataItem.Macro + "_MIN");
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_MAX, signalDataItem.Macro + "_MAX");
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_DEF, signalDataItem.Macro + "_DEF");
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_ALARM_DELAY_BEF, signalDataItem.AlarmBefDelay.ToString());
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_ALARM_DELAY_AFT, signalDataItem.AlarmAftDelay.ToString());
                        ret += tmp;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_SYSTEM_SIGNAL_ENABLE_DIST(string codeBlock)
        {
            string ret = "";
            try
            {
                for (int i = 0; i < systemSignalEnableBits.Count && i < BPLibApi.SYSTEM_SIGNAL_TABLE_NUM; i++)
                {
                    byte[] bits = systemSignalEnableBits[i];
                    int signalDist = i;
                    int byteNum = -1;
                    for (int j = bits.Length - 1; j >= 0; j--)
                    {
                        if (bits[j] != 0)
                        {
                            for(int k = BPLibApi.SYSTEM_SIGNAL_CLASS_END; k >= BPLibApi.SYSTEM_SIGNAL_CLASS_START; k--)
                            {
                                /* when j > 0*/
                                if (j / (1 << (k - 1)) != 0)
                                {
                                    byteNum = 1 << k;
                                    break;
                                }
                                else
                                {
                                    /* when j == 0 */
                                    byteNum = 1;
                                }
                            }
                            break;
                        }
                    }
                    if(byteNum > 0)
                    {
                        /* const BP_UINT8 g_SysMapDis_<DIST_N>[] = {<ENABLE_LIST>}; */
                        string tmp = codeBlock;
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_DIST_N, i.ToString());
                        string bitsArrayString = "";
                        for(int j = 0; j < bits.Length && j < byteNum; j++)
                        {
                            bitsArrayString += bits[j].ToString() + ",";
                        }
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_ENABLE_LIST, bitsArrayString);
                        ret += tmp;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_SYSTEM_SIGNAL_ENABLE_DIST_UNIT(string codeBlock)
        {
            string ret = "";
            Boolean endFlag = true;
            try
            {
                for (int i = systemSignalEnableBits.Count - 1; i >= 0; i--)
                {
                    byte[] bits = systemSignalEnableBits[i];
                    int signalDist = i;
                    int signalDistClass = -1;
                    for (int j = bits.Length - 1; j >= 0; j--)
                    {
                        if (bits[j] != 0)
                        {
                            for (int k = BPLibApi.SYSTEM_SIGNAL_CLASS_END; k >= BPLibApi.SYSTEM_SIGNAL_CLASS_START; k--)
                            {
                                /* when j > 0*/
                                if (j / (1 << (k - 1)) != 0)
                                {
                                    signalDistClass = k;
                                    break;
                                }
                                else
                                {
                                    /* when j == 0 */
                                    signalDistClass = 1;
                                }
                            }
                            break;

                        }
                    }
                    if(signalDistClass > 0)
                    {
                        /*
        private static string BLOCK_CHILD_TAG_DIST_N_MAP = @"<DIST_N_MAP>";
private static string BLOCK_CHILD_TAG_DIST_CLASS = @"<DIST_CLASS>";
private static string BLOCK_CHILD_TAG_DIST_END_FLAG = @"<DIST_END_FLAG>";
{<DIST_N_MAP> | <DIST_CLASS> | <DIST_END_FLAG>, sizeof(g_SysMapDis_<DIST_N>) / sizeof(BP_UINT8), g_SysMapDis_<DIST_N>}, 
*/
                        string tmp = codeBlock;
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_DIST_N_MAP, (i << 4).ToString());
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_DIST_CLASS, (signalDistClass << 1).ToString());
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_DIST_N, i.ToString());
                        if(endFlag)
                        {
                            endFlag = false;
                            tmp = tmp.Replace(BLOCK_CHILD_TAG_DIST_END_FLAG, "1");
                        }
                        else
                        {
                            tmp = tmp.Replace(BLOCK_CHILD_TAG_DIST_END_FLAG, "0");
                        }
                        /* last dist first handled, so tmp is the newest line */
                        ret = tmp + ret;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2systemSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2systemSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        string tmp = codeBlock;
                        foreach (string childTag in childCodeBlockTag2systemCustomDlg.Keys)
                        {
                            tmp = childCodeBlockTag2systemCustomDlg[childTag](tmp, signalDataItem);
                        }
                        if (0 != signalDataItem.CustomInfo)
                        {
                            tmp = "\r\n/* Custom info: 0x" + Convert.ToString(signalDataItem.SignalId, 16) + " */" + "\r\n" + tmp;
                            tmp = tmp.Replace(BLOCK_CHILD_TAG_MACRO, signalDataItem.Macro);
                        }
                        
                        ret += tmp;
                        
                    }

                }

                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_LANGUAGE_SUPPORTED_INFO(string codeBlock)
        {
            string ret = "";
            if(0 == customSignalLanguageMask)
            {
                return ret;
            }
            try
            {
                ret = codeBlock;
                UInt32 stdLangNum = 0;
                for (int i = LanguageResourceItem.STD_LANGUAGE_START_INDEX; i < LanguageResourceItem.STD_LANGUAGE_START_INDEX + LanguageResourceItem.MAX_STD_LANGUAGE_NUM; i++)
                {
                    if ((customSignalLanguageMask & (1 << i)) != 0)
                    {
                        stdLangNum++;
                    }
                }
                ret = ret.Replace(BLOCK_CHILD_TAG_CUSTOM_SIGNAL_STD_LANG_NUM, stdLangNum.ToString());
                ret = ret.Replace(BLOCK_CHILD_TAG_CUSTOM_SIGNAL_STD_LANG_MASK, string.Format(" 0x{0:X2}", customSignalLanguageMask & LanguageResourceItem.MAX_STD_LANGUAGE_NUM_MASK));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_CUSTOM_SIGNAL_MACRO_DEFINED(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2customSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2customSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        string tmp = REGEX_CODE_BLOCK_MACRO_DEFINED.Replace(codeBlock, "$1" + signalDataItem.Macro + "$3" + string.Format(" 0x{0:X4}", signalDataItem.SignalId));
                        ret += tmp;

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_CUSTOM_SIGNAL_MIN_MAX_DEF_VAL(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2customSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2customSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        string tmp = codeBlock;
                        tmp = REGEX_CODE_BLOCK_SIGNAL_MIN.Replace(tmp, "${1}" + signalDataItem.Macro + "${3}" + ValueTypeToCode(signalDataItem.ValueType1) + "${5}" + signalDataItem.MinValue.ToString() + "${7}");
                        tmp = REGEX_CODE_BLOCK_SIGNAL_MAX.Replace(tmp, "${1}" + signalDataItem.Macro + "${3}" + ValueTypeToCode(signalDataItem.ValueType1) + "${5}" + signalDataItem.MaxValue.ToString() + "${7}");
                        tmp = REGEX_CODE_BLOCK_SIGNAL_DEF.Replace(tmp, "${1}" + signalDataItem.Macro + "${3}" + ValueTypeToCode(signalDataItem.ValueType1) + "${5}" + signalDataItem.DefaultValue.ToString() + "${7}");
                        ret += tmp;

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_CUSTOM_SIGNAL_ID_2_VAL(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2customSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2customSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        string tmp = codeBlock;
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_MACRO, signalDataItem.Macro);
                        ret += tmp;

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_CUSTOM_SIGNAL_TABLE(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2customSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2customSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        string tmp = codeBlock;
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_MACRO, signalDataItem.Macro);
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_TYPE_DEFINED, ValueTypeToEnumCode(signalDataItem.ValueType1));
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_STATISTICS, signalDataItem.Statistics ? BP_CODE_ENABLE_STATISTICS : BP_CODE_DISABLE_STATISTICS);
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_DISPLAY, signalDataItem.Display ? BP_CODE_ENABLE_DISPLAY : BP_CODE_DISABLE_DISPLAY);
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_ACCURACY, signalDataItem.Accuracy.ToString());
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_ALARM, signalDataItem.Alarm ? BP_CODE_ENABLE_ALARM : BP_CODE_DISABLE_ALARM);
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_PERMISSION, BcPermissionToCode(signalDataItem.BcPermission1));
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_ALARM_CLASS, BcAlarmClassToCode(signalDataItem.AlarmClass));
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_CUSTOM_INFO, 0 == signalDataItem.CustomInfo ? BP_CODE_NO_CUSTOM_INFO : BP_CODE_HAS_CUSTOM_INFO);
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_MIN, signalDataItem.Macro + "_MIN");
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_MAX, signalDataItem.Macro + "_MAX");
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_DEF, signalDataItem.Macro + "_DEF");
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_ALARM_DELAY_BEF, signalDataItem.AlarmBefDelay.ToString());
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_ALARM_DELAY_AFT, signalDataItem.AlarmAftDelay.ToString());
                        ret += tmp;

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_SIGNAL_NAME_LANGUAGE(string codeBlock)
        {
            string ret = "";
            try
            {
                if(!prefix2LangDictionary.ContainsKey(PREFIX_LANG_CUSTOM_SIGNAL))
                {
                    return ret;
                }
                Dictionary<UInt16, LanguageResourceItem>  id2LanguageResourceItemDictionary = prefix2LangDictionary[PREFIX_LANG_CUSTOM_SIGNAL];
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2customSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2customSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        if(!id2LanguageResourceItemDictionary.ContainsKey((ushort)signalDataItem.SignalId))
                        {
                            Console.WriteLine("error !ContainsKey(signalDataItem.SignalId");
                            continue;
                            // return;
                        }
                        LanguageResourceItem lri = id2LanguageResourceItemDictionary[(ushort)signalDataItem.SignalId];
                        string tmp = "/* " + signalDataItem.Macro + " */\r\n" + codeBlock;
                        for (int i = 2; i < 8; i++)
                        {
                            string blockTag = "";
                            switch(i)
                            {
                                case 2:
                                    blockTag = BLOCK_CHILD_TAG_SPANISH;
                                    break;
                                case 3:
                                    blockTag = BLOCK_CHILD_TAG_ARABIC;
                                    break;
                                case 4:
                                    blockTag = BLOCK_CHILD_TAG_RUSSIAN;
                                    break;
                                case 5:
                                    blockTag = BLOCK_CHILD_TAG_FRENCH;
                                    break;
                                case 6:
                                    blockTag = BLOCK_CHILD_TAG_ENGLISH;
                                    break;
                                case 7:
                                    blockTag = BLOCK_CHILD_TAG_CHINESE;
                                    break;
                                default:
                                    return ret;
    
                            }
                            if((customSignalLanguageMask & (1 << i)) != 0)
                            {
                                tmp = tmp.Replace(blockTag, lri.LanguageMap[(ushort)i]);
                            }
                            else
                            {
                                tmp = deleteBlockLine(tmp, blockTag);
                            }
                        }
                        ret += tmp;

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlockLanguage(string prefix, string codeBlock)
        {
            string ret = "";
            try
            {
                if (!prefix2LangDictionary.ContainsKey(prefix))
                {
                    return ret;
                }
                Dictionary<UInt16, LanguageResourceItem> id2LanguageResourceItemDictionary = prefix2LangDictionary[prefix];
                ArrayList arrayList = new ArrayList();
                arrayList.AddRange(id2LanguageResourceItemDictionary.Keys);
                arrayList.Sort();
                foreach (UInt16 key in arrayList)
                {
                    if(0 == key)
                    {
                        /* "0" is the default language resource(NULL) */
                        continue;
                    }
                    LanguageResourceItem lri = id2LanguageResourceItemDictionary[key];

                    string tmp = codeBlock;
                    for (int i = 2; i < 8; i++)
                    {
                        string blockTag = "";
                        switch (i)
                        {
                            case 2:
                                blockTag = BLOCK_CHILD_TAG_SPANISH;
                                break;
                            case 3:
                                blockTag = BLOCK_CHILD_TAG_ARABIC;
                                break;
                            case 4:
                                blockTag = BLOCK_CHILD_TAG_RUSSIAN;
                                break;
                            case 5:
                                blockTag = BLOCK_CHILD_TAG_FRENCH;
                                break;
                            case 6:
                                blockTag = BLOCK_CHILD_TAG_ENGLISH;
                                break;
                            case 7:
                                blockTag = BLOCK_CHILD_TAG_CHINESE;
                                break;
                            default:
                                return ret;

                        }
                        if ((customSignalLanguageMask & (1 << i)) != 0)
                        {
                            tmp = tmp.Replace(blockTag, lri.LanguageMap[(ushort)i]);
                        }
                        else
                        {
                            tmp = deleteBlockLine(tmp, blockTag);
                        }


                    }
                    ret += tmp;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_SIGNAL_UNIT_LANGUAGE(string codeBlock)
        {
            return constructCodeBlockLanguage(PREFIX_LANG_CUSTOM_UNIT, codeBlock);
        }

        private string constructCodeBlock_SIGNAL_GROUP_LANGUAGE(string codeBlock)
        {
            return constructCodeBlockLanguage(PREFIX_LANG_CUSTOM_GROUP, codeBlock);
        }

        private string constructCodeBlock_SIGNAL_ENUM_LANGUAGE(string codeBlock)
        {
            return constructCodeBlockLanguage(PREFIX_LANG_CUSTOM_ENUM, codeBlock);
        }

        private string constructCodeBlock_SIGNAL_UNIT_LANGUAGE_UNIT(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2customSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2customSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        if (0 == signalDataItem.UnitLangId)
                        {
                            /* "0" is the default language resource(NULL) */
                            continue;
                        }
                        string tmp = codeBlock;
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_MACRO, signalDataItem.Macro);
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_SIGNAL_UNIT_LANGUAGE_INDEX, signalDataItem.UnitLangId.ToString());
                        ret += tmp;

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_SIGNAL_GROUP_LANGUAGE_UNIT(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2customSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2customSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        if (0 == signalDataItem.GroupLangId)
                        {
                            /* "0" is the default language resource(NULL) */
                            continue;
                        }
                        string tmp = codeBlock;
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_MACRO, signalDataItem.Macro);
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_SIGNAL_GROUP_LANGUAGE_INDEX, signalDataItem.GroupLangId.ToString());
                        ret += tmp;

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_SIGNAL_ENUM_LANGUAGE_MAP(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2customSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2customSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        if (BPValueType.ENUM != signalDataItem.ValueType1)
                        {
                            continue;
                        }
                        string tmp = codeBlock;
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_MACRO, signalDataItem.Macro);
                        string enumMapTmp = "";
                        if(null != signalDataItem.EnumLangIdTable && 0 != signalDataItem.EnumLangIdTable.Count)
                        {
                            foreach(UInt16 key in signalDataItem.EnumLangIdTable.Keys)
                            {
                                enumMapTmp += "{" + key.ToString() + "," + signalDataItem.EnumLangIdTable[key].ToString() + "},\r\n";
                            }
                        }

                        tmp = tmp.Replace(BLOCK_CHILD_TAG_SIGNAL_ENUM_LANGUAGE_UNIT, enumMapTmp);
                        ret += tmp;

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructCodeBlock_SIGNAL_ENUM_LANGUAGE_MAP_UNIT(string codeBlock)
        {
            string ret = "";
            try
            {
                foreach (string prefix in prefixLists)
                {
                    if (!prefix2customSignalDataItemVariableList.ContainsKey(prefix))
                    {
                        continue;
                    }

                    List<SignalDataItem> signalDataItems = prefix2customSignalDataItemVariableList[prefix];
                    foreach (SignalDataItem signalDataItem in signalDataItems)
                    {
                        /* input.replace("$", "$$") */
                        if (!signalDataItem.Enabled)
                        {
                            continue;
                        }
                        if (BPValueType.ENUM != signalDataItem.ValueType1)
                        {
                            continue;
                        }
                        string tmp = codeBlock;
                        tmp = tmp.Replace(BLOCK_CHILD_TAG_MACRO, signalDataItem.Macro);
                        ret += tmp;

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_ENABLE_ALARM(string codeBlock, SignalDataItem signalDataItem)
        {
            if(null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if(null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_IS_ALARM)) != 0)
                {
                    ret = ret.Replace(BLOCK_CHILD_TAG_ALARM, signalDataItem.Alarm ? BP_CODE_ENABLE_ALARM : BP_CODE_DISABLE_ALARM);
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_ALARM);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_UNIT_ID(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_UNIT_LANG)) != 0)
                {
                    ret = ret.Replace(BLOCK_CHILD_TAG_UNIT_ID, signalDataItem.UnitLangId.ToString());
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_UNIT_ID);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_PERMISSION(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_PERMISSION)) != 0)
                {
                    ret = ret.Replace(BLOCK_CHILD_TAG_PERMISSION, signalDataItem.BcPermission1 == BcPermission.RO ? BP_CODE_PERMISSION_RO : BP_CODE_PERMISSION_RW);
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_PERMISSION);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_DISPLAY(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_IS_DISPLAY)) != 0)
                {
                    ret = ret.Replace(BLOCK_CHILD_TAG_DISPLAY, signalDataItem.Display ? BP_CODE_ENABLE_DISPLAY : BP_CODE_DISABLE_DISPLAY);
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_DISPLAY);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_ACCURACY(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_ACCURACY)) != 0)
                {
                    ret = ret.Replace(BLOCK_CHILD_TAG_ACCURACY, signalDataItem.Accuracy.ToString());
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_ACCURACY);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_CUSTOM_MIN_VAL(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_MIN_VAL)) != 0)
                {
                    ret = ret.Replace(BLOCK_CHILD_TAG_CUSTOM_MIN_VAL, "{." + ValueTypeToCode(signalDataItem.ValueType1) + " = " + signalDataItem.MinValue.ToString() + "}");
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_CUSTOM_MIN_VAL);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_CUSTOM_MAX_VAL(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_MAX_VAL)) != 0)
                {
                    ret = ret.Replace(BLOCK_CHILD_TAG_CUSTOM_MAX_VAL, "{." + ValueTypeToCode(signalDataItem.ValueType1) + " = " + signalDataItem.MaxValue.ToString() + "}");
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_CUSTOM_MAX_VAL);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_CUSTOM_DEF_VAL(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_DEF_VAL)) != 0)
                {
                    ret = ret.Replace(BLOCK_CHILD_TAG_CUSTOM_DEF_VAL, "{." + ValueTypeToCode(signalDataItem.ValueType1) + " = " + signalDataItem.DefaultValue.ToString() + "}");
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_CUSTOM_DEF_VAL);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_GROUP_ID(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_GROUP_LANG)) != 0)
                {
                    ret = ret.Replace(BLOCK_CHILD_TAG_GROUP_ID, signalDataItem.GroupLangId.ToString());
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_GROUP_ID);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_KEY_2_VALUE(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_ENUM_LANG)) != 0)
                {
                    // {< KEY >, < VALUE >},
                    string replaceValues = "";
                    if(signalDataItem.EnumLangIdTable != null && signalDataItem.EnumLangIdTable.Count != 0)
                    {
                        foreach(UInt16 key in signalDataItem.EnumLangIdTable.Keys)
                        {
                            replaceValues += "\t{" + key + "," + signalDataItem.EnumLangIdTable[key] + "},\r\n";
                        }
                    }
                    ret = ret.Replace(BLOCK_CHILD_TAG_KEY_2_VALUE, replaceValues);
                    ret = ret.Replace(BLOCK_CHILD_TAG_CUSTOM_ENUM_LANG_SIZE, REPLACE_VALUE_CUSTOM_ENUM_LANG_SIZE);
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_CUSTOM_ENUM_LANG_SIZE);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_KEY_2_VALUE);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_STATISTICS(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_EN_STATISTICS)) != 0)
                {
                    ret = ret.Replace(BLOCK_CHILD_TAG_STATISTICS, signalDataItem.Statistics ? BP_CODE_ENABLE_STATISTICS : BP_CODE_DISABLE_STATISTICS);
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_STATISTICS);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_ALARM_CLASS(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_ALM_CLASS)) != 0)
                {
                    string alarmClassTmp = BP_CODE_ALARM_CLASS_NONE;
                    switch (signalDataItem.AlarmClass)
                    {
                        case BcAlarmClass.EMERGENCY:
                            alarmClassTmp = BP_CODE_ALARM_CLASS_EMERGENCY;
                            break;
                        case BcAlarmClass.SERIOUS:
                            alarmClassTmp = BP_CODE_ALARM_CLASS_SERIOUS;
                            break;
                        case BcAlarmClass.WARNING:
                            alarmClassTmp = BP_CODE_ALARM_CLASS_WARNING;
                            break;
                        case BcAlarmClass.NOTICE:
                            alarmClassTmp = BP_CODE_ALARM_CLASS_ATTENTION;
                            break;
                        case BcAlarmClass.INFO:
                            alarmClassTmp = BP_CODE_ALARM_CLASS_INFO;
                            break;
                        case BcAlarmClass.NONE:
                            alarmClassTmp = BP_CODE_ALARM_CLASS_NONE;
                            break;
                    }
                    ret = ret.Replace(BLOCK_CHILD_TAG_ALARM_CLASS, alarmClassTmp);
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_ALARM_CLASS);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_ALARM_DELAY_BEF(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_ALM_DLY_BEFORE)) != 0)
                {
                    ret = ret.Replace(BLOCK_CHILD_TAG_ALARM_DELAY_BEF, signalDataItem.AlarmBefDelay.ToString());
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_ALARM_DELAY_BEF);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_SYSTEM_SIGNAL_CUSTOM_VALUE_ALARM_DELAY_AFT(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                uint customInfo = signalDataItem.CustomInfo;
                if ((customInfo & (1 << BPLibApi.SYS_SIG_CUSTOM_TYPE_ALM_DLY_AFTER)) != 0)
                {
                    ret = ret.Replace(BLOCK_CHILD_TAG_ALARM_DELAY_AFT, signalDataItem.AlarmAftDelay.ToString());
                }
                else
                {
                    int tagIndex = ret.IndexOf(BLOCK_CHILD_TAG_ALARM_DELAY_AFT);
                    if (tagIndex < 0)
                    {
                        return ret;
                    }
                    /* find the first ';', then find the '\n' which indicates the end of code block */
                    int lineEndIndex = ret.IndexOf(';', tagIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    lineEndIndex = ret.IndexOf('\n', lineEndIndex);
                    if (lineEndIndex < 0)
                    {
                        return ret;
                    }
                    // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
                    int lineStartIndex = ret.LastIndexOf("const", tagIndex);
                    if (lineStartIndex < 0)
                    {
                        return ret;
                    }
                    /* '1' means the last character '\n' */
                    ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_CUSTOM_SIGNAL_STD_LANG_NUM(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                ret = ret.Replace(BLOCK_CHILD_TAG_CUSTOM_SIGNAL_STD_LANG_NUM, "12345");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        private string constructSystemCustomCodeBlock_CUSTOM_SIGNAL_STD_LANG_MASK(string codeBlock, SignalDataItem signalDataItem)
        {
            if (null == codeBlock || codeBlock.Length == 0)
            {
                return "";
            }
            if (null == signalDataItem)
            {
                return codeBlock;
            }

            string ret = codeBlock;
            try
            {
                ret = ret.Replace(BLOCK_CHILD_TAG_CUSTOM_SIGNAL_STD_LANG_MASK, "xyz");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ret = codeBlock;
            }

            return ret;
        }

        /*
         * example:
         * "
         *  <MACRO_DEFINED> 
         *       #define <MACRO>            <SIGNAL_ID>
         *  </MACRO_DEFINED>
         *  "
         *  : "     #define <MACRO>            <SIGNAL_ID>" is [para] codeBlockTag
         *  : "MACRO_DEFINED" [para] codeBlockTag
         ***********/
        public string constructCodeBlock(string codeBlockTag, string codeBlock)
        {
            string ret = "";

            try
            {
                if(!codeBlockTag2Dlg.ContainsKey(codeBlockTag))
                {
                    return ret;
                }
                DlgConstructCodeBlock dlgConstructCodeBlock = codeBlockTag2Dlg[codeBlockTag];

                string tmp = dlgConstructCodeBlock(codeBlock);
                ret += tmp;

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }

        private string deleteBlockLine(string block, string blockTag)
        {
            string ret = block;
            if(String.IsNullOrWhiteSpace(block) || String.IsNullOrWhiteSpace(blockTag))
            {
                return ret;
            }

            int tagIndex = ret.IndexOf(blockTag);
            if (tagIndex < 0)
            {
                return ret;
            }
            /* find the first ';', then find the '\n' which indicates the end of code block */
            int lineEndIndex = ret.IndexOf('\n', tagIndex);
            if (lineEndIndex < 0)
            {
                return ret;
            }
            /*
            lineEndIndex = ret.IndexOf('\n', lineEndIndex);
            if (lineEndIndex < 0)
            {
                return ret;
            }
            */
            // int lineStartIndex = ret.LastIndexOf("const", isAlarmTagIndex, isAlarmTagIndex);
            int lineStartIndex = ret.LastIndexOf("\n", tagIndex);
            if (lineStartIndex < 0)
            {
                lineStartIndex = 0;
            }
            else
            {
                lineStartIndex += 1;
            }
            /* '1' means the last character '\n' */
            ret = ret.Remove(lineStartIndex, lineEndIndex + 1 - lineStartIndex);

            return ret;
        }
    }

    
}
