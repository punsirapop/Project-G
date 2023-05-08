using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationRenderer : MonoBehaviour
{
    [SerializeField] private Image _CurrentImage;
    [SerializeField] private Button _PreviousImageButton;
    [SerializeField] private Button _NextImageButton;
    [SerializeField] ProgressIndicator _ProgressIndicator;
    private InformationSO _Information;
    private int _CurrentImageIndex;

    private void Awake()
    {
        _PreviousImageButton.interactable = true;
        _NextImageButton.interactable = true;
        _PreviousImageButton.onClick.AddListener(() => ChangeImage(-1));
        _NextImageButton.onClick.AddListener(() => ChangeImage(1));
    }

    private void Start()
    {
        _CurrentImageIndex = 0;
        ChangeImage(0);
        RenderImage();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void SetInformation(InformationSO newInformation)
    {
        _Information = newInformation;
    }

    public void RenderImage()
    {
        _CurrentImage.sprite = _Information.Images[_CurrentImageIndex];
        _ProgressIndicator.SetIndicator(_CurrentImageIndex, _Information.Images.Length - 1);
    }

    public void ChangeImage(int amount)
    {
        _CurrentImageIndex += amount;
        if (_CurrentImageIndex <= 0)
        {
            _CurrentImageIndex = 0;
            _PreviousImageButton.gameObject.SetActive(false);
        }
        else if (_CurrentImageIndex >= _Information.Images.Length - 1)
        {
            _CurrentImageIndex = _Information.Images.Length - 1;
            _NextImageButton.gameObject.SetActive(false);
        }
        else
        {
            _PreviousImageButton.gameObject.SetActive(true);
            _NextImageButton.gameObject.SetActive(true);
        }
        RenderImage();
    }
}
