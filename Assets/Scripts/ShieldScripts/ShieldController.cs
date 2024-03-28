using UnityEngine;
using VoxelTools;

public class ShieldController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public int healthPoints = 4;
    public Color colorDamageParent;
    public Color colorDamageChield;
    [SerializeField] private MeshRenderer _meshRendererParent;
    [SerializeField] private MeshRenderer _meshRendererChield;
    private Color _colorStartChield;
    private Color _colorStartParent;
    private VoxelRootObject voxelRootObject;
    private ShieldManager _shieldManager;
    private float rotationSpeedStart;

    void Awake()
    {
        voxelRootObject = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<VoxelRootObject>();
        _shieldManager = FindObjectOfType<ShieldManager>();

        _colorStartChield = _meshRendererChield.material.color;
        _colorStartParent = _meshRendererParent.material.color;

        rotationSpeedStart = rotationSpeed;
    }
    void Update()
    {
        this.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("DamageForCar"))
        {
            ResponseLight();
        }
    }

    void ResponseLight()
    {
        voxelRootObject.IsDestructible = false;

        _meshRendererParent.material.color = colorDamageParent;
        _meshRendererChield.material.color = colorDamageChield;

        rotationSpeed = 240;

        //await UniTask.Delay(250);

        _meshRendererChield.material.color = _colorStartChield;
        _meshRendererParent.material.color = _colorStartParent;

        rotationSpeed = rotationSpeedStart;

        if (healthPoints != 0)
        {

            healthPoints--;
        }
        else
        {
            _shieldManager.RemoveShield(false);
            voxelRootObject.IsDestructible = true;
        }
    }
}
