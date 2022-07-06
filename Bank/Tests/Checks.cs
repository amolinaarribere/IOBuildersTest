using Bank.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public static class Checks
    {
        public static bool CheckUser(User expectedUser, User actualUSer)
        {
            if (expectedUser == null || actualUSer == null) return false;

            if (string.Compare(expectedUser.salt, actualUSer.salt) != 0 ||
                string.Compare(expectedUser.passportId, actualUSer.passportId) != 0 ||
                expectedUser.birthdate != actualUSer.birthdate ||
                string.Compare(expectedUser.familyName, actualUSer.familyName) != 0 ||
                string.Compare(expectedUser.address, actualUSer.address) != 0 ||
                string.Compare(expectedUser.name, actualUSer.name) != 0 ||
                string.Compare(expectedUser.password, actualUSer.password) != 0) return false;

            return true;
        }

        public static bool CheckAccount(Account expectedAccount, Account actualAccount)
        {
            if (string.Compare(expectedAccount.address, actualAccount.address) != 0 ||
                    string.Compare(expectedAccount.publicKey, actualAccount.publicKey) != 0 ||
                    string.Compare(expectedAccount.privateKey, actualAccount.privateKey) != 0 ||
                    string.Compare(expectedAccount.userPassport, actualAccount.userPassport) != 0 ||
                    string.Compare(expectedAccount.amount, actualAccount.amount) != 0) return false;

            return true;
        }

        public static bool CheckTransfer(Transfer expectedTransfer, Transfer actualTransfer)
        {
            if (string.Compare(expectedTransfer.addressSender, actualTransfer.addressSender) != 0 ||
                    string.Compare(expectedTransfer.addressReceiver, actualTransfer.addressReceiver) != 0 ||
                    string.Compare(expectedTransfer.amount, actualTransfer.amount) != 0 ||
                    expectedTransfer.transationDate != actualTransfer.transationDate ||
                    expectedTransfer.id != actualTransfer.id) return false;

            return true;
        }
    }
}
