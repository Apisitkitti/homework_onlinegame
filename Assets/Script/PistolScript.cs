using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class PistolScript : NetworkBehaviour
{
  public PistolSpawner PistolGenerate;
  public GameObject effectPrefab;
  public float destroyDelay;
  public float bulletSpeed;

  void Start()
  {
    if (!IsOwner) return;
    SpawnEffect();
    StartCoroutine(AutoDestroy());
  }
  private void OnCollisionEnter(Collision collision)
  {
    if (!IsOwner) return;
    if (collision.gameObject.tag == "Player")
    {
      ulong networkObjId = GetComponent<NetworkObject>().NetworkObjectId;
      StopCoroutine(AutoDestroy());
      PistolGenerate.DestroyServerRpc(networkObjId);
    }
  }
  void FixedUpdate()
  {
    transform.position += transform.right * bulletSpeed * Time.deltaTime;
  }
  private void SpawnEffect()
  {
    GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);

    effect.GetComponent<NetworkObject>().Spawn();
  }

  IEnumerator AutoDestroy()
  {
    ulong networkObjId = GetComponent<NetworkObject>().NetworkObjectId;
    yield return new WaitForSeconds(destroyDelay);
    PistolGenerate.DestroyServerRpc(networkObjId);
  }
}
