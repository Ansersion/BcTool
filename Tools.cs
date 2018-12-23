using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BcTool
{
    public class Tools
    {

        public struct SignalTableMetaData
        {
            public string version;
            public int recordNum;
        }

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

        public static IntPtr mallocIntPtr(BPLibApi.BP_SigId2EnumSignalMap[] sigId2EnumSignalMapArray)
        {
            IntPtr ret = IntPtr.Zero;
            if (null == sigId2EnumSignalMapArray || 0 == sigId2EnumSignalMapArray.Length)
            {
                return ret;
            }
            try
            {
                int size = Marshal.SizeOf(sigId2EnumSignalMapArray[0]);
                size *= sigId2EnumSignalMapArray.Length;
                ret = Marshal.AllocHGlobal(size);

                long LongPtr = ret.ToInt64(); // Must work both on x86 and x64
                unsafe
                {
                    for (int i = 0; i < sigId2EnumSignalMapArray.Length; i++)
                    {
                        BPLibApi.BP_SigId2EnumSignalMap* tmp = (BPLibApi.BP_SigId2EnumSignalMap*)LongPtr;
                        tmp->SigId = sigId2EnumSignalMapArray[i].SigId;
                        tmp->EnumSignalMap = sigId2EnumSignalMapArray[i].EnumSignalMap;
                        tmp->EnumSignalMapNum = sigId2EnumSignalMapArray[i].EnumSignalMapNum;
                        LongPtr += Marshal.SizeOf(sigId2EnumSignalMapArray[0]);
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

        public static IntPtr mallocIntPtr(UInt16 sigId, char customType, object customValue)
        {
            IntPtr ret = IntPtr.Zero;
            try
            {
                switch(customType)
                {
                    case BPLibApi.SYS_SIG_CUSTOM_TYPE_DEF_VAL:
                        break;
                }
                /*
                ret = Marshal.AllocHGlobal(data.Length + 1); // 1: for '\0'
                byte[] bytes = Tools.addCStringEndFlag(System.Text.Encoding.UTF8.GetBytes(data));
                Marshal.Copy(bytes, 0, ret, bytes.Length);
                */
            }
            catch (Exception e)
            {
                freeIntPtr(ret);
                Console.Write(e.Message);
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

        public static IntPtr mallocIntPtr(string[] langArray)
        {
            IntPtr ret = IntPtr.Zero;
            if (null == langArray || 0 == langArray.Length)
            {
                return ret;
            }
            try
            {
                int size = Marshal.SizeOf(typeof(IntPtr));
                size *= langArray.Length;
                ret = Marshal.AllocHGlobal(size);

                long LongPtr = ret.ToInt64(); // Must work both on x86 and x64
                unsafe
                {
                    for (int i = 0; i < langArray.Length; i++)
                    {
                        IntPtr * tmp = (IntPtr * )LongPtr;
                        *tmp = mallocIntPtr(langArray[i]);
                        LongPtr += Marshal.SizeOf(typeof(IntPtr));
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

        public static IntPtr mallocIntPtr(BPLibApi.BP_EnumSignalMap enumSignalMap)
        {
            IntPtr ret = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(enumSignalMap);
                ret = Marshal.AllocHGlobal(size);

                long LongPtr = ret.ToInt64(); // Must work both on x86 and x64
                unsafe
                {
                    BPLibApi.BP_EnumSignalMap* tmp = (BPLibApi.BP_EnumSignalMap*)LongPtr;
                    tmp->Key = enumSignalMap.Key;
                    tmp->Val = enumSignalMap.Val;
                }
            }
            catch (Exception e)
            {
                freeIntPtr(ret);
                Console.Write(e.Message);
            }
            return ret;
        }

        public static IntPtr mallocIntPtr(BPLibApi.BP_CusLangMap[] langMap)
        {
            IntPtr ret = IntPtr.Zero;
            if (null == langMap || 0 == langMap.Length)
            {
                return ret;
            }
            try
            {
                int size = Marshal.SizeOf(typeof(BPLibApi.BP_CusLangMap));
                size *= langMap.Length;
                ret = Marshal.AllocHGlobal(size);

                long LongPtr = ret.ToInt64(); // Must work both on x86 and x64
                unsafe
                {
                    for (int i = 0; i < langMap.Length; i++)
                    {
                        BPLibApi.BP_CusLangMap* tmp = (BPLibApi.BP_CusLangMap*)LongPtr;
                        tmp->SigId = langMap[i].SigId;
                        tmp->LangId = langMap[i].LangId;
                        LongPtr += Marshal.SizeOf(typeof(BPLibApi.BP_CusLangMap));
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

        public static IntPtr mallocIntPtr(byte[] mem, int offset, int size)
        {
            IntPtr ret = IntPtr.Zero;
            if (null == mem)
            {
                return ret;
            }
            if(offset < 0 || size <= 0)
            {
                return ret;
            }
            if(offset + size > mem.Length)
            {
                return ret;
            }
            
            try
            {
                ret = Marshal.AllocHGlobal(size);

                long LongPtr = ret.ToInt64(); // Must work both on x86 and x64
                unsafe
                {
                    int end = offset + size;
                    for (int i = offset; i < end; i++)
                    {
                        byte* tmp = (byte*)LongPtr;
                        *tmp = mem[i];
                        LongPtr += Marshal.SizeOf(typeof(byte));
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

        public static IntPtr mallocIntPtr(List<BPLibApi.BP_SysSigMap> bpSysSigMapList)
        {
            IntPtr ret = IntPtr.Zero;
            if (null == bpSysSigMapList)
            {
                return ret;
            }
            if (bpSysSigMapList.Count <= 0)
            {
                return ret;
            }

            try
            {
                int size = Marshal.SizeOf(typeof(BPLibApi.BP_SysSigMap));
                size *= bpSysSigMapList.Count;
                ret = Marshal.AllocHGlobal(size);

                long LongPtr = ret.ToInt64(); // Must work both on x86 and x64
                unsafe
                {
                    for (int i = 0; i < bpSysSigMapList.Count; i++)
                    {
                        BPLibApi.BP_SysSigMap* tmp = (BPLibApi.BP_SysSigMap *)LongPtr;
                        tmp->Dist = bpSysSigMapList[i].Dist;
                        tmp->SigMapSize = bpSysSigMapList[i].SigMapSize;
                        tmp->SigMap = bpSysSigMapList[i].SigMap;
                        LongPtr += Marshal.SizeOf(typeof(BPLibApi.BP_SysSigMap));
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

        public static void freeLangIntPtr(IntPtr ptr, BP_WORD size)
        {
            if (null == ptr || IntPtr.Zero == ptr)
            {
                return;
            }
            long LongPtr = ptr.ToInt64(); // Must work both on x86 and x64
            unsafe
            {
                for (BP_WORD i = 0; i < size; i++)
                {
                    IntPtr* tmpIntPtr = (IntPtr*)LongPtr;
                    Tools.freeIntPtr(*tmpIntPtr);
                    LongPtr += Marshal.SizeOf(typeof(IntPtr));
                }
            }
            Tools.freeIntPtr(ptr);
        }

        public static void freeIntPtr(IntPtr ptr)
        {
            if(null != ptr && IntPtr.Zero != ptr)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static void setDefaultValue(ref BPLibApi.BP_SigId2Val sigId2Value, SignalDataItem sdi)
        {
            if(null == sdi)
            {
                return;
            }
            sigId2Value.SigId = (UInt16)(sdi.SignalId);
            setSigTypeU(ref sigId2Value.SigVal, sdi.ValueType1, sdi.DefaultValue);

        }

        public static void setSysSignalEnableBits(ref List<byte[]> enableBits, UInt16 signalID)
        {
            if(signalID < BPLibApi.SYSTEM_START_SIGNAL_ID)
            {
                return;
            }
            int signalIDOffset = signalID - BPLibApi.SYSTEM_START_SIGNAL_ID;
            int dist = signalIDOffset / BPLibApi.SYSTEM_SIGNAL_STEP;
            int byteOffset = (signalIDOffset - dist * BPLibApi.SYSTEM_SIGNAL_STEP) / 8;
            int bitOffset = (signalIDOffset - dist * BPLibApi.SYSTEM_SIGNAL_STEP) % 8;
            enableBits[dist][byteOffset] |= (byte)((1 << bitOffset));

        }

        public static void setSigTable(ref BPLibApi.BP_SigTable sigTable, SignalDataItem sdi)
        {
            if(null == sdi)
            {
                return;
            }

            sigTable.SigId = (UInt16)(sdi.SignalId & 0xFFFF);
            setValueType(ref sigTable.SigType, sdi.ValueType1);
            sigTable.EnStatics = sdi.Statistics ? BPLibApi.BP_EN_STATISTICS : BPLibApi.BP_DIS_STATISTICS;
            sigTable.IsDisplay = sdi.Display ? BPLibApi.BP_NOT_DISPLAY : BPLibApi.BP_DISPLAY;
            sigTable.Accuracy = (UInt16)(sdi.Accuracy & 0xFFFF);
            sigTable.EnAlarm = sdi.Alarm ? BPLibApi.BP_EN_ALARM : BPLibApi.BP_DIS_ALARM;
            setPermission(ref sigTable.Perm, sdi.BcPermission1);
            setAlarmClass(ref sigTable.AlmClass, sdi.AlarmClass);
            /* TODO: set hasCustomInfo flag */
            // sigTable.HasCustomInfo = sdi.HasCustomInfo ? BPLibApi.BP_SYS_SIGNAL_CUSTOM : BPLibApi.BP_SYS_SIGNAL_NONE_CUSTOM;
            BPLibApi.SigTypeU sigTypeUTmp = new BPLibApi.SigTypeU();
            sigTypeUTmp.t_u32 = 0;
            setSigTypeU(ref sigTypeUTmp, sdi.ValueType1, sdi.MinValue);
            sigTable.MinVal = Marshal.AllocHGlobal(Marshal.SizeOf(sigTypeUTmp));
            Marshal.StructureToPtr(sigTypeUTmp, sigTable.MinVal, false);
            sigTypeUTmp.t_u32 = 0;
            setSigTypeU(ref sigTypeUTmp, sdi.ValueType1, sdi.MaxValue);
            sigTable.MaxVal = Marshal.AllocHGlobal(Marshal.SizeOf(sigTypeUTmp));
            Marshal.StructureToPtr(sigTypeUTmp, sigTable.MaxVal, false);
            sigTypeUTmp.t_u32 = 0;
            setSigTypeU(ref sigTypeUTmp, sdi.ValueType1, sdi.DefaultValue);
            sigTable.DefVal = Marshal.AllocHGlobal(Marshal.SizeOf(sigTypeUTmp));
            Marshal.StructureToPtr(sigTypeUTmp, sigTable.DefVal, false);

            sigTable.DelayBeforeAlm = (char)(sdi.AlarmBefDelay & 0xFF);
            sigTable.DelayAfterAlm = (char)(sdi.AlarmAftDelay & 0xFF);
        }

        public static void setAlarmClass(ref UInt16 alarmClass, SignalDataItem.BcAlarmClass bcAlarmClass)
        {
            switch(bcAlarmClass)
            {
                case SignalDataItem.BcAlarmClass.EMERGENCY:
                    alarmClass = BPLibApi.BP_SIGNAL_ALARM_CLASS_EMERGENCY;
                    break;
                case SignalDataItem.BcAlarmClass.SERIOUS:
                    alarmClass = BPLibApi.BP_SIGNAL_ALARM_CLASS_SERIOUS;
                    break;
                case SignalDataItem.BcAlarmClass.WARNING:
                    alarmClass = BPLibApi.BP_SIGNAL_ALARM_CLASS_WARNING;
                    break;
                case SignalDataItem.BcAlarmClass.NOTICE:
                    alarmClass = BPLibApi.BP_SIGNAL_ALARM_CLASS_NOTICE;
                    break;
                case SignalDataItem.BcAlarmClass.INFO:
                    alarmClass = BPLibApi.BP_SIGNAL_ALARM_CLASS_INFO;
                    break;
            }
        }

        public static void setPermission(ref UInt16 perm, SignalDataItem.BcPermission bcperm)
        {
            switch(bcperm)
            {
                case SignalDataItem.BcPermission.RO:
                    perm = BPLibApi.BP_SIGNAL_PERMISSION_RO;
                    break;
                case SignalDataItem.BcPermission.RW:
                    perm = BPLibApi.BP_SIGNAL_PERMISSION_RW;
                    break;
            }
        }

        public static void setValueType(ref UInt16 valueTypeUInt16, ValueType valueType)
        {
            if(null == valueType)
            {
                return;
            }
            switch(valueType)
            {
                case SignalDataItem.ValueType.U32:
                    valueTypeUInt16 = BPLibApi.BP_VALUE_TYPE_U32;
                    break;
                case SignalDataItem.ValueType.U16:
                    valueTypeUInt16 = BPLibApi.BP_VALUE_TYPE_U16;
                    break;
                case SignalDataItem.ValueType.I32:
                    valueTypeUInt16 = BPLibApi.BP_VALUE_TYPE_I32;
                    break;
                case SignalDataItem.ValueType.I16:
                    valueTypeUInt16 = BPLibApi.BP_VALUE_TYPE_I16;
                    break;
                case SignalDataItem.ValueType.ENUM:
                    valueTypeUInt16 = BPLibApi.BP_VALUE_TYPE_ENUM;
                    break;
                case SignalDataItem.ValueType.FLOAT:
                    valueTypeUInt16 = BPLibApi.BP_VALUE_TYPE_FLT;
                    break;
                case SignalDataItem.ValueType.STRING:
                    valueTypeUInt16 = BPLibApi.BP_VALUE_TYPE_STR;
                    break;
                case SignalDataItem.ValueType.BOOLEAN:
                    valueTypeUInt16 = BPLibApi.BP_VALUE_TYPE_BOOL;
                    break;
            }
        }

        public static void setSigTypeU(ref BPLibApi.SigTypeU sigTypeU, SignalDataItem.ValueType valueType, Object value)
        {
            if(null == valueType)
            {
                return;
            }
            if(null == value)
            {
                return;
            }
            switch(valueType)
            {
                case SignalDataItem.ValueType.U32:
                    sigTypeU.t_u32 = (UInt32)value;
                    break;
                case SignalDataItem.ValueType.U16:
                    sigTypeU.t_u16 = (UInt16)value;
                    break;
                case SignalDataItem.ValueType.I32:
                    sigTypeU.t_i32 = (Int32)value;
                    break;
                case SignalDataItem.ValueType.I16:
                    sigTypeU.t_i16 = (Int16)value;
                    break;
                case SignalDataItem.ValueType.ENUM:
                    sigTypeU.t_enm = (UInt16)value;
                    break;
                case SignalDataItem.ValueType.FLOAT:
                    sigTypeU.t_flt = (float)value;
                    break;
                case SignalDataItem.ValueType.STRING:
                    /* TODO */
                    // sigTypeU.t_str = (UInt32)value;
                    break;
                case SignalDataItem.ValueType.BOOLEAN:
                    sigTypeU.t_bool = (char)value;
                    break;
                case SignalDataItem.ValueType.MEM:
                    break;
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

        public static Boolean initBP_SigTable(ref BPLibApi.BP_SigTable sigTable, UInt16 sigId, UInt16 sigType, Boolean enStatistics, Boolean enDisplay, UInt16 accuracy, Boolean enAlarm, UInt16 perm, UInt16 alarmClass, Boolean hasCustomInfo, BPLibApi.SigTypeU min, BPLibApi.SigTypeU max, BPLibApi.SigTypeU def, char dba, char daa)
        {

            Boolean ret = false;

            sigTable.SigId = sigId;
            if(sigType > BPLibApi.SIG_TYPE_MAX_INDEX)
            {
                return ret;
            }
            sigTable.SigType = sigType;
            sigTable.EnStatics = enStatistics ? (ushort)1 : (ushort)0;
            sigTable.IsDisplay = enDisplay ? (ushort)1 : (ushort)0;
            if(accuracy > BPLibApi.MAX_ACCURACY)
            {
                accuracy = BPLibApi.MAX_ACCURACY;
            }
            sigTable.Accuracy = accuracy;
            sigTable.EnAlarm = enAlarm ? (ushort)1 : (ushort)0;
            sigTable.Perm = perm > 0 ? (ushort)1 : (ushort)0;
            if(alarmClass > BPLibApi.MAX_ALARM_CLASS_INDEX)
            {
                alarmClass = BPLibApi.INVALID_ALARM_CLASS;
            }

            sigTable.MinVal = Marshal.AllocHGlobal(Marshal.SizeOf(min));
            Marshal.StructureToPtr(min, sigTable.MinVal, false);
            sigTable.MaxVal = Marshal.AllocHGlobal(Marshal.SizeOf(max));
            Marshal.StructureToPtr(max, sigTable.MaxVal, false);
            sigTable.DefVal = Marshal.AllocHGlobal(Marshal.SizeOf(def));
            Marshal.StructureToPtr(def, sigTable.DefVal, false);

            sigTable.DelayBeforeAlm = dba;
            sigTable.DelayAfterAlm = daa;

            ret = true;
            return ret;

        }

        public static string signalTableInfoParser(string header, string signalTableFirstLine)
        {
            string ret = "";
            try
            {
                string pattern = @"<" + header + @">" + @"(.+)" + @"</" + header + @">";
                Regex tmp = new Regex(pattern);

                Match mat = tmp.Match(signalTableFirstLine);
                if (null == mat || mat.Groups.Count < 2)
                {
                    return ret;
                }

                ret = mat.Groups[1].Value;

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return ret;
        }

        public void destroy(BPLibApi.BP_SigTable bpSigTable)
        {
            try
            {
                freeIntPtr(bpSigTable.MinVal);
                freeIntPtr(bpSigTable.MaxVal);
                freeIntPtr(bpSigTable.DefVal);
            }
            catch(Exception e)
            {
                // e.Message
            }
            
        }

    }
}
