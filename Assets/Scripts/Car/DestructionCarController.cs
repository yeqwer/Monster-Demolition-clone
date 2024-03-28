using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelDestruction;

public class DestructionCarController : MonoBehaviour
{
    public GameObject bodyCar;
    public List<GameObject> wheelsCar;
    public List<VoxelFragment> voxelFragments = new List<VoxelFragment>();
    public List<GameObject> toRemove;
    private CarDamageable carDamageable;

    void Awake()
    {
        carDamageable = GetComponent<CarDamageable>();
    }

    void Start()
    {
        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {

        yield return new WaitForSeconds(2);

        // foreach (GameObject wheel in wheelsCar) {
        //     Destroy(wheel.GetComponentInChildren<MeshCollider>());
        // }
        Calculate();
    }

    void Calculate()
    {

        VoxelFragment[] list = bodyCar.GetComponentsInChildren<VoxelFragment>(true);
        voxelFragments.AddRange(list);

        foreach (VoxelFragment fragment in list)
        {
            Destroy(fragment.GetComponentInChildren<MeshCollider>());
        }

        carDamageable.MaxHealth = voxelFragments.Count;
        carDamageable.CurrentHealth = voxelFragments.Count - toRemove.Count;
    }

    public void Refresh()
    {

        toRemove = new List<GameObject>();

        foreach (VoxelFragment fragment in voxelFragments)
        {

            // await UniTask.Delay(1);

            if (fragment.gameObject.activeSelf & !toRemove.Contains(fragment.gameObject))
            {
                toRemove.Add(fragment.gameObject);
            }
        }
        carDamageable.CurrentHealth = voxelFragments.Count - toRemove.Count;
    }

    public IEnumerator Destroyer()
    {

        yield return new WaitForSeconds(3);

        foreach (VoxelFragment fragment in voxelFragments)
        {
            Destroy(fragment.gameObject);
        }
    }

    [ContextMenu("Enable car collider")]
    public void EnableCarCollider()
    {
        bodyCar.GetComponentInChildren<MeshCollider>().enabled = true;
    }

    [ContextMenu("Disable car collider")]
    public void DisableCarCollider()
    {
        bodyCar.GetComponentInChildren<MeshCollider>().enabled = false;
    }
}