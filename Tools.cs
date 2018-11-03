using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcTool
{
    class Tools
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
    }
}
