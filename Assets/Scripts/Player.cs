using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

  [SerializeField] private float moveSpeed = 7f;

  private bool isWalking;

  private void Update() {
    Vector2 inputVector = new Vector2(0, 0);
    if (Input.GetKey(KeyCode.W)) {
      inputVector.y = +1;
    }
    if (Input.GetKey(KeyCode.A)) {
      inputVector.x = -1;
    }
    if (Input.GetKey(KeyCode.S)) {
      inputVector.y = -1;
    }
    if (Input.GetKey(KeyCode.D)) {
      inputVector.x = +1;
    }

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
    inputVector = inputVector.normalized;
    transform.position += moveDir * moveSpeed * Time.deltaTime;

    isWalking = moveDir != Vector3.zero;
    float slerpSpeed = 10f;
    transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * slerpSpeed);

  }

  public bool IsWalking() {
    return isWalking;
  }
}
