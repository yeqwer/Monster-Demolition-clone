using UnityEngine;

public class RocksController : MonoBehaviour
{
    public GameObject rock;
    public GameObject aim;

    [ContextMenu("Spawn rock")]
    void SpawnRock() {
        rock.SetActive(true);
        aim.SetActive(false);
    }

    [ContextMenu("Stop rock")]
    void StopRock() {
        rock.SetActive(false);
        aim.SetActive(true);
    }
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            SpawnRock();
        }
    }
}
