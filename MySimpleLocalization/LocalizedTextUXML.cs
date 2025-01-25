using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LocalizedTextUXML : MonoBehaviour
{
    
    [SerializeField]
    private List<NameKey> _names = new List<NameKey>();
    
    private  LocalizationManager _localizationManager = LocalizationManager.Instance;
    private UIDocument _uiDocument;

    private void Awake()
    {
       
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start()
    {
        
        UpdateText();
        _localizationManager.OnLanguageChanged += UpdateText;
    }

    private void OnDestroy()
    {
        _localizationManager.OnLanguageChanged-= UpdateText;
    }

    private void UpdateText()
    {
        foreach (var item in _names)
        {
            var visualElement = _uiDocument.rootVisualElement.Q(item.name);
            switch (visualElement)
            {
                case Button button:
                    button.text = _localizationManager.GetLocalizedText(item.name);
                    break;
                case Label label:
                    label.text = _localizationManager.GetLocalizedText(item.name);
                    break;
                case null:
                    Debug.LogError($"Can't find text for name: {item.name}");
                    break;
                default:
                    Debug.LogError($"Can't find type for name: {item.name}");
                    break;
            }
        }
    }
    
    [Serializable]
    public class NameKey
    {
        public string name;
        public string key;
    }
}