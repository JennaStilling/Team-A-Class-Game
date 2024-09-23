using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceInvadersGame : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    public GameObject enemyPrefab;
    public Transform bulletSpawnPoint;
    //public Transform enemySpawnPoint;
    public RectTransform gamePanel;

    public int playerLives = 3;
    public int score = 0;
    public int enemiesPerRow = 2;
    public int rowsOfEnemies = 3;
    public float playerSpeed = 5f;
    public float bulletSpeed = 10f;
    public float enemySpeed = 1f;
    public float timeBetweenEnemyRows = 2f;

    private GameObject player;
    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> bullets = new List<GameObject>();
    private bool gameActive = false;
    private bool isGameActive = false;
    private bool playerInRange = false;
    private float enemyDirectionX = 1f;
    private float movementStep = 0.04f;

    private GameObject scoreText;
    private GameObject livesText;

    void Start()
    {
        isGameActive = false;
        SetupUI();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isGameActive)
        {
            StartGame();
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
        textObject = Instantiate(new GameObject(), gamePanel);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();

        textComponent.text = textContent;
        textComponent.color = color;
        textComponent.fontSize = 24;

        textRect.anchoredPosition = position;
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
        StartCoroutine(SpawnEnemiesOneByOne());
    }


    void UpdateUI()
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score;
        livesText.GetComponent<TextMeshProUGUI>().text = "Lives: " + playerLives;
    }

    void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player);
        }

        player = Instantiate(playerPrefab); // No need to parent to gamePanel
        player.transform.position = new Vector3(0, -4, 0); // Set world position
    }

    

    void HandlePlayerMovement()
    {
        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        float clampedX = Mathf.Clamp(player.transform.position.x, -screenHalfWidth + 1, screenHalfWidth - 1);
        player.transform.position = new Vector3(clampedX, player.transform.position.y, player.transform.position.z);

    }

    void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, player.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        bullets.Add(bullet);
    }

    void HandleBulletMovement()
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            GameObject bullet = bullets[i];
            bullet.transform.position += Vector3.up * bulletSpeed * Time.deltaTime;

            // Remove bullet if it goes off-screen
            if (bullet.transform.position.y > gamePanel.rect.height / 2)
            {
                Destroy(bullet);
                bullets.RemoveAt(i); // Remove from the list and destroy the bullet
                continue; // Continue to avoid further processing on the removed bullet
            }

            // Check collision with enemies
            foreach (GameObject enemy in enemies)
            {
                if (Vector3.Distance(bullet.transform.position, enemy.transform.position) < 20f)
                {
                    Destroy(bullet);  // Destroy bullet
                    Destroy(enemy);   // Destroy enemy
                    bullets.RemoveAt(i);  // Remove bullet from the list
                    enemies.Remove(enemy);  // Remove enemy from the list
                    score += 10;  // Update score
                    UpdateUI();  // Refresh UI
                    break;  // Break the loop to avoid further checks for this bullet
                }
            }

            void OnTriggerEnter2D(Collider2D other)
            {
                if (other.gameObject.CompareTag("Bullet"))
                {
                    Destroy(other.gameObject); // Destroy bullet
                    Destroy(gameObject); // Destroy enemy
                    score += 10;
                    UpdateUI();
                }
            }

        }
    }

    IEnumerator SpawnEnemiesOneByOne()
    {
        for (int col = 0; col < enemiesPerRow; col++)
        {
            GameObject enemy = Instantiate(enemyPrefab, gamePanel);

            // Set the x position based on the column and clamp it to within the bounds of your canvas
            float x = Mathf.Lerp(-0.4f, 0.4f, (float)col / (enemiesPerRow - 1));

            // Set the y to start at 0.3
            float y = 0.3f;

            // Adjust the enemy position relative to the canvas position
            enemy.transform.position = new Vector3(x, y, 0) + gamePanel.transform.position;

            enemies.Add(enemy);

            // Wait for some time before spawning the next enemy
            yield return new WaitForSeconds(1f);  // 1 second delay, adjust as necessary
        }
    }

    void HandleEnemyMovement()
    {
        if (!gameActive) return;  // Stop if the game is over

        foreach (GameObject enemy in enemies)
        {
            // Move enemy along the x-axis
            enemy.transform.position += new Vector3(enemySpeed * enemyDirectionX * Time.deltaTime, 0, 0);

            // Check if the enemy has reached the bounds (-0.4, 0.4)
            if (enemy.transform.position.x > 0.4f)
            {
                // Reverse direction
                enemyDirectionX = -1f;
                // Move enemy down by one row
                enemy.transform.position += new Vector3(0, -0.1f, 0);
            }
            else if (enemy.transform.position.x < -0.4f)
            {
                // Reverse direction
                enemyDirectionX = 1f;
                // Move enemy down by one row
                enemy.transform.position += new Vector3(0, -0.1f, 0);
            }

            // Check if the enemy has reached the y = -0.3 (Game Over condition)
            if (enemy.transform.position.y <= -0.3f)
            {
                EndGame();
                break;
            }
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(other.gameObject); // Destroy bullet
            Destroy(gameObject); // Destroy enemy
            score += 10;
            UpdateUI();
        }
    }

    void EndGame()
    {
        gameActive = false;
        Debug.Log("Game Over! Tickets Won: " + score);

        foreach (GameObject enemy in enemies)
        {
            // Optionally, you could deactivate their movement component or freeze their position
            enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;  // Assuming you're using Rigidbody2D
        }
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
        StartCoroutine(SpawnEnemiesOneByOne());
    }

    void ClearEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        enemies.Clear();
    }

    void ClearBullets()
    {
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
        bullets.Clear();
    }


}
