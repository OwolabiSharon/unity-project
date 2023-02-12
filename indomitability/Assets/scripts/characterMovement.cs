using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class characterMovement : MonoBehaviour
{
    public static characterMovement instance;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float jumpHeight = 100f;
    [SerializeField] float slide = 1f;
    [SerializeField] float airSlam = 3f;
    [SerializeField] float fallSpeed = 1f;
    [SerializeField] float extraJumps = 1f;
    [SerializeField] float airSlamPoints = 2f;
    [SerializeField] float triggerPoints = 1f;
    [SerializeField] float jerkForward = 1f;
    [SerializeField] float balloonForce_x = 1f;
    [SerializeField] float balloonForce_y = 1f;
    [SerializeField] Transform playerHurt;
    public Transform collectCoin;
    [SerializeField] Transform balloon;
    
    //public TextMeshProUGUI ScoreText;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    BoxCollider2D myBodyTrigger;
    CapsuleCollider2D myBodyCollider;
    Animator myAnimator;
    AnimatorStateInfo stateInfo;
    SpriteRenderer spriteRenderer;
    Color originalColor;
    bool doubleJumped;
    bool bufferedSlide;
    float gravityScaleAtStart;
    bool isAlive = true;
    bool isInvinsible = false;
    public float hpBar = 100;
    public float totalPoints = 0;
    public float coins = 0;
    private InputAction action;
    public InputActionAsset inputActionAsset;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI pointText;
    public bool isPlaying = false;
    // public GameObject mainMenu;
    // public GameObject gamePlay;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
       // MenuReference = FindObjectOfType<Menu>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myBodyTrigger = GameObject.Find ("playerTriggerCollider").GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
        action = inputActionAsset.FindAction("Slide");
        if(!isPlaying)
        {
             myAnimator.SetBool("isPlaying", false);
             myAnimator.SetBool("isIdle", true);
        }
       
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
        action.canceled += ctx => {
    if (myAnimator != null) {
        myAnimator.SetBool("sliding", false);
    }
};
       if (action.ReadValue<float>() > 0)
       {
           myAnimator.SetBool("sliding", true);
           bufferedSlide = true;
       }else{
            bufferedSlide = false;
       }
       if(bufferedSlide && myBodyCollider.IsTouchingLayers(LayerMask.GetMask("ground")))
        {
            slideFunction();
        }

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
            myAnimator.SetBool("jumping", false);
            myAnimator.SetTrigger("landing");
            maxHeight = myRigidbody.velocity.y;
        }
        
        //constant running
        if(runSpeed < maxSpeed)
        {
            runSpeed += 0.0001f;
        }
        
        //if is grounded
        if (myBodyTrigger.IsTouchingLayers(LayerMask.GetMask("ground")) && isPlaying)
        {
            whileGrounded();
            myAnimator.SetBool("isPlaying", true);
            myAnimator.SetBool("isIdle", false);
            Vector2 playerVelocity = new Vector2 (runSpeed, myRigidbody.velocity.y);
            myRigidbody.velocity = playerVelocity;
        }else if(myBodyTrigger.IsTouchingLayers(LayerMask.GetMask("ground")) && !isPlaying)
        {
            whileGrounded();
        }
        else
        {
            myAnimator.SetBool("isGrounded", false);
        }
        //death
        if (hpBar <= 0)
        {
            Die();
            // gamePlay.SetActive(false);
            // mainMenu.SetActive(true);
            StartCoroutine(LoadNextLevel());
        }
    }
    IEnumerator LoadNextLevel()
    {   
        yield return new WaitForSecondsRealtime(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void whileGrounded()
    {
        myAnimator.SetBool("jumping", false);
        myAnimator.SetBool("falling", false);
        myAnimator.SetBool("isGrounded", true);
    }
    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if(value.isPressed && myBodyCollider.IsTouchingLayers(LayerMask.GetMask("ground")))
        {
            myAnimator.SetBool("jumping", true);
            myRigidbody.velocity = new Vector2 (myRigidbody.velocity.x, jumpHeight);  
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
    }
    
    void gainPoints(float points)
    {
        totalPoints += points;
        pointText.text = $"Points: {totalPoints}";
    }

    void OnTriggerEnter2D(Collider2D other) {
        //balloon
        if (other.gameObject.tag == "balloon")
        {
            Transform particle = other.transform.Find("particles");
            Vector3 particleSpawn = particle.position;
            Instantiate(balloon,particleSpawn,Quaternion.identity);
            if (extraJumps == 0)
            {
                extraJumps += 1;
            } 
            myRigidbody.velocity = new Vector2 (balloonForce_x, balloonForce_y); 
            if (stateInfo.IsName("airSlam")) 
            {
                gainPoints(airSlamPoints);
            }
            Destroy(other.gameObject);
        }

        //damage causing interactables
        if (other.gameObject.tag == "causeDamage" || other.gameObject.tag == "tree" )
            {
                Transform particle = other.transform.Find("particles");
                Vector3 particleSpawn = particle.position;
                
                //other.GetComponentInChildren<ParticleSystem>().Play();
                //partic.play()
                if (stateInfo.IsName("airSlam") && other.gameObject.tag == "causeDamage") 
                {
                    gainPoints(airSlamPoints);
                    myAnimator.SetBool("jumping", true);
                    myRigidbody.velocity = new Vector2 (runSpeed, jumpHeight); 
                    if (extraJumps == 0)
                    {
                        extraJumps += 1;
                    } 
                    Instantiate(playerHurt,particleSpawn,Quaternion.identity);
                    Destroy(other.gameObject);
                }else
                {
                    if (isInvinsible) { return; }
                    myRigidbody.velocity = new Vector2 (myRigidbody.velocity.x, jerkForward); 
                    myAnimator.SetTrigger("tookDamage");
                    hpBar -= 20f;    
                    hpText.text = $"HP: {hpBar}";
                    Instantiate(playerHurt,particleSpawn,Quaternion.identity);
                    StartCoroutine(Invinsibility());
                }
            }
         if (other.gameObject.tag == "extraPoints")
         {
            gainPoints(triggerPoints);
         }
    }

    void OnSlide(InputValue value)
    {
        if(value.isPressed && myBodyCollider.IsTouchingLayers(LayerMask.GetMask("ground")))
        {
           slideFunction();
        }
    }

    void slideFunction()
    {
       myAnimator.SetBool("sliding", true);
       //myRigidbody.velocity += new Vector2 (slide, 0f); 
    }

    void OnAirSlam(InputValue value)
    {
        if (bufferedSlide) { return; }
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
        myRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        myAnimator.SetTrigger("death");
    }

    private IEnumerator Invinsibility()
    {
        Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a * 0.5f);
        spriteRenderer.color = newColor;
        isInvinsible = true;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = originalColor;
        isInvinsible = false;
        //myRigidbody.velocity = new Vector2 (0f, (-1 * airSlam)); 
         
    }
}
