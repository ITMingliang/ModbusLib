using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modbus.Communication.Helper
{
    public class UnicodeOperate
    {
        // 转换接收到的字符串
        public string UTF8ToUnicode(string recvStr)
        {
            byte[] tempStr = Encoding.UTF8.GetBytes(recvStr);
            byte[] tempDef = Encoding.Convert(Encoding.UTF8, Encoding.Default, tempStr);
            string msgBody = Encoding.Default.GetString(tempDef);
            return msgBody;
        }
        // 转换要发送的字符数组
        public byte[] UnicodeToUTF8(string sendStr)
        {
            byte[] msgBody = Encoding.UTF8.GetBytes(sendStr);
            return msgBody;
        }
    }
}
