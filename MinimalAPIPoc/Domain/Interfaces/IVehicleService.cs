using MinimalAPIPoc.Domain.Entities;

namespace MinimalAPIPoc.Domain.Interfaces
{
    public interface IVehicleService
    {
        List<Vehicle> GetAllVehicles(int? page = 1, string? name = null, string? make = null);
        Vehicle? GetVehicleById(int id);
        void CreateVehicle(Vehicle vehicle);
        void UpdateVehicle(Vehicle vehicle);
        void DeleteVehicle(int id);
    }
}
