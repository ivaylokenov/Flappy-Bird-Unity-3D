using UnityEngine;

public class BirdController : MonoBehaviour
{
    public float flapSpeed = 1000f;
    public float maxFlapSpeed = 100f;
    public float forwardSpeed = 100f;

    public AudioClip flapSound;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    private bool didFlap;
    private bool isDead;

    private bool gameStarted;

    private Vector2 originalPosition;

    private GameObject startButton;

    private int score = 0;

    public void Start()
    {
        this.rb = this.GetComponent<Rigidbody2D>();
        this.animator = this.GetComponent<Animator>();
        this.audioSource = this.GetComponent<AudioSource>();

        this.startButton = GameObject.Find("StartButton");
        this.originalPosition = new Vector2(
            this.transform.position.x,
            this.transform.position.y);

        this.rb.gravityScale = 0;
        this.forwardSpeed = 0;
        this.animator.enabled = false;
    }

    // read input, change graphics
    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isDead)
            {
                if (!this.gameStarted)
                {
                    var renderer = startButton.GetComponent<SpriteRenderer>();
                    renderer.enabled = false;
                    this.forwardSpeed = 5;
                    this.rb.gravityScale = 1;
                    this.animator.enabled = true;
                }

                this.didFlap = true;
                this.audioSource.PlayOneShot(this.flapSound);
            }
            else
            {
                Application.LoadLevel("Play");
            }
        }
    }

    // apply physics
    public void FixedUpdate()
    {
        var velocity = this.rb.velocity;
        velocity.x = this.forwardSpeed;
        this.rb.velocity = velocity;

        if (this.rb.velocity.y > 0)
        {
            this.rb.MoveRotation(30);
        }
        else if (!isDead)
        {
            var angle = velocity.y * 8;
            if (angle < -90)
            {
                angle = -90;
            }
            this.rb.MoveRotation(angle);
        }

        if (didFlap)
        {
            didFlap = false;
            this.rb.AddForce(new Vector2(0, flapSpeed), ForceMode2D.Impulse);

            var updatedVelocity = this.rb.velocity;
            if (updatedVelocity.y > this.maxFlapSpeed)
            {
                updatedVelocity.y = this.maxFlapSpeed;
                this.rb.velocity = updatedVelocity;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.CompareTag("Floor") || collider.gameObject.CompareTag("PipeCollision"))
        {
            this.isDead = true;
            this.animator.SetBool("BirdDead", true);
            this.forwardSpeed = 0;

            var currentHighScore = PlayerPrefs.GetInt("HighScore", 0);

            if (this.score > currentHighScore)
            {
                PlayerPrefs.SetInt("HighScore", this.score);
            }

            var renderer = startButton.GetComponent<SpriteRenderer>();
            renderer.enabled = true;

            var startButtonX = Camera.main.transform.position.x;
            var startButtonY = Camera.main.transform.position.y;

            var startButtonPosition = this.startButton.transform.position;
            startButtonPosition.x = startButtonX;
            startButtonPosition.y = startButtonY;
            this.startButton.transform.position = startButtonPosition;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pipe"))
        {
            this.score++;
            Debug.Log(score);
        }
    }
}
