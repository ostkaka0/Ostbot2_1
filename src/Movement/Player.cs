using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OstBot_2_1
{
    public class Player : EEPlayer
    {
        int userId;
        //string name;

        public Player(PlayerIOClient.Message m)
            : base(m.GetInt(0), m.GetString(1).ToLower(), m.GetInt(2), m.GetFloat(3), m.GetFloat(4), m.GetBoolean(5), m.GetBoolean(6), m.GetBoolean(7), m.GetInt(8), false, false, 0)
        {

        }

        public int UserId
        {
            get { return this.userId; }
        }

        public string Name
        {
            get { return this.name; }
        }

    }
}
