// SPDX-License-Identifier: GPL-3.0

pragma solidity 0.8.10;

import './IBank.sol';
import "@openzeppelin/contracts/proxy/utils/Initializable.sol";
import "@openzeppelin/contracts/proxy/transparent/TransparentUpgradeableProxy.sol";


contract Bank is IBank, Initializable {
    // state
    struct Account{
        bool activated;
        uint256 balance;
    }

    mapping(address => Account) public _accounts;
    address public _owner;

    // events
    event AccountActivation(address indexed account);
    event Deposit(address indexed from, address indexed account, uint256 amount);
    event Transfer(address indexed from, address indexed To, uint256 amount);

    // modifiers
    modifier checkAccountActivation(address account, bool Activated){
        if(Activated) require(true == _accounts[account].activated, "Account must be activated");
        else require(false == _accounts[account].activated, "Account is already activated");
        _;
    }

    modifier isOwner(address account){
        require(_owner == account, "Not the owner");
        _;
    }

    modifier isBalanceEnough(address account, uint256 amount){
        require(_accounts[account].balance >= amount, "Not enough balance");
        _;
    }

    // initializer
    function Bank_init(address owner) public initializer {
        _owner = owner;
    }

    // functions
    function activateAccount(address account) external
        isOwner(msg.sender)
        checkAccountActivation(account, false)
    {
        _accounts[account].activated = true;

        emit AccountActivation(account);
    }

    function depositFunds(address account, uint256 funds) external
        isOwner(msg.sender)
        checkAccountActivation(account, true)
    {
        _accounts[account].balance += funds;

        emit Deposit(msg.sender, account, funds);
    }

    function transferFunds(address To, uint256 funds) external
    {
       transferFundsFromTo(msg.sender, To, funds);
    }

    function transferFundsFor(address From, address To, uint256 funds) external
        isOwner(msg.sender)
    {
       transferFundsFromTo(From, To, funds);

    }


    function transferFundsFromTo(address From, address To, uint256 funds) internal
        checkAccountActivation(From, true)
        checkAccountActivation(To, true)
        isBalanceEnough(From, funds)
    {
        _accounts[From].balance -= funds;
        _accounts[To].balance += funds;

        emit Transfer(From, To, funds);
    }

}