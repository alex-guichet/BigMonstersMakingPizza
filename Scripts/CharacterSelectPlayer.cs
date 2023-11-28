using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{

   [SerializeField] private int playerIndex;
   [SerializeField] private GameObject playerReadyText;
   [SerializeField] private PlayerVisual playerVisual;
   [SerializeField] private Button kickButton;
   [SerializeField] private TextMeshPro playerNameText;
   
   
   private void Start()
   {
      
      kickButton.onClick.AddListener(() =>
      {
         
         PlayerData playerData = KitchenGameMultiplier.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
         KitchenGameLobby.Instance.KickPlayer(playerData.playerId.ToString());
         KitchenGameMultiplier.Instance.KickPlayer(playerData.clientId);
      });
      
      KitchenGameMultiplier.Instance.OnPlayerDataListChanged += KitchenGameMultiplierOnPlayerDataListChanged;
      CharacterSelectReady.Instance.OnPlayerReadyUpdate += CharacterSelectReadyOnPlayerReadyUpdate;
      UpdatePlayer();
      kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
   }

   private void CharacterSelectReadyOnPlayerReadyUpdate(object sender, EventArgs e)
   {
      UpdatePlayer();
   }

   private void KitchenGameMultiplierOnPlayerDataListChanged(object sender, EventArgs e)
   {
      UpdatePlayer();
   }

   private void UpdatePlayer()
   {
      if (playerIndex < KitchenGameMultiplier.Instance.GetPlayerConnectedCount())
      {
         Show();

         PlayerData playerData = KitchenGameMultiplier.Instance.GetPlayerData(playerIndex);
         
         playerReadyText.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

         playerNameText.text = playerData.playerName.ToString();
         
         playerVisual.SetMaterialColor(KitchenGameMultiplier.Instance.GetPlayerColor(playerData.colorId));
      }
      else
      {
         Hide();
      }

   }

   private void Hide()
   {
      gameObject.SetActive(false);
   }
   
   private void Show()
   {
      gameObject.SetActive(true);
   }

   private void OnDestroy()
   {
      KitchenGameMultiplier.Instance.OnPlayerDataListChanged -= KitchenGameMultiplierOnPlayerDataListChanged;
   }
}
