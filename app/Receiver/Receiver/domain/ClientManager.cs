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

        private JourneyManager manager = JourneyManager.Instance;

        private List<ObserverRM> rmObservers = new List<ObserverRM>();

        private JourneyManagerWin gui;

        private Client client;
        internal Client Client
        {
            get { return client; }
            set { client = value; }
        }

        private int guests, table, billID;

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

        public int BillID
        {
            get { return billID; }
            set { billID = value; }
        }

        private double amount;

        public double Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        static readonly ClientManager instance = new ClientManager();

        static ClientManager() { }

        ClientManager() { }

        public static ClientManager Instance
        {
            get
            {
                return instance;
            }
        }

        public List<ShortClient> getClientsData()
        {
            return xmlClientsDataDecoder(adapter.sendMeClientsData());
        }

        public Client getClientData(string dni)
        {
            return xmlClientDecoder(adapter.sendMeClientData(dni));
        }

        public void setGuiReference(JourneyManagerWin gui)
        {
            this.gui = gui;
        }

        public void newStandardClient()
        {
            adapter.sendMeClientStatus(xmlClientBuilder());
        }

        public string manageNFCClient(string xml)
        {
            Client = xmlClientDecoder(xml);
            int status = adapter.sendMeClientStatus(xml);
            switch (status)
            {
                case -1:
                case 0:
                    gui.delegateToNFCClientHasArrived(Client);
                    return adapter.sendMeClientRecommendation(Client.Dni);
                case 1:
                    Table = adapter.sendMeTable(Client.Dni);
                    Amount = adapter.sendMeBillAmount(Table);
                    BillID = adapter.sendMeBillID(Table);
                    payBill(BillID, 2); // Pago por NFC
                    manager.RoomManager.confirmDeallocation(Table);
                    gui.delegateToShowReceiverEvent(2, "El cliente " + Client.Dni + " paga su cuenta por NFC.");
                    gui.delegateToShowReceiverEvent(3, "El cliente " + Client.Dni + " deja libre la mesa " + Table + " y abandona el restaurante.");
                    gui.delegateToNFCClientHasPaid(Client, Table, BillID, Amount);
                    return "La cuenta de la mesa " + Table + " ha sido cobrada satisfactoriamente. " +
                        Amount + " Euros han sido decrementados de su cuenta.";
                case 2:
                    Table = adapter.sendMeTable(Client.Dni);
                    gui.delegateToNFCClientHasLeft(Client, Table);
                    return "Gracias por su visita.";
                default:
                    return "";
            }
        }

        public void payBill(int billID, int type)
        {
            adapter.sendBillPayment(billID, type);
        }

        private List<ShortClient> xmlClientsDataDecoder(string sXml)
        {
            List<ShortClient> lsc = new List<ShortClient>();
            if (!sXml.Equals(""))
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList clients = xml.GetElementsByTagName("Clients");
                XmlNodeList cList = ((XmlElement)clients[0]).GetElementsByTagName("Client");
                foreach (XmlElement client in cList)
                {
                    ShortClient sc = new ShortClient();
                    sc.Dni = Convert.ToString(client.GetAttribute("dni"));
                    XmlNodeList name = ((XmlElement)client).GetElementsByTagName("Name");
                    sc.Name = Convert.ToString(name[0].InnerText);
                    XmlNodeList surname = ((XmlElement)client).GetElementsByTagName("Surname");
                    sc.Surname = Convert.ToString(surname[0].InnerText);
                    XmlNodeList appearances = ((XmlElement)client).GetElementsByTagName("Appearances");
                    sc.Appearances = Convert.ToInt32(appearances[0].InnerText);
                    XmlNodeList status = ((XmlElement)client).GetElementsByTagName("Status");
                    sc.Status = Convert.ToString(status[0].InnerText);
                    lsc.Add(sc);
                }
            }
            return lsc;
        }

        private Client xmlClientDecoder(string sXml)
        {
            Client client = new Client();
            if (!sXml.Equals(""))
            {
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
            return client;
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
