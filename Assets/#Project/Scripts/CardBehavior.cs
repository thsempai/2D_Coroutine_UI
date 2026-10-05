using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class CardBehavior : MonoBehaviour
{
    private const string FACE_UP_ANIMATION = "IsFaceUp";
    private const string MOUSE_OVER_ANIMATION = "IsFocused";

    [SerializeField] private Sprite spriteFaceDown;
    [SerializeField] private Sprite spriteFaceUp;

    // do no have the warning about mask effect.
#pragma warning disable CS0108
    [SerializeField] private SpriteRenderer renderer;

    public bool IsFaceUp { get; private set; } = false;
    public bool IsFocused { get; private set; } = false;

    private LevelManager manager = null;
    public int Id { get; private set; }
    public int FaceId { get; private set; }



#pragma warning restore CS0108

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (renderer == null)
        {
            renderer = GetComponentInChildren<SpriteRenderer>();
            if (renderer == null)
            {
                Debug.LogError("CardBehavior needs a SpriteRender to works.");
            }
        }

        if (spriteFaceDown == null)
        {
            spriteFaceDown = renderer.sprite;
        }
        else
        {
            renderer.sprite = spriteFaceDown;
        }

    }

    private void Turn(bool isFaceUp)
    {
        animator.SetBool(FACE_UP_ANIMATION, isFaceUp);
        IsFaceUp = isFaceUp;
    }

    private void SetFocus(bool isFocused)
    {
        IsFocused = isFocused;
        animator.SetBool(MOUSE_OVER_ANIMATION, isFocused);
    }

    public void Focus() => SetFocus(true);

    public void UnFocus() => SetFocus(false);

    public void TurnFaceUp() => Turn(true);
    public void TurnFaceDown() => Turn(false);

    private void FaceUp()
    {
        renderer.sprite = spriteFaceUp;
    }

    private void FaceDown()
    {
        renderer.sprite = spriteFaceDown;
    }

    public void ConnectToManager(LevelManager manager, int id)
    {
        this.manager = manager;
        Id = id;
    }

    public void SetFace(Sprite faceUp, int faceId)
    {
        spriteFaceUp = faceUp;
        FaceId = faceId;
    }

}
