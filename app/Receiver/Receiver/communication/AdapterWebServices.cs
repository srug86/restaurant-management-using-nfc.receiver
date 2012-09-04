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

        /* Implementación de un 'Singleton' para esta clase */
        static AdapterWebServices() { }

        AdapterWebServices() { }

        public static AdapterWebServices Instance
        {
            get
            {
                return instance;
            }
        }

        // Devuelve XML con las plantillas del restaurante
        public string sendMeRooms()
        {
            return proxy.getRooms();
        }

        // Devuelve datos resumen de la plantilla actual
        public string sendMeCurrentRoom()
        {
            return proxy.getCurrentRoom();
        }

        // Devuelve XML con los datos de la plantilla seleccionada
        public string sendMeRoom(string name, bool save)
        {
            return proxy.getRoom(name, save);
        }

        // Envía XML con los datos de la plantilla a la BD
        public void sendRoom(string name, string xml)
        {
            proxy.saveRoom(name, xml);
        }

        // Devuelve XML con los datos del restaurante
        public string sendMeRestaurantData()
        {
            return proxy.getRestaurant();
        }

        // Envía XML con los datos del restaurante a la BD
        public void sendRestaurantData(string xml)
        {
            proxy.saveRestaurant(xml);
        }

        // Devuelve XML con el estado actualizado de las mesas
        public string sendMeTablesStatus()
        {
            return proxy.getTablesStatus();
        }

        // Devuelve el identificador de la mesa donde está el cliente 'dni'
        public int sendMeTable(string dni)
        {
            return proxy.getTableID(dni);
        }

        // Devuelve el importe total de la factura de la mesa 'tableID'
        public double sendMeBillAmount(int tableID)
        {
            return proxy.getBillAmount(tableID);
        }

        // Devuelve el identificador de la factura de la mesa 'tableID'
        public int sendMeBillID(int tableID)
        {
            return proxy.getBillID(tableID);
        }

        // Envía la confirmación de un pago NFC y devuelve el estado resultante
        public string sendBillPayment(int billID, int type)
        {
            return proxy.payBill(billID, type);
        }

        // Devuelve XML con el perfil del cliente 'dni'
        public string sendMeClientData(string dni)
        {
            return proxy.getClient(dni);
        }

        // Devuelve XML con el listado de clientes del restaurante
        public string sendMeClientsData()
        {
            return proxy.getClients();
        }

        // Devuelve estado del cliente 'client'
        public int sendMeClientStatus(string client)
        {
            return proxy.getClientStatus(client);
        }

        // Devuelve XML con las recomendaciones calculadas para el cliente 'client'
        public string sendMeClientRecommendation(string client)
        {
            return proxy.getRecommendation(client);
        }

        // Envía solicitud de ubicación para un cliente y devuelve el estado resultante
        public string sendAllocationTable(string client, int table, int guests)
        {
            return proxy.setAllocationTable(client, table, guests);
        }

        // Envía solicitud de desubicación para un cliente y devuelve el estado resultante
        public string sendDeallocationTable(int table)
        {
            return proxy.setDeallocationTable(table);
        }

        // Envía solicitud de ubicación para un cliente y devuelve el estado resultante
        public string sendDeallocationTable(string client)
        {
            return proxy.setDeallocationClient(client);
        }

        // Envía solicitud de iniciar nueva jornada con la plantilla 'room'
        public void sendResetJourney(string room)
        {
            proxy.resetJourney(room);
        }
    }
}
