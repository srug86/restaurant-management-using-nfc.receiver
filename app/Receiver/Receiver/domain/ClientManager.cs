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

        public void newStandardClient()
        {
            adapter.sendMeClientStatus(xmlClientBuilder());
        }

        public void exitClient()
        {
        }

        public void manageNFCClient()
        {
        }

        private void xmlClientDecoder(string xml)
        {
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
}
