using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PacmanMazeCharacterMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;

    public Slider slider;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successSound;


    [Header("Success Settings")]
    [SerializeField] private string targetTag = "mazeTarget";
    [SerializeField] private GameObject greatJobPopup;
    [SerializeField] private GameObject tryJobPopup;
     [SerializeField] private GameObject levelUpPopup;
    [SerializeField] private TMP_Text popupMessage;
    [SerializeField] private string successMessage = "Bravo! Great Job!";
    [SerializeField] private float restartDelay = 2f;
    [SerializeField] public GameObject[] targetObjectsArry; // Target object ko public banaya taaki inspector se assign kiya ja sake

    public Color[] colorsSlider; // Array of colors for each level
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private Vector3 originalScale;
    public bool hasReachedTarget = false;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 targetWorldPos;      // Jahan user ne touch kiya hai, woh world position
    public bool isMovingToTarget = false; // Kya player abhi target ki taraf ja raha hai?

    private void Awake()
    {
        originalScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();

        // Configure Rigidbody for smooth movement without gravity
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        // Auto-get AudioSource if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (greatJobPopup != null)
        {
            greatJobPopup.SetActive(false);
        }

        // Auto-find Slider in scene if not assigned in Inspector
        if (slider == null)
        {
            slider = FindFirstObjectByType<Slider>();
        }

        // Default colors if array is not configured
        if (colorsSlider == null || colorsSlider.Length == 0)
        {
            colorsSlider = new Color[]
            {
                Color.red,
                new Color(1f, 0.5f, 0f), // Orange
                Color.orange,
                Color.yellow,
                Color.green
            };
        }
    }

    void Start()
    {
        DisableAnimator();

        if (MazeTraverseManager.instance != null && targetObjectsArry != null)
        {
            int currentLevel = MazeTraverseManager.instance.mazeLevelIndex;
            if (currentLevel >= 0 && currentLevel < targetObjectsArry.Length)
            {
                if (targetObjectsArry[currentLevel] != null && targetObjectsArry[currentLevel].transform.parent != null)
                {
                    Animator anim = targetObjectsArry[currentLevel].transform.parent.GetComponent<Animator>();
                    if (anim != null)
                    {
                        anim.enabled = true;
                    }
                }
            }

            UpdateSlider(currentLevel);
        }
    }

    private void Update()
    {
        if (hasReachedTarget) return;

        Pointer currentPointer = Pointer.current;
        if (currentPointer == null) return;

        // Jab tap/click ho
        if (currentPointer.press.wasPressedThisFrame)
        {
            Vector2 screenPos = currentPointer.position.ReadValue();
            targetWorldPos = mainCamera.ScreenToWorldPoint(screenPos);
            Vector2 charPos = (Vector2)transform.position;

            Vector2 diff = targetWorldPos - charPos;

            // Direction decide karo
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                moveDirection = (diff.x > 0) ? Vector2.right : Vector2.left;
            }
            else
            {
                moveDirection = (diff.y > 0) ? Vector2.up : Vector2.down;
            }

            isMovingToTarget = true; // Movement start karo
        }
    }

    private void FixedUpdate()
    {
        if (hasReachedTarget)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isMovingToTarget)
        {
            Vector2 currentPos = (Vector2)transform.position;

            // Check karo ki player touch/target position par pahunch gaya ya cross karne wala hai
            bool hasPassedTarget = false;

            if (moveDirection == Vector2.up && currentPos.y >= targetWorldPos.y)
            {
                hasPassedTarget = true;
            }
            else if (moveDirection == Vector2.down && currentPos.y <= targetWorldPos.y)
            {
                hasPassedTarget = true;
            }
            else if (moveDirection == Vector2.right && currentPos.x >= targetWorldPos.x)
            {
                hasPassedTarget = true;
            }
            else if (moveDirection == Vector2.left && currentPos.x <= targetWorldPos.x)
            {
                hasPassedTarget = true;
            }

            if (hasPassedTarget)
            {
                // Target par pahunch gaya, ab ruk jao aur exact position par snap kar do
                StopMovementAtTarget();
            }
            else
            {
                // Move karte raho
                rb.linearVelocity = moveDirection * moveSpeed;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void StopMovementAtTarget()
    {
        rb.linearVelocity = Vector2.zero;

        // Exact touch position par snap kar do taaki aage na nikle
        Vector3 pos = transform.position;
        if (moveDirection == Vector2.up || moveDirection == Vector2.down)
        {
            pos.y = targetWorldPos.y;
        }
        else if (moveDirection == Vector2.left || moveDirection == Vector2.right)
        {
            pos.x = targetWorldPos.x;
        }
        transform.position = pos;

        isMovingToTarget = false;
        moveDirection = Vector2.zero;
    }

    private void LateUpdate()
    {
        // Ensure scale never changes
        if (transform.localScale != originalScale)
        {
            transform.localScale = originalScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckTarget(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckTarget(other);
    }

    private void CheckTarget(Collider2D other)
    {
        if (hasReachedTarget) return;

        if (other.CompareTag(targetTag))
        {
            hasReachedTarget = true;
            isMovingToTarget = false;
            moveDirection = Vector2.zero;
            rb.linearVelocity = Vector2.zero;

            // 1. Turn character Green
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.green;
            }

            // 2. Show Bravo / Great Job Popup
            if (popupMessage != null)
            {
                popupMessage.text = successMessage;
            }



            if (MazeTraverseManager.instance != null)
            {
                if (!MazeTraverseManager.instance.isheadPhone && other.gameObject.name == "Headphones" && MazeTraverseManager.instance.mazeLevelIndex == 0)
                {
                    MazeTraverseManager.instance.isheadPhone = true;
                    if (greatJobPopup != null)
                    {
                        greatJobPopup.SetActive(true);
                    }
                    other.transform.GetChild(0).gameObject.SetActive(true);
                    other.transform.GetComponent<BoxCollider2D>().enabled = false;
                    PlaySuccessAudio();
                    StartCoroutine(ReloadSceneAfterDelay());
                }
                else if (!MazeTraverseManager.instance.isbedsheet && other.gameObject.name == "blanket" && MazeTraverseManager.instance.mazeLevelIndex == 1)
                {
                    if (greatJobPopup != null)
                    {
                        greatJobPopup.SetActive(true);
                    }
                    MazeTraverseManager.instance.isbedsheet = true;
                    other.transform.GetChild(0).gameObject.SetActive(true);
                    other.transform.GetComponent<BoxCollider2D>().enabled = false;
                    PlaySuccessAudio();
                    StartCoroutine(ReloadSceneAfterDelay());
                }
                else if (!MazeTraverseManager.instance.isblackBall && other.gameObject.name == "ball" && MazeTraverseManager.instance.mazeLevelIndex == 2)
                {
                    if (greatJobPopup != null)
                    {
                        greatJobPopup.SetActive(true);
                    }
                    MazeTraverseManager.instance.isblackBall = true;
                    other.transform.GetChild(0).gameObject.SetActive(true);
                    other.transform.GetComponent<BoxCollider2D>().enabled = false;
                    PlaySuccessAudio();
                    StartCoroutine(ReloadSceneAfterDelay());
                }
                else if (!MazeTraverseManager.instance.istravel && other.gameObject.name == "Summer" && MazeTraverseManager.instance.mazeLevelIndex == 3)
                {
                    if (greatJobPopup != null)
                    {
                        greatJobPopup.SetActive(true);
                    }
                    MazeTraverseManager.instance.istravel = true;
                    other.transform.GetChild(0).gameObject.SetActive(true);
                    other.transform.GetComponent<BoxCollider2D>().enabled = false;
                    PlaySuccessAudio();
                    StartCoroutine(ReloadSceneAfterDelay());
                }
                else
                {
                    if(tryJobPopup != null)
                    {
                        tryJobPopup.SetActive(true);
                    }
                    Debug.Log("No matching condition for the collided object.");
                    SimpleAudioPlayerTraversePath.instance.PlaySound2();
                }
            }

        }
    }

    public void PlaySuccessAudio()
    {
        if (MazeTraverseManager.instance.mazeLevelIndex <= 2)
        {
            audioSource.PlayOneShot(successSound);
        }

        if (SimpleAudioPlayerTraversePath.instance != null && MazeTraverseManager.instance.mazeLevelIndex == 3)
        {
            SimpleAudioPlayerTraversePath.instance.PlaySound1();
        }
    }



    private IEnumerator ReloadSceneAfterDelay()
    {
        yield return new WaitForSecondsRealtime(restartDelay);

        hasReachedTarget = false;
        isMovingToTarget = false;
        moveDirection = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        // Reset character color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        if (greatJobPopup != null)
        {
            greatJobPopup.SetActive(false);
        }

        if (MazeTraverseManager.instance != null)
        {
            MazeTraverseManager.instance.mazeLevelIndex++;
            int currentLevel = MazeTraverseManager.instance.mazeLevelIndex;

            DisableAnimator();

            // Enable next target animator if within array bounds
            if (targetObjectsArry != null && currentLevel >= 0 && currentLevel < targetObjectsArry.Length)
            {
                if (targetObjectsArry[currentLevel] != null && targetObjectsArry[currentLevel].transform.parent != null)
                {
                    Animator anim = targetObjectsArry[currentLevel].transform.parent.GetComponent<Animator>();
                    if (anim != null)
                    {
                        anim.enabled = true;
                    }
                }
            }
            else
            {
                Debug.Log("All maze targets completed or index out of bounds!");
            }

            UpdateSlider(currentLevel);
        }
        if(MazeTraverseManager.instance.mazeLevelIndex >= 4)
        {
            if (levelUpPopup != null)
            {
                levelUpPopup.SetActive(true);
            }
        }
    }

    private void UpdateSlider(int levelIndex)
    {
        if (slider != null)
        {
            slider.value = levelIndex;

            if (slider.fillRect != null)
            {
                Image fillImage = slider.fillRect.GetComponent<Image>();
                if (fillImage != null && colorsSlider != null && levelIndex >= 0 && levelIndex < colorsSlider.Length)
                {
                    fillImage.color = colorsSlider[levelIndex];
                }
            }
        }
    }

    public void DisableAnimator()
    {
        if (targetObjectsArry == null) return;

        for (int i = 0; i < targetObjectsArry.Length; i++)
        {
            if (targetObjectsArry[i] != null && targetObjectsArry[i].transform.parent != null)
            {
                Animator anim = targetObjectsArry[i].transform.parent.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.enabled = false;
                }
            }
        }
    }
}
