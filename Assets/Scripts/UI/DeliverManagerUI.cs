using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverManagerUI : MonoBehaviour {
  [SerializeField] private Transform container;
  [SerializeField] private Transform recipeTemplate;

  private void Awake() {
    recipeTemplate.gameObject.SetActive(false);
  }

  private void Start() {
    DeliverManager.Instance.OnRecipeSpawned += DeliverManager_OnRecipeSpawned;
    DeliverManager.Instance.OnRecipeCompleted += DeliverManager_OnRecipeCompleted;

    UpdateVisual();
  }

  private void DeliverManager_OnRecipeSpawned(object sender, EventArgs e) {
    UpdateVisual();
  }

  private void DeliverManager_OnRecipeCompleted(object sender, EventArgs e) {
    UpdateVisual();
  }

  private void UpdateVisual() {
    foreach (Transform child in container) {
      if (child == recipeTemplate) continue;
      Destroy(child.gameObject);
    }

    foreach (RecipeSO recipeSO in DeliverManager.Instance.GetWaitingRecipeSOList()) {
      Transform recipeTransform = Instantiate(recipeTemplate, container);
      recipeTransform.gameObject.SetActive(true);
      recipeTransform.GetComponent<DeliverManagerSingleUI>().SetRecipeSO(recipeSO);
    }
  }
}
