using Microsoft.Xna.Framework;
using MonoGear.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Network
{
    public class NetworkEntity
    {
        /// <summary>
        /// The WorldEntity this object represents
        /// </summary>
        public WorldEntity WorldEntity { get; protected set; }
        /// <summary>
        /// The owner of this object
        /// </summary>
        public NetworkPlayer Owner { get; set; }
        /// <summary>
        /// Identifier of this entity, it is the same on all clients
        /// </summary>
        public int Identifier { get; set; }
        /// <summary>
        /// Enabled entities get updated, disabled entities do not
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Returns true if we are authorative over this entity
        /// </summary>
        public bool IsAuthorative
        {
            get
            {
                return Owner == NetManager.LocalPlayer;
            }
        }

        private Dictionary<string, SyncValue> syncData;

        public NetworkEntity(WorldEntity entity)
        {
            WorldEntity = entity;
            syncData = new Dictionary<string, SyncValue>();
            Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Update(ref NetworkMessage message)
        {
            if(!Enabled)
            {
                return;
            }
            if(IsAuthorative)
            {
                // We are authorative, we may modify the message and it will be send to other players
                message.command = Protocol.GAME_COM_UPDATE;
                message.AddInteger(Identifier);

                foreach(var data in syncData)
                {
                    // Check to see if a value has changed
                    if(data.Value.HasChanged)
                    {
                        data.Value.HasChanged = false;
                        message.AddSyncValue(data.Key, data.Value);
                    }
                }
            }
            else
            {
                // We may only update our own state
                int index = 4;
                List<byte> nameBytes = new List<byte>();
                string valueName = "";
                SyncValue value;
                while(index < message.dataLength)
                {
                    // Extract a string from the message
                    nameBytes.Clear();
                    while(message.data[index] != 0)
                    {
                        nameBytes.Add(message.data[index]);
                        index++;
                    }
                    // Move past the terminator
                    index++;
                    valueName = Encoding.UTF8.GetString(nameBytes.ToArray());
                    // Extract the value
                    // First byte is the length
                    byte length = message.data[index];
                    byte[] data = new byte[length];
                    int di = 0;
                    while(di < length)
                    {
                        data[di] = message.data[index + di];
                        di++;
                    }
                    // Update index
                    index += length;
                    syncData.TryGetValue(valueName, out value);
                    if(value != null)
                    {
                        // Finally update the value itself
                        value.SetBytes(data);
                    }
                    else
                    {
                        // Shouldn't happen
                        // TODO: Log
                    }
                }
            }
        }

        /// <summary>
        /// Sets a vector2 sync value with the given name
        /// </summary>
        /// <param name="key">The name of the value</param>
        /// <param name="value">The value</param>
        /// <returns>True on success</returns>
        public bool SetSyncValue(string key, Vector2 value)
        {
            if(!syncData.ContainsKey(key))
            {
                syncData.Add(key, new SyncVector2(value));
                return true;
            }
            else
            {
                var v = syncData[key] as SyncVector2;
                if(v != null)
                {
                    v.SetValue(value);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sets a float sync value with the given name
        /// </summary>
        /// <param name="key">The name of the value</param>
        /// <param name="value">The value</param>
        /// <returns>True on success</returns>
        public bool SetSyncValue(string key, float value)
        {
            if(!syncData.ContainsKey(key))
            {
                syncData.Add(key, new SyncFloat(value));
                return true;
            }
            else
            {
                var v = syncData[key] as SyncFloat;
                if(v != null)
                {
                    v.SetValue(value);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sets an int sync value with the given name
        /// </summary>
        /// <param name="key">The name of the value</param>
        /// <param name="value">The value</param>
        /// <returns>True on success</returns>
        public bool SetSyncValue(string key, int value)
        {
            if(!syncData.ContainsKey(key))
            {
                syncData.Add(key, new SyncInt32(value));
                return true;
            }
            else
            {
                var v = syncData[key] as SyncInt32;
                if(v != null)
                {
                    v.SetValue(value);
                    return true;
                }
            }
            return false;
        }

        public SyncValue GetSyncValue(string key)
        {
            SyncValue v;
            syncData.TryGetValue(key, out v);
            return v;
        }

        public int GetSyncInt(string key)
        {
            var v = GetSyncValue(key) as SyncInt32;
            if(v != null)
            {
                return v.GetValue();
            }
            return 0;
        }

        public float GetSyncFloat(string key)
        {
            var v = GetSyncValue(key) as SyncFloat;
            if(v != null)
            {
                return v.GetValue();
            }
            return 0;
        }

        public Vector2 GetSyncVector2(string key)
        {
            var v = GetSyncValue(key) as SyncVector2;
            if(v != null)
            {
                return v.GetValue();
            }
            return 0;
        }
    }
}
