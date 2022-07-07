
let Bank = artifacts.require("./Bank");
let TransparentUpgradeableProxy = artifacts.require("@openzeppelin/contracts/proxy/transparent/TransparentUpgradeableProxy.sol")


module.exports = async function(deployer, network, accounts){
 
  const Owner = "0x3184FA428bB8A71f8DBA653b3FEB7E07A220D748";

  // Libraries -----------------------------------------------------------------------------------------------------------------------------------------------------------------
  await deployer.deploy(Bank);
  BankInstance = await Bank.deployed();
  console.log("Bank deployed");

  var BankProxyInitializerMethod = {
    "inputs": [
      {
        "internalType": "address",
        "name": "owner",
        "type": "address"
      }
    ],
    "name": "Bank_init",
    "outputs": [],
    "stateMutability": "nonpayable",
    "type": "function"
  };

  var BankProxyInitializerParameters = [Owner];
  var BankProxyData = web3.eth.abi.encodeFunctionCall(BankProxyInitializerMethod, BankProxyInitializerParameters);


  await deployer.deploy(TransparentUpgradeableProxy, BankInstance.address, Owner, BankProxyData);
  TransparentUpgradeableProxyIns = await TransparentUpgradeableProxy.deployed();
  var ProxyAddress = TransparentUpgradeableProxyIns.address;
  console.log("Proxy deployed : " + ProxyAddress);

}