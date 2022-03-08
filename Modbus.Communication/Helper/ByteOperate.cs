using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Modbus.Communication.Helper
{
    public class ByteOperate
    {

        /// <summary>
        /// Byte 数组转十六进制字符串
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public string ByteToHex(byte[] Bytes)
        {
            string str = string.Empty;
            foreach (byte Byte in Bytes)
            {
                str += String.Format("{0:X2}", Byte) + " ";
            }
            return str.Trim();
        }

        /// <summary>
        /// 字符串转十六进制Byte数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] strToToHexByte(string hexString)
        {
            try
            {
                hexString = hexString.Replace(" ", "");
                if ((hexString.Length % 2) != 0)
                    hexString += " ";
                byte[] returnBytes = new byte[hexString.Length / 2];
                for (int i = 0; i < returnBytes.Length; i++)
                    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                return returnBytes;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// 截取Byte数组
        /// </summary>
        /// <param name="byteArr"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte[] SubByteArray(byte[] byteArr, int start, int len)
        {
            byte[] Res = new byte[len];
            if (byteArr != null && byteArr.Length > len)
            {
                for (int i = 0; i < len; i++)
                {
                    Res[i] = byteArr[i + start];
                }
            }
            return Res;
        }

        private byte[] StructToBytes(Object structobj, int size)
        {
            byte[] tempBytes = new byte[size];
            IntPtr strcutIntptr = Marshal.AllocHGlobal(size);
            //将结构体拷贝到内存中
            Marshal.StructureToPtr(structobj, strcutIntptr, false);
            //从内存空间拷贝到byte数组中
            Marshal.Copy(strcutIntptr, tempBytes, 0, size);
            Marshal.FreeHGlobal(strcutIntptr);
            return tempBytes;

        }

        private object BytesToStruct(byte[] bytes, Type _type)
        {
            int _size = Marshal.SizeOf(_type);
            IntPtr structInnptr = Marshal.AllocHGlobal(_size);
            Marshal.Copy(bytes, 0, structInnptr, _size);
            object obj = Marshal.PtrToStructure(structInnptr, _type);
            Marshal.FreeHGlobal(structInnptr);
            return obj;
        }

        private byte[] IntptrToBytes(IntPtr tempIntptr, int _size)
        {
            byte[] tempBytes = new byte[_size];
            //从内存空间拷贝到byte数组中
            Marshal.Copy(tempIntptr, tempBytes, 0, _size);
            return tempBytes;
        }

        private IntPtr BytesToInptr(byte[] bytes, Type _type)
        {
            int _size = Marshal.SizeOf(_type);
            IntPtr structInnptr = Marshal.AllocHGlobal(_size);
            Marshal.Copy(bytes, 0, structInnptr, _size);
            return structInnptr;
        }


    }
}
