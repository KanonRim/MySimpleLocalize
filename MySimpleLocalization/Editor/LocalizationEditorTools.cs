#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KoroBox.MySimpleLocalization.Editor
{
public class LocalizationSettingsWindow : EditorWindow
{
    private string targetLanguage = "en";
    private string baseLanguage = "ru";
    private int translatorTypeIndex = 0;
    private string libreTranslateUrl = "http://localhost:5000";

    private readonly string[] translatorOptions = { "GoogleTranslator", "LibreTranslate" };

    [MenuItem("Tools/Localization/Settings")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationSettingsWindow>("Localization Settings");
    }

    private void OnGUI()
    {
        GUILayout.Label("Localization Settings", EditorStyles.boldLabel);

        targetLanguage = EditorGUILayout.TextField("Target Language (e.g., en)", targetLanguage);
        baseLanguage = EditorGUILayout.TextField("Base Language (e.g., ru)", baseLanguage);

        translatorTypeIndex = EditorGUILayout.Popup("Translator Type", translatorTypeIndex, translatorOptions);

        if (translatorTypeIndex == 1) // LibreTranslate
        {
            libreTranslateUrl = EditorGUILayout.TextField("LibreTranslate URL", libreTranslateUrl);
        }

        if (GUILayout.Button("Translate"))
        {
            TranslateAllFiles(targetLanguage, baseLanguage, translatorTypeIndex, libreTranslateUrl);
        }
        if (GUILayout.Button("Generate File List"))
        {
            GenerateFileList();
        }
    }

    public static void GenerateFileList()
    {
        string localizationPath = Path.Combine(Application.streamingAssetsPath, "Localization");
        if (!Directory.Exists(localizationPath))
        {
            Debug.LogError($"Localization directory not found: {localizationPath}");
            return;
        }

        string[] files = Directory.GetFiles(localizationPath, "*.json");
        string fileListPath = Path.Combine(localizationPath,LocalizationManager.LocalizationFileListName);

        File.WriteAllLines(fileListPath, files.Select(Path.GetFileName));

        Debug.Log($"File list generated at {fileListPath}");
        AssetDatabase.Refresh();
    }
    
    public static async void TranslateAllFiles(string targetLanguage, string baseLanguage, int translatorTypeIndex, string libreTranslateUrl)
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
                System.IO.Path.Combine(sourceDirectory, baseLanguage + ".json"),
                System.IO.Path.Combine(sourceDirectory, targetLanguage + ".json"),
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

#endif
}