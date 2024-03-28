using UnityEngine;

public class LazerStarter : MonoBehaviour
{
    private GameObject target;
    private LineRenderer lineRenderer;

    void Awake() {
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }
    void Update() {
        if (target != null) {
            lineRenderer.gameObject.SetActive(true);
            this.gameObject.transform.LookAt(target.transform.position);
            this.gameObject.transform.localScale = new Vector3(1, 1, Vector3.Distance(this.gameObject.transform.position, target.transform.position)/10f);
        } else { 
            lineRenderer.gameObject.SetActive(false);
            target = GameObject.FindGameObjectWithTag("TargetLazer"); 
        }
    } 
}