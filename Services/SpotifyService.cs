using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyTopSongsApp.Models;

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

        public async Task<List<SpotifyTrack>> SearchTracksAsync(string query, string filterType = "track")
        {
            var searchResults = new List<SpotifyTrack>();

            if (_spotify == null)
                await InitializeAsync();

            var type = filterType switch
            {
                "artist" => SearchRequest.Types.Artist,
                "album" => SearchRequest.Types.Album,
                _ => SearchRequest.Types.Track
            };

            var searchRequest = new SearchRequest(type, query)
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
}
