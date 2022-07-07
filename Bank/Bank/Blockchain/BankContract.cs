using Microsoft.Extensions.Configuration;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bank.Blockchain
{
    public class BankContract : IBankContract
    {
        private readonly IConfiguration _config;
        private string clientURL;
        private string OwnerPrivateKey;
        private Account account;
        private Web3 web3;
        private string abi;
        private string bankContractAddress;
        private Nethereum.Contracts.Contract contract;

        public BankContract(IConfiguration config)
        {
            _config = config;
            initialize();
        }

        public void initialize()
        {
            clientURL = _config.GetSection("Blockchain").GetSection("clientURL").Value;
            bankContractAddress = _config.GetSection("Blockchain").GetSection("bankContractAddress").Value;
            abi = File.ReadAllText(@"./Blockchain/contractABI.json");
            OwnerPrivateKey = _config.GetSection("Blockchain").GetSection("OwnerSecretKey").Value;
            account = new Account(OwnerPrivateKey);
            web3 = new Web3(account, clientURL);
            web3.TransactionManager.UseLegacyAsDefault = true;
            contract = web3.Eth.GetContract(abi, bankContractAddress);
        }

        public async Task<bool> ActivateAccountAsync(string address)
        {
            object[] inputParams = new object[1];
            inputParams[0] = address;

            return await invokeBlockchainFunction(account.Address, "activateAccount", inputParams);
        }

        public async Task<bool> DepositAsync(string address, string amount)
        {
            object[] inputParams = new object[2];
            inputParams[0] = address;
            inputParams[1] = amount;

            return await invokeBlockchainFunction(account.Address, "depositFunds", inputParams);
        }

        public async Task<bool> TransferAsync(string addressSender, string addressReceiver, string amount, string privateKeySender)
        {
            object[] inputParams = new object[3];
            inputParams[0] = addressSender;
            inputParams[1] = addressReceiver;
            inputParams[2] = amount;

            return await invokeBlockchainFunction(account.Address, "transferFundsFor", inputParams);
        }


        public async Task<bool> invokeBlockchainFunction(string senderAddress, string functionName, object[] input)
        {
            try
            {
                var Function = contract.GetFunction(functionName);

                var gas = await Function.EstimateGasAsync(senderAddress, null, null, input);

                var receipt =
                    await Function.SendTransactionAndWaitForReceiptAsync(senderAddress, gas, null, null, input);

                if (receipt.Status.Value == 0) return false;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
   

    }
}
