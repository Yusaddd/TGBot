using Telegram.Bot.Types.ReplyMarkups;
namespace ConsoleApp25
{
    internal class Buttons
    {
        //// инлайн кнопки пример:
        //internal static IReplyMarkup? GetOneASButton(string t1)
        //{
        //    var buttons = new List<List<InlineKeyboardButton>>
        //    {
        //        new List<InlineKeyboardButton>
        //        {
        //            InlineKeyboardButton.WithCallbackData(t1, "Записаться к Алексею Сергееву")
        //        }
        //    };
        //    return new InlineKeyboardMarkup(buttons);
        //}
        ////Inline клавиатура:
        //var buttons = new List<List<IKeyboardButton>>
        //{
        //    new List<KeyboardButton> { InlineKeyboardButton.WithCallbackData(t1), InlineKeyboardButton.WithCallbackData(t2) },
        //    new List<KeyboardButton> { InlineKeyboardButton.WithCallbackData(t3), InlineKeyboardButton.WithCallbackData(t4) },
        //    new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(t5), InlineKeyboardButton.WithCallbackData(t6) },
        //    new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(t7), InlineKeyboardButton.WithCallbackData(t8) },
        //    new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(t9) }
        //};

        //return new KeyboardMarkup(buttons);
        internal static IReplyMarkup? GetOneButton(string t1)
        {
            var buttons = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{ new KeyboardButton(t1) }
                };
            return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
        }
        internal static IReplyMarkup? GetThreeButtons(string t1, string t2, string t3)
        {
            var buttons = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{ new KeyboardButton(t1), new KeyboardButton(t2) },
                    new List<KeyboardButton>{ new KeyboardButton(t3) }
                };
            return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
        }
        internal static IReplyMarkup? GetFourButtons(string t1, string t2, string t3, string t4)
        {
            var buttons = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{ new KeyboardButton(t1), new KeyboardButton(t2) },
                    new List<KeyboardButton>{ new KeyboardButton(t3), new KeyboardButton(t4) }
                };
            return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
        }
        internal static IReplyMarkup? GetSevenButtons(string t1, string t2, string t3, string t4, string t5, string t6, string t7)
        {
            var buttons = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{ new KeyboardButton(t1), new KeyboardButton(t2) },
                    new List<KeyboardButton>{ new KeyboardButton(t3), new KeyboardButton(t4) },
                    new List<KeyboardButton>{ new KeyboardButton(t5), new KeyboardButton(t6) },
                    new List<KeyboardButton>{ new KeyboardButton(t7) }
                };
            return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
        }
        internal static IReplyMarkup? GetTenButtons(string t1, string t2, string t3, string t4, string t5, string t6, string t7, string t8, string t9, string t10)
        {
            var buttons = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{ new KeyboardButton(t1), new KeyboardButton(t2), new KeyboardButton(t3), new KeyboardButton(t4), new KeyboardButton(t5) },
                    new List<KeyboardButton>{ new KeyboardButton(t6), new KeyboardButton(t7), new KeyboardButton(t8), new KeyboardButton(t9), new KeyboardButton(t10) }
                };
            return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
        }
    }
}
