using MinimalAPIPoc.Domain.DTO;
using MinimalAPIPoc.Domain.Entities;
using MinimalAPIPoc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Mocks
{
    internal class AdminServiceMock : IAdminService
    {
        private static List<Admin> _admins = new List<Admin>
        {
            new Admin { Id = 1, Username = "admin1", Password = "password1", Role = "Admin" },
            new Admin { Id = 2, Username = "admin2", Password = "password2", Role = "Editor" },
            new Admin { Id = 3, Username = "admin3", Password = "password3" },
            new Admin { Id = 4, Username = "admin4", Password = "password4" },
            new Admin { Id = 5, Username = "admin5", Password = "password5" },
            new Admin { Id = 6, Username = "admin6", Password = "password6" },
            new Admin { Id = 7, Username = "admin7", Password = "password7" },
            new Admin { Id = 8, Username = "admin8", Password = "password8" },
            new Admin { Id = 9, Username = "admin9", Password = "password9" },
            new Admin { Id = 10, Username = "admin10", Password = "password10" },
            new Admin { Id = 11, Username = "admin11", Password = "password11" },
            new Admin { Id = 12, Username = "admin12", Password = "password12" }
        };
        public Admin Create(Admin admin)
        {
            admin.Id = _admins.Max(a => a.Id) + 1;
            _admins.Add(admin);
            return admin;
        }

        public List<Admin> GetAllAdmins(int? page)
        {
            if (page == null || page < 1) page = 1;

            return _admins
                .Skip(((int)page - 1) * 10)
                .Take(10)
                .ToList();
        }

        public Admin? GetByUsername(string username)
        {
            return _admins.Find(a => a.Username == username);
        }

        public Admin Login(LoginDTO loginDTO)
        {
            return _admins.Find( a => a.Username == loginDTO.Username && a.Password == loginDTO.Password)!;
        }
    }
}
