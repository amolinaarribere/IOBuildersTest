using Bank.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Bank.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Bank.DTO;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private BankContext _db;

        public UserControllerTests()
        {
            var Init = new Init<UserController>();

            _db = Init.createContext();

            _controller = new UserController(Init.createLogger(), _db);

            Init.FillDB(_db);
        }

        [TestMethod]
        public void GetUsers()
        {
            // Arrange
            List<UserOutputDTO> expectedOutput = generateOutput();

            // Act
            var result = _controller.Get() as OkObjectResult;
            var usersOutput = result.Value as List<UserOutputDTO>;

            // Assert
            CheckUsersOutput(expectedOutput, usersOutput);
        }

        [TestMethod]
        public void GetUser()
        {
            // Arrange
            List<UserOutputDTO> expectedOutput = generateOutput();

            // Act
            List<User> users = _db.Users.ToListAsync().Result;
            List<UserOutputDTO> usersOutput = new List<UserOutputDTO>();

            foreach (User user in users)
            {
                var result = _controller.Get(user.passportId) as OkObjectResult;
                var resultContent = result.Value as UserOutputDTO?;
                usersOutput.Add((UserOutputDTO)resultContent);
            }

            // Assert
            CheckUsersOutput(expectedOutput, usersOutput);
        }

        [TestMethod]
        public void PostUsers()
        {
            // Arrange
            var newTestUsers = File.ReadAllText(@"../../../Files/NewTestUsers.json");
            List<User> newUsers = JsonConvert.DeserializeObject<List<User>>(newTestUsers);

            // Act
            foreach(User newUser in newUsers)
            {
                _controller.Post(newUser);
            }

            // Assert
            foreach(User newUser in newUsers)
            {
                Assert.IsTrue(Checks.CheckUser(newUser, _db.Users.Find(newUser.passportId)));
            }
        }

        public List<UserOutputDTO> generateOutput()
        {
            List<UserOutputDTO> output = new List<UserOutputDTO>();

            List<User> users = _db.Users.ToListAsync().Result;


            foreach (User user in users)
            {
                var query = _db.Accounts.Where(w => w.userPassport == user.passportId);
                List<Account> userAccounts = (query != null) ? query.ToList() : null;

                output.Add(new UserOutputDTO(user, userAccounts));
            }

            return output;
        }

        public bool CheckUserOutput(UserOutputDTO expectedUserOutput, UserOutputDTO actualUserOutput)
        {
            if(!Checks.CheckUser(expectedUserOutput.userInfo, actualUserOutput.userInfo)) return false;

            if (expectedUserOutput.accounts.Count != actualUserOutput.accounts.Count) return false;

            int i = 0;

            foreach(Account account in expectedUserOutput.accounts)
            {
                if (!Checks.CheckAccount(account, actualUserOutput.accounts[i++])) return false;
            }

            return true;
        }

        public void CheckUsersOutput(List<UserOutputDTO> expectedUsersOutput, List<UserOutputDTO> actualUsersOutput)
        {
            int i = 0;
            Assert.AreEqual(expectedUsersOutput.Count, actualUsersOutput.Count);

            foreach (UserOutputDTO expectedUserOutput in expectedUsersOutput)
            {
                Assert.IsTrue(CheckUserOutput(expectedUserOutput, actualUsersOutput[i++]));
            }
        }
    }
}
