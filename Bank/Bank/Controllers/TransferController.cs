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
    public class TransferController : ControllerBase
    {

        private readonly ILogger<TransferController> _logger;

        private BankContext _db;

        public TransferController(ILogger<TransferController> logger, BankContext bankContext)
        {
            _logger = logger;
            _db = bankContext;
        }

        // Transfer funds from account A to account B if both exist and account A has enough funds
        [HttpPost]
        public IActionResult Post([FromBody] TransferInputDTO transaction)
        {
            try 
            {

                if (_db.Accounts.Find(transaction.addressSender) == null ||
                    _db.Accounts.Find(transaction.addressReceiver) == null)
                {
                    return BadRequest("Transaction content is wrong");
                }

                Account sender = _db.Accounts.Find(transaction.addressSender);
                Account receiver = _db.Accounts.Find(transaction.addressReceiver);

                if (BigInteger.Parse(sender.amount) < BigInteger.Parse(transaction.amount))
                {
                    return BadRequest("Sender does not have enough funds");
                }

                sender.amount = (BigInteger.Parse(sender.amount) - BigInteger.Parse(transaction.amount)).ToString();
                receiver.amount = (BigInteger.Parse(receiver.amount) + BigInteger.Parse(transaction.amount)).ToString();

                Transfer newTtransfer = getTransfer(transaction);

                _db.Transfers.Add(newTtransfer);

                _db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
            
        }

        internal Transfer getTransfer(TransferInputDTO transfer)
        {
            Transfer newTtransfer = new Transfer();
            newTtransfer.id = Guid.NewGuid();
            newTtransfer.addressSender = transfer.addressSender;
            newTtransfer.addressReceiver = transfer.addressReceiver;
            newTtransfer.amount = transfer.amount;
            newTtransfer.transationDate = DateTime.Now;

            return newTtransfer;
        }

    }
}
