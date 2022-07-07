// SPDX-License-Identifier: GPL-3.0

pragma solidity 0.8.10;

/**
 * @title Storage
 * @dev Store & retrieve value in a variable
 */


interface IBank {

    function activateAccount(address account) external;
    function depositFunds(address account, uint256 funds) external;
    function transferFunds(address To, uint256 funds) external;
    function transferFundsFor(address From, address To, uint256 funds) external;

}