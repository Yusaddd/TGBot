using static System.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
using MySqlConnector;

namespace ConsoleApp25
{
    internal class Program
    {
        static void Main(string[] args) // Начало работы, ввод токена, подключение к бд
        {
            #region подключение к бд
            string connectionString;
            connectionString = @"Server=127.0.0.1;Database=medical_db;port=3306;User id=root;PASSWORD=Yu20sad21!";
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
            cnn.Close();
            WriteLine("Выход из БД");
            #endregion

            var client = new TelegramBotClient("7603978903:AAEAsYC76L0oIJv6UrEWAc8OQYCDh8GTN0A");
            // Регистрация команд
            client.SetMyCommandsAsync(new List<BotCommand>
            {
                new BotCommand { Command = "/start", Description = "Начать общение с ботом" },
                new BotCommand { Command = "/appointment", Description = "Записать на приём" },
                new BotCommand { Command = "/cancel", Description = "Отменить приём" },
                new BotCommand { Command = "/clinic", Description = "Информация о клинике" },
                new BotCommand { Command = "/doctors", Description = "Список докторов в клинике" },
                new BotCommand { Command = "/help", Description = "Справочная служба" },
                new BotCommand { Command = "/news", Description = "Новости клиники" },
                new BotCommand { Command = "/write", Description = "Написать врачу" }
            });
            client.StartReceiving(Interaction.Update, Interaction.Error);

            ReadLine();
        }
    }
}
