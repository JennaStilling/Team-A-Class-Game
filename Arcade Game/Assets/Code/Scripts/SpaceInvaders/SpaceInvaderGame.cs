using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceInvadersGame : MonoBehaviour
{
    [SerializeField] private AudioSource _fireSound;

    public int TokenCost = 2;
    public GameObject shipPrefab; // Changed back to GameObject
    public GameObject bulletPrefab; // Changed back to GameObject
    public GameObject enemyPrefab;  // Changed back to GameObject
    public RectTransform bulletSpawnPoint; // Kept as RectTransform
    public Canvas menuCanvas; // Reference to the Menu Canvas
    public Canvas minigameCanvas; // Reference to the 2D minigame Canvas
    public RectTransform minigameZone; // Kept as RectTransform

    
    public int score = 0;
    public int maxEnemiesOnScreen = 5; // Max enemies on screen at a time
    public float playerSpeed = 2f;
    public float bulletSpeed = 10f;
    public float enemySpeed = 0.1f;
    public float spawnDelay = 1f;
    public float activationRadius = 5f;

    private RectTransform ship;
    private List<RectTransform> enemies = new List<RectTransform>();
    private List<RectTransform> bullets = new List<RectTransform>();
    private bool gameActive = false;
    private bool playerInRange = false;
    private bool movingRight = true; // For enemy movement

    private GameObject scoreText;
    private GameObject livesText;
    private GameObject timerText;
    public float gameTime = 10f; // 10 seconds game timer
    private bool timerActive = false;

    void Start()
    {
        gameActive = false;
        //SetupUI();

        menuCanvas.gameObject.SetActive(true);
        minigameCanvas.gameObject.SetActive(false); // Initially hide the Canvas

        _fireSound = GetComponent<AudioSource>();

    }

    void Update()
    {
        // Handle minigame Canvas visibility
        if (Vector3.Distance(Camera.main.transform.position, minigameZone.position) < activationRadius)
        {
            minigameCanvas.gameObject.SetActive(true); // Show the Canvas
        }
        else
        {
            minigameCanvas.gameObject.SetActive(false); // Hide the Canvas
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !gameActive)
        {
            StartGame();
        }

        if (gameActive)
        {
            HandlePlayerMovement();
            HandleBulletMovement();
            HandleEnemyMovement();

            if (Input.GetKeyDown(KeyCode.J))
            {
                ShootBullet();
            }
        }

        // Handle the game timer
        if (timerActive)
        {
            gameTime -= Time.deltaTime;
            if (gameTime <= 0)
            {
                timerActive = false;
                EndGame(); // End the game when the timer hits 0
            }
        }

    }

    //void SetupUI()
    //{
    //    CreateUIText(ref scoreText, "Score: " + score, new Vector2(0, 0), Color.white);
    //    CreateUIText(ref timerText, "Time: 10s", new Vector2(0, 3), Color.red);

    //}

    void CreateUIText(ref GameObject textObject, string textContent, Vector2 position, Color color)
    {
        textObject = new GameObject("UIText");
        textObject.transform.SetParent(minigameCanvas.transform);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();

        textComponent.text = textContent;
        textComponent.color = color;
        textComponent.fontSize = 0.5f;

        textRect.anchoredPosition = position;
        textRect.sizeDelta = new Vector2(1, 1); // Set a size that fits the text
    }

    public void SetPlayerInRange(bool inRange)
    {
        playerInRange = inRange;

        if (playerInRange)
        {
            Debug.Log("Player is near the machine. Press 'E' to start.");
        }
        else
        {
            Debug.Log("Player left the machine area.");
        }
    }

    void StartGame()
    {
        if (!GameManager.Instance.SpendToken(TokenCost))
        {
            Debug.Log("Not enough tokens");
            return;
        }

        Debug.Log("Game Started! Tokens Spent: " + TokenCost);

        menuCanvas.gameObject.SetActive(false);
        minigameCanvas.gameObject.SetActive(true);

        gameActive = true;
        score = 0;

        UpdateUI();

        gameTime = 10f; // Reset the game timer
        timerActive = true; // Activate the timer
        
        CreatePlayer(); // This will log the initial position of the ship
        SpawnEnemy();
    }

    void UpdateUI()
    {
        if (scoreText != null && livesText != null)
        {
            scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score;  
        }

        if (timerText != null)
        {
            timerText.GetComponent<TextMeshProUGUI>().text = "Time: " + Mathf.CeilToInt(gameTime) + "s";
        }
    }

    void CreatePlayer()
    {
        if (ship != null)
        {
            Destroy(ship.gameObject);
        }

        GameObject playerObject = Instantiate(shipPrefab, minigameCanvas.transform); // Parent to the Canvas
        ship = playerObject.GetComponent<RectTransform>();

        // Get the screen width and height of the canvas
        float screenWidth = minigameCanvas.GetComponent<RectTransform>().rect.width;
        float screenHeight = minigameCanvas.GetComponent<RectTransform>().rect.height;

        // Place the ship in the center horizontally and slightly above the bottom vertically
        ship.anchoredPosition = new Vector2(0, -0.45f * screenHeight);
        Debug.Log("Ship Spawned at: " + ship.anchoredPosition);
    }

    void HandlePlayerMovement()
    {
        // Check for player input (K for left, L for right)
        float movementX = 0;
        if (Input.GetKey(KeyCode.K)) movementX = -playerSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.L)) movementX = playerSpeed * Time.deltaTime;

        // Move player horizontally, scaled to canvas width
        RectTransform canvasRect = minigameCanvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;

        // Scale the movement by canvas width (to account for smaller screen sizes)
        float scaledMovementX = movementX * canvasWidth;

        // Move the ship
        ship.anchoredPosition += new Vector2(scaledMovementX, 0);

        // Clamp the ship's x-position within the left and right bounds of the canvas
        float clampedX = Mathf.Clamp(ship.anchoredPosition.x, -canvasWidth / 2 + 0.05f, canvasWidth / 2 - 0.05f);

        // Apply the clamped position
        ship.anchoredPosition = new Vector2(clampedX, ship.anchoredPosition.y);

        // Log the ship's position for debugging
        Debug.Log("Ship Position After Movement: " + ship.anchoredPosition);
    }
    void ShootBullet()
    {
        GameObject bulletObject = Instantiate(bulletPrefab, minigameCanvas.transform); // Parent to the Canvas
        RectTransform bullet = bulletObject.GetComponent<RectTransform>();

        PlayFireSound();

        // Spawn the bullet from the current position of the ship
        bullet.anchoredPosition = new Vector2(ship.anchoredPosition.x, ship.anchoredPosition.y + 0.1f);

        bullets.Add(bullet);
    }

    void HandleBulletMovement()
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            RectTransform bullet = bullets[i];
            bullet.anchoredPosition += new Vector2(0, bulletSpeed * Time.deltaTime);

            // Check if bullet goes off-screen
            RectTransform canvasRect = minigameCanvas.GetComponent<RectTransform>();
            if (bullet.anchoredPosition.y > canvasRect.rect.height / 2)
            {
                Destroy(bullet.gameObject);
                bullets.RemoveAt(i);
                continue;
            }

            // Check collision with enemies using rect bounds
            foreach (RectTransform enemy in enemies)
            {
                if (IsRectOverlapping(bullet, enemy))
                {
                    Destroy(bullet.gameObject);  // Destroy bullet
                    Destroy(enemy.gameObject);   // Destroy enemy
                    bullets.RemoveAt(i);  // Remove bullet from the list
                    enemies.Remove(enemy);  // Remove enemy from the list

                    score += 1;  // Update score
                    UpdateUI();  // Refresh UI

                    // Respawn a new enemy after one is destroyed
                    SpawnEnemy();
                    break;
                }
            }
        }
    }
    bool IsRectOverlapping(RectTransform rect1, RectTransform rect2)
    {
        Vector3[] rect1Corners = new Vector3[4];
        Vector3[] rect2Corners = new Vector3[4];

        rect1.GetWorldCorners(rect1Corners);
        rect2.GetWorldCorners(rect2Corners);

        Rect rect1World = new Rect(rect1Corners[0].x, rect1Corners[0].y, rect1.rect.width, rect1.rect.height);
        Rect rect2World = new Rect(rect2Corners[0].x, rect2Corners[0].y, rect2.rect.width, rect2.rect.height);

        return rect1World.Overlaps(rect2World);
    }



    void SpawnEnemy()
    {
        int enemiesToSpawn = 2; // Number of enemies to spawn at once

        // Get the width and height of the canvas for spacing calculations
        float xSpacing = 0.1f * minigameCanvas.GetComponent<RectTransform>().rect.width;
        float ySpacing = 0.1f * minigameCanvas.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemyObject = Instantiate(enemyPrefab, minigameCanvas.transform); // Parent to the Canvas
            RectTransform enemy = enemyObject.GetComponent<RectTransform>();

            // Adjust the starting position for each enemy
            Vector2 startPos = new Vector2(
                -0.4f * minigameCanvas.GetComponent<RectTransform>().rect.width + (enemies.Count % 4 + i) * xSpacing,
                0.3f * minigameCanvas.GetComponent<RectTransform>().rect.height - (enemies.Count / 4) * ySpacing
            );

            enemy.anchoredPosition = startPos;
            enemies.Add(enemy);
        }
    }

    void HandleEnemyMovement()
    {
        float screenWidth = minigameCanvas.GetComponent<RectTransform>().rect.width;
        float screenHeight = minigameCanvas.GetComponent<RectTransform>().rect.height;

        foreach (RectTransform enemy in enemies)
        {
            if (enemy == null) continue;

            // Move enemy horizontally
            enemy.anchoredPosition += new Vector2(enemySpeed * (movingRight ? 1 : -1) * Time.deltaTime, 0);

            // Check for boundary collisions
            if (enemy.anchoredPosition.x > screenWidth / 2 || enemy.anchoredPosition.x < -screenWidth / 2)
            {
                movingRight = !movingRight; // Reverse direction
                enemy.anchoredPosition += new Vector2(0, -0.1f * screenHeight); // Move down a row
            }

            // Check for game over condition
            if (enemy.anchoredPosition.y <= -0.3f * screenHeight)
            {
                EndGame();
                return;
            }
        }
    }

    void EndGame()
    {
        gameActive = false;

        GameManager.Instance.AddTickets(score);
        Debug.Log("Game Over! Tickets Won: " + score);

        // Clear bullets and enemies
        ClearBullets();
        ClearEnemies();
        ClearPlayer();
        menuCanvas.gameObject.SetActive(true);
    }


    void ClearEnemies()
    {
        foreach (RectTransform enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        enemies.Clear();
    }

    void ClearPlayer()
    {
        Destroy(ship.gameObject);
    }

    void ClearBullets()
    {
        foreach (RectTransform bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
        bullets.Clear();
    }

    private void PlayFireSound()
    {
        if (_fireSound != null)
        {
            _fireSound.Play(); // Play the assigned audio clip
        }
    }



}
