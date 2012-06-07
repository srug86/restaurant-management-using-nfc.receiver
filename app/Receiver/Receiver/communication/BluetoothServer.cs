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

        //Guid service;
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
        

/*        public BluetoothServer() {
            exit = false;
            service = new Guid("888794c2-65ce-4de1-aa15-74a11342bc63");
            BluetoothRadio br = BluetoothRadio.PrimaryRadio;
            br.Mode = RadioMode.Discoverable;

            btListener = new BluetoothListener(service);
            btListener.Start();

            Thread th = new Thread(new ThreadStart(this.runBluetooth));
        }
        */
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
                    // Lectura del perfil del cliente
                    /*StreamReader sr = new StreamReader(client.GetStream(), Encoding.UTF8);
                    string clientData = "";
                    while (true)
                    {
                        string aux = sr.ReadLine();
                        clientData += aux;
                        if (aux.Equals("</Profile>"))
                        {
                            //sr.Close();
                            break;
                        }
                    }
                    /*do
                    {
                        clientData += sr.ReadLine();
                    } while (!sr.EndOfStream);*/
                    //manager = JourneyManager.Instance;
                    //string recommendation = manager.ClientManager.manageNFCClient(clientData.Substring(2));

                    /*byte[] byteArray = Encoding.UTF8.GetBytes(recommendation);
                    MemoryStream ms = new MemoryStream(byteArray);
                    BinaryReader br = new BinaryReader(ms);
                    StreamWriter sw = new StreamWriter(client.GetStream());
                    int count = 0;
                    do
                    {
                        count = br.Read(byteArray, 0, byteArray.Length);
                        if (count > 0)
                            sw.WriteLine(Convert.ToBase64String(byteArray, 0, count));
                    } while (count > 0);
                    sw.Flush();
                    sw.Close();
                    br.Close();
                    ms.Close();*/

                    //bool conn = client.Connected;
                    //client.Connect(client.RemoteEndPoint);
                    StreamWriter sw = new StreamWriter(client.GetStream(), Encoding.UTF8);
                    //sw.Write(recommendation);
                    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                    Byte[] bytes = encoding.GetBytes("hola\nsoy\nSergio");
                    sw.Write(bytes);
                    sw.Flush();
                    //sr.Close();
                    sw.Close();
                }
                catch (Exception e) {
                }
            } while (!exit);
        }

        public void closeBluetooth()
        {
            exit = true;
            btListener.Stop();
        }
    }
}
