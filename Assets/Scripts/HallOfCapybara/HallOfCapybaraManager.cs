using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HallOfCapybaraManager : MonoBehaviour
{
    [SerializeField] private Transform _CapybaraImageHolder;
    [SerializeField] private GameObject _CapybaraImagePrefab;

    private void Start()
    {
        foreach (Transform child in _CapybaraImageHolder)
        {
            Destroy(child.gameObject);
        }
        foreach (CapybaraSO capybara in PlayerManager.CapybaraDatabase.GetAllCapybara(true))
        {
            GameObject newCapybaraImage = Instantiate(_CapybaraImagePrefab, _CapybaraImageHolder);
            newCapybaraImage.GetComponent<Image>().sprite = capybara.Sprites[0];
            if (capybara.LockStatus == LockableStatus.Lock)
            {
                newCapybaraImage.GetComponent<Image>().color = Color.black;
            }
            else
            {
                newCapybaraImage.GetComponent<Image>().color = Color.white;
            }
            newCapybaraImage.GetComponent<CapybaraImage>().SetImage(capybara.Name, capybara.FoundCount);
        }
    }
}
