using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Capybara : MonoBehaviour
{
    // Popup heart
    [SerializeField] private Image _HeartFill;
    private Animator _Animator;
    // Basic information
    [SerializeField] private Image _CapybaraImage;
    private CapybaraSO _Capybara;
    private int _TapCount;
    // Movement
    private bool _IsFromRight;
    private int _CurrentSpriteIndex;
    private float _TimeBeforeSwap;
    private float _TimeCountBeforeSwap;

    private void Awake()
    {
        _Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_Capybara == null)
        {
            return;
        }
        float timeDelta = Time.deltaTime;
        // Swapping sprites
        _TimeCountBeforeSwap -= timeDelta;
        if (_TimeCountBeforeSwap <= 0)
        {
            _TimeCountBeforeSwap = _TimeBeforeSwap;
            _CurrentSpriteIndex++;
            if (_CurrentSpriteIndex > _Capybara.Sprites.Length - 1)
            {
                _CurrentSpriteIndex = 0;
            }
            _CapybaraImage.sprite = _Capybara.Sprites[_CurrentSpriteIndex];
        }
        // Shifting position
        float deltaX = _Capybara.WalkedPixelPerSeconds * timeDelta;
        deltaX = _IsFromRight ? -deltaX : deltaX;
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(
            rectTransform.anchoredPosition.x + deltaX, 
            rectTransform.anchoredPosition.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    public void SetCapybara(CapybaraSO capy, bool isFromRight)
    {
        _Capybara = capy;
        _HeartFill.color = capy.HeartColor;
        _TapCount = 0;
        _IsFromRight = isFromRight;
        if (isFromRight)
        {
            GetComponent<RectTransform>().localScale = new Vector3(-1, 1, 1); ;
        }
        _TimeBeforeSwap = 1/capy.StepPerSecond;
        _TimeCountBeforeSwap = _TimeBeforeSwap;
        
    }

    // Count the tap
    public void OnClickCapybara()
    {
        _TapCount++;
        _HeartFill.fillAmount = (float)_TapCount / (float)_Capybara.RequiredTapCount;
        _Animator.Rebind();
        _Animator.Update(0f);
        _Animator.Play("CapybaraHeartPopup");
        if (_TapCount >= _Capybara.RequiredTapCount)
        {
            _Capybara.Found();
            CapybaraManager.Instance.OpenOverlay(_Capybara);
            Destroy(gameObject);
        }
    }
}
