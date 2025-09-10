using Microsoft.EntityFrameworkCore;
using MinimalAPIPoc.Domain.Entities;
using MinimalAPIPoc.Domain.Services;
using MinimalAPIPoc.Infrastructure.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain.Services
{
    [TestClass]
    public class VehicleServiceTest
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [TestMethod]
        public void CreateVehicle_ShouldAddVehicleToDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new VehicleService(context);

            var vehicle = new Vehicle
            {
                Name = "Civic",
                Make = "Honda",
                Year = 2020
            };

            // Act
            service.CreateVehicle(vehicle);

            // Assert
            var storedVehicle = context.Vehicles.FirstOrDefault();
            Assert.IsNotNull(storedVehicle);
            Assert.AreEqual("Civic", storedVehicle!.Name);
            Assert.AreEqual("Honda", storedVehicle.Make);
            Assert.AreEqual(2020, storedVehicle.Year);
        }

        [TestMethod]
        public void DeleteVehicle_ShouldRemoveVehicleById()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var vehicle = new Vehicle { Name = "F-150", Make = "Ford", Year = 2019 };
            context.Vehicles.Add(vehicle);
            context.SaveChanges();

            var service = new VehicleService(context);

            // Act
            service.DeleteVehicle(vehicle.Id);

            // Assert
            Assert.AreEqual(0, context.Vehicles.Count());
        }

        [TestMethod]
        public void GetAllVehicles_ShouldReturnPaginatedAndFilteredResults()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            for (int i = 1; i <= 25; i++)
            {
                context.Vehicles.Add(new Vehicle
                {
                    Name = $"Vehicle {i}",
                    Make = i % 2 == 0 ? "Toyota" : "Ford",
                    Year = 2015 + i
                });
            }

            context.SaveChanges();
            var service = new VehicleService(context);

            // Act
            var page1 = service.GetAllVehicles(1);
            var page2 = service.GetAllVehicles(2);
            var filteredByName = service.GetAllVehicles(1, name: "Vehicle 2"); // Matches Vehicle 2, 12, 20, 21, etc.
            var filteredByMake = service.GetAllVehicles(1, make: "Toyota");

            // Assert
            Assert.AreEqual(10, page1.Count);
            Assert.AreEqual(10, page2.Count);
            Assert.IsTrue(filteredByName.All(v => v.Name.ToLower().Contains("vehicle 2")));
            Assert.IsTrue(filteredByMake.All(v => v.Make == "Toyota"));
        }

        [TestMethod]
        public void GetVehicleById_ShouldReturnCorrectVehicle()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var vehicle = new Vehicle
            {
                Name = "Corolla",
                Make = "Toyota",
                Year = 2018
            };

            context.Vehicles.Add(vehicle);
            context.SaveChanges();

            var service = new VehicleService(context);

            // Act
            var result = service.GetVehicleById(vehicle.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Corolla", result!.Name);
        }

        [TestMethod]
        public void UpdateVehicle_ShouldModifyExistingVehicle()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var vehicle = new Vehicle
            {
                Name = "Camry",
                Make = "Toyota",
                Year = 2020
            };

            context.Vehicles.Add(vehicle);
            context.SaveChanges();

            var service = new VehicleService(context);

            // Act
            vehicle.Name = "Camry Hybrid";
            vehicle.Year = 2021;
            service.UpdateVehicle(vehicle);

            // Assert
            var updated = context.Vehicles.Find(vehicle.Id);
            Assert.IsNotNull(updated);
            Assert.AreEqual("Camry Hybrid", updated!.Name);
            Assert.AreEqual(2021, updated.Year);
        }
    }
}
