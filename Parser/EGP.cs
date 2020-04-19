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
    public abstract class EGP
    {
        public TvStreamInfo[] GetInfo()
        {
            dynamic data = GatherData();
            TvStreamInfo[] res = ProcessData(data);
            return res;
        }
        
        protected abstract object GatherData();
        protected abstract TvStreamInfo[] ProcessData(object source);
    }
}
