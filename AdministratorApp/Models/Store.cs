﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdministratorApp.Models
{
    public class Store
    {
        

        public Store(string name, string address, int telephone, string manager)
        {
            Name = name;
            Address = address;
            Telephone = telephone;
            Manager = manager;
        }

        

        public string Name { get; set; }
        public string Address { get; set; }
        public int Telephone { get; set; }
        public string Manager { get; set; }

        public Store AddStore()
        {

        }

        public override string ToString()
        {
            return $"{Name} {Address} {Telephone} {Manager}";
        }
    }
}
