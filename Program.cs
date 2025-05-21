using System;
using static System.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ConsoleApp25
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //WriteLine("Hello, World!");
            var client = new TelegramBotClient("7761444018:AAEHvj1YolegUkZiBA6S7DeMFOlSeMeJaL8");
            client.StartReceiving(Update, Error);

            ReadLine();
        }

        private static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;

            // обработка сообщений
            if (message != null)
            {
                if (message.Text.ToLower().Contains("здарова"))
                {
                    WriteLine($"{message.Chat.Username ?? "unknown"} | {message.Text}");
                    await client.SendMessage(message.Chat.Id, "здоровей видали");
                    return;
                }
            }
            
            // обработка фото, пока отвергает
            if (message.Photo != null)
            {
                await client.SendMessage(message.Chat.Id, "Отправь документом");
                return;
            }

            // скачивание и отправка обратно документа
            if (message.Document != null)
            {
                await client.SendMessage(message.Chat.Id, "Скачиваю документ");
                var fileID = update.Message.Document.FileId;
                var fileInfo = await client.GetFile(fileID);
                var filePath = fileInfo.FilePath;

                string destinationFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{message.Document.FileName}";
                await using FileStream fileStream = System.IO.File.OpenWrite(filePath);
                await client.DownloadFile(filePath, fileStream);
                fileStream.Close();

                //// тут ошибка, хотя делал всё по видео, хз почему так
                //await using Stream stream = System.IO.File.OpenRead(destinationFilePath);
                //InputFile document = new(stream, message.Document.FileName.Replace(".jpg", " (edited).jpg"));
                ////await client.SendDocument(message.Chat.Id, InputStreamFile, caption: "edited file", parseMode: ParseMode.Html);
                //await client.SendDocument(message.Chat.Id, document);
                return;
            }
        }

        private static Task Error(ITelegramBotClient client, Exception err, CancellationToken ctoken)
        {
            throw new NotImplementedException();
        }
    }
}
