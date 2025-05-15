using SpotifyAPI.Web;

namespace SpotifyTopSongsApp.Services
{
    public class SpotifyService
    {
        private readonly SpotifyOptions _options;
        private SpotifyClient _spotify;

        public SpotifyService(SpotifyOptions options)
        {
            _options = options;
        }

        private async Task InitializeAsync()
        {
            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(_options.ClientId, _options.ClientSecret);
            var token = await new OAuthClient(config).RequestToken(request);
            _spotify = new SpotifyClient(config.WithToken(token.AccessToken));
        }

        public async Task<List<(string Title, string Artist, int Popularity, string ReleaseDate)>> GetTopTracksAsync()
        {
            try
            {
                if (_spotify == null)
                    await InitializeAsync();

                var playlistId = "37i9dQZF1DXcBWIGoYBM5M"; 
                var page = await _spotify.Playlists.GetItems(playlistId, new PlaylistGetItemsRequest { Limit = 50 });

                var allTracks = new List<FullTrack>();

                do
                {
                    var currentTracks = page.Items
                        .Where(item => item.Track is FullTrack)
                        .Select(item => item.Track as FullTrack)
                        .ToList();

                    allTracks.AddRange(currentTracks);

                    if (page.Next != null)
                        page = await _spotify.NextPage(page);
                    else
                        break;

                } while (page.Items.Count > 0);

                Console.WriteLine($"{allTracks.Count} morceaux extraits de la playlist.");

                var tracks = allTracks
                    .OrderByDescending(t => t.Popularity)
                    .Select(track => (
                        track.Name,
                        string.Join(", ", track.Artists.Select(a => a.Name)),
                        track.Popularity,
                        track.Album.ReleaseDate
                    ))
                    .ToList();

                return tracks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" ERREUR API Spotify : {ex.Message}");
                return new();
            }
        }
    }
}
