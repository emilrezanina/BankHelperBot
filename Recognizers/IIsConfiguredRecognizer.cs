using Microsoft.Bot.Builder;

namespace BankHelperBot.Recognizers
{
    public interface IIsConfiguredRecognizer : IRecognizer
    {
        bool IsConfigured { get; }
    }
}