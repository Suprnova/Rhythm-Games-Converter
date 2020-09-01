using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic.CompilerServices;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.Windows.Markup;
using Newtonsoft.Json;

namespace rhythm_games_converter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            source.SelectedIndex = 0;
            search.SelectedIndex = 0;
            prov.SelectedIndex = 0;
        }
        public static class Globals
        {
            public static string links = string.Empty;
            public static string[] linksFinal = new string[100000];
        }
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();
        private void SourceChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void SearchChanged(object sender, SelectionChangedEventArgs e)
        {
            if (search.SelectedIndex == 0)
            {
                provider.Text = "BEMANIWiki";
            }
            else if (search.SelectedIndex == 1)
            {
                provider.Text = "Chorus";
            }
            else if (search.SelectedIndex == 2)
            {
                provider.Text = "SEGA";
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var browseDialog = new VistaFolderBrowserDialog();
            if (browseDialog.ShowDialog() == true)
            {
                dir.Text = browseDialog.SelectedPath.ToString();
            }
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            string webpage = Globals.linksFinal[resultsList.SelectedIndex];
            string command = "start " + webpage;
            System.Diagnostics.Process.Start(@"cmd", @"/c " + command);
        }
        private void SourceClone()
        {
            var titles = CloneFiles(dir.Text);
            foreach (string title in titles)
            {
                if (!String.IsNullOrWhiteSpace(title))
                {
                    sourceSongs.Items.Add(title);
                }
            }
            if (search.SelectedIndex == 0)
            {
                searchBtn.IsEnabled = false;
                source.IsEnabled = false;
                search.IsEnabled = false;
                dir.IsEnabled = false;
                browse.IsEnabled = false;
                prov.IsEnabled = false;
                results.Text = BEMANIMatching(titles, null);
            }
            else if (search.SelectedIndex == 2)
            {
                searchBtn.IsEnabled = false;
                source.IsEnabled = false;
                search.IsEnabled = false;
                dir.IsEnabled = false;
                browse.IsEnabled = false;
                prov.IsEnabled = false;
                results.Text = MaiMaiMatching(titles, null);
            }
        }
        private void SourceOsu()
        {
            (List<string> titles, List<string> titlesUni, List<string> artists) = OsuFiles(dir.Text);
            foreach (string title in titles)
            {
                if (!String.IsNullOrWhiteSpace(title))
                {
                    sourceSongs.Items.Add(title);
                }
            }
            if (search.SelectedIndex == 0)
            {
                searchBtn.IsEnabled = false;
                source.IsEnabled = false;
                search.IsEnabled = false;
                dir.IsEnabled = false;
                browse.IsEnabled = false;
                prov.IsEnabled = false;
                results.Text = BEMANIMatching(titles, titlesUni);
            }
            else if (search.SelectedIndex == 1)
            {
                searchBtn.IsEnabled = false;
                source.IsEnabled = false;
                search.IsEnabled = false;
                dir.IsEnabled = false;
                browse.IsEnabled = false;
                prov.IsEnabled = false;

                Globals.links = CloneMatching(titles, titlesUni, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.linksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;

                }
            }
            else if (search.SelectedIndex == 2)
            {
                searchBtn.IsEnabled = false;
                source.IsEnabled = false;
                search.IsEnabled = false;
                dir.IsEnabled = false;
                browse.IsEnabled = false;
                prov.IsEnabled = false;
                results.Text = MaiMaiMatching(titles, titlesUni);
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (source.SelectedIndex == 1 && search.SelectedIndex == 1)
            {
                MessageBox.Show("You cannot use the same game as the source and the search.", "Error");
                return;
            }
            else if (File.Exists(dir.Text))
            {
                MessageBox.Show("The directory provided does not refer to a folder.", "Error");
                return;
            }
            else if (Directory.Exists(dir.Text))
            {               
                if (source.SelectedIndex == 0) 
                {
                    SourceOsu();
                }
                else if (source.SelectedIndex == 1) 
                {
                    SourceClone();
                }
                else 
                {
                    SourceStep(); ;
                }
            }
            else
            {
                MessageBox.Show("The directory provided is invalid.", "Error");
                return;
            }
        }
        public static string ScrapePage(string webpage)
        {
            string page = string.Empty;
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(webpage);
            HttpWebResponse res = (HttpWebResponse)Req.GetResponse();
            using (StreamReader sr = new StreamReader(res.GetResponseStream()))
            {
                page = sr.ReadToEnd();
            }
            return page;
        }
        private void SourceStep()
        {
            var titles = StepFiles(dir.Text);
            foreach (string title in titles)
            {
                if (!String.IsNullOrWhiteSpace(title))
                {
                    sourceSongs.Items.Add(title);
                }
            }
            if (search.SelectedIndex == 0)
            {
                searchBtn.IsEnabled = false;
                source.IsEnabled = false;
                search.IsEnabled = false;
                dir.IsEnabled = false;
                browse.IsEnabled = false;
                prov.IsEnabled = false;
                results.Text = BEMANIMatching(titles, null);
            }
            else if (search.SelectedIndex == 1)
            {
                searchBtn.IsEnabled = false;
                source.IsEnabled = false;
                search.IsEnabled = false;
                dir.IsEnabled = false;
                browse.IsEnabled = false;
                prov.IsEnabled = false;

                Globals.links = CloneMatching(titles, null, null);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.linksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;

                }
            }
            else if (search.SelectedIndex == 2)
            {
                searchBtn.IsEnabled = false;
                source.IsEnabled = false;
                search.IsEnabled = false;
                dir.IsEnabled = false;
                browse.IsEnabled = false;
                prov.IsEnabled = false;
                results.Text = MaiMaiMatching(titles, null);
            }
        }
        public List<string> CloneFiles(string directory)
        {
            string[] files = Directory.GetFiles(directory, "*.ini", SearchOption.AllDirectories);
            var titles = new List<string>();
            string result = string.Empty;
            App.Current.MainWindow.Hide();
            AllocConsole();
            Console.WriteLine("Indexing Clone Hero files...");
            foreach (string file in files)
            {
                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    if (line.Contains("name")) // title saved
                    {
                        var text = line.Replace("name", "");
                        var text2 = text.Replace("=", "");
                        result = text2.Trim();
                        if (!titles.Contains(result))
                        {
                            titles.Add(result);

                            //goto nextfile;\
                            break;
                        }
                    }
                }
            }
            return titles;
        }
        public static (List<string> titles, List<string> titlesUni, List<string> artists) OsuFiles(string directory)
        {
            string[] files = Directory.GetFiles(directory, "*.osu", SearchOption.AllDirectories);
            var titles = new List<string>();
            var titlesUni = new List<string>();
            var artists = new List<string>();
            string result = String.Empty;
            string resultUni = String.Empty;
            string artist = String.Empty;
            App.Current.MainWindow.Hide();
            AllocConsole();
            Console.WriteLine("Indexing osu! files...");
            foreach (string file in files)
            {
                var lines = File.ReadAllLines(file);

                result = string.Empty;
                resultUni = string.Empty;
                artist = string.Empty;
                foreach (var line in lines)
                {
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    if (line.Contains("Title:")) // title saved
                    {
                        var text = line.Replace("Title:", "");
                        if (text.Contains("(TV Size)"))
                        {
                            text = text.Replace("(TV Size)", "");
                        }
                        if (text.Contains("（TV Size）"))
                        {
                            text = text.Replace("（TV Size）", "");
                        }
                        result = text.Trim();
                    }
                    else if (line.Contains("TitleUnicode:")) // unicode title saved
                    {
                        var textUni = line.Replace("TitleUnicode:", "");
                        if (textUni.Contains("(TV Size)"))
                        {
                            textUni = textUni.Replace("(TV Size)", "");
                        }
                        if (textUni.Contains("（TV Size）"))
                        {
                            textUni = textUni.Replace("（TV Size）", "");
                        }
                        resultUni = textUni.Trim();
                        if (resultUni == string.Empty)
                        {
                            resultUni = "N/A";
                        }
                    }
                    else if (line.Contains("Artist:"))
                    {
                        artist = line.Replace("Artist:", "");
                        artist = artist.Trim();
                    }
                    else if (!String.IsNullOrWhiteSpace(result) && !String.IsNullOrWhiteSpace(resultUni) && !String.IsNullOrWhiteSpace(artist)) //title and unicode title filled, move on
                    {
                        if (!titles.Contains(result))
                        {
                            titles.Add(result);
                            titlesUni.Add(resultUni);
                            artists.Add(artist);

                            //goto nextfile;\
                            break;
                        }
                        else { break; }
                    }
                ////no match, next line
                //nextline:
                //    Console.Write("");
                } // loops to line
            //nextfile:
            //    Console.Write("");
            } // loops to file
            return (titles, titlesUni, artists);
        }
        public List<string> StepFiles(string directory)
        {
            string[] files = Directory.GetFiles(directory, "*.sm", SearchOption.AllDirectories);
            var titles = new List<string>();
            string result = string.Empty;
            App.Current.MainWindow.Hide();
            AllocConsole();
            Console.WriteLine("Indexing Stepmania files...");
            foreach (string file in files)
            {
                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    if (line.Contains("#TITLE:")) // title saved
                    {
                        var text = line.Replace("#TITLE:", "");
                        result = text.Trim(';');
                        if (!titles.Contains(result))
                        {
                            titles.Add(result);
                            //goto nextfile;\
                            break;
                        }
                    }
                }
            }
            return titles;
        }
        protected static int origRow;
        protected static int origCol;

        protected static void WriteAt(string s, int x)
        {
            try
            {
                Console.SetCursorPosition(0, origRow + x);
                Console.Write(s);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }
        public string BEMANIMatching(List<string> titles, List<string> titlesUni)
        {
            MainWindow w = new MainWindow();
            string[] matches = new string[1000000];
            Console.Clear();
            origRow = Console.CursorTop;
            Console.WriteLine("Scraping webpages... (0/19)");
            Console.WriteLine("...................");
            string IIDXPage = ScrapePage("https://bemaniwiki.com/index.php?beatmania%20IIDX%2027%20HEROIC%20VERSE/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("IIDX Page Downloaded (1/19)            ", 0);
            WriteAt("x..................", 1);
            string IIDXPage2 = ScrapePage("https://bemaniwiki.com/index.php?beatmania%20IIDX%2027%20HEROIC%20VERSE/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("IIDX Page 2 Downloaded (2/19)          ", 0);
            WriteAt("xx.................", 1);
            string pmPage = ScrapePage("https://bemaniwiki.com/index.php?pop%27n%20music%20peace/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("pop'n'music Page Downloaded (3/19)     ", 0);
            WriteAt("xxx................", 1);
            string pmPage2 = ScrapePage("https://bemaniwiki.com/index.php?pop%27n%20music%20peace/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("pop'n'music Page 2 Downloaded (4/19)   ", 0);
            WriteAt("xxxx...............", 1);
            string DDRPage = ScrapePage("https://bemaniwiki.com/index.php?DanceDanceRevolution%20A20/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("DDR Page Downloaded (5/19)             ", 0);
            WriteAt("xxxxx..............", 1);
            string DDRPage2 = ScrapePage("https://bemaniwiki.com/index.php?DanceDanceRevolution%20A20/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("DDR 2 Page Downloaded (6/19)           ", 0);
            WriteAt("xxxxxx.............", 1);
            string GDPage = ScrapePage("https://bemaniwiki.com/index.php?GITADORA%20NEX%2BAGE/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("GITADORA Page Downloaded (7/19)        ", 0);
            WriteAt("xxxxxxx............", 1);
            string GDPage2 = ScrapePage("https://bemaniwiki.com/index.php?GITADORA%20NEX%2BAGE/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8%28%A1%C1XG3%29").ToUpper();
            WriteAt("GITADORA Page 2 Downloaded (8/19)      ", 0);
            WriteAt("xxxxxxxx...........", 1);
            string GDPage3 = ScrapePage("https://bemaniwiki.com/index.php?GITADORA%20NEX%2BAGE/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8%28GITADORA%A1%C1%29").ToUpper();
            WriteAt("GITADORA Page 3 Downloaded (9/19)      ", 0);
            WriteAt("xxxxxxxxx..........", 1);
            string jubeatPage = ScrapePage("https://bemaniwiki.com/index.php?jubeat%20festo/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("jubeat Page Downloaded (10/19)         ", 0);
            WriteAt("xxxxxxxxxx.........", 1);
            string jubeatPage2 = ScrapePage("https://bemaniwiki.com/index.php?jubeat%20festo/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("jubeat Page 2 Downloaded (11/19)       ", 0);
            WriteAt("xxxxxxxxxxx........", 1);
            string reflectPage = ScrapePage("https://bemaniwiki.com/index.php?REFLEC%20BEAT%20%CD%AA%B5%D7%A4%CE%A5%EA%A5%D5%A5%EC%A5%B7%A5%A2/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("REFLEC BEAT Page Downloaded (12/19)    ", 0);
            WriteAt("xxxxxxxxxxxx.......", 1);
            string reflectPage2 = ScrapePage("https://bemaniwiki.com/index.php?REFLEC%20BEAT%20%CD%AA%B5%D7%A4%CE%A5%EA%A5%D5%A5%EC%A5%B7%A5%A2/%A5%EA%A5%E1%A5%A4%A5%AF%C9%E8%CC%CC%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("REFELC BEAT Page 2 Downloaded (13/19)  ", 0);
            WriteAt("xxxxxxxxxxxxx......", 1);
            string SDVXPage = ScrapePage("https://bemaniwiki.com/index.php?SOUND%20VOLTEX%20VIVID%20WAVE/%B5%EC%B6%CA/%B3%DA%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("SOUND VOLTEX Page Downloaded (14/19)   ", 0);
            WriteAt("xxxxxxxxxxxxxx.....", 1);
            string SDVXPage2 = ScrapePage("https://bemaniwiki.com/index.php?SOUND%20VOLTEX%20VIVID%20WAVE/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("SOUND VOLTEX Page 2 Downloaded (15/19) ", 0);
            WriteAt("xxxxxxxxxxxxxxx....", 1);
            string nostalgiaPage = ScrapePage("https://bemaniwiki.com/index.php?%A5%CE%A5%B9%A5%BF%A5%EB%A5%B8%A5%A2%20Op.3/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("NOSTALGIA Page Downloaded (16/19)      ", 0);
            WriteAt("xxxxxxxxxxxxxxxx...", 1);
            string nostalgiaPage2 = ScrapePage("https://bemaniwiki.com/index.php?%A5%CE%A5%B9%A5%BF%A5%EB%A5%B8%A5%A2%20Op.3/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("NOSTALGIA Page 2 Downloaded (17/19)    ", 0);
            WriteAt("xxxxxxxxxxxxxxxxx..", 1);
            string DRSDPage = ScrapePage("https://bemaniwiki.com/index.php?DANCERUSH%20STARDOM/%BC%FD%CF%BF%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("DANCERUSH STARDOM Page Downloaded (18/19)", 0);
            WriteAt("xxxxxxxxxxxxxxxxxx.", 1);
            string MUSECAPage = ScrapePage("https://bemaniwiki.com/index.php?MUSECA%201%2B1/2/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8").ToUpper();
            WriteAt("MUSECA Page Downloaded (19/19)           ", 0);
            WriteAt("xxxxxxxxxxxxxxxxxxx", 1);
            var matchesIIDX = new List<string>();
            var matchesPM = new List<string>();
            var matchesDDR = new List<string>();
            var matchesGD = new List<string>();
            var matchesjubeat = new List<string>();
            var matchesreflect = new List<string>();
            var matchesSDVX = new List<string>();
            var matchesnostalgia = new List<string>();
            var matchesDRSD = new List<string>();
            var matchesMUSECA = new List<string>();
            Console.WriteLine("\nFinding matches...");
            for (var i = 0; i<titles.Count; i++)
            {
                var title = titles[i];
                var titleUnicode = String.Empty;
                if (titlesUni != null)
                {
                    titleUnicode = titlesUni[i];
                }
                else
                {
                    titleUnicode = "NOT AVAILABLE";
                }
                string songBracket = ">" + title.ToUpper() + "<";
                string songBracketUnicode = string.Empty;
                if (titlesUni != null)
                {
                    songBracketUnicode = ">" + titlesUni[i].ToUpper() + "<";
                }
                else
                {
                    songBracketUnicode = ">N/A<";
                }
                bool containsUnicode = !songBracketUnicode.Contains(">N/A<");
                if (IIDXPage.Contains(songBracket) || IIDXPage2.Contains(songBracket))
                {
                    matchesIIDX.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (IIDXPage.Contains(songBracketUnicode) || IIDXPage2.Contains(songBracketUnicode))
                    {
                        matchesIIDX.Add(titleUnicode + " - (" + title);
                    }
                }
                if (pmPage.Contains(songBracket) || pmPage2.Contains(songBracket))
                {
                    matchesPM.Add(title);

                }
                else if (containsUnicode == true)
                {
                    if (pmPage.Contains(songBracketUnicode) || pmPage2.Contains(songBracketUnicode))
                    {
                        matchesPM.Add(titleUnicode + " - (" + title);

                    }
                }
                if (DDRPage.Contains(songBracket) || DDRPage2.Contains(songBracket))
                {
                    matchesDDR.Add(title);

                }
                else if (containsUnicode == true)
                {
                    if (DDRPage.Contains(songBracketUnicode) || DDRPage2.Contains(songBracketUnicode))
                    {
                        matchesDDR.Add(titleUnicode + " - (" + title);

                    }
                }
                if (GDPage.Contains(songBracket) || GDPage2.Contains(songBracket) || GDPage3.Contains(songBracket))
                {
                    matchesGD.Add(title);

                }
                else if (containsUnicode == true)
                {
                    if (GDPage.Contains(songBracketUnicode) || GDPage2.Contains(songBracketUnicode) || GDPage2.Contains(songBracket))
                    {
                        matchesGD.Add(titleUnicode + " - (" + title);
                    }
                }
                if (jubeatPage.Contains(songBracket) || jubeatPage2.Contains(songBracket))
                {
                    matchesjubeat.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (jubeatPage.Contains(songBracketUnicode) || jubeatPage2.Contains(songBracketUnicode))
                    {
                        matchesjubeat.Add(titleUnicode + " - (" + title);
                    }
                }
                if (reflectPage.Contains(songBracket) || reflectPage2.Contains(songBracket))
                {
                    matchesreflect.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (reflectPage.Contains(songBracketUnicode) || reflectPage2.Contains(songBracketUnicode))
                    {
                        matchesreflect.Add(titleUnicode + " - (" + title);
                    }
                }
                if (SDVXPage.Contains(songBracket) || SDVXPage2.Contains(songBracket))
                {
                    matchesSDVX.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (SDVXPage.Contains(songBracketUnicode) || SDVXPage2.Contains(songBracketUnicode))
                    {
                        matchesSDVX.Add(titleUnicode + " - (" + title);
                    }
                }
                if (nostalgiaPage.Contains(songBracket) || nostalgiaPage2.Contains(songBracket))
                {
                    matchesnostalgia.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (nostalgiaPage.Contains(songBracketUnicode) || nostalgiaPage2.Contains(songBracketUnicode))
                    {
                        matchesnostalgia.Add(titleUnicode + " - (" + title);
                    }
                }
                if (DRSDPage.Contains(songBracket))
                {
                    matchesDRSD.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (DRSDPage.Contains(songBracketUnicode))
                    {
                        matchesDRSD.Add(titleUnicode + " - (" + title);
                    }
                }
                if (MUSECAPage.Contains(songBracket))
                {
                    matchesMUSECA.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (MUSECAPage.Contains(songBracketUnicode))
                    {
                        matchesMUSECA.Add(titleUnicode + " - (" + title);
                    }
                }
            }
            var sb = new StringBuilder(4096);
            string newline = Environment.NewLine;
            sb.AppendLine("[beatmania IIDX]");
            matchesIIDX.ForEach(s => sb.AppendLine(s));

            sb.AppendLine(newline + "[pop'n'music]");
            matchesPM.ForEach(s => sb.AppendLine(s));

            sb.AppendLine(newline + "[Dance Dance Revolution]");
            matchesDDR.ForEach(s => sb.AppendLine(s));

            sb.AppendLine(newline + "[Gitadora]");
            matchesGD.ForEach(s => sb.AppendLine(s));

            sb.AppendLine(newline + "[jubeat]");
            matchesjubeat.ForEach(s => sb.AppendLine(s));

            sb.AppendLine(newline + "[REFLEC BEAT]");
            matchesreflect.ForEach(s => sb.AppendLine(s));

            sb.AppendLine(newline + "[Sound Voltex]");
            matchesSDVX.ForEach(s => sb.AppendLine(s));

            sb.AppendLine(newline + "[NOSTALGIA]");
            matchesnostalgia.ForEach(s => sb.AppendLine(s));

            sb.AppendLine(newline + "[DANCERUSH STARDOM]");
            matchesDRSD.ForEach(s => sb.AppendLine(s));

            sb.AppendLine(newline + "[MÚSECA]");
            matchesMUSECA.ForEach(s => sb.AppendLine(s));

            App.Current.MainWindow.Show();
            FreeConsole();
            resultsList.Visibility = Visibility.Hidden;
            return sb.ToString();
        }
        public string CloneMatching(List<string> titles, List<string> titlesUni, List<string> artists)
        {
            // god yes i know i dont know how to use jsons leave me alone
            var json = string.Empty;
            var cloneMatches = new List<string>();
            var cloneLinks = new List<string>();
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                var artist = string.Empty;
                if (artists != null)
                {
                    artist = artists[i];
                }
                else
                {
                }
                using (WebClient wc = new WebClient())
                {
                    json = wc.DownloadString("https://chorus.fightthe.pw/api/search?query=name%3D%22" + title + "%22");
                }
                var parsed = JObject.Parse(json);
                if (json.Length < 100)
                {
                    Console.Clear();
                    Console.WriteLine(i + "/" + titles.Count);
                    continue;
                } 
                string songName = parsed.First.First.First.First.Next.ToString().Replace("\"name\":", "").Replace("\"", "").Trim();
                string songNameUpper = songName.ToUpper();
                string titleUpper = title.ToUpper();
                if (songNameUpper == titleUpper)
                {
                    string link = string.Empty;
                    string search = parsed.First.First.First.ToString();
                    if (!string.IsNullOrEmpty(artist))
                    {
                        if (!search.Contains(artist))
                        {
                            continue;
                        }
                    }                    
                    if (search.Contains("\"link\":"))
                    {
                        int index = search.IndexOf("\"link\":");
                        link = search.Substring(index + 9);
                        int index2 = link.IndexOf("\"");
                        link = link.Substring(0, index2);
                    }
                    cloneMatches.Add(title);
                    cloneLinks.Add(link);
                }
                Console.Clear();
                Console.WriteLine(i + "/" + titles.Count);
            }
            var sb = new StringBuilder(4096);
            cloneMatches.ForEach(s => resultsList.Items.Add(s));
            try
            {
                resultsList.SelectedIndex = 0;
            }
            catch { };
            App.Current.MainWindow.Show();
            FreeConsole();
            cloneLinks.ForEach(s => sb.AppendLine(s));
            return sb.ToString();
        }
        public string MaiMaiMatching(List<string> titles, List<string> titlesUni)
        {
            var json = String.Empty;
            var maimaiMatches = new List<string>();
            Console.Clear();
            Console.WriteLine("Matching maimai songs...");
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString("https://maimai.sega.com/data/DXsongs.json");
            }
            for (var i = 0; i < titles.Count; i++)
            {
                var title = "\"" + titles[i] + "\"";
                var titleUnicode = string.Empty;
                if (titlesUni != null) 
                {
                    titleUnicode = "\"" + titlesUni[i] + "\"";
                }
                else
                {
                    titleUnicode = "NOT AVAILABLE";
                }
                if (json.Contains(title))
                {

                    maimaiMatches.Add(title.Replace("\"", ""));
                }
                else if (json.Contains(titleUnicode))
                {
                    maimaiMatches.Add(titleUnicode.Replace("\"", "") + " - (" + title.Replace("\"","") + ")");
                }
            }
            var sb = new StringBuilder(4096);
            sb.AppendLine("[maimai]");
            maimaiMatches.ForEach(s => sb.AppendLine(s));
            App.Current.MainWindow.Show();
            FreeConsole();
            resultsList.Visibility = Visibility.Hidden;
            return sb.ToString();
        }

        private void resultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
