using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Receiver.communication
{
    class AdapterWebServices
    {
        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private webServer.MobiCartaWebServicesSoapClient proxy = new webServer.MobiCartaWebServicesSoapClient();

        static readonly AdapterWebServices instance = new AdapterWebServices();

        static AdapterWebServices() { }

        AdapterWebServices() { }

        public static AdapterWebServices Instance
        {
            get
            {
                return instance;
            }
        }

        public string sendMeRooms()
        {
            return proxy.getRooms();
        }

        public string sendMeCurrentRoom()
        {
            return proxy.getCurrentRoom();
        }

        public string sendMeRoom(string name, bool save)
        {
            return proxy.getRoom(name, save);
        }

        public void sendRoom(string name, string xml)
        {
            proxy.saveRoom(name, xml);
        }

        public string sendMeRestaurantData()
        {
            return proxy.getRestaurant();
        }

        public void sendRestaurantData(string xml)
        {
            proxy.saveRestaurant(xml);
        }

        public string sendMeTablesStatus()
        {
            return proxy.getTablesStatus();
        }

        public int sendMeTable(string dni)
        {
            return proxy.getTableID(dni);
        }

        public double sendMeBillAmount(int tableID)
        {
            return proxy.getBillAmount(tableID);
        }

        public int sendMeBillID(int tableID)
        {
            return proxy.getBillID(tableID);
        }

        public string sendBillPayment(int billID, int type)
        {
            return proxy.payBill(billID, type);
        }

        public string sendMeClientData(string dni)
        {
            return proxy.getClient(dni);
        }

        public string sendMeClientsData()
        {
            return proxy.getClients();
        }

        public int sendMeClientStatus(string client)
        {
            return proxy.getClientStatus(client);
        }

        public string sendMeClientRecommendation(string client)
        {
            return proxy.getRecommendation(client);
        }

        public string sendAllocationTable(string client, int table, int guests)
        {
            return proxy.setAllocationTable(client, table, guests);
        }

        public string sendDeallocationTable(int table)
        {
            return proxy.setDeallocationTable(table);
        }

        public string sendDeallocationTable(string client)
        {
            return proxy.setDeallocationClient(client);
        }

        public void sendResetJourney(string room)
        {
            proxy.resetJourney(room);
        }
    }
}
