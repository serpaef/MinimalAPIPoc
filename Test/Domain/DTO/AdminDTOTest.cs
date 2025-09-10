using MinimalAPIPoc.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain.DTO
{
    [TestClass]
    public class AdminDTOTest
    {
        [TestMethod]
        public void TestGetSetProperties()
        {
            //arrange
            var adminDTO = new MinimalAPIPoc.Domain.DTO.AdminDTO();
            //act
            adminDTO.Username = "admin";
            adminDTO.Password = "password";
            adminDTO.Role = Role.Admin.ToString();
            //assert
            Assert.AreEqual("admin", adminDTO.Username);
            Assert.AreEqual("password", adminDTO.Password);
            Assert.AreEqual("Admin", adminDTO.Role);
        }
    }
}
