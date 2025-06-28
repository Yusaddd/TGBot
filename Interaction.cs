using static System.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using MySqlConnector;
using static ConsoleApp25.Buttons;
using Telegram.Bot.Types.Enums;

namespace ConsoleApp25
{
    internal class Interaction
    {
        #region для работы ф-ий, обход циклов с помощью переменных
        private static int IsAppointOrWrite { get; set; } // 1 -- appointment, 2 -- write, 3 -- отмена записи; нужно для ф-ий назначения записи и для написания врачу

        private static int Step { get; set; } // шаги выполнения команды
        private static string Doctor { get; set; } // сохранение врача
        private static string Info { get; set; } // сохранение имени, фамилии и номера телефона пользователя
        private static DateTime Date { get; set; } // сохранение даты проведения встречи
        private static int newsId = 1; // сохранение id новости
        #endregion

        #region текстовые константы
        // конст для подключения к бд
        static string connectionString = @"Server=127.0.0.1;Database=medical_db;port=3306;User id=root;PASSWORD=Yu20sad21!";

        // конст для вводного сообщения
        private const string text1 = "Записаться на приём"; private const string text2 = "Отменить приём";
        private const string text3 = "Информация о клинике"; private const string text4 = "Просмотр докторов клиники"; 
        private const string text5 = "Просмотр новостей клиники"; private const string text6 = "Написать лечащему врачу";
        private const string text7 = "Справочная служба";

        // первое сообщение
        private const string hello_text = "Клиника \"Мед-вышка\" – современный многопрофильный медицинский центр," +
            " предоставляющий широкий спектр амбулаторных услуг: консультации врачей различных специальностей" +
            " (терапевты, педиатры, узкие специалисты), диагностику (УЗИ, ЭКГ, лабораторные анализы)," +
            " физиотерапию и косметологические процедуры. Клиника ориентирована на комфорт пациентов" +
            " и оперативное обслуживание, стремится минимизировать время ожидания и бумажную волокиту.";

        // конст для врачей (написать)
        private const string textAS = "Алексей Сергеев"; private const string textOI = "Ольга Иванова"; private const string textIP = "Ирина Петрова";

        // Дата и время на запись
        private const string textDay1 = "ПН 12:00"; private const string textDay2 = "ПН 14:00"; private const string textDay3 = "ПН 16:00";
        private const string textDay4 = "ПН 18:00"; private const string textDay5 = "ПН 20:00"; private const string textDay6 = "ВТ 12:00";
        private const string textDay7 = "ВТ 14:00"; private const string textDay8 = "ВТ 16:00"; private const string textDay9 = "ВТ 18:00";
        private const string textDay10 = "ВТ 20:00"; private const string textDay11 = "СР 12:00"; private const string textDay12 = "СР 14:00";
        private const string textDay13 = "СР 16:00"; private const string textDay14 = "СР 18:00"; private const string textDay15 = "СР 20:00";
        private const string textDay16 = "ЧТ 12:00"; private const string textDay17 = "ЧТ 14:00"; private const string textDay18 = "ЧТ 16:00";
        private const string textDay19 = "ЧТ 18:00"; private const string textDay20 = "ЧТ 20:00"; private const string textDay21 = "ПТ 12:00";
        private const string textDay22 = "ПТ 14:00"; private const string textDay23 = "ПТ 16:00"; private const string textDay24 = "ПТ 18:00";
        private const string textDay25 = "ПТ 20:00"; private const string textDay26 = "СБ 12:00"; private const string textDay27 = "СБ 14:00";
        private const string textDay28 = "СБ 16:00"; private const string textDay29 = "СБ 18:00"; private const string textDay30 = "СБ 20:00";
        private const string textDay31 = "ВС 12:00"; private const string textDay32 = "ВС 14:00"; private const string textDay33 = "ВС 16:00";
        private const string textDay34 = "ВС 18:00"; private const string textDay35 = "ВС 20:00";
        private static readonly string[] dateArray = new[]
            {
            textDay1, textDay2, textDay3, textDay4, textDay5,
            textDay6, textDay7, textDay8, textDay9, textDay10,
            textDay11, textDay12, textDay13, textDay14, textDay15,
            textDay16, textDay17, textDay18, textDay19, textDay20,
            textDay21, textDay22, textDay23, textDay24, textDay25,
            textDay26, textDay27, textDay28, textDay29, textDay30,
            textDay31, textDay32, textDay33, textDay34, textDay35
            };

