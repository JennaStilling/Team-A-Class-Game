using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceInvadersGame : MonoBehaviour
{
    public GameObject playerPrefab; // Changed back to GameObject
    public GameObject bulletPrefab; // Changed back to GameObject
    public GameObject enemyPrefab;  // Changed back to GameObject
    public RectTransform bulletSpawnPoint; // Kept as RectTransform
    public Canvas minigameCanvas; // Reference to the 2D minigame Canvas
    public RectTransform minigameZone; // Kept as RectTransform

    public int playerLives = 3;
    public int score = 0;
    public int maxEnemiesOnScreen = 5; // Max enemies on screen at a time
    public float playerSpeed = 5f;
    public float bulletSpeed = 10f;
    public float enemySpeed = 0.1f;
    public float spawnDelay = 1f;
    public float activationRadius = 5f;

    private RectTransform player;
    private List<RectTransform> enemies = new List<RectTransform>();
    private List<RectTransform> bullets = new List<RectTransform>();
    private bool gameActive = false;
    private bool playerInRange = false;
    private bool movingRight = true; // For enemy movement

    private GameObject scoreText;
    private GameObject livesText;

    void Start()
    {
        gameActive = false;
        SetupUI();
        minigameCanvas.gameObject.SetActive(false); // Initially hide the Canvas
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

        if (playerInRange && Input.GetKeyDown(KeyCode.R) && !gameActive)
        {
            RestartGame();
        }

        if (gameActive)
        {
            HandlePlayerMovement();
            HandleBulletMovement();
            HandleEnemyMovement();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShootBullet();
            }
        }
    }

    void SetupUI()
    {
        CreateUIText(ref scoreText, "Score: " + score, new Vector2(0, 50), Color.white);
        CreateUIText(ref livesText, "Lives: " + playerLives, new Vector2(0, -50), Color.white);
    }

    void CreateUIText(ref GameObject textObject, string textContent, Vector2 position, Color color)
    {
        textObject = new GameObject("UIText");
        textObject.transform.SetParent(minigameCanvas.transform);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();

        textComponent.text = textContent;
        textComponent.color = color;
        textComponent.fontSize = 24;

        textRect.anchoredPosition = position;
        textRect.sizeDelta = new Vector2(200, 50); // Set a size that fits the text
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
        gameActive = true;
        score = 0;
        playerLives = 3;

        UpdateUI();

        CreatePlayer();
        StartCoroutine(SpawnEnemies());
    }

    void UpdateUI()
    {
        if (scoreText != null && livesText != null)
        {
            scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score;
            livesText.GetComponent<TextMeshProUGUI>().text = "Lives: " + playerLives;
        }
    }

    void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player.gameObject);
        }

        GameObject playerObject = Instantiate(playerPrefab, minigameCanvas.transform); // Parent to the Canvas
        player = playerObject.GetComponent<RectTransform>();
        player.anchoredPosition = new Vector2(0, -4); // Adjust based on UI position
    }

    void HandlePlayerMovement()
    {
        float movementX = Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime;
        player.anchoredPosition += new Vector2(movementX, 0);

        // Adjust based on UI boundaries
        RectTransform canvasRect = minigameCanvas.GetComponent<RectTransform>();
        float clampedX = Mathf.Clamp(player.anchoredPosition.x, -canvasRect.rect.width / 2 + 50, canvasRect.rect.width / 2 - 50);
        player.anchoredPosition = new Vector2(clampedX, player.anchoredPosition.y);
    }

    void ShootBullet()
    {
        GameObject bulletObject = Instantiate(bulletPrefab, minigameCanvas.transform); // Parent to the Canvas
        RectTransform bullet = bulletObject.GetComponent<RectTransform>();
        bullet.anchoredPosition = bulletSpawnPoint.anchoredPosition;
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

            // Check collision with enemies
            foreach (RectTransform enemy in enemies)
            {
                if (Vector2.Distance(bullet.anchoredPosition, enemy.anchoredPosition) < 10f) // Adjust collision distance
                {
                    Destroy(bullet.gameObject);  // Destroy bullet
                    Destroy(enemy.gameObject);   // Destroy enemy
                    bullets.RemoveAt(i);  // Remove bullet from the list
                    enemies.Remove(enemy);  // Remove enemy from the list

                    score += 10;  // Update score
                    UpdateUI();  // Refresh UI

                    // Respawn a new enemy after one is destroyed
                    SpawnEnemy();
                    break;
                }
            }
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (gameActive)
        {
            // Check if there are fewer than maxEnemiesOnScreen enemies
            if (enemies.Count < maxEnemiesOnScreen)
            {
                StartCoroutine(SpawnEnemy());
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnEnemy()
    {
        GameObject enemyObject = Instantiate(enemyPrefab, minigameCanvas.transform); // Parent to the Canvas
        RectTransform enemy = enemyObject.GetComponent<RectTransform>();

        // Adjust spacing between enemies
        float xSpacing = 0.1f * minigameCanvas.GetComponent<RectTransform>().rect.width;
        float ySpacing = 0.1f * minigameCanvas.GetComponent<RectTransform>().rect.height;
        Vector2 startPos = new Vector2(-0.4f * minigameCanvas.GetComponent<RectTransform>().rect.width + (enemies.Count % 4) * xSpacing, 0.3f * minigameCanvas.GetComponent<RectTransform>().rect.height - (enemies.Count / 4) * ySpacing);

        enemy.anchoredPosition = startPos;
        Debug.Log("Enemy spawned at: " + startPos);
        enemies.Add(enemy);
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
        Debug.Log("Game Over! Tickets Won: " + score);

        // Clear bullets and enemies
        ClearBullets();
        ClearEnemies();
    }

    void RestartGame()
    {
        // Reset lives, score, and clear all game objects
        score = 0;
        playerLives = 3;
        UpdateUI();
        ClearEnemies();
        ClearBullets();
        CreatePlayer();
        StartCoroutine(SpawnEnemies());
    }

    void ClearEnemies()
    {
        foreach (RectTransform enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        enemies.Clear();
    }

    void ClearBullets()
    {
        foreach (RectTransform bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
        bullets.Clear();
    }
}
