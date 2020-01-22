using BankHelperBot.Details;
using Microsoft.Bot.Schema;

namespace BankHelperBot.Cards
{
    public interface ICardFactory
    {
        Attachment CreateWelcomeCard();
        Attachment CreateContactCard(LoanFormDetails loanFormDetails);
        Attachment CreateLoanFormCardAttachment(LoanFormDetails loanFormDetails);
    }
}