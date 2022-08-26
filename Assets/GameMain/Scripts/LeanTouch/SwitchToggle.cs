using System.Collections;
using System.Collections.Generic;
using MoreMountains.MMInterface;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class SwitchToggle : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
    [Header("Switch")]
    public MMSpriteReplace BackGround;
    public MMSpriteReplace Toggle;
    public GameObject TextOn;
    public GameObject TextOff;
    public enum SwitchStates { Left, Right }
    [HideInInspector]
    public SwitchStates CurrentSwitchState = SwitchStates.Right;
    [Tooltip("初始方向，一般右侧为on")]
    public SwitchStates InitialState = SwitchStates.Right;

    [Header("Binding")]
    /// the methods to call when the switch is turned on
    public UnityEvent SwitchOn;
    /// the methods to call when the switch is turned off
    public UnityEvent SwitchOff;

    private float toggleX;
    protected RectTransform _rectTransform;

    void Awake()
    {
        InitializeState();

    }

    public virtual void InitializeState()
    {
        if (Toggle != null)
        {
            _rectTransform = Toggle.GetComponent<RectTransform>();
            toggleX = Mathf.Abs(_rectTransform.localPosition.x);
        }
        CurrentSwitchState = InitialState;

    }

    public bool IsOn
    {
        get
        {
            return CurrentSwitchState == SwitchStates.Right;
        }
        set
        {
            CurrentSwitchState = value ? SwitchStates.Right : SwitchStates.Left;
            Init();
        }
    }

    public void Init()
    {
        if (CurrentSwitchState == SwitchStates.Left)
        {
            SwitchToOff();
        }
        else
        {
            SwitchToOn();
        }
    }

    public virtual void Switch()
    {
        if (CurrentSwitchState == SwitchStates.Left)
        {
            SwitchToOn();
        }
        else
        {
            SwitchToOff();
        }
    }

    public void SwitchToOn()
    {
        CurrentSwitchState = SwitchStates.Right;
        BackGround?.SwitchToOnSprite();
        Toggle?.SwitchToOnSprite();
        SwitchOn?.Invoke();
        ToggleToRight();
        TextOn?.SetActive(true);
        TextOff?.SetActive(false);
    }

    public void SwitchToOff()
    {
        CurrentSwitchState = SwitchStates.Left;
        BackGround?.SwitchToOffSprite();
        Toggle?.SwitchToOffSprite();
        SwitchOff?.Invoke();
        ToggleToLeft();
        TextOn?.SetActive(false);
        TextOff?.SetActive(true);
    }

    public void ToggleToLeft()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (_rectTransform == null)
            {
                _rectTransform = Toggle.GetComponent<RectTransform>();
            }
            toggleX = Mathf.Abs(_rectTransform.localPosition.x);
            _rectTransform.localPosition = new Vector3(-toggleX, 0, 0);
        }
#endif
        _rectTransform.DOLocalMoveX(-toggleX, 0.1f).SetUpdate(true);
    }

    public void ToggleToRight()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (_rectTransform == null)
            {
                _rectTransform = Toggle.GetComponent<RectTransform>();
            }
            toggleX = Mathf.Abs(_rectTransform.localPosition.x);
            _rectTransform.localPosition = new Vector3(toggleX, 0, 0);
        }
#endif
        _rectTransform.DOLocalMoveX(toggleX, 0.1f).SetUpdate(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Switch();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Switch();
    }


}
