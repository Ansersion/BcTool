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
                freeIntPtr(ret);
                Console.Write(e.Message);
            }

            return ret;
        }

        
        public static IntPtr mallocIntPtr(BPLibApi.BP_SigId2Val[] array)
        {
            IntPtr ret = IntPtr.Zero;
            if(null == array || 0 == array.Length)
            {
                return ret;
            }
            try
            {
                int size = Marshal.SizeOf(array[0]);
                size *= array.Length;
                ret = Marshal.AllocHGlobal(size);

                long LongPtr = ret.ToInt64(); // Must work both on x86 and x64
                unsafe
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        BPLibApi.BP_SigId2Val* tmp = (BPLibApi.BP_SigId2Val*)LongPtr;
                        tmp->SigId = array[i].SigId;
                        tmp->SigVal = array[i].SigVal;
                        LongPtr += Marshal.SizeOf(array[0]);
                    }
                }
            }
            catch(Exception e)
            {
                freeIntPtr(ret);
                Console.Write(e.Message);
            }
            return ret;
        }

        public static IntPtr mallocIntPtr(BPLibApi.BP_SigTable[] array)
        {
            IntPtr ret = IntPtr.Zero;
            if (null == array || 0 == array.Length)
            {
                return ret;
            }
            try
            {
                int size = Marshal.SizeOf(array[0]);
                size *= array.Length;
                ret = Marshal.AllocHGlobal(size);

                long LongPtr = ret.ToInt64(); // Must work both on x86 and x64
                unsafe
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        BPLibApi.BP_SigTable* tmp = (BPLibApi.BP_SigTable*)LongPtr;
                        tmp->SigId = array[i].SigId;
                        tmp->SigType = array[i].SigType;
                        tmp->EnStatics = array[i].EnStatics;
                        tmp->IsDisplay = array[i].IsDisplay;
                        tmp->Accuracy = array[i].Accuracy;
                        tmp->EnAlarm = array[i].EnAlarm;
                        tmp->Perm = array[i].Perm;
                        tmp->AlmClass = array[i].AlmClass;
                        tmp->HasCustomInfo = array[i].HasCustomInfo;
                        tmp->Reserved = array[i].Reserved;
                        tmp->MinVal = array[i].MinVal;
                        tmp->MaxVal = array[i].MaxVal;
                        tmp->DefVal = array[i].DefVal;
                        tmp->DelayBeforeAlm = array[i].DelayBeforeAlm;
                        tmp->DelayAfterAlm = array[i].DelayAfterAlm;
                        LongPtr += Marshal.SizeOf(array[0]);
                    }
                }
            }
            catch (Exception e)
            {
                freeIntPtr(ret);
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
