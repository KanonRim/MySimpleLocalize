using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace KoroBox.MySimpleLocalization
{
    public class LocalizationManager
    {
        public const string LocalizationFileListName = "LocalizationFileList.txt";

        private static bool _initialized = false;
        private static LocalizationManager _instance;
        public static LocalizationManager Instance => _instance ??= new LocalizationManager();
        public event Action OnLanguageChanged;

        private Dictionary<string, string> _localizedTexts;
        private string _currentLanguage = "en";
        private static IEnumerable<string> _supportedLanguages;

        public bool IsReady => _localizedTexts != null && _localizedTexts.Count > 0;

        public string CurrentLanguage => _currentLanguage;

        private LocalizationManager()
        {
        }

        public IEnumerator Initialize(string language = "en")
        {
            if (_initialized)
                yield break;
            _initialized = true;
            yield return LoadSupportedLanguages();
            yield return LoadLanguage(language);
        }

        public void ChangeToNextLanguage()
        {
            var list = _supportedLanguages.ToList();
            int currentIndex = list.IndexOf(_currentLanguage);
            int nextIndex = (currentIndex + 1) % _supportedLanguages.Count();
            LoadLanguage(list[nextIndex]);
        }

        public IEnumerator LoadLanguage(string languageCode)
        {
            string filePath = Path.Combine("Localization", $"{languageCode}.json");
            return StreamingAssetsHelper.ReadFile(filePath, (data) =>
            {
                string jsonData = data.ToString(); //todo EmtyFile
                _localizedTexts = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
                _currentLanguage = languageCode;
                OnLanguageChanged?.Invoke();
            });
        }

        public static IEnumerator LoadSupportedLanguages([CanBeNull] Action<IEnumerable<string>> callback = null)
        {
            if (_supportedLanguages == null || !_supportedLanguages.Any())
            {
                yield return StreamingAssetsHelper.ReadFile("Localization" + "/" + LocalizationFileListName, (text) =>
                {
                    _supportedLanguages =
                        text?.Split('\n')
                            .Where(line => line.Trim().EndsWith(".json"))
                            .Select(line => line.Trim().Replace(".json", ""));
                });
            }

            callback?.Invoke(_supportedLanguages);
        }

        public string GetLocalizedText(string key)
        {
            if (_localizedTexts == null)
            {
                throw new Exception("No localized text was loaded");
            }

            if (_localizedTexts.TryGetValue(key, out string value))
            {
                return value;
            }
            else
            {
#if UNITY_EDITOR
                _localizedTexts.Add(key, string.Empty);
                string filePath =
                    StreamingAssetsHelper.GetFilePath(Path.Combine("Localization", $"{_currentLanguage}.json"));
                File.WriteAllText(filePath, JsonConvert.SerializeObject(_localizedTexts));
#endif
                return key;
            }
        }

        public static string G(string key)
        {
            return Instance.GetLocalizedText(key);
        }

        public string this[string key] => G(key);
    }
}