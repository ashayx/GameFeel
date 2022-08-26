using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Events;

public class LeanTouchButton : MonoBehaviour
{

    [Header("Binding")]
    /// The method(s) to call when the button gets pressed down
    public UnityEvent ButtonPressedFirstTime;
    /// The method(s) to call when the button gets released
    public UnityEvent ButtonReleased;
    /// The method(s) to call while the button is being pressed
    //public UnityEvent ButtonPressed;
    [Header("Opacity")]
    /// the new opacity to apply to the canvas group when the button is pressed
    public float PressedOpacity = 0.3f;
    public float IdleOpacity = 1f;
    public float DisabledOpacity = 0.3f;

    [System.NonSerialized]
    private List<LeanFinger> fingers = new List<LeanFinger>();
    protected CanvasGroup _canvasGroup;
    protected float _initialOpacity;

    protected virtual void Awake()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup != null)
        {
            _initialOpacity = IdleOpacity;
            _canvasGroup.alpha = _initialOpacity;
            _initialOpacity = _canvasGroup.alpha;
        }
    }

    protected virtual void OnEnable()
    {
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerUp += HandleFingerUp;
        //LeanTouch.OnFingerUpdate += HandleFingerUpdate;

    }

    protected virtual void OnDisable()
    {
        LeanTouch.OnFingerDown -= HandleFingerDown;
        LeanTouch.OnFingerUp -= HandleFingerUp;
        //LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }

    private void HandleFingerDown(LeanFinger finger)
    {
        if (LeanTouch.ElementOverlapped(gameObject, finger.ScreenPosition) == false)
        {
            return;
        }

        if (finger.Index == LeanTouch.HOVER_FINGER_INDEX)
        {
            return;
        }

        fingers.Add(finger);

        if (fingers.Count == 1)
        {
            if (ButtonPressedFirstTime != null)
            {
                ButtonPressedFirstTime.Invoke();
            }
            SetOpacity(PressedOpacity);


        }
    }

    private void HandleFingerUp(LeanFinger finger)
    {
        if (fingers.Contains(finger) == false)
        {
            return;
        }


        fingers.Remove(finger);
        if (ButtonReleased != null)
        {
            ButtonReleased.Invoke();
        }
        SetOpacity(_initialOpacity);

    }

    protected virtual void SetOpacity(float newOpacity)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = newOpacity;
        }
    }
}
