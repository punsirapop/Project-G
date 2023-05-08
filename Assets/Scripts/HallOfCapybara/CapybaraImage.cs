using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CapybaraImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _NameText;
    [SerializeField] private TextMeshProUGUI _FoundCountText;
    private bool _IsUnlock;

    private void OnEnable()
    {
        _NameText.gameObject.SetActive(false);
        _FoundCountText.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_IsUnlock)
        {
            GetComponent<Image>().color = Color.gray;
            _NameText.gameObject.SetActive(true);
            _FoundCountText.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_IsUnlock)
        {
            GetComponent<Image>().color = Color.white;
            _NameText.gameObject.SetActive(false);
            _FoundCountText.gameObject.SetActive(false);
        }
    }

    public void SetImage(string name, int count)
    {
        _NameText.text = name;
        _FoundCountText.text = count.ToString();
        _IsUnlock = (count > 0);
    }
}
