using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BcTool
{
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

        [DllImport(@"bplib.dll", EntryPoint = "BP_PackDisconn", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BP_PackDisconn(ref BPContext bp_context);


    }
}
