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
            try
            {
                return proxy.getRooms();
            }
            catch (EndpointNotFoundException e)
            { 
                return "";
            }
        }

        public string sendMeCurrentRoom()
        {
            return proxy.getCurrentRoom();
        }

        public string sendMeRoom(string name)
        {
            try
            {
                return proxy.getRoom(name);
            }
            catch (EndpointNotFoundException e)
            {
                return "";
            }
        }

        public void sendRoom(string xml)
        {
            proxy.saveRoom(xml);
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
