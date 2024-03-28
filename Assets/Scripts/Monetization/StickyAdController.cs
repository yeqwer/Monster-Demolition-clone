#if YANDEX_BUILD
using KiYandexSDK;
#endif
using UnityEngine;

public class StickyAdController : MonoBehaviour
{
#if YANDEX_BUILD
    private void Start()
    {
        AdvertSDK.StickyAdActive(true);
    }
#endif
}
