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
    public class WalletController : ControllerBase
    {

        private readonly ILogger<WalletController> _logger;
        private readonly IConfiguration _config;


        private BankContext _db;

        public WalletController(ILogger<WalletController> logger, IConfiguration config, BankContext bankContext)
        {
            _logger = logger;
            _config = config;
            _db = bankContext;
        }

        // Retrieve the wallet plus all the transactions the wallet was involved in
        [HttpGet("{publicKey}")]
        public IActionResult Get(string publicKey)
        {
            try
            {
                WalletDTO walletDTO = getWalletsDTO(publicKey);

                if (walletDTO.walletInfo == null) return Ok(null);

                return Ok(walletDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // Generates a new wallet for an existing user
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
                string _public = keys[0];
                string _address = keys[1];
                string _secret = keys[2];

                CustodialWallet newWallet = new CustodialWallet();
                newWallet.publicKey = _public;
                newWallet.address = _address;
                newWallet.privateKey = _secret;
                newWallet.userPassport = passportID;
                newWallet.amount = "0";

                _db.Wallets.Add(newWallet);
                _db.SaveChanges();

                return Ok("wallet " + newWallet.publicKey + " created for user with passport " + passportID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // deposits some money into a wallet
        [HttpPut]
        [Route("{publicKey}/deposit/{amount}")]
        public IActionResult Put(string publicKey, string amount)
        {
            try
            {
                BigInteger amountBI = BigInteger.Parse(amount);
                CustodialWallet wallet = _db.Wallets.Find(publicKey);

                if (wallet == null)
                {
                    return BadRequest("Wallet with public Key " + publicKey + " does not exist");
                }

                BigInteger newAmount = BigInteger.Parse(wallet.amount) + amountBI;
                wallet.amount = newAmount.ToString();

                _db.SaveChanges();

                return Ok("Amount deposited in wallet with public key " + publicKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        internal WalletDTO getWalletsDTO(string publicKey)
        {
            WalletDTO walletDTO = new WalletDTO();

            CustodialWallet wallet = _db.Wallets.Find(publicKey);

            if (wallet != null)
            {
                var query = _db.Transactions.Where(t => t.publicKeySender == publicKey || t.publicKeyReceiver == publicKey);
                List<BankTransaction> walletTransactions = (query != null) ? query.ToList() : null;
                walletDTO = new WalletDTO(wallet, walletTransactions);
            }

            return walletDTO;
        }

        internal List<string> generateKeys()
        {
            List<string> keys = new List<string>();

            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            string Password = _config.GetSection("Password").Value;

            var wallet = new Nethereum.HdWallet.Wallet(mnemo.ToString(), Password);

            keys.Add(wallet.GetAccount(0).PublicKey);
            keys.Add(wallet.GetAccount(0).Address);
            keys.Add(wallet.GetAccount(0).PrivateKey);

            return keys;
        }
    }
}
