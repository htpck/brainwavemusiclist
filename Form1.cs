using System;
using NeuroSky.ThinkGear;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace hilalm1
{

    public partial class Form1 : Form
    {
        public class Node
        {
            public Node next;
            public String song;
            public int mvalue;
            public int avalue;

        }

        static Connector connector;
        bool changed = false;
        static int medValue;
        static int attValue;
        static int m = 1;
        static int a = 1;
        List<Node> myList = new List<Node>();
        List<Node> AttList = new List<Node>();
        List<Node> MedList = new List<Node>();

        public void updateAttLists()
        {
            AttList = myList.OrderBy(o => o.avalue).ToList();
        }

        public void updateMedLists()
        {
            MedList = myList.OrderBy(o => o.mvalue).ToList();
        }

        public void modifyData(String str, int mv, int av)
        {
            
            foreach (Node element in myList)
            {
                if (element.song == str)
                {
                    element.mvalue = mv;
                    element.avalue = av;
                    //element.mvalue = (mv + element.mvalue)/2;
                    //element.avalue = (av + element.avalue)/2;
                    //Console.WriteLine("m2 " + element.song + " " + element.avalue + " " + element.mvalue);
                    break; 
                }
            }


        }
        public static async Task funcstop()
        {
            System.Console.WriteLine("Goodbye.");
            connector.Close();
        }
            public static async Task funcmain()
        {

            Console.WriteLine("HelloEEG!");

            // Initialize a new Connector and add event handlers

            connector = new Connector();
            connector.DeviceConnected += new EventHandler(OnDeviceConnected);
            connector.DeviceConnectFail += new EventHandler(OnDeviceFail);
            connector.DeviceValidating += new EventHandler(OnDeviceValidating);

            // Scan for devices across COM ports
            // The COM port named will be the first COM port that is checked.
            connector.ConnectScan("COM40");

            // Blink detection needs to be manually turned on
            connector.setBlinkDetectionEnabled(true);
            Thread.Sleep(450000);




            System.Console.WriteLine("Goodbye.");
            connector.Close();
            Environment.Exit(0);
        }


        // Called when a device is connected 

        static void OnDeviceConnected(object sender, EventArgs e)
        {

            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            Console.WriteLine("Device found on: " + de.Device.PortName);
            de.Device.DataReceived += new EventHandler(OnDataReceived);

        }


        // Called when scanning fails

        static void OnDeviceFail(object sender, EventArgs e)
        {

            Console.WriteLine("No devices found! :(");

        }



        // Called when each port is being validated

        static void OnDeviceValidating(object sender, EventArgs e)
        {

            Console.WriteLine("Validating: ");

        }


        static void OnDataReceived(object sender, EventArgs e)
        {

            //Device d = (Device)sender;

            Device.DataEventArgs de = (Device.DataEventArgs)e;
            NeuroSky.ThinkGear.DataRow[] tempDataRowArray = de.DataRowArray;

            TGParser tgParser = new TGParser();
            tgParser.Read(de.DataRowArray);

            for (int i = 0; i < tgParser.ParsedData.Length; i++)
            {

                if (tgParser.ParsedData[i].ContainsKey("Attention"))
                {
                    attValue += (int)tgParser.ParsedData[i]["Attention"];
                    Console.WriteLine("Att Value:" + a + " " + attValue);
                    a++;

                }
                if (tgParser.ParsedData[i].ContainsKey("Meditation"))
                {
                    medValue += (int)tgParser.ParsedData[i]["Meditation"];
                    Console.WriteLine("Med Value:" + m + " " + medValue);
                    m++;
                }
            }


        }
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        String[] paths, files;


        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.URL = paths[listBox2.SelectedIndex];
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000; // 1 second
            medValue = medValue / m;
            attValue = attValue / a;
            modifyData(listBox2.Items[listBox2.SelectedIndex].ToString(), medValue, attValue);
            medValue = 0;
            attValue = 0;
            m = 1;
            a = 1;
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                timer1.Stop();
                axWindowsMediaPlayer1.URL = paths[listBox2.SelectedIndex];
                changed = false;
                timer1.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!changed && axWindowsMediaPlayer1.playState != WMPLib.WMPPlayState.wmppsPlaying)
            {
                medValue = medValue / m;
                attValue = attValue / a;
                modifyData(listBox2.Items[listBox1.SelectedIndex].ToString(), medValue, attValue);
                medValue = 0;
                attValue = 0;
                m = 1;
                a = 1;
                if (listBox2.SelectedIndex < files.Length - 1) listBox2.SelectedIndex++;
                else listBox2.SelectedIndex = 0;
                axWindowsMediaPlayer1.URL = paths[listBox2.SelectedIndex];
                changed = true;
            }

        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await Task.Run(() => funcmain());
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            await Task.Run(() => funcstop());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            updateAttLists();
            listBox1.Items.Clear();
            for (int i = 0; i < AttList.Count; i++)
            {
                listBox1.Items.Add(AttList[i].song);
            }
            
            listBox2.Items.Clear();
            for (int i = 0; i < AttList.Count; i++)
            {
                listBox2.Items.Add(AttList[i].song);
            }
            listBox2.SelectedIndex = 0;
            button1.Text = "Playing Attention List";
            button1.Text = button1.Text.ToUpper();
            button1.BackColor = Color.LightSteelBlue;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            updateMedLists();
            listBox3.Items.Clear();
            for (int i = 0; i < MedList.Count; i++)
            {
                listBox3.Items.Add(MedList[i].song);
            }
            
            listBox2.Items.Clear();
            for (int i = 0; i < AttList.Count; i++)
            {
                listBox2.Items.Add(AttList[i].song);
            }
            listBox2.SelectedIndex = 0;
            button1.Text = "Playing Meditation List";
            button1.Text = button1.Text.ToUpper();
            button1.BackColor = Color.LightSteelBlue;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //funcmain();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                files = ofd.SafeFileNames;
                paths = ofd.FileNames;

                for(int i = 0; i < files.Length; i++)
                {
                    Node toAdd = new Node();

                    listBox2.Items.Add(files[i]);
                    
                    toAdd.song = files[i];
                    toAdd.mvalue = 0;
                    toAdd.avalue = 0;
                    myList.Add(toAdd);
                    button1.Enabled = false;
                }
                
            }

        }
    }
}
