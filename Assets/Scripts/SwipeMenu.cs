using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SwipeMenu : MonoBehaviour
{
    public Scrollbar scrollBar;
    float scroll_pos = 0;
    float[] pos;
    [Range(0.001f, 0.3f)] public float smoothTime = .1f;
    public Vector2 shrinkFactor = new Vector2(.8f, .8f);

    [System.Serializable] public class IntegerEvent : UnityEvent<int> { }
    public IntegerEvent OnSwipeMenuSelectionChanged = new IntegerEvent();

    void Start()
    {
        scrollBar.value = scrollBar.value -= 0.04f;
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
        for (int i = 0; i < pos.Length; i++)
        {
            if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
            {
                scrollBar.value = Mathf.Lerp(scrollBar.value, pos[i], .07f);
            }
        }
    }

    private bool forceMove = false;
    private int forceMoveTo = 1;
    int old_i = -1;
    int new_i = -1;

    void Update()
    {
        if (!forceMove)
        {
            pos = new float[transform.childCount];
            float distance = 1f / (pos.Length - 1f);
            for (int i = 0; i < pos.Length; i++)
            {
                pos[i] = distance * i;
            }
            if (Mouse.current.leftButton.isPressed || Touch.activeTouches.Count > 0)
            {
                scroll_pos = scrollBar.value;
            }
            else
            {
                for (int i = 0; i < pos.Length; i++)
                {
                    if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                    {
                        scrollBar.value = Mathf.Lerp(scrollBar.value, pos[i], smoothTime);
                        new_i = i;
                        if (i != old_i)
                        {
                            MenuItemChanged();
                        }
                        old_i = i;
                    }

                }
                for (int i = 0; i < pos.Length; i++)
                {
                    if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                    {
                        transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), smoothTime);
                        for (int a = 0; a < pos.Length; a++)
                        {
                            if (a != i)
                                transform.GetChild(a).localScale = Vector2.Lerp(transform.GetChild(a).localScale, shrinkFactor, smoothTime);
                        }
                    }

                }
            }
        }
        else
        {
            scrollBar.value = Mathf.Lerp(scrollBar.value, pos[forceMoveTo], smoothTime);

            transform.GetChild(forceMoveTo).localScale = Vector2.Lerp(transform.GetChild(forceMoveTo).localScale, new Vector2(1f, 1f), smoothTime);
            for (int a = 0; a < pos.Length; a++)
            {
                if (a != forceMoveTo)
                    transform.GetChild(a).localScale = Vector2.Lerp(transform.GetChild(a).localScale, shrinkFactor, smoothTime);
            }

            if (Mathf.Abs(scrollBar.value - pos[forceMoveTo]) <= .001f)
            {
                forceMove = false;
                scroll_pos = scrollBar.value;
            }
        }
    }

    public void ShowNext() => JumpToPosition(new_i + 1);

    public void ShowPrevious() => JumpToPosition(new_i - 1);

    public void JumpToPosition(int i)
    {
        forceMove = true;
        forceMoveTo = i % transform.childCount;
    }

    void MenuItemChanged()
    {
        OnSwipeMenuSelectionChanged.Invoke(new_i);
    }
}
