using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawnerScript : NetworkBehaviour
{
  public List<Behaviour> scripts;
  private Renderer[] renderers;
  void Start()
  {
    // mainPlayer = gameObject.GetComponent<Movement>();
    renderers = GetComponentsInChildren<Renderer>();
  }
  void SetPlayerState(bool state)
  {
    foreach (var scripts in scripts) { scripts.enabled = state; }
    foreach (var renderers in renderers) { renderers.enabled = state; }
  }
  private Vector3 GetRandomPos()
  {
    Vector3 randPos = new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    return randPos;
  }
  public void Respawn()
  {
    RespawnServerRpc();
  }
  [ServerRpc]
  private void RespawnServerRpc()
  {
    Vector3 pos = GetRandomPos();
    RespawnClientRpc(pos);
  }
  [ClientRpc]
  private void RespawnClientRpc(Vector3 spawnPos)
  {
    StartCoroutine(RespawnCoroutine(spawnPos));
  }
  IEnumerator RespawnCoroutine(Vector3 spawnPos)
  {
    SetPlayerState(false);
    transform.position = spawnPos;
    yield return new WaitForSeconds(3f);
    SetPlayerState(true);
  }
}
