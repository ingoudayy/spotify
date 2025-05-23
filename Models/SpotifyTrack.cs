namespace SpotifyTopSongsApp.Models
{
    public class SpotifyTrack
    {
        public string TrackName { get; set; }
        public string ArtistName { get; set; }
        public string ImageUrl { get; set; }
        public string SpotifyLink { get; set; }
        public int Popularity { get; set; }
        public int DurationMs { get; set; }
        public string AudioUrl { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public SpotifyTrack() { } 
    }
}
