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

    public int TokenCost = 2;
    public float paddleSpeed = 2f;
    private Vector2 ballVelocity = new Vector2(300f, 300f); // Example speed

    public float activationRadius = 5f;
    private bool gameActive = false;
    private bool playerInRange = false;
    private GameObject scoreText;
    public int score = 0;

    // Start is called before the first frame update
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
            SpawnPingPong();

        }

        

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

        UpdateUI();


        CreatePlayer(); // This will log the initial position of player paddle
        CreateEnemy();
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

        GameObject playerObject = Instantiate(playerPaddlePrefab, minigameCanvas.transform);
        playerPaddle = playerObject.GetComponent<RectTransform>();
        playerPaddle.anchoredPosition = new Vector2(-minigameCanvas.GetComponent<RectTransform>().rect.width / 2 + playerPaddle.rect.width / 2, 0);

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

        GameObject enemyObject = Instantiate(EnemyPaddlePrefab, minigameCanvas.transform);
        enemyPaddle = enemyObject.GetComponent<RectTransform>();
        enemyPaddle.anchoredPosition = new Vector2(minigameCanvas.GetComponent<RectTransform>().rect.width / 2 - enemyPaddle.rect.width / 2, 0); // Enemy on right side
    }

    void HandleEnemyMovement()
    {
        if (enemyPaddle == null || !gameActive) return;

        GameObject ball = GameObject.FindWithTag("Ball");
        if (ball == null) return;

        RectTransform ballRect = ball.GetComponent<RectTransform>();
        float ballY = ballRect.anchoredPosition.y;

        float moveAmount = paddleSpeed * Time.deltaTime;

        // Move towards the ball's Y position
        if (enemyPaddle.anchoredPosition.y < ballY) // Move up
        {
            enemyPaddle.anchoredPosition += new Vector2(0, moveAmount);
        }
        else if (enemyPaddle.anchoredPosition.y > ballY) // Move down
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
        GameObject ball = Instantiate(pingPongPrefab, minigameCanvas.transform);
        RectTransform ballRect = ball.GetComponent<RectTransform>();
        ballRect.anchoredPosition = Vector2.zero; // Center the ball
    }

    void HandleBallMovement()
    {
        if (!gameActive) return;

        // Assuming ball has been spawned and referenced
        RectTransform ball = GameObject.FindWithTag("Ball").GetComponent<RectTransform>();

        // Move the ball
        ball.anchoredPosition += ballVelocity * Time.deltaTime;

        // Check for paddle collisions
        if (CheckCollision(ball, playerPaddle))
        {
            ballVelocity.x = Mathf.Abs(ballVelocity.x); // Bounce to the right
            PlayHitSound();
        }
        else if (CheckCollision(ball, enemyPaddle))
        {
            ballVelocity.x = -Mathf.Abs(ballVelocity.x); // Bounce to the left
            PlayHitSound();
        }

        // Check for top and bottom bounds collisions
        float canvasHeight = minigameCanvas.GetComponent<RectTransform>().rect.height;
        if (ball.anchoredPosition.y + ball.sizeDelta.y / 2 > canvasHeight / 2 || ball.anchoredPosition.y - ball.sizeDelta.y / 2 < -canvasHeight / 2)
        {
            ballVelocity.y = -ballVelocity.y; // Reverse Y direction (bounce off top or bottom)
            PlayHitSound();
        }

        // Check if the ball goes out of bounds (left or right side)
        float canvasWidth = minigameCanvas.GetComponent<RectTransform>().rect.width;
        if (ball.anchoredPosition.x + ball.sizeDelta.x / 2 > canvasWidth / 2 || ball.anchoredPosition.x - ball.sizeDelta.x / 2 < -canvasWidth / 2)
        {
            score += 10;
            UpdateUI();

            endGame(); // End the game when the ball goes out of bounds (can add scoring logic)
        }
    }

    bool CheckCollision(RectTransform ball, RectTransform paddle)
    {
        Vector2 ballPos = ball.anchoredPosition;
        Vector2 paddlePos = paddle.anchoredPosition;

        Vector2 ballSize = ball.sizeDelta;
        Vector2 paddleSize = paddle.sizeDelta;

        // Calculate the bounds of the ball and paddle
        float ballLeft = ballPos.x - ballSize.x / 2;
        float ballRight = ballPos.x + ballSize.x / 2;
        float ballTop = ballPos.y + ballSize.y / 2;
        float ballBottom = ballPos.y - ballSize.y / 2;

        float paddleLeft = paddlePos.x - paddleSize.x / 2;
        float paddleRight = paddlePos.x + paddleSize.x / 2;
        float paddleTop = paddlePos.y + paddleSize.y / 2;
        float paddleBottom = paddlePos.y - paddleSize.y / 2;

        // Check if the rectangles overlap (collision detection)
        return (ballRight > paddleLeft && ballLeft < paddleRight &&
                ballTop > paddleBottom && ballBottom < paddleTop);
    }


    void endGame()
    {
        gameActive = false;

        GameManager.Instance.AddTickets(score);
        Debug.Log("Game Over! Tickets Won: " + score);

        menuCanvas.gameObject.SetActive(true);
        minigameCanvas.gameObject.SetActive(false);
    }

    private void PlayHitSound()
    {
        if (_pongHitSound != null)
        {
            _pongHitSound.Play();
        }
    }

}
