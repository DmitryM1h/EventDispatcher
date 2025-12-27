using System;
using System.Collections.Generic;
using System.Text;
using Users.Domain.Interfaces;

namespace Users.Domain
{
    public class User : UserBase, IEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }

    }
}
