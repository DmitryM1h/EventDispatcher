using System;
using System.Collections.Generic;
using System.Text;
using Users.Domain.Interfaces;

namespace Users.Domain
{
    public abstract class UserBase
    {
        public Guid Id { get; private set; }
    }
}
