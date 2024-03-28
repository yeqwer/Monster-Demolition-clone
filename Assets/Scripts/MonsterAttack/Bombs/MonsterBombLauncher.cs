using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MonsterBombLauncher : MonoBehaviour {

    public GameObject monsterBomb;    
    public List<GameObject> targets = new List<GameObject>();
    public List<GameObject> bombs = new List<GameObject>();

    void Update() {
        if (bombs != null) {
            foreach (GameObject bomb in bombs) {
                if (bomb != null) {
                    float dist = Vector3.Distance(bomb.transform.position, bomb.transform.parent.position);
                    if (dist > 2) {
                        bomb.transform.localPosition = Vector3.Lerp(bomb.transform.localPosition, Vector3.zero, Time.deltaTime * 2); 
                        bomb.transform.localRotation = Quaternion.Euler(dist * 10, 0, 0);
                    } 
                }
            }
        }
    }
    [ContextMenu("Spawn")]
    public async void SpawnBombs() {

        targets.Clear();
        bombs.Clear();
        
        targets.AddRange(GameObject.FindGameObjectsWithTag("TargetBombs"));
        
        if (targets != null) {
            foreach (GameObject trg in targets) {

                await UniTask.Delay(200);
                if (trg.transform.childCount <= 1) {
                    GameObject bomb = Instantiate(monsterBomb, this.transform.position, this.transform.rotation);
                    bomb.transform.SetParent(trg.transform, true);
                    bombs.Add(bomb);
                }
            }
        }
    }
}