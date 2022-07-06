using Bank.DTO;
using Bank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using NBitcoin;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.HdWallet;
using Microsoft.Extensions.Configuration;

namespace Bank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _config;
        private BankContext _db;

        public AccountController(ILogger<AccountController> logger, IConfiguration config, BankContext bankContext)
        {
            _logger = logger;
            _config = config;
            _db = bankContext;
        }

        // Retrieve the account plus all the transfers the account was involved in
        [HttpGet("{address}")]
        [ProducesResponseType(typeof(AccountOutputDTO), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult Get(string address)
        {
            try
            {
                AccountOutputDTO accountDTO = getAccountsDTO(address);

                if (accountDTO.accountInfo == null) return Ok(null);

                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // Generates a new account for an existing user
        [HttpPost("{passportID}")]
        public IActionResult Post(string passportID)
        {
            try
            {
                if (_db.Users.Find(passportID) == null)
                {
                    return BadRequest("User with passport " + passportID + " does not exist");
                }

                List<string> keys = generateKeys();

                Models.Account newAccount = generateAccount(keys, passportID);

                _db.Accounts.Add(newAccount);
                _db.SaveChanges();

                return Ok("account " + newAccount.address + " created for user with passport " + passportID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // deposits some money into a account
        [HttpPut]
        [Route("{address}/deposit/{amount}")]
        public IActionResult Put(string address, string amount)
        {
            try
            {
                BigInteger amountBI = BigInteger.Parse(amount);
                Models.Account account = _db.Accounts.Find(address);

                if (account == null)
                {
                    return BadRequest("account with address " + address + " does not exist");
                }

                account.amount = (BigInteger.Parse(account.amount) + amountBI).ToString();

                _db.Transfers.Add(generateDepositTransfer(address, amount));

                _db.SaveChanges();

                return Ok("Amount deposited in account with address " + address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        internal AccountOutputDTO getAccountsDTO(string address)
        {
            AccountOutputDTO accountDTO = new AccountOutputDTO();

            Models.Account account = _db.Accounts.Find(address);

            if (account != null)
            {
                var query = _db.Transfers.Where(t => t.addressSender == address || t.addressReceiver == address);
                List<Transfer> accountTransactions = (query != null) ? query.ToList() : null;
                accountDTO = new AccountOutputDTO(account, accountTransactions);
            }

            return accountDTO;
        }

        internal List<string> generateKeys()
        {
            List<string> keys = new List<string>();

            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            string Password = _config.GetSection("Password").Value;

            var wallet = new Wallet(mnemo.ToString(), Password);

            keys.Add(wallet.GetAccount(0).PublicKey);
            keys.Add(wallet.GetAccount(0).Address);
            keys.Add(wallet.GetAccount(0).PrivateKey);

            return keys;
        }

        internal Models.Account generateAccount(List<string> keys, string passportID)
        {
            Models.Account newAccount = new Models.Account();
            newAccount.publicKey = keys[0];
            newAccount.address = keys[1];
            newAccount.privateKey = keys[2];
            newAccount.userPassport = passportID;
            newAccount.amount = "0";

            return newAccount;
        }

        internal Transfer generateDepositTransfer(string address, string amount)
        {
            Transfer depositTtransfer = new Transfer();
            depositTtransfer.id = Guid.NewGuid();
            depositTtransfer.addressSender = "-";
            depositTtransfer.addressReceiver = address;
            depositTtransfer.amount = amount;
            depositTtransfer.transationDate = DateTime.Now;

            return depositTtransfer;
        }
    }
}
