// SPDX-License-Identifier: GPL-3.0

pragma solidity 0.8.10;

import './IBank.sol';

contract Bank is IBank{
    // state
    struct Account{
        bool activated;
        uint256 balance;
    }

    mapping(address => Account) private _accounts;
    address _owner;

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

    // constructor
    constructor(address owner){
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
        checkAccountActivation(msg.sender, true)
        checkAccountActivation(To, true)
        isBalanceEnough(msg.sender, funds)
    {
        _accounts[msg.sender].balance -= funds;
        _accounts[To].balance += funds;

        emit Deposit(msg.sender, To, funds);
    }
}