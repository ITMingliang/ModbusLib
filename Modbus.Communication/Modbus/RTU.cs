using Modbus.Communication.Helper;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modbus.Communication
{
    public class RTU
    {
        public Action<int, List<byte>> ResponseData;

        private static RTU _instance;
        private static SerialInfo _serialInfo;
        CrcCheck c =new CrcCheck();
        ByteOperate bit =new ByteOperate();
        SerialPort _serialPort;
        bool _isBusing = false;

        int _currentSlave;
        int _funcCode;
        int _wordLen;
        int _startAddr;

        private RTU(SerialInfo serialInfo)
        {
            _serialPort = new SerialPort();
            _serialInfo = serialInfo;
        }

        public static RTU GetInstance(SerialInfo serialInfo)
        {
            lock ("rtu")
            {
                if (_instance == null)
                    _instance = new RTU(serialInfo);
                return _instance;
            }
        }


        public bool Connection()
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();

                _serialPort.PortName = _serialInfo.PortName;
                _serialPort.BaudRate = _serialInfo.BaudRate;
                _serialPort.DataBits = _serialInfo.DataBit;
                _serialPort.Parity = _serialInfo.Parity;
                _serialPort.StopBits = _serialInfo.StopBits;

                _serialPort.ReceivedBytesThreshold = 1;
                _serialPort.DataReceived += _serialPort_DataReceived;

                _serialPort.Open();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        int _receiveByteCount = 0;
        byte[] _byteBuffer = new byte[512];
        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte _receiveBytes;
            while (_serialPort.BytesToRead > 0)
            {
                _receiveBytes = (byte)_serialPort.ReadByte();
                _byteBuffer[_receiveByteCount] = _receiveBytes;
                _receiveByteCount++;
                if (_receiveByteCount >= 512)
                {
                    _receiveByteCount = 0;
                    //清除输入缓冲区
                    _serialPort.DiscardInBuffer();
                    return;
                }
            }

            if (_byteBuffer[0] == (byte)_currentSlave && _byteBuffer[1] == _funcCode && _receiveByteCount >= _wordLen + 5)
            {
                // 检查crc
                // ...........
                // 返回数据
                ResponseData?.Invoke(_startAddr, new List<byte>(bit.SubByteArray(_byteBuffer, 0, _wordLen + 3)));
                _serialPort.DiscardInBuffer();
            }
        }

        public async Task<bool> Send(int slaveAddr, byte funcCode, int startAddr, int len)
        {
            _currentSlave = slaveAddr;
            _funcCode = funcCode;
            _startAddr = startAddr;

            if (funcCode == 0x01)
                _wordLen = len / 8 + ((len % 8 > 0) ? 1 : 0);
            if (funcCode == 0x03)
                _wordLen = len * 2;

            List<byte> sendBuffer = new List<byte>();
            sendBuffer.Add((byte)slaveAddr);
            sendBuffer.Add(funcCode);
            sendBuffer.Add((byte)(startAddr / 256));
            sendBuffer.Add((byte)(startAddr % 256));
            sendBuffer.Add((byte)(len / 256));
            sendBuffer.Add((byte)(len % 256));

            byte[] crc = c.Crc16(sendBuffer.ToArray(), 6);
            sendBuffer.AddRange(crc);

            try
            {
                while (_isBusing) { }

                _isBusing = true;
                _serialPort.Write(sendBuffer.ToArray(), 0, 8);
                _isBusing = false;

                await Task.Delay(1000);
            }
            catch
            {
                return false;
            }
                _receiveByteCount = 0;
            return true;
        }

    }
}
