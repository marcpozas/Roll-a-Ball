using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public float boostMultiplier = 8f;
    public float jumpForce = 5f;
    public AudioClip coinSound;
    public AudioClip jumpSound;
    public AudioClip fireSound;
    public AudioClip boxSound;
    public AudioClip boostSound;
    public GameObject gameOverPanel;

    private Rigidbody rb;
    private float movementX;
    private float movementY;
    private int count;
    private bool onRamp = false;
    private bool isGrounded = true;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
        gameOverPanel.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    public void OnJump(InputValue value)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            audioSource.PlayOneShot(jumpSound);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            audioSource.PlayOneShot(fireSound);
            GameOver();
        }
        else if (collision.gameObject.CompareTag("Box"))
        {
            audioSource.PlayOneShot(boxSound);
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            audioSource.PlayOneShot(boxSound);
        }
    }

    void FixedUpdate()
    {
        if (movementX != 0 || movementY != 0)
        {
            float currentSpeed = onRamp ? speed * boostMultiplier : speed;
            Vector3 movement = new Vector3(movementX, 0.0f, movementY);
            rb.AddForce(movement * currentSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
            audioSource.PlayOneShot(coinSound);
        }
        else if (other.gameObject.CompareTag("Lava"))
        {
            audioSource.PlayOneShot(fireSound);
            GameOver();
        }
        else if (other.gameObject.CompareTag("Ramp"))
        {
            onRamp = true;
            audioSource.PlayOneShot(boostSound);
        }
        else if (other.gameObject.CompareTag("Box"))
        {
            audioSource.PlayOneShot(boxSound);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ramp"))
        {
            onRamp = false;
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString() + " / 12";
        if (count >= 12)
        {
            //winTextObject.SetActive(true);
            //GetComponent<MeshRenderer>().enabled = false;
            //GetComponent<Collider>().enabled = false;

            //rb.velocity = Vector3.zero;
            //rb.isKinematic = true;

            //this.enabled = false;
            winTextObject.SetActive(true);
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            StartCoroutine(WinAnimation());
            this.enabled = false;
        }
    }

    void GameOver()
    {
        //GetComponent<MeshRenderer>().enabled = false;
        //GetComponent<Collider>().enabled = false;
        StartCoroutine(DeathAnimation());


        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        this.enabled = false;

        gameOverPanel.SetActive(true);
    }

    IEnumerator WinAnimation()
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.Rotate(Vector3.up * 360 * Time.deltaTime);
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
    }

    IEnumerator DeathAnimation()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.Rotate(Vector3.up * 600 * Time.deltaTime);
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
    }
}
