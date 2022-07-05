using Bank.DTO;
using Bank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Numerics;

namespace Bank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {

        private readonly ILogger<TransactionController> _logger;

        private BankContext _db;

        public TransactionController(ILogger<TransactionController> logger, BankContext bankContext)
        {
            _logger = logger;
            _db = bankContext;
        }

        // Transfer funds from account A to account B if both exist and account A has enough funds
        [HttpPost]
        public IActionResult Post([FromBody] TransactionDTO transaction)
        {
            try 
            {
                transaction.addTransactionId();

                if (_db.Wallets.Find(transaction.transactionInfo.publicKeySender) == null ||
                    _db.Wallets.Find(transaction.transactionInfo.publicKeyReceiver) == null ||
                    transaction.transactionInfo.transationDate == null)
                {
                    return BadRequest("Transaction content is wrong");
                }

                CustodialWallet sender = _db.Wallets.Find(transaction.transactionInfo.publicKeySender);
                CustodialWallet receiver = _db.Wallets.Find(transaction.transactionInfo.publicKeyReceiver);

                if (BigInteger.Parse(sender.amount) < BigInteger.Parse(transaction.transactionInfo.amount))
                {
                    return BadRequest("Sender does not have enough funds");
                }

                sender.amount = (BigInteger.Parse(sender.amount) - BigInteger.Parse(transaction.transactionInfo.amount)).ToString();
                receiver.amount = (BigInteger.Parse(receiver.amount) + BigInteger.Parse(transaction.transactionInfo.amount)).ToString();

               

                _db.Transactions.Add(transaction.transactionInfo);

                _db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
            
        }

    }
}
