using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Domain
{
    public class User
    {
        //Efficiency is not where this is at
        private IEnumerable<ReleaseView> releasesOfAuthor;
        public string Name { get; private set; }

        public User(string name, IEnumerable<ReleaseView> releasesOfAuthor)
        {
            Name = name;
            this.releasesOfAuthor = releasesOfAuthor;
        }

        public List<SourceView> SourcesWrittenAt()
        {
            List<SourceView> sources = new List<SourceView>();
            foreach(ReleaseView release in releasesOfAuthor)
            {
                sources.Add(release.Source);
            }
            return sources.Distinct().ToList();
        }

        public int PostsAt(SourceView source)
        {
            return ReleasesAtSource(source).Count();
        }

        public string MostUsedThread(SourceView source)
        {
            var threads = ReleasesAtSource(source)
                .Select(x => x.Name)
                .Distinct();
            int max = 0;
            string mostUsed = null;
            foreach (string thread in threads)
            {
                int count = releasesOfAuthor.Where(x => x.Name == thread).Count();
                if(count > max)
                {
                    max = count;
                    mostUsed = thread;
                }
            }
            return mostUsed;
        }

        public int BusiestDay(SourceView source)
        {
            int[] releasesPerDay = ReleasesPerDay(source);
            int max = 0;
            int maxId = -1;
            for(int i=0; i<releasesPerDay.Length; i++)
            {
                if(max < releasesPerDay[i])
                {
                    max = releasesPerDay[i];
                    maxId = i;
                }
            }
            return maxId;
        }
        private int[] ReleasesPerDay(SourceView source)
        {
            var releasesAtSource = ReleasesAtSource(source);
            int[] releasesPerDay = new int[7];
            foreach (ReleaseView release in releasesAtSource)
            {
                releasesPerDay[(int)release.DateFound.DayOfWeek]++;
            }
            return releasesPerDay;
        }

        private IEnumerable<ReleaseView> ReleasesAtSource(SourceView source)
        {
            return releasesOfAuthor.Where(x => x.Source.Equals(source));
        }

        public int LeastBusyDay(SourceView source)
        {
            int[] releasesPerDay = ReleasesPerDay(source);
            int min = int.MaxValue;
            int minId = -1;
            for(int i=0; i<releasesPerDay.Length; i++)
            {
                if(min > releasesPerDay[i])
                {
                    min = releasesPerDay[i];
                    minId = i;
                }
            }
            return minId;
        }

        public List<List<int>> OfflineTimes(SourceView source)
        {
            int[][] releasesPerDayPerHour = ReleasesPerDayPerHour(source);
            int offlineLimit = releasesPerDayPerHour[0].Length/10;
            List<List<int>> ret = new List<List<int>>();
            for(int i=0; i<releasesPerDayPerHour.Length; i++)
            {
                ret.Add(new List<int>());
                int[] copy = new int[releasesPerDayPerHour[i].Length];
                Array.Copy(releasesPerDayPerHour[i], copy, releasesPerDayPerHour[i].Length);
                Array.Sort(copy);
                int min = copy[offlineLimit];
                for(int j=0; j<releasesPerDayPerHour[i].Length; j++)
                {
                    if (releasesPerDayPerHour[i][j] <= min)
                        ret[i].Add(j);
                }
            }
            return ret;
        }

        private int[][] ReleasesPerDayPerHour(SourceView source)
        {
            int[][] ret = new int[7][];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = new int[24];

            var releases = ReleasesAtSource(source);
            foreach(ReleaseView release in releases)
            {
                ret[(int)release.DateFound.DayOfWeek][release.DateFound.Hour]++;
            }

            return ret;
        }
    }
}
