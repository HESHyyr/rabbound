using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPointer : MonoBehaviour
{
    private Camera uiCamera;
    public Transform target;
    private RectTransform pointerRectTransform;
    private Transform mPlayer;

    private void Start()
    {
        pointerRectTransform = transform.Find("Pointer").GetComponent<RectTransform>();
        mPlayer = GameObject.Find("Player").transform;
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
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //then set pos
        //Vector3 targetPosScreenPoint = Camera.main.WorldToScreenPoint(target.position);
        //bool isOffScreen = targetPosScreenPoint.x <= 0 || targetPosScreenPoint.x >= Screen.width
        //                    || targetPosScreenPoint.y <= 0 || targetPosScreenPoint.y >= Screen.height;
        //if (isOffScreen) {
        //    Vector3 cappedTargetScreenPos = targetPosScreenPoint;
        //    if (cappedTargetScreenPos.x <= 0) cappedTargetScreenPos.x = 0;
        //    if (cappedTargetScreenPos.x >= Screen.width) cappedTargetScreenPos.x = Screen.width;
        //    if (cappedTargetScreenPos.y <= 0) cappedTargetScreenPos.y = 0;
        //    if (cappedTargetScreenPos.y >= Screen.height) cappedTargetScreenPos.y = Screen.height;

        //    Vector3 pointerWorldPos = uiCamera.ScreenToViewportPoint(cappedTargetScreenPos);
        //    pointerRectTransform.position = pointerWorldPos;
        //}
        //else {

        //}
    }
}
