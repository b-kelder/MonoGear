using MonoGear.Engine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Network
{
    public class NetManager
    {
        public static NetworkPlayer LocalPlayer { get; private set; }
        public static bool IsNetworkGame { get; private set; }
        private static bool keepRunning;
        private static int id = 912;

        private const int TXPORT = 5000;
        private const int RXPORT = 5001;

        private static ConcurrentQueue<NetworkMessage> txMessages = new ConcurrentQueue<NetworkMessage>();
        private static ConcurrentQueue<NetworkMessage> rxMessages = new ConcurrentQueue<NetworkMessage>();
        private static List<NetworkPlayer> players = new List<NetworkPlayer>();
        private static IPEndPoint txEndpoint;

        #region Transmission threads
        private async static void TxThread()
        {
            using(UdpClient txClient = new UdpClient(TXPORT))
            {
                NetworkMessage msg;
                byte[] bytes;
                while(keepRunning)
                {
                    // TODO: TImeout n shit
                    if(txMessages.TryDequeue(out msg))
                    {
                        bytes = msg.GetBytes();
                        await txClient.SendAsync(bytes, bytes.Length, txEndpoint);
                    }
                }
            }
        }

        private async static void RxThread()
        {
            using(UdpClient rxClient = new UdpClient(RXPORT))
            {
                while(keepRunning)
                {
                    // TODO: Timeout or something so we can stop this madness
                    var result = await rxClient.ReceiveAsync();
                    rxMessages.Enqueue(NetworkMessage.FromBytes(result.Buffer));
                }
            }
        }
        #endregion

        public static void Start(IPEndPoint server)
        {
            txEndpoint = server;
            keepRunning = true;
            Task.Run(new Action(RxThread));
            Task.Run(new Action(TxThread));
        }

        public static void Stop()
        {
            keepRunning = false;
        }

        public static void OnUpdate(IReadOnlyCollection<WorldEntity> removedEntities,
            IReadOnlyCollection<WorldEntity> newLocalEntities,
            IReadOnlyCollection<WorldEntity> newGlobalEntities,
            IReadOnlyCollection<WorldEntity> localEntities,
            IReadOnlyCollection<WorldEntity> globalEntities)
        {
            // Handle despawns
            foreach(var ent in removedEntities)
            {
                DespawnEntity(ent);
            }

            // Handle spawns
            foreach(var ent in newLocalEntities)
            {
                SpawnEntity(ent);
                UpdateEntity(ent);
            }

            foreach(var ent in newGlobalEntities)
            {
                SpawnEntity(ent);
                UpdateEntity(ent);
            }

            // Handle updates
            foreach(var ent in globalEntities)
            {
                UpdateEntity(ent);
            }

            foreach(var ent in localEntities)
            {
                UpdateEntity(ent);
            }

            // Handle incoming stuff
            HandleIncomingMessages(localEntities);
        }

        private static void HandleIncomingMessages(IEnumerable<WorldEntity> localEntities)
        {
            NetworkMessage msg;
            // TODO: Add time limit? Although that will mean we can start lagging behind A LOT
            while(rxMessages.Count > 0)
            { 
                if(rxMessages.TryDequeue(out msg))
                {
                    switch(msg.command)
                    {
                        // Net
                        // Game
                        case Protocol.GAME_COM_SPAWN:
                            HandleSpawnMessage(msg);
                            break;
                        case Protocol.GAME_COM_DESPAWN:
                            HandleDespawnMessage(msg, localEntities);
                            break;
                        case Protocol.GAME_COM_UPDATE:
                            HandleUpdateMessage(msg, localEntities);
                            break;
                        default:
                            // TODO: Log? Can we do that?
                            break;
                    }
                }
            }
        }

        private static void HandleSpawnMessage(NetworkMessage msg)
        {
            if(msg.dataLength >= 6)
            {
                // Id (4) + non-empty string (2) is in the message
                // Extract the string from the message
                List<byte> nameBytes = new List<byte>();
                int i = 4;
                while(msg.data[i] != 0)
                {
                    nameBytes.Add(msg.data[i]);
                    i++;
                }
                // Spawn the entity
                WorldEntity ent = EntityTypeDatabase.Instantiate(Encoding.UTF8.GetString(nameBytes.ToArray()));
                if(ent != null)
                {
                    ent.NetworkEntity.Owner = GetNetworkPlayer(msg.source);
                    // Id is the first part of the data
                    ent.NetworkEntity.Identifier = BitConverter.ToInt32(msg.data.ToArray(), 0);
                    // TODO: Support for global entities? Is that even needed?
                    MonoGearGame.SpawnLevelEntity(ent);
                }
                else
                {
                    // FUCK FUCK FUCK
                    // TODO: LOGGING
                }
            }
        }

        private static void HandleDespawnMessage(NetworkMessage msg, IEnumerable<WorldEntity> localEntities)
        {
            if(msg.dataLength >= 4)
            {
                // Message contains id
                var src = GetNetworkPlayer(msg.source);
                // Id is the first thing in the data
                int id = BitConverter.ToInt32(msg.data.ToArray(), 0);

                // Find the entity and destroy it
                foreach(var ent in localEntities)
                {
                    if(ent.NetworkEntity.Identifier == id && ent.NetworkEntity.Owner == src)
                    {
                        MonoGearGame.DestroyEntity(ent);
                        break;
                    }
                }
            }
        }

        private static void HandleUpdateMessage(NetworkMessage msg, IEnumerable<WorldEntity> localEntities)
        {
            if(msg.dataLength >= 4)
            {
                // Message contains id
                // Id is the first thing in the data
                int id = BitConverter.ToInt32(msg.data.ToArray(), 0);

                // Find the entity and update it
                foreach(var ent in localEntities)
                {
                    if(ent.NetworkEntity.Identifier == id)
                    {
                        ent.NetworkEntity.Update(ref msg);
                        break;
                    }
                }
            }
        }

        private static void DespawnEntity(WorldEntity ent)
        {
            if(ent.NetworkEntity.Enabled && ent.NetworkEntity.IsAuthorative)
            {
                NetworkMessage msg;
                // it is enabled and we are in control, send spawn message
                msg = new NetworkMessage();
                msg.command = Protocol.GAME_COM_DESPAWN;
                msg.AddInteger(ent.NetworkEntity.Identifier);
                txMessages.Enqueue(msg);
            }
        }

        private static void SpawnEntity(WorldEntity ent)
        {
            if(ent.NetworkEntity.Enabled && ent.NetworkEntity.IsAuthorative)
            {
                ent.NetworkEntity.Identifier = GetNewIdentifier();
                NetworkMessage msg;
                // it is enabled and we are in control, send spawn message
                msg = new NetworkMessage();
                msg.command = Protocol.GAME_COM_SPAWN;
                msg.AddInteger(ent.NetworkEntity.Identifier);
                // TODO: Maybe not use the entire bloody classname
                msg.AddString(ent.GetType().AssemblyQualifiedName);
                txMessages.Enqueue(msg);
            }
        }

        private static void UpdateEntity(WorldEntity ent)
        {
            if(ent.NetworkEntity.Enabled && ent.NetworkEntity.IsAuthorative)
            {
                NetworkMessage msg;
                // it is enabled and we are in control
                msg = new NetworkMessage();
                ent.NetworkEntity.Update(ref msg);
                // Only send message if entity deemed it necessary
                if(msg.command == Protocol.GAME_COM_UPDATE)
                {
                    txMessages.Enqueue(msg);
                }
            }
        }

        public static NetworkPlayer GetNetworkPlayer(byte id)
        {
            foreach(var player in players)
            {
                if(player.Id == id)
                {
                    return player;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a new identifier that may be used for spawning a new NetworkEntity
        /// </summary>
        /// <returns></returns>
        public static int GetNewIdentifier()
        {
            // This gives us room to spawn a million things without overlapping into anyone else's space.
            // That's enough, right?
            return (1000000 * LocalPlayer.Id) + id++;
        }
    }
}
