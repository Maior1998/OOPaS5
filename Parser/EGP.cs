using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace OOPaS5.Parser
{
    /// <summary>
    /// Программа телепередач.
    /// </summary>
    public abstract partial class EGP
    {
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
