using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OstBot_2_1
{
    public partial class ConsoleWindow : Form
    {
        public ConsoleWindow()
        {
            InitializeComponent();
        }

        public void Write(string str)
        {
            this.Invoke(new Action(()=>
                textBox1.Text += str
                ));
            Console.Write(str);
        }

        public void WriteLine(string str)
        {
            this.Invoke(new Action(() =>
                textBox1.Text += str + Environment.NewLine
                ));
            Console.WriteLine(str);
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Console_Load(object sender, EventArgs e)
        {
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textBox2_KeyPress);
        }

        private void textBox2_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (textBox2.Focused)
                {
                    textBox1.Text += "> " + textBox2.Text + System.Environment.NewLine;
                    Program.ostBot.subBotHandler.onCommand(sender, textBox2.Text, -1,Program.ostBot);
                    textBox2.Text = "";
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            /*lock (terminalStringLock)
            {
                textBox1.Text += terminalString;
                terminalString = "";
            }*/
        }

    }
}
