using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingGuiObject : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Transform uiObject;
    [SerializeField] private Vector3 offset;
    private Transform ownTransform;

    private void Awake()
    {
        ownTransform = transform; // For performance reasons
    }

    private void LateUpdate()
    {
        uiObject.position = camera.WorldToScreenPoint(ownTransform.position + offset);
    }

    private void OnEnable()
    {
        if (uiObject != null)
            uiObject.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (uiObject != null)
            uiObject.gameObject.SetActive(false);
    }
}
