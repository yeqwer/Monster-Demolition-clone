using System;
using UnityEngine;
using Zenject;

public class BoosterOfferContainer : MonoBehaviour
{
    [SerializeField]
    private Transform _equipmentBoosterCardsContainer;

    [Inject]
    private DiContainer _diContainer;

    [Inject]
    private void Construct(RewardManager rewardManager, LevelManager levelManager)
    {
        rewardManager.OnRewardOffered += OnRewardOffered;
        levelManager.OnLevelLoaded += levelIndex => RemoveAllBoosterCards();
    }

    private void OnRewardOffered(EquipmentObject equipmentObject, int count, Action action, bool isAdReward)
    {
        var boosterCardItem = BoosterCardItem.Instantiate(_diContainer, _equipmentBoosterCardsContainer, equipmentObject, count, isAdReward);
        boosterCardItem.OnClick += action;
    }

    private void RemoveAllBoosterCards()
    {
        for (int i = 0; i < _equipmentBoosterCardsContainer.childCount; i++)
            Destroy(_equipmentBoosterCardsContainer.GetChild(i).gameObject);
    }
}
