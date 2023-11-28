using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Netcode;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask counterLayer;
    [SerializeField] private LayerMask collisionsLayer;
    [SerializeField] private Transform kitchenObjectSpawnPoint;
    [SerializeField] private List<Vector3> playerSpawnPointList;
    [SerializeField] private PlayerVisual playerVisual;
    
    private KitchenObject _kitchenObject;
    
    private BaseCounter _selectedCounter;

    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;
    public static event EventHandler OnAnyDroppedSomething;
    
    public event EventHandler<OnSelectedCounterChangedEventArgument> OnSelectedCounterChanged;
    public event EventHandler OnPickUp;
    public event EventHandler OnDrop;
    public class OnSelectedCounterChangedEventArgument : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    public static Player localInstance { get; private set; }
    
    public static void ResetStaticData()
    {
        OnAnyDroppedSomething = null;
        OnAnyPickedSomething = null;
        OnAnyPlayerSpawned = null;
    }

    
    
    public bool isWalking { get; private set; }
    
    private void HandleInteraction()
    {
        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, interactDistance, counterLayer))
        {
            if (hitInfo.transform.TryGetComponent(out BaseCounter clearCounter))
            {
                SelectCounter(clearCounter);
            }
            else
            {
                SelectCounter(null);
            }
        }
        else
        {
            SelectCounter(null);
        }
    }

    private void HandleMovementServerAuth()
    {
        Vector2 playerInput = GameInput.Instance.GetInputMovementNormalized();
        HandleMovementServerRpc(playerInput);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void HandleMovementServerRpc(Vector2 playerInput)
    {
        Vector3 moveDirection = new Vector3(playerInput.x, 0, playerInput.y);

        float playerHeight = 2f;
        float playerRadius = .7f;
        float distanceMove = moveSpeed * Time.deltaTime;

        isWalking = moveDirection != Vector3.zero;

        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirection, Quaternion.identity, collisionsLayer);
        
        if (!canMove)
        {
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = Mathf.Abs(moveDirection.x) > .5f && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirectionX, Quaternion.identity, collisionsLayer);

            if (canMove)
            {
                moveDirection = moveDirectionX;
            }
            else
            {
                Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = Mathf.Abs(moveDirection.z) > .5f && !Physics.BoxCast(transform.position, Vector3.one * playerRadius,  moveDirectionZ, Quaternion.identity, collisionsLayer);

                if (canMove)
                {
                    moveDirection = moveDirectionZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDirection * distanceMove;
        }

        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
    }
    
    private void HandleMovement()
    {
        Vector2 playerInput = GameInput.Instance.GetInputMovementNormalized();
        Vector3 moveDirection = new Vector3(playerInput.x, 0, playerInput.y);

        float playerHeight = 2f;
        float playerRadius = .7f;
        float distanceMove = moveSpeed * Time.deltaTime;

        isWalking = moveDirection != Vector3.zero;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirection, distanceMove);
        
        if (!canMove)
        {
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = Mathf.Abs(moveDirection.x) > .5f && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirectionX, distanceMove);

            if (canMove)
            {
                moveDirection = moveDirectionX;
            }
            else
            {
                Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = Mathf.Abs(moveDirection.z) > .5f && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirectionZ, distanceMove);

                if (canMove)
                {
                    moveDirection = moveDirectionZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDirection * distanceMove;
        }

        if (moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
        }
    }
    
    private void GameInputOnInteractAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())
            return;
        
        if (_selectedCounter != null)
        {
            _selectedCounter.Interact(this);
        }
    }

    private void GameInputOnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())
            return;
        
        if (_selectedCounter != null)
        {
            _selectedCounter.InteractAlternate(this);
        }
    }
    
    private void SelectCounter(BaseCounter selectedCounter)
    {
        _selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgument {
            selectedCounter = _selectedCounter
        });
    }
    
    public Transform GetObjectSpawnPoint()
    {
        return kitchenObjectSpawnPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
        OnPickUp?.Invoke(this, EventArgs.Empty);
        OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
        
    }
   
    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }
   
    public void ClearKitchenObject()
    {
        _kitchenObject = null;
        OnDrop?.Invoke(this, EventArgs.Empty);
        OnAnyDroppedSomething?.Invoke(this, EventArgs.Empty);
    }
   
    public bool HasKitchenObject()
    {
        return _kitchenObject != null;
    }

    private void Awake()
    {
        //instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            localInstance = this;
        }
        
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
        transform.position = playerSpawnPointList[KitchenGameMultiplier.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

        PlayerData playerData = KitchenGameMultiplier.Instance.GetPlayerDataFromClientId(OwnerClientId);
        
        playerVisual.SetMaterialColor(KitchenGameMultiplier.Instance.GetPlayerColor(playerData.colorId));

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += PlayerOnClientDisconnectCallback;
        }
    }

    private void PlayerOnClientDisconnectCallback(ulong clientId)
    {
        if (OwnerClientId == clientId && HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInputOnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInputOnInteractAlternateAction;
    }



    private void Update()
    {
        if (!IsOwner)
            return;

        //HandleMovementServerAuth();
        HandleMovement();
        HandleInteraction();
    }

    public NetworkObject GetNetworkObject()
    {
        return GetComponent<NetworkObject>();
    }
        
}
