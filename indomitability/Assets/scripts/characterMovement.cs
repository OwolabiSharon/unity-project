using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] float airSlamPoints = 2f;
    [SerializeField] float triggerPoints = 1f;
    [SerializeField] float jerkForward = 1f;
    
    //public TextMeshProUGUI ScoreText;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    CapsuleCollider2D myBodyCollider;
    Animator myAnimator;
    AnimatorStateInfo stateInfo;
    bool doubleJumped;
    float gravityScaleAtStart;
    bool isAlive = true;
    float hpBar = 100;
    float totalPoints = 0;
    private InputAction action;
    public InputActionAsset inputActionAsset;
    

    // Start is called before the first frame update
    void Start()
    {

        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
        action = inputActionAsset.FindAction("Slide");
        InvokeRepeating("slideBuffer", 2.0f, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = myAnimator.GetCurrentAnimatorStateInfo(0);
        //point system
        //ScoreText.text = totalPoints.ToString();
        if (totalPoints > PlayerPrefs.GetFloat("highscore"))
        {
            PlayerPrefs.SetFloat("highscore", totalPoints );
        }
        //slide action
        action.canceled += ctx => {
            myAnimator.SetBool("sliding", false);
       };

        //up and down jumping animation
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
        
        //constant running
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
        //death
        if (hpBar <= 0)
        {
            Die();
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
            myAnimator.SetBool("falling", false);
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
        if (other.gameObject.tag == "balloon")
        {
            Destroy(other.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "causeDamage")
            {
                if (stateInfo.IsName("airSlam")) 
                {
                    totalPoints += airSlamPoints;
                    myAnimator.SetBool("jumping", true);
                    myRigidbody.velocity = new Vector2 (runSpeed, jumpHeight);  
                    extraJumps += 1;
                    Destroy(other.gameObject);
                }else
                {
                    myRigidbody.velocity = new Vector2 (jerkForward, jerkForward); 
                    myAnimator.SetTrigger("tookDamage");
                    hpBar -= 20f;    
                }
            }
         if (other.gameObject.tag == "extraPoints")
         {
                totalPoints += triggerPoints;
         }
    }

    void OnSlide(InputValue value)
    {
        if(value.isPressed && myBodyCollider.IsTouchingLayers(LayerMask.GetMask("ground")))
        {
            myAnimator.SetBool("sliding", true);
            myRigidbody.velocity += new Vector2 (slide, 0f);  

        }
    }

    void slideBuffer()
    {
        //Debug.Log(action.triggered);
    }

    void OnAirSlam(InputValue value)
    {
        myAnimator.SetTrigger("airSlam");
        if(value.isPressed && !myBodyCollider.IsTouchingLayers(LayerMask.GetMask("ground")))
        {
            myAnimator.SetBool("jumping", false);
            myAnimator.SetTrigger("airSlam");
            myRigidbody.velocity = new Vector2 (runSpeed, (-1 * airSlam));
            // myRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            // StartCoroutine(WaitAndRunFunction());
        }
        myAnimator.SetTrigger("airSlam");
    }

    void Die()
    {
        isAlive = false;
        myAnimator.SetTrigger("death");
    }

     // private IEnumerator WaitAndRunFunction()
    // {
    //     //yield return new WaitForSeconds(0.2f);
    //     yield return new WaitForSeconds(0f);
    //     myRigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
    //     myRigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    //     //myRigidbody.velocity = new Vector2 (0f, (-1 * airSlam)); 
         
    // }

    public void OnButtonReleased(InputAction.CallbackContext ctx)
    {
        if (!action.triggered)
        {
            // Code to run when the button is released
        }
    }
}
