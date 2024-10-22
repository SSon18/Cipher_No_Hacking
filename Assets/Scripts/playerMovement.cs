using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] public float speed;
    public Rigidbody2D body;
    private Animator anim;
    private bool grounded;

    private void Awake()
    {
        // Grabbing reference for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        // Flipping character's position
        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKey(KeyCode.Space) && grounded)
        {
            // Jump();
        }

        //// Setting animator parameters
        //anim.SetBool("run", horizontalInput != 0);
        //anim.SetBool("onGround", grounded);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }
}
