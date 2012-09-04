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
        // Identificador único del servicio publicado por el servidor Bluetooth
        Guid service = new Guid("888794c2-65ce-4de1-aa15-74a11342bc63");

        private JourneyManagerWin win;
        public JourneyManagerWin Win
        {
            get { return win; }
            set { win = value; }
        }

        bool exit;

        static readonly BluetoothServer instance = new BluetoothServer();

        /* Implementación de un 'Singleton' para esta clase */
        static BluetoothServer() { }

        BluetoothServer() { }

        public static BluetoothServer Instance
        {
            get
            {
                return instance;
            }
        }
        
        // Inicializa el servidor Bluetooth
        public void initBluetooth()
        {
            exit = false;

            BluetoothRadio br = BluetoothRadio.PrimaryRadio;    // Radio Bluetooth de tipo Primario
            br.Mode = RadioMode.Discoverable;                   // Radio Bluetooth visible a los clientes

            btListener = new BluetoothListener(service);
            btListener.Start();

            Thread th = new Thread(new ThreadStart(this.runBluetooth));
            th.Start();
        }
        
        // Hilo que mantiene el servicio Bluetooth
        public void runBluetooth()
        {
            do
            {
                try
                {
                    BluetoothClient client = btListener.AcceptBluetoothClient();    // Acepta conexión con un cliente
                    StreamReader sr = new StreamReader(client.GetStream(), Encoding.UTF8);
                    string clientData = "";
                    while (true)
                    {
                        string aux = sr.ReadLine();
                        clientData += aux;
                        if (aux.Equals("</Profile>")) break;    // Recibe XML con los datos del perfil del cliente
                    }
                    manager = JourneyManager.Instance;
                    string recommendation = manager.ClientManager.manageNFCClient(clientData.Substring(2));
                    StreamWriter sw = new StreamWriter(client.GetStream(), Encoding.ASCII);
                    sw.Write(Convert.ToString(recommendation)); // Se le envía recomendación personalizada
                    sw.Flush();
                    sw.Close();
                    sr.Close();
                    client.Close(); // Da por finalizada la comunicación con el cliente
                }
                catch (Exception e) { }
            } while (!exit);
        }

        // Termina con el servidor Bluetooth
        public void closeBluetooth()
        {
            exit = true;
            btListener.Stop();
        }
    }
}
