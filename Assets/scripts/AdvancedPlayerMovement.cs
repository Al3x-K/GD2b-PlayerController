using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class AdvancedPlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float jumpHeight = 7f; 
    public float dashSpeed = 20f;
    public float crouchHeight = 0.5f;
    public LayerMask whatIsGround; 
    public Transform groundCheckPoint; 
    public float groundCheckRadius = 0.2f;

    public AudioClip jumpSound; 
    public AudioClip dashSound; 
    public AudioClip footstepSound; 

    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 2;
    public LayerMask enemyLayers;

    private Rigidbody2D body; 
    private Animator anim; 
    private AudioSource audioSource; 
    public bool grounded; 
    public bool canDoubleJump = false;
    private bool isDashing = false; 
    private bool isCrouching = false;
    private bool facingRight = true; 


    
    // Start is called before the first frame update
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)&& grounded)
        {
            Jump();
            canDoubleJump = true; 
             
        }
        else if(Input.GetKeyDown(KeyCode.Space)&&canDoubleJump)
        {
            Jump();
            canDoubleJump = false;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);
        
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        body.velocity = new Vector2(horizontalInput*speed, body.velocity.y);
        anim.SetBool("Walk", horizontalInput !=0);

        if(horizontalInput != 0 && grounded)
        {
            PlaySound(footstepSound);
        }

        
        if((horizontalInput>0&& !facingRight)||(horizontalInput<0&&facingRight))
        {
            Flip();
        }

        if(Input.GetKey(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(Dash());
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow)&& grounded)
        {
            if(!isCrouching)
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
                isCrouching = true;
            }
             else if (isCrouching)
            {
                transform.localScale = new Vector3(transform.localScale.x,1f,transform.localScale.z);
                isCrouching = false; 
            }
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            Attack();
        }
        
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
    private void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale; 
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale; 
        facingRight = !facingRight; 
    }
    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpHeight);
        anim.SetTrigger("Jump");
        grounded = false;
        PlaySound(jumpSound);
    }

    IEnumerator Dash()
    {
        PlaySound(dashSound);
        float originalSpeed = speed; 
        speed = dashSpeed; 
        isDashing = true; 
        yield return new WaitForSeconds(0.2f);
        speed = originalSpeed; 
        isDashing = false; 
    }
    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                Debug.Log("The player took damage");
                enemyController.TakeDamage(attackDamage);
                
            }

        }
    }
    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); 
    }
}