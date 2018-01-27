using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;


namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;
        bool in_game = false;
        int l;
        int h;
        int num = 0;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            

            message.Text = message.Text.Replace(" ", "+");

            if (message.Text.Length == 4 && (message.Text.Substring(0, 4).ToLower() == "game"))
            {
                Random random = new Random();
                in_game = true;
                
                l = 1;
                h = 100;
                num = random.Next(l, h + 1);
                
            	await context.PostAsync($"{this.count++}: I know a game! Let's play guess a number: I'm thinking of a number between " + l + " and " +
                    h + ". Take a guess!");
                context.Wait(MessageReceivedAsync);
            }
            else if (in_game)
            {
                int guess;
                if (Int32.TryParse(message.Text, out guess))
                {
                    if (guess < num)
                    {
                        await context.PostAsync($"{this.count++}: Guess is too low.");
                        context.Wait(MessageReceivedAsync);
                    }
                    else if (guess > num)
                    {
                        await context.PostAsync($"{this.count++}: Guess is too high.");
                        context.Wait(MessageReceivedAsync);
                    }
                    else
                    {
                        in_game = false;
                        await context.PostAsync($"{this.count++}: You win!.");
                        context.Wait(MessageReceivedAsync);
                    }
                }
                else if (message.Text.Length > 9 && (message.Text.Substring(0, 9).ToLower() == "quit+game"))
                {
                    in_game = false;
                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    await context.PostAsync($"{this.count++}: Please enter a guess or \"Quit Game\" to quit.");
                    context.Wait(MessageReceivedAsync);
                }
            }
            else if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            else if (message.Text.Length > 7 && (message.Text.Substring(0, 6).ToLower() == "google" || message.Text.Substring(0, 6).ToLower() == "search"))
            {
                await context.PostAsync($"{this.count++}: http://lmgtfy.com/?q={message.Text.Substring(7)}");
                context.Wait(MessageReceivedAsync);
            }
            else if (message.Text.Length > 5 && (message.Text.Substring(0, 4).ToLower() == "bing"))
            {
                await context.PostAsync($"{this.count++}: http://lmgtfy.com/?s=b&q={message.Text.Substring(5)}");
                context.Wait(MessageReceivedAsync);
            }
            else if (message.Text.Length > 6 && (message.Text.Substring(0, 5).ToLower() == "yahoo"))
            {
                await context.PostAsync($"{this.count++}: http://lmgtfy.com/?s=y&q={message.Text.Substring(6)}");
                context.Wait(MessageReceivedAsync);
            }
            else if (message.Text.Length > 5 && (message.Text.Substring(0, 4).ToLower() == "duck"))
            {
                await context.PostAsync($"{this.count++}: http://lmgtfy.com/?s=d&q={message.Text.Substring(5)}");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                await context.PostAsync($"{this.count++}: Search using \"Search\" or \"Google\" or \"Bing\" or \"Yahoo\" or \"Duck\".");
                context.Wait(MessageReceivedAsync);
            }
            
            
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}