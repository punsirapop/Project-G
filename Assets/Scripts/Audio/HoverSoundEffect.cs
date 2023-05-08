using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverSoundEffect : MonoBehaviour, IPointerEnterHandler
{
    public string _SoundEffectName = "Hover"; // Name of the sound effect to play

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundEffectManager.Instance.PlaySoundEffect(_SoundEffectName);
    }
}
