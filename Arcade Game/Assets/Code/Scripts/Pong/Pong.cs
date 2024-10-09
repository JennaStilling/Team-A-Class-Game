using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Pong : MonoBehaviour
{
    [SerializeField] private AudioSource _pongHitSound;
    public GameObject playerPaddlePrefab;
    public GameObject EnemyPaddlePrefab;
    public GameObject pingPongPrefab;

    private RectTransform playerPaddle;
    private RectTransform enemyPaddle;

    public Canvas menuCanvas; // Reference to the Menu Canvas
    public Canvas minigameCanvas; // Reference to the 2D minigame Canvas
    public RectTransform minigameZone;
    private GameObject currentBall;

    public int TokenCost = 2;
    public float paddleSpeed = 2f;
    public float enemyPaddleSpeed = 1f;
    private Vector2 ballVelocity = new Vector2(1f, 1f); // Example speed

    public float activationRadius = 5f;
    private bool gameActive = false;
    private bool playerInRange = false;
    private GameObject scoreText;
    public int score = 0;
    private int playerWins = 0;
    private int enemyWins = 0;
    private int roundsPlayed = 0;
    private const int totalRounds = 3;

    void Start()
    {
        gameActive = false;
        menuCanvas.gameObject.SetActive(true);
        minigameCanvas.gameObject.SetActive(false);
        _pongHitSound = GetComponent<AudioSource>();
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
            HandleEnemyMovement();
            HandleBallMovement();
        }
    }

    public void SetPlayerInRange(bool inRange)
    {
        playerInRange = inRange;
        Debug.Log(playerInRange ? "Player is near the machine. Press 'E' to start." : "Player left the machine area.");
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

        roundsPlayed = 0;
        score = 0;
        UpdateUI();
        CreatePlayer();
        CreateEnemy();
        SpawnPingPong();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score;
        }
    }

    void CreatePlayer()
    {
        float canvasHeight = minigameCanvas.GetComponent<RectTransform>().rect.height;
        float inset = 0.15f;

        GameObject playerObject = Instantiate(playerPaddlePrefab, minigameCanvas.transform);
        playerPaddle = playerObject.GetComponent<RectTransform>();
        playerPaddle.anchoredPosition = new Vector2(
            (minigameCanvas.GetComponent<RectTransform>().rect.width / 2 - playerPaddle.rect.width / 2) - inset,
            0);
    }

    void HandlePlayerMovement()
    {
        float canvasHeight = minigameCanvas.GetComponent<RectTransform>().rect.height;

        if (playerPaddle == null) return;

        float moveAmount = paddleSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.I)) // Move up
        {
            playerPaddle.anchoredPosition += new Vector2(0, moveAmount);
        }
        if (Input.GetKey(KeyCode.K)) // Move down
        {
            playerPaddle.anchoredPosition -= new Vector2(0, moveAmount);
        }

        // Clamp the player paddle position within screen bounds
        playerPaddle.anchoredPosition = new Vector2(playerPaddle.anchoredPosition.x, Mathf.Clamp(playerPaddle.anchoredPosition.y, -canvasHeight / 2, canvasHeight / 2));
    }

    void CreateEnemy()
    {
        float canvasHeight = minigameCanvas.GetComponent<RectTransform>().rect.height;
        float inset = 0.15f;

        GameObject enemyObject = Instantiate(EnemyPaddlePrefab, minigameCanvas.transform);
        enemyPaddle = enemyObject.GetComponent<RectTransform>();
        enemyPaddle.anchoredPosition = new Vector2(
            (-minigameCanvas.GetComponent<RectTransform>().rect.width / 2 + enemyPaddle.rect.width / 2) + inset,
            0);
    }

    void HandleEnemyMovement()
    {
        if (enemyPaddle == null || !gameActive) return;
        if (currentBall == null) return; // Ensure the ball exists

        RectTransform ballRect = currentBall.GetComponent<RectTransform>();
        float ballY = ballRect.anchoredPosition.y;

        // Delay the enemy movement
        StartCoroutine(MoveEnemyTowardsBall(ballY));
    }

    IEnumerator MoveEnemyTowardsBall(float targetY)
    {
        yield return new WaitForSeconds(0.5f); // Delay

        float moveAmount = enemyPaddleSpeed * Time.deltaTime;

        // Move towards the target Y position
        if (enemyPaddle.anchoredPosition.y < targetY) // Move up
        {
            enemyPaddle.anchoredPosition += new Vector2(0, moveAmount);
        }
        else if (enemyPaddle.anchoredPosition.y > targetY) // Move down
        {
            enemyPaddle.anchoredPosition -= new Vector2(0, moveAmount);
        }

        // Clamp enemy paddle to stay within the screen
        float canvasHeight = minigameCanvas.GetComponent<RectTransform>().rect.height;
        enemyPaddle.anchoredPosition = new Vector2(
            enemyPaddle.anchoredPosition.x,
            Mathf.Clamp(enemyPaddle.anchoredPosition.y, -canvasHeight / 2, canvasHeight / 2)
        );
    }

    void SpawnPingPong()
    {
        if (currentBall == null) // Check if the ball already exists
        {
            currentBall = Instantiate(pingPongPrefab, minigameCanvas.transform);
            RectTransform ballRect = currentBall.GetComponent<RectTransform>();
            ballRect.anchoredPosition = Vector2.zero; // Center the ball

            // Set a random direction for the ball
            ballVelocity = GetRandomBallVelocity();
        }
    }

    private Vector2 GetRandomBallVelocity()
    {
        // Randomize the angle between -45 and 45 degrees for a more diagonal movement
        float angle = Random.Range(-45f, 45f);

        // Calculate the velocity components based on the angle
        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);

        // Normalize the vector and scale it by desired speed
        return new Vector2(x, y).normalized * 0.8f; // Adjust the speed (1f) as needed
    }


    void HandleBallMovement()
    {
        if (!gameActive || currentBall == null) return;

        RectTransform ball = currentBall.GetComponent<RectTransform>();

        // Move the ball
        ball.anchoredPosition += ballVelocity * Time.deltaTime;

        // Check for paddle collisions
        if (CheckPaddleCollision(ball, playerPaddle) || CheckPaddleCollision(ball, enemyPaddle))
        {
            ballVelocity.x = -ballVelocity.x; // Reverse X direction on paddle hit
            PlayHitSound();
        }

        // Check for top and bottom bounds collisions
        float canvasHeight = minigameCanvas.GetComponent<RectTransform>().rect.height;
        if (ball.anchoredPosition.y + ball.sizeDelta.y / 2 > canvasHeight / 2 || ball.anchoredPosition.y - ball.sizeDelta.y / 2 < -canvasHeight / 2)
        {
            ballVelocity.y = -ballVelocity.y; // Reverse Y direction (bounce off top or bottom)
            PlayHitSound();
        }

        // Check for scoring
        float canvasWidth = minigameCanvas.GetComponent<RectTransform>().rect.width;

        // Right side scoring (enemy scores)
        if (ball.anchoredPosition.x + ball.sizeDelta.x / 2 > canvasWidth / 2 + enemyPaddle.rect.width / 2)
        {
            enemyWins++;
            Debug.Log("Enemy scored!");
            ResetBall();
            CheckRoundOutcome();
        }
        // Left side scoring (player scores)
        else if (ball.anchoredPosition.x - ball.sizeDelta.x / 2 < -canvasWidth / 2 - playerPaddle.rect.width / 2)
        {
            playerWins++;
            score += 10;
            Debug.Log("Player scored!");
            ResetBall();
            CheckRoundOutcome();
        }
    }

    bool CheckPaddleCollision(RectTransform ball, RectTransform paddle)
    {
        // Calculate the bounds for the ball
        Vector2 ballPos = ball.anchoredPosition;
        Vector2 ballHalfSize = ball.sizeDelta / 2;

        // Calculate the bounds for the paddle
        Vector2 paddlePos = paddle.anchoredPosition;
        Vector2 paddleHalfSize = paddle.sizeDelta / 2;

        // Check for overlap
        return (ballPos.x + ballHalfSize.x > paddlePos.x - paddleHalfSize.x &&
                ballPos.x - ballHalfSize.x < paddlePos.x + paddleHalfSize.x &&
                ballPos.y + ballHalfSize.y > paddlePos.y - paddleHalfSize.y &&
                ballPos.y - ballHalfSize.y < paddlePos.y + paddleHalfSize.y);
    }

    void ResetBall()
    {
        if (currentBall != null)
        {
            Destroy(currentBall); // Destroy the ball
            currentBall = null; // Reset current ball reference
        }
        SpawnPingPong(); // Spawn a new ball
    }

    void PlayHitSound()
    {
        if (_pongHitSound != null)
        {
            _pongHitSound.Play();
        }
    }

    void CheckRoundOutcome()
    {
        roundsPlayed++;
        if (roundsPlayed >= totalRounds)
        {
            // End the game and determine winner
            EndGame();
        }
        else
        {
            // Prepare for the next round
            ResetBall();
        }
    }

    void EndGame()
    {
        gameActive = false;

        StopAllCoroutines();

        GameManager.Instance.AddTickets(score);
        Debug.Log("Game Over! Tickets Won: " + score);
        
        playerWins = 0;
        enemyWins = 0;
        ClearBall();
        ClearPaddles();

        menuCanvas.gameObject.SetActive(true);
        minigameCanvas.gameObject.SetActive(false);
    }

    void ClearBall()
    {
        if (currentBall != null)
        {
            Destroy(currentBall); // Destroy the ball
            currentBall = null; // Clear the reference
        }
    }

    void ClearPaddles()
    {
        if (playerPaddle != null)
        {
            Destroy(playerPaddle.gameObject); // Destroy player paddle
            playerPaddle = null; // Clear the reference
        }

        if (enemyPaddle != null)
        {
            Destroy(enemyPaddle.gameObject); // Destroy enemy paddle
            enemyPaddle = null; // Clear the reference
        }
    }


}
