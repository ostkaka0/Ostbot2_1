using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OstBot_2_1
{
    public class tPlayer
    {
        int userId;
        string name;

        public tPlayer(int userId, string name)
        {
            this.userId = userId;
            this.name = name;
        }

        public string Name
        {
            get { return (this == null)? "" : this.name; }
        }

        public int UserId
        {
            get { return (this == null)? -1 : this.userId; }
        }
    }
}
