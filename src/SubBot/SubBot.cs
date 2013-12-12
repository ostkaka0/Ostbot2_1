using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OstBot_2_1
{
    public abstract class SubBot
    {
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
                            Program.form1.checkedListBox_SubBots.SetItemChecked(id, value)
                        ));
                    }

                    if (value)
                    {
                        Program.console.WriteLine(this.GetType().Name + ".cs is enabled.");
                    }
                    else
                    {
                        Program.console.WriteLine(this.GetType().Name + ".cs is disabled.");
                    }
                }
            }
        }

        public SubBot(OstBot ostBot)
        {
            new Task(() =>
                {
                    try
                    {
                        while (!ostBot.connected)
                            Thread.Sleep(1000);

                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        while (ostBot.connected)
                        {
                            while (enabled)
                            {
                                Update();
                                int time = (int)stopwatch.ElapsedMilliseconds;
                                if (time < UpdateSleep)
                                    Thread.Sleep(UpdateSleep - time);
                                stopwatch.Reset();
                            }
                            Thread.Sleep(200);
                        }
                    }
                    catch (Exception e)
                    {
                        ostBot.shutdown();
                        throw e;
                    }
                }).Start();
        }

        ~SubBot()
        {
        }

        public abstract void onMessage(object sender, OstBot ostBot, PlayerIOClient.Message m);
        public abstract void onDisconnect(object sender, string reason);
        public abstract void onCommand(object sender, string text, string[] args, int userId, /*_*//*Player player,*/ string name, bool isBotMod);
        public abstract void Update();

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}
