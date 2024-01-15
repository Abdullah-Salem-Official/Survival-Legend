using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Check if the animation has reached the last frame
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            // Detach the child objects from the parent before destroying it
            Transform[] childObjects = GetComponentsInChildren<Transform>();
            foreach (Transform child in childObjects)
            {
                child.SetParent(null);
            }

            Destroy(gameObject);
        }
    }
}
