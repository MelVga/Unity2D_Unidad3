using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private float moveX;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        anim.SetFloat("Speed", Mathf.Abs(moveX));
        anim.SetBool("Grounded", isGrounded);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("Jump");
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
    }
}
