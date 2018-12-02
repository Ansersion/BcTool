using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BcTool
{
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

        [StructLayout(LayoutKind.Auto, CharSet = CharSet.Ansi)]
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
        public static extern IntPtr BP_SetSysSigId2ValTable(IntPtr cus_sig_id_2_val, ref UInt64 cus_sig_num);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetSysSigTable", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_SetSysSigTable(IntPtr cus_sig_table);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetSysCustomUnitTable", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern IntPtr BP_SetSysCustomUnitTable(IntPtr sys_custom_unit_table, ref UInt64 sys_custom_unit_table_num);

        [DllImport(@"bplib.dll", EntryPoint = "BP_SetSysSigMap", CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern IntPtr BP_SetSysSigMap(IntPtr sys_sig_map, ref UInt64 sys_sig_map_size);
    }
}
