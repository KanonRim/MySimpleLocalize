using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using KoroBox.MySimpleLocalization;

namespace KoroBox.MySimpleLocalization.Editor
{
    public class LocalizationSettingsWindow : EditorWindow
    {
        private string _targetLanguage = "en";
        private string _baseLanguage = "ru";
        private int _translatorTypeIndex = 0;
        private string _libreTranslateUrl = "http://localhost:5000";
        private string _sourceDirectory = Application.dataPath + "/StreamingAssets/Localization";
        private readonly string[] _translatorOptions = { "GoogleTranslator", "LibreTranslate" };

        [MenuItem("Tools/Localization/Settings")]
        public static void ShowWindow()
        {
            GetWindow<LocalizationSettingsWindow>("Localization Settings");
        }

        private void OnGUI()
        {
            GUILayout.Label("Localization Settings", EditorStyles.boldLabel);

            _targetLanguage = EditorGUILayout.TextField("Target Language (e.g., en)", _targetLanguage);
            _baseLanguage = EditorGUILayout.TextField("Base Language (e.g., ru)", _baseLanguage);

            _translatorTypeIndex = EditorGUILayout.Popup("Translator Type", _translatorTypeIndex, _translatorOptions);

            if (_translatorTypeIndex == 1) // LibreTranslate
            {
                _libreTranslateUrl = EditorGUILayout.TextField("LibreTranslate URL", _libreTranslateUrl);
            }

            if (GUILayout.Button("Translate"))
            {
                TranslateFiles(_targetLanguage, _baseLanguage);
            }

            if (GUILayout.Button("Translate Missing Keys"))
            {
                TranslateMissingKeysAsync(_targetLanguage, _baseLanguage);
            }
            
            if (GUILayout.Button("Generate File List"))
            {
                GenerateFileList();
            }
        }

        private static void GenerateFileList()
        {
            string localizationPath = Path.Combine(Application.streamingAssetsPath, "Localization");
            if (!Directory.Exists(localizationPath))
            {
                Debug.LogError($"Localization directory not found: {localizationPath}");
                return;
            }

            string[] files = Directory.GetFiles(localizationPath, "*.json");
            string fileListPath = Path.Combine(localizationPath, LocalizationManager.LocalizationFileListName);

            File.WriteAllLines(fileListPath, files.Select(Path.GetFileName));

            Debug.Log($"File list generated at {fileListPath}");
            AssetDatabase.Refresh();
        }

        private async void TranslateFiles(string targetLanguage, string baseLanguage)
        {
            var translator = GetLocalizationTranslator();
            try
            {
                await translator.TranslateLocalizationFileAsync(
                    Path.Combine(_sourceDirectory, baseLanguage + ".json"),
                    Path.Combine(_sourceDirectory, targetLanguage + ".json"),
                    targetLanguage,
                    baseLanguage
                );

                Debug.Log("Translation of all files completed!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}");
            }
        }
        private async void TranslateMissingKeysAsync(string targetLanguage, string baseLanguage)
        {
            var translator = GetLocalizationTranslator();
            try
            {
                await translator.TranslateMissingKeysAsync(
                    Path.Combine(_sourceDirectory, baseLanguage + ".json"),
                    Path.Combine(_sourceDirectory, targetLanguage + ".json"),
                    targetLanguage,
                    baseLanguage
                );
                Debug.Log("Translation missing Keys completed!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}");
            }
        }
        private LocalizationTranslator GetLocalizationTranslator()
        {
            LocalizationTranslator translator;
            if (_translatorTypeIndex == 0)
            {
                translator = new LocalizationTranslator(new GoogleTranslator());
            }
            else
            {
                translator = new LocalizationTranslator(new LibreTranslate(_libreTranslateUrl));
            }

            return translator;
        }


    }
}
