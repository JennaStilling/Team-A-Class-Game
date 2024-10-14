using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlockMovementUI : MonoBehaviour
{
    public float speed = 300f; // Speed for the block movement
    private RectTransform blockRectTransform;
    private RectTransform hitAreaRectTransform;
    private DDRGame gameController;
    private bool isHit = false;
    private string assignedKey;

    private Image hitAreaImage; // Reference to the hit area's Image component

    // Initialize block movement
    public void Initialize(RectTransform column, RectTransform hitArea, float blockSpeed, DDRGame controller)
    {
        speed = blockSpeed;
        blockRectTransform = GetComponent<RectTransform>();
        hitAreaRectTransform = hitArea;
        gameController = controller;

        hitAreaImage = hitArea.GetComponent<Image>(); // Get the Image component of the hit area

        // Assign key based on column index
        assignedKey = column.name switch
        {
            "Column1" => "c",
            "Column2" => "v",
            "Column3" => "n",
            "Column4" => "m",
            _ => ""
        };
    }

    // Update is called once per frame
    void Update()
    {
        // Move the block down the column
        blockRectTransform.anchoredPosition += Vector2.down * speed * Time.deltaTime;

        // Check if the block is in the 'Hit' area
        if (blockRectTransform.anchoredPosition.y <= hitAreaRectTransform.anchoredPosition.y + 50f && !isHit)
        {
            // If the player presses the correct key, destroy the block
            if (Input.GetKeyDown(assignedKey))
            {
                Debug.Log("Block hit successfully!");
                isHit = true;
                ChangeHitAreaColor(Color.green); // Change the hit area's color to green
                StartCoroutine(ResetHitAreaColor()); // Start coroutine to reset the color
                Destroy(gameObject);
            }
        }

        // If the block goes below the hit area and wasn't hit, trigger a miss
        if (blockRectTransform.anchoredPosition.y <= hitAreaRectTransform.anchoredPosition.y - 100f && !isHit)
        {
            gameController.OnBlockMissed();
            Destroy(gameObject); // Destroy the block if it reaches the bottom without a hit
        }
    }

    // Change the color of the hit area
    void ChangeHitAreaColor(Color color)
    {
        if (hitAreaImage != null)
        {
            hitAreaImage.color = color;
        }
    }

    // Coroutine to reset the hit area's color after a short delay
    IEnumerator ResetHitAreaColor()
    {
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds
        ChangeHitAreaColor(Color.white); // Reset the color back to white
    }
}
