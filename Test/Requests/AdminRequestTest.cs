using MinimalAPIPoc.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Test.Helpers;

namespace Test.Requests
{
    [TestClass]
    public class AdminRequestTest
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Setup.ClassInit(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }

        [TestMethod]
        public async Task PostLoginTest()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Username = "admin1",
                Password = "password1"
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");

            // Act
            var response = await Setup.client.PostAsync("/admin/login", content);

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
        }

    }
}
