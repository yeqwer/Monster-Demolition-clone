using UnityEngine;

public class SpawnBombsTargets : MonoBehaviour
{
    public GameObject tagetBombs;
    [HideInInspector] public int targetMin;
    [HideInInspector] public int targetMax;
    private float randomValueY;
    private MonsterBombLauncher monsterBombLauncher;

    [ContextMenu("Spawn targets")]
    public void SpawnTargets()
    {

        randomValueY = 0;

        if (tagetBombs != null)
        {

            for (int i = 0; i < Random.Range(targetMin, targetMax); i++) {
    
                int randomValueX_firstLine = Random.Range(-5, -3);
                int randomValueX_secondLine = Random.Range(3, 5);

                randomValueY += Random.Range(-20f,-10f);

                Instantiate(tagetBombs, this.transform.position + new Vector3(randomValueX_firstLine, 1, randomValueY), Quaternion.identity);
                Instantiate(tagetBombs, this.transform.position + new Vector3(randomValueX_secondLine, 1, randomValueY), Quaternion.identity);
            }
        }
        monsterBombLauncher = FindObjectOfType<MonsterBombLauncher>();
        monsterBombLauncher.SpawnBombs();
    }
}