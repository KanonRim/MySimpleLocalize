using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace KoroBox.MySimpleLocalization.Editor
{
    public class LibreTranslate : ITranslator
    {
        private readonly string _apiUrl;
        private readonly HttpClient _httpClient;

        public LibreTranslate(string apiUrl)
        {
            _apiUrl = apiUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

        public async Task<string> TranslateTextAsync(string text, string targetLanguage, string sourceLanguage ="")
        {
            var requestBody = new
            {
                q = text,
                source = string.IsNullOrEmpty(sourceLanguage) ? "auto" : sourceLanguage,
                target = targetLanguage,
                format = "text"
            };

            var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}/translate", content);
            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"Ошибка: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                throw new Exception("Ошибка при запросе к LibreTranslate API.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

            if (responseData != null && responseData.ContainsKey("translatedText"))
            {
                return responseData["translatedText"];
            }

            throw new Exception("Ошибка при обработке ответа от LibreTranslate API.");
        }
    }
}