using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KoroBox.MySimpleLocalization.Editor
{
    public class LocalizationTranslator
    {
        private readonly ITranslator _translator;

        public LocalizationTranslator(ITranslator translator)
        {
            _translator = translator;
        }

        public async Task TranslateLocalizationFileAsync(string inputFilePath, string outputFilePath,
            string targetLanguage, string currentLanguage)
        {
            if (!File.Exists(inputFilePath))
                throw new FileNotFoundException($"Файл {inputFilePath} не найден.");

            // Чтение содержимого исходного файла
            var jsonContent = await File.ReadAllTextAsync(inputFilePath);
            var localizationData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

            if (localizationData == null)
                throw new InvalidOperationException("Не удалось загрузить данные локализации из JSON файла.");

            // Перевод каждого текста
            var translatedData = new Dictionary<string, string>();
            foreach (var entry in localizationData)
            {
                Console.WriteLine($"Перевод ключа: {entry.Key}...");
                string translatedText = await _translator.TranslateTextAsync(entry.Value, targetLanguage,currentLanguage);
                translatedData[entry.Key] = translatedText;
            }

            // Сохранение переведённых данных
            var translatedJsonContent = JsonConvert.SerializeObject(translatedData, Formatting.Indented);
            await File.WriteAllTextAsync(outputFilePath, translatedJsonContent);

            Console.WriteLine($"Файл {outputFilePath} успешно создан.");
        }
    }
}