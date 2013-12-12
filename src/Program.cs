using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace OstBot_2_1
{
    public class Program
    {
        public static OstBot ostBot;
        public static Form1 form1;
        public static ConsoleWindow console;

        [STAThread]
         void Main(string[] args)
        {

            /*try
            {*/
            Application.EnableVisualStyles();
            console = new ConsoleWindow();
            console.Show();
            ostBot = new OstBot();
            form1 = new Form1();
            Application.Run(form1);
            /*}
            catch (Exception e)
            {
                OstBot.shutdown();
                ostBot = null;
                throw e;
            }*/
        }
    }
}
