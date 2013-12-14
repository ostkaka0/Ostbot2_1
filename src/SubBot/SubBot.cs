using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OstBot_2_1
{
    public abstract class BotSystem
    {
        protected Task updateTask;
        protected int UpdateSleep = 200;
        public int id = -1;
        private bool enabledValue;
        public bool enabled
        {
            get { return enabledValue; }
            set
            {
                if (value != enabled)
                {
                    enabledValue = value;
                    if (id != -1)
                    {
                        Program.form1.Invoke(new Action(() =>
                            Program.form1.checkedListBox_BotSystems.SetItemChecked(id, value)
                        ));
                    }

                    if (value)
                    {
                        this.onEnable();
                        updateTask.Start();
                        Program.console.WriteLine(this.GetType().Name + ".cs is enabled.");
                    }
                    else
                    {
                        this.updateTask.Dispose();
                        this.onDisable();
                        Program.console.WriteLine(this.GetType().Name + ".cs is disabled.");
                    }
                }
            }
        }

        public BotSystem(OstBot ostBot)
        {
            updateTask = new Task(() =>
                {
                    try
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                            while (enabled)
                            {
                                Update();
                                int time = (int)stopwatch.ElapsedMilliseconds;
                                if (time < UpdateSleep)
                                    Thread.Sleep(UpdateSleep - time);
                                stopwatch.Reset();
                            }
                    }
                    catch (Exception e)
                    {
                        ostBot.shutdown();
                        throw e;
                    }
                });
        }

        ~BotSystem()
        {
        }

        public abstract void onEnable();
        public abstract void onDisable();
        public abstract void onMessage(object sender, OstBot ostBot, PlayerIOClient.Message m);
        public abstract void onDisconnect(object sender, string reason);
        public abstract void onCommand(object sender, OstBot ostBot, string text, string[] args, int userId, Player player, string name, bool isBotMod);
        public abstract void Update();

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}
