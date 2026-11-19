using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class PianoKeys : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float pitch;
    [SerializeField] private Image piano;
    [SerializeField] private Sprite pianoPressed;

    public void PlayKey()
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioClip);
        piano.overrideSprite = pianoPressed;
        StartCoroutine(ResetPianoSprite());
    }

    private IEnumerator ResetPianoSprite()
    {
        yield return new WaitForSeconds(0.1f);
        piano.overrideSprite = null;
    }
}
    
