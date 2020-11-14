using System.Collections;
using TMPro;
using UnityEngine;

public class PlusTextPopup : MonoBehaviour
{
    public Vector3 offset;
    public TextMeshProUGUI text;

    void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    public void SetValue(int value)
    {
        text.text = "+" + value;
    }
    public void SetValue(string value)
    {
        text.text = value;
    }
}
