
let Bank = artifacts.require("./Bank");
let TransparentUpgradeableProxy = artifacts.require("@openzeppelin/contracts/proxy/transparent/TransparentUpgradeableProxy.sol")


module.exports = async function(deployer, network, accounts){
 
  const Owner = accounts[0];

  // Libraries -----------------------------------------------------------------------------------------------------------------------------------------------------------------
  await deployer.deploy(Bank, Owner);
  BankInstance = await Bank.deployed();
  console.log("Bank deployed");

  /*await deployer.deploy(TransparentUpgradeableProxy, BankInstance.address, Owner);
  TransparentUpgradeableProxyIns = await TransparentUpgradeableProxy.deployed();
  var ProxyAddress = TransparentUpgradeableProxyIns.address;
  console.log("Proxy deployed : " + ProxyAddress);*/

}