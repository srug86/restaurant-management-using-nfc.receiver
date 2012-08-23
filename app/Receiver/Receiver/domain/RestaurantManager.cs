using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Receiver.communication;
using System.Xml;

namespace Receiver.domain
{
    class RestaurantManager
    {
        private AdapterWebServices adapter = AdapterWebServices.Instance;

        static readonly RestaurantManager instance = new RestaurantManager();

        static RestaurantManager() { }

        RestaurantManager() { }

        public static RestaurantManager Instance
        {
            get
            {
                return instance;
            }
        }

        public List<Object> getRestaurantData()
        {
            return xmlRestaurantDataDecoder(adapter.sendMeRestaurantData());
        }

        public void setRestaurantData(List<Object> data)
        {
            adapter.sendRestaurantData(xmlRestaurantDataBuilder(data));
        }

        private List<Object> xmlRestaurantDataDecoder(string sXml)
        {
            List<Object> objects = new List<Object>();
            if (sXml != "")
            {
                Company company = new Company();
                Address address = new Address();
                double iva, discount = 0.0;
                int discountedVisit = 0;
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList restaurant = xml.GetElementsByTagName("Restaurant");
                XmlNodeList companyD = ((XmlElement)restaurant[0]).GetElementsByTagName("Company");
                XmlNodeList nif = ((XmlElement)companyD[0]).GetElementsByTagName("NIF");
                company.NIF = Convert.ToString(nif[0].InnerText).Trim();
                XmlNodeList name = ((XmlElement)companyD[0]).GetElementsByTagName("Name");
                company.Name = Convert.ToString(name[0].InnerText).Trim();
                XmlNodeList phone = ((XmlElement)companyD[0]).GetElementsByTagName("Phone");
                company.Phone = Convert.ToInt32(phone[0].InnerText);
                XmlNodeList fax = ((XmlElement)companyD[0]).GetElementsByTagName("Fax");
                company.Fax = Convert.ToInt32(phone[0].InnerText);
                XmlNodeList email = ((XmlElement)companyD[0]).GetElementsByTagName("Email");
                company.Email = Convert.ToString(email[0].InnerText).Trim();
                objects.Add(company);
                XmlNodeList addressD = ((XmlElement)restaurant[0]).GetElementsByTagName("Address");
                XmlNodeList street = ((XmlElement)addressD[0]).GetElementsByTagName("Street");
                address.Street = Convert.ToString(street[0].InnerText).Trim();
                XmlNodeList number = ((XmlElement)addressD[0]).GetElementsByTagName("Number");
                address.Number = Convert.ToString(number[0].InnerText).Trim();
                XmlNodeList zip = ((XmlElement)addressD[0]).GetElementsByTagName("ZipCode");
                address.ZipCode = Convert.ToInt32(zip[0].InnerText);
                XmlNodeList town = ((XmlElement)addressD[0]).GetElementsByTagName("Town");
                address.Town = Convert.ToString(town[0].InnerText).Trim();
                XmlNodeList state = ((XmlElement)addressD[0]).GetElementsByTagName("State");
                address.State = Convert.ToString(state[0].InnerText).Trim();
                objects.Add(address);
                XmlNodeList rate = ((XmlElement)restaurant[0]).GetElementsByTagName("Rate");
                iva = Convert.ToDouble(((XmlElement)rate[0]).GetAttribute("iva"));
                objects.Add(iva);
                discount = Convert.ToDouble(((XmlElement)rate[0]).GetAttribute("discount"));
                objects.Add(discount);
                discountedVisit = Convert.ToInt32(((XmlElement)rate[0]).GetAttribute("discountedVisit"));
                objects.Add(discountedVisit);
            }
            return objects;
        }

        private string xmlRestaurantDataBuilder(List<Object> data)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Restaurant>\n";
            xml += "\t<Company>\n";
            xml += "\t\t<NIF>" + ((Company)data[0]).NIF + "</NIF>\n";
            xml += "\t\t<Name>" + ((Company)data[0]).Name + "</Name>\n";
            xml += "\t\t<Phone>" + ((Company)data[0]).Phone + "</Phone>\n";
            xml += "\t\t<Fax>" + ((Company)data[0]).Fax + "</Fax>\n";
            xml += "\t\t<Email>" + ((Company)data[0]).Email + "</Email>\n";
            xml += "\t</Company>\n";
            xml += "\t<Address>\n";
            xml += "\t\t<Street>" + ((Address)data[1]).Street + "</Street>\n";
            xml += "\t\t<Number>" + ((Address)data[1]).Number + "</Number>\n";
            xml += "\t\t<ZipCode>" + ((Address)data[1]).ZipCode + "</ZipCode>\n";
            xml += "\t\t<Town>" + ((Address)data[1]).Town + "</Town>\n";
            xml += "\t\t<State>" + ((Address)data[1]).State + "</State>\n";
            xml += "\t</Address>\n";
            xml += "\t<Rate iva=\"" + data[2] + "\" discount=\"" + data[3] + "\" discountedVisit=\"" + data[4] + "\"/>\n";
            xml += "</Restaurant>";
            return xml;
        }
    }

    public class Company
    {
        private string name, nif, email;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string NIF
        {
            get { return nif; }
            set { nif = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        private int phone, fax;

        public int Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public int Fax
        {
            get { return fax; }
            set { fax = value; }
        }

        public Company() { }

        public Company(string name, string nif, int phone, int fax, string email)
        {
            Name = name;
            NIF = nif;
            Phone = phone;
            Fax = fax;
            Email = email;
        }
    }
}
