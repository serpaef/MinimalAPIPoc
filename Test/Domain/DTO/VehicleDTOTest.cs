using MinimalAPIPoc.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain.DTO
{
    [TestClass]
    public class VehicleDTOTest
    {
        [TestMethod]
        public void TestGetSetProperties()
        {
            // Arrange
            var vehicleDTO = new VehicleDTO();
            var testMake = "Toyota";
            var testName = "Camry";
            var testYear = 2020;
            // Act
            vehicleDTO.Make = testMake;
            vehicleDTO.Name = testName;
            vehicleDTO.Year = testYear;
            // Assert
            Assert.AreEqual(testMake, vehicleDTO.Make);
            Assert.AreEqual(testName, vehicleDTO.Name);
            Assert.AreEqual(testYear, vehicleDTO.Year);
        }
    }
}
