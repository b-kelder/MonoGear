using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Network
{
    /// <summary>
    /// Network message
    /// FORMAT:
    /// | source | command | dataLength | data |
    /// 1        2         3            5      5 + dataLength
    /// </summary>
    public class NetworkMessage
    {
        public byte source;
        public byte command;
        public ushort dataLength;
        public List<byte> data;

        public NetworkMessage()
        {
            source = 0;
            command = Protocol.NET_COM_PING;
            dataLength = 0;
            data = new List<byte>();
        }

        /// <summary>
        /// Adds an integer to the message
        /// </summary>
        /// <param name="value">Value to add</param>
        public void AddInteger(int value)
        {
            dataLength += 4;
            data.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Adds a string to the message
        /// </summary>
        /// <param name="str">The string</param>
        public void AddString(string str)
        {
            // Add the string (null terminated)
            var c = Encoding.UTF8.GetBytes(str);
            dataLength += (ushort)(c.Length + 1);
            data.AddRange(c);
            data.Add(0x00);
        }

        /// <summary>
        /// Adds a SyncValue to the message
        /// </summary>
        /// <param name="value"></param>
        public void AddSyncValue(string name, SyncValue value)
        {
            // Add the string (null terminated)
            var c = Encoding.UTF8.GetBytes(name);
            dataLength += (ushort)(c.Length + 1);
            data.AddRange(c);
            data.Add(0x00);

            // Add the value
            var b = value.GetBytes();
            if(b.Length > 0xFF)
            {
                throw new ArgumentOutOfRangeException("Length too large for value with name " + name);
            }
            dataLength += (ushort)(b.Length + 1);
            // Add length of the value
            data.Add((byte)b.Length);
            // Add the value
            data.AddRange(b);
        }

        public static NetworkMessage FromBytes(byte[] bytes)
        {
            NetworkMessage msg = new NetworkMessage();
            msg.source = bytes[0];
            msg.command = bytes[1];
            msg.dataLength = BitConverter.ToUInt16(bytes, 2);
            for(int i = 0; i < msg.dataLength; i++)
            {
                msg.data.Add(bytes[i]);
            }
            return msg;
        }

        public byte[] GetBytes()
        {
            List<byte> totalBytes = new List<byte>();
            totalBytes.Add(source);
            totalBytes.Add(command);
            totalBytes.AddRange(BitConverter.GetBytes(dataLength));
            totalBytes.AddRange(data);
            return totalBytes.ToArray();
        }
    }
}
