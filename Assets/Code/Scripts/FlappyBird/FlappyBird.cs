using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlappyBird : MonoBehaviour
{
    [SerializeField] private AudioSource _flapSound;
    public GameObject playerBirdPrefab;
    public GameObject pipePrefab;
    public GameObject backgroundPrefab;
    private GameObject background;
    public RectTransform bkg; //Background

    private RectTransform playerBird;

    public Canvas menuCanvas; // Reference to the Menu Canvas
    public Canvas minigameCanvas; // Reference to the 2D minigame Canvas
    public RectTransform minigameZone;
    

    public int TokenCost = 2;
    public float activationRadius = 5f;
    private bool gameActive = false;
    private bool playerInRange = false;
    private GameObject scoreText;
    public int score = 0;
    private float birdVelocityY = 0f; // Bird's vertical velocity
    private float gravity = -0.5f; // Gravity force
    private float flapStrength = 0.4f; // Strength of each flap
    
    public float pipeSpawnInterval = 10f; // Time between pipe spawns
    public float pipeSpeed = 1f; // Speed of pipe movement
    private List<RectTransform> pipes = new List<RectTransform>();

    // Start is called before the first frame update
    void Start()
    {
        gameActive = false;
        menuCanvas.gameObject.SetActive(true);
        minigameCanvas.gameObject.SetActive(false);
        _flapSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
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
            BirdMovement();
            UpdatePipes();
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

        bkg.localPosition = minigameCanvas.transform.localPosition;
        SetBackground();

        score = 0;
        UpdateUI();
        SpawnBird();
        SpawnPipes();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score;
        }
    }

    void SpawnBird()
    {
        // Instantiate the bird at a starting position
        playerBird = Instantiate(playerBirdPrefab, minigameCanvas.transform).GetComponent<RectTransform>();
        playerBird.anchoredPosition = new Vector2(0, 0); // Start in the middle of the screen
        birdVelocityY = 0f;
    }

    void BirdMovement()
    {
        if (Input.GetKeyDown(KeyCode.K)) // Bird flaps when K is pressed
        {
            birdVelocityY = flapStrength;
            _flapSound.Play(); // Play flap sound
        }

        // Apply gravity to the bird's velocity
        birdVelocityY += gravity * Time.deltaTime;

        // Clamp bird's velocity to prevent too fast falling or rising
        birdVelocityY = Mathf.Clamp(birdVelocityY, -20f, flapStrength);

        // Update bird's position based on its velocity
        playerBird.anchoredPosition += new Vector2(0, birdVelocityY * Time.deltaTime);

        // Check if the bird falls out of bounds
        if (playerBird.anchoredPosition.y <= -minigameCanvas.GetComponent<RectTransform>().rect.height / 2 ||
            playerBird.anchoredPosition.y >= minigameCanvas.GetComponent<RectTransform>().rect.height / 2)
        {
            EndGame(); // End the game if the bird goes off-screen
        }
    }

    void SpawnPipes()
    {
        StartCoroutine(SpawnPipesCoroutine());
    }

    IEnumerator SpawnPipesCoroutine()
    {
        RectTransform canvasRectTransform = minigameCanvas.GetComponent<RectTransform>();

        while (gameActive)
        {
            // Determine if we are spawning a top or bottom pipe
            bool spawnTopPipe = Random.value > 0.5f; // 50% chance to spawn top pipe

            RectTransform newPipe = Instantiate(pipePrefab, minigameCanvas.transform).GetComponent<RectTransform>();

            // Set pipe's starting position with the appropriate Y value
            if (spawnTopPipe)
            {
                // Spawn at the top with some padding
                newPipe.anchoredPosition = new Vector2(canvasRectTransform.rect.width / 2, 0.3f); // Set Y to 0.3
                newPipe.localEulerAngles = new Vector3(0, 0, 180); // Rotate for top pipe
            }
            else
            {
                // Spawn at the bottom with some padding
                newPipe.anchoredPosition = new Vector2(canvasRectTransform.rect.width / 2, -0.3f); // Set Y to -0.3
                newPipe.localEulerAngles = Vector3.zero; // No rotation for bottom pipe
            }

            pipes.Add(newPipe);

            yield return new WaitForSeconds(pipeSpawnInterval);
        }
    }

    void UpdatePipes()
    {
        RectTransform canvasRectTransform = minigameCanvas.GetComponent<RectTransform>();

        for (int i = pipes.Count - 1; i >= 0; i--)
        {
            // Move each pipe to the left by a certain speed
            RectTransform pipe = pipes[i];
            pipe.anchoredPosition -= new Vector2(pipeSpeed * Time.deltaTime, 0);

            // Check for collision with the bird
            if (CheckBirdCollision(playerBird, pipe))
            {
                EndGame(); // End the game if there is a collision with a pipe
                return; // Exit the loop to avoid multiple calls to EndGame
            }

            // Check if the bird has passed the pipe
            if (pipe.anchoredPosition.x < playerBird.anchoredPosition.x && !pipe.CompareTag("Passed"))
            {
                pipe.tag = "Passed"; // Mark pipe as passed
                score++; // Increment score for successful pipe dodge
                UpdateUI();
                Debug.Log("Score incremented! Current score: " + score);
            }

            // If the pipe moves off-screen (left side), destroy it
            if (pipe.anchoredPosition.x + pipe.rect.width / 2 < -canvasRectTransform.rect.width / 2)
            {
                Destroy(pipe.gameObject);
                pipes.RemoveAt(i);
            }
        }
    }

    bool CheckBirdCollision(RectTransform bird, RectTransform pipe)
    {
        // Calculate the bounds for the bird
        Vector2 birdPos = bird.anchoredPosition;
        Vector2 birdHalfSize = bird.sizeDelta / 2;

        // Calculate the bounds for the pipe
        Vector2 pipePos = pipe.anchoredPosition;
        Vector2 pipeHalfSize = pipe.sizeDelta / 2;

        // Check for overlap
        return (birdPos.x + birdHalfSize.x > pipePos.x - pipeHalfSize.x &&
                birdPos.x - birdHalfSize.x < pipePos.x + pipeHalfSize.x &&
                birdPos.y + birdHalfSize.y > pipePos.y - pipeHalfSize.y &&
                birdPos.y - birdHalfSize.y < pipePos.y + pipeHalfSize.y);
    }


    void SetBackground()
    {
        // Instantiate the background at the correct position
        background = Instantiate(backgroundPrefab, minigameCanvas.transform); // Store reference
        RectTransform backgroundRectTransform = background.GetComponent<RectTransform>();

        // Set the size of the background to match the panel's size
        RectTransform gameScreenRectTransform = minigameCanvas.GetComponentInChildren<RectTransform>();
        backgroundRectTransform.sizeDelta = gameScreenRectTransform.sizeDelta;

        // Set position to zero for X and Y, and -0.002 for Z
        backgroundRectTransform.anchoredPosition = Vector2.zero;
        backgroundRectTransform.localPosition = new Vector3(0, 0, -0.002f);

        // Set the background to be behind other UI elements
        backgroundRectTransform.SetAsFirstSibling();
    }

    

    void EndGame()
    {
        gameActive = false;

        StopAllCoroutines();
        foreach (var pipe in pipes)
        {
            Destroy(pipe.gameObject); // Destroy remaining pipes
        }
        
        ResetBird();
        ResetPipes();

        if (background != null) 
        {
            Destroy(background); 
            background = null; 
        }

        birdVelocityY = 0f;
        GameManager.Instance.AddTickets(score);
        Debug.Log("Game Over! Tickets Won: " + score);
        ResetScore();

        menuCanvas.gameObject.SetActive(true);
        minigameCanvas.gameObject.SetActive(false);
    }

    void ResetBird()
    {
        if (playerBird != null)
        {
            Destroy(playerBird.gameObject);
            playerBird = null; 
        }
    }

    void ResetPipes()
    {
        foreach (var pipe in pipes)
        {
            Destroy(pipe.gameObject);
        }
        pipes.Clear(); 
    }

    void ResetScore()
    {
        score = 0;
        //UpdateUI(); 
    }


}
