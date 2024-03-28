using UnityEngine;
using Zenject;

public class SelectedCarViewer : MonoBehaviour
{
    [SerializeField]
    private Transform _carContainer;

    [SerializeField]
    private Transform _cameraTransform;

    [SerializeField]
    private float _rotationSpeed;

    [SerializeField]
    private float _distanceBetweenCars = 30f;

    private (GameObject car, Vector3 position)[] _allCarItems;

    private (GameObject car, Vector3 position) _selectedCarItem;

    private Rigidbody _selectedCarRigidbody;

    [Inject]
    private void Construct(CarSelectManager carSelectManager)
    {
        carSelectManager.OnCarSelectedIndex += OnCarSelected;

        _allCarItems = new (GameObject car, Vector3 hidePosition)[carSelectManager.AllCars.Count];

        Vector3 nextCarOffset = Vector3.zero;
        for (int i = 0; i < _allCarItems.Length; i++)
        {
            nextCarOffset += Vector3.forward * _distanceBetweenCars;

            _allCarItems[i].position = _carContainer.position + nextCarOffset;

            _allCarItems[i].car = Instantiate(carSelectManager.AllCars[i],
                _allCarItems[i].position,
                Quaternion.identity,
                _carContainer);

            _allCarItems[i].car.GetComponent<Rigidbody>().isKinematic = true;
            _allCarItems[i].car.transform.Find("Lights").gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (_selectedCarRigidbody is not null)
        {
            Quaternion newRotation = _selectedCarRigidbody.rotation * Quaternion.Euler(0f, _rotationSpeed * Time.fixedDeltaTime, 0f);
            _selectedCarRigidbody.MoveRotation(newRotation);
        }
    }

    private void OnCarSelected(int index)
    {
        if (_selectedCarRigidbody is not null)
        {
            _selectedCarRigidbody.rotation = Quaternion.identity;
        }

        _selectedCarItem = _allCarItems[index];
        _selectedCarRigidbody = _selectedCarItem.car.GetComponent<Rigidbody>();
        _selectedCarRigidbody.GetComponentInChildren<ParticleSystem>().Play();
        _cameraTransform.position = _selectedCarItem.position;
    }
}
