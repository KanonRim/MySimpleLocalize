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
                TranslateAllFiles(_targetLanguage, _baseLanguage, _translatorTypeIndex, _libreTranslateUrl);
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

        private static async void TranslateAllFiles(string targetLanguage, string baseLanguage, int translatorTypeIndex,
            string libreTranslateUrl)
        {
            string sourceDirectory = Application.dataPath + "/StreamingAssets/Localization";

            LocalizationTranslator translator;

            if (translatorTypeIndex == 0)
            {
                translator = new LocalizationTranslator(new GoogleTranslator());
            }
            else
            {
                translator = new LocalizationTranslator(new LibreTranslate(libreTranslateUrl));
            }

            try
            {
                await translator.TranslateLocalizationFileAsync(
                    Path.Combine(sourceDirectory, baseLanguage + ".json"),
                    Path.Combine(sourceDirectory, targetLanguage + ".json"),
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
    }
}
