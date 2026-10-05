using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    private const string ACTION_MAP = "Game";
    private const string ACTION_MOUSE_POSITION = "Mouse Position";
    private const string ACTION_MOUSE_CLICK = "Click";
    [SerializeField] private InputActionAsset inputActions;
    InputAction mousePosition;
    InputAction mouseClick;
    [SerializeField] private LevelManager levelManager;

    private void Awake()
    {
        mousePosition = inputActions.FindActionMap(ACTION_MAP).FindAction(ACTION_MOUSE_POSITION);
        mouseClick = inputActions.FindActionMap(ACTION_MAP).FindAction(ACTION_MOUSE_CLICK);

        mouseClick.performed += ctx => { OnClick(ctx); };

        if (levelManager == null)
        {
            levelManager = FindAnyObjectByType<LevelManager>();
            if (levelManager == null)
            {
                Debug.LogError("MouseManager need a LevelManager.");
            }
        }

    }

    void OnEnable()
    {
        inputActions.FindActionMap(ACTION_MAP).Enable();
    }

    void OnDisable()
    {
        inputActions.FindActionMap(ACTION_MAP).Disable();
    }

    private void Update()
    {
        CheckMouseOver();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        levelManager.MouseClick();
    }

    private void CheckMouseOver()
    {
        Vector2 position = mousePosition.ReadValue<Vector2>();
        position = Camera.main.ScreenToWorldPoint(position);

        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.up, 0.001f);

        if (hit.collider != null)
        {
            CardBehavior cardBehavior = hit.collider.GetComponentInParent<CardBehavior>();
            levelManager.MouseOnCard(cardBehavior);
        }
        else
        {
            levelManager.MouseOnCard(null);
        }
    }
}
