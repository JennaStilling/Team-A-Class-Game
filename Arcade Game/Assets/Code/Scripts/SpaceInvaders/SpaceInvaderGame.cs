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
    public Transform enemySpawnPoint;
    public RectTransform gamePanel;

    public int playerLives = 3;
    public int score = 0;
    public int enemiesPerRow = 5;
    public int rowsOfEnemies = 3;
    public float playerSpeed = 5f;
    public float bulletSpeed = 10f;
    public float enemySpeed = 2f;
    public float timeBetweenEnemyRows = 2f;

    private GameObject player;
    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> bullets = new List<GameObject>();
    private bool gameActive = false;
    private bool isGameActive = false;
    private bool playerInRange = false;

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
        SpawnEnemies();
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

        player = Instantiate(playerPrefab, gamePanel);
        player.transform.position = new Vector3(0, -gamePanel.rect.height / 2 + 100, 0);
    }

    void SpawnEnemies()
    {
        for (int row = 0; row < rowsOfEnemies; row++)
        {
            for (int col = 0; col < enemiesPerRow; col++)
            {
                GameObject enemy = Instantiate(enemyPrefab, gamePanel);
                float x = col * 60 - (enemiesPerRow * 30);
                float y = row * 60 + gamePanel.rect.height / 2 - 100;

                enemy.transform.position = new Vector3(x, y, 0);
                enemies.Add(enemy);
            }
        }
    }

    void HandlePlayerMovement()
    {
        float move = Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime;
        player.transform.position += new Vector3(move, 0, 0);

        float clampedX = Mathf.Clamp(player.transform.position.x, -gamePanel.rect.width / 2 + 50, gamePanel.rect.width / 2 - 50);
        player.transform.position = new Vector3(clampedX, player.transform.position.y, player.transform.position.z);
    }

    void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity, gamePanel);
        bullets.Add(bullet);
    }

    void HandleBulletMovement()
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            GameObject bullet = bullets[i];
            bullet.transform.position += Vector3.up * bulletSpeed * Time.deltaTime;

            if (bullet.transform.position.y > gamePanel.rect.height / 2)
            {
                Destroy(bullet);
                bullets.RemoveAt(i);
            }

            foreach (GameObject enemy in enemies)
            {
                if (Vector3.Distance(bullet.transform.position, enemy.transform.position) < 20f)
                {
                    Destroy(bullet);
                    Destroy(enemy);
                    bullets.RemoveAt(i);
                    enemies.Remove(enemy);
                    score += 10;
                    UpdateUI();
                    break;
                }
            }
        }
    }

    void HandleEnemyMovement()
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.transform.position += Vector3.down * enemySpeed * Time.deltaTime;

            if (enemy.transform.position.y < -gamePanel.rect.height / 2)
            {
                Destroy(enemy);
                enemies.Remove(enemy);
                playerLives--;
                UpdateUI();
                if (playerLives <= 0)
                {
                    EndGame();
                }
                break;
            }
        }

        if (enemies.Count == 0)
        {
            Debug.Log("You Win! All enemies defeated.");
            EndGame();
        }
    }

    void EndGame()
    {
        gameActive = false;
        Debug.Log("Game Over! Final Score: " + score);
    }
}
