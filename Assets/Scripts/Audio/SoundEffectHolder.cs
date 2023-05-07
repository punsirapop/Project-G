using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectHolder : MonoBehaviour
{
    private string _SoundEffectName = "Button"; // Name of the sound effect to play

    public void PlaySoundEffect(string soundEffect)
    {
        if(soundEffect == null)
        {
            soundEffect = _SoundEffectName;
        }
        SoundEffectManager.Instance.PlaySoundEffect(soundEffect);
    }
}
