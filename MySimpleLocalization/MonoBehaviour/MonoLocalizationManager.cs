using UnityEngine;
using KoroBox.MySimpleLocalization;

public class  MonoLocalizationManager : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(LocalizationManager.Instance.Initialize());
    }
}