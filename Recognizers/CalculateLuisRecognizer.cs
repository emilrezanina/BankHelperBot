using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;

namespace BankHelperBot.Recognizers
{
    public sealed class CalculateLuisRecognizer : IIsConfiguredRecognizer
    {
        private readonly LuisRecognizer _recognizer;

        public CalculateLuisRecognizer(IConfiguration configuration)
        {
            var luisIsConfigured = !string.IsNullOrEmpty(configuration["LuisAppId"]) &&
                                   !string.IsNullOrEmpty(configuration["LuisAPIKey"]) &&
                                   !string.IsNullOrEmpty(configuration["LuisAPIHostName"]);
            if (!luisIsConfigured) return;
            var luisApplication = new LuisApplication(
                configuration["LuisAppId"],
                configuration["LuisAPIKey"],
                "https://" + configuration["LuisAPIHostName"]);
            _recognizer = new LuisRecognizer(luisApplication);
        }

        public bool IsConfigured => _recognizer != null;

        public async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext,
            CancellationToken cancellationToken)
            => await _recognizer.RecognizeAsync(turnContext, cancellationToken);

        public async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken)
            where T : IRecognizerConvert, new()
            => await _recognizer.RecognizeAsync<T>(turnContext, cancellationToken);
    }
}