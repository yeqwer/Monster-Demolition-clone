#if YANDEX_BUILD
using KiYandexSDK;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelTools;
using Zenject;
using Random = UnityEngine.Random;

public class RewardManager : MonoBehaviour
{
    private Dictionary<EquipmentType, EquipmentObject> _equipmentObjectsDictionary;

    [Inject]
    private DataPersistenceManager _dataPersistenceManager;

    [Inject]
    private CurrencyManager _currencyManager;

    [SerializeField]
    private int _minCurrencyBound = 200;

    [SerializeField]
    private int _maxCurrencyBound = 400;

    private VoxelRootObject _monster;

    private int _previousDamagePercentage;

    private float _previousDamageRatio;

    public event Action<EquipmentObject, int, Action, bool> OnRewardOffered;

    [Inject]
    private void Construct(CarSpawnManager carSpawnManager, LevelManager levelManager, MonsterManager monsterManager, GameManagerBase gameManager)
    {
        monsterManager.OnMonsterLoad += monster =>
        {
            _monster = monster.GetComponent<VoxelRootObject>();
            _previousDamagePercentage = _monster.DamagePercentage;
        };

        carSpawnManager.OnCarSpawned += carController =>
        {
            EquipmentInventory inventory = _dataPersistenceManager.PlayerData.EquipmentInventory;

            // Equipping item if there is one in inventory
            if (inventory.PeekFirstItem(out EquipmentType item))
            {
                switch (item)
                {
                    case EquipmentType.Bomb:
                        carController.GetComponent<BombManager>().AddBomb(1);
                        break;
                    case EquipmentType.Rocket:
                        carController.GetComponent<RocketManager>().AddRocket(1);
                        break;
                    case EquipmentType.Shield:
                        carController.GetComponent<ShieldManager>().AddShield(1);
                        break;
                }
            }

            carController.GetComponent<Demolisher>().OnHitDestructible += (destructible) =>
            {
                //float destroyedPercentsInRound = _previousDamagePercentage - _monster.DamagePercentage;
                //_previousDamagePercentage = _monster.DamagePercentage;
                //destroyedPercentsInRound = destroyedPercentsInRound == 0 ? 0.1f : destroyedPercentsInRound;
                //GiveRandomAmountOfCurrency(destroyedPercentsInRound);

                float destroyedRatioInRound = _monster.DamageRatio - _previousDamageRatio;
                _previousDamageRatio = _monster.DamageRatio;
                GiveRandomAmountOfCurrency(destroyedRatioInRound);
            };
        };

        gameManager.OnStateChanged += state =>
        {
            if (state is GameState.Playing)
                _dataPersistenceManager.PlayerData.EquipmentInventory.RemoveFirstItem();
        };

        levelManager.OnLevelCompleted += levelIndex =>
        {
            OfferRandomReward(Random.Range(1, 3));
            OfferRandomAdReward(Random.Range(3, 6));
            OfferRandomAdReward(Random.Range(3, 6));
        };
    }

    private void Awake()
    {
        EquipmentObject[] equipmentObjects = Resources.LoadAll<EquipmentObject>("ScriptableObjects/Equipment");

        _equipmentObjectsDictionary = equipmentObjects.ToDictionary(e => e.EquipmentType);
    }

    private void OfferReward(EquipmentType type, int count)
    {
        Action action = () =>
        {
            for (int i = 0; i < count; i++)
                _dataPersistenceManager.PlayerData.EquipmentInventory.AddItemToEnd(type);
        };

        OnRewardOffered?.Invoke(_equipmentObjectsDictionary[type], count, action, false);
    }

    private void OfferAdReward(EquipmentType type, int count)
    {
#if YANDEX_BUILD
        Action action = () =>
        {
            AdvertSDK.RewardAd(onRewarded: () =>
            {
                for (int i = 0; i < count; i++)
                    _dataPersistenceManager.PlayerData.EquipmentInventory.AddItemToEnd(type);
            });
        };

        OnRewardOffered?.Invoke(_equipmentObjectsDictionary[type], count, action, true);
#else
        OfferReward(type, count);
#endif
    }

    private void OfferRandomReward(int count)
    {
        EquipmentType type;

        int equipmentTypesCount = Enum.GetValues(typeof(EquipmentType)).Length;

        type = (EquipmentType)Random.Range(0, equipmentTypesCount);

        OfferReward(type, count);
    }

    private void OfferRandomAdReward(int count)
    {
        EquipmentType type;

        int equipmentTypesCount = Enum.GetValues(typeof(EquipmentType)).Length;

        type = (EquipmentType)Random.Range(0, equipmentTypesCount);

        OfferAdReward(type, count);
    }

    private void GiveRandomAmountOfCurrency(float multiplier)
    {
        int currency = Random.Range(_minCurrencyBound, _maxCurrencyBound);

        _currencyManager.Add((uint)(currency * multiplier));
    }
}
