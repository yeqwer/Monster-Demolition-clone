using Agava.YandexGames;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationSwitcher : MonoBehaviour
{
    public void SetCurrentLocalization()
    {
        DontDestroyOnLoad(gameObject);

        string currentLocaleName = YandexGamesSdk.Environment.i18n.lang;
        StartCoroutine(SwitchLocalization(currentLocaleName));
    }

    public IEnumerator SwitchLocalization(string twoLetterISOName)
    {
        yield return LocalizationSettings.InitializationOperation;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales
            .Where(l => l.Identifier.CultureInfo.TwoLetterISOLanguageName == twoLetterISOName)
            .First();
    }

    public IEnumerator SwitchLocalization(int id)
    {
        yield return LocalizationSettings.InitializationOperation;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
    }

#if UNITY_EDITOR
    [ContextMenu("Switch to english")]
    public void SwitchToEnglish()
    {
        StartCoroutine(SwitchLocalization("en"));
    }

    [ContextMenu("Switch to russian")]
    public void SwitchToRussian()
    {
        StartCoroutine(SwitchLocalization("ru"));
    }
#endif
}
