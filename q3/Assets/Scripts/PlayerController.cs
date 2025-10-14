using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject startPanel;
    private bool canMove;
    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private float jumpImpulse = 6f;
    private float groundRadius = 0.5f;
    private bool isGrounded = false;

    // power up
    public GameObject powerUpObject;
    public TextMeshProUGUI powerUpText;
    private float speedBoost = 10f;
    private float baseSpeed;
    public Material playerMaterial;
    private Renderer rend;
    private Material baseMaterial;

    //break wall
    public GameObject northWallMid;

    //win check
    private PlayerInput playerInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        canMove = false;
        count = 0;

        playerInput = GetComponent<PlayerInput>();
        rend = GetComponent<Renderer>();
        baseMaterial = rend.material;
        baseSpeed = speed;
        SetCountText();
        winTextObject.SetActive(false);
        startPanel.SetActive(true);
        powerUpText.gameObject.SetActive(false);
        powerUpObject.SetActive(false);
    }

    public void StartGame()
    {
        canMove = true;
        rb.useGravity = true;
        startPanel.SetActive(false);
    }

    void Update()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer, QueryTriggerInteraction.Ignore);
        }
    }

    void OnMove(InputValue movementValue)
    {
        if (!canMove)
        {
            movementX = 0;
            movementY = 0;
            return;
        }
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }
    
    void OnJump(InputValue value)
    {
        if (!canMove) return;
        if (!value.isPressed) return;
        if (!isGrounded) return;
        rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 12)
        {
            powerUpObject.SetActive(true);
        }
        
    }

    System.Collections.IEnumerator HideAfterSeconds(GameObject go, float sec)
    {
        yield return new WaitForSeconds(sec);
        if (go != null) go.SetActive(false);
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
        else if (other.gameObject.CompareTag("Powerup"))
        {
            ApplyPowerUp();
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Checkpoint"))
        {
            other.gameObject.SetActive(false);
            playerInput.enabled = false;
            canMove = false;
            winTextObject.SetActive(true);
            StartCoroutine(HideAfterSeconds(winTextObject, 3f));
        }
    }

    void ApplyPowerUp()
    {
        speed = baseSpeed + speedBoost;
        rend.material = playerMaterial;
        powerUpText.gameObject.SetActive(true);
        northWallMid.SetActive(false);
        StartCoroutine(HideAfterSeconds(powerUpText.gameObject, 3f));
    }
}
