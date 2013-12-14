﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerIOClient;
using System.IO;
using System.Runtime.Serialization;
/*
namespace OstBot_2_
{
    public class BotPlayer : Player
    {
        Stopwatch betaDigTimer = new Stopwatch();
        public Inventory inventory = new Inventory(100);
        protected int xp = 0;
        protected int xpRequired;
        protected int digLevel_ = 0;
        protected int digMoney_ = 0;
        protected bool betaDig = false;
        protected bool fastDig = true;
        protected int userId;
        protected string name;

        public BotPlayer(PlayerIOClient.Message m)
            : base(m.GetInt(0), m.GetString(1).ToLower(), m.GetInt(2), m.GetFloat(3), m.GetFloat(4), m.GetBoolean(5), m.GetBoolean(6), m.GetBoolean(7), m.GetInt(8), false, false, 0)
        {
            Load();
        }

        ~BotPlayer()
        {
            Save();
        }

        public void Save()
        {
            Pair<IFormatter, Stream> writeStuff = inventory.Save(@"data\" + name);
            writeStuff.first.Serialize(writeStuff.second, digXp);
            writeStuff.first.Serialize(writeStuff.second, digMoney);
            writeStuff.second.Close();
        }

        public void Load()
        {
            if (File.Exists(@"data\" + name))
            {
                digLevel_ = 0;
                Pair<IFormatter, Stream> writeStuff = inventory.Load(@"data\" + name);
                digXp = (int)writeStuff.first.Deserialize(writeStuff.second);
                digMoney_ = (int)writeStuff.first.Deserialize(writeStuff.second);
                writeStuff.second.Close();
                xpRequired = getXpRequired(digLevel);
            }
        }

        public int digRange { get { return ((digLevel_ > 0 && fastDig) ? 2 : 1) + ((betaDig) ? 1 : 0); } }

        public int xpRequired_ { get { return xpRequired; } }

        public int digLevel { get { return digLevel_; } }

        public int digMoney { get { return digMoney_; } set { digMoney_ = value; } }

        public int digStrength { get { return 1 + digLevel/4; } }

        private static int getXpRequired(int level) { return BetterMath.Fibonacci(level + 2) * 8; }

        public int digXp
        {
            get { return xp; }
            set
            {
                if (value > xp)
                {
                    xp = value;
                    if (xp >= xpRequired)
                        while (xp >= xpRequired)
                            xpRequired = getXpRequired(++digLevel_);
                    else
                        xpRequired = getXpRequired((digLevel_ = getLevel(xp)));
                }
            }
        }

        private static int getLevel(int xp)
        {
            int level = 0;

            while (xp > getXpRequired(level))
                level++;

            return level;
        }
    }
}
*/