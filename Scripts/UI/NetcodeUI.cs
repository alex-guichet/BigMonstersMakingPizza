using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetcodeUI : MonoBehaviour
{
   [SerializeField] private Button hostButton;
   [SerializeField] private Button clientButton;

   private void Start()
   {
      hostButton.onClick.AddListener(() =>
      {
         KitchenGameMultiplier.Instance.StartHost();
         Hide();
      });
      
      clientButton.onClick.AddListener(() =>
      {
         KitchenGameMultiplier.Instance.StartClient();
         Hide();
      });
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
