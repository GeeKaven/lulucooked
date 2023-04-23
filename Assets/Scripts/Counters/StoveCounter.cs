using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {

  public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
  public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

  public class OnStateChangedEventArgs : EventArgs {
    public State state;
  }

  public enum State {
    Idle,
    Frying,
    Fried,
    Burned,
  }

  [SerializeField] private FryingRecipeSO[] fryingRecipeSOArr;
  [SerializeField] private BurningRecipeSO[] burningRecipesArr;


  private float fryingTimer;
  private FryingRecipeSO fryingRecipeSO;
  private State state;
  private float burningTimer;
  private BurningRecipeSO burningRecipeSO;

  private void Start() {
    state = State.Idle;
  }
  private void Update() {
    if (HasKitchenObject()) {
      switch (state) {
        case State.Idle:
          break;
        case State.Frying:
          fryingTimer += Time.deltaTime;

          OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
          });

          if (fryingTimer > fryingRecipeSO.fryingTimerMax) {

            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(fryingRecipeSO.outPut, this);
            state = State.Fried;
            burningTimer = 0f;
            burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
              state = state
            });


          }
          break;
        case State.Fried:
          burningTimer += Time.deltaTime;

          OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
          });

          if (burningTimer > burningRecipeSO.burningTimerMax) {

            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(burningRecipeSO.outPut, this);

            state = State.Burned;

            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
              state = state
            });

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
              progressNormalized = 0f
            });
          }
          break;
        case State.Burned:
          break;
      }
    }
  }

  public override void Interact(Player player) {
    if (!HasKitchenObject()) {
      if (player.HasKitchenObject()) {
        if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
          player.GetKitchenObject().SetKitchenObjectParent(this);
          fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
          state = State.Frying;
          fryingTimer = 0f;

          OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
            state = state
          });

          OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
          });
        }
      } else {
        // Player not carrying anything
      }
    } else {
      if (player.HasKitchenObject()) {

      } else {
        GetKitchenObject().SetKitchenObjectParent(player);

        state = State.Idle;

        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
          state = state
        });

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
          progressNormalized = 0f
        });
      }
    }
  }

  private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {

    FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);

    return fryingRecipeSO != null;
  }

  private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
    FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);

    return fryingRecipeSO != null ? fryingRecipeSO.outPut : null;
  }

  private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
    foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArr) {
      if (fryingRecipeSO.input == inputKitchenObjectSO) {
        return fryingRecipeSO;
      }
    }
    return null;
  }

  private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
    foreach (BurningRecipeSO burningRecipeSO in burningRecipesArr) {
      if (burningRecipeSO.input == inputKitchenObjectSO) {
        return burningRecipeSO;
      }
    }
    return null;
  }
}
