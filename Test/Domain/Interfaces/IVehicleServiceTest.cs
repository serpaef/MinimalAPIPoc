using MinimalAPIPoc.Domain.Entities;
using MinimalAPIPoc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain.Interfaces
{
    internal class VehicleServiceMock : IVehicleService
    {
        public void CreateVehicle(Vehicle vehicle)
        {
            if (vehicle == null) throw new ArgumentException(nameof(vehicle));
            return;
        }

        public void DeleteVehicle(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID");
            return;
        }

        public List<Vehicle> GetAllVehicles(int? page = 1, string? name = null, string? make = null)
        {
            return new List<Vehicle>
            {
                new Vehicle { Id = 1, Make = "Toyota", Name = "Corolla", Year = 2020 },
                new Vehicle { Id = 2, Make = "Honda", Name = "Civic", Year = 2019 }
            };
        }

        public Vehicle? GetVehicleById(int id)
        {
            if (id <= 0) return null;
            return new Vehicle { Id = id, Make = "Toyota", Name = "Corolla", Year = 2020 };
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            if (vehicle == null) throw new ArgumentException(nameof(vehicle));
            return;
        }
    }

    [TestClass]
    public class IVehicleServiceTest
    {
        [TestMethod]
        public void TestGetVehicleById()
        {
            // Arrange
            var vehicleService = new VehicleServiceMock();
            int testId = 1;
            // Act
            var result = vehicleService.GetVehicleById(testId);
            var resultNull = vehicleService.GetVehicleById(0);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(typeof(Vehicle).IsInstanceOfType(result));
            Assert.IsNull(resultNull);
        }

        [TestMethod]
        public void TestGetAllVehicles()
        {
            // Arrange
            var vehicleService = new VehicleServiceMock();
            // Act
            var result = vehicleService.GetAllVehicles();
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(typeof(List<Vehicle>).IsInstanceOfType(result));
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCreateUpdateDeleteVehicle()
        {
            // Arrange
            var vehicleService = new VehicleServiceMock();
            var vehicle = new Vehicle { Id = 3, Make = "Ford", Name = "Focus", Year = 2018 };
            // Act & Assert
            vehicleService.CreateVehicle(null);
            vehicleService.UpdateVehicle(null);
            vehicleService.DeleteVehicle(0);
        }
    }
}
