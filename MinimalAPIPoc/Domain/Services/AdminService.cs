using Microsoft.EntityFrameworkCore;
using MinimalAPIPoc.Domain.DTO;
using MinimalAPIPoc.Domain.Entities;
using MinimalAPIPoc.Domain.Interfaces;
using MinimalAPIPoc.Infrastructure.Db;

namespace MinimalAPIPoc.Domain.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _db;

        public AdminService(ApplicationDbContext db)
        {
            _db = db;
        }

        public Admin Login(LoginDTO loginDTO)
        {
            var admin = _db.Admins.Where(a => a.Username == loginDTO.Username && a.Password == loginDTO.Password).FirstOrDefault();

            return admin!;
        }
    }
}
