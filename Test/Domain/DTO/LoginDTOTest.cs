using MinimalAPIPoc.Domain.DTO;

namespace Test.Domain.DTO
{
    [TestClass]
    public class LoginDTOTest
    {
        [TestMethod]
        public void TestGetSetProperties()
        {
            // Arrange
            var loginDTO = new LoginDTO();
            var testUsername = "testUser";
            var testPassword = "testPassword";
            // Act
            loginDTO.Username = testUsername;
            loginDTO.Password = testPassword;
            // Assert
            Assert.AreEqual(testUsername, loginDTO.Username);
            Assert.AreEqual(testPassword, loginDTO.Password);
        }
    }
}
