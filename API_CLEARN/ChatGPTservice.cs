using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;


public class ChatGptService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public ChatGptService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"];
        _httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", "org-pjEd4Tre5pFUo3609tC5bXAc");
    }

    public async Task<string> GetResponseAsync(string prompt)
    {
        // Poskytnutie systémovej správy na nastavenie brainroty, zábavného štýlu
        var requestBody = new
        {
            model = "gpt-4",
            messages = new[]
            {
                new { role = "system", content = "Si Brainrot: excentrický, cool a neformálny lektor C programovania, ktorý používa kreatívne analógie a hravý jazyk. Tvoj štýl je nekonvenčný a inšpiratívny." },
                new { role = "user", content = prompt }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        _httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", "org-pjEd4Tre5pFUo3609tC5bXAc");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        // Parsovanie JSON-u a extrakcia iba textovej odpovede
        using var jsonDoc = JsonDocument.Parse(responseString);
        var answer = jsonDoc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return answer;
    }
}