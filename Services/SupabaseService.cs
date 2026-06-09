using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BloemFinder.API.Services;

public class SupabaseService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly JsonSerializerOptions _jsonOptions;

    public SupabaseService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;

        _httpClient.BaseAddress = new Uri(_config["Supabase:Url"] ?? throw new InvalidOperationException("Supabase:Url missing"));
        _httpClient.DefaultRequestHeaders.Add("apikey", _config["Supabase:AnonKey"]);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config["Supabase:ServiceRoleKey"]}");

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    // Properties
    public async Task<List<object>> GetPropertiesAsync()
    {
        var response = await _httpClient.GetAsync("rest/v1/properties?select=*");
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(content) || content == "[]")
            return new List<object>();

        return JsonSerializer.Deserialize<List<object>>(content, _jsonOptions) ?? new List<object>();
    }

    public async Task<object?> GetPropertyByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"rest/v1/properties?id=eq.{id}&select=*,rooms(*)");
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(content) || content == "[]")
            return null;

        var list = JsonSerializer.Deserialize<List<object>>(content, _jsonOptions);
        return list?.FirstOrDefault();
    }

    public async Task<bool> CreatePropertyAsync(object property)
    {
        var json = JsonSerializer.Serialize(property, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PostAsync("rest/v1/properties", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdatePropertyAsync(Guid id, object updates)
    {
        var json = JsonSerializer.Serialize(updates, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PatchAsync($"rest/v1/properties?id=eq.{id}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeletePropertyAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"rest/v1/properties?id=eq.{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<object>> GetPendingPropertiesAsync()
    {
        var response = await _httpClient.GetAsync("rest/v1/properties?select=*&is_verified=eq.false");
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(content) || content == "[]")
            return new List<object>();

        return JsonSerializer.Deserialize<List<object>>(content, _jsonOptions) ?? new List<object>();
    }

    // Rooms
    public async Task<bool> CreateRoomAsync(object room)
    {
        var json = JsonSerializer.Serialize(room, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PostAsync("rest/v1/rooms", content);
        return response.IsSuccessStatusCode;
    }

    // Reviews
    public async Task<List<object>> GetReviewsByPropertyAsync(Guid propertyId)
    {
        var response = await _httpClient.GetAsync($"rest/v1/reviews?select=*&property_id=eq.{propertyId}&order=created_at.desc");
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(content) || content == "[]")
            return new List<object>();

        return JsonSerializer.Deserialize<List<object>>(content, _jsonOptions) ?? new List<object>();
    }

    public async Task<bool> CreateReviewAsync(object review)
    {
        var json = JsonSerializer.Serialize(review, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PostAsync("rest/v1/reviews", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteReviewAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"rest/v1/reviews?id=eq.{id}");
        return response.IsSuccessStatusCode;
    }

    // Favorites
    public async Task<List<object>> GetFavoritesByUserAsync(string userId)
    {
        var response = await _httpClient.GetAsync($"rest/v1/favorites?select=*,properties(*)&user_id=eq.{userId}");
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(content) || content == "[]")
            return new List<object>();

        return JsonSerializer.Deserialize<List<object>>(content, _jsonOptions) ?? new List<object>();
    }

    public async Task<bool> AddFavoriteAsync(object favorite)
    {
        var json = JsonSerializer.Serialize(favorite, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PostAsync("rest/v1/favorites", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveFavoriteAsync(string userId, Guid propertyId)
    {
        var response = await _httpClient.DeleteAsync($"rest/v1/favorites?user_id=eq.{userId}&property_id=eq.{propertyId}");
        return response.IsSuccessStatusCode;
    }

    // Reports
    public async Task<bool> CreateReportAsync(object report)
    {
        var json = JsonSerializer.Serialize(report, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PostAsync("rest/v1/reports", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<object>> GetReportsAsync()
    {
        var response = await _httpClient.GetAsync("rest/v1/reports?select=*,properties(name)&order=created_at.desc");
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(content) || content == "[]")
            return new List<object>();

        return JsonSerializer.Deserialize<List<object>>(content, _jsonOptions) ?? new List<object>();
    }
}