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

        public Client(string dni, string name, string surname)
        {
            Dni = dni;
            Name = name;
            Surname = surname;
        }
    }
}
