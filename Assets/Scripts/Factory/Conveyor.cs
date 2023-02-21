using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    private WeaponChromosome[] _AllWeapon;
    private WeaponChromosome[] _RenderedWeapon;
    [SerializeField] private GameObject _WeaponImagePrefab;
    [SerializeField] private int _RenderedCount;
    [SerializeField] private int _HiddenCount;
    private int _StartIndex;

    public void Start()
    {
        _AllWeapon = FactoryManager.Instance.GetAllWeapon();
        _RenderedWeapon = new WeaponChromosome[_RenderedCount];
        _StartIndex = 0;
        ResetRendering();
    }

    // Reset item icon on conveyor to animate smoothly
    public void ResetRendering()
    {
        foreach (Transform item in this.transform)
        {
            Destroy(item.gameObject);
        }
        _StartIndex += _HiddenCount;
        if (_StartIndex >= _AllWeapon.Length)
        {
            _StartIndex = _StartIndex - _AllWeapon.Length;
        }
        int renderIndex = _StartIndex;
        // Reset all rendered Weapon
        for (int i = 0; i < _RenderedCount; i++)
        {
            if (renderIndex >= _AllWeapon.Length)
            {
                renderIndex = 0;
            }
            _RenderedWeapon[i] = _AllWeapon[renderIndex];
            renderIndex++;
        }
        // Instantiate actual gameobject
        foreach (WeaponChromosome chromosome in _RenderedWeapon)
        {
            GameObject me = Instantiate(_WeaponImagePrefab, this.transform);
            me.GetComponent<SpriteRenderer>().sprite = chromosome.Image;
        }
    }
}
