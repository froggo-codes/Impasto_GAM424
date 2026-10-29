using UnityEngine;

//Defines the Door.Open method.

public class Door : MonoBehaviour
{
    [SerializeField] private AudioClip _openSound;
    
    private Animator _animator;
    private AudioSource _audioSource;
    private bool _isOpen;

    private void Awake()
    {
        //load animator component
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }
    
    [ContextMenu(itemName:"Open Door")]
    public void Open()
    {
        if (_isOpen) return;
        _isOpen = true;
        
        //door open logic
        _animator.SetTrigger(name:"Open");
        _audioSource.PlayOneShot(_openSound);
    }
}
