//disable warning about readonly field
#pragma warning disable IDE0044

using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    [SerializeField][Range(2, 24)] private int cardsNumber = 6;
    [SerializeField] private int cardsByLine = 4;
    [SerializeField] private Vector2 offSet = Vector2.one * 0.2f;
    [SerializeField] private CardBehavior prefab;
    [SerializeField] private Sprite[] spritesFaceUp;
    [SerializeField] private float timeBeforeFaceDown;
    [SerializeField] private float timeBeforeVictoryDisplay = 1f;
    [SerializeField] private GameObject[] gameObjectsActivateOnVictory;

    public MouseManager MouseManager { get; private set; }

    private List<CardBehavior> cardBehaviors = new();

    private List<int> faceIdAlreadyFaceUp = new();

    private int Pairs => faceIdAlreadyFaceUp.Count;

    private int cardFocusedId = -1;

    private int firstCardFaceUpId = -1;

    private int secondCardFaceUpId = -1;

    private CardBehavior CardFocused => cardFocusedId >= 0 ? cardBehaviors[cardFocusedId] : null;

    private CardBehavior FirstCardFaceUp => firstCardFaceUpId >= 0 ? cardBehaviors[firstCardFaceUpId] : null;

    private CardBehavior SecondCardFaceUp => secondCardFaceUpId >= 0 ? cardBehaviors[secondCardFaceUpId] : null;

    private void Start()
    {
        if (cardsNumber % 2 != 0)
        {
            cardsNumber++;
            Debug.LogWarning($"Card Number need to be an even number then it's became {cardsNumber}.");
        }

        foreach (GameObject go in gameObjectsActivateOnVictory)
        {
            go.SetActive(false);
        }
    }


    public void SetCardsNumber(int n)
    {
        cardsNumber = n;

        if (cardsNumber % 2 != 0)
        {
            cardsNumber++;
            Debug.LogWarning($"Card Number need to be an even number then it's became {cardsNumber}.");
        }
    }
    public void InstantiateCards()
    {
        List<int> facesPoolIndex = new();

        for (int n = 0; n < cardsNumber / 2; n++)
        {
            int index = Random.Range(0, spritesFaceUp.Length);
            while (facesPoolIndex.Contains(index))
                index = Random.Range(0, spritesFaceUp.Length);

            facesPoolIndex.Add(index);
            facesPoolIndex.Add(index);
        }

        BoxCollider2D collider = prefab.GetComponentInChildren<BoxCollider2D>();
        float width = collider.size.x;
        float height = collider.size.y;

        int lines = cardsNumber / cardsByLine;
        if (cardsNumber % cardsByLine != 0) lines++;

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
                if (cardBehaviors.Count >= cardsNumber) return;
            }
        }
    }

    public void ResetGame()
    {
        List<int> facesPoolIndex = new();

        for (int n = 0; n < cardsNumber / 2; n++)
        {
            int index = Random.Range(0, spritesFaceUp.Length);
            while (facesPoolIndex.Contains(index))
                index = Random.Range(0, spritesFaceUp.Length);

            facesPoolIndex.Add(index);
            facesPoolIndex.Add(index);
        }

        foreach (CardBehavior cardBehavior in cardBehaviors)
        {
            int rndIndex = Random.Range(0, facesPoolIndex.Count);
            int faceIndex = facesPoolIndex[rndIndex];
            cardBehavior.SetFace(spritesFaceUp[faceIndex], faceIndex);
            facesPoolIndex.RemoveAt(rndIndex);
        }

        foreach (GameObject go in gameObjectsActivateOnVictory)
        {
            go.SetActive(false);
        }

        faceIdAlreadyFaceUp.Clear();
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
        else if (cardBehavior.Id != cardFocusedId && cardBehavior.Id != firstCardFaceUpId)
        {
            if (CardFocused != null)
            {
                CardFocused.UnFocus();
            }
            if (!faceIdAlreadyFaceUp.Contains(cardBehavior.FaceId))
            {
                cardFocusedId = cardBehavior.Id;
                cardBehavior.Focus();
            }
        }
    }

    public void ConnectMouseManager(MouseManager mouseManager)
    {
        MouseManager = mouseManager;
    }

    public void MouseClick()
    {
        if (CardFocused != null)
        {
            CardFocused.TurnFaceUp();

            if (FirstCardFaceUp == null)
            {
                firstCardFaceUpId = cardFocusedId;

            }
            else
            {
                secondCardFaceUpId = cardFocusedId;
                StartCoroutine(CheckResult());
            }

            CardFocused.UnFocus();
            cardFocusedId = -1;
        }
    }

    private IEnumerator VictoryDisplay()
    {
        yield return new WaitForSeconds(timeBeforeVictoryDisplay);
        foreach (CardBehavior cardBehavior in cardBehaviors)
        {
            cardBehavior.TurnFaceDown();
        }

        foreach (GameObject go in gameObjectsActivateOnVictory)
        {
            go.SetActive(true);
        }
    }

    private IEnumerator CheckResult()
    {
        MouseManager.enabled = false;

        yield return new WaitForSeconds(timeBeforeFaceDown);

        if (FirstCardFaceUp.FaceId != SecondCardFaceUp.FaceId)
        {
            FirstCardFaceUp.TurnFaceDown();
            SecondCardFaceUp.TurnFaceDown();
        }
        else
        {
            faceIdAlreadyFaceUp.Add(FirstCardFaceUp.FaceId);
            Debug.Log($"Nombre de paires retournées : {Pairs}");
            if (Pairs >= cardsNumber / 2)
            {
                StartCoroutine(VictoryDisplay());
            }
        }

        firstCardFaceUpId = -1;
        secondCardFaceUpId = -1;

        MouseManager.enabled = true;
    }
}
