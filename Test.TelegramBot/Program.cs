using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Test.TelegramBot
{
    public class Program
    {
        private readonly static string token = "7160117001:AAElSx3Do2WUW1M6j2PaR7vOnkZlOYiMPho";
        private static TelegramBotClient _client;
        static async Task Main(string[] args)
        {
            _client = new TelegramBotClient(token);
            var apiIntegration = new ApiIntegrationWithBot();
            var getAllProducts = await apiIntegration.GetValuesAsync();

            using CancellationTokenSource tokenSource = new();
            var receiverOptions = new ReceiverOptions();

            _client.StartReceiving(
                updateHandler: UpdateHanddler,
                pollingErrorHandler: ErrorHandler,
                receiverOptions: receiverOptions,
                cancellationToken: tokenSource.Token);

            Console.ReadKey();
            tokenSource.Cancel();

            async Task GenerateInlineKeyboardButton(long chatId, CancellationToken token)
            {
                var sortNumbers = getAllProducts.Select(p => (p.id, p.sortNumber));

                var buttons = sortNumbers.Select(item =>
                    InlineKeyboardButton.WithCallbackData(item.sortNumber.ToString(), $"{item.id}")
                ).ToArray();

                var inlineKeyboard = new InlineKeyboardMarkup(
                    Enumerable.Range(0, (int)Math.Ceiling((double)buttons.Length / 2))
                        .Select(i => buttons.Skip(i * 2).Take(2).ToArray())
                );

                await _client.SendTextMessageAsync(
                    chatId,
                    "Choose a sort number:",
                    replyMarkup: inlineKeyboard,
                    cancellationToken: token
                );
            }



            async Task UpdateHanddler(ITelegramBotClient client, Update update, CancellationToken token)
            {
                if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
                {
                    var chatId = update.Message.Chat.Id;
                    var text = update.Message.Text;

                    if (text == "/start")
                    {
                        await GenerateInlineKeyboardButton(chatId, token);
                    }
                }

                if (update.CallbackQuery != null)
                {
                    if (getAllProducts.Any(p => p.id.ToString() == update.CallbackQuery.Data))
                    {
                        (string videoData, string videoFileName) = await apiIntegration.GetVideoDatasOfProduct(long.Parse(update.CallbackQuery.Data!));
                        Stream videoStream = apiIntegration.GetStreamAsync(videoData);

                        Message message = await _client.SendVideoAsync(
                        chatId: update.CallbackQuery.Message!.Chat.Id,
                        video: InputFile.FromStream(videoStream, $"{videoFileName}"),
                        supportsStreaming: true,
                        cancellationToken: token);
                    }

                }
            }

            Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }
            Console.ReadKey();
        }




    }
}