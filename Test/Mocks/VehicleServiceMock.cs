using MinimalAPIPoc.Domain.Entities;
using MinimalAPIPoc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Mocks
{
    public class VehicleServiceMock : IVehicleService
    {
        private static List<Vehicle> _vehicles = new List<Vehicle>
        {
            new Vehicle { Id = 1, Name = "Civic", Make = "Honda", Year = 2020 },
            new Vehicle { Id = 2, Name = "Accord", Make = "Honda", Year = 2019 },
            new Vehicle { Id = 3, Name = "Model S", Make = "Tesla", Year = 2021 },
            new Vehicle { Id = 4, Name = "Model 3", Make = "Tesla", Year = 2022 },
            new Vehicle { Id = 5, Name = "Mustang", Make = "Ford", Year = 2018 },
            new Vehicle { Id = 6, Name = "F-150", Make = "Ford", Year = 2017 },
            new Vehicle { Id = 7, Name = "Camry", Make = "Toyota", Year = 2020 },
            new Vehicle { Id = 8, Name = "Corolla", Make = "Toyota", Year = 2019 },
            new Vehicle { Id = 9, Name = "Altima", Make = "Nissan", Year = 2021 },
            new Vehicle { Id = 10, Name = "Sentra", Make = "Nissan", Year = 2022 },
            new Vehicle { Id = 11, Name = "3 Series", Make = "BMW", Year = 2018 },
            new Vehicle { Id = 12, Name = "5 Series", Make = "BMW", Year = 2017 }
        };

        public void CreateVehicle(Vehicle vehicle)
        {
            vehicle.Id = _vehicles.Max(v => v.Id) + 1;
            _vehicles.Add(vehicle);
        }

        public void DeleteVehicle(int id)
        {
            _vehicles.RemoveAll(v => v.Id == id);
        }

        public List<Vehicle> GetAllVehicles(int? page = 1, string? name = null, string? make = null)
        {
            return _vehicles
                .Where(v => (string.IsNullOrEmpty(name) || v.Name.ToLower().Contains(name.ToLower())) &&
                            (string.IsNullOrEmpty(make) || v.Make.ToLower().Contains(make.ToLower())))
                .Skip(((page ?? 1) - 1) * 10)
                .Take(10)
                .ToList();
        }

        public Vehicle? GetVehicleById(int id)
        {
            return _vehicles.FirstOrDefault(v => v.Id == id);
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            var existingVehicle = _vehicles.FirstOrDefault(v => v.Id == vehicle.Id);
            if (existingVehicle is not null)
            {
                existingVehicle.Name = vehicle.Name;
                existingVehicle.Make = vehicle.Make;
                existingVehicle.Year = vehicle.Year;
            }
        }
    }
}
