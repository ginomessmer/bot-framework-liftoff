using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace BotFrameworkLiftoff.Web.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private int count;

        public Task StartAsync(IDialogContext context)
        {
            this.count = 0;
            context.SayAsync("Why hello there!");
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            activity.Text = activity.Text ?? string.Empty;

            var command = activity.Text.ToLowerInvariant();

            // check if the user said reset
            if (command.StartsWith("/"))
            {
                switch (command)
                {
                    case "/start":
                        await ReplyWithStartMessageAsync(context);
                        break;
                    case "/cards":
                        await ReplyWithCardsAsync(context);
                        break;
                    case "/gif":
                        await ReplyWithGifAsync(context);
                        break;
                    default:
                        await context.SayAsync("Sorry, I didn't catched this. Check out the commands for available methods.");
                        break;
                }
            }
            else
            {
                // calculate something for us to return
                int length = activity.Text.Length;

                // increment the counter
                this.count++;

                // say reply to the user
                await context.SayAsync($"{count}: You sent {activity.Text} which was {length} characters", $"{count}: You said {activity.Text}", new MessageOptions() { InputHint = InputHints.AcceptingInput });
                context.Wait(MessageReceivedAsync);
            }

        }

        private async Task ReplyWithGifAsync(IDialogContext context)
        {
            var gifCard = new AnimationCard("We all do love cats", "Subtitle about cats belongs here", "Write something about cats",
                new ThumbnailUrl("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTZqgfFQmgPpaogLWfDzWIgiVLAHPvGFi2VMrBhnrnWhtd4JCYAfg"), new List<MediaUrl>() { new MediaUrl("https://media.giphy.com/media/ziYUNUTpi4SiI/giphy.gif") });

            var message = context.MakeMessage();
            message.Attachments.Add(gifCard.ToAttachment());
            await context.PostAsync(message);

            context.Wait(MessageReceivedAsync);
        }

        private async Task ReplyWithStartMessageAsync(IDialogContext context)
        {
            await context.SayAsync("Why hello there!");
            context.Wait(MessageReceivedAsync);
        }

        private async Task ReplyWithCardsAsync(IDialogContext context)
        {
            var card = new HeroCard("Hero Card", "I'm a fancy hero card", "Insert some text here to represent the card", 
                new List<CardImage>()
                {
                    new CardImage("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTZqgfFQmgPpaogLWfDzWIgiVLAHPvGFi2VMrBhnrnWhtd4JCYAfg")
                }// , 
                //new List<CardAction>()
                //{
                //    new CardAction("button", "Action 1"),
                //    new CardAction("button", "Action 2")
                //}
            );

            var message = context.MakeMessage();
            message.Attachments.Add(card.ToAttachment());
            await context.PostAsync(message);

            context.Wait(MessageReceivedAsync);
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            // check if user wants to reset the counter or not
            if (confirm)
            {
                this.count = 1;
                await context.SayAsync("Reset count.", "I reset the counter for you!");
            }
            else
            {
                await context.SayAsync("Did not reset count.", $"Counter is not reset. Current value: {this.count}");
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}