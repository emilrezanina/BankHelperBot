using BankHelperBot.Details;

namespace BankHelperBot.Services
{
    public interface ILoanFormViewService
    {
        LoanFormDetails GetLoanFormDetail(int loanId);
    }
}