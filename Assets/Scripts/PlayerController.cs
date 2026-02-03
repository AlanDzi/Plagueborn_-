using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 9f;
    public float jumpForce = 12f;
    public float mouseSensitivity = 2f;
    public float gravityMultiplier = 2.5f;

    [Header("Stamina System")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 15f;
    public float sprintStaminaCost = 20f;
    public float jumpStaminaCost = 15f;
    public float attackStaminaCost = 10f;
    public float minStaminaToSprint = 10f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask = -1;

    [Header("Head Bob")]
    public float bobFrequency = 2.5f;
    public float bobAmount = 0.15f;
    public float sprintBobMultiplier = 1.8f;

    [Header("Audio")]
    public AudioClip[] footstepSounds;
    public float footstepVolume = 0.5f;
    public float walkStepInterval = 0.5f;
    public float sprintStepInterval = 0.3f;

    private Camera playerCamera;
    private Rigidbody rb;
    private bool isGrounded;
    private bool isSprinting;
    private bool canSprint;
    private AudioSource audioSource;

    private float xRotation = 0f;

    private float bobTimer = 0f;
    private Vector3 originalCameraPos;

    private float lastFootstepTime = 0f;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;

        currentStamina = maxStamina;

        Physics.gravity = new Vector3(0, -9.81f * gravityMultiplier, 0);

        originalCameraPos = playerCamera.transform.localPosition;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f;
        audioSource.volume = footstepVolume;
    }

    void Update()
    {
        
        if (UIManager.Instance != null && UIManager.Instance.IsAnyUIOpen)
            return;

        HandleStamina();
        HandleMouseLook();
        HandleMovement();
        HandleJump();
        HandleHeadBob();
        HandleFootsteps();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    void HandleStamina()
    {
        canSprint = currentStamina >= minStaminaToSprint;

        if (isSprinting && isGrounded)
        {
            currentStamina -= sprintStaminaCost * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);
        }
        else if (!isSprinting)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Max(currentStamina, 0f);
    }

    public bool CanUseStamina(float amount)
    {
        return currentStamina >= amount;
    }

    void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        CheckGroundWithRaycast();

        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift);
        isSprinting = wantsToSprint && canSprint && isGrounded;

        if (wantsToSprint && !canSprint)
        {
            isSprinting = false;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        direction = direction.normalized;

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 moveVelocity = direction * currentSpeed;

        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }

    void CheckGroundWithRaycast()
    {
        RaycastHit hit;
        
        Collider playerCollider = GetComponent<Collider>();
        Vector3 rayOrigin = playerCollider.bounds.center;
        rayOrigin.y = playerCollider.bounds.min.y;
        
        Vector3 rayDirection = Vector3.down;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, groundCheckDistance, groundMask))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && CanUseStamina(jumpStaminaCost))
        {
            UseStamina(jumpStaminaCost);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (rb.linearVelocity.y < 0f && !isGrounded)
        {
            rb.AddForce(Vector3.down * gravityMultiplier * 2f, ForceMode.Acceleration);
        }
    }

    void HandleFootsteps()
    {
        if (!isGrounded) return;

        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float speed = horizontalVel.magnitude;

        if (speed > 0.5f)
        {
            float stepInterval = isSprinting ? sprintStepInterval : walkStepInterval;

            if (Time.time >= lastFootstepTime + stepInterval)
            {
                PlayFootstepSound();
                lastFootstepTime = Time.time;
            }
        }
    }

    void PlayFootstepSound()
    {
        if (footstepSounds == null || footstepSounds.Length == 0) return;
        if (audioSource == null) return;

        AudioClip footstepClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(footstepClip, footstepVolume);
    }

    void HandleHeadBob()
    {
        if (!isGrounded) return;

        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float speed = horizontalVel.magnitude;

        if (speed > 0.1f)
        {
            float frequency = bobFrequency;
            float amplitude = bobAmount;

            if (isSprinting)
            {
                frequency *= sprintBobMultiplier;
                amplitude *= sprintBobMultiplier;
            }

            bobTimer += Time.deltaTime * frequency * speed;
            float bobOffset = Mathf.Sin(bobTimer) * amplitude;

            Vector3 newPos = originalCameraPos;
            newPos.y += bobOffset;
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                newPos,
                Time.deltaTime * 12f
            );
        }
        else
        {
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                originalCameraPos,
                Time.deltaTime * 12f
            );
            bobTimer = 0f;
        }
    }

    void OnDrawGizmosSelected()
    {
        Collider playerCollider = GetComponent<Collider>();
        if (playerCollider != null)
        {
            Vector3 rayOrigin = playerCollider.bounds.center;
            rayOrigin.y = playerCollider.bounds.min.y;
            
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawRay(rayOrigin, Vector3.down * groundCheckDistance);
            Gizmos.DrawWireCube(rayOrigin + Vector3.down * groundCheckDistance, Vector3.one * 0.1f);
        }
    }
}