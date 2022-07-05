using Bank.DTO;
using Bank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;

        private BankContext _db;

        public UserController(ILogger<UserController> logger, BankContext bankContext)
        {
            _logger = logger;
            _db = bankContext;
        }

        // Retrieve all the Users and their wallets
        [HttpGet]
        public IActionResult Get()
        {
            try 
            {
                List<string> passports = _db.Users.Select(u => u.passportId).ToList();
                List<UserDTO> userDTOs = getUsersDTO(passports);
                return Ok(userDTOs);
            }
            catch (Exception ex) { 
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }

        }

        // Retreive one specific user and its wallets
        [HttpGet("{passportID}")]
        public IActionResult Get(string passportID)
        {
            try
            {
                List<string> passports = new List<string>() { passportID };
                List<UserDTO> userDTOs = getUsersDTO(passports);

                if(userDTOs.Count == 0) return Ok(null);
                else if (userDTOs.Count > 1) throw new Exception("More than one single user, unexpected error");

                return Ok(userDTOs[0]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // Create new user (only if it does not already exist)
        [HttpPost]
        public IActionResult Post( [FromBody] User _user)
        {
            try
            {
                if (_db.Users.Find(_user.passportId) != null)
                {
                    return BadRequest("User with passport " + _user.passportId + " already exists");
                }

                _db.Users.Add(_user);
                _db.SaveChanges();

                return Ok("User with passport " + _user.passportId + " added");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }



        internal List<UserDTO> getUsersDTO(List<string> passportIds)
        {
            List<UserDTO> users = new List<UserDTO>();

            foreach(string passportId in passportIds)
            {
                User user = _db.Users.Find(passportId);
                if (user != null)
                {
                    var query = _db.Wallets.Where(w => w.userPassport == passportId);
                    List<CustodialWallet> userWallets = (query != null) ? query.ToList() : null;
                    users.Add(new UserDTO(user, userWallets));
                }
            }

            return users;


        }
    }
}
