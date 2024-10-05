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
    GameObject currentBall;

    public int TokenCost = 2;
    public float paddleSpeed = 2f;
    public float enemyPaddleSpeed = 0.5f;
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
            HandleBallMovement();

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
        float inset = 0.2f; // The distance you want to move the paddle towards the center

        GameObject playerObject = Instantiate(playerPaddlePrefab, minigameCanvas.transform);
        playerPaddle = playerObject.GetComponent<RectTransform>();
        playerPaddle.anchoredPosition = new Vector2(
            (minigameCanvas.GetComponent<RectTransform>().rect.width / 2 - playerPaddle.rect.width / 2) - inset, // Move inwards by inset
            0); // Centered vertically
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
        float inset = 0.2f; // The distance you want to move the paddle towards the center

        GameObject enemyObject = Instantiate(EnemyPaddlePrefab, minigameCanvas.transform);
        enemyPaddle = enemyObject.GetComponent<RectTransform>();
        enemyPaddle.anchoredPosition = new Vector2(
            (-minigameCanvas.GetComponent<RectTransform>().rect.width / 2 + enemyPaddle.rect.width / 2) + inset, // Move inwards by inset
            0); // Centered vertically
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
        yield return new WaitForSeconds(0.2f); // Introduce a 0.2 second delay (adjust as needed)

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
        }
    }

    void HandleBallMovement()
    {
        if (!gameActive) return;

        RectTransform ball = GameObject.FindWithTag("Ball").GetComponent<RectTransform>();

        // Move the ball
        ball.anchoredPosition += ballVelocity * Time.deltaTime;

        if (CheckCollision(ball, playerPaddle))
        {
            ballVelocity.x = Mathf.Abs(ballVelocity.x); // Bounce to the right
            ball.anchoredPosition = new Vector2(playerPaddle.anchoredPosition.x + playerPaddle.rect.width / 2 + ball.rect.width / 2, ball.anchoredPosition.y); // Move ball outside the paddle
            PlayHitSound();
        }
        else if (CheckCollision(ball, enemyPaddle))
        {
            ballVelocity.x = -Mathf.Abs(ballVelocity.x); // Bounce to the left
            ball.anchoredPosition = new Vector2(enemyPaddle.anchoredPosition.x - enemyPaddle.rect.width / 2 - ball.rect.width / 2, ball.anchoredPosition.y); // Move ball outside the paddle
            PlayHitSound();
        }

        // Check for top and bottom bounds collisions
        float canvasHeight = minigameCanvas.GetComponent<RectTransform>().rect.height;
        if (ball.anchoredPosition.y + ball.sizeDelta.y / 2 > canvasHeight / 2 || ball.anchoredPosition.y - ball.sizeDelta.y / 2 < -canvasHeight / 2)
        {
            ballVelocity.y = -ballVelocity.y; // Reverse Y direction (bounce off top or bottom)
            PlayHitSound();
        }

        // Check for scoring conditions
        CheckScoring(ball);
    }

    void CheckScoring(RectTransform ball)
    {
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
            Debug.Log("Player scored!");
            ResetBall();
            CheckRoundOutcome();
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

        bool isColliding = (ballRight > paddleLeft && ballLeft < paddleRight &&
                        ballTop > paddleBottom && ballBottom < paddleTop);

        if (isColliding)
        {
            Debug.Log("Collision detected between ball and paddle!");
        }

        return isColliding;
    }

    void ResetBall()
    {
        RectTransform ball = GameObject.FindWithTag("Ball").GetComponent<RectTransform>();
        ball.anchoredPosition = Vector2.zero; // Center the ball

        // Give the ball a random direction but normalize the velocity for consistent speed
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        ballVelocity = randomDirection * 1f; // Adjust the speed as necessary

    }

    void CheckRoundOutcome()
    {
        if (playerWins >= 2)
        {
            Debug.Log("Player wins the game!");
            score = 20;
            endGame();
        }
        else if (enemyWins >= 2)
        {
            Debug.Log("Enemy wins the game. Player loses.");
            score = 0; // Set player score to 0
            endGame();
        }
        else if (roundsPlayed >= totalRounds)
        {
            // Game ends after 3 rounds
            if (playerWins > enemyWins)
            {
                Debug.Log("Player wins the game!");
                score = 20;
            }
            else
            {
                Debug.Log("Enemy wins the game. Player loses.");
                score = 0;
            }
            endGame();
        }
    }


    void endGame()
    {
        gameActive = false;

        StopAllCoroutines();

        GameManager.Instance.AddTickets(score);
        Debug.Log("Game Over! Tickets Won: " + score);
        //ResetBall();
        ClearPaddles();
        ClearBall();
        playerWins = 0;
        enemyWins = 0;

        menuCanvas.gameObject.SetActive(true);
        minigameCanvas.gameObject.SetActive(false);
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

    void ClearBall()
    {
        if (currentBall != null)
        {
            Destroy(currentBall); // Destroy the ball
            currentBall = null; // Clear the reference
        }
    }

    private void PlayHitSound()
    {
        if (_pongHitSound != null)
        {
            _pongHitSound.Play();
        }
    }

}
