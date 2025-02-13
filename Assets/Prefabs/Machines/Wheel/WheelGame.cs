using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelGame : MonoBehaviour
{
    [System.Serializable]
    public struct Segment
    {
        public string name;        // Name of the segment (e.g., "10 Tickets")
        public int ticketValue;    // Ticket value for this segment
        public float angle;        // Angle position of the segment
    }

    public TextMeshProUGUI CostUI;
    public TextMeshProUGUI StatusUI;
    public int TokenCost = 3;
    public Transform wheel;              // The wheel object to spin
    public Segment[] segments;           // Array of segments defining the wheel
    public int minFullSpins = 4;         // Minimum number of full rotations
    public int maxFullSpins = 10;        // Maximum number of full rotations
    public float spinDuration = 5f;      // Duration of the spin animation (seconds)
    public int extraFullSpins = 3;       // Random extra full spins for realism

    private bool isSpinning = false;     // Is the wheel currently spinning?
    private bool gameStarted = false;    // Has the player pressed "E" to start the game?

    private bool playerInRange = false;

    void Update()
    {
        if (!gameStarted)
        {
            CheckForGameStart();
            StatusUI.text = "Press 'E' to play";
        }
        else
        {
            if (!isSpinning)
            {
                StatusUI.text = "Click to Spin";
                if (Input.GetMouseButtonDown(0) && playerInRange)  // Detects mouse down for starting spin
                {
                    StartSpin();
                    StatusUI.text = "Good Luck!";
                }
            }
        }
        CostUI.text = "Token Cost: "+TokenCost;
    }

    // Check if the player presses the "E" key to start the game
    void CheckForGameStart()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {

            if (!GameManager.Instance.SpendToken(TokenCost))
            {
                Debug.Log("Not enough tokens");
                return;
            }
            Debug.Log("Wheel Game Started!");
            StatusUI.text = "Click to Spin";
            // Reset the wheel position to (0, 0, 0)
            wheel.localEulerAngles = Vector3.zero;

            gameStarted = true;
        }
    }

    // Start the spin process
    void StartSpin()
    {
        if (!isSpinning)
        {
            // Choose a random segment to land on
            int chosenSegmentIndex = Random.Range(0, segments.Length);
            Segment chosenSegment = segments[chosenSegmentIndex];

            Debug.Log($"Chosen segment: {chosenSegment.name} with {chosenSegment.ticketValue} tickets.");

            // Randomize the number of full spins between minFullSpins and maxFullSpins
            int fullSpins = Random.Range(minFullSpins, maxFullSpins + 1);

            // Randomize the number of extra spins for more realism
            int additionalSpins = Random.Range(1, extraFullSpins + 1);

            // Calculate the total target angle (full spins + segment angle + additional spins)
            float totalSpins = fullSpins + additionalSpins;
            float targetAngle = totalSpins * 360 + chosenSegment.angle;

            // Start the spinning animation
            StartCoroutine(SpinWheelAnimation(targetAngle, spinDuration, chosenSegment));
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

    // Coroutine to handle the entire spin animation from start to finish
    IEnumerator SpinWheelAnimation(float targetAngle, float duration, Segment chosenSegment)
    {
        isSpinning = true;

        float elapsedTime = 0f;
        float initialAngle = wheel.localEulerAngles.x;  // Starting angle of the wheel

        // Pre-calculate the total angle the wheel needs to rotate (full spins + segment)
        float totalAngle = initialAngle + targetAngle;

        // Animate the wheel's rotation over the given duration
        while (elapsedTime < duration)
        {
            // Calculate the percentage of time passed
            float t = elapsedTime / duration;

            // Apply a smooth cubic easing-out function for natural deceleration
            float easedT = EaseOutCubic(t);

            // Interpolate the wheel's rotation based on the easing function
            float currentAngle = Mathf.Lerp(initialAngle, totalAngle, easedT);

            // Rotate the wheel to the interpolated angle
            wheel.localEulerAngles = new Vector3(currentAngle % 360, 0, 0);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the wheel lands exactly at the target angle at the end
        wheel.localEulerAngles = new Vector3(targetAngle % 360, 0, 0);

        // Award tickets to the player after the spin is complete
        AwardTickets(chosenSegment.ticketValue);

        isSpinning = false;

        // Reset gameStarted to false so the player has to press "E" to start again
        gameStarted = false;
    }

    // Smooth easing-out function for deceleration
    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    // Award tickets to the player
    void AwardTickets(int tickets)
    {
        Debug.Log($"Awarding {tickets} tickets to the player.");
        GameManager.Instance.AddTickets(tickets);
    }
}
