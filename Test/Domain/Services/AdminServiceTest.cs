using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalAPIPoc.Domain.Entities;
using MinimalAPIPoc.Domain.Enums;
using MinimalAPIPoc.Domain.Services;
using MinimalAPIPoc.Infrastructure.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain.Services
{
    [TestClass]
    public class AdminServiceTest
    {
        public ApplicationDbContext CreateTestContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
        
        [TestMethod]
        public void AdminPersistanceTest() 
        {
            //Arrange
            using var context = CreateTestContext();

            var adminService = new AdminService(context);

            var admin = new Admin();
            admin.Username = "testAdmin";
            admin.Password = "testPassword";
            admin.Role = Role.Admin.ToString();

            //Act
            adminService.Create(admin);

            //Assert
            Assert.AreEqual(1, adminService.GetAllAdmins(1).Count);
        }

        [TestMethod]
        public void GetByUsernameTest()
        {
            //Arrange
            using var context = CreateTestContext();

            var adminService = new AdminService(context);

            var admin = new Admin();
            admin.Username = "testAdmin";
            admin.Password = "testPassword";
            admin.Role = Role.Admin.ToString();

            //Act
            adminService.Create(admin);
            var retrievedAdmin = adminService.GetByUsername(admin.Username);

            //Assert
            Assert.AreEqual(admin.Username, retrievedAdmin.Username);
        }

        [TestMethod]
        public void GetAllAdmins_ShouldReturnPaginatedResults()
        {
            // Arrange
            using var context = CreateTestContext();

            for (int i = 1; i <= 25; i++)
            {
                context.Admins.Add(new Admin
                {
                    Username = $"admin{i}",
                    Password = $"pass{i}",
                    Role = "user"
                });
            }

            context.SaveChanges();

            var service = new AdminService(context);

            // Act
            var page1 = service.GetAllAdmins(1);
            var page2 = service.GetAllAdmins(2);
            var page3 = service.GetAllAdmins(3);
            var pageNull = service.GetAllAdmins(null);
            var pageZero = service.GetAllAdmins(0);

            // Assert
            Assert.AreEqual(10, page1.Count);
            Assert.AreEqual("admin1", page1.First().Username);
            Assert.AreEqual("admin10", page1.Last().Username);

            Assert.AreEqual(10, page2.Count);
            Assert.AreEqual("admin11", page2.First().Username);
            Assert.AreEqual("admin20", page2.Last().Username);

            Assert.AreEqual(5, page3.Count);
            Assert.AreEqual("admin21", page3.First().Username);
            Assert.AreEqual("admin25", page3.Last().Username);

            Assert.AreEqual(10, pageNull.Count); // null deve ser tratado como página 1
            Assert.AreEqual("admin1", pageNull.First().Username);

            Assert.AreEqual(10, pageZero.Count); // página 0 deve virar página 1
            Assert.AreEqual("admin1", pageZero.First().Username);
        }
    }
}
