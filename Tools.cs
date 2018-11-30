using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BcTool
{
    public class Tools
    {
        private static byte[] cStringEndFlag = new byte[1] { 0 };

        public static byte[] addCStringEndFlag(byte[] c_str)
        {
            byte[] ret;
            if (null == c_str)
            {
                ret = new byte[cStringEndFlag.Length];
                System.Buffer.BlockCopy(cStringEndFlag, 0, ret, 0, cStringEndFlag.Length);
            }
            else
            {
                ret = new byte[c_str.Length + cStringEndFlag.Length];
                System.Buffer.BlockCopy(c_str, 0, ret, 0, c_str.Length);
                System.Buffer.BlockCopy(cStringEndFlag, 0, ret, c_str.Length, cStringEndFlag.Length);
            }

            return ret;
        }

        public static IntPtr mallocIntPtr(string data)
        {
            IntPtr ret = IntPtr.Zero;
            try
            {
                ret = Marshal.AllocHGlobal(data.Length + 1); // 1: for '\0'
                byte[] bytes = Tools.addCStringEndFlag(System.Text.Encoding.UTF8.GetBytes(data));
                Marshal.Copy(bytes, 0, ret, bytes.Length);
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }


            return ret;
        }

        public static void freeIntPtr(IntPtr ptr)
        {
            if(null != ptr && IntPtr.Zero != ptr)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static byte[] packBuf2Bytes(ref BPLibApi.PackBuf packBuf)
        {
            byte[] ret = null;
            try
            {
                ret = new byte[packBuf.MsgSize];
                Marshal.Copy(packBuf.PackStart, ret, 0, ret.Length);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                ret = null;
            }


            return ret;
        }
    }
}
