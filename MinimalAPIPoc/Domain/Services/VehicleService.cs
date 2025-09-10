using MinimalAPIPoc.Domain.Entities;
using MinimalAPIPoc.Domain.Interfaces;
using MinimalAPIPoc.Infrastructure.Db;

namespace MinimalAPIPoc.Domain.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _context;

        public VehicleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
        }

        public void DeleteVehicle(int id)
        {
            var vehicle = _context.Vehicles.Find(id);

            if (vehicle is not null)
            {
                _context.Vehicles.Remove(vehicle);
                _context.SaveChanges();
                return;
            }
        }

        public List<Vehicle> GetAllVehicles(int? page = 1, string? name = null, string? make = null)
        {
            var vehicleQuery = _context.Vehicles.AsQueryable();
            
            if (page == null || page < 1) page = 1;

            if (!string.IsNullOrEmpty(name))
            {
                vehicleQuery = vehicleQuery.Where(v => v.Name.ToLower().Contains(name.ToLower()));
            }

            if (!string.IsNullOrEmpty(make))
            {
                vehicleQuery = vehicleQuery.Where(v => v.Make.ToLower().Contains(make.ToLower()));
            }

            const int PAGE_SIZE = 10;


            vehicleQuery = vehicleQuery
                .Skip(((int)page - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE);

            return vehicleQuery.ToList();

        }

        public Vehicle? GetVehicleById(int id)
        {
            return _context.Vehicles.Find(id);
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            _context.SaveChanges();
        }
    }
}
