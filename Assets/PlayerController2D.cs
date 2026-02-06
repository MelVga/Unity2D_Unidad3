using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;

    private float moveX;
    private bool isGrounded;
    private float nextShootTime;

    // Guarda la última dirección hacia donde mira el personaje
    private Vector2 lastDirection = Vector2.right;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Movimiento horizontal
        moveX = Input.GetAxisRaw("Horizontal");

        // Guardar última dirección
        if (moveX > 0)
            lastDirection = Vector2.right;
        else if (moveX < 0)
            lastDirection = Vector2.left;

        // Detectar si está tocando el suelo
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Animaciones
        anim.SetFloat("Speed", Mathf.Abs(moveX));
        anim.SetBool("Grounded", isGrounded);

        // Saltar
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("Jump");
        }

        // Disparar con tecla F
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootCooldown;
        }

        // Voltear sprite visualmente
        if (moveX != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(moveX) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        // Instanciar bala en el centro del personaje
        GameObject bulletObj = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Bullet bullet = bulletObj.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Fire(lastDirection);
        }
    }
}
