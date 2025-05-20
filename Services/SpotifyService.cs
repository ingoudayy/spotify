using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<List<SpotifyTrack>> SearchTracksAsync(string query)
        {
            var searchResults = new List<SpotifyTrack>();

            if (_spotify == null)
                await InitializeAsync();

            var searchRequest = new SearchRequest(SearchRequest.Types.Track | SearchRequest.Types.Artist, query)
            {
                Limit = 10
            };

            var searchResponse = await _spotify.Search.Item(searchRequest);

            foreach (var track in searchResponse.Tracks.Items)
            {
                searchResults.Add(new SpotifyTrack
                {
                    TrackName = track.Name,
                    ArtistName = track.Artists[0].Name,
                    SpotifyLink = track.ExternalUrls["spotify"],
                    ImageUrl = track.Album.Images.Count > 0 ? track.Album.Images[0].Url : string.Empty,
                    Popularity = track.Popularity,
                    DurationMs = track.DurationMs
                });
            }

            return searchResults;
        }
    }

    public class SpotifyTrack
    {
        public string TrackName { get; set; }
        public string ArtistName { get; set; }
        public string ImageUrl { get; set; }
        public string SpotifyLink { get; set; }

        // ✅ Améliorations UX/UI
        public int Popularity { get; set; }
        public int DurationMs { get; set; }
    }
}
