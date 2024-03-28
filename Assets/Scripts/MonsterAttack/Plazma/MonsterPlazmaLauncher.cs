using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MonsterPlazmaLauncher : MonoBehaviour
{
    public int DelayStartInMilliseconds = 100;
    public List<GameObject> targets;

    void Awake() {
    }

    [ContextMenu("Start Plazma")]
    public async void StartPlazma(GameObject plazmaObj) {

        targets = new List<GameObject>();
        targets.AddRange(GameObject.FindGameObjectsWithTag("TargetPlazma"));

        foreach (var i in targets) 
        {
            await UniTask.Delay(DelayStartInMilliseconds);
            
            var pl =  Instantiate(plazmaObj, this.transform.position, plazmaObj.transform.rotation); 
            pl.GetComponent<PlazmaController>().target = i;
            pl.GetComponent<Rigidbody>().AddForce(Vector3.up * 5, ForceMode.Impulse);
        }    
    }
}
