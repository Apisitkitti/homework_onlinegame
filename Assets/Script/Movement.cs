using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Movement : NetworkBehaviour
{
  public float speed = 0.5f;
  public float rotationSpeed = 0.5f;
  Rigidbody rb;
  void Start()
  {
    rb = this.GetComponent<Rigidbody>();
  }
  void FixedUpdate()
  {
    playerMovement();
  }
  void playerMovement()
  {
    if (IsOwner)
    {
      float translation = Input.GetAxis("Vertical") * speed;
      translation *= Time.deltaTime;
      rb.MovePosition(rb.position + this.transform.forward * translation);

      float rotation = Input.GetAxis("Horizontal");
      if (rotation != 0)
      {
        rotation *= rotationSpeed;
        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turn);
      }
      else
      {
        rb.angularVelocity = Vector3.zero;
      }
    }
  }
}
