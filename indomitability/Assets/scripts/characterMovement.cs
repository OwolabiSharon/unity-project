using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class characterMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float jumpHeight = 100f;
    [SerializeField] float slide = 1f;
    [SerializeField] float airSlam = 3f;
    [SerializeField] float fallSpeed = 1f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float extraJumps = 1f;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    BoxCollider2D myBodyCollider;
    Animator myAnimator;
    bool doubleJumped;
    float gravityScaleAtStart;
    bool isAlive = true;
    AnimatorStateInfo stateInfo;
    

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
        stateInfo = myAnimator.GetCurrentAnimatorStateInfo(0);
        if (!isAlive) { return; }
        float maxHeight = 0;
        
        if (myRigidbody.velocity.y > 0)
        {
           myAnimator.SetBool("jumping", true);
           maxHeight = myRigidbody.velocity.y;
        } else if (myRigidbody.velocity.y < maxHeight)
        {
            myAnimator.SetBool("jumping", false);
            myAnimator.SetBool("falling", true);
            myRigidbody.velocity -= new Vector2 (0f, fallSpeed);
            maxHeight = myRigidbody.velocity.y;
        } else if (maxHeight == 0 && myRigidbody.velocity.y == maxHeight)
        {
            myAnimator.SetTrigger("landing");
            maxHeight = myRigidbody.velocity.y;
        }
        

        if(runSpeed < maxSpeed)
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
        if (!isAlive) { return; }
        if(value.isPressed && myBodyCollider.IsTouchingLayers(LayerMask.GetMask("ground")))
        {
            myAnimator.SetBool("jumping", true);
            myRigidbody.velocity = new Vector2 (0f, jumpHeight);  
            extraJumps += 1;
        }
        else if(value.isPressed && extraJumps > 0)
        {
            myAnimator.SetTrigger("doubleJump");
            myRigidbody.velocity = new Vector2 (myRigidbody.velocity.x, jumpHeight);   
            extraJumps -= 1;
        }
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (extraJumps > 0)
        {
            extraJumps -= 1;
        }
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "causeDamage")
            {
                if (stateInfo.IsName("airSlam")) 
                {
                    myAnimator.SetBool("jumping", true);
                    myRigidbody.velocity = new Vector2 (0f, jumpHeight);  
                    extraJumps += 1;
                    Destroy(other.gameObject);
                }else
                {
                    myAnimator.SetTrigger("tookDamage");
                    Die();
                }
            }
    }

    void OnSlide(InputValue value)
    {
        if(value.isPressed && myBodyCollider.IsTouchingLayers(LayerMask.GetMask("ground")))
        {
            myAnimator.SetTrigger("sliding");
            myRigidbody.velocity = new Vector2 (slide, 0f);  

        }
        // else if (!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("ground")))
        // {
        //     myAnimator.SetBool("sliding", false);
        // }
    }

    void OnAirSlam(InputValue value)
    {
        if(value.isPressed && !myBodyCollider.IsTouchingLayers(LayerMask.GetMask("ground")))
        {
            myAnimator.SetTrigger("airSlam");
            myRigidbody.velocity = new Vector2 (0f, (-1 * airSlam)); 
        }
    }

    void Die()
    {
            isAlive = false;
    }
}
