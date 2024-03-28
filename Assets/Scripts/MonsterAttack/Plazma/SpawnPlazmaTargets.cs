using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SpawnPlazmaTargets : MonoBehaviour
{
    public GameObject tagetPlazma;
    [HideInInspector] public PlazmaType plazmaType;
    public List<GameObject> plazmaObj;
    [HideInInspector] public int plazmaMin;
    [HideInInspector] public int plazmaMax;
    private MonsterPlazmaLauncher monsterPlazmaLauncher;
    private GameObject playerCar;

    [Inject]
    private void Construct(CarSpawnManager carSpawnManager, MonsterManager monsterManager)
    {
        carSpawnManager.OnCarSpawned += carController => playerCar = carController.gameObject;
        monsterManager.OnMonsterLoad += monster => monsterPlazmaLauncher = monster.GetComponentInChildren<MonsterPlazmaLauncher>();
    }

    [ContextMenu("Spawn plazma targets")]
    public void SpawnTargets()
    {

        if (tagetPlazma != null)
        {

            for (int i = 0; i < Random.Range(plazmaMin, plazmaMax); i++)
            {

                float randomValueX = Random.Range(-5, 5);
                float randomValueY = Random.Range(-10, 10);

                var rb = playerCar.GetComponent<Rigidbody>();

                var car_random_velocity = playerCar.transform.position.z + (rb.velocity.z * 2) + randomValueY;
                var plazmaZ = car_random_velocity < 180 ? car_random_velocity : car_random_velocity = 180 + randomValueY;
                //Debug.LogWarning(plazmaZ);
                Instantiate(tagetPlazma, new Vector3(randomValueX, 0, this.transform.localPosition.z + plazmaZ), Quaternion.identity);
                // Instantiate(tagetPlazma, this.transform.position + new Vector3(randomValueX, 0, playerCar.transform.position.z + randomValueY), Quaternion.identity);
            }
        }

        switch (plazmaType)
        {
            case PlazmaType.Purple:
                monsterPlazmaLauncher.StartPlazma(plazmaObj[0]);
                break;
            case PlazmaType.Yellow:
                monsterPlazmaLauncher.StartPlazma(plazmaObj[1]);
                break;
        }
    }
}
public enum PlazmaType
{
    Purple,
    Yellow,
}