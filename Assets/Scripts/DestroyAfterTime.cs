using System.Collections;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float secondsDelay = 3f;
    [SerializeField] private DestroyType type = DestroyType.Destroy;

    private void OnEnable()
    {
        StartCoroutine(DestroyObject());
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(secondsDelay);
        if (type == DestroyType.Deaktivate)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }

    public enum DestroyType
    {
        Destroy,
        Deaktivate
    }

}
