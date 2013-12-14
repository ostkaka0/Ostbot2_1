using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OstBot_2_1
{
    public class BotSystemHandler
    {
        //delegate BotSystem BotSystemConstructor(OstBot ostBot);
        private Dictionary<string, BotSystem> botSystems = new Dictionary<string, BotSystem>();

        public BotSystem getBotSystem(string name)
        {
            lock (botSystems)
            {
                if (botSystems.ContainsKey(name))
                    return botSystems[name];
                else
                    return null;
            }
        }

        public void AddBotSystem(BotSystem botSystem)
        {
            lock (botSystems)
                botSystems.Add(botSystem.GetType().ToString(), botSystem);

            Program.form1.Invoke(new Action(() =>
                {
                    Program.form1.checkedListBox_BotSystems.Items.Add(botSystem);
                    botSystem.id = Program.form1.checkedListBox_BotSystems.Items.Count - 1;
                    Program.form1.checkedListBox_BotSystems.SetItemChecked(botSystem.id, botSystem.enabled);
                }));
        }

        public  void RemoveBotSystem(BotSystem botSystem)
        {
            lock (botSystems)
                botSystems.Remove(botSystem.GetType().ToString());
        }

        public  void onMessage(object sender, OstBot ostBot, PlayerIOClient.Message m)
        {
            lock (botSystems)
            {
                foreach (var pair in botSystems)
                {
                    if (pair.Value != null)
                    {
                        new Task(() =>//new Thread(() =>
                            {
                                pair.Value.onMessage(sender, ostBot, m);
                            }).Start();
                    }
                }
            }
        }

        public  void OnDisconnect(object sender, string reason)
        {
            lock (botSystems)
            {
                foreach (var pair in botSystems)
                {
                    if (pair.Value != null)
                    {
                        new Task(() =>
                        {
                            pair.Value.onDisconnect(sender, reason);
                            pair.Value.onDisable();
                        }).Start();
                    }
                }

                System.Threading.Thread.Sleep(500);

                botSystems.Clear();

                Program.form1.Invoke(new Action(()=>
                    Program.form1.checkedListBox_BotSystems.Items.Clear()
                    ));
            }
        }

        public  void onCommand(object sender, string text, int userId, Player player, OstBot ostBot)
        {
            string[] args = text.Split(' ');

            string[] arg = text.ToLower().Split(' ');
            string name = "";
            /*Player player;

            lock (ostBot.playerList)
            {
                if (ostBot.playerList.ContainsKey(userId))
                {
                    player = ostBot.playerList[userId];
                    name = player.Name;
                }
                else
                {
                    player = new Player(PlayerIOClient.Message.Create("m", -1, "", 0, 0, 0, false, false, false, 0, false, false, 0)); //new EEPlayer(-1, "", 0, 0, 0, false, false, false, 0, false, false, 0);
                }
            }*/
            bool isBotMod = (name == "ostkaka" || name == "botost" || name == "gustav9797" || name == "gbot" || player.ismod || userId == -1);

            lock (botSystems)
            {
                foreach (var pair in botSystems)
                {
                    if (pair.Value != null)
                    {
                        //new Task(() =>//new Thread(() =>
                        //{
                            pair.Value.onCommand(sender, ostBot, text, args, userId, player, name, isBotMod);
                        //}).Start();
                    }
                }
            }
        }
    }
}
