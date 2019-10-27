using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPointer : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Sprite pointer;
    [SerializeField] Sprite aim;

    private Camera uiCamera;
    private RectTransform pointerRectTransform;
    private Transform player;

    private void Start()
    {
        pointerRectTransform = transform.Find("Pointer").GetComponent<RectTransform>();
        player = GameObject.Find("Player").transform;
        uiCamera = Camera.main;
    }
    private void Update()
    {
        Vector3 toPosition = target.position;
        Vector3 fromPosition = Camera.main.transform.position;
        fromPosition.z = 0;
        //first rotate
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg-90 ;

        //Debug.Log(Camera.main.transform.eulerAngles.z );
        angle -= Camera.main.transform.eulerAngles.z ;
        pointerRectTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //then set pos
        if (!CheckInScreen())
        {
            OutScreenPointer(angle);
        }
        else {
            Image img = pointerRectTransform.gameObject.GetComponent<Image>();
            img.sprite = aim;
            Vector3 sPos = Camera.main.WorldToScreenPoint(target.position);
            pointerRectTransform.position = sPos;
        }
    }
    bool CheckInScreen() {
        Vector3 sPos = Camera.main.WorldToScreenPoint(target.position);
        if (sPos.x > 0 && sPos.x < Screen.width
            &&sPos.y>0 && sPos.y<Screen.height) {
            return true;
        }
        return false;
    }
    void OutScreenPointer(float angle) {
        pointerRectTransform.gameObject.GetComponent<Image>().sprite = pointer;
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        angle *= Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = -Mathf.Sin(angle);

        Vector3 screenPos;

        //y = mx+b
        float m = cos / sin;

        Vector3 screenBounds = screenCenter * 0.8f;

        //check up and down first
        if (cos > 0)
        {
            screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
        }
        else
        {
            screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
        }
        //then check right and left
        if (screenPos.x > screenBounds.x)
        {
            screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
        }
        else if (screenPos.x < -screenBounds.x)
        {
            screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
        }

        screenPos += screenCenter;
        pointerRectTransform.position = screenPos;
    }
}
