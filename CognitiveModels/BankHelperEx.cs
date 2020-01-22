using System.Linq;

namespace BankHelperBot
{
    public partial class BankHelper
    {
        public int LoanId => Entities.LoanId?.FirstOrDefault() ?? -1;
    }
}