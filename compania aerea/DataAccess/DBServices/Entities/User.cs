﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DBServices.Entities
{
    public enum UserType
    {
        Socio,
        Cliente
    }
    public class User
   {//Las entidades tienen los mismos campos de la tabla de la base de datos,
    //además esto te permite  cambiar facilmente a Entity Framework.

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int PostalCode { get; set; }
        public string Street { get; set; }
        public int StreetNumber { get; set; }
        public string Neighborhood { get; set; }
        public int CityId { get; set; }
        public UserType Type { get; set; }
    }
}
