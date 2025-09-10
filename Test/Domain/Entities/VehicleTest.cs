using MinimalAPIPoc.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain.Entities
{  
    [TestClass]
    public class VehicleTest
    {
        [TestMethod]
        public void TestGetSetProperties()
        {
            //arrange
            var vehicle = new Vehicle();
            //act
            vehicle.Id = 1;
            vehicle.Make = "Toyota";
            vehicle.Name = "Camry";
            vehicle.Year = 2020;
            //assert
            Assert.AreEqual(1, vehicle.Id);
            Assert.AreEqual("Toyota", vehicle.Make);
            Assert.AreEqual("Camry", vehicle.Name);
            Assert.AreEqual(2020, vehicle.Year);
        }

    }
}
