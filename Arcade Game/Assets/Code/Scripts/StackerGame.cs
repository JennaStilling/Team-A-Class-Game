using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StackerGame : MonoBehaviour
{
    public int TokenCost = 2;
    public int MinorPrize = 100;
    public int Jackpot = 500;

    public RectTransform blockPrefab;    
    public RectTransform gamePanel;
    public GameObject textPrefab;
    public GameObject imagePrefab;
    public int rows = 12;                 
    public int columns = 3;             
    public float stepSize = 100f;        
    public float padding = 5f;           
    public float rowHeight = 55f;       
    public float timeBetweenSteps = 0.3f;
    private float stepTimer = 0f;       
    private bool isStopped = true;       
    private bool isGameActive = false;   
    private bool movingRight = true;     
    private int currentRow = 0;         
    public float speedIncreaseFactor = 0.02f;  
    public float minimumStepTime = 0.05f; 
    public float initialStepTime = 0.3f;  
    private bool continuedAfterMinorPrize = false;
    private bool minorPrizeReached = false;
    private bool awaitingPlayerChoice = false;
    public TextMeshProUGUI ContinueOrCollect;

    private RectTransform[] currentBlocks;
    private List<RectTransform> previousBlocks = new List<RectTransform>(); 

    public float gridLeftBoundary = -500f;
    public float gridRightBoundary = 500f;

    private bool playerInRange = false;

    private GameObject minorPrizeText;
    private GameObject jackpotText;
    private GameObject minorPrizeBackground; 
    private GameObject jackpotBackground;

    void Start()
    {
        isGameActive = false;
        isStopped = true;
        ContinueOrCollect.enabled = false;
        CreatePrizeLabelWithBackground(ref minorPrizeBackground, ref minorPrizeText, "MINOR: " + MinorPrize + " Tickets", 9, Color.white);

        CreatePrizeLabelWithBackground(ref jackpotBackground, ref jackpotText, "MAJOR: " + Jackpot + " Tickets", 14, Color.yellow);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isGameActive)
        {
            StartGame();
        }

        if (isGameActive && !isStopped && !awaitingPlayerChoice)
        {
            MoveBlocks();
        }

        if (playerInRange && Input.GetMouseButtonDown(0) && !isStopped && isGameActive && !awaitingPlayerChoice)
        {
            StopBlocks();
        }

        if (awaitingPlayerChoice)
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.A))
            {
                ContinueOrCollect.enabled = false;
                Debug.Log("You collected the MINOR PRIZE!");
                GameManager.Instance.AddTickets(MinorPrize);
                ResetGame(); 
            }
            else if (playerInRange && Input.GetKeyDown(KeyCode.D))
            {
                ContinueOrCollect.enabled = false;
                Debug.Log("You're going for the JACKPOT!");
                ContinueFromMinorPrize(); 
            }
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

        currentRow = 0;
        columns = 3;  
        isGameActive = true;
        isStopped = false;
        minorPrizeReached = false; 
        awaitingPlayerChoice = false; 
        previousBlocks.Clear(); 
        timeBetweenSteps = initialStepTime;

        SpawnBlocks(columns);
    }

    void CreatePrizeLabelWithBackground(ref GameObject backgroundObject, ref GameObject textObject, string textContent, int rowNumber, Color backgroundColor)
    {
        backgroundObject = Instantiate(imagePrefab, gamePanel);

        RectTransform bgRect = backgroundObject.GetComponent<RectTransform>();

        float panelHeight = gamePanel.rect.height;
        float rowYPosition = (-panelHeight / 2) + (rowNumber * rowHeight);
        bgRect.anchoredPosition = new Vector2(0, rowYPosition);
        bgRect.sizeDelta = new Vector2(gamePanel.rect.width, rowHeight);  

        textObject = Instantiate(textPrefab, backgroundObject.transform);

        Image backgroundImage = backgroundObject.GetComponent<Image>();
        backgroundImage.color = backgroundColor;

        RectTransform textRect = textObject.GetComponent<RectTransform>();

        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = bgRect.sizeDelta; 

        TextMeshProUGUI textComponent = textObject.GetComponent<TextMeshProUGUI>();
        textComponent.text = textContent;
    }

    void MoveBlocks()
    {
        stepTimer += Time.deltaTime;

        if (stepTimer >= timeBetweenSteps)
        {
            stepTimer = 0f;

            float firstBlockXPosition = currentBlocks[0].anchoredPosition.x; 

            if (movingRight)
            {
                firstBlockXPosition += stepSize;
            }
            else
            {
                firstBlockXPosition -= stepSize; 
            }

            float leftmostPosition = firstBlockXPosition;  
            float rightmostPosition = firstBlockXPosition + (stepSize * (currentBlocks.Length - 1)) + (padding * (currentBlocks.Length - 1));

            if (rightmostPosition > gridRightBoundary)
            {
                movingRight = false;  
                return;
            }
            else if (leftmostPosition < gridLeftBoundary)
            {
                movingRight = true; 
                return;
            }

            for (int i = 0; i < currentBlocks.Length; i++)
            {
                currentBlocks[i].anchoredPosition = new Vector2(firstBlockXPosition + (i * (stepSize + padding)), currentBlocks[i].anchoredPosition.y);
            }
        }
    }


    void StopBlocks()
    {
        isStopped = true;

        if (currentRow > 0)
        {
            StartCoroutine(CheckAndHandleUnalignedBlocks());
        }
        else
        {
            MoveToNextRow();
        }
    }

    IEnumerator CheckAndHandleUnalignedBlocks()
    {
        List<RectTransform> alignedBlocks = new List<RectTransform>();
        float alignmentTolerance = stepSize / 2;

        for (int i = 0; i < currentBlocks.Length; i++)
        {
            bool isAligned = false;

            for (int j = 0; j < previousBlocks.Count; j++)
            {
                if (Mathf.Abs(currentBlocks[i].anchoredPosition.x - previousBlocks[j].anchoredPosition.x) <= alignmentTolerance)
                {
                    alignedBlocks.Add(currentBlocks[i]);
                    isAligned = true;
                    Debug.Log("Block " + i + " aligned with previous block " + j);
                    break;
                }
            }

            if (!isAligned)
            {
                Debug.Log("Block " + i + " misaligned and will drop.");
                StartCoroutine(AnimateMisalignedBlock(currentBlocks[i]));
            }
        }

        yield return new WaitForSeconds(1f);

        columns = alignedBlocks.Count;

        if (columns == 0)
        {
            Debug.Log("Game Over! No more aligned blocks.");
            ResetGame();
        }
        else
        {
            currentBlocks = alignedBlocks.ToArray();

            MoveToNextRow();
        }
    }



    IEnumerator AnimateMisalignedBlock(RectTransform block)
    {
        float dropSpeed = 200f;
        while (block.anchoredPosition.y > -gamePanel.rect.height / 2 - 50)
        {
            block.anchoredPosition += new Vector2(0, -dropSpeed * Time.deltaTime);
            yield return null;
        }

        Image blockImage = block.GetComponent<Image>();
        Color originalColor = blockImage.color;
        for (int i = 0; i < 3; i++)
        {
            blockImage.color = Color.white; 
            yield return new WaitForSeconds(0.1f);
            blockImage.color = originalColor; 
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(block.gameObject);
    }

    void MoveToNextRow()
    {
        previousBlocks.Clear();
        previousBlocks.AddRange(currentBlocks);

        if (!awaitingPlayerChoice && (!minorPrizeReached || continuedAfterMinorPrize))
        {
            currentRow++;
        }

        if (currentRow > 2 && columns == 3)
        {
            columns = 2;
            Debug.Log("Reduced to 2 blocks after row 3.");
        }

        if (currentRow > 6 && columns == 2)
        {
            columns = 1;
            Debug.Log("Reduced to 1 block after row 7.");
        }

        if (currentRow == 10 && !minorPrizeReached && !continuedAfterMinorPrize)
        {
            minorPrizeReached = true;
            awaitingPlayerChoice = true;
            isStopped = true;
            Debug.Log("You've reached the MINOR PRIZE! Press 'A' to collect or 'D' to continue.");
            ContinueOrCollect.enabled = true;
            return;
        }

        if (currentRow == 15)
        {
            Debug.Log("JACKPOT! You win the grand prize!");
            GameManager.Instance.AddTickets(Jackpot);
            ResetGame();
            return;
        }

        if (!awaitingPlayerChoice)
        {
            timeBetweenSteps = Mathf.Max(timeBetweenSteps - speedIncreaseFactor, minimumStepTime);
        }

        if (currentRow < rows && columns > 0)
        {
            SpawnBlocks(columns);
        }
        else if (currentRow >= rows)
        {
            Debug.Log("You Win! All blocks stacked.");
            ResetGame();
        }
        else
        {
            Debug.Log("Game Over! No more blocks.");
            ResetGame();
        }
    }



    void ContinueFromMinorPrize()
    {
        currentRow = 10;
        continuedAfterMinorPrize = true;  
        awaitingPlayerChoice = false;   
        isStopped = false;              
        SpawnBlocks(columns);            
        Debug.Log("Continuing from row 11...");
    }

    void CenterRemainingBlocks(int blockCount)
    {
        if (blockCount == 0)
            return;

        float totalRowWidth = blockCount * stepSize + (blockCount - 1) * padding;

        float startingXPosition = -(totalRowWidth / 2);

        for (int i = 0; i < blockCount; i++)
        {
            currentBlocks[i].anchoredPosition = new Vector2(startingXPosition + (i * (stepSize + padding)), currentBlocks[i].anchoredPosition.y);
        }
    }



    void SpawnBlocks(int blockCount)
    {
        currentBlocks = new RectTransform[blockCount];

        float panelHeight = gamePanel.rect.height;
        float rowYPosition = (-panelHeight / 2) + (currentRow * rowHeight);

        if (previousBlocks.Count >= blockCount)
        {
            for (int i = 0; i < blockCount; i++)
            {
                currentBlocks[i] = Instantiate(blockPrefab, gamePanel);
                currentBlocks[i].anchoredPosition = new Vector2(previousBlocks[i].anchoredPosition.x, rowYPosition);
            }
        }
        else
        {
            float totalRowWidth = blockCount * stepSize + (blockCount - 1) * padding;
            float startingXPosition = -(totalRowWidth / 2);

            for (int i = 0; i < blockCount; i++)
            {
                currentBlocks[i] = Instantiate(blockPrefab, gamePanel);
                currentBlocks[i].anchoredPosition = new Vector2(startingXPosition + (i * (stepSize + padding)), rowYPosition);
            }
        }

        isStopped = false; 
    }



    void ResetGame()
    {
        foreach (Transform child in gamePanel)
        {
            Destroy(child.gameObject);
        }

        timeBetweenSteps = initialStepTime;
        isGameActive = false;
        isStopped = true;
        awaitingPlayerChoice = false;      
        minorPrizeReached = false;         
        continuedAfterMinorPrize = false;  

        CreatePrizeLabelWithBackground(ref minorPrizeBackground, ref minorPrizeText, "MINOR: " + MinorPrize + " Tickets", 9, Color.white);

        CreatePrizeLabelWithBackground(ref jackpotBackground, ref jackpotText, "MAJOR: " + Jackpot + " Tickets", 14, Color.yellow);

        Debug.Log("Game Reset. Press 'E' to play again.");
    }

}
