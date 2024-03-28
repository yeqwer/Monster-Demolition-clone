using System;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    public GameObject bombObj;

    public GameObject bombSocet;

    public List<Bomb> bombs = new List<Bomb>();

    void Start()
    {
        RefreshBombList();
    }

    public void AddBomb(int count)
    {
        for (int i = 0; i < count; i++)
        {
            bombs.Add(new Bomb());
        }
        if (bombs.Count != 0 & bombSocet.transform.childCount == 0)
        {
            var nowBobm = Instantiate(bombObj, bombSocet.transform.position, bombSocet.transform.rotation);
            nowBobm.transform.parent = bombSocet.transform;
            nowBobm.transform.localScale = new Vector3(1, 1, 1);
        }
        RefreshBombList();
    }

    public void RemoveBomb(bool all)
    {
        if (all)
        {
            bombs.Clear();
        }
        else
        {
            bombs.RemoveAt(bombs.Count - 1);
        }
        RefreshBombList();
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh bomb list")]
#endif
    public void RefreshBombList()
    {
        if (bombs.Count != 0 & bombSocet.transform.childCount == 0)
        {
            var nowBobm = Instantiate(bombObj, bombSocet.transform.position, bombSocet.transform.rotation);
            nowBobm.transform.parent = bombSocet.transform;
            nowBobm.transform.localScale = new Vector3(1, 1, 1);

        }
        else if (bombs.Count == 0 & bombSocet.transform.childCount != 0)
        {
            Destroy(bombSocet.transform.GetChild(0).gameObject);
        }
    }
}

[Serializable] public class Bomb { GameObject bomb; }