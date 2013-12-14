using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace OstBot_2_1
{
    public partial class Form1 : Form
    {
        public int runtime = 0;
        public delegate void lambdaFunction(Form1 form1);
        Queue<string[]> sayString = new Queue<string[]>();
        public Queue<lambdaFunction> lambdaFunctionQueue = new Queue<lambdaFunction>();
        object sayStringLock = 0;
        AnnoyingBot annoyingBot;


        public Form1()
        {
            InitializeComponent();
            updateComboBoxes(0);
        }

        public void say(string player, string text)
        {
            lock (sayStringLock)
            {
                sayString.Enqueue(new string[] {player + ": ", text + Environment.NewLine});
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine(System.DateTime.Now.ToShortDateString() + "_" + System.DateTime.Now.ToShortTimeString().Replace(":", "-"));

            this.textBox_ChatText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(CheckKeys);

            backgroundWorker_CodeCracker.ProgressChanged += new ProgressChangedEventHandler
                    (backgroundWorker_CodeCracker_ProgressChanged);

            backgroundWorker_CodeCracker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_CodeCracker_RunWorkerCompleted);

            /*using (Stream input = File.OpenRead(Environment.CurrentDirectory + @"\\runtime.dat"))
            using (Stream output = File.OpenWrite(Environment.CurrentDirectory + @"\\runtime.dat"))
            {
                int value = input.

                runtime = value + 1;
                output.WriteByte((byte)runtime);
            }*/

            Console.WriteLine(runtime);
        }

        private void Form1_Closing(object sender, EventArgs e)
        {
            Program.ostBot.shutdown();
        }

        private void CheckKeys(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (textBox_ChatText.Focused)
                {
                    Program.ostBot.connection.Send(PlayerIOClient.Message.Create("say", new object[] {textBox_ChatText.Text}));
                    say("You: ", textBox_ChatText.Text);
                    textBox_ChatText.Text = "";
                }
            }
        }

        private void updateComboBoxes(int level)
        {
            //this.comboBox_Server.Items.Clear();
            //this.comboBox_Email.Items.Clear();
            //this.comboBox_RoomType.Items.Clear();

            if (!File.Exists("login.txt"))
                File.Create("login.txt").Close();
            StreamReader reader = new StreamReader(System.Environment.CurrentDirectory + @"\login.txt");

            List<string> lines = new List<string>();

            string line;
            string server = "";
            string email = "";
            string roomType = "";

            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
                if (!line.StartsWith("\t"))
                {
                    server = line;

                    if (level == 0)
                        this.comboBox_Server.Items.Add(line);

                    if (server == this.comboBox_Server.Text && level <= 1)
                        this.comboBox_Email.Items.Clear();
                }
                else if (line.StartsWith("\t") && !line.StartsWith("\t\t") && server == this.comboBox_Server.Text)
                {
                    email = line.Replace("\t", "");

                    if (level <= 1)
                        this.comboBox_Email.Items.Add(email);

                    if (email == this.comboBox_Email.Text && level <= 2)
                        this.comboBox_RoomType.Items.Clear();
                }
                else if (line.StartsWith("\t\t") && !line.StartsWith("\t\t\t") && email == this.comboBox_Email.Text)
                {
                    roomType = line.Replace("\t", "");
                    this.comboBox_RoomType.Items.Add(roomType);
                }
                else if (line.StartsWith("\t\t\t") && !line.StartsWith("\t\t\t\t") && email == this.comboBox_Email.Text)
                {
                    this.textBox_Password.Text = line.Replace("\t", "");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.groupBox_Connect.Enabled = false;
            if (!Program.ostBot.connected)
            {
                lock (button_Connect.Text)
                    this.button_Connect.Text = "Connecting...";
                Program.console.WriteLine("Connecting...");
                Program.ostBot.Connect();
                if (Program.ostBot.connected)
                {
                    lock (button_Connect.Text)
                        this.button_Connect.Text = "Disconnect";
                    this.comboBox_RoomType.Enabled = false;
                    this.comboBox_WorldId.Enabled = false;
                    Program.console.WriteLine("Connecting succeeded!");
                }
                else
                {
                    lock (button_Connect.Text)
                        this.button_Connect.Text = "Connect";
                    Program.console.WriteLine("Connecting failed!");
                }
            }
            else
            {
                lock (button_Connect.Text)
                    this.button_Connect.Text = "Disconnecting...";
                Program.console.WriteLine("Disconnecting...");
                bool reconnect = this.checkBox_Reconnect.Enabled;
                this.checkBox_Reconnect.Enabled = false;
                Program.ostBot.connection.Disconnect();
                this.checkBox_Reconnect.Enabled = reconnect;
                this.checkBox_Reconnect.Enabled = true;
                lock (button_Connect.Text)
                    this.button_Connect.Text = "Connect";
                this.comboBox_RoomType.Enabled = true;
                this.comboBox_WorldId.Enabled = true;
            }
            this.groupBox_Connect.Enabled = true;
        }

        private void button_Login_Click(object sender, EventArgs e)
        {
            Program.ostBot.Login(comboBox_Server.Text, comboBox_Email.Text, textBox_Password.Text);
        }

        private void comboBox_Server_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(sender.ToString());
            updateComboBoxes(1);
            if (this.comboBox_Email.Items.Count > 0)
                this.comboBox_Email.Text = this.comboBox_Email.Items[0].ToString();

            annoyingBot = new AnnoyingBot(comboBox_Server.Text, "annoying.ostkaka@gmail.com", "kasekakorna");
        }

        private void comboBox_Email_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateComboBoxes(2);
            if (this.comboBox_RoomType.Items.Count > 0)
                this.comboBox_RoomType.Text = this.comboBox_RoomType.Items[0].ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string lastPlayer = "";
            while (sayString.Count > 0)
            {
                string[] pair = sayString.Dequeue();

                if (pair[0] != lastPlayer)
                {
                    lastPlayer = pair[0];

                    richTextBox1.SelectionFont = richTextBox1.Font;
                    richTextBox1.SelectionStart = richTextBox1.SelectionStart + richTextBox1.SelectionLength;
                    richTextBox1.SelectionLength = 0;

                    richTextBox1.SelectionFont = new Font(richTextBox1.Font,
                    richTextBox1.SelectionFont.Style | FontStyle.Italic);
                    richTextBox1.Text += pair[0];

                    richTextBox1.SelectionStart = richTextBox1.SelectionStart + richTextBox1.SelectionLength;
                    richTextBox1.SelectionLength = 0;
                    richTextBox1.SelectionFont = richTextBox1.Font;
                }
                richTextBox1.Text += pair[1];
            }
            //richTextBox1.Text += sayString;
            sayString.Clear();

            lock (lambdaFunctionQueue)
            {
                while (lambdaFunctionQueue.Count > 0)
                {
                    lambdaFunctionQueue.Dequeue()(this);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PlayerIOClient.RoomInfo[] roomInfo = Program.ostBot.client.Multiplayer.ListRooms(comboBox_RoomType.Text, new Dictionary<string,string>(), 200000, 0);

            //checkedListBox_Rooms.Items.Clear();
            listView1.Items.Clear();

            Array.Sort(roomInfo, delegate(PlayerIOClient.RoomInfo a, PlayerIOClient.RoomInfo b) { return b.OnlineUsers.CompareTo(a.OnlineUsers); });

            foreach (var room in roomInfo)
            {
                listView1.Items.Add(new ListViewItem(new string[] { room.Id, room.OnlineUsers.ToString(), room.RoomType }));
                //checkedListBox_Rooms.Items.Add(room.Id.ToString());
                //checkedListBox_Rooms.Items.Add(room.OnlineUsers.ToString());
                //checkedListBox_Rooms.Items.Add(room.ToString());
                Console.WriteLine(room.ToString());
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox_RoomType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.ostBot.client != null)
            {
                PlayerIOClient.RoomInfo[] roomInfo = Program.ostBot.client.Multiplayer.ListRooms(comboBox_RoomType.Text, new Dictionary<string, string>(), 200000, 0);

                //checkedListBox_Rooms.Items.Clear();
                listView1.Items.Clear();

                Array.Sort(roomInfo, delegate(PlayerIOClient.RoomInfo a, PlayerIOClient.RoomInfo b) { return b.OnlineUsers.CompareTo(a.OnlineUsers); });

                foreach (var room in roomInfo)
                {
                    string name = (!room.RoomData.ContainsKey("name")) ? "" : room.RoomData["name"];
                    string plays = (!room.RoomData.ContainsKey("plays")) ? "" : room.RoomData["plays"];


                    listView1.Items.Add(new ListViewItem(new string[] { name , room.OnlineUsers.ToString(), plays, room.Id}));
                    //checkedListBox_Rooms.Items.Add(room.Id.ToString());
                    //checkedListBox_Rooms.Items.Add(room.OnlineUsers.ToString());
                    //checkedListBox_Rooms.Items.Add(room.ToString());
                    foreach (var pair in room.RoomData)
                    {
                        //Program.console.WriteLine(pair.Key);
                    }
                    Console.WriteLine(room.Id.ToString());
                }
            }
        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
                comboBox_WorldId.Text = listView1.SelectedItems[0].SubItems[3].Text;
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox_Hide_CheckedChanged(object sender, EventArgs e)
        {
            /*if (checkBox_Hide.Checked)
                textBox_CrackedCode.PasswordChar = '*';
            else
                textBox_CrackedCode.PasswordChar = UseSystemPasswordChar;*/

        }

        private string toDigits(int digits, int number)
        {
            string r = number.ToString();

            for (int i = r.Length; i < digits; i++)
                r = "0" + r;

            return r;
        }

        private void button_CrackCode_Click(object sender, EventArgs e)
        {
            button_CrackCode.Enabled = false;

            if (Program.ostBot.connection == null)
                return;

            backgroundWorker_CodeCracker.RunWorkerAsync();
        }

        private void backgroundWorker_CodeCracker_DoWork(object sender, DoWorkEventArgs e)
        {
            int codeMinValue = 0;

            for (int i = 0; i + codeMinValue < numericUpDown_MaxCrackCode.Value; i++)
            {
                    Program.ostBot.connection.Send("access",
                        toDigits((int)numericUpDown_CrackCodeDigits.Value, codeMinValue + i));

                if ((codeMinValue + i) % 2 == 0)
                    Thread.Sleep(1);

                if ((codeMinValue + i) % 100 == 0)
                {
                    Program.console.WriteLine((codeMinValue + i).ToString());
                    backgroundWorker_CodeCracker.ReportProgress(codeMinValue + i);
                }

                if (Program.ostBot.hasCode)
                {
                    Program.ostBot.hasCode = false;
                    codeMinValue = (codeMinValue + i) - 1000;
                    if (codeMinValue < 0)
                        codeMinValue = 0;
                    backgroundWorker_CodeCracker.ReportProgress(codeMinValue);
                    break;
                }
            }

            for (int i = 0; i < 1000 && i + codeMinValue < numericUpDown_MaxCrackCode.Value; i++)
            {
                Program.ostBot.connection.Send("access",
                    toDigits((int)numericUpDown_CrackCodeDigits.Value, codeMinValue + i));

                Thread.Sleep(5);

                if ((codeMinValue + i) % 100 == 0)
                {
                    Program.console.WriteLine((i + codeMinValue).ToString());
                    backgroundWorker_CodeCracker.ReportProgress(codeMinValue+i);
                }

                if (Program.ostBot.hasCode)
                {
                    Program.ostBot.hasCode = false;
                    codeMinValue = codeMinValue +i - 100;
                    backgroundWorker_CodeCracker.ReportProgress(codeMinValue);
                    break;
                }
            }

            for (int i = 0; i < 100 && i + codeMinValue < numericUpDown_MaxCrackCode.Value; i++)
            {
                Program.ostBot.connection.Send("access",
                    toDigits((int)numericUpDown_CrackCodeDigits.Value, codeMinValue + i));

                Thread.Sleep(50);

                Program.console.WriteLine((i + codeMinValue).ToString());
                backgroundWorker_CodeCracker.ReportProgress(codeMinValue + i);

                if (Program.ostBot.hasCode)
                {
                    Program.ostBot.hasCode = false;
                    codeMinValue = codeMinValue+  i-10;
                    backgroundWorker_CodeCracker.ReportProgress(codeMinValue);
                    break;
                }
            }

            for (int i = 0; i < 10 && i + codeMinValue < numericUpDown_MaxCrackCode.Value; i++)
            {
                Program.ostBot.connection.Send("access",
                    toDigits((int)numericUpDown_CrackCodeDigits.Value, codeMinValue + i));

                Thread.Sleep(500);

                Program.console.WriteLine((i + codeMinValue).ToString());
                backgroundWorker_CodeCracker.ReportProgress(codeMinValue + i);

                if (Program.ostBot.hasCode)
                {
                    Program.ostBot.hasCode = false;
                    codeMinValue += i;
                    backgroundWorker_CodeCracker.ReportProgress(codeMinValue);
                    break;
                }
            }
        }

        void backgroundWorker_CodeCracker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // This function fires on the UI thread so it's safe to edit
            // the UI control directly, no funny business with Control.Invoke :)
            // Update the progressBar with the integer supplied to us from the
            // ReportProgress() function.
            progressBar_CodeCracker.Value = e.ProgressPercentage;
            textBox_CrackedCode.Text = toDigits((int)numericUpDown_CrackCodeDigits.Value, e.ProgressPercentage);
            //lblStatus.Text = "Processing......" + progressBar1.Value.ToString() + "%";
        }

        void backgroundWorker_CodeCracker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar_CodeCracker.Value = 0;

            button_CrackCode.Enabled = true;
        }

        private void progressBar_CodeCracker_Click(object sender, EventArgs e)
        {

        }

        private void groupBox_Login_Enter(object sender, EventArgs e)
        {

        }

        private void numericUpDown_CrackCodeDigits_ValueChanged(object sender, EventArgs e)
        {
            progressBar_CodeCracker.Maximum = (int)Math.Pow(10, (double)numericUpDown_CrackCodeDigits.Value) + 1000 + 100 + 10;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Program.ostBot.room.setDrawSleep((int)numericUpDown1.Value);
        }

        private void button_EnterCode_Click(object sender, EventArgs e)
        {
            if (Program.ostBot.connected)
            {
                if (Program.ostBot.connection != null)
                    Program.ostBot.connection.Send("access", textBox4.Text);
            }
        }

        private void listBox_PlayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_PlayerList.SelectedItem == null)
                return;

            Clipboard.Clear();
            Thread.Sleep(100);
            lock (listBox_PlayerList.Items)
                Clipboard.SetDataObject(
                    listBox_PlayerList.SelectedItem.ToString(), //text to store in clipboard
                    false,       //do not keep after our app exits
                    5,           //retry 5 times
                    200);        //200ms delay between retries
                //Clipboard.SetText(listBox_PlayerList.SelectedItem.ToString());
            
        }

        private void checkedListBox_BotSystems_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox_BotSystems.Items.Count; i++)
            {
                var BotSystem = checkedListBox_BotSystems.Items[i] as BotSystem;
                BotSystem.enabled = checkedListBox_BotSystems.GetItemChecked(i);
            }
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            if (annoyingBot != null)
            {
                annoyingBot.Connect(comboBox_RoomType.Text);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            annoyingBot = null;
        }

        public void PushPlacedData(Dictionary<int,int> data)
        {
            foreach(var i in data)
            {
                string name;
                lock (Program.ostBot.playerList)
                {
                    if (Program.ostBot.playerList.ContainsKey(i.Key))
                        name = Program.ostBot.playerList[i.Key].Name;
                    else if (Program.ostBot.leftPlayerList.ContainsKey(i.Key))
                        name = Program.ostBot.leftPlayerList[i.Key].Name;
                    else
                        name = "[" + i.Key.ToString() + "]";
                }

                if (chartPlacedBlocks.Series.FindByName(name) == null)
                {
                    var v = new System.Windows.Forms.DataVisualization.Charting.Series(name, 0);
                    v.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedArea;

                    for (int ii = 0; ii < 250; ii++)
                        v.Points.AddY(0);

                    //v.Label = name;
                    //v.SmartLabelStyle.CalloutLineWidth = 2560;

                    chartPlacedBlocks.Series.Add(v);
                }
                chartPlacedBlocks.Series[name].Points.AddY(i.Value);
                chartPlacedBlocks.Series[name].Points.RemoveAt(0);

                /*if (chartPlacedBlocks.Series[name].Points.Count > 100)
                {
                    chartPlacedBlocks.Series[name].Points.RemoveAt(0);
                }*/

            }
        }

        private void chartPlacedBlocks_Click(object sender, EventArgs e)
        {

        }

    }
}
