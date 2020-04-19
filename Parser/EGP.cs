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
        private TvStreamInfo[] cache;

        public TvStreamInfo[] GetInfo()
        {
            if (!(cache is null)) return cache;
            dynamic data = GatherData();
            TvStreamInfo[] res = ProcessData(data);
            cache = res;
            return res;
        }

        public TvStreamInfo[] GetInfo(DateTime start, DateTime end)
        {
            TvStreamInfo[] source = GetInfo();
            TvStreamInfo[] res = source.Select(show=>new TvStreamInfo()
            {
                channelId = show.channelId,
                channelName = show.channelName,
            }).ToArray();
            for (int i = 0; i < res.Length; i++)
            {
                res[i].records = source[i].records
                    .Where(record => record.startTime > start && record.endTime < end)
                    .ToArray();
            }
            return res;
        }

        public double GetAverageDur(int channelId)
        {
            return GetInfo()
                .First(show => show.channelId == channelId).records
                .Select(record => (record.endTime - record.startTime).TotalMinutes)
                .Average();
        }

        protected abstract object GatherData();
        protected abstract TvStreamInfo[] ProcessData(object source);
    }
}
