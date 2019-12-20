using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Assyst.Models;

namespace Assyst.Extensions
{
    public static class EventExtensions
    {
        // id типа действия 'Остановить таймер заявки'
        private const string StopTimerActionTypeId = "151";
        // текст сообщения при 'остановке таймера заявки'
        public const string StopTimerMessage = "Таймер заявки остановлен";

        public static string SetTimeLeft(DateTime endTime)
        {
            var nowTime = DateTime.Now;
            var difference = endTime.Subtract(nowTime);
            var difHours = Math.Floor(difference.TotalHours);
            var difMinutes = Math.Abs(difference.Minutes);
            return Convert.ToString(difHours, CultureInfo.InvariantCulture) + "h:" + (difMinutes < 10 ? "0" : "") + Convert.ToString(difMinutes) + "m";
        }

        public static long GetTimeLeft(string endTime)
        {
            string[] numbers = Regex.Split(endTime, @"\D+");
            var hours = int.Parse(numbers[1]);
            var minutes = int.Parse(numbers[2]);
            var result = hours * 60 + minutes;
            return result;
        }

        /// <summary>
        /// Функция возвращает Eventid по псевдокоду
        /// </summary>
        public static long? GetEventIdByFormattedReference(string psevdoId)
        {
            if (string.IsNullOrEmpty(psevdoId)) return null;

            var firstSymbol = psevdoId.Substring(0, 1);
            var lastSymbols = psevdoId.Substring(1);

            switch (firstSymbol)
            {
                case "P":
                case "p":
                    return Convert.ToInt64(lastSymbols);
                case "R":
                case "S":
                case "r":
                case "s":
                    return Convert.ToInt64(lastSymbols) + 5000000;
                case "T":
                case "t":
                case "D":
                case "d":
                    return Convert.ToInt64(lastSymbols) + 10000000;
                default:
                    return Convert.ToInt64(psevdoId) + 10000000;
            }
        }
    }
}



