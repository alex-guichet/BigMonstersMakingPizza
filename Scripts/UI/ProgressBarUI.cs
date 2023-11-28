using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
   [SerializeField] private GameObject hasProgressGameObject;
   [SerializeField] private Image bar;

   private IHasProgress _hasProgress;
   
   private void Start()
   {
      _hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
      
      if (_hasProgress == null)
      {
         Debug.LogError("The GameObject" + hasProgressGameObject.name + "doesn't have an IHasProgressInterface attached to it");
         return;
      }

      _hasProgress.OnProgressChanged += OnProgressChanged;

      bar.fillAmount = 0f;
      
      Hide();
   }

   private void OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
   {
      var progression = e.progressNormalized;
      bar.fillAmount = progression;
      Show();
      
      if (progression is 0 or 1)
      {
         Hide();
      }
      else
      {
         Show();
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
}
