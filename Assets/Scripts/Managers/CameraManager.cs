using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraManager : MonoBehaviour
{
    public enum CameraState
    {
        Start,
        InGame,
        InFight,
        End
    }

    [Serializable]
    private class CameraSet
    {
        public CameraState cameraState;
        [HideInInspector]
        public GameObject targetFollow;
        [HideInInspector]
        public Vector3 rootOffset;
        public Vector3 cameraOffset;
        public Vector3 rotation;
        public float FOV;
    }

    private CinemachineVirtualCamera _virtCam;

    private CinemachineFramingTransposer _orbitalTransposer;

    [SerializeField]
    private CameraSet[] _cameraSet = new CameraSet[4]
    {
        new CameraSet { cameraState = CameraState.Start },
        new CameraSet { cameraState = CameraState.InGame },
        new CameraSet { cameraState = CameraState.InFight },
        new CameraSet { cameraState = CameraState.End },
    };

    private Dictionary<CameraState, CameraSet> _cameraStateSetPairs = new Dictionary<CameraState, CameraSet>();

    [Inject]
    private void Construct(GameManagerBase gameManager, CarSpawnManager carSpawnManager, LevelManager levelManager)
    {
        gameManager.OnStateChanged += OnGameStateChanged;
        carSpawnManager.OnCarSpawned += OnCarSpawn;
        levelManager.OnLevelLoaded += levelIndex => OnLevelLoad();
    }

    private void Awake()
    {
        _virtCam = GetComponent<CinemachineVirtualCamera>();
        _orbitalTransposer = _virtCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        _cameraStateSetPairs = _cameraSet.ToDictionary(cameraSet => cameraSet.cameraState);
    }

    public void SetCamera(int i)
    {
        _virtCam.Follow = _cameraSet[i].targetFollow.transform; //folow object
        _virtCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = _cameraSet[i].rootOffset + _cameraSet[i].cameraOffset; //position
        StopAllCoroutines();
        StartCoroutine(RotationCam(_virtCam.transform.eulerAngles, _cameraSet[i].rotation, 0.6f)); //rotation
        StartCoroutine(ChangeFov(_virtCam.m_Lens.FieldOfView, _cameraSet[i].FOV, 0.6f)); //FOV
    }

    public void SetCamera(CameraState state)
    {
        CameraSet cameraSet = _cameraStateSetPairs[state];

        _virtCam.Follow = cameraSet.targetFollow.transform; //folow object
        _virtCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = cameraSet.rootOffset + cameraSet.cameraOffset; //position
        StopAllCoroutines();
        StartCoroutine(RotationCam(_virtCam.transform.eulerAngles, cameraSet.rotation, 0.6f)); //rotation
        StartCoroutine(ChangeFov(_virtCam.m_Lens.FieldOfView, cameraSet.FOV, 0.6f)); //FOV
    }

    IEnumerator ChangeFov(float source, float target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            _virtCam.m_Lens.FieldOfView = Mathf.Lerp(source, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        _virtCam.m_Lens.FieldOfView = target;
    }

    IEnumerator RotationCam(Vector3 source, Vector3 target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            _virtCam.transform.eulerAngles = Vector3.Lerp(source, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        _virtCam.transform.eulerAngles = target;
    }

    private void OnCarSpawn(CarController carController)
    {
        CarData carData = carController.gameObject.GetComponent<CarData>();

        _cameraStateSetPairs[CameraState.Start].targetFollow = carController.gameObject;
        _cameraStateSetPairs[CameraState.Start].rootOffset = carData.CarObject.CameraOffset;

        _cameraStateSetPairs[CameraState.InGame].targetFollow = carController.gameObject;
        _cameraStateSetPairs[CameraState.InGame].rootOffset = carData.CarObject.CameraOffset;

        _cameraStateSetPairs[CameraState.InFight].targetFollow = carController.gameObject;
        _cameraStateSetPairs[CameraState.InFight].rootOffset = carData.CarObject.CameraOffset;
    }

    private void OnLevelLoad()
    {
        _cameraStateSetPairs[CameraState.End].targetFollow = GameObject.FindGameObjectWithTag("MonsterStart");
    }

    private void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Start:
                SetCamera(CameraState.Start);
                break;
            case GameState.Playing:
                SetCameraDamping(1f, 1f, 1f);
                SetCamera(CameraState.InGame);
                break;
            case GameState.Finish:
                SetCamera(CameraState.InFight);
                break;
            case GameState.End:
                SetCamera(CameraState.End);
                SetCameraDamping(0f, 0f, 0f);
                break;
            case GameState.Fail:
                SetCameraDamping(0f, 0f, 0f);
                break;
        }
    }

    private void SetCameraDamping(float x, float y, float z)
    {
        _orbitalTransposer.m_XDamping = x;
        _orbitalTransposer.m_YDamping = y;
        _orbitalTransposer.m_ZDamping = z;
    }
}
