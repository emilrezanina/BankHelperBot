// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with CoreBot .NET Template version v4.6.2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankHelperBot.Cards;
using BankHelperBot.Details;
using BankHelperBot.Recognizers;
using BankHelperBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace BankHelperBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly IIsConfiguredRecognizer _recognizer;
        protected readonly ILogger Logger;
        private readonly ICardFactory _cardFactory;
        private readonly ILoanFormViewService _loanFormViewService;

        private const string CalculateStartText =
            "Ockay, we can start calculate your loan. I had some question for you.";

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(IIsConfiguredRecognizer recognizer, LoanCalculationDialog loanCalculationDialog,
            ILogger<MainDialog> logger, ICardFactory cardFactory, ILoanFormViewService loanFormViewService)
            : base(nameof(MainDialog))
        {
            _recognizer = recognizer;
            Logger = logger;
            _cardFactory = cardFactory;
            _loanFormViewService = loanFormViewService;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(loanCalculationDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (!_recognizer.IsConfigured)
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }

            var messageText = stepContext.Options?.ToString() ??
                              $"Hello {stepContext.Context.Activity.From.Name},\n what can I help you with today?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions {Prompt = promptMessage},
                cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (IsGreetingsContext(stepContext))
            {
                return await GreetingsActStepAsync(stepContext, cancellationToken);
            }

            if (_recognizer.IsConfigured)
            {
                return await LuisActStepAsync(stepContext, cancellationToken);
            }

            return await SimpleActStepAsync(stepContext, cancellationToken);
        }

        private static bool IsGreetingsContext(WaterfallStepContext stepContext)
        {
            return stepContext.Context.Activity.Text.Contains("Hi");
        }

        private static async Task<DialogTurnResult> GreetingsActStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var messageText = stepContext.Options?.ToString() ??
                              $"Hello {stepContext.Context.Activity.From.Name},\n what can I help you with today?";
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text(messageText, inputHint: InputHints.IgnoringInput), cancellationToken);
            return new DialogTurnResult(DialogTurnStatus.Empty);
        }

        private async Task<DialogTurnResult> LuisActStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var luisResult = await _recognizer.RecognizeAsync<BankHelper>(stepContext.Context, cancellationToken);
            switch (luisResult.TopIntent().intent)
            {
                case BankHelper.Intent.CalculateLoan:
                {
                    return await StartCalculateLoanDialog(stepContext, cancellationToken);
                }
                case BankHelper.Intent.ShowLoan:
                {
                    var loanFormDetail = _loanFormViewService.GetLoanFormDetail(luisResult.LoanId);
                    if (loanFormDetail == null)
                    {
                        var didntFindLoan = $"Sorry, a loan with {luisResult.LoanId.ToString()} was not found.";
                        var message = MessageFactory.Text(didntFindLoan);
                        await stepContext.Context.SendActivityAsync(message, cancellationToken);
                    }
                    else
                    {
                        var loanFormCard = _cardFactory.CreateLoanFormCardAttachment(loanFormDetail);
                        var response = MessageFactory.Attachment(loanFormCard);
                        await stepContext.Context.SendActivitiesAsync(new[] {response}, cancellationToken);  
                    }
                    break;
                }
                case BankHelper.Intent.ShowContact:
                {
                    var contactCard = _cardFactory.CreateContactCard(null);
                    var response = MessageFactory.Attachment(contactCard);
                    await stepContext.Context.SendActivitiesAsync(new[] {response}, cancellationToken);
                    break;
                }
                default:
                {
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText =
                        $"Sorry, I didn't get that. Please try asking in a different way (intent was {luisResult.TopIntent().intent})";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText,
                        didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
                }
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private static async Task<DialogTurnResult> SimpleActStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Text.Contains("calculate"))
            {
                return await StartCalculateLoanDialog(stepContext, cancellationToken);
            }

            string text = stepContext.Options as string ?? "I do not understand your question.";
            var promptMessage = MessageFactory.Text(text, text, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions() {Prompt = promptMessage},
                cancellationToken);
        }

        private static async Task<DialogTurnResult> StartCalculateLoanDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text(CalculateStartText, inputHint: InputHints.IgnoringInput),
                cancellationToken);
            return await stepContext.BeginDialogAsync(nameof(LoanCalculationDialog), new LoanFormDetails() { Id = LoanFormDetails.IdCounter },
                cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (stepContext.Result is LoanFormDetails result)
            {
                var messageText = "Unfortunately there is no loan offer for you.";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            // Restart the main dialog with a different message the second time around
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}