using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 3f;
    [SerializeField] private GameObject frontVisual;
    [SerializeField] private GameObject backVisual;
    [SerializeField] private GameObject leftVisual;
    [SerializeField] private GameObject rightVisual;

    private Rigidbody2D body;
    private Vector2 movement;
    private Facing facing = Facing.Front;

    private enum Facing
    {
        Front,
        Back,
        Left,
        Right
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        if (frontVisual == null)
        {
            frontVisual = FindVisual("Player_Front");
        }

        if (backVisual == null)
        {
            backVisual = FindVisual("Player_Back");
        }

        if (leftVisual == null)
        {
            leftVisual = FindVisual("Player_Left");
        }

        if (rightVisual == null)
        {
            rightVisual = FindVisual("Player_Right");
        }

        UpdateVisuals(false);
    }

    private void Update()
    {
        movement = ReadMovement();

        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        bool isMoving = movement.sqrMagnitude > 0.0001f;
        if (isMoving)
        {
            facing = GetFacing(movement);
        }

        UpdateVisuals(isMoving);
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private static Vector2 ReadMovement()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return Vector2.zero;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            horizontal -= 1f;
        }

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            horizontal += 1f;
        }

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            vertical -= 1f;
        }

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            vertical += 1f;
        }

        return new Vector2(horizontal, vertical);
    }

    private GameObject FindVisual(string visualName)
    {
        Transform visual = transform.Find(visualName);
        return visual != null ? visual.gameObject : null;
    }

    private static Facing GetFacing(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x < 0f ? Facing.Left : Facing.Right;
        }

        return direction.y < 0f ? Facing.Front : Facing.Back;
    }

    private void UpdateVisuals(bool isMoving)
    {
        SetVisual(frontVisual, facing == Facing.Front, isMoving);
        SetVisual(backVisual, facing == Facing.Back, isMoving);
        SetVisual(leftVisual, facing == Facing.Left, isMoving);
        SetVisual(rightVisual, facing == Facing.Right, isMoving);
    }

    private static void SetVisual(GameObject visual, bool isActive, bool isMoving)
    {
        if (visual == null)
        {
            return;
        }

        if (visual.activeSelf != isActive)
        {
            visual.SetActive(isActive);
        }

        if (isActive && visual.TryGetComponent(out Animator animator))
        {
            animator.enabled = isMoving;
        }
    }
}
