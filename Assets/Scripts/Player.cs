using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public event EventHandler <OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterlayerMask;
    private bool isWalking;
    private Vector3 lastInteractionDir;
    private ClearCounter selectedCounter;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There are more then one player instances");
        }
        Instance = this;
    }
    private void Start()
    {
        gameInput.OnInteractActions += GameInput_OnInteractActions;
    }
    private void GameInput_OnInteractActions(object sender, System.EventArgs e)
    {
        if(selectedCounter != null)
        {
            selectedCounter.Interact(); 
        }
    }
    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }
    public bool IsWalking()
    {
        return isWalking;
    }
    private void HandleInteractions()
    {
        float interactDistance = 2f;

        Vector2 inputVector = gameInput.GetMovementVectorNormalize();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if(moveDir != Vector3.zero)
        {
            lastInteractionDir = moveDir;
        }
       if( Physics.Raycast(transform.position, lastInteractionDir, out RaycastHit raycastHit, interactDistance,counterlayerMask)){
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
                // Clear Counter
                if(clearCounter != selectedCounter)
                {
                    SetSelectedCounter(selectedCounter);  
                }
                else
                {
                    SetSelectedCounter(null);
                }
            }else
            {
                SetSelectedCounter(null);
            }
        }
    }

    private void HandleMovement()
    {
        {
            Vector2 inputVector = gameInput.GetMovementVectorNormalize();
            Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
            float moveDistance = moveSpeed * Time.deltaTime;
            float PlayerRadius = 0.7f;
            float playerHeight = 2f;
            bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, PlayerRadius, moveDir, moveDistance);

            if (!canMove)
            {
                Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, PlayerRadius, moveDirX, moveDistance);

                if (canMove)
                {
                    moveDir = moveDirX;
                }
                else
                {
                    Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                    canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, PlayerRadius, moveDirZ, moveDistance);
                    if (canMove)
                    {
                        moveDir = moveDirZ;
                    }
                    else
                    {
                    }
                }
            }
            if (canMove)
            {
                transform.position += moveDir * moveDistance;
            }
            isWalking = moveDir != Vector3.zero;
            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        }
    }
    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }
}
