using System;

namespace KoroBox.MySimpleLocalization
{
    public class LocalizedText
    {
        private readonly string _key;
        public event Action<string> OnLanguageChanged;
        public string Value
        {
            get;
            private set;
        }

        public LocalizedText (string key)
        {
            _key  = key;
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
            Value =  LocalizationManager.Instance.IsReady ? LocalizationManager.Instance.GetLocalizedText(_key) : ""; 
        }

        private void UpdateText()
        {
            Value = LocalizationManager.Instance.GetLocalizedText(_key);
            OnLanguageChanged?.Invoke(Value);
        }
    }
}