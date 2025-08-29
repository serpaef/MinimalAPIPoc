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

        public Admin Create(Admin admin)
        {
            _db.Admins.Add(admin);
            _db.SaveChanges();

            return admin;
        }

        public List<Admin> GetAllAdmins(int? page)
        {
            if (page == null || page < 1) page = 1;
            const int PAGE_SIZE = 10;

            var admins = _db.Admins
                .Skip(((int)page - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToList();

            return admins.ToList();
        }
    }
}
