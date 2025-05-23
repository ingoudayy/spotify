using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Linq;
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
            if (_spotify != null) return;

            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(_options.ClientId, _options.ClientSecret);
            var token = await new OAuthClient(config).RequestToken(request);
            _spotify = new SpotifyClient(config.WithToken(token.AccessToken));
        }

        public async Task<List<SpotifyTrack>> SearchTracksAsync(string query, string filterType = "track")
        {
            await InitializeAsync();
            var searchResults = new List<SpotifyTrack>();

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

            if (searchResponse.Tracks != null)
            {
                foreach (var track in searchResponse.Tracks.Items)
                {
                    searchResults.Add(new SpotifyTrack
                    {
                        TrackName = track.Name,
                        ArtistName = track.Artists.FirstOrDefault()?.Name ?? "",
                        SpotifyLink = track.ExternalUrls["spotify"],
                        ImageUrl = track.Album.Images.FirstOrDefault()?.Url ?? "",
                        Popularity = track.Popularity,
                        DurationMs = track.DurationMs
                    });
                }
            }

            return searchResults;
        }

        public async Task<List<SpotifyTrack>> GetTopTracksAsync()
        {
            await InitializeAsync();

            var playlistId = "37i9dQZF1DXcBWIGoYBM5M"; // "Today's Top Hits"
            var playlistItems = await _spotify.Playlists.GetItems(playlistId, new PlaylistGetItemsRequest { Limit = 10 });

            var tracks = new List<SpotifyTrack>();

            foreach (var item in playlistItems.Items)
            {
                if (item.Track is FullTrack track)
                {
                    tracks.Add(new SpotifyTrack
                    {
                        TrackName = track.Name,
                        ArtistName = track.Artists.FirstOrDefault()?.Name ?? "",
                        SpotifyLink = track.ExternalUrls["spotify"],
                        ImageUrl = track.Album.Images.FirstOrDefault()?.Url ?? "",
                        Popularity = track.Popularity,
                        DurationMs = track.DurationMs
                    });
                }
            }

            return tracks;
        }

        public async Task<List<SpotifyTrack>> GetTopArtistsAsync()
        {
            await InitializeAsync();

            var result = await _spotify.Browse.GetCategoryPlaylists("toplists", new CategoriesPlaylistsRequest { Country = "FR" });

            var artists = new List<SpotifyTrack>();
            var playlistId = result.Playlists.Items.FirstOrDefault()?.Id;

            if (!string.IsNullOrEmpty(playlistId))
            {
                var playlist = await _spotify.Playlists.GetItems(playlistId, new PlaylistGetItemsRequest { Limit = 10 });

                foreach (var item in playlist.Items)
                {
                    if (item.Track is FullTrack track)
                    {
                        var artist = track.Artists.FirstOrDefault();
                        if (artist != null && !artists.Any(a => a.Id == artist.Id))
                        {
                            var artistDetails = await _spotify.Artists.Get(artist.Id);
                            artists.Add(new SpotifyTrack
                            {
                                Id = artist.Id,
                                Name = artist.Name,
                                SpotifyLink = artist.ExternalUrls["spotify"],
                                ImageUrl = artistDetails.Images.FirstOrDefault()?.Url ?? ""
                            });
                        }
                    }
                }
            }

            return artists;
        }
    }
}