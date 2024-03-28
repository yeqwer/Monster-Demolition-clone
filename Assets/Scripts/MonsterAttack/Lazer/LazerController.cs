using System.Collections.Generic;
using UnityEngine;

public class LazerController : MonoBehaviour
{
    private Material mat;
    private GameObject obj;
    public List<Texture> textures = new List<Texture>();  
    private float timer = 0.05f;
    private int count = 0;

    void Awake() {
        obj = this.gameObject;
        mat = obj.GetComponent<LineRenderer>().material;
    }

    void FixedUpdate() {

        timer -= Time.deltaTime;

        if (timer <= 0.0f)
        {
            Spinner();
            timer = 0.05f;
        }  
    }

    void Spinner() {
        count ++;
        if (count < textures.Count) {
            mat.SetTexture("_MainTex", textures[count]);
        } else { count = 0; } 
    }
}
