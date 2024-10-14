using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDRGame : MonoBehaviour
{
    public GameObject blockUIPrefab; // Prefab for the block (Image or UI element)
    public RectTransform[] columns; // Assign the RectTransforms for Column1, Column2, etc.
    public RectTransform[] hitAreas; // Assign the RectTransforms for the 'Hit' areas in each column
    public float blockSpeed = 300f; // Adjust speed based on your canvas size and requirements
    public float spawnInterval = 1.5f; // Time between spawning blocks

    private bool gameStarted = false; // Tracks if the game has started
    private bool playerInRange = false; // Tracks if the player is near the game machine
    private float spawnTimer = 0f; // Timer for spawning blocks

    // Update is called once per frame
    void Update()
    {
        // Start the game when the player presses 'E' and is in range
        if (playerInRange && !gameStarted && Input.GetKeyDown(KeyCode.E))
        {
            StartGame();
        }

        // If the game has started, start spawning blocks
        if (gameStarted)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                SpawnBlock();
                spawnTimer = 0f;
            }
        }
    }

    // Function to set if the player is in range
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

    // Function to start the game
    void StartGame()
    {
        Debug.Log("Game started!");
        gameStarted = true;
    }

    // Function to spawn a block UI element
    void SpawnBlock()
    {
        // Choose a random column to spawn the block
        int randomColumn = Random.Range(0, columns.Length);

        // Instantiate the block as a child of the selected column
        GameObject newBlock = Instantiate(blockUIPrefab, columns[randomColumn]);

        // Set the position of the block to the top of the column
        RectTransform blockRect = newBlock.GetComponent<RectTransform>();
        blockRect.anchoredPosition = new Vector2(0, 500); // Adjust the starting Y position if needed

        // Initialize block movement and assign the hit area
        BlockMovementUI blockMovement = newBlock.GetComponent<BlockMovementUI>();
        blockMovement.Initialize(columns[randomColumn], hitAreas[randomColumn], blockSpeed, this); // Pass the hit area
    }

    // Called when a block is missed
    public void OnBlockMissed()
    {
        Debug.Log("Block missed, game over.");
        gameStarted = false;
    }
}
