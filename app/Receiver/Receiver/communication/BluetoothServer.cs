using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System.Threading;
using System.IO;
using Receiver.domain;

namespace Receiver.communication
{
    class BluetoothServer
    {
        private JourneyManager manager = JourneyManager.Instance;

        BluetoothListener btListener;
        Guid service = new Guid("{0x2345}");
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
        }

        public void runBluetooth()
        {
            do
            {
                try
                {
                    BluetoothClient client = btListener.AcceptBluetoothClient();
                    StreamReader sr = new StreamReader(client.GetStream(), Encoding.UTF8);
                    String clientData = sr.ReadLine();
                   // manager.ClientManager.manageNFCClient(clientData);
                    sr.Close();
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
