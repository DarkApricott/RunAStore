using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ReviewPoints
{
    positive,
    neutral,
    negative
}

public class ReviewMeter : MonoBehaviour
{
    [Range(1, 100)]
    public int reviewScore = 50;

    public ReviewPoints review;

    public Vector2Int[] spawnRates = new Vector2Int[System.Enum.GetValues(typeof(ReviewPoints)).Length];

    [SerializeField] private TMP_Text reviewText;

    private UIManager uiManager;

    [SerializeField] private Sprite[] reviewSprites = new Sprite[3];

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    private void LateUpdate()
    {
        if (reviewScore >= 75)
        {
            review = ReviewPoints.positive;

            uiManager.mainHappinessEmoji.sprite = reviewSprites[0];
            reviewText.color = Color.green;
        }
        
        if(reviewScore < 75 && reviewScore >= 30)
        {
            review = ReviewPoints.neutral;

            uiManager.mainHappinessEmoji.sprite = reviewSprites[1];
            reviewText.color = Color.white;
        }

        else if(reviewScore < 30)
        {
            review = ReviewPoints.negative;

            uiManager.mainHappinessEmoji.sprite = reviewSprites[2];
            reviewText.color = Color.red;
        }

        reviewText.text = reviewScore + "%";

        if(reviewScore < 0)
        {
            reviewScore = 0;
        }

        if(reviewScore > 100)
        {
            reviewScore = 100;
        }
    }
}
