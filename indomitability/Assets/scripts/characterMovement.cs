using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class characterMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpHeight = 10f;
    [SerializeField] float slide = 1f;
    [SerializeField] float extraGrav = 5f;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    BoxCollider2D myBodyCollider;
    Animator myAnimator;
    bool doubleJumpPossible;
    float gravityScaleAtStart;
    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        float maxHeight = 0;
        
        if (myRigidbody.velocity.y > 0)
        {
           myAnimator.SetBool("jumping", true);
           maxHeight = myRigidbody.velocity.y;
        } else if (myRigidbody.velocity.y < maxHeight)
        {
            myAnimator.SetBool("jumping", false);
            myAnimator.SetBool("falling", true);
            maxHeight = myRigidbody.velocity.y;
        } else if (maxHeight == 0 && myRigidbody.velocity.y == maxHeight)
        {
            myAnimator.SetTrigger("landing");
            maxHeight = myRigidbody.velocity.y;
        }
        

        if(runSpeed < 10f)
        {
            runSpeed += 0.0001f;
        }
        
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("ground")))
        {
             myAnimator.SetBool("falling", false);
            myAnimator.SetBool("isGrounded", true);
            Vector2 playerVelocity = new Vector2 (runSpeed, myRigidbody.velocity.y);
            myRigidbody.velocity = playerVelocity;
        }else
        {
            myAnimator.SetBool("isGrounded", false);
        }
    }

    void OnJump(InputValue value)
    {
        if(value.isPressed && myBodyCollider.IsTouchingLayers(LayerMask.GetMask("ground")))
        {
            myAnimator.SetBool("jumping", true);
            myRigidbody.velocity = new Vector2 (0f, jumpHeight);  
            doubleJumpPossible = true;
        }
        else if(value.isPressed && doubleJumpPossible)
        {
            myAnimator.SetTrigger("doubleJump");
            myRigidbody.velocity = new Vector2 (myRigidbody.velocity.x, jumpHeight);   
            doubleJumpPossible = false;
        }
        
    }

    void OnSlide(InputValue value)
    {
        if(value.isPressed && myBodyCollider.IsTouchingLayers(LayerMask.GetMask("ground")))
        {
            myAnimator.SetTrigger("sliding");
            myRigidbody.velocity = new Vector2 (slide, 0f);  
        }
    }
}