        // конст Назад
        private const string textBack = "Назад";
        #endregion

        #region для первого сообщения
        private static async Task SendWelcomeMessage(ITelegramBotClient client, Message message)
        {
            string imagePath = Path.Combine(Environment.CurrentDirectory,$"первое сообщение.jpg");
            using (var stream = System.IO.File.OpenRead(imagePath))
            {
                var caption = hello_text;
                await client.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(stream), caption: caption,
                replyMarkup: GetSevenButtons(text1, text2, text3, text4, text5, text6, text7));
            }
        }
        private static readonly Dictionary<long, bool> _sentWelcomeMessages = new Dictionary<long, bool>();
        #endregion
        internal static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;

            // Проверяем, отправил ли бот приветствие этому пользователю ранее
            if (!_sentWelcomeMessages.ContainsKey(message?.Chat?.Id ?? 0)) // Если нет, то...
            {

                await SendWelcomeMessage(client, message); // Отправляем приветствие
                _sentWelcomeMessages.Add(message?.Chat?.Id ?? 0, true); // Отмечаем, что приветствие отправлено
            }

            // Обработка сообщений и команд
            if (message?.Text != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                // Если пользоавтель хочет назначить встречу
                if (IsAppointOrWrite == 1) // Признак того, что пользователь хочет назначить встречу
                {
                    // Иниц доктора, выбор даты
                    if (Step == 1)
                    {
                        if (message.Text.ToLower().Contains("алекс"))
                        {
                            Doctor = "Алексей Сергеев";
                            await client.SendTextMessageAsync(message.Chat.Id, "Отлично, ваш врач -- Алексей Сергеев!\n\nНа этой неделе можно встретиться с врачом в следующие дни:",
                                replyMarkup: GetTenButtons(textDay1, textDay4, textDay7, textDay10, textDay13, textDay16, textDay19, textDay22, textDay25, textBack));
                            Step = 2;
                        }
                        else if (message.Text.ToLower().Contains("ири"))
                        {
                            Doctor = "Ирина Петрова";
                            await client.SendTextMessageAsync(message.Chat.Id, "Отлично, ваш врач -- Ирина Петрова!\n\nНа этой неделе можно встретиться с врачом в следующие дни:",
                                replyMarkup: GetTenButtons(textDay2, textDay5, textDay8, textDay11, textDay14, textDay17, textDay20, textDay23, textDay26, textBack));
                            Step = 2;
                        }
                        else if (message.Text.ToLower().Contains("оль"))
                        {
                            Doctor = "Ольга Иванова";
                            await client.SendTextMessageAsync(message.Chat.Id, "Отлично, ваш врач -- Ольга Иванова!\n\nНа этой неделе можно встретиться с врачом в следующие дни:",
                                replyMarkup: GetTenButtons(textDay3, textDay6, textDay9, textDay12, textDay15, textDay18, textDay21, textDay24, textDay27, textBack));
                            Step = 2;
                        }
                        else if (!message.Text.ToLower().Contains("назад"))
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Введите доктора для записи", replyMarkup: GetFourButtons(textIP, textAS, textOI, textBack));
                        }
                        else
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Запись на приём отменена! Могу помочь вам в следующих услугах:",
                                replyMarkup: GetSevenButtons(text1, text2, text3, text4, text5, text6, text7));
                            Doctor = null;
                            IsAppointOrWrite = -1; // Меняем значение, чтобы выйти из условия
                            Step = 1; // возвращаемся на первый шаг
                        }
                    }
                    // Иниц даты, ввод инфо о пользователе
                    if (Step == 2)
                    {
                        if (message.Text.ToLower().Contains("назад"))
                        {
                            IsAppointOrWrite = -1; // Меняем значение, чтобы выйти из условия
                            Step = 1; // возвращаемся на первый шаг
                        }
                        else if (dateArray.Contains(message.Text))
                        {
                            Date = ParseDateTime(message.Text);

                            await client.SendTextMessageAsync(message.Chat.Id, $"Отлично, дата встречи -- {message.Text}!\n\nВведите, пожалуйста, Ваше имя, фамилию и номер телефона через пробел",
                                replyMarkup: GetOneButton(textBack));
                            Step = 3;
                        }
                        else
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Выберите дату, в которую врач сможет провести встречу");
                        }
                    }
                    // иниц инфо о пользователе, переход к вводу данных в бд
                    if (Step == 3)
                    {
                        var tmp = await client.GetUpdatesAsync();
                        var userInput = tmp.First().Message;

                        if (userInput.Text.ToLower().Contains("назад"))
                        {
                            Step = 2; // возвращаемся на второй шаг
                        }
                        else
                        {
                            var parts = userInput.Text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length != 3)
                            {
                                await client.SendTextMessageAsync(message.Chat.Id, "Введите верные данные в правильном порядке");
                            }
                            else
                            {
                                Info = userInput.Text;
                                Step = 4;
                            }
                        }
                    }
                    // Ввод данных в бд
                    if (Step == 4)
                    {
                        if (message.Text.ToLower().Contains("назад"))
                        {
                            Step = 3; // возвращаемся на первый шаг
                        }
                        else
                        {
                            // Разделение информации о пользователе
                            var parts = Info.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var firstName = parts[0];
                            var lastName = parts[1];
                            var phoneNumber = parts[2];

                            // Создание пользователя в таблице user
                            var userId = await CreateUser(firstName, lastName, phoneNumber);

                            // Создание встречи в таблице appointment
                            await CreateAppointment(userId, Doctor, Date);

                            await client.SendTextMessageAsync(message.Chat.Id, "Запись создана. Могу помочь в следующих услугах",
                                replyMarkup: GetSevenButtons(text1, text2, text3, text4, text5, text6, text7));
                            IsAppointOrWrite = -1; // Меняем значение, чтобы выйти из условия
                            Step = 1; // возвращаемся на первый шаг
                        }
                    }
                }

                // Если пользоавтель хочет написать врачу
                else if (IsAppointOrWrite == 2) // Признак того, что пользователь хочет написать врачу
                {
                    if (message.Text.ToLower().Contains("алекс"))
                    {
                        var email = await GetDoctorEmail("Алексей Сергеев");
                        await client.SendTextMessageAsync(message.Chat.Id, $"Предлагаю Вам напрямую связаться с врачом по рабочей почте\n{email}",
                            replyMarkup: GetSevenButtons(text1, text2, text3, text4, text5, text6, text7));
                    }
                    else if (message.Text.ToLower().Contains("ири"))
                    {
                        var email = await GetDoctorEmail("Ирина Петрова");
                        await client.SendTextMessageAsync(message.Chat.Id, $"Предлагаю Вам напрямую связаться с врачом по рабочей почте\n{email}",
                            replyMarkup: GetSevenButtons(text1, text2, text3, text4, text5, text6, text7));
                    }
                    else if (message.Text.ToLower().Contains("оль"))
                    {
                        var email = await GetDoctorEmail("Ольга Иванова");
                        await client.SendTextMessageAsync(message.Chat.Id, $"Предлагаю Вам напрямую связаться с врачом по рабочей почте\n{email}",
                            replyMarkup: GetSevenButtons(text1, text2, text3, text4, text5, text6, text7));
                    }
                    IsAppointOrWrite = -1; // Меняем значение, чтобы выйти из условия
                }

                // Если пользователь хочет отменить запись
                else if (IsAppointOrWrite == 3)
                {
                    // Поиск доктора, выбор даты
                    if (Step == 1)
                    {
                        if (message.Text.ToLower().Contains("алекс"))
                        {
                            Doctor = "Алексей Сергеев";
                            await client.SendTextMessageAsync(message.Chat.Id, "Отлично, ваш врач -- Алексей Сергеев!\n\nВыберите дату, в которую у вас должна быть встреча:",
                                replyMarkup: GetTenButtons(textDay1, textDay4, textDay7, textDay10, textDay13, textDay16, textDay19, textDay22, textDay25, textBack));
                            Step = 2;
                        }
                        else if (message.Text.ToLower().Contains("ири"))
                        {
                            Doctor = "Ирина Петрова";
                            await client.SendTextMessageAsync(message.Chat.Id, "Отлично, ваш врач -- Ирина Петрова!\n\nВыберите дату, в которую у вас должна быть встреча:",
                                replyMarkup: GetTenButtons(textDay2, textDay5, textDay8, textDay11, textDay14, textDay17, textDay20, textDay23, textDay26, textBack));
                            Step = 2;
                        }
                        else if (message.Text.ToLower().Contains("оль"))
                        {
                            Doctor = "Ольга Иванова";
                            await client.SendTextMessageAsync(message.Chat.Id, "Отлично, ваш врач -- Ольга Иванова!\n\nВыберите дату, в которую у вас должна быть встреча:",
                                replyMarkup: GetTenButtons(textDay3, textDay6, textDay9, textDay12, textDay15, textDay18, textDay21, textDay24, textDay27, textBack));
                            Step = 2;
                        }
                        else if (!message.Text.ToLower().Contains("назад"))
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Введите доктора для нахождения записи", replyMarkup: GetFourButtons(textIP, textAS, textOI, textBack));
                        }
                        else
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Как скажете! Могу помочь вам в следующих услугах:",
                                replyMarkup: GetSevenButtons(text1, text2, text3, text4, text5, text6, text7));
                            Doctor = null;
                            IsAppointOrWrite = -1; // Меняем значение, чтобы выйти из условия
                            Step = 1; // возвращаемся на первый шаг
                        }
                    }
                    // Поиск даты, ввод инфо о пользователе
                    if (Step == 2)
                    {
                        if (message.Text.ToLower().Contains("назад"))
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Выберите дату, в которую у вас должна быть встреча:");
                            Step = 1; // возвращаемся на первый шаг
                        }
                        else if (dateArray.Contains(message.Text))
                        {
                            Date = ParseDateTime(message.Text);

                            await client.SendTextMessageAsync(message.Chat.Id, $"Отлично, дата встречи -- {message.Text}!\n\nВведите, пожалуйста, Ваше имя, фамилию и номер телефона через пробел",
                                replyMarkup: GetOneButton(textBack));
                            Step = 3;
                        }
                        else
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Выберите дату, в которую врач должен провести встречу");
                        }
                    }
                    // Поиск инфо о пользователе, переход к вводу данных в бд
                    if (Step == 3)
                    {
                        var tmp = await client.GetUpdatesAsync();
                        var userInput = tmp.First().Message;

                        if (userInput.Text.ToLower().Contains("назад"))
                        {
                            Step = 2; // возвращаемся на второй шаг
                        }
                        else
                        {
                            var parts = userInput.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length != 3)
                            {
                                await client.SendTextMessageAsync(message.Chat.Id, "Введите верные данные в правильном порядке");
                            }
                            else
                            {
                                Info = userInput.Text;
                                Step = 4;
                            }
                        }
                    }
                    // Ввод данных в бд
                    if (Step == 4)
                    {
                        if (message.Text.ToLower().Contains("назад"))
                        {
                            Step = 3; // возвращаемся на первый шаг
                        }
                        else
                        {
                            // Разделение информации о пользователе
                            var parts = Info.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var firstName = parts[0];
                            var lastName = parts[1];
                            var phoneNumber = parts[2];

                            // Id пользователя в таблице user
                            var userId = await GetUserId(firstName, lastName, phoneNumber);

                            // Поиск встречи в таблице appointment
                            var appointmentId = await FindAppointment(Doctor, userId, Date);
                            if (appointmentId == -1)
                            {
                                await client.SendTextMessageAsync(message.Chat.Id, "Нет записей на отмену.");
                                return;
                            }

                            await UpdateAppointmentStatus(appointmentId, "отменён");

                            await client.SendTextMessageAsync(message.Chat.Id, "Запись отменена. Могу помочь в следующих услугах");
                            IsAppointOrWrite = -1; // Меняем значение, чтобы выйти из условия
                            Step = 1; // возвращаемся на первый шаг
                        }
                    }
                }
                // Если пользователь вводит команду
                else if (message.Text.ToLower().Contains("/"))
                {
                    switch (message.Text.ToLower())
                    {
                        case "/appointment":
                            await AppointmentCommand(client, message);
                            break;
                        case "/cancel":
                            await CancelCommand(client, message);
                            break;
                        case "/clinic":
                            await ClinicsCommand(client, message);
                            break;
                        case "/doctors":
                            await client.SendTextMessageAsync(message.Chat.Id, "Информация о докторах в нашей клинике:");
                            await DoctorsCommand(client, message);
                            break;
                        case "/help":
                            await HelpCommand(client, message);
                            break;
                        case "/news":
                            await NewsCommand(client, message);
                            break;
                        case "/write":
                            await WriteCommand(client, message);
                            break;

                        default:
                            if (!_sentWelcomeMessages.ContainsKey(message.Chat.Id))
                                await client.SendTextMessageAsync(message.Chat.Id, "Введите существующую команду из меню");
                            break;
                    }
                }

                #region возможные варианты ввода команд без /
                // Инфо о клинике
                else if (message.Text.ToLower().Contains("инфо") && message.Text.ToLower().Contains("клиник"))
                {
                    _ = ClinicsCommand(client, message);
                }

                // Записаться на приём
                else if (message.Text.ToLower().Contains("записать") && (message.Text.ToLower().Contains("прием")
                    || message.Text.ToLower().Contains("приём") || message.Text.ToLower().Contains("к доктор")))
                {
                    _ = AppointmentCommand(client, message);
                }

                // Отменить приём
                else if (message.Text.ToLower().Contains("отменить") && (message.Text.ToLower().Contains("прием") || message.Text.ToLower().Contains("приём")))
                {
                    _ = CancelCommand(client, message);
                }

                // Выдать список докторов
                else if (message.Text.ToLower().Contains("доктор"))
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Информация о докторах в нашей клинике:");
                    _ = DoctorsCommand(client, message);
                }

                // Справочная служба
                else if (message.Text.ToLower().Contains("справочн") || message.Text.ToLower().Contains("служб"))
                {
                    _ = HelpCommand(client, message);
                }

                // Новости клиники
                else if (message.Text.ToLower().Contains("новост") && message.Text.ToLower().Contains("клиник"))
                {
                    _ = NewsCommand(client, message);
                }

                // Написать врачу
                else if ((message.Text.ToLower().Contains("вопрос") || message.Text.ToLower().Contains("задат") || message.Text.ToLower().Contains("спрос")
                    || message.Text.ToLower().Contains("напис")) && (message.Text.ToLower().Contains("врач") || message.Text.ToLower().Contains("алекс") 
                    || message.Text.ToLower().Contains("серг") || message.Text.ToLower().Contains("ири") || message.Text.ToLower().Contains("петр") 
                    || message.Text.ToLower().Contains("ол") || message.Text.ToLower().Contains("иван")))
                {
                    _ = WriteCommand(client, message);
                }
                #endregion
                #region приветствие + иное
                // Если пользователь вводит приветствие
                else if (message.Text.ToLower().Contains("здравствуйте") || message.Text.ToLower().Contains("приветствую") || message.Text.ToLower().Contains("салам алейкум")
                     || message.Text.ToLower().Contains("доброе утро") || message.Text.ToLower().Contains("добрый день") || message.Text.ToLower().Contains("добрый вечер")
                     || message.Text.ToLower().Contains("доброй ночи"))
                {
                    // Функция отправки фотографии пользователю
                    string imagePath = Path.Combine(Environment.CurrentDirectory, "котик.jpg");
                    using (var stream = System.IO.File.OpenRead(imagePath))
                    {
                        var result = client.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(stream), caption: "Держите котика для хорошего настроения <3").Result;
                    }

                    await client.SendTextMessageAsync(message.Chat.Id, "Здравствуйте! Могу помочь вам в следующих услугах:",
                        replyMarkup: GetSevenButtons(text1, text2, text3, text4, text5, text6, text7));
                }

                // Если пользователь вводит что-то иное
                else await client.SendTextMessageAsync(message.Chat.Id, "Простите, я не знаю, что вам на это ответить, я умею общаться только на темы, " +
                    "связанные с клиниками, докторами и прочим.\nПожалуйста, выберите, что вас интересует:",
                    replyMarkup: GetSevenButtons(text1, text2, text3, text4, text5, text6, text7));
                #endregion
            }
            else
            {
                await client.SendTextMessageAsync(message?.Chat?.Id ?? 0, "Пожалуйста, введите не пустое сообщение!");
            }

            //// Обработка inline кнопок не работает
            //if (update.CallbackQuery != null && update.CallbackQuery.Data != null)
            //{
            //    var button = update.CallbackQuery;
            //    if (button.Data == "Записаться к Алексею Сергееву")
            //    {
            //        _ = client.SendTextMessageAsync(button.Message.Chat.Id, "ПОЛУЧИЛООООСЬ");
            //    }
            //}
            CheckAppointments(client);
            //NewsCommandWithTask(client, message);
        }

        #region команды клавиатуры
        private static async Task AppointmentCommand(ITelegramBotClient client, Message message)
        {
            _ = DoctorsCommand(client, message);
            await Task.Delay(1300); // Задержка в 1.3 секунды
            await client.SendTextMessageAsync(message.Chat.Id, "К какому врачу вы желаете записаться на приём?", replyMarkup: GetThreeButtons(textIP, textAS, textOI));
            (IsAppointOrWrite, Step) = (1, 1); // отметили, что работаем с назначением встречи с врачом, начинаем с первого шага
        }
        private static async Task CancelCommand(ITelegramBotClient client, Message message)
        {
            _ = DoctorsCommand(client, message);
            await Task.Delay(5300); // Задержка в 5.3 секунды
            await client.SendTextMessageAsync(message.Chat.Id, "У какого врача вы записаны на приём?", replyMarkup: GetThreeButtons(textIP, textAS, textOI));
            (IsAppointOrWrite, Step) = (3, 1); // отметили, что работаем с отменой встречи, начинаем с первого шага
        }
        private static async Task ClinicsCommand(ITelegramBotClient client, Message message)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync(); WriteLine("Подключение к базе данных успешно!");

            const string sql = @"SELECT * FROM clinics WHERE id = @Id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", 1); // Получаем строку с id = 1

            try
            {
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var info = reader.GetString("description"); // Берем значение из колонки description

                    await client.SendTextMessageAsync(message.Chat.Id, $"Клиника Мед-Вышка:\n{info}");
                }
                else
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Нет информации о клинике.");
                }
            }
            catch (Exception ex)
            {
                WriteLine($"Ошибка при получении информации о клинике: {ex.Message}");
                await client.SendTextMessageAsync(message.Chat.Id, "Возникла ошибка при обработке вашего запроса.");
            }
            connection.Close(); WriteLine("Выход из БД");
        }
        private static async Task DoctorsCommand(ITelegramBotClient client, Message message)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync(); WriteLine("Подключение к базе данных успешно!");

            const string sql = @"SELECT * FROM doctors WHERE clinic_id = @CliniсId";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CliniсId", 1); // Фильтруем по clinic_id = 1

            using var reader = await command.ExecuteReaderAsync();

            var doctors = new List<Dictionary<string, object>>();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(reader.GetName(i), reader.GetValue(i));
                }
                doctors.Add(row);
            }
            for (int i = 0; i < doctors.Count; i++)
            {
                var row = doctors[i];
                var firstName = row["first_name"].ToString();
                var lastName = row["last_name"].ToString();
                var specialty = row["specialty"].ToString();
                var email = row["email"].ToString();
                var priceList = row["price_list"].ToString();

                string imagePath = Path.Combine(Environment.CurrentDirectory, $"{firstName} {lastName}.jpg");
                using (var stream = System.IO.File.OpenRead(imagePath))
                {
                    var caption = $"Имя: {firstName} {lastName}\nСпециальность: {specialty}\nEmail: {email}\nЦены: {priceList}";
                    await client.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(stream), caption: caption);
                }
            }
            connection.Close(); WriteLine("Выход из БД");
        }
        private static async Task HelpCommand(ITelegramBotClient client, Message message)
        {
            string pathToPdf = Path.Combine(Environment.CurrentDirectory, "Справочная-служба.pdf");
            using (var stream = System.IO.File.OpenRead(pathToPdf))
            {
                var inputFile = new InputOnlineFile(stream, "application/pdf");

                await client.SendDocumentAsync(
                    chatId: message.Chat.Id,
                    document: inputFile,
                    caption: "Справочная служба");
            }
        }
        private static async Task NewsCommand(ITelegramBotClient client, Message message)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            const string sql = @"SELECT content FROM news";
            using var command = new MySqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            var news = new List<string>();
            while (await reader.ReadAsync())
            {
                news.Add(reader.GetString("content"));
            }

            if (news.Any())
            {
                if (newsId > news.Count)
                {
                    newsId = 1; // Переходим снова к первой новости, если достигли конца списка
                }

                if (newsId == 1)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, news[newsId - 1]);
                }
                else if (newsId == 2)
                {
                    string imagePath = Path.Combine(Environment.CurrentDirectory, "новость 2.jpg");
                    using (var stream = System.IO.File.OpenRead(imagePath))
                    {
                        await client.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(stream), caption: news[newsId - 1]);
                    }
                }

                newsId++; // Переключаемся на следующую новость
            }
            else
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Новостей пока нет.");
            }
        }
        private static async Task NewsCommandWithTask(ITelegramBotClient client, Message message)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            const string sql = @"SELECT content FROM news";
            using var command = new MySqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            var news = new List<string>();
            while (await reader.ReadAsync())
            {
                news.Add(reader.GetString("content"));
            }

            // Первая новость через 5 минут
            await Task.Delay(60000 * 5);
            await client.SendTextMessageAsync(message.Chat.Id, news[0]);

            // Вторая новость через 10 минут
            await Task.Delay(60000 * 5); // Дополнительные 5 минут после первой новости
            string imagePath = Path.Combine(Environment.CurrentDirectory, "новость 2.jpg");
            using (var stream = System.IO.File.OpenRead(imagePath))
            {
                await client.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(stream), caption: news[1]);
            }
            Task.Delay(-1);
        }
        private static async Task WriteCommand(ITelegramBotClient client, Message message)
        {
            _ = DoctorsCommand(client, message);
            await Task.Delay(5300); // Задержка в 5.3 секунды
            await client.SendTextMessageAsync(message.Chat.Id, "Какому врачу вы желаете написать?", replyMarkup: GetThreeButtons(textIP, textAS, textOI));
            IsAppointOrWrite = 2; // отметили, что работаем с написанием врачу
        }
        private static async Task<string> GetDoctorEmail(string fullName)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            const string sql = @"SELECT email FROM doctors WHERE first_name = @FirstName AND last_name = @LastName";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@FirstName", fullName.Split(' ')[0]);
            command.Parameters.AddWithValue("@LastName", fullName.Split(' ')[1]);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetString("email");
            }

            return "Email не найден";
        }
        private static DateTime ParseDateTime(string text)
        {
            var dayMap = new Dictionary<string, DayOfWeek>
            {
                {"ПН", DayOfWeek.Monday},
                {"ВТ", DayOfWeek.Tuesday},
                {"СР", DayOfWeek.Wednesday},
                {"ЧТ", DayOfWeek.Thursday},
                {"ПТ", DayOfWeek.Friday},
                {"СБ", DayOfWeek.Saturday},
                {"ВС", DayOfWeek.Sunday}
            };

            var parts = text.Split(' ');
            var day = dayMap[parts[0]];
            var time = DateTime.Parse(parts[1]);

            var today = DateTime.Today;
            var nextDay = today.AddDays((int)day - (int)today.DayOfWeek);
            if (nextDay < today)
            {
                nextDay = nextDay.AddDays(7);
            }

            return nextDay.Add(time.TimeOfDay);
        }
        private static async Task<int> CreateUser(string firstName, string lastName, string phoneNumber)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            const string sql = @"INSERT INTO user (first_name, last_name, phone_number) 
                        VALUES (@FirstName, @LastName, @PhoneNumber)";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@LastName", lastName);
            command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

            await command.ExecuteNonQueryAsync();

            // Получение ID созданного пользователя
            const string selectSql = @"SELECT LAST_INSERT_ID()";
            using var selectCommand = new MySqlCommand(selectSql, connection);
            var userId = await selectCommand.ExecuteScalarAsync();

            return Convert.ToInt32(userId);
        }
        private static async Task<int> GetUserId(string firstName, string lastName, string phoneNumber)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            const string sql = @"SELECT id FROM user WHERE first_name = @FirstName AND last_name = @LastName AND phone_number = @PhoneNumber";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@LastName", lastName);
            command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32("id");
            }

            return -1; // Возвращаем -1, если пользователь не найден
        }
        private static async Task<int> GetDoctorId(string doctorName)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            const string sql = @"SELECT id FROM doctors WHERE first_name = @FirstName AND last_name = @LastName";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@FirstName", doctorName.Split(' ')[0]);
            command.Parameters.AddWithValue("@LastName", doctorName.Split(' ')[1]);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32("id");
            }

            return -1; // Возвращаем -1, если доктор не найден
        }
        private static async Task CreateAppointment(int userId, string doctorName, DateTime date)
        {
            var doctorId = await GetDoctorId(doctorName);
            if (doctorId == -1)
            {
                throw new Exception("Доктор не найден");
            }

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            const string sql = @"INSERT INTO appointment (clinic_id, doctor_id, user_id, date, time, status, created_at) 
                        VALUES (@ClinicId, @DoctorId, @UserId, @Date, @Time, @Status, @CreatedAt)";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ClinicId", 1);
            command.Parameters.AddWithValue("@DoctorId", doctorId);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Date", date.Date);
            command.Parameters.AddWithValue("@Time", date.TimeOfDay);
            command.Parameters.AddWithValue("@Status", "назначен");
            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

            await command.ExecuteNonQueryAsync();
        }
        private static async Task<int> FindAppointment(string doctor, int userId, DateTime date)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            const string sql = @"SELECT id FROM appointment 
                        WHERE doctor_id = @DoctorId AND user_id = @UserId AND date = @Date AND status = 'назначен'";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@DoctorId", await GetDoctorId(doctor));
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Date", date.Date);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32("id");
            }

            return -1; // Возвращаем -1, если запись не найдена
        }
        private static async Task UpdateAppointmentStatus(int appointmentId, string status)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            const string sql = @"UPDATE appointment SET status = @Status WHERE id = @AppointmentId";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@AppointmentId", appointmentId);

            await command.ExecuteNonQueryAsync();
        }
        public static async Task CheckAndUpdateAppointments()
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            // SQL-запрос выбирает записи, срок которых прошел
            const string sql = @"UPDATE appointment SET status='завершен' WHERE date <= CURDATE() AND time <= CURTIME() AND status <> 'завершен'";

            using var command = new MySqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }
        private static async Task CheckAppointments(ITelegramBotClient client)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            // Получаем все записи со статусом "назначен"
            const string sql = @"SELECT id, user_id, date, time FROM appointment WHERE status = 'назначен'";
            using var command = new MySqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var appointmentId = reader.GetInt32("id");
                var userId = reader.GetInt32("user_id");
                var appointmentDate = reader.GetDateTime("date");
                var appointmentTime = reader.GetTimeSpan("time");

                // Вычисляем время до начала записи
                var appointmentDateTime = appointmentDate.Add(appointmentTime);
                var timeToAppointment = appointmentDateTime - DateTime.Now;

                // Проверяем, нужно ли отправить уведомление
                if (timeToAppointment.TotalDays <= 1 && timeToAppointment.TotalDays > 0)
                {
                    await client.SendTextMessageAsync(userId, "У вас запись через 1 день.");
                }
                else if (timeToAppointment.TotalHours <= 3 && timeToAppointment.TotalHours > 0)
                {
                    await client.SendTextMessageAsync(userId, "У вас запись через 3 часа.");
                }
                else if (timeToAppointment.TotalHours <= 1 && timeToAppointment.TotalHours > 0)
                {
                    await client.SendTextMessageAsync(userId, "У вас запись через 1 час.");
                }
                else if (timeToAppointment.TotalMinutes <= 15 && timeToAppointment.TotalMinutes > 0)
                {
                    await client.SendTextMessageAsync(userId, "У вас запись через 15 минут.");
                }
                else if (timeToAppointment.TotalMinutes == 0)
                {
                    // Обновляем статус встречи на "завершён"
                    const string updateSql = @"UPDATE appointment SET status = 'завершён' WHERE id = @appointmentId";
                    using var updateCommand = new MySqlCommand(updateSql, connection);
                    updateCommand.Parameters.AddWithValue("@appointmentId", appointmentId);
                    await updateCommand.ExecuteNonQueryAsync();
                }
            }
        }
        public static async Task StartBackgroundJob(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await CheckAndUpdateAppointments();
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
        }
        #endregion
        internal static Task Error(ITelegramBotClient client, Exception err, CancellationToken ctoken)
        {
            WriteLine($"Ошибка: {err.Message}");
            return Task.CompletedTask;
        }
    }
}
