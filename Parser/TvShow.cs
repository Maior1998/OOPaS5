using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Navigation;

namespace OOPaS5.Parser
{

    /// <summary>
    /// Представляет собой одну запись о телепередаче на канале.
    /// </summary>
    public class TvShow
    {
        /// <summary>
        /// Представляет типы передач.
        /// </summary>
        public enum ShowType
        {
            /// <summary>
            /// Неизвестный тип передачи.
            /// </summary>
            Unknown,
            /// <summary>
            /// Сериал.
            /// </summary>
            Series,
            /// <summary>
            /// Художественный фильм.
            /// </summary>
            FeatureFilm,
            /// <summary>
            /// Познавательное.
            /// </summary>
            Cognitive,
            /// <summary>
            /// Спорт.
            /// </summary>
            Sport,
            /// <summary>
            /// Для взрослых.
            /// </summary>
            Erotic
        }
        private static readonly Dictionary<ShowType, string> showTypesDictionary = new Dictionary<ShowType, string>()
            {
                {ShowType.Unknown,"Неизвестный тип" },
                {ShowType.Series,"Сериал" },
                {ShowType.Cognitive,"Познавательное" },
                {ShowType.Erotic,"Для взрослых" },
                {ShowType.FeatureFilm,"Художественный фильм" },
                {ShowType.Sport,"Спорт" }
            };

        public static string GetStringByType(ShowType source) => showTypesDictionary[source];

        public static ShowType GetTypeByString(string source)
        {
            return showTypesDictionary.All(elem => elem.Value != source) 
                ? ShowType.Unknown : 
                showTypesDictionary.First(elem => elem.Value == source).Key;
        }
        /// <summary>
        /// Название телепередачи.
        /// </summary>
        public string title;
        /// <summary>
        /// Описание телепередачи.
        /// </summary>
        public string description;
        /// <summary>
        /// Время старта передачи.
        /// </summary>
        public DateTime startTime;
        /// <summary>
        /// Время конца передачи.
        /// </summary>
        public DateTime endTime;
        /// <summary>
        /// Тип передачи.
        /// </summary>
        public ShowType type;

        public override string ToString()
        {
            return title;
        }
    }

}