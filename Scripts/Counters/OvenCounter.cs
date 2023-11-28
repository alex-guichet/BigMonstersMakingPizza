using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OvenCounter : BaseCounter, IHasProgress
{
    
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }
    
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    [SerializeField] private float cookingTimerMax;
    [SerializeField] private float burningTimerMax;
    
    private NetworkVariable<float> _cookingTimer = new();
    private NetworkVariable<float> _burningTimer = new();

    private FryingRecipeSO[] _fryingRecipeSo;
    private BurningRecipeSO[] _burningRecipeSo;
    private List<KitchenObjectSO> _kitchenObjectSoToCook = new();
    
    private NetworkVariable<State> _currentState = new();

    private PlateKitchenObject _currentPlateKitchenObject;
    

    public override void OnNetworkSpawn()
    {
        _cookingTimer.OnValueChanged += FryingTimerOnValueChanged;
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
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = _cookingTimer.Value/cookingTimerMax
        });
    }
    
    private void BurningTimerOnValueChanged(float previousValue, float newValue)
    {
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
                    _cookingTimer.Value += Time.deltaTime;
                    
                    if (_cookingTimer.Value > cookingTimerMax)
                    {
                        foreach (KitchenObjectSO kitchenObjectSo in _kitchenObjectSoToCook)
                        {
                            if (kitchenObjectSo.kitchenObjectSoCooked == null)
                                continue;
                            _currentPlateKitchenObject.TryRemoveIngredients(kitchenObjectSo);
                            _currentPlateKitchenObject.AddIngredientsWithoutAllowedOnPlateCheck(kitchenObjectSo.kitchenObjectSoCooked);
                        }

                        SetKitchenObjectSoToCookListServerRpc();
                        _currentState.Value = State.Fried;
                        _burningTimer.Value = 0f;
                    }
                    break;
                case State.Fried:
                    _burningTimer.Value += Time.deltaTime;
                    
                    if (_burningTimer.Value > burningTimerMax)
                    {
                        foreach (KitchenObjectSO kitchenObjectSo in _kitchenObjectSoToCook)
                        {
                            if (kitchenObjectSo.kitchenObjectSoCooked == null)
                                continue;
                            _currentPlateKitchenObject.TryRemoveIngredients(kitchenObjectSo);
                            _currentPlateKitchenObject.AddIngredientsWithoutAllowedOnPlateCheck(kitchenObjectSo.kitchenObjectSoCooked);
                        }

                        SetKitchenObjectSoToCookListServerRpc();
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.HasAllowedKitchenObjectOnPlate())
                    {
                        plateKitchenObject.TurnOffUIIcons();
                        KitchenObject kitchenObject = player.GetKitchenObject();
                        kitchenObject.SetKitchenObjectParent(this);
                        
                        SetStateIdleServerRpc();
                        InteractLogicOnCounterPlacedServerRpc();
                    }
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
                GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject);
                plateKitchenObject.TurnOnUIIcons();
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
    private void InteractLogicOnCounterPlacedServerRpc()
    {
        SetCurrentPlateKitchenObjectClientRpc();
        SetKitchenObjectSoToCookListClientRpc();
        _cookingTimer.Value = 0f;
        _burningTimer.Value = 0f;
        _currentState.Value = State.Frying;
    }
    
    [ServerRpc (RequireOwnership = false)]
    private void SetKitchenObjectSoToCookListServerRpc()
    {
        SetKitchenObjectSoToCookListClientRpc();
    }
    

    [ClientRpc]
    private void SetKitchenObjectSoToCookListClientRpc()
    {
        if (_kitchenObjectSoToCook.Count > 0)
        {
            _kitchenObjectSoToCook.Clear();
        }
        
        foreach (var kitchenObjectSo in _currentPlateKitchenObject.GetKitchenObjectSoList())
        {
            _kitchenObjectSoToCook.Add(kitchenObjectSo);
        }
    }


    [ClientRpc]
    private void SetCurrentPlateKitchenObjectClientRpc()
    {
        if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
        {
            _currentPlateKitchenObject = plateKitchenObject;
        }
    }

    public State GetCurrentState()
    {
        return _currentState.Value;
    }
}
