using BankHelperBot.Details;

namespace BankHelperBot.Services
{
    public class FakeLoanFormViewService : ILoanFormViewService
    {
        public LoanFormDetails GetLoanFormDetail(int loanId)
        {
            if (loanId == 1000)
            {
                return new LoanFormDetails()
                {
                    Id = loanId,
                    MaritalStatus = "Alone",
                    ChildCount = 0,
                    WorkStatus = true,
                    Income = 1500,
                    Debt = false
                };
            }

            return null;
        }
    }
}