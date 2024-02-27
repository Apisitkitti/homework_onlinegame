using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using QFSW.QC;
using TMPro;
using System;
using System.Linq;
using Unity.VisualScripting;
using Unity.Mathematics;
using QFSW.QC.Parsers;
using System.Net.Http.Headers;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
  public TMP_InputField userNameInputField;
  public TMP_InputField passCodeInputField;
  public TMP_Dropdown skinSelector;
  public List<Material> statusObjectColor;
  public GameObject loginPannel;
  public GameObject leaveButton;
  public GameObject scorePanel;
  public List<GameObject> spawnPoint;
  public List<uint> AlternativePlayerPrefabs;

  private int room_id = 0;

  void Start()
  {
    NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
    NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    SetUIVisible(false);
  }
  public void SetUIVisible(bool isUserLogin)
  {
    if (isUserLogin)
    {
      loginPannel.SetActive(false);
      scorePanel.SetActive(true);
      leaveButton.SetActive(true);
    }
    else
    {
      loginPannel.SetActive(true);
      scorePanel.SetActive(false);
      leaveButton.SetActive(false);
    }
  }

  private void HandleClientDisconnect(ulong clientId)
  {
    Debug.Log("HandleClientDisconnect client ID = " + clientId);
    if (NetworkManager.Singleton.IsHost) { }
    else if (NetworkManager.Singleton.IsHost) { leaveButtonFunc(); }
  }

  private void HandleClientConnected(ulong clientId)
  {
    Debug.Log("HandleClientConnect client ID = " + clientId);
    if (clientId == NetworkManager.Singleton.LocalClientId)
    {
      SetUIVisible(true);
    }

  }
  public void leaveButtonFunc()
  {
    if (NetworkManager.Singleton.IsHost)
    {
      NetworkManager.Singleton.Shutdown();
      NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
    }
    else if (NetworkManager.Singleton.IsClient)
    {
      NetworkManager.Singleton.Shutdown();
    }
    SetUIVisible(false);
  }

  private void HandleServerStarted()
  {
    Debug.Log("HandleServerStart");
  }

  private void OnDestroy()
  {
    if (NetworkManager.Singleton == null) { return; }
    NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
    NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
    NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
  }
  public void Host()
  {
    NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    NetworkManager.Singleton.StartHost();
    room_id = int.Parse(passCodeInputField.GetComponent<TMP_InputField>().text);

    Debug.Log("start host");
  }
  private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
  {
    // The client identifier to be authenticated
    var clientId = request.ClientNetworkId;

    // Additional connection data defined by user code
    var connectionData = request.Payload;
    int byteLength = connectionData.Length;
    bool isApprove = false;
    if (byteLength > 0)
    {
      string rawData = System.Text.Encoding.ASCII.GetString(connectionData, 0, byteLength);
      string[] informationSplit = rawData.Split(":");
      string hostData = userNameInputField.GetComponent<TMP_InputField>().text;
      string usernameClient = informationSplit[0];
      int passcodeClient = int.Parse(informationSplit[1]);
      int SkinSelect = int.Parse(informationSplit[2]);
      Debug.Log(SkinSelect);
      isApprove = ApproveConnection(usernameClient, hostData, passcodeClient);
      response.PlayerPrefabHash = AlternativePlayerPrefabs[SkinSelect];
    }
    else
    {
      if (NetworkManager.Singleton.IsHost)
      {
        response.PlayerPrefabHash = AlternativePlayerPrefabs[skinSelected()];
      }
    }
    // Your approval logic determines the following values
    response.Approved = isApprove;
    response.CreatePlayerObject = true;
    // The Prefab hash value of the ;, if null the default NetworkManager player Prefab is used

    // Position to spawn the player object (if null it uses default of Vector3.zero)
    response.Position = Vector3.zero;

    // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
    response.Rotation = Quaternion.identity;
    setSpawnLocation(clientId, response);

    // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
    // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
    response.Reason = "Some reason for not approving the client";

    // If additional approval steps are needed, set this to true until the additional steps are complete
    // once it transitions from true to false the connection approval response will be processed.
    response.Pending = false;
  }
  private void setSpawnLocation(ulong clientId,
  NetworkManager.ConnectionApprovalResponse response)
  {
    Vector3 spawnPos = Vector3.zero;
    Quaternion spawnRo = Quaternion.identity;
    if (clientId == NetworkManager.Singleton.LocalClientId)
    {
      GameObject selectSpawn = spawnPoint[UnityEngine.Random.Range(0, spawnPoint.Count)];
      spawnPos = selectSpawn.transform.position;
      spawnRo = selectSpawn.transform.rotation;
    }
    else
    {
      GameObject selectSpawn = spawnPoint[UnityEngine.Random.Range(0, spawnPoint.Count)];
      spawnPos = selectSpawn.transform.position;
      spawnRo = selectSpawn.transform.rotation;
    }
    response.Position = spawnPos;
    response.Rotation = spawnRo;

  }
  public void Client()
  {
    string userName = userNameInputField.GetComponent<TMP_InputField>().text;
    int userPasscode = int.Parse(passCodeInputField.GetComponent<TMP_InputField>().text);
    int playerSkin = skinSelected();
    NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(userName + ":" + userPasscode + ":" + playerSkin);
    NetworkManager.Singleton.StartClient();
    Debug.Log("start client");
  }
  public bool ApproveConnection(string clientUsername, string hostUsername, int passcode)
  {
    bool isApprove = System.String.Equals(clientUsername.Trim(), hostUsername.Trim()) ? false : true;


    if (isApprove && (passcode == room_id))
    {
      return true;
    }
    else
    {
      return false;
    }

  }
  public int skinSelected()
  {
    if (skinSelector.GetComponent<TMP_Dropdown>().value == 0)
    {
      return 0;
    }
    else if (skinSelector.GetComponent<TMP_Dropdown>().value == 1)
    {
      return 1;
    }
    else if (skinSelector.GetComponent<TMP_Dropdown>().value == 2)
    {
      return 2;
    }
    else if (skinSelector.GetComponent<TMP_Dropdown>().value == 3)
    {
      return 3;
    }
    return 0;

  }
}

