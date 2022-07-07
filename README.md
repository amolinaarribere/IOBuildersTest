# IOBuildersTest

## Introduction

### How it works

This project implements a **"Closed Bank"**, a bank where its users are only allowed to transfer funds with each other *(they cannot send or receive funds to/from external users)*.

The **Bank owner** is the only one allowed to add users to the bank and mint funds which must be assigned to existing users the moment the get minted.

**Users** are identified by their *passport ID*, they can have one or several accounts and they cannot withdraw theirs funds, they can simply transfer them to other users accounts.

### Architecture
The project contains three components :

- **Smart Contract**: A smart contract deployed on the blockchain represents the bank with its activated accounts and their funds. "Users" are not represented on the blockchain, only their accounts.
- **Bank DB**: The Bank has an internal DB where it stores the information about its users, their accounts and all the deposits and transfers that took place.
- **Rest API** : The Bank exposes an API to interact with the blockchain contract. Users accounts are assigned a unique **custodial wallet**, the Bank holds the private keys as well as User PI in its internal DB.

## REST API

The REST API exposes three resources:

### Users
  
  - Creates new users that will be persisted on the Bank internal DB. 
  - Returns information about the existing users, their personal information (name, birthdate, ...) and their list of accounts.

### Accounts

  - Creates new accounts for existing users. This new accounts are activated in the blockchain contract and persisted in the Bank internal DB.
  - Assigns new funds to existing users accounts
  - Returns information about the existing accounts, their configuration information (address, private key, ...) and the list of transfers in which they were involved.

### Transfers

  - Used to transfer funds from one user account to another.


## Bank internal DB
All users, accounts and transfers are persisted in the Bank internal DB.

## Blockchain contract
A smart contract that keeps track of all the activated accounts (Addresses) and their funds.

Only the **Contract Owner (Bank owner)** can activate accounts and assign funds.

Only **accounts** can later transfer their funds to other accounts.