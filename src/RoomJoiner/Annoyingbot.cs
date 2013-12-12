using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PlayerIOClient;

namespace OstBot_2_1
{
    public class AnnoyingBot : RoomJoiner
    {
        private List<string> players = new List<string>();

        public AnnoyingBot(string gameId, string email, string password)
            : base(gameId, email, password)
        {
            //System.IO.StreamWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + "\\players\\lol.txt");
            //Console.WriteLine(System.DateTime.Now.ToShortDateString());
        }

        ~AnnoyingBot()
        {
            //File.WriteAllLines(Environment.CurrentDirectory + @"\players\" + System.DateTime.Now.ToLongTimeString() + ".txt",players.ToArray());
            //System.IO.StreamWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + "\\players\\"
                //+ System.DateTime.Now.ToShortDateString() + "_" + System.DateTime.Now.ToShortTimeString().Replace(":", "-") + ".txt");

            /*foreach (var s in players)
            {
                file.WriteLine(s);
            }

            file.Close();*/
        }

        public override void OnMessage(RoomConnection sender, Message m)
        {
            switch (m.Type)
            {
                case "init":
                    new Task(()=>
                        {
                    //Console.WriteLine(sender.Room);
                    //sender.Connection.Send("say", @"Hi, everybody! Yes, I am annoying.");
                    //System.Threading.Thread.Sleep(25);
                    sender.Connection.Send("say", @"Play Cheese Edits: https://www.mediafire.com/?9114ak36c52wqe6");
                    System.Threading.Thread.Sleep(100);
                    sender.Connection.Disconnect();
                    this.connections.Remove(sender.Room);
                        }).Start();
                    break;

                case "add":
                    if (!players.Contains(m.GetString(1).ToLower()))
                        players.Add(m.GetString(1).ToLower());
                    break;
            }
        }

        public List<string> Players
        {
            get { return players; }
        }
    }
}
