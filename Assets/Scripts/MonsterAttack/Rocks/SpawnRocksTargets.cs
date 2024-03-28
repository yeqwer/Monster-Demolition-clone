using System.Collections.Generic;
using UnityEngine;

public class SpawnRocksTargets : MonoBehaviour
{
    [HideInInspector] public RockType rockType;
    public List<GameObject> tagetsRock;
    [HideInInspector] public int targetMin;
    [HideInInspector] public int targetMax;
    private float randomValueY;
    private GameObject targetRock;

    [ContextMenu("Spawn targets")]
    public void SpawnTargets()
    {
        switch (rockType)
        {
            case RockType.Grey:
                targetRock = tagetsRock[0];
                break;
            case RockType.Henry:
                targetRock = tagetsRock[1];
                break;
        }

        randomValueY = 0;

        if (targetRock != null)
        {

            for (int i = 0; i < Random.Range(targetMin, targetMax); i++)
            {

                int randomValueX_firstLine = Random.Range(-5, -3);
                int randomValueX_secondLine = Random.Range(3, 5);

                randomValueY += Random.Range(-20f,-10f);

                Instantiate(targetRock, this.transform.position + new Vector3(randomValueX_firstLine, 1, randomValueY), Quaternion.identity);
                Instantiate(targetRock, this.transform.position + new Vector3(randomValueX_secondLine, 1, randomValueY), Quaternion.identity);
            }
        }
    }
}

public enum RockType
{
    Grey,
    Henry,
}