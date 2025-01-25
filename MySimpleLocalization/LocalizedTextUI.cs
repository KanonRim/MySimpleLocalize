using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedTextUI : MonoBehaviour
{
    [SerializeField] private string _key;
    private LocalizedText _localizedText;

    private void Awake()
    {
        _localizedText = new LocalizedText(_key);
    }

    private void Start()
    {
        
        UpdateText(_localizedText.Value);
        _localizedText.OnLanguageChanged += UpdateText;
    }

    private void OnDestroy()
    {
        _localizedText.OnLanguageChanged-= UpdateText;
    }

    private void UpdateText(string newText)
    {
        var textComponent = GetComponent<Text>();
        textComponent.text =newText ;
    }
}