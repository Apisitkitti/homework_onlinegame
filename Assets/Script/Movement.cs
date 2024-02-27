using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
public class Movement : NetworkBehaviour
{
  [SerializeField] GameObject statusObject;
  public float speed = 0.5f;
  public float rotationSpeed = 0.5f;
  Rigidbody rb;
  public TMP_Text namePrefab;
  private TMP_Text nameLabel;
  private LoginManager loginManager;

  private NetworkVariable<int> posX = new NetworkVariable<int>(
    0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
  private NetworkVariable<bool> isOfflineStatus = new NetworkVariable<bool>(
    false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
  );
  public struct NetworkString : INetworkSerializable
  {
    public FixedString32Bytes info;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
      serializer.SerializeValue(ref info);
    }
    public override string ToString()
    {
      return info.ToString();
    }
    public static implicit operator NetworkString(string v) =>
      new NetworkString() { info = new FixedString32Bytes(v) };
  }
  public NetworkVariable<NetworkString> playerNameA = new NetworkVariable<NetworkString>(
    new NetworkString { info = "Player" },
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
  public NetworkVariable<NetworkString> playerNameB = new NetworkVariable<NetworkString>(
  new NetworkString { info = "Player" },
  NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

  public override void OnNetworkSpawn()
  {
    GameObject canvas = GameObject.FindWithTag("MainCanvas");
    nameLabel = Instantiate(namePrefab, Vector3.zero, Quaternion.identity) as TMP_Text;
    nameLabel.transform.SetParent(canvas.transform);
    posX.OnValueChanged += (int previousValue, int newValue) =>
    {
      Debug.Log("Owner ID = " + OwnerClientId + ": Pos X = " + posX.Value);
    };
    if (IsOwner)
    {
      loginManager = GameObject.FindObjectOfType<LoginManager>();
      if (loginManager != null)
      {
        string name = loginManager.userNameInputField.text;
        if (IsOwnedByServer) { playerNameA.Value = name; }
        else { playerNameB.Value = name; }
      }
    }
  }
  void Update()
  {
    Vector3 nameLabelPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2.5f, 0));
    nameLabel.text = gameObject.name;
    nameLabel.transform.position = nameLabelPos;
    if (IsOwner)
    {
      if (Input.GetKeyDown(KeyCode.F))
      {
        isOfflineStatus.Value = !isOfflineStatus.Value;
      }
      if (Input.GetKeyDown(KeyCode.Space))
      {
        TestServerRpc("Hello", new ServerRpcParams());
      }
      if (Input.GetKeyDown(KeyCode.L))
      {
        ClientRpcSendParams clientRpcSendParams = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 1 } };
        ClientRpcParams clientRpcParams = new ClientRpcParams { Send = clientRpcSendParams };
        TestClientRpc("Hi,this is Server", clientRpcParams);
      }


    }
    UpdatePlayerPrefab();
    ChangeColor();
  }
  [ServerRpc]
  private void TestServerRpc(string msg, ServerRpcParams servertRpcParams)
  {
    Debug.Log("test server rpc from server" + msg);
  }
  [ClientRpc]
  private void TestClientRpc(string msg, ClientRpcParams clientRpcParams)
  {
    Debug.Log("test server rpc from client" + msg);
  }
  void UpdatePlayerPrefab()
  {
    if (IsOwnedByServer)
    {
      nameLabel.text = playerNameA.Value.ToString();
    }
    else { nameLabel.text = playerNameB.Value.ToString(); }

  }
  public override void OnDestroy()
  {
    if (nameLabel != null) Destroy(nameLabel.gameObject);
    base.OnDestroy();
  }
  void Start()
  {
    rb = this.GetComponent<Rigidbody>();
    loginManager = GameObject.FindAnyObjectByType<LoginManager>();
  }
  // void FixedUpdate()
  // {
  //   playerMovement();
  // }
  // void playerMovement()
  // {
  //   if (IsOwner)
  //   {
  //     float translation = Input.GetAxis("Vertical") * speed;
  //     translation *= Time.deltaTime;
  //     rb.MovePosition(rb.position + this.transform.forward * translation);

  //     float rotation = Input.GetAxis("Horizontal");
  //     if (rotation != 0)
  //     {
  //       rotation *= rotationSpeed;
  //       Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
  //       rb.MoveRotation(rb.rotation * turn);
  //     }
  //     else
  //     {
  //       rb.angularVelocity = Vector3.zero;
  //     }
  //   }
  // }
  void ChangeColor()
  {
    if (IsOwnedByServer && OwnerClientId == 0)
    {
      if (isOfflineStatus.Value) { gameObject.GetComponentInChildren<Renderer>().material = loginManager.statusObjectColor[1]; }
      else { statusObject.GetComponent<Renderer>().material = loginManager.statusObjectColor[0]; }

    }
    else
    {
      if (isOfflineStatus.Value) { gameObject.GetComponentInChildren<Renderer>().material = loginManager.statusObjectColor[1]; }
      else { statusObject.GetComponent<Renderer>().material = loginManager.statusObjectColor[0]; }
    }
  }
  private void OnEnable()
  {
    if (nameLabel != null)
    {
      nameLabel.enabled = true;
    }
  }
  private void OnDisable()
  {
    if (nameLabel != null)
    {
      nameLabel.enabled = false;
    }
  }
}
