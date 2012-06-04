using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Receiver.communication;
using System.Xml;
using System.Windows.Threading;
using Receiver.presentation;

namespace Receiver.domain
{
    class ClientManager
    {
        private AdapterWebServices adapter = AdapterWebServices.Instance;

        private List<ObserverRM> rmObservers = new List<ObserverRM>();

        private JourneyManagerWin gui;

        private Client client;
        internal Client Client
        {
          get { return client; }
          set { client = value; }
        }

        private int guests, table;

        public int Table
        {
            get { return table; }
            set { table = value; }
        }
        public int Guests
        {
          get { return guests; }
          set { guests = value; }
        }

        private double amount;

        public double Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public ClientManager() { }

        public void setGuiReference(JourneyManagerWin gui)
        {
            this.gui = gui;
        }

        public void newStandardClient()
        {
            adapter.sendMeClientStatus(xmlClientBuilder());
        }

        public void manageNFCClient(string xml)
        {
            xmlClientDecoder(xml);
            int status = adapter.sendMeClientStatus(xml);
            switch (status)
            {
                case -1:
                case 0:
                    gui.delegateToNFCClientHasArrived(Client);
                    break;
                case 1:
                    Table = adapter.sendMeTable(Client.Dni);
                    Amount = adapter.sendMeBillAmount(Table);
                    gui.delegateToNFCClientHasPaid(Client, Table, Amount);
                    break;
                case 2:
                    Table = adapter.sendMeTable(Client.Dni);
                    gui.delegateToNFCClientHasLeft(Client, Table);
                    break;
            }
        }

        private void xmlClientDecoder(string sXml)
        {
            client = new Client();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(sXml);
            XmlNodeList profile = xml.GetElementsByTagName("Profile");
            XmlNodeList dni = ((XmlElement)profile[0]).GetElementsByTagName("DNI");
            client.Dni = Convert.ToString(dni[0].InnerText);
            XmlNodeList name = ((XmlElement)profile[0]).GetElementsByTagName("Name");
            client.Name = Convert.ToString(name[0].InnerText);
            XmlNodeList surname = ((XmlElement)profile[0]).GetElementsByTagName("Surname");
            client.Surname = Convert.ToString(surname[0].InnerText);
            XmlNodeList address = ((XmlElement)profile[0]).GetElementsByTagName("Address");
            XmlNodeList street = ((XmlElement)address[0]).GetElementsByTagName("Street");
            client.Address.Street = Convert.ToString(street[0].InnerText);
            XmlNodeList number = ((XmlElement)address[0]).GetElementsByTagName("Number");
            client.Address.Number = Convert.ToString(number[0].InnerText);
            XmlNodeList zip = ((XmlElement)address[0]).GetElementsByTagName("ZipCode");
            client.Address.ZipCode = Convert.ToInt32(zip[0].InnerText);
            XmlNodeList town = ((XmlElement)address[0]).GetElementsByTagName("Town");
            client.Address.Town = Convert.ToString(town[0].InnerText);
            XmlNodeList state = ((XmlElement)address[0]).GetElementsByTagName("State");
            client.Address.State = Convert.ToString(state[0].InnerText);
        }

        private string xmlClientBuilder()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Profile>\n";
            xml += "\t<DNI>" + client.Dni + "</DNI>\n";
            xml += "\t<Name>" + client.Name + "</Name>\n";
            xml += "\t<Surname>" + client.Surname + "</Surname>\n";
            xml += "\t<Address>\n";
            xml += "\t\t<Street>" + client.Address.Street + "</Street>\n";
            xml += "\t\t<Number>" + client.Address.Number + "</Number>\n";
            xml += "\t\t<ZipCode>" + client.Address.ZipCode + "</ZipCode>\n";
            xml += "\t\t<Town>" + client.Address.Town + "</Town>\n";
            xml += "\t\t<State>" + client.Address.State + "</State>\n";
            xml += "\t</Address>\n";
            xml += "</Profile>";
            return xml;
        }

/*        private void clientArrives()
        {
            for (int i = 0; i < rmObservers.Count; i++)
            {
                ObserverRM obs = (ObserverRM)rmObservers[i];
                obs.notifyNFCEntry(client.Dni, client.Name, client.Surname);
            }
        }

        private void nfcClientHasArrived()
        {
            DelegateOfTheArrivingClient oSwitchClient = new DelegateOfTheArrivingClient();
            DelegateOfTheArrivingClient.ArrivingClientDelegate oClientDelegate =
                new DelegateOfTheArrivingClient.ArrivingClientDelegate(clientArrives);
            oSwitchClient.clientArrives += oClientDelegate;
            oSwitchClient.changeContentsArrivingClient = Client;

            oSwitchClient.clientArrives -= oClientDelegate;
        }

        private void clientLeaves()
        {
            for (int i = 0; i < rmObservers.Count; i++)
            {
                ObserverRM obs = (ObserverRM)rmObservers[i];
                obs.notifyNFCExit(Client.Dni, Client.Name, Client.Surname, Table);
            }
        }

        private void nfcClientHasLeft()
        {
            DelegateOfTheLeavingClient oSwitchClient = new DelegateOfTheLeavingClient();
            DelegateOfTheLeavingClient.LeavingClientDelegate oClientDelegate =
                new DelegateOfTheLeavingClient.LeavingClientDelegate(clientLeaves);
            oSwitchClient.clientLeaves += oClientDelegate;
            oSwitchClient.changeContentsLeavingClient = Client;

            oSwitchClient.clientLeaves -= oClientDelegate;
        }

        private void clientPays()
        {
            for (int i = 0; i < rmObservers.Count; i++)
            {
                ObserverRM obs = (ObserverRM)rmObservers[i];
                obs.notifyNFCPayment(Client.Dni, Client.Name, Client.Surname, Table, Amount);
            }
        }

        private void nfcClientHasPaid()
        {
            DelegateOfThePayingClient oSwitchClient = new DelegateOfThePayingClient();
            DelegateOfThePayingClient.PayingClientDelegate oClientDelegate =
                new DelegateOfThePayingClient.PayingClientDelegate(clientPays);
            oSwitchClient.clientPays += oClientDelegate;
            oSwitchClient.changeContentsPayingClient = Client;

            oSwitchClient.clientPays -= oClientDelegate;
        }

        public void registerInterest(ObserverRM obs)
        {
            rmObservers.Add(obs);
        }*/
    }

    public interface SubjectRM
    {
        void registerInterest(ObserverRM obs);
    }

    public interface ObserverRM
    {
        void notifyNFCEntry(string DNI, string name, string surname);
        void notifyNFCExit(string DNI, string name, string surname, int table);
        void notifyNFCPayment(string DNI, string name, string surname, int table, double amount);
    }

    class DelegateOfTheArrivingClient
    {
        public delegate void ArrivingClientDelegate();

        public event ArrivingClientDelegate clientArrives;

        public object changeContentsArrivingClient
        {
            set
            {
                clientArrives();
            }
        }
    }

    class DelegateOfTheLeavingClient
    {
        public delegate void LeavingClientDelegate();

        public event LeavingClientDelegate clientLeaves;

        public object changeContentsLeavingClient
        {
            set
            {
                clientLeaves();
            }
        }
    }

    class DelegateOfThePayingClient
    {
        public delegate void PayingClientDelegate();

        public event PayingClientDelegate clientPays;

        public object changeContentsPayingClient
        {
            set
            {
                clientPays();
            }
        }
    }
}
