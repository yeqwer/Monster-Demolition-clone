using System.Collections;
using UnityEngine;
using Zenject;

public class RocketGun : MonoBehaviour
{
    public GameObject rocket;
    public float countRocketPerStart = 6f;
    public float DelayStartInSeconds = 0.5f;

    private bool canRemove = false;
    private RocketManager rocketManager;

    [Inject]
    private GameManagerBase _gameManager;

    void Awake()
    {
        rocketManager = FindObjectOfType<RocketManager>();
    }

    private void Start()
    {
        _gameManager.OnStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
            _gameManager.OnStateChanged -= OnGameStateChanged;
    }

    private void Update()
    {
        float scaler = 0.1f;
        if (canRemove)
        {
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.zero, Time.deltaTime * 3f);
        }
        if (this.transform.localScale.x < scaler)
        {
            Destroy(this.gameObject);
            rocketManager.RemoveRocket(false);
        }
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state is GameState.Playing)
            StartCoroutine(StartRocketsCoroutine(DelayStartInSeconds));
    }

    [ContextMenu("Start rockets")]
    public void StartRockets()
    {
        for (int i = 0; i < countRocketPerStart; i++)
        {
            Instantiate(rocket, this.transform.position, rocket.transform.rotation);
        }
        canRemove = true;
    }

    private IEnumerator StartRocketsCoroutine(float delayInSeconds)
    {
        for (int i = 0; i < countRocketPerStart; i++)
        {
            yield return new WaitForSeconds(delayInSeconds);

            Instantiate(rocket, this.transform.position, rocket.transform.rotation);
        }
        canRemove = true;
    }
}
