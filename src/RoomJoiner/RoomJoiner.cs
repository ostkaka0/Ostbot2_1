using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerIOClient;

namespace OstBot_2_1
{
    public abstract class RoomJoiner
    {
        protected Client client;
        protected Dictionary<string, RoomConnection> connections = new Dictionary<string, RoomConnection>();

        public RoomJoiner(string gameId, string email, string password)
        {
            client = PlayerIO.QuickConnect.SimpleConnect(gameId, email, password);
        }

        public void Connect(string roomType)
        {
            new Task(() =>
            {
                List<PlayerIOClient.RoomInfo> roomInfo = new List<RoomInfo>(client.Multiplayer.ListRooms(roomType, new Dictionary<string, string>(), 200000, 0));

                foreach (var room in roomInfo)
                {
                    if (!this.connections.ContainsKey(room.Id))
                        this.connections.Add(room.Id, new RoomConnection(this, client, room.Id));
                }
            }).Start();
        }

        public virtual List<PlayerIOClient.RoomInfo> FilterRooms(List<PlayerIOClient.RoomInfo> roomInfo)
        {
            for(int i = 0; i < roomInfo.Count; i++)
            {
                string plays = (!roomInfo[i].RoomData.ContainsKey("plays")) ? "" : roomInfo[i].RoomData["plays"];

                if (int.Parse(plays) < MinOnline)
                    roomInfo.RemoveAt(i);
            }

            return roomInfo;
        }

        public virtual void OnConnectFailed(RoomConnection sender)
        {
            connections.Remove(sender.Room);
        }

        public virtual void OnConnected(RoomConnection sender)
        {

        }

        public abstract void OnMessage(RoomConnection sender, Message m);

        public virtual void OnDisconnect(RoomConnection sender, string reason)
        {
            Console.WriteLine("{0} kicked by reason {1}", sender.Room, reason);
            connections.Remove(sender.Room);
        }

        protected virtual int MinOnline
        {
            get { return 5; }
        }

    }
}
