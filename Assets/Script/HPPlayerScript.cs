using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class HPPlayerScript : NetworkBehaviour
{
  TMP_Text p1Text;
  TMP_Text p2Text;
  Movement mainPlayer;
  private Animator anim;
  public NetworkVariable<int> hpP1 = new NetworkVariable<int>(5,
  NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

  public NetworkVariable<int> hpP2 = new NetworkVariable<int>(5,
  NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

  // Start is called before the first frame update
  void Start()
  {
    p1Text = GameObject.Find("P1HPText (TMP)").GetComponent<TMP_Text>();
    p2Text = GameObject.Find("P2HPText (TMP)").GetComponent<TMP_Text>();
    anim = GetComponent<Animator>();
    mainPlayer = GetComponent<Movement>();
  }

  private void UpdatePlayerNameAndScore()
  {
    if (IsOwnedByServer)
    {
      p1Text.text = $"{mainPlayer.playerNameA.Value} : {hpP1.Value}";
    }
    else
    {
      p2Text.text = $"{mainPlayer.playerNameB.Value} : {hpP2.Value}";
    }
  }

  // Update is called once per frame
  void Update()
  {
    UpdatePlayerNameAndScore();

  }

  private void OnCollisionEnter(Collision collision)
  {
    if (!IsLocalPlayer) return;
    if (collision.gameObject.tag == "DeathZone")
    {
      if (IsOwnedByServer)
      {
        gameObject.GetComponent<PlayerSpawnerScript>().Respawn();
        hpP1.Value = 5;
      }
      else
      {
        gameObject.GetComponent<PlayerSpawnerScript>().Respawn();
        hpP2.Value = 5;

      }
    }
    if (collision.gameObject.tag == "Bomb" || collision.gameObject.tag == "Pistol")
    {
      if (IsOwnedByServer)
      {
        hpP1.Value = hpP1.Value - 2;
        anim.SetTrigger("damage");
        if (hpP1.Value <= 0)
        {
          gameObject.GetComponent<PlayerSpawnerScript>().Respawn();
          hpP1.Value = 5;
        }
      }
      else
      {
        hpP2.Value = hpP2.Value - 2;
        anim.SetTrigger("damage");
        if (hpP2.Value <= 0)
        {
          gameObject.GetComponent<PlayerSpawnerScript>().Respawn();

          hpP2.Value = 5;
        }
      }

    }
  }
}

