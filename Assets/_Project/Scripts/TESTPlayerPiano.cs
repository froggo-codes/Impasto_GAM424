using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPiano : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float pitch;

    void PlayKey()
    {
      //  audioSource.pitch = pitch;
      //  audioSource.PlayOneShot(audioClip);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playKeys()
    {
      //  if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
      //      moveInput -= 1f;

       // if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        //    moveInput += 1f;


      //  if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
       //     climbInput += 1f;

      //  if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        //    climbInput -= 1f;

      //  if (Keyboard.current.spaceKey.wasPressedThisFrame)
       //     jumpPressed = true;

       // if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        //    dashPressed = true;
    }
}
