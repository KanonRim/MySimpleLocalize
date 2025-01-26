using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KoroBox.MySimpleLocalization.Editor
{
    public class GoogleTranslator : ITranslator
    {
        private readonly HttpClient _httpClient;

        public GoogleTranslator(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<string> TranslateTextAsync(string text, string targetLanguage, string currentLanguage = "")
        {
            try
            {
                string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={currentLanguage}&tl={targetLanguage}&dt=t&q={Uri.EscapeDataString(text)}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to fetch translation. Status code: {response.StatusCode}");
                }
                string result = await response.Content.ReadAsStringAsync();
                var jsonData = Newtonsoft.Json.Linq.JArray.Parse(result);
                string translation  = jsonData[0][0][0].ToString(); 
                translation = translation.Trim();

                return translation;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Error making HTTP request: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Error parsing JSON response: {ex.Message}", ex);
            }
        }
    }
}