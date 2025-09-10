using Microsoft.IdentityModel.Tokens;
using MinimalAPIPoc.Domain.DTO;
using MinimalAPIPoc.Domain.Entities;
using MinimalAPIPoc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain.Interfaces
{
    public class AdminServiceMock : IAdminService
    {
        public Admin Create(Admin admin)
        {
            return new Admin();
        }

        public List<Admin> GetAllAdmins(int? page)
        {
            var admins = new List<Admin>
            {
                new Admin { Id = 1, Username = "admin1", Password = "password1", Role = "SuperAdmin" },
                new Admin { Id = 2, Username = "admin2", Password = "password2", Role = "Admin" }
            };

            return admins;
        }

        public Admin? GetByUsername(string username)
        {
            if (String.IsNullOrEmpty(username)) return null;
            return new Admin { Id = 1, Username = "admin1", Password = "password1", Role = "SuperAdmin" };
        }

        public Admin Login(LoginDTO loginDTO)
        {
            return new Admin { Id = 1, Username = loginDTO.Username, Password = loginDTO.Password, Role = "SuperAdmin" };
        }
    }

    [TestClass]
    public class IAdminServiceTest
    {
        [TestMethod]
        public void TestLogin()
        {
            // Arrange
            var adminService = new AdminServiceMock();
            var loginDTO = new LoginDTO { Username = "admin1", Password = "password1" };
            // Act
            var result = adminService.Login(loginDTO);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(typeof(Admin).IsInstanceOfType(result));
        }

        [TestMethod]
        public void TestGetByUsername()
        {
            // Arrange
            var adminService = new AdminServiceMock();
            var username = "admin1";
            // Act
            var resultWithUsername = adminService.GetByUsername(username);
            var resultWithNull = adminService.GetByUsername(null);
            var resultWithEmpty = adminService.GetByUsername(string.Empty);
            // Assert
            Assert.IsNotNull(resultWithUsername);
            Assert.IsTrue(typeof(Admin).IsInstanceOfType(resultWithUsername));
            Assert.IsNull(resultWithNull);
            Assert.IsNull(resultWithEmpty);
        }

        [TestMethod]
        public void TestGetAllAdmins()
        {
            // Arrange
            var adminService = new AdminServiceMock();
            // Act
            var resultWithPage = adminService.GetAllAdmins(1);
            var resultWithoutPage = adminService.GetAllAdmins(null);
            // Assert
            Assert.IsNotNull(resultWithPage);
            Assert.IsTrue(typeof(List<Admin>).IsInstanceOfType(resultWithPage));
            Assert.IsNotNull(resultWithoutPage);
            Assert.IsTrue(typeof(List<Admin>).IsInstanceOfType(resultWithoutPage));
        }

        [TestMethod]
        public void TestCreate()
        {
            // Arrange
            var adminService = new AdminServiceMock();
            var newAdmin = new Admin { Username = "newadmin", Password = "newpassword", Role = "Admin" };
            // Act
            var result = adminService.Create(newAdmin);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(typeof(Admin).IsInstanceOfType(result));
        }
    }
}
