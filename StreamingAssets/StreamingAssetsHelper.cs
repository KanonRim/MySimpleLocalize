using System;
using System.Collections;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

public static class StreamingAssetsHelper
{
    /// <summary>
    /// Получить полный путь к файлу в папке StreamingAssets.
    /// </summary>
    /// <param name="relativePath">Относительный путь к файлу в папке StreamingAssets.</param>
    /// <returns>Полный путь к файлу.</returns>
    public static string GetFilePath(string relativePath)
    {
        string path = string.Empty;

#if UNITY_EDITOR || UNITY_STANDALONE  
        // Локальный путь для редактора и настольных приложений.
        path = Path.Combine(Application.streamingAssetsPath, relativePath);
#elif UNITY_ANDROID || UNITY_WEBGL
        // Для Android нужно учитывать доступ через jar: (работа с WWW или UnityWebRequest).
        path = Path.Combine(Application.streamingAssetsPath, relativePath);
#elif UNITY_IOS
        // Для iOS путь аналогичен Editor, но через Application.streamingAssetsPath.
        path = Path.Combine(Application.streamingAssetsPath, relativePath);
#else
        path = Path.Combine(Application.streamingAssetsPath, relativePath);
#endif

        return path;
    }

    /// <summary>
    /// Асинхронное чтение файла из StreamingAssets (удобно для Android и WebGL).
    /// </summary>
    /// <param name="relativePath">Относительный путь к файлу.</param>
    /// <param name="callback"></param>
    /// <returns>Текстовое содержимое файла.</returns>
    public static IEnumerator ReadFile(string relativePath, [CanBeNull] Action<string> callback = null)
    {
            string fullPath = GetFilePath(relativePath); // Assuming GetFilePath is implemented correctly.

            if (fullPath.Contains("://"))
            {
                    UnityWebRequest request = UnityWebRequest.Get(fullPath);
                    yield return request.SendWebRequest();
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                            throw new IOException($"Error reading file from URL: {request.error}");
                    }
                        
                    callback?.Invoke(request.downloadHandler.text);
                    yield return request.downloadHandler.text;
            }
            else
            {
                    if (File.Exists(fullPath))
                    {
                            callback?.Invoke(File.ReadAllText(fullPath));
                    }
                    else
                    {
                            throw new FileNotFoundException($"File not found: {fullPath}");
                    }
            }
    }
}
