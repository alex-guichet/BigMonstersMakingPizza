using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance;

    public event EventHandler OnPause;
    public event EventHandler OnInteractAction ;
    public event EventHandler OnInteractAlternateAction ;
    public event EventHandler OnRebindBinding;
    
    private PlayerInputActionMap _playerInputActionMap;

    private const string PLAYER_PREFS_BINDINGS = "bindings";
    
    public enum Binding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlt,
        Pause,
        GamePadInteract,
        GamePadInteractAlt,
        GamePadPause
    }
    
    public Vector2 GetInputMovementNormalized()
    {
        Vector2 playerInput = _playerInputActionMap.Player.Move.ReadValue<Vector2>();
        
        playerInput = playerInput.normalized;

        return playerInput;
    }

    private void Awake()
    {
        Instance = this;
        
        _playerInputActionMap = new PlayerInputActionMap();
        _playerInputActionMap.Enable();
        
        _playerInputActionMap.Player.Interact.performed += InteractOnPerformed;
        _playerInputActionMap.Player.AlternateInteract.performed += AlternateInteractOnPerformed ;
        _playerInputActionMap.Player.Pause.performed += PauseOnPerformed ;

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            _playerInputActionMap.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
    }

    private void OnDestroy()
    {
        _playerInputActionMap.Player.Interact.performed -= InteractOnPerformed;
        _playerInputActionMap.Player.AlternateInteract.performed -= AlternateInteractOnPerformed ;
        _playerInputActionMap.Player.Pause.performed -= PauseOnPerformed ;
        _playerInputActionMap.Dispose();
    }

    private void PauseOnPerformed(InputAction.CallbackContext obj)
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }

    private void AlternateInteractOnPerformed(InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractOnPerformed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            case Binding.MoveUp:
                return _playerInputActionMap.Player.Move.bindings[1].ToDisplayString();
            case Binding.MoveDown:
                return _playerInputActionMap.Player.Move.bindings[2].ToDisplayString();
            case Binding.MoveLeft:
                return _playerInputActionMap.Player.Move.bindings[3].ToDisplayString();
            case Binding.MoveRight:
                return _playerInputActionMap.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return _playerInputActionMap.Player.Interact.GetBindingDisplayString(0);
            case Binding.InteractAlt:
                return _playerInputActionMap.Player.AlternateInteract.GetBindingDisplayString(0);
            case Binding.Pause:
                return _playerInputActionMap.Player.Pause.GetBindingDisplayString(0);
            case Binding.GamePadInteract:
                return _playerInputActionMap.Player.Interact.GetBindingDisplayString(1);
            case Binding.GamePadInteractAlt:
                return _playerInputActionMap.Player.AlternateInteract.GetBindingDisplayString(1);
            case Binding.GamePadPause:
                return _playerInputActionMap.Player.Pause.GetBindingDisplayString(1);
            default:
                throw new ArgumentOutOfRangeException(nameof(binding), binding, null);
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        _playerInputActionMap.Disable();

        InputAction inputAction;
        int bindingIndex;

        switch (binding)
        {
            case Binding.MoveUp:
                inputAction = _playerInputActionMap.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.MoveDown:
                inputAction = _playerInputActionMap.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.MoveLeft:
                inputAction = _playerInputActionMap.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.MoveRight:
                inputAction = _playerInputActionMap.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = _playerInputActionMap.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlt:
                inputAction = _playerInputActionMap.Player.AlternateInteract;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = _playerInputActionMap.Player.Pause;
                bindingIndex = 0;
                break;
            case Binding.GamePadInteract:
                inputAction = _playerInputActionMap.Player.Interact;
                bindingIndex = 1;
                break;
            case Binding.GamePadInteractAlt:
                inputAction = _playerInputActionMap.Player.AlternateInteract;
                bindingIndex = 1;
                break;
            case Binding.GamePadPause:
                inputAction = _playerInputActionMap.Player.Pause;
                bindingIndex = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(binding), binding, null);
        }
        
        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete((callback) =>
        {
            callback.Dispose();
            _playerInputActionMap.Enable();
            onActionRebound();
            PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, _playerInputActionMap.SaveBindingOverridesAsJson());
        }).Start();
        
        OnRebindBinding?.Invoke(this, EventArgs.Empty);
    }

}
