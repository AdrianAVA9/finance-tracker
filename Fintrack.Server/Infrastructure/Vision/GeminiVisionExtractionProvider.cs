using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Fintrack.Server.Application.Abstractions.Vision;
using Fintrack.Server.Application.DTOs.Vision;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fintrack.Server.Infrastructure.Vision;

public class GeminiVisionExtractionProvider : IVisionExtractionProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiVisionExtractionProvider> _logger;

    public GeminiVisionExtractionProvider(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiVisionExtractionProvider> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ExtractedReceiptData> ExtractReceiptDataAsync(
        Stream imageStream, 
        string mimeType, 
        IEnumerable<string> validCategories, 
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Gemini API key is not configured.");
        }

        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream, cancellationToken);
        var base64Image = Convert.ToBase64String(memoryStream.ToArray());

        var categoriesList = string.Join(", ", validCategories.Select(c => $"\"{c}\""));

        var prompt = $@"
You are an expert receipt and invoice data extractor. 
I am providing an image of a receipt or invoice. Please extract the data into a structured JSON format.

CRITICAL INSTRUCTION:
You MUST categorize the overall expense and EACH line item using ONLY one of the following exact categories: [{categoriesList}]. 
Do NOT use any other categories. If you are unsure, pick the closest one from the list. If none are close, choose the most generic one from the list.

The output MUST be pure JSON matching this exact structure, with no markdown formatting or backticks:
{{
  ""Invoice"": {{
    ""MerchantName"": ""string"",
    ""Date"": ""YYYY-MM-DDTHH:MM:SS"",
    ""TotalAmount"": 0.0,
    ""InvoiceNumber"": ""string""
  }},
  ""Expense"": {{
    ""Merchant"": ""string"",
    ""Date"": ""YYYY-MM-DDTHH:MM:SS"",
    ""TotalAmount"": 0.0,
    ""OverallCategory"": ""string"",
    ""InvoiceNumber"": ""string""
  }},
  ""LineItems"": [
    {{
      ""Description"": ""string"",
      ""Quantity"": 0,
      ""UnitPrice"": 0.0,
      ""TotalPrice"": 0.0,
      ""Category"": ""string""
    }}
  ]
}}
";

        var requestPayload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new { text = prompt },
                        new 
                        { 
                            inline_data = new 
                            { 
                                mime_type = mimeType, 
                                data = base64Image 
                            } 
                        }
                    }
                }
            },
            generationConfig = new
            {
                response_mime_type = "application/json"
            }
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, 
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}")
        {
            Content = JsonContent.Create(requestPayload)
        };

        var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Gemini API error: {StatusCode} {ErrorContent}", response.StatusCode, errorContent);
            throw new Exception($"Gemini API query failed with status {response.StatusCode}");
        }

        var responseData = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        
        try
        {
            var textContent = responseData
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(textContent))
            {
                 throw new Exception("Gemini returned empty text.");
            }

            // Sometimes models return formatting despite instructions, so we clean it up
            textContent = textContent.Trim();
            if (textContent.StartsWith("```json"))
            {
                textContent = textContent.Substring(7);
                if (textContent.EndsWith("```"))
                {
                    textContent = textContent.Substring(0, textContent.Length - 3);
                }
            }
            else if (textContent.StartsWith("```"))
            {
                textContent = textContent.Substring(3);
                if (textContent.EndsWith("```"))
                {
                    textContent = textContent.Substring(0, textContent.Length - 3);
                }
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var extractedData = JsonSerializer.Deserialize<ExtractedReceiptData>(textContent, options);

            return extractedData ?? new ExtractedReceiptData();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse Gemini API response.");
            throw new Exception("Failed to parse the receipt data from the AI response.", ex);
        }
    }
}
