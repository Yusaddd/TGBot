using System;
using System.Collections.Generic;
using static System.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using MySqlConnector;
using static ConsoleApp25.Buttons;

namespace ConsoleApp25
{
    internal class Program
    {
        #region текстовые константы
        // конст для регистрации
        private const string reg_text1 = "Есть аккаунт"; private const string reg_text2 = "Нет аккаунта";

        // конст для вводного сообщения
        private const string text1 = "Выбрать клинику"; private const string text2 = "Информация о клинике";
        private const string text3 = "Записаться на приём"; private const string text4 = "Отменить приём";
        private const string text5 = "Просмотр докторов клиники"; private const string text6 = "Просмотр новостей клиники";
        private const string text7 = "Справочная служба"; private const string text8 = "FAQ";
        private const string text9 = "Информация о пользователе";
        #endregion

        private static readonly Dictionary<long, bool> _sentWelcomeMessages = new Dictionary<long, bool>();
        static void Main(string[] args) // Начало работы, ввод токена, подключение к бд
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
            // Регистрация команд
            client.SetMyCommandsAsync(new List<BotCommand>
            {
                new BotCommand { Command = "/start", Description = "Начать общение с ботом" },
                new BotCommand { Command = "/appointment", Description = "Записать на приём" },
                new BotCommand { Command = "/cancel", Description = "Отменить запись на приём" },
                new BotCommand { Command = "/clinics", Description = "Список клиник" },
                new BotCommand { Command = "/info", Description = "Информация о клинике" },
                new BotCommand { Command = "/doctors", Description = "Список докторов" },
                new BotCommand { Command = "/faq", Description = "Часто задаваемые вопросы" },
                new BotCommand { Command = "/help", Description = "Справочная служба" },
                new BotCommand { Command = "/news", Description = "Новости клиники" },
                new BotCommand { Command = "/user", Description = "Информация о пользователе" }
            });
            client.StartReceiving(Update, Error);

            ReadLine();
        }
        private static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;

