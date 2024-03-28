using System;
using UnityEngine;
using Zenject;

public class ZenjectInstantiator : MonoBehaviour
{
    [Serializable]
    private class InstantiatingGameObjectInfo
    {
        public GameObject gameObject;
        public bool isDontDestroyOnLoad;
    }

    [SerializeField]
    private InstantiatingGameObjectInfo[] _gameObjectInfos;

    [Inject]
    private void Construct(DiContainer diContainer)
    {
        foreach (var gameObjectInfo in _gameObjectInfos)
        {
            GameObject gameObjectInstance = diContainer.InstantiatePrefab(gameObjectInfo.gameObject);

            if (gameObjectInfo.isDontDestroyOnLoad)
                DontDestroyOnLoad(gameObjectInstance);
        }

        Destroy(gameObject);
    }
}
