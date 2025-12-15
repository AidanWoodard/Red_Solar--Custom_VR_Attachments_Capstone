using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimator : MonoBehaviour
{
    [SerializeField] private InputActionReference playerInput;
    private Animator animator;

    float gripTarg = 0;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator not found");
        }
    }

    void Update()
    {
        gripTarg = Mathf.Lerp(gripTarg, playerInput.action.ReadValue<float>(), .5f);
        animator.SetFloat("Grip", gripTarg);
        animator.SetFloat("Blend", gripTarg);
    }
}
