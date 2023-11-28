using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSoArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSoArray;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    private NetworkVariable<float> _fryingTimer = new();
    private NetworkVariable<float> _burningTimer = new();

    private FryingRecipeSO _fryingRecipeSo;
    private BurningRecipeSO _burningRecipeSo;
    
    private NetworkVariable<State> _currentState = new();
    

    public override void OnNetworkSpawn()
    {
        _fryingTimer.OnValueChanged += FryingTimerOnValueChanged;
        _burningTimer.OnValueChanged += BurningTimerOnValueChanged;
        _currentState.OnValueChanged += CurrentStateOnValueChanged;
    }

    private void CurrentStateOnValueChanged(State previousState, State newState)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
        {
            state = _currentState.Value
        });

        if (_currentState.Value == State.Burned || _currentState.Value == State.Idle)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });
        }
    }

    private void FryingTimerOnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = _fryingRecipeSo != null ? _fryingRecipeSo.fryingTimerMax : 1f;
        
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = _fryingTimer.Value/fryingTimerMax
        });
    }
    
    private void BurningTimerOnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = _burningRecipeSo != null ? _burningRecipeSo.burningTimerMax : 1f;
        
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = _burningTimer.Value/burningTimerMax
        });
    }


    private void Update()
    {
        if (!IsServer)
            return;
        
        if (HasKitchenObject())
        {
            switch (_currentState.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    _fryingTimer.Value += Time.deltaTime;
                    
                    if (_fryingTimer.Value > _fryingRecipeSo.fryingTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSo.output, this);
                        _currentState.Value = State.Fried;
                        _burningTimer.Value = 0f;
                        SetBurningRecipeSoClientRpc(KitchenGameMultiplier.Instance.GetKitchenObjectSoIndex(GetKitchenObject().GetKitchenObjectSO()));
                    }
                    break;
                case State.Fried:
                    _burningTimer.Value += Time.deltaTime;
                    
                    if (_burningTimer.Value > _burningRecipeSo.burningTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(_burningRecipeSo.output, this);
                        _currentState.Value = State.Burned;
                        
                    }
                    break;
                case State.Burned:
                    
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public override void Interact(Player player){
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasFryingRecipeSo(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicOnCounterPlacedServerRpc(KitchenGameMultiplier.Instance.GetKitchenObjectSoIndex(kitchenObject.GetKitchenObjectSO()));
                }
            }
            else
            {
                //Player Doesn't have a kitchen object
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                SetStateIdleServerRpc();
            }
            else
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredients(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        SetStateIdleServerRpc();
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        _currentState.Value = State.Idle;
    }
    

    [ServerRpc (RequireOwnership = false)]
    private void InteractLogicOnCounterPlacedServerRpc(int kitchenObjectSoIndex)
    {
        SetFryingRecipeSoClientRpc(kitchenObjectSoIndex);
        _fryingTimer.Value = 0f;
        _burningTimer.Value = 0f;
        _currentState.Value = State.Frying;
    }
    
    [ClientRpc]
    private void SetFryingRecipeSoClientRpc(int kitchenObjectSoIndex)
    {
        KitchenObjectSO kitchenObjectSo = KitchenGameMultiplier.Instance.GetKitchenObjectSoFromIndex(kitchenObjectSoIndex);
        _fryingRecipeSo = GetFryingRecipeSo(kitchenObjectSo);
    }
    
    
    [ClientRpc]
    private void SetBurningRecipeSoClientRpc(int kitchenObjectSoIndex)
    {
        KitchenObjectSO kitchenObjectSo = KitchenGameMultiplier.Instance.GetKitchenObjectSoFromIndex(kitchenObjectSoIndex);
        _burningRecipeSo = GetBurningRecipeSo(kitchenObjectSo);
    }

    private bool HasFryingRecipeSo(KitchenObjectSO inputKitchenObject)
    {
        var fryingRecipeSo = GetFryingRecipeSo(inputKitchenObject);
        return fryingRecipeSo != null;
    }
    
    private FryingRecipeSO GetFryingRecipeSo(KitchenObjectSO inputKitchenObject)
    {
        foreach (var fryingRecipeSo in fryingRecipeSoArray)
        {
            if (inputKitchenObject == fryingRecipeSo.input)
            {
                return fryingRecipeSo;
            }
        }
        return null;
    }
    
    
    private BurningRecipeSO GetBurningRecipeSo(KitchenObjectSO inputKitchenObject)
    {
        foreach (var burningRecipeSo in burningRecipeSoArray)
        {
            if (inputKitchenObject == burningRecipeSo.input)
            {
                return burningRecipeSo;
            }
        }
        return null;
    }

    public State GetCurrentState()
    {
        return _currentState.Value;
    }
}