            // Проверяем, отправил ли бот приветствие этому пользователю ранее
            if (!_sentWelcomeMessages.ContainsKey(message.Chat.Id)) // Если нет, то...
            {
                await SendWelcomeMessage(client, message); // Отправляем приветствие
                _sentWelcomeMessages.Add(message.Chat.Id, true); // Отмечаем, что приветствие отправлено
            }
            // Обработка сообщений и команд
            if (message.Text != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                // Если пользователь вводит команду
                if (message.Text.ToLower().Contains("/"))
                {
                    switch (message.Text.ToLower())
                        {
                            case "/appointment":
                                await AppointmentCommand(client, message);
                                break;
                            case "/cancel":
                                await CancelCommand(client, message);
                                break;
                            case "/clinics":
                                await ClinicsCommand(client, message);
                                break;
                            case "/info":
                                await InfoCommand(client, message);
                                break;
                            case "/doctors":
                                await DoctorsCommand(client, message);
                                break;
                            case "/faq":
                                await FaqCommand(client, message);
                                break;
                            case "/help":
                                await HelpCommand(client, message);
                                break;
                            case "/news":
                                await NewsCommand(client, message);
                                break;
                            case "/user":
                                await UserCommand(client, message);
                                break;

                            default:
                                if (!_sentWelcomeMessages.ContainsKey(message.Chat.Id))
                                    await client.SendTextMessageAsync(message.Chat.Id, "Мы получили ваше сообщение, отправьте ещё!");
                                break;
                        }
                }
                // Если пользователь вводит приветствие
                else if (message.Text.ToLower().Contains("здравствуйте") || message.Text.ToLower().Contains("приветствую") || message.Text.ToLower().Contains("салам алейкум")
                     || message.Text.ToLower().Contains("доброе утро") || message.Text.ToLower().Contains("добрый день") || message.Text.ToLower().Contains("добрый вечер")
                     || message.Text.ToLower().Contains("у меня есть вопрос"))
                {
                    string imagePath = Path.Combine(Environment.CurrentDirectory, "котик.jpg");
                    using (var stream = System.IO.File.OpenRead(imagePath)) 
                    {
                        var result = client.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(stream), caption: "йоу").Result; 
                    }
                    await client.SendTextMessageAsync(message.Chat.Id, "Здравствуйте! Могу помочь вам в следующих услугах:",
                        replyMarkup: GetNineButtons(text1, text2, text3, text4, text5, text6, text7, text8, text9));
                }
                // Если пользователь вводит что-то иное
                else await client.SendTextMessageAsync(message.Chat.Id, "Простите, я не знаю, что вам на это ответить, я умею общаться только на темы, " +
                    "связанные с клиниками, докторами и прочим.\nПожалуйста, выберите, что вас интересует:",
                    replyMarkup: GetNineButtons(text1, text2, text3, text4, text5, text6, text7, text8, text9));
            }
            else
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Пожалуйста, введите не пустое сообщение!");
            }

            //if (update.CallbackQuery != null)
            //{
            //    await HandleCallbackQuery(client, update.CallbackQuery);
            //}
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
        //private static async Task HandleCallbackQuery(ITelegramBotClient client, CallbackQuery callbackQuery)
        //{
        //    if (callbackQuery.Data == "Есть аккаунт")
        //    {
        //        await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "круто");
        //    }
        //    if (callbackQuery.Data == "Нет аккаунта")
        //    {
        //        await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "пфф лох");
        //    }
        //}
        #region команды инлайн клавиатуры
        private static async Task AppointmentCommand(ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Можно записаться на приём в следующие даты:");
            // Добавьте здесь логику для обработки команды /appointment
        }
        private static async Task CancelCommand(ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Выберите запланированный приём на отмену записи:");
            // Добавьте здесь логику для обработки команды /cancel
        }
        private static async Task ClinicsCommand(ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Вот список доступных клиник:");
            // Добавьте здесь логику для обработки команды /clinics
        }
        private static async Task InfoCommand(ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Информация о клинике:");
            // Добавьте здесь логику для обработки команды /info
        }
        private static async Task DoctorsCommand(ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Информация о докторах в данной клинике:");
            // Добавьте здесь логику для обработки команды /doctors
        }
        private static async Task FaqCommand(ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Вот ответы на часто задаваемые вопросы:");
            // Добавьте здесь логику для обработки команды /faq
        }
        private static async Task HelpCommand(ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Справочная служба:");
            // Добавьте здесь логику для обработки команды /help
        }
        private static async Task NewsCommand(ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Новости клиники:");
            // Добавьте здесь логику для обработки команды /news
        }
        private static async Task UserCommand(ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Ваша информация:");
            // Добавьте здесь логику для обработки команды /user
        }
        #endregion
        private static async Task SendWelcomeMessage(ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Здравствуйте! Я — Бот, который может:\n\n" +
                      "--Помочь с выбором клиники\n" +
                      "--Записать на прием к врачу\n" +
                      "--Задать вопрос напрямую врачу и получить от него ответ\n" +
                      "--Делиться свежими новостями из мира медицины\n\n" +
                      "Вы уже зарегистрированы?",
                replyMarkup: GetTwoButtons(reg_text1, reg_text2));
        }

        //private static async Task SendImageToChat(ITelegramBotClient botClient, long chatId)
        //{
        //    // Путь к вашему файлу изображения
        //    string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "unnamed.jpg");

        //    // Получение потока файла
        //    using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
        //    {
        //        // Создание InputMediaPhoto объекта с потоком
        //        InputMediaPhoto media = new(fs);

        //        // Отправляем фото пользователю
        //        await botClient.SendPhotoAsync(chatId, media);
        //    }
        //}

        private static Task Error(ITelegramBotClient client, Exception err, CancellationToken ctoken)
        {
            WriteLine($"Ошибка: {err.Message}");
            return Task.CompletedTask;
        }
    }
}
