using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System.Threading;
using System.IO;
using Receiver.domain;
using Receiver.presentation;

namespace Receiver.communication
{
    class BluetoothServer
    {
        private JourneyManager manager = JourneyManager.Instance;

        BluetoothListener btListener;
        Guid service = new Guid("888794c2-65ce-4de1-aa15-74a11342bc63");

        private JourneyManagerWin win;
        public JourneyManagerWin Win
        {
            get { return win; }
            set { win = value; }
        }

        bool exit;

        static readonly BluetoothServer instance = new BluetoothServer();

        static BluetoothServer() { }

        BluetoothServer() { }

        public static BluetoothServer Instance
        {
            get
            {
                return instance;
            }
        }
        
        public void initBluetooth()
        {
            exit = false;

            BluetoothRadio br = BluetoothRadio.PrimaryRadio;
            br.Mode = RadioMode.Discoverable;

            btListener = new BluetoothListener(service);
            btListener.Start();

            Thread th = new Thread(new ThreadStart(this.runBluetooth));
            th.Start();
        }
        
        public void runBluetooth()
        {
            do
            {
                try
                {
                    BluetoothClient client = btListener.AcceptBluetoothClient();
                    StreamReader sr = new StreamReader(client.GetStream(), Encoding.UTF8);
                    string clientData = "";
                    while (true)
                    {
                        string aux = sr.ReadLine();
                        clientData += aux;
                        if (aux.Equals("</Profile>")) break;
                    }
                    manager = JourneyManager.Instance;
                    string recommendation = manager.ClientManager.manageNFCClient(clientData.Substring(2));
                    StreamWriter sw = new StreamWriter(client.GetStream(), Encoding.ASCII);
                    sw.Write(Convert.ToString(recommendation));
                    sw.Flush();
                    sw.Close();
                    sr.Close();
                    client.Close();
                }
                catch (Exception e) { }
            } while (!exit);
        }

        public void closeBluetooth()
        {
            exit = true;
            btListener.Stop();
        }
    }
}
