using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Receiver.webServices;
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

        private webServices.MobiCartaWebServicesSoapClient proxy = new webServices.MobiCartaWebServicesSoapClient();

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

        public bool connect(string url)
        {
            try
            {
                Url = url;
                return proxy.connect();
            }
            catch (Exception e) { return false; }
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

        public int sendMeClientStatus(string dni, string name, string surname)
        {
            return proxy.getClientStatus(dni, name, surname);
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
