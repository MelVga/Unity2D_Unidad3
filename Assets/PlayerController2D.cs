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
        // Movimiento horizontal
        moveX = Input.GetAxisRaw("Horizontal");

        // Voltear el personaje según la dirección
        if (moveX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Detectar si está en el suelo
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Enviar parámetros al Animator
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveX));
            anim.SetBool("Grounded", isGrounded);
        }

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (anim != null)
            {
                anim.SetTrigger("Jump");
            }
        }

        // Disparo con tecla F
        if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Falta asignar bulletPrefab o firePoint en el Inspector");
            return;
        }

        GameObject bulletObj = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Bullet bullet = bulletObj.GetComponent<Bullet>();

        if (bullet != null)
        {
            Vector2 direction = transform.localScale.x > 0
                ? Vector2.right
                : Vector2.left;

            bullet.Fire(direction);
        }
        else
        {
            Debug.LogWarning("El prefab de la bala no tiene el script Bullet");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}

