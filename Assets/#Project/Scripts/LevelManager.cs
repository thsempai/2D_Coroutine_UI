//disable warning about readonly field
#pragma warning disable IDE0044

using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField][Range(2, 24)] private int cardNumber = 6;
    [SerializeField] private int cardsByLine = 4;
    [SerializeField] private Vector2 offSet = Vector2.one * 0.2f;
    [SerializeField] private CardBehavior prefab;
    [SerializeField] private Sprite[] spritesFaceUp;

    private List<CardBehavior> cardBehaviors = new();

    private int cardFocusedId = -1;

    private CardBehavior CardFocused => cardFocusedId >= 0 ? cardBehaviors[cardFocusedId] : null;

    private void Start()
    {
        if (cardNumber % 2 != 0)
        {
            cardNumber++;
            Debug.LogWarning($"Card Number need to be an even number then it's became {cardNumber}.");
        }

        InstantiateCards();
    }

    private void InstantiateCards()
    {
        List<int> facesPoolIndex = new();

        for (int index = 0; index < cardNumber / 2; index++)
        {
            facesPoolIndex.Add(index);
            facesPoolIndex.Add(index);
        }

        BoxCollider2D collider = prefab.GetComponentInChildren<BoxCollider2D>();
        float width = collider.size.x;
        float height = collider.size.y;

        int lines = cardNumber / cardsByLine;
        if (cardNumber % cardsByLine != 0) lines++;

        float px = (cardsByLine * (width + offSet.x) - offSet.x) / -2f;
        float py = (lines * (height + offSet.y) - offSet.y) / -2f;
        Vector2 origin = new(px, py);

        for (int y = 0; y < lines; y++)
        {
            for (int x = 0; x < cardsByLine; x++)
            {
                Vector2 position = origin + y * (height + offSet.y) * Vector2.up;
                position += x * (width + offSet.x) * Vector2.right;
                CardBehavior cardBehavior = Instantiate(prefab, position, Quaternion.identity);

                int rndIndex = Random.Range(0, facesPoolIndex.Count);
                int faceIndex = facesPoolIndex[rndIndex];
                cardBehavior.SetFace(spritesFaceUp[faceIndex], faceIndex);
                facesPoolIndex.RemoveAt(rndIndex);

                cardBehavior.ConnectToManager(this, cardBehaviors.Count);

                cardBehaviors.Add(cardBehavior);
                if (cardBehaviors.Count >= cardNumber) return;
            }
        }
    }

    public void MouseOnCard(CardBehavior cardBehavior)
    {
        if (cardBehavior == null)
        {
            if (CardFocused != null)
            {
                CardFocused.UnFocus();
                cardFocusedId = -1;
            }

        }
        else if (cardBehavior.Id != cardFocusedId)
        {
            if (CardFocused != null)
            {
                CardFocused.UnFocus();
            }
            cardFocusedId = cardBehavior.Id;
            cardBehavior.Focus();
        }
    }

    public void MouseClick()
    {
        if (CardFocused != null)
        {
            CardFocused.TurnFaceUp();
        }
    }
}
