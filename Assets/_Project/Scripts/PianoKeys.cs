using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PianoKeys : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float pitch;

    public void PlayKey()
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioClip);
    }
}
