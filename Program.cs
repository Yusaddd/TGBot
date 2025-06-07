using System;
using static System.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MySqlConnector;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using static ConsoleApp25.Buttons;

namespace ConsoleApp25
{
    internal class Program
    {
        private const string text1 = "Есть аккаунт"; private const string text2 = "Нет аккаунта";
        private static readonly Dictionary<long, bool> _sentWelcomeMessages = new Dictionary<long, bool>();
        static void Main(string[] args)
        {
            #region подключение к бд
            string connectionString;
            connectionString = @"Server=127.0.0.1;Database=tg_bot_for_clinic;port=3306;User id=root;PASSWORD=Yu20sad21!";
            MySqlConnection cnn = new MySqlConnection(connectionString);
            try
            {
                cnn.Open();
                WriteLine("Подключение к базе данных успешно!");
            }
            catch (Exception ex)
            {
                WriteLine($"Ошибка при подключении к базе данных: {ex.Message}");
            }
            #endregion

            var client = new TelegramBotClient("7603978903:AAEAsYC76L0oIJv6UrEWAc8OQYCDh8GTN0A");
            client.StartReceiving(Update, Error);

            ReadLine();
        }

        private static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;

            // Проверяем, отправил ли бот приветствие этому пользователю ранее
            if (!_sentWelcomeMessages.ContainsKey(message.Chat.Id))
            {
                await SendWelcomeMessage(client, message); // Отправляем приветствие
                _sentWelcomeMessages.Add(message.Chat.Id, true); // Отмечаем, что приветствие отправлено
            }

            // Обработка остальных команд
            if (message.Text != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                switch (message.Text.ToLower())
                {
                    case "здравствуйте":
                        await client.SendMessage(message.Chat.Id, "Здравствуйте!");
                        break;
                    case "здрасьте":
                        await client.SendMessage(message.Chat.Id, "хуясьте");
                        break;

                    default:
                        // Другие команды обрабатываются аналогично...
                        break;
                }
            }

            //var text = update.Message.Text;
            //string imagePath = null;
            //switch (text)
            //{
            //    case text1:
            //        imagePath = Path.Combine(Environment.CurrentDirectory, "unnamed.jpg");
            //        await client.SendPhoto(message.Chat.Id, imagePath);
            //        await client.SendMessage(message.Chat.Id, "Отлично! Давайте проведём вход в аккаунт.\nВведите ваш логин");

            //        await client.SendMessage(message.Chat.Id, "Введите ваш пароль");

            //        break;
            //    case text2:
            //        imagePath = Path.Combine(Environment.CurrentDirectory, "unnamed.jpg");
            //        await client.SendPhoto(message.Chat.Id, imagePath);
            //        await client.SendMessage(message.Chat.Id, "Супер! Давайте проведём регистрацию.\nВведите ваш логин");

            //        await client.SendMessage(message.Chat.Id, "Введите ваш пароль");

            //        break;
            //}

            //switch (update.Type)
            //{
            //    case Telegram.Bot.Types.Enums.UpdateType.Message:

            //break;
            //}

            //// обработка фото, пока отвергает
            //if (message.Photo != null)
            //{
            //    await client.SendMessage(message.Chat.Id, "Отправь документом");
            //    return;
            //}

            //// скачивание и отправка обратно документа
            //if (message.Document != null)
            //{
            //    await client.SendMessage(message.Chat.Id, "Скачиваю документ");
            //    var fileID = update.Message.Document.FileId;
            //    var fileInfo = await client.GetFile(fileID);
            //    var filePath = fileInfo.FilePath;

            //    string destinationFilePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{message.Document.FileName}";
            //    await using FileStream fileStream = System.IO.File.OpenWrite(filePath);
            //    await client.DownloadFile(filePath, fileStream);
            //    fileStream.Close();

            //    //// тут ошибка, хотя делал всё по видео, хз почему так
            //    //await using Stream stream = System.IO.File.OpenRead(destinationFilePath);
            //    //InputFile document = new(stream, message.Document.FileName.Replace(".jpg", " (edited).jpg"));
            //    ////await client.SendDocument(message.Chat.Id, InputStreamFile, caption: "edited file", parseMode: ParseMode.Html);
            //    //await client.SendDocument(message.Chat.Id, document);
            //    return;
            //}
        }
        private static async Task SendWelcomeMessage(ITelegramBotClient client, Message message)
        {
            await client.SendMessage(
                chatId: message.Chat.Id,
                text: $"Здравствуйте! Я — Бот, который может:\n\n" +
                      "--Помочь с выбором клиники\n" +
                      "--Записать на прием к врачу\n" +
                      "--Задать вопрос напрямую врачу и получить от него ответ\n" +
                      "--Делиться свежими новостями из мира медицины\n\n" +
                      "Вы уже зарегистрированы?",
                replyMarkup: GetTwoButtons(text1, text2));
        }


        private static Task Error(ITelegramBotClient client, Exception err, CancellationToken ctoken)
        {
            Console.WriteLine(err.Message);
            return Task.CompletedTask;
        }
    }
}
