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
            client.StartReceiving(Interaction.Update, Interaction.Error);

            ReadLine();
        }
    }
}
