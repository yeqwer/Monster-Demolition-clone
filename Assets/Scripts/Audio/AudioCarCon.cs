using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioCarCon : AudioController
{
    [SerializeField] private AudioSource _audioSourceBump;
    [SerializeField] private AudioSource _audioSourceEngine;
    [SerializeField] private AudioClip[] _audioClipBump;
    [SerializeField] private AudioClip _audioClipEngine;
    private Rigidbody _rb;
    private float _pitchCount;
    private bool oneTime = true;
    public float pitchCount
    {
        get
        {
            return _pitchCount;
        }
        set
        {
            if (value > 0 & value < 3)
            {
                _pitchCount = value;
            }
            else if (value < 0 & value > -3)
            {
                _pitchCount = -value;
            }
            else { _pitchCount = 2; }
        }
    }

    public void Awake()
    {
        _rb = GetComponentInParent<Rigidbody>();

        pitchCount = 0;

        if (SceneManager.GetActiveScene().name == "Game")
        {
            PlaySound(_audioSourceEngine, _audioClipEngine);
        }
    }
    public void Update()
    {
        pitchCount = _rb != null ? _rb.velocity.z * 0.04f : 0;
        ChangePitch(_audioSourceEngine, 1 + pitchCount);
    }

    public void Play()
    {
        if (oneTime)
        {
            PlaySound(_audioSourceBump, _audioClipBump[Random.Range(0, _audioClipBump.Length - 1)]);
            Disconnect();
            oneTime = false;
        }
    }
}