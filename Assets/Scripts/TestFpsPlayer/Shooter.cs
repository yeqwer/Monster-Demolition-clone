using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject bullet;
    public Transform pointShoot;
    public float force;

    void Update() { 
        if (Input.GetMouseButtonDown(0)) {
            GameObject bul = Instantiate(bullet, pointShoot.position, Quaternion.identity);
            bul.GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * force;
        }
    }
}
