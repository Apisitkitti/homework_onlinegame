using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkBut : MonoBehaviour
{
  public void ServerActivate()
  {
    NetworkManager.Singleton.StartServer();
  }
  public void HostActivate()
  {
    NetworkManager.Singleton.StartHost();
  }
  public void ClientActivate()
  {
    NetworkManager.Singleton.StartClient();
  }

}
