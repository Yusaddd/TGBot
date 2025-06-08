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
        internal static IReplyMarkup? GetNineButtons(string t1, string t2, string t3, string t4, string t5, string t6, string t7, string t8, string t9)
        {
            var buttons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(t1), InlineKeyboardButton.WithCallbackData(t2) },
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(t3), InlineKeyboardButton.WithCallbackData(t4) },
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(t5), InlineKeyboardButton.WithCallbackData(t6) },
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(t7), InlineKeyboardButton.WithCallbackData(t8) },
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(t9) }
            };
            return new InlineKeyboardMarkup(buttons);
        }
    }
}
