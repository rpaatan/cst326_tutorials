using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }
    // a part of the singleton pattern. not a field, but a property (logic can be added when getting / setting field.)
    
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged; 
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter; 
    }
    
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask; 
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance. what");
        }
        Instance = this; 
    }
    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction; 
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this); 
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this); 
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
        // Rather than creating a member variable here, we create an instance of moveDir
        // in this function so we get a moveDir unaffected by the checking/changes we do on our moveDir to handle diagonals. 
        Vector2 inputVector = gameInput.GetMovementVectorNormalized(); 
        
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        // stores our last direction so we can still interact with items when we're not moving.
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir; 
        }

        float interactDistance = 2f; 
        
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            // RaycastHit becomes a variable we can utilize later. Function above outputs a boolean, but also gives us the output of that local variable.
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // has clear counter.
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null); 
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized(); 
        
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime; 
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = 
            moveDir.x !=0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius,moveDir, moveDistance);

        // Code that allows us to hug the wall and slide against it when moving diagonally against an object we're colliding with. 
        if (!canMove)
        {
            // If we cannot move towards moveDir, attempt only X movement. 
            
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized; 
            // Normalize the movement so that we're not moving slower diagonally -- opposite issue from before. 
            
            canMove = 
                moveDir.x !=0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius,moveDirX, moveDistance);

            if (canMove)
            {
                // If you can only move on the X, then move on the X. 
                moveDir = moveDirX; 
            }
            else
            {
                // If we cannot move on the X, check if we can move on the Z. 
                
              Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized; 
                canMove = 
                    !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius,moveDirZ, moveDistance);

                if (canMove)
                {
                    // If we can move on the Z, then move on the Z. 
                    moveDir = moveDirZ; 
                }
                else
                {
                    // We cannot move on either the X or Z, so we cannot move at all.
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

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter; 
        
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
            // selectedCounter, the first one, belongs to the OnSelectedCounterChangedEventArgs. The second is our reference.
        }); 
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint; 
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject; 
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null; 
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null; 
    }
}
