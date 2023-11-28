using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] private Button soundEffectButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button backButton;
    
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAltButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamePadInteractButton;
    [SerializeField] private Button gamePadInteractAltButton;
    [SerializeField] private Button gamePadPauseButton;
    
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAltText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI gamePadInteractText;
    [SerializeField] private TextMeshProUGUI gamePadInteractAltText;
    [SerializeField] private TextMeshProUGUI gamePadPauseText;
    
    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private TextMeshProUGUI musicButtonText;
    
    [SerializeField] private Transform updateBindingTransform;
    
    
    public static OptionsUI Instance { get; private set; }

    private Action _onCloseButtonAction;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateVisual();
        Hide();
        HideBindingUpdate();
        
        GameManager.Instance.OnLocalTogglePause += GameManagerOnLocalTogglePause;
        
        soundEffectButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        
        backButton.onClick.AddListener(() =>
        {
            _onCloseButtonAction.Invoke();
            Hide();
        });
        
        moveUpButton.onClick.AddListener(() =>
        {
            ShowBindingUpdate();
            GameInput.Instance.RebindBinding(GameInput.Binding.MoveUp, () =>
            {
                UpdateVisual();
                HideBindingUpdate();
            });
        });
        
        moveDownButton.onClick.AddListener(() =>
        {
            ShowBindingUpdate();
            GameInput.Instance.RebindBinding(GameInput.Binding.MoveDown, () =>
            {
                UpdateVisual();
                HideBindingUpdate();
            });
        });
        
        moveLeftButton.onClick.AddListener(() =>
        {
            ShowBindingUpdate();
            GameInput.Instance.RebindBinding(GameInput.Binding.MoveLeft, () =>
            {
                UpdateVisual();
                HideBindingUpdate();
            });
        });
        
        moveRightButton.onClick.AddListener(() =>
        {
            ShowBindingUpdate();
            GameInput.Instance.RebindBinding(GameInput.Binding.MoveRight, () =>
            {
                UpdateVisual();
                HideBindingUpdate();
            });
        });
        
        
        interactButton.onClick.AddListener(() =>
        {
            ShowBindingUpdate();
            GameInput.Instance.RebindBinding(GameInput.Binding.Interact, () =>
            {
                UpdateVisual();
                HideBindingUpdate();
            });
        });
        
        interactAltButton.onClick.AddListener(() =>
        {
            ShowBindingUpdate();
            GameInput.Instance.RebindBinding(GameInput.Binding.InteractAlt, () =>
            {
                UpdateVisual();
                HideBindingUpdate();
            });
        });
        
        pauseButton.onClick.AddListener(() =>
        {
            ShowBindingUpdate();
            GameInput.Instance.RebindBinding(GameInput.Binding.Pause, () =>
            {
                UpdateVisual();
                HideBindingUpdate();
            });
        });
        
        gamePadInteractButton.onClick.AddListener(() =>
        {
            ShowBindingUpdate();
            GameInput.Instance.RebindBinding(GameInput.Binding.GamePadInteract, () =>
            {
                UpdateVisual();
                HideBindingUpdate();
            });
        });
        
        
        gamePadInteractAltButton.onClick.AddListener(() =>
        {
            ShowBindingUpdate();
            GameInput.Instance.RebindBinding(GameInput.Binding.GamePadInteractAlt, () =>
            {
                UpdateVisual();
                HideBindingUpdate();
            });
        });
        
        
        gamePadPauseButton.onClick.AddListener(() =>
        {
            ShowBindingUpdate();
            GameInput.Instance.RebindBinding(GameInput.Binding.GamePadPause, () =>
            {
                UpdateVisual();
                HideBindingUpdate();
            });
        });
    }

    private void GameManagerOnLocalTogglePause(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePaused())
        {
            Hide();
        }
    }

    private void UpdateVisual()
    {
        soundEffectText.text = "Sound effect volume : " + SoundManager.Instance.GetVolume();
        musicButtonText.text = "Music volume : " + MusicManager.Instance.GetVolume();

        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        gamePadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePadInteract);
        gamePadInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePadInteractAlt);
        gamePadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePadPause);
    }

    public void Show(Action onCloseButtonAction)
    {
        _onCloseButtonAction = onCloseButtonAction;
        gameObject.SetActive(true);
        soundEffectButton.Select();
    }
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void ShowBindingUpdate()
    {
        updateBindingTransform.gameObject.SetActive(true);
    }
    
    private void HideBindingUpdate()
    {
        updateBindingTransform.gameObject.SetActive(false);
    }
    
    
}
