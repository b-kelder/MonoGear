using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Network
{
    public static class Protocol
    {
        /*
         *  Network commands
         */
        public const byte NET_COM_CONFIRM = 0x01;
        public const byte NET_COM_CONNECT = 0x02;
        public const byte NET_COM_DISCONNECT = 0x03;
        public const byte NET_COM_PING = 0x04;

        /*
         *  Game commands
         */
        public const byte GAME_COM_SPAWN = 0x41;
        public const byte GAME_COM_DESPAWN = 0x42;
        public const byte GAME_COM_UPDATE = 0x43;
    }
}
