using System.Threading.Tasks;

namespace KoroBox.MySimpleLocalization.Editor
{
    public interface ITranslator
    {
        Task<string> TranslateTextAsync(string text, string targetLanguage, string currentLanguage = "");
    }
}