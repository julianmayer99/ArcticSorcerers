using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance;

    public Color bg_FloatingUIObject_Highlightet;
    public Color bg_FloatingUIObject;
    public Color txt_FloatingUIObject_Highlightet;
    public Color txt_FloatingUIObject;

    public PlayerColor[] playerColors;
    public TeamColor[] teamColors;

    private void Awake()
    {
        Instance = this;
    }

    [System.Serializable]
    public enum ColorField
    {
        Bg_FloatingUIObject_Highlightet,
        Bg_FloatingUIObject,
        Txt_FloatingUIObject_Highlightet,
        Txt_FloatingUIObject
    }

    [System.Serializable]
    public class PlayerColor
    {
        public string name;
        public Material material;
        public Color ui_color_dark;
        public Color ui_color_normal;
        [HideInInspector] public bool isInUse = false;
    }

    [System.Serializable]
    public class TeamColor
    {
        public string name;
        public Color ui_color_dark;
        public Color ui_color_normal;
        public Sprite teamIcon;
        [HideInInspector] public bool isInUse = false;
    }

    public IEnumerable<PlayerColor> UnoccupiedPlayerColors => playerColors.Where(c => !c.isInUse);

    public Color GetColor(ColorField color)
    {
        switch (color)
        {
            case ColorField.Bg_FloatingUIObject_Highlightet: return bg_FloatingUIObject_Highlightet;
            case ColorField.Bg_FloatingUIObject: return bg_FloatingUIObject;
            case ColorField.Txt_FloatingUIObject_Highlightet: return txt_FloatingUIObject_Highlightet;
            case ColorField.Txt_FloatingUIObject: return txt_FloatingUIObject;
            default: return Color.white;
        }
    }
}
