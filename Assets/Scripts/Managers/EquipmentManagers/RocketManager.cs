using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RocketManager : MonoBehaviour
{
    [Inject]
    private DiContainer _diContainer;

    public GameObject rocketObj;

    public GameObject rocketSocet;

    public List<Rocket> rockets = new List<Rocket>();

    void Start()
    {
        RefreshRocketList();
    }

    public void AddRocket(int count)
    {
        for (int i = 0; i < count; i++)
        {
            rockets.Add(new Rocket());
        }
        if (rockets.Count != 0 & rocketSocet.transform.childCount == 0)
        {
            var nowBobm = Instantiate(rocketObj, rocketSocet.transform.position, rocketSocet.transform.rotation);
            nowBobm.transform.parent = rocketSocet.transform;
            nowBobm.transform.localScale = new Vector3(1, 1, 1);
        }
        RefreshRocketList();
    }

    public void RemoveRocket(bool all)
    {
        if (all)
        {
            rockets.Clear();
        }
        else
        {
            rockets.RemoveAt(rockets.Count - 1);
        }
        RefreshRocketList();
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh list")]
#endif
    public void RefreshRocketList()
    {
        if (rockets.Count != 0 & rocketSocet.transform.childCount == 0)
        {
            //var nowBobm = Instantiate(rocketObj, rocketSocet.transform.position, rocketSocet.transform.rotation);
            //nowBobm.transform.parent = rocketSocet.transform;
            var nowBobm = _diContainer.InstantiatePrefab(rocketObj, rocketSocet.transform.position, rocketSocet.transform.rotation, rocketSocet.transform);
            nowBobm.transform.localScale = new Vector3(1, 1, 1);

        }
        else if (rockets.Count == 0 & rocketSocet.transform.childCount != 0)
        {
            Destroy(rocketSocet.transform.GetChild(0).gameObject);
        }
    }
}

[Serializable] public class Rocket { GameObject rocket; }