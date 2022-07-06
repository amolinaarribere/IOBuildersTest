using System;
using System.Threading.Tasks;

namespace Bank.Controllers.Common
{
    public static class waiting
    {
        internal static async Task waitForBlockchainOperation(Task<bool> blockchainOperationTask)
        {
            bool Result = await blockchainOperationTask;

            if (!Result)
            {
                throw new Exception("Blockchain operation did not work");
            }

        }
    }
}
