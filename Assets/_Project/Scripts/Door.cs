using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        //load animator component
        _animator = GetComponent<Animator>();
    }
    
    [ContextMenu(itemName:"Open Door")]
    public void Open()
    {
        //door open logic
        _animator.SetTrigger(name:"Open");
    }
}
