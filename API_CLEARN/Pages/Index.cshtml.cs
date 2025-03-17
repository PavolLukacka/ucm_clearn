using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

public class IndexModel : PageModel
{
    private readonly ChatGptService _chatGptService;

    // Thread-safe dictionary to store last request time by IP address
    private static ConcurrentDictionary<string, DateTime> _lastRequestTimes = new ConcurrentDictionary<string, DateTime>();

    public string ChatGptResponse { get; set; }
    public string UzivatelskaOtazka { get; set; }
    public string DostupneModely { get; set; } // Možnosť budúceho výberu modelu

    public IndexModel(ChatGptService chatGptService)
    {
        _chatGptService = chatGptService;
    }

    public async Task<IActionResult> OnPostAsync(string prompt)
    {
        // Získanie IP adresy klienta
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Kontrola cooldown: ak posledný request od tejto IP je mladší ako 5 minút, zobraz chybovú správu.
        if (_lastRequestTimes.TryGetValue(ipAddress, out var lastRequestTime))
        {
            if (DateTime.UtcNow - lastRequestTime < TimeSpan.FromMinutes(5))
            {
                ModelState.AddModelError(string.Empty, "Príliš rýchle požiadavky. Prosím, počkajte 5 minút pred ďalším odoslaním.");
                return Page();
            }
        }

        // Aktualizácia času posledného requestu pre danú IP adresu
        _lastRequestTimes[ipAddress] = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(prompt))
        {
            UzivatelskaOtazka = prompt;
            ChatGptResponse = await _chatGptService.GetResponseAsync(prompt);
        }
        return Page();
    }
}