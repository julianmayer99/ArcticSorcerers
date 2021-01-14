using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDollyMovement : MonoBehaviour
{
    [SerializeField] private bool randomStartPoint = true;
    [SerializeField] private float speed = 2f;

    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    private float startTime;
    private float journeyLength;
    private Transform ownTransform;
    private bool isMooving = false;
    private byte moveDirection = 1;
    private System.Action onMoveCompleteCallback;

    private void Start()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition.position, endPosition.position);
        ownTransform = transform;
    }

    public void StartMoveFromStartToEnd(System.Action onMoveComplete)
    {
        startTime = Time.time;
        moveDirection = 1;
        isMooving = true;
        onMoveCompleteCallback = onMoveComplete;
    }

    public void StartMoveFromEndToStart()
    {
        startTime = Time.time;
        moveDirection = 0;
        isMooving = true;
    }
    public void SetToStart()
    {
        transform.position = startPosition.position;
        transform.rotation = startPosition.rotation;
    }
    public void SetToEnd()
    {
        transform.position = endPosition.position;
        transform.rotation = endPosition.rotation;
    }

    private void Update()
    {
        if (!isMooving)
            return;

        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * speed * -1;
        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;
        fractionOfJourney = .5f + .5f * Mathf.Sin((fractionOfJourney - .5f) * Mathf.PI); // beautiful Animation

        // Set our position as a fraction of the distance between the markers.
        ownTransform.position = moveDirection == 1
            ? Vector3.Lerp(startPosition.position, endPosition.position, fractionOfJourney)
            : Vector3.Lerp(endPosition.position, startPosition.position, fractionOfJourney);

        ownTransform.rotation = moveDirection == 1
            ? Quaternion.Lerp(startPosition.rotation, endPosition.rotation, fractionOfJourney)
            : Quaternion.Lerp(endPosition.rotation, startPosition.rotation, fractionOfJourney);

        if (fractionOfJourney > .999f)
        {
            isMooving = false;
            if (onMoveCompleteCallback != null)
                onMoveCompleteCallback.Invoke();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(startPosition.position, endPosition.position);
    }
}
