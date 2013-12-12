using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerIOClient;

namespace OstBot_2_1
{
    public class RoomConnection
    {
        Connection connection;
        RoomJoiner roomJoiner;
        string room;

        public RoomConnection(RoomJoiner roomJoiner, Client client, string room)
        {
                    try
                    {
                        this.roomJoiner = roomJoiner;
                        this.room = room;

                        Dictionary<string, string> roomData = new Dictionary<string, string>();
                        Dictionary<string, string> joinData = new Dictionary<string, string>();

                        connection = client.Multiplayer.JoinRoom(room, null);//client.Multiplayer.JoinRoom(room, new Dictionary<string,string>(), );
                        connection.OnMessage += new MessageReceivedEventHandler(OnMessage);
                        connection.OnDisconnect += new DisconnectEventHandler(OnDisconnect);

                        connection.Send("init");
                        connection.Send("init2");
                    }
                    catch (Exception e)
                    {
                        roomJoiner.OnConnectFailed(this);
                    }
        }

        public Connection Connection
        {
            get { return connection; }
        }

        public string Room
        {
            get { return room; }
            set { room = value; }
        }

       private void OnMessage(object sender, Message m)
       {
           roomJoiner.OnMessage(this, m);
       }

       private void OnDisconnect(object sender, string reason)
       {
           roomJoiner.OnDisconnect(this, reason);
       }

    }
}
