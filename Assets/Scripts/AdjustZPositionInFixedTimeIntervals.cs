using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustZPositionInFixedTimeIntervals : MonoBehaviour
{
    public float secondsInterval = 2f;
    public bool setZToZero = true;
    private Vector3 initialPosition;
    private Transform ownTransform;

    void Start()
    {
        initialPosition = transform.position;
        InvokeRepeating(nameof(ResetZPosition), secondsInterval, secondsInterval);
        ownTransform = transform;
        if (setZToZero)
            initialPosition.z = 0;
    }

    void ResetZPosition()
    {
        // Not creating a new Vector3 object for performance reasons
        initialPosition.x = ownTransform.position.x;
        initialPosition.y = ownTransform.position.y;
        ownTransform.position = initialPosition;
    }
}
