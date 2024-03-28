using UnityEngine;
using UnityEngine.UI;

public class SetToggle : MonoBehaviour
{
    public RectTransform slider;
    public RectTransform posA;
    public RectTransform posB;
    public bool value = true;
    public float timer;

    private void Awake()
    {
        value = true;
    }

    public void SetToggleSlider() 
    {
        if (value) 
        {
            value = false;
        } 
        else 
        {
            value = true;
        }
    }

    private void Update() 
    {
        if (value) 
        {
            // slider.transform.localPosition = Vector3.Slerp(posB.localPosition, posA.localPosition, Time.deltaTime * timer);
            slider.transform.localPosition = new Vector3(Mathf.Lerp(slider.transform.localPosition.x, posA.localPosition.x, Time.deltaTime * timer) , 0, 0);
            slider.GetComponent<Image>().color = Color.Lerp(slider.GetComponent<Image>().color, Color.green, Time.deltaTime * timer);
        } 
        else 
        {
            // slider.transform.localPosition = Vector3.Slerp(posA.localPosition, posB.localPosition, Time.deltaTime * timer);
            slider.transform.localPosition = new Vector3(Mathf.Lerp(slider.transform.localPosition.x, posB.localPosition.x, Time.deltaTime * timer) , 0, 0);
            slider.GetComponent<Image>().color = Color.Lerp(slider.GetComponent<Image>().color, Color.red, Time.deltaTime * timer);
        }
    }
}