using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BcTool
{
    public struct BP_WORD
    {
        // A struct's fields should not be exposed
        private UInt64 value;

        // As we are using implicit conversions we can keep the constructor private
        private BP_WORD(UInt64 value)
        {
            this.value = value;
        }

        /// <summary>
        /// Implicitly converts a <see cref="System.Int32"/> to a Record.
        /// </summary>
        /// <param name="value">The <see cref="System.Int32"/> to convert.</param>
        /// <returns>A new Record with the specified value.</returns>
        public static implicit operator BP_WORD(UInt64 value)
        {
            return new BP_WORD(value);
        }

        public static implicit operator BP_WORD(int value)
        {
            return new BP_WORD((ulong)value);
        }
        /// <summary>
        /// Implicitly converts a Record to a <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="record">The Record to convert.</param>
        /// <returns>
        /// A <see cref="System.Int32"/> that is the specified Record's value.
        /// </returns>
        public static implicit operator UInt64(BP_WORD word)
        {
            return word.value;
        }
    }

    [global::System.AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    sealed class BitfieldLengthAttribute : Attribute
    {
        uint length;

        public BitfieldLengthAttribute(uint length)
        {
            this.length = length;
        }

        public uint Length { get { return length; } }
    }

    static class PrimitiveConversion
    {
        public static long ToLong<T>(T t) where T : struct
        {
            long r = 0;
            int offset = 0;

            // For every field suitably attributed with a BitfieldLength
            foreach (System.Reflection.FieldInfo f in t.GetType().GetFields())
            {
                object[] attrs = f.GetCustomAttributes(typeof(BitfieldLengthAttribute), false);
                if (attrs.Length == 1)
                {
                    uint fieldLength = ((BitfieldLengthAttribute)attrs[0]).Length;

                    // Calculate a bitmask of the desired length
                    long mask = 0;
                    for (int i = 0; i < fieldLength; i++)
                        mask |= 1 << i;

                    r |= ((UInt32)f.GetValue(t) & mask) << offset;

                    offset += (int)fieldLength;
                }
            }

            return r;
        }
    }

    public class BPLibApi
    {

        public const int SYSTEM_SIGNAL_TABLE_NUM = 16;
        public const int FIX_HEADER_SIZE = 3;
        public const UInt16 SIG_TYPE_MAX_INDEX = 7;
        public const UInt16 MAX_ACCURACY = 7;
        public const UInt16 MAX_ALARM_CLASS_INDEX = 4;
        public const UInt16 INVALID_ALARM_CLASS = 255;
        public const UInt16 MAX_ALARM_DELAY = 255;

        public const UInt16 BP_VALUE_TYPE_U32 = 0;
        public const UInt16 BP_VALUE_TYPE_U16 = 1;
        public const UInt16 BP_VALUE_TYPE_I32 = 2;
        public const UInt16 BP_VALUE_TYPE_I16 = 3;
        public const UInt16 BP_VALUE_TYPE_ENUM = 4;
        public const UInt16 BP_VALUE_TYPE_FLT = 5;
        public const UInt16 BP_VALUE_TYPE_STR = 6;
        public const UInt16 BP_VALUE_TYPE_BOOL = 7;

        public const UInt16 BP_DIS_STATISTICS = 0;
        public const UInt16 BP_EN_STATISTICS = 1;
        public const UInt16 BP_NOT_DISPLAY = 0;
        public const UInt16 BP_DISPLAY = 1;
        public const UInt16 BP_DIS_ALARM = 0;
        public const UInt16 BP_EN_ALARM = 1;
        public const UInt16 BP_SIGNAL_PERMISSION_RO = 0;
        public const UInt16 BP_SIGNAL_PERMISSION_RW = 1;
        public const UInt16 BP_SIGNAL_ALARM_CLASS_EMERGENCY = 0;
        public const UInt16 BP_SIGNAL_ALARM_CLASS_SERIOUS = 1;
        public const UInt16 BP_SIGNAL_ALARM_CLASS_WARNING = 2;
        public const UInt16 BP_SIGNAL_ALARM_CLASS_NOTICE = 3;
        public const UInt16 BP_SIGNAL_ALARM_CLASS_INFO = 4;
        public const UInt16 BP_SYS_SIGNAL_NONE_CUSTOM = 0;
        public const UInt16 BP_SYS_SIGNAL_CUSTOM = 1;

        public const char BP_DEFAULT_ALARM_DELAY = (char)5;

        public const UInt16 SYSTEM_START_SIGNAL_ID = 0xE000;
        public const UInt16 SYSTEM_END_SIGNAL_ID = 0xEFFF;
        public const UInt16 SYSTEM_SIGNAL_STEP = 0x200;
        public const UInt16 SYSTEM_SIGNAL_CLASS_START = 1;
        public const UInt16 SYSTEM_SIGNAL_CLASS_END = 6;
        public const UInt16 SYSTEM_SIGNAL_DIST_OFFSET = 4;
        public const UInt16 SYSTEM_SIGNAL_CLASS_OFFSET = 1;


        public const char SYS_SIG_CUSTOM_IS_DISPLAY = (char)13;
        public const char SYS_SIG_CUSTOM_TYPE_PERMISSION = (char)12;
        public const char SYS_SIG_CUSTOM_TYPE_UNIT_LANG = (char)11;
        public const char SYS_SIG_CUSTOM_TYPE_ALM_DLY_AFTER = (char)10;
        public const char SYS_SIG_CUSTOM_TYPE_ALM_DLY_BEFORE = (char)9;
        public const char SYS_SIG_CUSTOM_TYPE_ALM_CLASS = (char)8;
        public const char SYS_SIG_CUSTOM_TYPE_IS_ALARM = (char)7;
        public const char SYS_SIG_CUSTOM_TYPE_DEF_VAL = (char)6;
        public const char SYS_SIG_CUSTOM_TYPE_MAX_VAL = (char)5;
        public const char SYS_SIG_CUSTOM_TYPE_MIN_VAL = (char)4;
        public const char SYS_SIG_CUSTOM_TYPE_ACCURACY = (char)3;
        public const char SYS_SIG_CUSTOM_TYPE_GROUP_LANG = (char)2;
        public const char SYS_SIG_CUSTOM_TYPE_ENUM_LANG = (char)1;
        public const char SYS_SIG_CUSTOM_TYPE_EN_STATISTICS = (char)0;


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PackBuf
        {
            public IntPtr Buf;
            public UInt32 RmnLen;
            public IntPtr PackStart;
            public UInt64 MsgSize;
            public UInt64 BufSize;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BPContext
        {
            /* suggest: not change these variables durring time */
            public byte Encryption;
            public byte CrcType;
            public byte BPLevel;
            public byte PerformanceLimit;
            public byte IsDeviceClient;
            /* suggest end */

            /* note: changed and token into effect only after a new CONNECTION */
            public ushort BPAlivePeroid;
            public ushort BPTimeout;
            /* note end */

            public IntPtr packBuf;
            public IntPtr name;
            public IntPtr password;
            //[MarshalAs(UnmanagedType.U8)]
            // public IntPtr password;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        public struct SigTypeU
        {
            [FieldOffset(0)]
            public UInt32 t_u32;
            [FieldOffset(0)]
            public UInt16 t_u16;
            [FieldOffset(0)]
            public Int32 t_i32;
            [FieldOffset(0)]
            public Int16 t_i16;
            [FieldOffset(0)]
            public UInt16 t_enm;
            [FieldOffset(0)]
            public float t_flt;
            [FieldOffset(0)]
            public IntPtr t_str;
            [FieldOffset(0)]
            public char t_bool;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BP_SigId2Val
        {
            public UInt16 SigId;
            [MarshalAs(UnmanagedType.Struct)]
            public SigTypeU SigVal;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BP_SigTable
        {
            public UInt16 SigId;
            [ BitfieldLength(4)]
            public UInt16 SigType;
            [BitfieldLength(1)]
            public UInt16 EnStatics;
            [BitfieldLength(1)]
            public UInt16 IsDisplay;
            [BitfieldLength(3)]
            public UInt16 Accuracy;
            [BitfieldLength(1)]
            public UInt16 EnAlarm;
            [BitfieldLength(1)]
            public UInt16 Perm;
            [BitfieldLength(3)]
            public UInt16 AlmClass;
            [BitfieldLength(1)]
            public UInt16 HasCustomInfo;
            [BitfieldLength(1)]
            public UInt16 Reserved;

            public IntPtr MinVal;
            public IntPtr MaxVal;
            public IntPtr DefVal;
            public char DelayBeforeAlm;
            public char DelayAfterAlm;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BP_CusLangMap
        {
            public UInt16 SigId;
            public UInt16 LangId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BP_EnumSignalMap
        {
            public UInt16 Key;
            public Int32 Val;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BP_SigId2EnumSignalMap
        {
            public UInt16 SigId;
            public IntPtr EnumSignalMap; // BP_EnumSignalMap * EnumSignalMap;
            public UInt64 EnumSignalMapNum;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BP_SysCustomUnit
        {
            public UInt16 SidId;
            public char CustomType;
            public IntPtr CustomValue;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BP_SysSigMap
        {
            public char Dist;
            public char SigMapSize;
            public IntPtr SigMap;
        }

        [DllImport("bplib.dll")]
        public static extern IntPtr getBPContextEmbeded();

        [DllImport(@"bplib.dll", EntryPoint = "strlen_bp", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 strlen_bp(string str);

        [DllImport(@"bplib.dll", EntryPoint = "BP_InitEmbededContext", CallingConvention = CallingConvention.Cdecl)]
        public static extern void BP_InitEmbededContext();

        [DllImport(@"bplib.dll", EntryPoint = "BP_Init2Default", CallingConvention = CallingConvention.Cdecl)]
        public static extern void BP_Init2Default(ref BPContext bp_context);

        [DllImport(@"bplib.dll", EntryPoint = "BP_InitPackBuf", CallingConvention = CallingConvention.Cdecl)]
        public static extern void BP_InitPackBuf(ref PackBuf pack_buf, IntPtr buf, ref UInt64 buf_size);

        [DllImport(@"bplib.dll", EntryPoint = "BP_PackConnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_PackConnect(ref BPContext bp_context, IntPtr name, IntPtr password);

        [DllImport(@"bplib.dll", EntryPoint = "BP_PackPing", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_PackPing(ref BPContext bp_context);

        [DllImport(@"bplib.dll", EntryPoint = "BP_PackDisconn", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_PackDisconn(ref BPContext bp_context);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetSysSigId2ValTable", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetSysSigId2ValTable(IntPtr sys_sig_id_2_val, ref UInt64 cus_sig_num);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetSysSigTable", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetSysSigTable(IntPtr sys_sig_table);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetSysCustomUnitTable", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern IntPtr BP_SetSysCustomUnitTable(IntPtr sys_custom_unit_table, ref UInt64 sys_custom_unit_table_num);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetSysSigMap", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern IntPtr BP_SetSysSigMap(IntPtr sys_sig_map, ref UInt64 sys_sig_map_size);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetCusSigId2ValTable", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetCusSigId2ValTable(IntPtr cus_sig_id_2_val, ref UInt64 cus_sig_num);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetCusSigTable", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetCusSigTable(IntPtr cus_sig_table);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetCusSigNameLang", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetCusSigNameLang(IntPtr cus_sig_name_lang, BP_WORD cus_sig_name_lang_size);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetCusSigUnitLang", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetCusSigUnitLang(IntPtr cus_sig_unit_lang, BP_WORD cus_sig_unit_lang_size);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetCusSigGroupLang", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetCusSigGroupLang(IntPtr cus_sig_group_lang, BP_WORD cus_sig_group_lang_size);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetCusSigEnumLang", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetCusSigEnumLang(IntPtr cus_sig_enum_lang, BP_WORD cus_sig_enum_lang_size);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetCusSigNameLangMap", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetCusSigNameLangMap(IntPtr cus_sig_name_lang_map, ref UInt64 cus_sig_name_lang_map_num);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetCusSigUnitLangMap", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetCusSigUnitLangMap(IntPtr cus_sig_unit_lang_map, ref UInt64 cus_sig_unit_lang_map_num);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetCusSigGroupLangMap", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetCusSigGroupLangMap(IntPtr cus_sig_group_lang_map, ref UInt64 cus_sig_group_lang_map_num);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetCusSigEnumLangMap", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetCusSigEnumLangMap(IntPtr cus_sig_enum_lang_map, ref UInt64 cus_sig_enum_lang_map_num);
    }
}
