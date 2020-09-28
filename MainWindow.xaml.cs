namespace rhythm_games_converter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Newtonsoft.Json.Linq;
    using Ookii.Dialogs.Wpf;
    using RestSharp;
    using SpotifyAPI.Web;
    using SpotifyAPI.Web.Auth;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            source.SelectedIndex = 0;
            search.SelectedIndex = 0;
            prov.SelectedIndex = 0;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // adds codepages for encoding
        }

        public static class Globals
        {
            public static string Links = string.Empty;
            public static string[] LinksFinal = new string[100000];
            public static List<string> SpotifyUris = new List<string>();
            public static string SpotifyID = "<REDACTED>";
            public static string SpotifySecret = "<REDACTED>";
            public static string SpotifyToken;
            public static string SpotifyPlaylist;
        }

        public class Beatmap
        {
            public string Title { get; set; }

            public string Artist { get; set; }

            public string BeatmapSet_ID { get; set; }

            public string Mapper { get; set; }
        }

        public class CloneSong
        {
            public string Name { get; set; }

            public string Artist { get; set; }

            public string Link { get; set; }

            public string Charter { get; set; }
        }

        public class WaccaSong
        {
            public TitleList Title { get; set; }

            public class TitleList
            {
                public string Ruby { get; set; }

                public string Display { get; set; }
            }

            public ArtistList Artist { get; set; }

            public class ArtistList
            {
                public string Ruby { get; set; }

                public string Display { get; set; }
            }
        }
        public class BeatSong
        {
            public string Key { get; set; }

            public MetadataList Metadata { get; set; }

            public class MetadataList
            {
                public string SongName { get; set; }

                public string SongAuthorName { get; set; }

                public string LevelAuthorName { get; set; }
            }
        }

        public class ChunSong
        {
            public string Title { get; set; }

            public string Reading { get; set; }

            public string Artist { get; set; }
        }

        public class MaiSong
        {
            public string Title { get; set; }

            public string Title_Kana { get; set; }

            public string Artist { get; set; }
        }

        public class OngekiSong
        {
            public string Title { get; set; }

            public string Title_Sort { get; set; }

            public string Artist { get; set; }
        }

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        private void SearchChanged(object sender, SelectionChangedEventArgs e)
        {
            moreOptions.IsEnabled = false;
            if (search.SelectedIndex == 0)
            {
                provider.Text = "BEMANIWiki";
                moreOptions.IsEnabled = true;
            }
            else if (search.SelectedIndex == 1)
            {
                provider.Text = "Chorus";
            }
            else if (search.SelectedIndex == 2 || search.SelectedIndex == 8 || search.SelectedIndex == 9 || search.SelectedIndex == 10)
            {
                provider.Text = "SEGA";
            }
            else if (search.SelectedIndex == 3)
            {
                provider.Text = "Cypher Gate";
            }
            else if (search.SelectedIndex == 4)
            {
                provider.Text = "groovecoaster.jp";
            }
            else if (search.SelectedIndex == 5)
            {
                provider.Text = "osusearch";
                moreOptions.IsEnabled = true;
            }
            else if (search.SelectedIndex == 6)
            {
                provider.Text = "Beat Saver";
            }
            else if (search.SelectedIndex == 7)
            {
                provider.Text = "Spotify";
            }
            else if (search.SelectedIndex == 11)
            {
                provider.Text = "Arcaea Wikia";
            }
            else if (search.SelectedIndex == 12)
            {
                provider.Text = "marv.jp";
            }
        }

        private void SourceChanged(object sender, SelectionChangedEventArgs e)
        {
            if (source.SelectedIndex == 4)
            {
                dirText.Visibility = Visibility.Hidden;
                urlText.Visibility = Visibility.Visible;
                browse.Visibility = Visibility.Hidden;
            }
            else
            {
                urlText.Visibility = Visibility.Hidden;
                dirText.Visibility = Visibility.Visible;
                browse.Visibility = Visibility.Visible;
            }
        }

        private void moreOptions_Click(object sender, RoutedEventArgs e)
        {
            if (search.SelectedIndex == 5)
            {
                Options options = new Options();
                options.Owner = this;
                options.ShowDialog();
            }
            else if (search.SelectedIndex == 0)
            {
                OptionsBemani optionsBemani = new OptionsBemani();
                optionsBemani.Owner = this;
                optionsBemani.ShowDialog();
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
            try
            {
                string webpage = Globals.LinksFinal[resultsList.SelectedIndex];
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "cmd.exe";
                string command = "start " + webpage;
                startInfo.Arguments = "/c" + command;
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (System.IndexOutOfRangeException)
            {
                return;
            }
        }

        private async void Authentication_Click(object sender, RoutedEventArgs e)
        {
            authenticate.Visibility = Visibility.Hidden;
            await SpotifyConfig();
            await Task.Run(() =>
            {
                do
                {

                }
                while (string.IsNullOrWhiteSpace(Globals.SpotifyToken));
            });    
            ((MainWindow)Application.Current.MainWindow).add.Visibility = Visibility.Visible;
            ((MainWindow)Application.Current.MainWindow).addAll.Visibility = Visibility.Visible;
        }

        private static async Task OnImplicitGrantReceived(object sender, ImplictGrantResponse response)
        {
            await _server.Stop();
            Globals.SpotifyToken = response.AccessToken;
        }
        private static async Task SpotifyConfig()
        {
            _server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);
            await _server.Start();

            _server.ImplictGrantReceived += OnImplicitGrantReceived;

            var request = new LoginRequest(_server.BaseUri, Globals.SpotifyID, LoginRequest.ResponseType.Token)
            {
                Scope = new List<string> { Scopes.PlaylistModifyPrivate, Scopes.PlaylistModifyPublic }
            };
            BrowserUtil.Open(request.ToUri());
        }

        private static EmbedIOAuthServer _server;

        private async void AddAll_Click(object sender, RoutedEventArgs e)
        {
            addAll.IsEnabled = false;
            var spotify = new SpotifyClient(Globals.SpotifyToken);
            if (string.IsNullOrWhiteSpace(Globals.SpotifyPlaylist))
            {
                string item = string.Empty;
                if (source.SelectedIndex == 0)
                {
                    item = "osu!";
                }
                else if (source.SelectedIndex == 1)
                {
                    item = "Clone Hero";
                }
                else if (source.SelectedIndex == 2)
                {
                    item = "Stepmania";
                }
                else if (source.SelectedIndex == 3)
                {
                    item = "Beat Saber";
                }
                var request = new PlaylistCreateRequest("Rhythm Games Converter - " + item);
                var user = await spotify.UserProfile.Current();
                var playlist = await spotify.Playlists.Create(user.Id, request);
                Globals.SpotifyPlaylist = playlist.Id;
            }
            if (!(Globals.SpotifyUris.Count > 100))
            {
                var request = new PlaylistAddItemsRequest(Globals.SpotifyUris);
                await spotify.Playlists.AddItems(Globals.SpotifyPlaylist, request);
            }
            else
            {
                prog.Visibility = Visibility.Visible;
                foreach (string uri in Globals.SpotifyUris)
                {
                    List<string> uris = new List<string>();
                    uris.Add(uri);
                    var request = new PlaylistAddItemsRequest(uris);
                    await spotify.Playlists.AddItems(Globals.SpotifyPlaylist, request);
                }
                prog.Visibility = Visibility.Hidden;
            }
            MessageBox.Show("Added all songs to a Spotify playlist labelled \"Rhythm Games Converter\".", "Success");
            addAll.IsEnabled = false;
        }

        private async void AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var spotify = new SpotifyClient(Globals.SpotifyToken);
            if (string.IsNullOrWhiteSpace(Globals.SpotifyPlaylist))
            {
                string item = string.Empty;
                if (source.SelectedIndex == 0)
                {
                    item = "osu!";
                }
                else if (source.SelectedIndex == 1)
                {
                    item = "Clone Hero";
                }
                else if (source.SelectedIndex == 2)
                {
                    item = "Stepmania";
                }
                else if (source.SelectedIndex == 3)
                {
                    item = "Beat Saber";
                }
                var request = new PlaylistCreateRequest("Rhythm Games Converter - " + item);
                var user = await spotify.UserProfile.Current();
                var playlist = await spotify.Playlists.Create(user.Id, request);
                Globals.SpotifyPlaylist = playlist.Id;
            }
            string uri = Globals.SpotifyUris[resultsList.SelectedIndex];
            List<string> uris = new List<string>();
            uris.Add(uri);
            var requestTrack = new PlaylistAddItemsRequest(uris);
            await spotify.Playlists.AddItems(Globals.SpotifyPlaylist, requestTrack);
            MessageBox.Show("Added selected song to a Spotify playlist labelled \"Rhythm Games Converter\"", "Success");
        }


        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            if ((source.SelectedIndex == 1 && search.SelectedIndex == 1) || (source.SelectedIndex == 0 && search.SelectedIndex == 5) || (source.SelectedIndex == 3 && search.SelectedIndex == 6) || (source.SelectedIndex == 4 && search.SelectedIndex == 7))
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
                else if (source.SelectedIndex == 2)
                {
                    SourceStep();
                }
                else if (source.SelectedIndex == 3)
                {
                    SourceBeatSaber();
                }
                else
                {
                    MessageBox.Show("That is not a valid directory or link.", "Error");
                }
            }
            else if (dir.Text.Contains("open.spotify.com/playlist/") && dir.Text.Contains("?si="))
            {
                await SourceSpotify();
            }
            else
            {
                if (!(source.SelectedIndex == 4))
                {
                    MessageBox.Show("The directory provided is invalid.", "Error");
                    return;
                }
                else
                {
                    MessageBox.Show("The URL provided is not a Spotify playlist link.", "Error");
                    return;
                }
            }
        }

        private void DisableButtons()
        {
            searchBtn.IsEnabled = false;
            source.IsEnabled = false;
            search.IsEnabled = false;
            dir.IsEnabled = false;
            browse.IsEnabled = false;
            prov.IsEnabled = false;
            moreOptions.IsEnabled = false;
        }

        private async Task SourceSpotify()
        {
            App.Current.MainWindow.Hide();
            AllocConsole();
            Console.Title = "Rhythm Games Converter";
            Console.WriteLine("Indexing Spotify songs...");
            string playlistURL = dir.Text;
            int index = playlistURL.IndexOf("playlist/");
            playlistURL = playlistURL.Substring(index + 9);
            int indexEnd = playlistURL.IndexOf("?si=");
            playlistURL = playlistURL.Remove(indexEnd);
            var config = SpotifyClientConfig.CreateDefault().WithRetryHandler(new SimpleRetryHandler() { RetryAfter = TimeSpan.FromSeconds(1) });
            var request = new ClientCredentialsRequest(Globals.SpotifyID, Globals.SpotifySecret);
            var response = await new OAuthClient(config).RequestToken(request);
            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
            var playlist = await spotify.Playlists.GetItems(playlistURL);
            var allOfPlaylist = await spotify.PaginateAll(playlist);
            var songs = new List<string>();
            var artists = new List<string>();
            foreach (PlaylistTrack<IPlayableItem> item in allOfPlaylist)
            {
                if (item.Track is FullTrack track)
                {
                    songs.Add(track.Name);
                    string artist = track.Artists.First().Name;
                    artists.Add(artist);
                    sourceSongs.Items.Add(track.Name + " by " + artist);
                }
            }
            sourceSongs.Items.SortDescriptions.Add(
            new SortDescription("", ListSortDirection.Ascending));

            if (search.SelectedIndex == 0)
            {
                DisableButtons();
                results.Text = BEMANIMatching(songs, null);
            }
            else if (search.SelectedIndex == 1)
            {
                DisableButtons();
                Globals.Links = CloneMatching(songs, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.Links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.LinksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;
                }
            }
            else if (search.SelectedIndex == 2)
            {
                DisableButtons();
                results.Text = MaiMaiMatching(songs, null);
            }
            else if (search.SelectedIndex == 3)
            {
                DisableButtons();
                results.Text = DJMAXMatching(songs, null, artists);
            }
            else if (search.SelectedIndex == 4)
            {
                DisableButtons();
                results.Text = GrooveCoasterMatching(songs, null);
            }
            else if (search.SelectedIndex == 5)
            {
                DisableButtons();
                Globals.Links = OsuMatching(songs, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.Links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.LinksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;
                }
            }
            else if (search.SelectedIndex == 6)
            {
                DisableButtons();
                Globals.Links = BeatSaberMatching(songs, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.Links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.LinksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;
                }
            }
            else if (search.SelectedIndex == 8)
            {
                DisableButtons();
                results.Text = ChunithmMatching(songs, null, artists);
            }
            else if (search.SelectedIndex == 9)
            {
                DisableButtons();
                results.Text = OngekiMatching(songs, null, artists);
            }
            else if (search.SelectedIndex == 10)
            {
                DisableButtons();
                results.Text = DivaMatching(songs, null, artists);
            }
            else if (search.SelectedIndex == 11)
            {
                DisableButtons();
                results.Text = ArcaeaMatching(songs, null, artists);
            }
            else if (search.SelectedIndex == 12)
            {
                DisableButtons();
                results.Text = WaccaMatching(songs, null, artists);
            }
        }

        private void SourceClone()
        {
            (List<string> titles, List<string> artists) = CloneFiles(dir.Text);
            foreach (string title in titles)
            {
                if (!string.IsNullOrWhiteSpace(title))
                {
                    sourceSongs.Items.Add(title);
                }
            }
            sourceSongs.Items.SortDescriptions.Add(
            new SortDescription("", ListSortDirection.Ascending));
            if (search.SelectedIndex == 0)
            {
                DisableButtons();
                results.Text = BEMANIMatching(titles, null);
            }
            else if (search.SelectedIndex == 2)
            {
                DisableButtons();
                results.Text = MaiMaiMatching(titles, null);
            }
            else if (search.SelectedIndex == 3)
            {
                DisableButtons();
                results.Text = DJMAXMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 4)
            {
                DisableButtons();
                results.Text = GrooveCoasterMatching(titles, null);
            }
            else if (search.SelectedIndex == 5)
            {
                DisableButtons();
                Globals.Links = OsuMatching(titles, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.Links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.LinksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;
                }
            }
            else if (search.SelectedIndex == 6)
            {
                DisableButtons();
                Globals.Links = BeatSaberMatching(titles, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.Links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.LinksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;
                }
            }
            else if (search.SelectedIndex == 7)
            {
                DisableButtons();
                SpotifyMatching(titles, artists);
            }
            else if (search.SelectedIndex == 8)
            {
                DisableButtons();
                results.Text = ChunithmMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 9)
            {
                DisableButtons();
                results.Text = OngekiMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 10)
            {
                DisableButtons();
                results.Text = DivaMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 11)
            {
                DisableButtons();
                results.Text = ArcaeaMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 12)
            {
                DisableButtons();
                results.Text = WaccaMatching(titles, null, artists);
            }
        }

        private void SourceOsu()
        {
            (List<string> titles, List<string> titlesUni, List<string> artists) = OsuFiles(dir.Text);
            foreach (string title in titles)
            {
                if (!string.IsNullOrWhiteSpace(title))
                {
                    sourceSongs.Items.Add(title);
                }
            }
            sourceSongs.Items.SortDescriptions.Add(
            new SortDescription("", ListSortDirection.Ascending));
            if (search.SelectedIndex == 0)
            {
                DisableButtons();
                results.Text = BEMANIMatching(titles, titlesUni);
            }
            else if (search.SelectedIndex == 1)
            {
                DisableButtons();
                Globals.Links = CloneMatching(titles, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.Links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.LinksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;
                }
            }
            else if (search.SelectedIndex == 2)
            {
                DisableButtons();
                results.Text = MaiMaiMatching(titles, titlesUni);
            }
            else if (search.SelectedIndex == 3)
            {
                DisableButtons();
                results.Text = DJMAXMatching(titles, titlesUni, artists);
            }
            else if (search.SelectedIndex == 4)
            {
                DisableButtons();
                results.Text = GrooveCoasterMatching(titles, titlesUni);
            }
            else if (search.SelectedIndex == 6)
            {
                DisableButtons();
                Globals.Links = BeatSaberMatching(titles, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.Links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.LinksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;
                }
            }
            else if (search.SelectedIndex == 7)
            {
                DisableButtons();
                SpotifyMatching(titles, artists);
            }
            else if (search.SelectedIndex == 8)
            {
                DisableButtons();
                results.Text = ChunithmMatching(titles, titlesUni, artists);
            }
            else if (search.SelectedIndex == 9)
            {
                DisableButtons();
                results.Text = OngekiMatching(titles, titlesUni, artists);
            }
            else if (search.SelectedIndex == 10)
            {
                DisableButtons();
                results.Text = DivaMatching(titles, titlesUni, artists);
            }
            else if (search.SelectedIndex == 11)
            {
                DisableButtons();
                results.Text = ArcaeaMatching(titles, titlesUni, artists);
            }
            else if (search.SelectedIndex == 12)
            {
                DisableButtons();
                results.Text = WaccaMatching(titles, titlesUni, artists);
            }
        }

        private void SourceStep()
        {
            (List<string> titles, List<string> artists) = StepFiles(dir.Text);
            foreach (string title in titles)
            {
                if (!string.IsNullOrWhiteSpace(title))
                {
                    sourceSongs.Items.Add(title);
                }
            }
            sourceSongs.Items.SortDescriptions.Add(
            new SortDescription("", ListSortDirection.Ascending));
            if (search.SelectedIndex == 0)
            {
                DisableButtons();
                results.Text = BEMANIMatching(titles, null);
            }
            else if (search.SelectedIndex == 1)
            {
                DisableButtons();
                Globals.Links = CloneMatching(titles, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.Links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.LinksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;
                }
            }
            else if (search.SelectedIndex == 2)
            {
                DisableButtons();
                results.Text = MaiMaiMatching(titles, null);
            }
            else if (search.SelectedIndex == 3)
            {
                DisableButtons();
                results.Text = DJMAXMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 4)
            {
                DisableButtons();
                results.Text = GrooveCoasterMatching(titles, null);
            }
            else if (search.SelectedIndex == 5)
            {
                DisableButtons();
                Globals.Links = OsuMatching(titles, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.Links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.LinksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;
                }
            }
            else if (search.SelectedIndex == 6)
            {
                DisableButtons();
                Globals.Links = BeatSaberMatching(titles, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.Links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.LinksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;
                }
            }
            else if (search.SelectedIndex == 7)
            {
                DisableButtons();
                SpotifyMatching(titles, artists);
            }
            else if (search.SelectedIndex == 8)
            {
                DisableButtons();
                results.Text = ChunithmMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 9)
            {
                DisableButtons();
                results.Text = OngekiMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 10)
            {
                DisableButtons();
                results.Text = DivaMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 11)
            {
                DisableButtons();
                results.Text = ArcaeaMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 12)
            {
                DisableButtons();
                results.Text = WaccaMatching(titles, null, artists);
            }
        }

        private void SourceBeatSaber()
        {
            (List<string> titles, List<string> artists) = BeatSaberFiles(dir.Text);
            foreach (string title in titles)
            {
                if (!string.IsNullOrWhiteSpace(title))
                {
                    sourceSongs.Items.Add(title);
                }
            }
            sourceSongs.Items.SortDescriptions.Add(
            new SortDescription("", ListSortDirection.Ascending));
            if (search.SelectedIndex == 0)
            {
                DisableButtons();
                results.Text = BEMANIMatching(titles, null);
            }
            else if (search.SelectedIndex == 1)
            {
                DisableButtons();
                Globals.Links = CloneMatching(titles, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.Links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.LinksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;
                }
            }
            else if (search.SelectedIndex == 2)
            {
                DisableButtons();
                results.Text = MaiMaiMatching(titles, null);
            }
            else if (search.SelectedIndex == 3)
            {
                DisableButtons();
                results.Text = DJMAXMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 4)
            {
                DisableButtons();
                results.Text = GrooveCoasterMatching(titles, null);
            }
            else if (search.SelectedIndex == 5)
            {
                DisableButtons();
                Globals.Links = OsuMatching(titles, artists);
                int i = 0;
                using (StringReader reader = new StringReader(Globals.Links))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            Globals.LinksFinal[i] = line;
                            i++;
                        }
                    }
                    while (line != null);
                    download.IsEnabled = true;
                }
            }
            else if (search.SelectedIndex == 7)
            {
                DisableButtons();
                SpotifyMatching(titles, artists);
            }
            else if (search.SelectedIndex == 8)
            {
                DisableButtons();
                results.Text = ChunithmMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 9)
            {
                DisableButtons();
                results.Text = OngekiMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 10)
            {
                DisableButtons();
                results.Text = DivaMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 11)
            {
                DisableButtons();
                results.Text = ArcaeaMatching(titles, null, artists);
            }
            else if (search.SelectedIndex == 12)
            {
                DisableButtons();
                results.Text = WaccaMatching(titles, null, artists);
            }
        }

        public static string ScrapePage(string webpage, bool utf8)
        {
            if (utf8 == true)
            {
                string page = string.Empty;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(webpage);
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                {
                    page = sr.ReadToEnd();
                }
                return page;
            }
            else // must be a bemaniwiki page encoded in EUC-JP, not UTF8
            {
                string page = string.Empty;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(webpage);
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("EUC-JP")))
                {
                    page = sr.ReadToEnd();
                }
                return page;
            }
        }

        public (List<string> titles, List<string> artists) CloneFiles(string directory)
        {
            App.Current.MainWindow.Hide();
            AllocConsole();
            Console.Title = "Rhythm Games Converter";
            Console.WriteLine("Indexing Clone Hero files...");
            string[] files = Directory.GetFiles(directory, "*.ini", SearchOption.AllDirectories);
            var titles = new List<string>();
            var artists = new List<string>();
            int i = 0;
            foreach (string file in files)
            {
                string result = string.Empty;
                string resultArtist = string.Empty;
                i++;
                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    if (line.Contains("name")) // title saved
                    {
                        var text = line.Replace("name", string.Empty);
                        var text2 = text.Replace("=", string.Empty);
                        result = text2.Trim();
                    }
                    else if (line.Contains("artist"))
                    {
                        var text = line.Replace("artist", string.Empty);
                        var text2 = text.Replace("=", string.Empty);
                        resultArtist = text2.Trim();
                    }
                    else if (!string.IsNullOrWhiteSpace(result) && !string.IsNullOrWhiteSpace(resultArtist)) // title and artist filled, move on
                    {
                        if (!titles.Contains(result))
                        {
                            titles.Add(result);
                            artists.Add(resultArtist);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                Console.WriteLine("Indexing Clone Hero files... " + i + "/" + files.Length);
            }
            return (titles, artists);
        }

        public (List<string> titles, List<string> artists) BeatSaberFiles(string directory)
        {
            App.Current.MainWindow.Hide();
            AllocConsole();
            Console.Title = "Rhythm Games Converter";
            Console.WriteLine("Indexing Beat Saber files...");
            string[] files = Directory.GetFiles(directory, "info.dat", SearchOption.AllDirectories);
            var titles = new List<string>();
            var artists = new List<string>();
            int i = 0;
            foreach (string file in files)
            {
                string result = string.Empty;
                string resultArtist = string.Empty;
                i++;
                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    if (line.Contains("\"_songName\":")) // title saved
                    {
                        var text = line.Replace("\"_songName\": ", string.Empty);
                        var text2 = text.Replace("\"", string.Empty);
                        result = text2.Trim(',');
                        result = result.Trim();
                    }
                    else if (line.Contains("\"_songAuthorName\":")) // title saved
                    {
                        var text = line.Replace("\"_songAuthorName\": ", string.Empty);
                        var text2 = text.Replace("\"", string.Empty);
                        resultArtist = text2.Trim(',');
                        resultArtist = resultArtist.Trim();
                    }
                    else if (!string.IsNullOrWhiteSpace(result) && !string.IsNullOrWhiteSpace(resultArtist)) // title and artist filled, move on
                    {
                        if (!titles.Contains(result))
                        {
                            titles.Add(result);
                            artists.Add(resultArtist);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                Console.WriteLine("Indexing Beat Saber files... " + i + "/" + files.Length);
            }
            return (titles, artists);
        }

        public static (List<string> titles, List<string> titlesUni, List<string> artists) OsuFiles(string directory)
        {
            App.Current.MainWindow.Hide();
            AllocConsole();
            Console.Title = "Rhythm Games Converter";
            Console.WriteLine("Indexing osu! files...");
            string[] files = Directory.GetFiles(directory, "*.osu", SearchOption.AllDirectories);
            var titles = new List<string>();
            var titlesUni = new List<string>();
            var artists = new List<string>();
            int i = 0;
            foreach (string file in files)
            {
                i++;
                var lines = File.ReadAllLines(file);

                string result = string.Empty;
                string resultUni = string.Empty;
                string artist = string.Empty;
                foreach (var line in lines)
                {
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    if (line.Contains("Title:")) // title saved
                    {
                        var text = line.Replace("Title:", string.Empty);
                        if (text.Contains("(TV Size)"))
                        {
                            text = text.Replace("(TV Size)", string.Empty);
                        }
                        if (text.Contains("（TV Size）"))
                        {
                            text = text.Replace("（TV Size）", string.Empty);
                        }
                        result = text.Trim();
                    }
                    else if (line.Contains("TitleUnicode:")) // unicode title saved
                    {
                        var textUni = line.Replace("TitleUnicode:", string.Empty);
                        if (textUni.Contains("(TV Size)"))
                        {
                            textUni = textUni.Replace("(TV Size)", string.Empty);
                        }
                        if (textUni.Contains("（TV Size）"))
                        {
                            textUni = textUni.Replace("（TV Size）", string.Empty);
                        }
                        resultUni = textUni.Trim();
                        if (resultUni == string.Empty)
                        {
                            resultUni = "N/A";
                        }
                    }
                    else if (line.Contains("Artist:"))
                    {
                        artist = line.Replace("Artist:", string.Empty);
                        artist = artist.Trim();
                    }
                    else if (!string.IsNullOrWhiteSpace(result) && !string.IsNullOrWhiteSpace(resultUni) && !string.IsNullOrWhiteSpace(artist)) // title and unicode title filled, move on
                    {
                        if (!titles.Contains(result))
                        {
                            titles.Add(result);
                            titlesUni.Add(resultUni);
                            artists.Add(artist);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // no match, next line
                } // loops to line
                Console.WriteLine("Indexing osu! files... " + i + "/" + files.Length);
            } // loops to file
            return (titles, titlesUni, artists);
        }

        public (List<string> titles, List<string> artists) StepFiles(string directory)
        {
            App.Current.MainWindow.Hide();
            AllocConsole();
            Console.Title = "Rhythm Games Converter";
            Console.WriteLine("Indexing Stepmania files...");
            string[] files = Directory.GetFiles(directory, "*.sm", SearchOption.AllDirectories);
            var titles = new List<string>();
            var artists = new List<string>();
            int i = 0;
            foreach (string file in files)
            {
                string result = string.Empty;
                string resultArtist = string.Empty;
                i++;
                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    if (line.Contains("#TITLE:")) // title saved
                    {
                        var text = line.Replace("#TITLE:", string.Empty);
                        result = text.Trim(';');
                    }
                    else if (line.Contains("#ARTIST:"))
                    {
                        var text = line.Replace("#ARTIST:", string.Empty);
                        resultArtist = text.Trim(';');
                    }
                    else if (!string.IsNullOrWhiteSpace(result) && !string.IsNullOrWhiteSpace(resultArtist)) // title and artist filled, move on
                    {
                        if (!titles.Contains(result))
                        {
                            titles.Add(result);
                            artists.Add(resultArtist);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                Console.WriteLine("Indexing Stepmania files... " + i + "/" + files.Length);
            }
            return (titles, artists);
        }

        public string GrooveCoasterMatching(List<string> titles, List<string> titlesUni)
        {
            var matches = new List<string>();
            if (titlesUni == null)
            {
                MessageBox.Show("These results may be inaccurate because the source you selected does not contain Unicode titles.", "Notice");
            }
            Console.Clear();
            Console.WriteLine("Fetching Groove Coaster song list...");
            string gcPage = ScrapePage("https://groovecoaster.jp/music/#animepops", true).ToUpper() + ScrapePage("https://groovecoaster.jp/music/#vocaloid", true).ToUpper() + ScrapePage("https://groovecoaster.jp/music/#touhou", true).ToUpper() + ScrapePage("https://groovecoaster.jp/music/#otogame", true).ToUpper() + ScrapePage("https://groovecoaster.jp/music/#game", true).ToUpper() + ScrapePage("https://groovecoaster.jp/music/#variety", true).ToUpper() + ScrapePage("https://groovecoaster.jp/music/#original", true).ToUpper();
            Console.Clear();
            Console.WriteLine("Finding matches...");
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                var titleUnicode = string.Empty;
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
                bool containsUnicode = false;
                if (titlesUni != null)
                {
                    songBracketUnicode = ">" + titlesUni[i].ToUpper() + "<";
                    containsUnicode = true;
                }
                else
                {
                    songBracketUnicode = ">N/A<";
                }
                if (gcPage.Contains(songBracket))
                {
                    matches.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (gcPage.Contains(songBracketUnicode))
                    {
                        matches.Add(titleUnicode + " - (" + title + ")");
                    }
                }
            }
            var sb = new StringBuilder(4096);
            matches.Sort();
            matches.ForEach(s => sb.AppendLine(s));
            App.Current.MainWindow.Show();
            FreeConsole();
            resultsList.Visibility = Visibility.Hidden;
            if (sb.Length == 0)
            {
                sb.AppendLine("No matches :(");
                results.FontSize = 50;
            }
            return sb.ToString();
        }

        public string DJMAXMatching(List<string> titles, List<string> titlesUni, List<string> artists)
        {
            var matches = new List<string>();
            Console.Clear();
            Console.WriteLine("Fetching DJMAX song list...");
            string djmaxPage = ScrapePage("http://cyphergate.net/index.php?title=DJMAX_RESPECT:Tracklist", true).ToUpper();
            Console.Clear();
            Console.WriteLine("Finding matches...");
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                var titleUnicode = string.Empty;
                if (titlesUni != null)
                {
                    titleUnicode = titlesUni[i];
                }
                else
                {
                    titleUnicode = "NOT AVAILABLE";
                }
                var artist = string.Empty;
                bool containsArtist = false;
                if (artists != null)
                {
                    containsArtist = true;
                    artist = artists[i];
                }
                else
                {
                }
                string songBracket = ">" + title.ToUpper() + "<";
                string songBracketUnicode = string.Empty;
                string artistBracket = string.Empty;
                if (titlesUni != null)
                {
                    songBracketUnicode = ">" + titlesUni[i].ToUpper() + "<";
                }
                else
                {
                    songBracketUnicode = ">N/A<";
                }
                if (artists != null)
                {
                    artistBracket = ">" + artists[i].ToUpper() + "<";
                }
                bool containsUnicode = !songBracketUnicode.Contains(">N/A<");
                if (djmaxPage.Contains(songBracket))
                {
                    if (containsArtist == true)
                    {
                        if (djmaxPage.Contains(artistBracket))
                        {
                            matches.Add(title + " by " + artist);
                        }
                    }
                    else
                    {
                        matches.Add(title);
                    }
                }
                else if (containsUnicode == true)
                {
                    if (djmaxPage.Contains(songBracketUnicode))
                    {
                        if (containsArtist == true)
                        {
                            if (djmaxPage.Contains(artistBracket))
                            {
                                matches.Add(titleUnicode + " - (" + title + ") by " + artist);
                            }
                        }
                        else
                        {
                            matches.Add(titleUnicode + " - (" + title + ") by " + artist);
                        }
                    }
                }
            }
            var sb = new StringBuilder(4096);
            matches.Sort();
            matches.ForEach(s => sb.AppendLine(s));
            App.Current.MainWindow.Show();
            FreeConsole();
            resultsList.Visibility = Visibility.Hidden;
            if (sb.Length == 0)
            {
                sb.AppendLine("No matches :(");
                results.FontSize = 50;
            }
            return sb.ToString();
        }

        public string BEMANIMatching(List<string> titles, List<string> titlesUni)
        {
            string[] matches = new string[1000000];
            if (titlesUni == null)
            {
                MessageBox.Show("These results may be inaccurate because the source you selected does not contain Unicode titles.", "Notice");
            }
            Console.Clear();
            Console.WriteLine("Scraping webpages...");
            string iidxPage = string.Empty;
            string iidxPage2 = string.Empty;
            string pmPage = string.Empty;
            string pmPage2 = string.Empty;
            string ddrPage = string.Empty;
            string ddrPage2 = string.Empty;
            string gdPage = string.Empty;
            string gdPage2 = string.Empty;
            string gdPage3 = string.Empty;
            string jubeatPage = string.Empty;
            string jubeatPage2 = string.Empty;
            string reflectPage = string.Empty;
            string reflectPage2 = string.Empty;
            string sdvxPage = string.Empty;
            string sdvxPage2 = string.Empty;
            string nostalgiaPage = string.Empty;
            string nostalgiaPage2 = string.Empty;
            string drsdPage = string.Empty;
            string musecaPage = string.Empty;
            if (beatmaniaSelection.Text == "1")
            {
                iidxPage = ScrapePage("https://bemaniwiki.com/index.php?beatmania%20IIDX%2027%20HEROIC%20VERSE/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("IIDX page 1 scraped...");
                iidxPage2 = ScrapePage("https://bemaniwiki.com/index.php?beatmania%20IIDX%2027%20HEROIC%20VERSE/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("IIDX page 2 scraped...");
            }
            if (popnmusicSelection.Text == "1")
            {
                pmPage = ScrapePage("https://bemaniwiki.com/index.php?pop%27n%20music%20peace/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("pop'n'music page 1 scraped...");
                pmPage2 = ScrapePage("https://bemaniwiki.com/index.php?pop%27n%20music%20peace/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("pop'n'music page 2 scraped...");
            }
            if (ddrSelection.Text == "1")
            {
                ddrPage = ScrapePage("https://bemaniwiki.com/index.php?DanceDanceRevolution%20A20/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("Dance Dance Revolution page 1 scraped...");
                ddrPage2 = ScrapePage("https://bemaniwiki.com/index.php?DanceDanceRevolution%20A20/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("Dance Dance Revolution page 2 scraped...");
            }
            if (gitadoraSelection.Text == "1")
            {
                gdPage = ScrapePage("https://bemaniwiki.com/index.php?GITADORA%20NEX%2BAGE/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("GITADORA page 1 scraped...");
                gdPage2 = ScrapePage("https://bemaniwiki.com/index.php?GITADORA%20NEX%2BAGE/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8%28%A1%C1XG3%29", false).ToUpper();
                Console.Clear();
                Console.WriteLine("GITADORA page 2 scraped...");
                gdPage3 = ScrapePage("https://bemaniwiki.com/index.php?GITADORA%20NEX%2BAGE/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8%28GITADORA%A1%C1%29", false).ToUpper();
                Console.Clear();
                Console.WriteLine("GITADORA page 3 scraped...");
            }
            if (jubeatSelection.Text == "1")
            {
                jubeatPage = ScrapePage("https://bemaniwiki.com/index.php?jubeat%20festo/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("jubeat page 1 scraped...");
                jubeatPage2 = ScrapePage("https://bemaniwiki.com/index.php?jubeat%20festo/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("jubeat page 2 scraped...");
            }
            if (reflecSelection.Text == "1")
            {
                reflectPage = ScrapePage("https://bemaniwiki.com/index.php?REFLEC%20BEAT%20%CD%AA%B5%D7%A4%CE%A5%EA%A5%D5%A5%EC%A5%B7%A5%A2/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("REFLEC BEAT page 1 scraped...");
                reflectPage2 = ScrapePage("https://bemaniwiki.com/index.php?REFLEC%20BEAT%20%CD%AA%B5%D7%A4%CE%A5%EA%A5%D5%A5%EC%A5%B7%A5%A2/%A5%EA%A5%E1%A5%A4%A5%AF%C9%E8%CC%CC%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("REFLEC BEAT page 2 scraped...");
            }
            if (sdvxSelection.Text == "1")
            {
                sdvxPage = ScrapePage("https://bemaniwiki.com/index.php?SOUND%20VOLTEX%20VIVID%20WAVE/%B5%EC%B6%CA/%B3%DA%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("Sound Voltex page 1 scraped...");
                sdvxPage2 = ScrapePage("https://bemaniwiki.com/index.php?SOUND%20VOLTEX%20VIVID%20WAVE/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("Sound Voltex page 2 scraped...");
            }
            if (nostalgiaSelection.Text == "1")
            {
                nostalgiaPage = ScrapePage("https://bemaniwiki.com/index.php?%A5%CE%A5%B9%A5%BF%A5%EB%A5%B8%A5%A2%20Op.3/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("Nostalgia page 1 scraped...");
                nostalgiaPage2 = ScrapePage("https://bemaniwiki.com/index.php?%A5%CE%A5%B9%A5%BF%A5%EB%A5%B8%A5%A2%20Op.3/%B5%EC%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("Nostalgia page 2 scraped...");
            }
            if (drsdSelection.Text == "1")
            {
                drsdPage = ScrapePage("https://bemaniwiki.com/index.php?DANCERUSH%20STARDOM/%BC%FD%CF%BF%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("DANCERUSH STARDOM page scraped...");
            }
            if (musecaSelection.Text == "1")
            {
                musecaPage = ScrapePage("https://bemaniwiki.com/index.php?MUSECA%201%2B1/2/%BF%B7%B6%CA%A5%EA%A5%B9%A5%C8", false).ToUpper();
                Console.Clear();
                Console.WriteLine("MUSECA page scraped...");
            }           
            var matchesIIDX = new List<string>();
            var matchesPM = new List<string>();
            var matchesDDR = new List<string>();
            var matchesGD = new List<string>();
            var matchesJubeat = new List<string>();
            var matchesReflect = new List<string>();
            var matchesSDVX = new List<string>();
            var matchesNostalgia = new List<string>();
            var matchesDRSD = new List<string>();
            var matchesMUSECA = new List<string>();
            Console.Clear();
            Console.WriteLine("Finding matches...");
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                var titleUnicode = string.Empty;
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
                if (iidxPage.Contains(songBracket) || iidxPage2.Contains(songBracket))
                {
                    matchesIIDX.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (iidxPage.Contains(songBracketUnicode) || iidxPage2.Contains(songBracketUnicode))
                    {
                        matchesIIDX.Add(titleUnicode + " - (" + title + ")");
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
                        matchesPM.Add(titleUnicode + " - (" + title + ")");
                    }
                }
                if (ddrPage.Contains(songBracket) || ddrPage2.Contains(songBracket))
                {
                    matchesDDR.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (ddrPage.Contains(songBracketUnicode) || ddrPage2.Contains(songBracketUnicode))
                    {
                        matchesDDR.Add(titleUnicode + " - (" + title + ")");
                    }
                }
                if (gdPage.Contains(songBracket) || gdPage2.Contains(songBracket) || gdPage3.Contains(songBracket))
                {
                    matchesGD.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (gdPage.Contains(songBracketUnicode) || gdPage2.Contains(songBracketUnicode) || gdPage2.Contains(songBracket))
                    {
                        matchesGD.Add(titleUnicode + " - (" + title + ")");
                    }
                }
                if (jubeatPage.Contains(songBracket) || jubeatPage2.Contains(songBracket))
                {
                    matchesJubeat.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (jubeatPage.Contains(songBracketUnicode) || jubeatPage2.Contains(songBracketUnicode))
                    {
                        matchesJubeat.Add(titleUnicode + " - (" + title + ")");
                    }
                }
                if (reflectPage.Contains(songBracket) || reflectPage2.Contains(songBracket))
                {
                    matchesReflect.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (reflectPage.Contains(songBracketUnicode) || reflectPage2.Contains(songBracketUnicode))
                    {
                        matchesReflect.Add(titleUnicode + " - (" + title + ")");
                    }
                }
                if (sdvxPage.Contains(songBracket) || sdvxPage2.Contains(songBracket))
                {
                    matchesSDVX.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (sdvxPage.Contains(songBracketUnicode) || sdvxPage2.Contains(songBracketUnicode))
                    {
                        matchesSDVX.Add(titleUnicode + " - (" + title + ")");
                    }
                }
                if (nostalgiaPage.Contains(songBracket) || nostalgiaPage2.Contains(songBracket))
                {
                    matchesNostalgia.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (nostalgiaPage.Contains(songBracketUnicode) || nostalgiaPage2.Contains(songBracketUnicode))
                    {
                        matchesNostalgia.Add(titleUnicode + " - (" + title + ")");
                    }
                }
                if (drsdPage.Contains(songBracket))
                {
                    matchesDRSD.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (drsdPage.Contains(songBracketUnicode))
                    {
                        matchesDRSD.Add(titleUnicode + " - (" + title + ")");
                    }
                }
                if (musecaPage.Contains(songBracket))
                {
                    matchesMUSECA.Add(title);
                }
                else if (containsUnicode == true)
                {
                    if (musecaPage.Contains(songBracketUnicode))
                    {
                        matchesMUSECA.Add(titleUnicode + " - (" + title + ")");
                    }
                }
            }
            var sb = new StringBuilder(4096);
            string newline = Environment.NewLine;
            if (beatmaniaSelection.Text == "1")
            {
                sb.AppendLine("[beatmania IIDX]");
                matchesIIDX.Sort();
                matchesIIDX.ForEach(s => sb.AppendLine(s));
                if (matchesIIDX.Count == 0)
                {
                    sb.AppendLine("No matches :(");
                }
                sb.AppendLine(newline);
            }
            if (popnmusicSelection.Text == "1")
            {
                sb.AppendLine("[pop'n'music]");
                matchesPM.Sort();
                matchesPM.ForEach(s => sb.AppendLine(s));
                if (matchesPM.Count == 0)
                {
                    sb.AppendLine("No matches :(");
                }
                sb.AppendLine(newline);
            }
            if (ddrSelection.Text == "1")
            {
                sb.AppendLine("[Dance Dance Revolution]");
                matchesDDR.Sort();
                matchesDDR.ForEach(s => sb.AppendLine(s));
                if (matchesDDR.Count == 0)
                {
                    sb.AppendLine("No matches :(");
                }
                sb.AppendLine(newline);
            }
            if (gitadoraSelection.Text == "1")
            {
                sb.AppendLine("[Gitadora]");
                matchesGD.Sort();
                matchesGD.ForEach(s => sb.AppendLine(s));
                if (matchesGD.Count == 0)
                {
                    sb.AppendLine("No matches :(");
                }
                sb.AppendLine(newline);
            }
            if (jubeatSelection.Text == "1")
            {
                sb.AppendLine("[jubeat]");
                matchesJubeat.Sort();
                matchesJubeat.ForEach(s => sb.AppendLine(s));
                if (matchesJubeat.Count == 0)
                {
                    sb.AppendLine("No matches :(");
                }
                sb.AppendLine(newline);
            }
            if (reflecSelection.Text == "1")
            {
                sb.AppendLine("[REFLEC BEAT]");
                matchesReflect.Sort();
                matchesReflect.ForEach(s => sb.AppendLine(s));
                if (matchesReflect.Count == 0)
                {
                    sb.AppendLine("No matches :(");
                }
                sb.AppendLine(newline);
            }
            if (sdvxSelection.Text == "1")
            {
                sb.AppendLine("[Sound Voltex]");
                matchesSDVX.Sort();
                matchesSDVX.ForEach(s => sb.AppendLine(s));
                if (matchesSDVX.Count == 0)
                {
                    sb.AppendLine("No matches :(");
                }
                sb.AppendLine(newline);
            }
            if (nostalgiaSelection.Text == "1")
            {
                sb.AppendLine("[NOSTALGIA]");
                matchesNostalgia.Sort();
                matchesNostalgia.ForEach(s => sb.AppendLine(s));
                if (matchesNostalgia.Count == 0)
                {
                    sb.AppendLine("No matches :(");
                }
                sb.AppendLine(newline);
            }
            if (drsdSelection.Text == "1")
            {
                sb.AppendLine("[DANCERUSH STARDOM]");
                matchesDRSD.Sort();
                matchesDRSD.ForEach(s => sb.AppendLine(s));
                if (matchesDRSD.Count == 0)
                {
                    sb.AppendLine("No matches :(");
                }
                sb.AppendLine(newline);
            }
            if (musecaSelection.Text == "1")
            {
                sb.AppendLine("[MÚSECA]");
                matchesMUSECA.Sort();
                matchesMUSECA.ForEach(s => sb.AppendLine(s));
                if (matchesMUSECA.Count == 0)
                {
                    sb.AppendLine("No matches :(");
                }
            }
            App.Current.MainWindow.Show();
            FreeConsole();
            resultsList.Visibility = Visibility.Hidden;
            return sb.ToString();
        }

        public string CloneMatching(List<string> titles, List<string> artists)
        {
            var json = string.Empty;
            var cloneMatches = new List<string>();
            var cloneLinks = new List<string>();
            Console.Clear();
            Console.WriteLine("Searching Clone Hero songs... ");
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                var artist = string.Empty;
                bool containsArtist = false;
                if (artists != null)
                {
                    containsArtist = true;
                    artist = artists[i];
                }
                else
                {
                }
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        json = wc.DownloadString("https://chorus.fightthe.pw/api/search?query=name%3D%22" + title + "%22");
                    }
                }
                catch
                {
                    continue;
                }
                if (json.Length < 100)
                {
                    Console.Clear();
                    Console.WriteLine("Searching Clone Hero songs... " + i + "/" + titles.Count);
                    continue;
                }
                JObject songSearch = JObject.Parse(json);
                IList<JToken> results = songSearch["songs"].Children().ToList();
                IList<CloneSong> songs = new List<CloneSong>();
                foreach (JToken result in results)
                {
                    CloneSong song = result.ToObject<CloneSong>();
                    if (song.Name.ToUpper() == title.ToUpper())
                    {
                        if (containsArtist == true)
                        {
                            if (song.Artist.ToUpper() == artist.ToUpper())
                            {
                                cloneMatches.Add(song.Name + " - Charted by " + song.Charter);
                                cloneLinks.Add(song.Link);
                            }
                        }
                        else
                        {
                            cloneMatches.Add(song.Name + " - Charted by " + song.Charter);
                            cloneLinks.Add(song.Link);
                        }
                    }
                }
                Console.Clear();
                Console.WriteLine("Searching Clone Hero songs... " + i + "/" + titles.Count);
            }
            var sb = new StringBuilder(4096);
            cloneMatches.ForEach(s => resultsList.Items.Add(s));
            try
            {
                resultsList.SelectedIndex = 0;
            }
            catch
            {
            }
            App.Current.MainWindow.Show();
            FreeConsole();
            cloneLinks.ForEach(s => sb.AppendLine(s));
            return sb.ToString();
        }

        public string MaiMaiMatching(List<string> titles, List<string> titlesUni)
        {
            var json = string.Empty;
            var maimaiMatches = new List<string>();
            if (titlesUni == null)
            {
                MessageBox.Show("These results may be inaccurate because the source you selected does not contain Unicode titles.", "Notice");
            }
            Console.Clear();
            Console.WriteLine("Matching maimai songs...");
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString("https://maimai.sega.com/data/DXsongs.json");
            }
            JArray maimaiSongs = JArray.Parse(json);
            IList<JToken> maimaiSongsList = maimaiSongs.Children().ToList();
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                var titleUnicode = string.Empty;
                bool containsUni = false;
                if (titlesUni != null)
                {
                    titleUnicode = titlesUni[i];
                    containsUni = true;
                }
                else
                {
                    titleUnicode = "NOT AVAILABLE";
                }
                foreach (JToken result in maimaiSongsList)
                {
                    MaiSong song = result.ToObject<MaiSong>();
                    if ((song.Title.ToUpper() == title.ToUpper()) || song.Title_Kana.ToUpper() == title.ToUpper())
                    {
                        maimaiMatches.Add(song.Title + " by " + song.Artist);
                    }
                    else if (containsUni == true)
                    {
                        if ((song.Title.ToUpper() == titleUnicode.ToUpper()) || song.Title_Kana.ToUpper() == titleUnicode.ToUpper())
                        {
                            maimaiMatches.Add(titleUnicode + " - (" + title + ") by " + song.Artist);
                        }
                    }
                }
            }
            var sb = new StringBuilder(4096);
            maimaiMatches.Sort();
            maimaiMatches.ForEach(s => sb.AppendLine(s));
            App.Current.MainWindow.Show();
            FreeConsole();
            resultsList.Visibility = Visibility.Hidden;
            if (sb.Length == 0)
            {
                sb.AppendLine("No matches :(");
                results.FontSize = 50;
            }
            return sb.ToString();
        }

        public string OsuMatching(List<string> titles, List<string> artists)
        {
            var json = string.Empty;
            var osuMatches = new List<string>();
            var osuLinks = new List<string>();
            Console.Clear();
            Console.WriteLine("Searching osu! songs... ");
            var selections = new System.Text.StringBuilder();
            if (osuSelection.Text == "1")
            {
                selections.Append("Standard");
            }
            if (maniaSelection.Text == "1")
            {
                if (!(selections.Length == 0))
                {
                    selections.Append(",Mania");
                }
                else
                {
                    selections.Append("Mania");
                }
            }
            if (taikoSelection.Text == "1")
            {
                if (!(selections.Length == 0))
                {
                    selections.Append(",Taiko");
                }
                else
                {
                    selections.Append("Taiko");
                }
            }
            if (ctbSelection.Text == "1")
            {
                if (!(selections.Length == 0))
                {
                    selections.Append(",CtB");
                }
                else
                {
                    selections.Append("CtB");
                }
            }
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                var artist = string.Empty;
                bool hasArtist = false;
                if (artists != null)
                {
                    hasArtist = true;
                    artist = artists[i];
                }
                else
                {
                }
                if (hasArtist == true)
                {
                    try
                    {
                        using (WebClient wc = new WebClient())
                        {
                            json = wc.DownloadString("https://osusearch.com/query/?title=\"" + title + "\"&artist=\"" + artist + "\"&statuses=Ranked,Loved&modes=" + selections.ToString());
                        }
                    }
                    catch
                    {
                        continue;
                    }
                    if (json.Length < 100)
                    {
                        try
                        {
                            using (WebClient wc = new WebClient())
                            {
                                json = wc.DownloadString("https://osusearch.com/query/?title=" + title + "&artist=" + artist + "&statuses=Ranked,Loved&modes=" + selections.ToString());
                            }
                        }
                        catch
                        {
                            continue;
                        }
                        if (json.Length < 100)
                        {
                            Console.Clear();
                            Console.WriteLine("Searching osu! songs... " + i + "/" + titles.Count);
                            continue;
                        }
                    }
                }
                else
                {
                    try
                    {
                        using (WebClient wc = new WebClient())
                        {
                            json = wc.DownloadString("https://osusearch.com/query/?title=\"" + title + "\"&statuses=Ranked,Loved");
                        }
                    }
                    catch
                    {
                        continue;
                    }
                    if (json.Length < 100)
                    {
                        try
                        {
                            using (WebClient wc = new WebClient())
                            {
                                json = wc.DownloadString("https://osusearch.com/query/?title=" + title + "&statuses=Ranked,Loved");
                            }
                        }
                        catch
                        {
                            continue;
                        }
                        if (json.Length < 100)
                        {
                            Console.Clear();
                            Console.WriteLine("Searching osu! songs... " + i + "/" + titles.Count);
                            continue;
                        }
                    }
                }
                JObject beatmapSearch = JObject.Parse(json);
                IList<JToken> results = beatmapSearch["beatmaps"].Children().ToList();
                IList<Beatmap> beatmaps = new List<Beatmap>();
                foreach (JToken result in results)
                {
                    Beatmap beatmap = result.ToObject<Beatmap>();
                    if (beatmap.Title.ToUpper() == title.ToUpper())
                    {
                        if (hasArtist == true)
                        {
                            if (beatmap.Artist.ToUpper() == artist.ToUpper())
                            {
                                osuMatches.Add(beatmap.Title + " - Mapped by " + beatmap.Mapper);
                                osuLinks.Add("https://osu.ppy.sh/beatmapsets/" + beatmap.BeatmapSet_ID);
                            }
                        }
                        else
                        {
                            osuMatches.Add(beatmap.Title + " - Mapped by " + beatmap.Mapper);
                            osuLinks.Add("https://osu.ppy.sh/beatmapsets/" + beatmap.BeatmapSet_ID);
                        }
                    }
                }
                Console.Clear();
                Console.WriteLine("Searching osu! songs... " + i + "/" + titles.Count);
            }
            var sb = new StringBuilder(4096);
            osuMatches.ForEach(s => resultsList.Items.Add(s));
            try
            {
                resultsList.SelectedIndex = 0;
            }
            catch
            {
            }
            App.Current.MainWindow.Show();
            FreeConsole();
            osuLinks.ForEach(s => sb.AppendLine(s));
            return sb.ToString();
        }
        private async Task SpotifyMatching(List<string> titles, List<string> artists)
        {
            Console.Clear();
            Console.WriteLine("Searching Spotify...");
            var spotifyMatches = new List<string>();
            var spotifyLinks = new List<string>();
            var spotifyUris = new List<string>();
            var config = SpotifyClientConfig.CreateDefault().WithRetryHandler(new SimpleRetryHandler() { RetryAfter = TimeSpan.FromSeconds(1) });
            var request = new ClientCredentialsRequest(Globals.SpotifyID, Globals.SpotifySecret);
            var response = await new OAuthClient(config).RequestToken(request);
            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
            for (var i = 0; i < titles.Count; i++)
            {
                var search = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.Track, "track:\"" + titles[i] + "\""));
                int index = 0;
                await foreach (var item in spotify.Paginate(search.Tracks, (s) => s.Tracks))
                {
                    index++;
                    if (item.Artists.First().Name.Contains(artists[i]))
                    {
                        spotifyMatches.Add(item.Name + " by " + item.Artists.First().Name);
                        spotifyLinks.Add(item.ExternalUrls.Values.First());
                        spotifyUris.Add(item.Uri);
                        break;
                    }
                    if (index > 100)
                    {
                        // too many results to bother, timeout and move on
                        break;
                    }
                }
                Console.Clear();
                Console.WriteLine("Searching Spotify... " + i + "/" + titles.Count);
            }
            spotifyUris.ForEach(s => Globals.SpotifyUris.Add(s));
            var sb = new StringBuilder(8192);
            spotifyLinks.ForEach(s => sb.AppendLine(s));
            string links = sb.ToString();
            int i2 = 0;
            spotifyMatches.ForEach(s => resultsList.Items.Add(s));
            try
            {
                resultsList.SelectedIndex = 0;
            }
            catch
            {
            }
            using (StringReader reader = new StringReader(links))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Globals.LinksFinal[i2] = line;
                        i2++;
                    }
                }
                while (line != null);
                if (resultsList.Items.Count != 0)
                {
                    download.IsEnabled = true;
                    authenticate.Visibility = Visibility.Visible;
                    download.Content = "Open";
                }

            }
            App.Current.MainWindow.Show();
            FreeConsole();
        }
        public string BeatSaberMatching(List<string> titles, List<string> artists)
        {
            var json = string.Empty;
            var beatMatches = new List<string>();
            var beatLinks = new List<string>();
            Console.Clear();
            Console.WriteLine("Searching Beat Saber songs... ");
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                var titleURL = title.Replace(" ", "%20");
                var artist = string.Empty;
                bool containsArtist = false;
                if (artists != null)
                {
                    containsArtist = true;
                    artist = artists[i];
                }
                else
                {
                }

                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers.Add("User-Agent: Other"); // done to avoid 403 errors
                        json = wc.DownloadString("https://beatsaver.com/api/search/text/:page?q=" + titleURL);
                    }
                }
                catch
                {
                    continue;
                }
                if (json.Length < 100)
                {
                    Console.Clear();
                    Console.WriteLine("Searching Beat Saber songs... " + i + "/" + titles.Count);
                    continue;
                }
                JObject songSearch = JObject.Parse(json);
                IList<JToken> results = songSearch["docs"].Children().ToList();
                IList<BeatSong> songs = new List<BeatSong>();
                foreach (JToken result in results)
                {
                    BeatSong song = result.ToObject<BeatSong>();
                    if (song.Metadata.SongName.ToUpper() == title.ToUpper())
                    {
                        if (containsArtist == true)
                        {
                            if (song.Metadata.SongAuthorName.ToUpper() == artist.ToUpper())
                            {
                                beatMatches.Add(song.Metadata.SongName + " - Created by " + song.Metadata.LevelAuthorName);
                                beatLinks.Add("https://beatsaver.com/beatmap/" + song.Key);
                            }
                        }
                        else
                        {
                            beatMatches.Add(song.Metadata.SongName + " - Created by " + song.Metadata.LevelAuthorName);
                            beatLinks.Add("https://beatsaver.com/beatmap/" + song.Key);
                        }
                    }
                }
                Console.Clear();
                Console.WriteLine("Searching Beat Saber songs... " + i + "/" + titles.Count);
            }
            var sb = new StringBuilder(4096);
            beatMatches.ForEach(s => resultsList.Items.Add(s));
            try
            {
                resultsList.SelectedIndex = 0;
            }
            catch
            {
            }
            beatLinks.ForEach(s => sb.AppendLine(s));
            return sb.ToString();
        }
        public string ChunithmMatching(List<string> titles, List<string> titlesUni, List<string> artists)
        {
            var json = string.Empty;
            var json2 = string.Empty;
            var chunithmMatches = new List<string>();
            if (titlesUni == null)
            {
                MessageBox.Show("These results may be inaccurate because the source you selected does not contain Unicode titles.", "Notice");
            }
            Console.Clear();
            Console.WriteLine("Searching Chunithm songs... ");
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString("https://chunithm.sega.jp/data/common.json");
                // only has 36 songs cant be bothered to write code for it /shrug 
                // json2 = wc.DownloadString("https://chunithm.sega.jp/data/course.json");
            }
            JArray chunithmSongs = JArray.Parse(json);
            IList<JToken> chunithmSongsList = chunithmSongs.Children().ToList();
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                bool containsUni = false;
                var titleUnicode = string.Empty;
                if (titlesUni != null)
                {
                    containsUni = true;
                    titleUnicode = titlesUni[i];
                }
                else
                {
                    titleUnicode = "NOT AVAILABLE";
                }
                string artist = artists[i];
                foreach (JToken result in chunithmSongsList)
                {
                    ChunSong song = result.ToObject<ChunSong>();
                    if ((song.Title.ToUpper() == title.ToUpper()) || song.Reading.ToUpper() == title.ToUpper())
                    {
                        if (song.Artist.ToUpper() == artist.ToUpper())
                        {
                            chunithmMatches.Add(song.Title + " by " + song.Artist);
                        }
                        else
                        {
                            
                        }
                    }
                    else if (containsUni == true)
                    {
                        if ((song.Title.ToUpper() == titleUnicode.ToUpper()) || song.Reading.ToUpper() == titleUnicode.ToUpper())
                        {
                            if (song.Artist.ToUpper() == artist.ToUpper())
                            {
                                chunithmMatches.Add(song.Title + " by " + song.Artist);
                            }
                            else
                            {

                            }
                        }
                    }                    
                }

                Console.WriteLine("Searching Chunithm songs... " + i + "/" + titles.Count);
            }
            var sb = new StringBuilder(4096);
            chunithmMatches.Sort();
            chunithmMatches.ForEach(s => sb.AppendLine(s));
            App.Current.MainWindow.Show();
            FreeConsole();
            resultsList.Visibility = Visibility.Hidden;
            if (sb.Length == 0)
            {
                sb.AppendLine("No matches :(");
                results.FontSize = 50;
            }
            return sb.ToString();
        }
        public string OngekiMatching(List<string> titles, List<string> titlesUni, List<string> artists)
        {
            var json = string.Empty;
            var ongekiMatches = new List<string>();
            if (titlesUni == null)
            {
                MessageBox.Show("These results may be inaccurate because the source you selected does not contain Unicode titles.", "Notice");
            }
            Console.Clear();
            Console.WriteLine("Searching Ongeki songs... ");
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString("https://ongeki.sega.jp/assets/json/music/music.json");
            }
            JArray ongekiSongs = JArray.Parse(json);
            IList<JToken> ongekiSongsList = ongekiSongs.Children().ToList();
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                bool containsUni = false;
                var titleUnicode = string.Empty;
                if (titlesUni != null)
                {
                    containsUni = true;
                    titleUnicode = titlesUni[i];
                }
                else
                {
                    titleUnicode = "NOT AVAILABLE";
                }
                string artist = artists[i];
                foreach (JToken result in ongekiSongsList)
                {
                    OngekiSong song = result.ToObject<OngekiSong>();
                    if ((song.Title.ToUpper() == title.ToUpper()) || song.Title_Sort.ToUpper() == title.ToUpper())
                    {
                        if (song.Artist.ToUpper() == artist.ToUpper())
                        {
                            ongekiMatches.Add(song.Title + " by " + song.Artist);
                        }
                        else
                        {

                        }
                    }
                    else if (containsUni == true)
                    {
                        if ((song.Title.ToUpper() == titleUnicode.ToUpper()) || song.Title_Sort.ToUpper() == titleUnicode.ToUpper())
                        {
                            if (song.Artist.ToUpper() == artist.ToUpper())
                            {
                                ongekiMatches.Add(song.Title + " by " + song.Artist);
                            }
                            else
                            {

                            }
                        }
                    }
                }

                Console.WriteLine("Searching Ongeki songs... " + i + "/" + titles.Count);
            }
            var sb = new StringBuilder(4096);
            ongekiMatches.Sort();
            ongekiMatches.ForEach(s => sb.AppendLine(s));
            App.Current.MainWindow.Show();
            FreeConsole();
            resultsList.Visibility = Visibility.Hidden;
            if (sb.Length == 0)
            {
                sb.AppendLine("No matches :(");
                results.FontSize = 50;
            }
            return sb.ToString();
        }
        public string DivaMatching(List<string> titles, List<string> titlesUni, List<string> artists)
        {
            var matches = new List<string>();
            Console.Clear();
            Console.WriteLine("Fetching Project Diva song list...");
            string divaPage = ScrapePage("https://miku.sega.com/futuretone/songs.html", true).ToUpper();
            Console.Clear();
            Console.WriteLine("Finding matches...");
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                var titleUnicode = string.Empty;
                bool containsUnicode = false;
                if (titlesUni != null)
                {
                    titleUnicode = titlesUni[i];
                    containsUnicode = true;
                }
                var artist = string.Empty;
                if (artists != null)
                {
                    artist = artists[i];
                }
                string songBracket = "ALT=\"" + title.ToUpper() + "\"";
                string songBracketUnicode = string.Empty;
                if (divaPage.Contains(songBracket))
                {
                    matches.Add(title + " - by " + artist);
                }
                else if (containsUnicode == true)
                {
                    songBracketUnicode = "ALT=\"" + titlesUni[i].ToUpper() + "\"";
                    if (divaPage.Contains(songBracketUnicode))
                    {
                        matches.Add(titleUnicode + " - (" + title + ") by " + artist);
                    }
                }
                Console.WriteLine("Finding matches... " + i + "/" + titles.Count);
            }
            var sb = new StringBuilder(4096);
            matches.Sort();
            matches.ForEach(s => sb.AppendLine(s));
            App.Current.MainWindow.Show();
            FreeConsole();
            resultsList.Visibility = Visibility.Hidden;
            if (sb.Length == 0)
            {
                sb.AppendLine("No matches :(");
                results.FontSize = 50;
            }
            return sb.ToString();
        }
        public string ArcaeaMatching(List<string> titles, List<string> titlesUni, List<string> artists)
        {
            var matches = new List<string>();
            Console.Clear();
            Console.WriteLine("Fetching Arcaea song list...");
            string arcaeaPage = ScrapePage("https://arcaea.fandom.com/wiki/Songs_by_Date", true).ToUpper();
            Console.Clear();
            Console.WriteLine("Finding matches...");
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                var titleUnicode = string.Empty;
                bool containsUnicode = false;
                if (titlesUni != null)
                {
                    titleUnicode = titlesUni[i];
                    containsUnicode = true;
                }
                var artist = string.Empty;
                if (artists != null)
                {
                    artist = artists[i];
                }
                string songBracket = "TITLE=\"" + title.ToUpper() + "\"";
                string songBracketUnicode = string.Empty;
                if (arcaeaPage.Contains(songBracket))
                {
                    matches.Add(title + " - by " + artist);
                }
                else if (containsUnicode == true)
                {
                    songBracketUnicode = "TITLE=\"" + titlesUni[i].ToUpper() + "\"";
                    if (arcaeaPage.Contains(songBracketUnicode))
                    {
                        matches.Add(titleUnicode + " - (" + title + ") by " + artist);
                    }
                }
                Console.WriteLine("Finding matches... " + i + "/" + titles.Count);
            }
            var sb = new StringBuilder(4096);
            matches.Sort();
            matches.ForEach(s => sb.AppendLine(s));
            App.Current.MainWindow.Show();
            FreeConsole();
            resultsList.Visibility = Visibility.Hidden;
            if (sb.Length == 0)
            {
                sb.AppendLine("No matches :(");
                results.FontSize = 50;
            }
            return sb.ToString();
        }
        public string WaccaMatching(List<string> titles, List<string> titlesUni, List<string> artists)
        {
            var matches = new List<string>();
            Console.Clear();
            if (titlesUni == null)
            {
                MessageBox.Show("These results may be inaccurate because the source you selected does not contain Unicode titles.", "Notice");
            }
            Console.WriteLine("Fetching WACCA song list...");
            var client = new RestClient("https://wacca.marv.jp");
            var request = new RestRequest("music/search.php", RestSharp.DataFormat.Json);
            request.AddParameter("cat", "all");
            var response = client.Post(request);
            JArray waccaSongs = JArray.Parse(response.Content);
            IList<JToken> waccaSongsList = waccaSongs.Children().ToList();
            string waccaPage = response.Content;
            Console.Clear();
            Console.WriteLine("Finding matches...");
            for (var i = 0; i < titles.Count; i++)
            {
                var title = titles[i];
                var titleUnicode = string.Empty;
                bool containsUnicode = false;
                if (titlesUni != null)
                {
                    titleUnicode = titlesUni[i];
                    containsUnicode = true;
                }
                var artist = string.Empty;
                if (artists != null)
                {
                    artist = artists[i];
                }
                foreach (JToken result in waccaSongsList)
                {
                    WaccaSong song = result.ToObject<WaccaSong>();
                    if ((song.Title.Display.ToUpper() == title.ToUpper()) || song.Title.Ruby.ToUpper() == title.ToUpper())
                    {
                        matches.Add(title + " by " + song.Artist.Display + " (" + song.Artist.Ruby + ")");
                    }
                    else if (containsUnicode == true)
                    {
                        if ((song.Title.Display.ToUpper() == titleUnicode.ToUpper()) || song.Title.Ruby.ToUpper() == titleUnicode.ToUpper())
                        {
                            matches.Add(titleUnicode + " (" + title + ") by " + song.Artist.Display + " (" + song.Artist.Ruby + ")");
                        }
                    }
                }
                Console.WriteLine("Finding matches... " + i + "/" + titles.Count);
            }
            var sb = new StringBuilder(4096);
            matches.Sort();
            matches.ForEach(s => sb.AppendLine(s));
            App.Current.MainWindow.Show();
            FreeConsole();
            resultsList.Visibility = Visibility.Hidden;
            if (sb.Length == 0)
            {
                sb.AppendLine("No matches :(");
                results.FontSize = 50;
            }
            return sb.ToString();
        }
    }
}
