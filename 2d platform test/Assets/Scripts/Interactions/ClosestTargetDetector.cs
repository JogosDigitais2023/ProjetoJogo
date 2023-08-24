using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosestTargetDetector : MonoBehaviour
{
    [SerializeField] private float detectionRange;
    [SerializeField] private bool toggleDetectionRadius;
    public Transform detectionPoint;
    public LayerMask detectionLayer;
    public Transform target;
    public GameObject targetObject;
    public GameObject arrow;

    private void Update()
    {
        DetectObject();
    }

    void DetectObject()
    {
        Collider2D collider = Physics2D.OverlapCircle(detectionPoint.position, detectionRange, detectionLayer);
        if (collider)
        {
            target = collider.transform;
            targetObject = collider.gameObject;
            //PointArrow(target);
        }
        else
        {
            target = null;
            targetObject = null;
            arrow.SetActive(false);
        }
    }

    void PointArrow(Transform target)
    {
        arrow.SetActive(true);


        var dir = target.position - arrow.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnDrawGizmosSelected()
    {
        if (toggleDetectionRadius)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(detectionPoint.position, detectionRange);
        }
    }
}
