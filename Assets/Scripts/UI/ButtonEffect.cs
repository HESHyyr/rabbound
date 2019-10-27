using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image buttonImage;

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0.5f);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 1.0f);
    }

}
