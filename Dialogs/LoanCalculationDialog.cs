using System.Threading;
using System.Threading.Tasks;
using BankHelperBot.Cards;
using BankHelperBot.Details;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace BankHelperBot.Dialogs
{
    public class LoanCalculationDialog : CancelAndHelpDialog
    {
        private const string MartialStatusStepMsgText = "What is your marital status?";
        private const string ChildrenCountStepMsgText = "How many children do you have?";
        private const string WorkStatusStepMsgText = "Are you employed?";
        private const string IncomeStepMsgText = "What is your monthly income?";
        private const string DebtStepMsgText = "Do you have any debt?";

        private readonly ICardFactory _cardFactory;

        public LoanCalculationDialog(ICardFactory cardFactory)
            : base(nameof(LoanCalculationDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                MaritalStatusStepAsync,
                ChildStepAsync,
                WorkStatusStepAsync,
                IncomeStepAsync,
                DebtStepAsync,
                ConfirmStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
            _cardFactory = cardFactory;
        }

        private async Task<DialogTurnResult> MaritalStatusStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var loanFormDetails = (LoanFormDetails) stepContext.Options;
            if (loanFormDetails.MaritalStatus == null)
            {
                var promptMessage = MessageFactory.Text(MartialStatusStepMsgText, MartialStatusStepMsgText,
                    InputHints.AcceptingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions() {Prompt = promptMessage},
                    cancellationToken);
            }

            return await stepContext.NextAsync(loanFormDetails.MaritalStatus, cancellationToken);
        }

        private async Task<DialogTurnResult> ChildStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var loanFormDetails = (LoanFormDetails) stepContext.Options;
            loanFormDetails.MaritalStatus = (string) stepContext.Result;
            if (loanFormDetails.ChildCount == null)
            {
                var promptMessage = MessageFactory.Text(ChildrenCountStepMsgText, ChildrenCountStepMsgText,
                    InputHints.AcceptingInput);
                return await stepContext.PromptAsync(nameof(NumberPrompt<int>),
                    new PromptOptions() {Prompt = promptMessage},
                    cancellationToken);
            }

            return await stepContext.NextAsync(loanFormDetails.ChildCount, cancellationToken);
        }

        private async Task<DialogTurnResult> WorkStatusStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var loanFormDetails = (LoanFormDetails) stepContext.Options;
            loanFormDetails.ChildCount = (int) stepContext.Result;
            if (loanFormDetails.WorkStatus == null)
            {
                var promptMessage = MessageFactory.Text(WorkStatusStepMsgText, WorkStatusStepMsgText,
                    InputHints.AcceptingInput);
                return await stepContext.PromptAsync(nameof(ConfirmPrompt),
                    new PromptOptions() {Prompt = promptMessage},
                    cancellationToken);
            }

            return await stepContext.NextAsync(loanFormDetails.WorkStatus, cancellationToken);
        }

        private async Task<DialogTurnResult> IncomeStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var loanFormDetails = (LoanFormDetails) stepContext.Options;
            loanFormDetails.WorkStatus = (bool) stepContext.Result;
            if (loanFormDetails.Income == null)
            {
                var promptMessage =
                    MessageFactory.Text(IncomeStepMsgText, IncomeStepMsgText, InputHints.AcceptingInput);
                return await stepContext.PromptAsync(nameof(NumberPrompt<int>),
                    new PromptOptions() {Prompt = promptMessage},
                    cancellationToken);
            }

            return await stepContext.NextAsync(loanFormDetails.Income, cancellationToken);
        }

        private async Task<DialogTurnResult> DebtStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var loanFormDetails = (LoanFormDetails) stepContext.Options;
            loanFormDetails.Income = (int) stepContext.Result;
            if (loanFormDetails.Debt == null)
            {
                var promptMessage = MessageFactory.Text(DebtStepMsgText, DebtStepMsgText, InputHints.AcceptingInput);
                return await stepContext.PromptAsync(nameof(ConfirmPrompt),
                    new PromptOptions() {Prompt = promptMessage},
                    cancellationToken);
            }

            return await stepContext.NextAsync(loanFormDetails.Debt, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var loanFormDetails = (LoanFormDetails) stepContext.Options;
            loanFormDetails.Debt = (bool) stepContext.Result;
            var confirmationCard = _cardFactory.CreateLoanFormCardAttachment(loanFormDetails);
            var response = MessageFactory.Attachment(confirmationCard);
            await stepContext.Context.SendActivitiesAsync(new[] {response}, cancellationToken);
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions(), cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if ((bool) stepContext.Result)
            {
                var loanFormDetails = (LoanFormDetails) stepContext.Options;
                var finalCard = _cardFactory.CreateContactCard(loanFormDetails);
                var response = MessageFactory.Attachment(finalCard);
                await stepContext.Context.SendActivitiesAsync(new[] {response}, cancellationToken);
                return await stepContext.EndDialogAsync(response, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}