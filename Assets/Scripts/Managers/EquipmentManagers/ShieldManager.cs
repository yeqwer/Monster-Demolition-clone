using System;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    public GameObject shieldObj;

    public GameObject shieldSocet;

    public List<Shield> shields = new List<Shield>();

    void Start()
    {
        RefreshShieldList();
    }

    public void AddShield(int count)
    {
        for (int i = 0; i < count; i++)
        {
            shields.Add(new Shield());
        }
        if (shields.Count != 0 & shieldSocet.transform.childCount == 0)
        {
            var nowBobm = Instantiate(shieldObj, shieldSocet.transform.position, shieldSocet.transform.rotation);
            nowBobm.transform.parent = shieldSocet.transform;
            nowBobm.transform.localScale = new Vector3(1, 1, 1);
        }
        RefreshShieldList();
    }

    public void RemoveShield(bool all)
    {
        if (all)
        {
            shields.Clear();
        }
        else
        {
            shields.RemoveAt(shields.Count - 1);
        }
        RefreshShieldList();
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh list")]
#endif
    public void RefreshShieldList()
    {
        if (shields.Count != 0 & shieldSocet.transform.childCount == 0)
        {
            var nowBobm = Instantiate(shieldObj, shieldSocet.transform.position, shieldSocet.transform.rotation);
            nowBobm.transform.parent = shieldSocet.transform;
            nowBobm.transform.localScale = new Vector3(1, 1, 1);

        }
        else if (shields.Count == 0 & shieldSocet.transform.childCount != 0)
        {
            Destroy(shieldSocet.transform.GetChild(0).gameObject);
        }
    }
}

[Serializable] public class Shield { GameObject shield; }