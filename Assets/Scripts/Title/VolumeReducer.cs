using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeReducer : MonoBehaviour
{
    [SerializeField] Slider _BgmSlider, _SfxSlider;

    private void Awake()
    {
        _BgmSlider.value = MusicManager.Volume;
        _SfxSlider.value = SoundEffectManager.Volume;
    }

    public void ChangeVolume(bool isBgm)
    {
        if (isBgm) MusicManager.Volume = _BgmSlider.value;
        else SoundEffectManager.Volume = _SfxSlider.value;
    }
}
