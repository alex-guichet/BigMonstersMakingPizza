using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI keyboardUpText;
  [SerializeField] private TextMeshProUGUI keyboardDownText;
  [SerializeField] private TextMeshProUGUI keyboardLeftText;
  [SerializeField] private TextMeshProUGUI keyboardRightText;
  [SerializeField] private TextMeshProUGUI keyboardInteractText;
  [SerializeField] private TextMeshProUGUI keyboardInteractAltText;
  [SerializeField] private TextMeshProUGUI keyboardPauseText;
  [SerializeField] private TextMeshProUGUI gamePadInteractText;
  [SerializeField] private TextMeshProUGUI gamePadInteractAltText;
  [SerializeField] private TextMeshProUGUI gamePadPauseText;
  [SerializeField] private TextMeshProUGUI interactText;


  private void Start()
  {
      UpdateVisual();
      GameManager.Instance.OnLocalPlayerReadyChanged += GameManagerOnLocalPlayerReadyChanged;
      GameInput.Instance.OnRebindBinding += GameInputOnRebindBinding;
  }

  private void GameInputOnRebindBinding(object sender, EventArgs e)
  {
      UpdateVisual();
  }

  private void GameManagerOnLocalPlayerReadyChanged(object sender, EventArgs e)
  {
      if (GameManager.Instance.IsLocalPlayerReady())
      {
          Hide();
      }
  }


  private void UpdateVisual()
  {
      keyboardUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
      keyboardDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
      keyboardLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
      keyboardRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
      keyboardInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
      keyboardInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
      keyboardPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
      gamePadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePadInteract);
      gamePadInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePadInteractAlt);
      gamePadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.GamePadPause);
      interactText.text = "PRESS " + GameInput.Instance.GetBindingText(GameInput.Binding.Interact) + "/" +
                          GameInput.Instance.GetBindingText(GameInput.Binding.GamePadInteract) + " TO CONTINUE";
  }

  private void Show()
  {
      gameObject.SetActive(true);
  }
  
  private void Hide()
  {
      gameObject.SetActive(false);
  }
}
