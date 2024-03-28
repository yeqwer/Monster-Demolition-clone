using System.Collections.Generic;
using UnityEngine;

public class MonsterLazerController : MonsterAttackControllerBase
{
    [SerializeField] private GameObject lazerSpawnPrefab;
    [SerializeField] private Color color = new Color(255, 255, 255);
    [HideInInspector] public List<LazerColorChanger> lazerColorChanger;

    void Start()
    {
        lazerColorChanger.AddRange(FindObjectsOfType<LazerColorChanger>());
    }

    [ContextMenu("Spawn lazer target")]
    public override void Starter()
    {
        foreach (var i in lazerColorChanger) { i.SetColor(color); }
        Instantiate(lazerSpawnPrefab, Vector3.zero, Quaternion.identity);
    }
}