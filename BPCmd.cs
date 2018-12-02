using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BcTool
{
    class BPCmd
    {
        public byte[] bpConnect(ref BPLibApi.BPContext bPContext, string sn, string passwd, ref string err)
        {
            byte[] ret = null;
            if (String.IsNullOrWhiteSpace(sn))
            {
                err = "bpConnect: String.IsNullOrWhiteSpace(sn)";
                return ret;
            }
            if (null == passwd)
            {
                err = "bpConnect: null == passwd";
                return ret;
            }
            IntPtr snPtr = IntPtr.Zero;
            IntPtr passwordPtr = IntPtr.Zero;
            try
            {
                snPtr = Tools.mallocIntPtr(sn);
                passwordPtr = Tools.mallocIntPtr(passwd);

                IntPtr intPtrPack = BPLibApi.BP_PackConnect(ref bPContext, snPtr, passwordPtr);
                BPLibApi.PackBuf packBufSend = (BPLibApi.PackBuf)Marshal.PtrToStructure(intPtrPack, typeof(BPLibApi.PackBuf));
                ret = Tools.packBuf2Bytes(ref packBufSend);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                ret = null;
            }
            finally
            {
                Tools.freeIntPtr(snPtr);
                Tools.freeIntPtr(passwordPtr);
            }
            return ret;
        }

        public byte[] bpPing(ref BPLibApi.BPContext bPContext, ref string err)
        {
            byte[] ret = null;
            try
            {
                IntPtr intPtrPack = BPLibApi.BP_PackPing(ref bPContext);
                BPLibApi.PackBuf packBufSend = (BPLibApi.PackBuf)Marshal.PtrToStructure(intPtrPack, typeof(BPLibApi.PackBuf));
                ret = Tools.packBuf2Bytes(ref packBufSend);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                ret = null;
            }
            return ret;
        }

        public byte[] bpDisconn(ref BPLibApi.BPContext bPContext)
        {
            byte[] ret = null;
            try
            {
                IntPtr intPtrPack = BPLibApi.BP_PackDisconn(ref bPContext);
                BPLibApi.PackBuf packBufSend = (BPLibApi.PackBuf)Marshal.PtrToStructure(intPtrPack, typeof(BPLibApi.PackBuf));
                ret = Tools.packBuf2Bytes(ref packBufSend);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                ret = null;
            }
            return ret;
        }

        public void destroy()
        {

        }
    }
}
