using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Receiver.communication;

namespace Receiver.domain
{
    class ClientManager
    {
        private AdapterWebServices adapter = AdapterWebServices.Instance;

        private Client client;
        internal Client Client
        {
          get { return client; }
          set { client = value; }
        }

        private int guests;
        public int Guests
        {
          get { return guests; }
          set { guests = value; }
        }

        public ClientManager() { }

        public void newClient(string id)
        {
        }

        public void exitClient()
        {
        }

        public void manageNFCClient()
        {
        }

        private void xmlClientData(string xml)
        {
        }
    }
}
