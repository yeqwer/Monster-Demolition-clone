using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BoosterCardItem : MonoBehaviour
{
    [SerializeField]
    private Image _itemImage;

    [SerializeField]
    private TextMeshProUGUI _itemCountText;

    [SerializeField]
    private GameObject _itemCheckIcon;

    [SerializeField]
    private GameObject _itemLabel;

    public event Action OnClick;

    public static BoosterCardItem Instantiate(DiContainer diContainer, Transform parent, EquipmentObject equipmentObject, int count, bool isAdReward)
    {
        GameObject boosterCardPrefab;

        if (isAdReward)
            boosterCardPrefab = Resources.Load<GameObject>("Prefabs/UI/Game/AdBoosterOfferCardItem");
        else
            boosterCardPrefab = Resources.Load<GameObject>("Prefabs/UI/Game/BoosterOfferCardItem");

        GameObject boosterCardInstance = diContainer.InstantiatePrefab(boosterCardPrefab, parent);

        BoosterCardItem boosterCardItem = boosterCardInstance.GetComponent<BoosterCardItem>();

        boosterCardItem._itemImage.sprite = equipmentObject.Icon;
        boosterCardItem._itemCountText.text = count.ToString();

        return boosterCardItem;
    }

    public void ProcessOnClick()
    {
        _itemLabel.SetActive(false);
        _itemCheckIcon.SetActive(true);
        OnClick?.Invoke();
    }
}
