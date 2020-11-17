using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageManupulationHelper : MonoBehaviour
{
    private Image image;
    public ColorManager.ColorField color1;
    public ColorManager.ColorField color2;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetColor1()
    {
        image.color = ColorManager.Instance.GetColor(color1);
    }
    public void SetColor2()
    {
        image.color = ColorManager.Instance.GetColor(color2);
    }
}
