using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Receiver.domain
{
    class Client
    {
        private string dni, name, surname;

        public string Dni
        {
            get { return dni; }
            set { dni = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }

        private Address address;

        public Address Address
        {
            get { return address; }
            set { address = value; }
        }

        public Client(string dni, string name, string surname, Address address)
        {
            Dni = dni;
            Name = name;
            Surname = surname;
            Address = address;
        }
    }

    public class Address
    {
        private string street, number, town, state;

        public string Street
        {
            get { return street; }
            set { street = value; }
        }

        public string Number
        {
            get { return number; }
            set { number = value; }
        }

        public string Town
        {
            get { return town; }
            set { town = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        private int zipCode;

        public int ZipCode
        {
            get { return zipCode; }
            set { zipCode = value; }
        }

        public Address() { }
    }
}
