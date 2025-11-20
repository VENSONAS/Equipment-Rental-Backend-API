using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Domain.Models
{
    public class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; }
       
        public User() { }

        public void SetCreatedTime()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public void ChangeName(string newName)
        {
            Name = newName;
        }

        public void ChangeEmail(string newEmail)
        {
            Email = newEmail;
        }

        public void Promote()
        {
            Role = UserRole.Admin;
        }
    }

    public enum UserRole
    {
        Customer,
        Admin
    }
}
