using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PistolSpawner : NetworkBehaviour
{
  public GameObject PistolPrefab;
  private List<GameObject> spawnedPistol = new List<GameObject>();
  void Update()
  {
    if (!IsOwner) return;
    if (Input.GetKeyDown(KeyCode.Mouse0))
    {
      SpawnPistolServerRpc();
    }
  }
  [ServerRpc]
  void SpawnPistolServerRpc()
  {
    Vector3 spawnPos = transform.position + (transform.forward * -1.5f) + (transform.up * 1.5f);
    Quaternion spawnRot = transform.rotation;
    GameObject pistol = Instantiate(PistolPrefab, spawnPos, spawnRot);
    spawnedPistol.Add(pistol);
    pistol.GetComponent<PistolScript>().PistolGenerate = this;
    pistol.GetComponent<NetworkObject>().Spawn();
  }

  [ServerRpc(RequireOwnership = false)]
  public void DestroyServerRpc(ulong networkObjId)
  {
    GameObject obj = findSpawnerPistol(networkObjId);
    if (obj == null) return;
    obj.GetComponent<NetworkObject>().Despawn();
    spawnedPistol.Remove(obj);
    Destroy(obj);
  }

  private GameObject findSpawnerPistol(ulong netWorkObjId)
  {
    foreach (GameObject pistol in spawnedPistol)
    {
      ulong pistolId = pistol.GetComponent<NetworkObject>().NetworkObjectId;
      if (pistolId == netWorkObjId) { return pistol; }
    }
    return null;
  }
}
