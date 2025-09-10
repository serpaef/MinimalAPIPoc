using MinimalAPIPoc.Domain.Entities;
using MinimalAPIPoc.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain.Entities
{
    [TestClass]
    public class AdminTest
    {
        [TestMethod]
        public void TestGetSetProperties()
        {
            //arrange
            var admin = new Admin();

            //act
            admin.Id = 1;
            admin.Username = "adminUser";
            admin.Password = "adminPass";
            admin.Role = Role.Admin.ToString();

            //assert
            Assert.AreEqual(1, admin.Id);
            Assert.AreEqual("adminUser", admin.Username);
            Assert.AreEqual("adminPass", admin.Password);
            Assert.AreEqual("Admin", admin.Role);
        }

    }
}
