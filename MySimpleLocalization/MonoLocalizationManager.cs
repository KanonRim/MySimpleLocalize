using UnityEngine;

public class  MonoLocalizationManager : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(LocalizationManager.Instance.Initialize());
    }
}