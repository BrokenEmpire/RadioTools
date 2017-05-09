using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RadioTools.ConsoleApp
{
    class Program
    {
        class Station
        {
            public int Index { get; set; }
            public string Genre { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
        }

        static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();
            var radioStations = new List<Station>();

            using (var sr = new StreamReader("RadioStations.csv"))
            {
                var index = 0;
                do
                {
                    var line = sr.ReadLine();
                    var data = line.Split(',');

                    radioStations.Add(new Station
                    {
                        Index = index,
                        Genre = data[0].Trim(new[] { '"', ' ', '&', '#' }).Replace("&", " and "),
                        Title = data[1].Trim(new[] { '"', ' ', '&', '#' }).Replace("&", " and "),
                        Url = data[data.GetUpperBound(0)].Trim(new[] { '"', ' '})
                    });

                    index++;                    
                }
                while (!sr.EndOfStream);
            }

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<playlist xmlns=\"http://xspf.org/ns/0/\" xmlns:vlc=\"http://www.videolan.org/vlc/playlist/ns/0/\" version=\"1\">");
            sb.AppendLine("\t<title>Playlist</title>");
            sb.AppendLine("\t<trackList>");

            foreach (var station in radioStations)
            {
                sb.AppendLine("\t\t<track>");
                sb.AppendLine(string.Format("\t\t\t<location>{0}</location>", station.Url));
                sb.AppendLine(string.Format("\t\t\t<title>{0}</title>", station.Title.Length == 0 ? station.Genre : station.Title));
                sb.AppendLine("\t\t\t<extension application=\"http://www.videolan.org/vlc/playlist/0\">");
                sb.AppendLine(string.Format("\t\t\t\t<vlc:id>{0}</vlc:id>", station.Index));
                sb.AppendLine("\t\t\t\t<vlc:option>network-caching=1000</vlc:option>");
                sb.AppendLine("\t\t\t</extension>");
                sb.AppendLine("\t\t</track>");
            }

            sb.AppendLine("\t</trackList>");
            sb.AppendLine("\t<extension application=\"http://www.videolan.org/vlc/playlist/0\">");

            radioStations.ForEach(s => sb.AppendLine(string.Format("\t\t<vlc:item tid=\"{0}\"/>", s.Index)));

            sb.AppendLine("\t</extension>");
            sb.AppendLine("</playlist>");

            using (var sw = new StreamWriter(Environment.SpecialFolder.UserProfile + "Radio1a.xspf", false))
                sw.Write(sb.ToString());

            watch.Stop();

            Console.WriteLine("-------------------------");
            Console.WriteLine(string.Format("Completed in {0} ms", watch.ElapsedMilliseconds));
            Console.Read();
        }
    }
}
