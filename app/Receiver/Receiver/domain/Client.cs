using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Receiver.domain
{
    // 'Client' contiene la información de un cliente
    public class Client
    {
        /* Atributos de la clase */
        private string dni, name, surname;
        // DNI
        public string Dni
        {
            get { return dni; }
            set { dni = value; }
        }
        // Nombre
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        // Apellido(s)
        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }

        // Dirección
        private Address address;
        public Address Address
        {
            get { return address; }
            set { address = value; }
        }

        /* Métodos constructores */
        public Client()
        {
            Address = new Address();
        }

        public Client(string dni, string name, string surname, Address address)
        {
            Dni = dni;
            Name = name;
            Surname = surname;
            Address = address;
        }
    }

    // 'Address' contiene la dirección del cliente
    public class Address
    {
        /* Atributos de la clase */
        private string street, number, town, state;
        // Calle
        public string Street
        {
            get { return street; }
            set { street = value; }
        }
        // Número de casa
        public string Number
        {
            get { return number; }
            set { number = value; }
        }
        // Localidad
        public string Town
        {
            get { return town; }
            set { town = value; }
        }
        // Provincia
        public string State
        {
            get { return state; }
            set { state = value; }
        }

        private int zipCode;
        // Código postal
        public int ZipCode
        {
            get { return zipCode; }
            set { zipCode = value; }
        }

        /* Métodos constructores */
        public Address() { }

        public Address(string street, string number, string town, string state, int zipcode)
        {
            Street = street;
            Number = number;
            Town = town;
            State = state;
            ZipCode = zipcode;
        }
    }
}
