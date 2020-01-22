using System.Collections.Generic;
using System.Drawing;
using System.IO;
using AdaptiveCards;
using BankHelperBot.Details;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankHelperBot.Cards
{
    public class CardFactory : ICardFactory
    {
        public Attachment CreateWelcomeCard()
        {
            const string cardResourcePath = "BankHelperBot.Cards.welcomeCard.json";

            using var stream = GetType().Assembly.GetManifestResourceStream(cardResourcePath);
            using var reader = new StreamReader(stream);
            var adaptiveCard = reader.ReadToEnd();
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject(adaptiveCard),
            };
        }

        public Attachment CreateContactCard(LoanFormDetails loanFormDetails)
        {
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveColumnSet()
                    {
                        Columns = new List<AdaptiveColumn>()
                        {
                            new AdaptiveColumn()
                            {
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveImage()
                                    {
                                        Style = AdaptiveImageStyle.Person,
                                        UrlString = "http://resources.rezanina.eu/peter_polama.png",
                                        Size = AdaptiveImageSize.Medium
                                    }
                                },
                                Width = "auto"
                            },
                            new AdaptiveColumn()
                            {
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock()
                                    {
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Text = "Peter Polama",
                                        Wrap = true
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Text = "Senior Consultant",
                                        Italic = true
                                    },
                                    new AdaptiveColumnSet()
                                    {
                                        Columns = new List<AdaptiveColumn>()
                                        {
                                            new AdaptiveColumn()
                                            {
                                                Items = new List<AdaptiveElement>()
                                                {
                                                    new AdaptiveImage("http://resources.rezanina.eu/phone_small.png")
                                                    {
                                                        Size = AdaptiveImageSize.Small
                                                    }
                                                }
                                            },
                                            new AdaptiveColumn()
                                            {
                                                Items = new List<AdaptiveElement>()
                                                {
                                                    new AdaptiveTextBlock()
                                                    {
                                                        Color = AdaptiveTextColor.Attention,
                                                        Text = "(+420) 987 987 987",
                                                        Spacing = AdaptiveSpacing.Medium,
                                                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Actions = new List<AdaptiveAction>()
                {
                    new AdaptiveOpenUrlAction()
                    {
                        UrlString = "https://www.quadient.com",
                        Title = "Show form"
                    },
                    new AdaptiveSubmitAction()
                    {
                        Title = "Contact support",
                        DataJson = "{\"email\":true}"
                    }
                }
            };
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JObject.FromObject(card),
            };
        }

        public Attachment CreateLoanFormCardAttachment(LoanFormDetails loanFormDetails)
        {
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveImage()
                    {
                        UrlString = "http://resources.rezanina.eu/BankHelperBot.PNG",
                        Size = AdaptiveImageSize.Auto,
                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                        BackgroundColor = "white"
                    },
                    new AdaptiveTextBlock()
                    {
                        Spacing = AdaptiveSpacing.Medium,
                        Size = AdaptiveTextSize.Medium,
                        Weight = AdaptiveTextWeight.Bolder,
                        Text = "Please confirm your personal information:",
                        Wrap = true,
                        MaxLines = 0
                    },
                    new AdaptiveColumnSet()
                    {
                        Columns = new List<AdaptiveColumn>()
                        {
                            new AdaptiveColumn()
                            {
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Size = AdaptiveTextSize.Small,
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Text = "Loan Id:",
                                    }, 
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Size = AdaptiveTextSize.Small,
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Text = "Marital status:",
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Size = AdaptiveTextSize.Small,
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Text = "Children count:",
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Size = AdaptiveTextSize.Small,
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Text = "Employed:",
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Size = AdaptiveTextSize.Small,
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Text = "Month income:",
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Size = AdaptiveTextSize.Small,
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Text = "Debt:",
                                        Separator = true
                                    }
                                }
                            },
                            new AdaptiveColumn()
                            {
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Size = AdaptiveTextSize.Small,
                                        Weight = AdaptiveTextWeight.Default,
                                        Text = loanFormDetails.Id.ToString()    
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Size = AdaptiveTextSize.Small,
                                        Weight = AdaptiveTextWeight.Default,
                                        Text = loanFormDetails.MaritalStatus,
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Size = AdaptiveTextSize.Small,
                                        Weight = AdaptiveTextWeight.Default,
                                        Text = loanFormDetails.ChildCount.ToString(),
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Size = AdaptiveTextSize.Small,
                                        Weight = AdaptiveTextWeight.Default,
                                        Text = loanFormDetails.WorkStatus.ToString(),
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Size = AdaptiveTextSize.Small,
                                        Weight = AdaptiveTextWeight.Default,
                                        Text = loanFormDetails.Income.ToString(),
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Spacing = AdaptiveSpacing.Small,
                                        Size = AdaptiveTextSize.Small,
                                        Weight = AdaptiveTextWeight.Default,
                                        Text = loanFormDetails.Debt.ToString(),
                                    },
                                }
                            }
                        },
                        Separator = true
                    },
                    new AdaptiveTextBlock()
                    {
                        Spacing = AdaptiveSpacing.Small,
                        Size = AdaptiveTextSize.Default,
                        Weight = AdaptiveTextWeight.Bolder,
                        Text = "You are reached a loan!",
                        Separator = true
                    },
                    new AdaptiveTextBlock()
                    {
                        Spacing = AdaptiveSpacing.Small,
                        Size = AdaptiveTextSize.Default,
                        Text = $"You can borrow up to {(loanFormDetails.Income * 10).ToString()}",
                        Wrap = true
                    }
                }
            };
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JObject.FromObject(card)
            };
        }
    }
}