using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web;

public class YouTubeService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public YouTubeService(HttpClient http)
    {
        _http = http;
        _apiKey = "AIzaSyCPBO1-h1KvRJ6CBUrzbATiJjeKHJO1Gm0";
    }

    public async Task<List<string>> SearchVideoIdsAsync(string query, int maxResults = 5)
    {
        var url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={HttpUtility.UrlEncode(query)}&key={_apiKey}&maxResults={maxResults}&type=video";
        var result = await _http.GetFromJsonAsync<YouTubeSearchResult>(url);
        return result?.Items?.Select(i => i.Id.VideoId).ToList() ?? new();
    }

    private class YouTubeSearchResult
    {
        public List<Item> Items { get; set; }

        public class Item
        {
            public Id Id { get; set; }
        }

        public class Id
        {
            public string VideoId { get; set; }
        }
    }
}
