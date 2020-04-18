using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace OOPaS5.Parser
{
    /// <summary>
    /// Программа телепередач.
    /// </summary>
    public abstract class EGP
    {
        
        /// <summary>
        /// Представляет информацию по одному каналу телепередач.
        /// </summary>
        public class TvStreamInfo
        {
            /// <summary>
            /// Название канала.
            /// </summary>
            public string channelName;

            public int channelId;
            /// <summary>
            /// Записи о передачах на данном канале.
            /// </summary>
            public TvShow[] records;
            /// <summary>
            /// Представляет собой одну запись о телепередаче на канале.
            /// </summary>
            public class TvShow
            {
                /// <summary>
                /// Название телепередачи.
                /// </summary>
                public string tvShowName;
                /// <summary>
                /// Время старта передачи.
                /// </summary>
                public DateTime startTime;
                /// <summary>
                /// Время конца передачи.
                /// </summary>
                public DateTime endTime;
            }

            public override string ToString()
            {
                return channelName;
            }
        }

        public TvStreamInfo[] GetInfo()
        {
            string data = GatherData();
            TvStreamInfo[] res = ProcessData(data);
            return res;
        }

        private static string ChoosePath()
        {
            OpenFileDialog file = new OpenFileDialog()
            {
                Title = "Choose source file",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            file.ShowDialog();
            return file.FileName;
        }
        private static string GatherData()
        {
            string filePath = ChoosePath();
            return File.ReadAllText(filePath);
        }
        protected abstract TvStreamInfo[] ProcessData(string source);
    }
}
