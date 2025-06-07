using System;
using static System.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MySqlConnector;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp25
{
    internal class Buttons
    {
        internal static IReplyMarkup? GetTwoButtons(string t1, string t2)
        {
            var buttons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData(t1),
                    InlineKeyboardButton.WithCallbackData(t2)
                }
            };
            return new InlineKeyboardMarkup(buttons);
        }
        internal static IReplyMarkup? GetThreeButtons(string t1, string t2, string t3)
        {
            var buttons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData(t1),
                    InlineKeyboardButton.WithCallbackData(t2),
                    InlineKeyboardButton.WithCallbackData(t3)
                }
            };
            return new InlineKeyboardMarkup(buttons);
        }
    }
}
