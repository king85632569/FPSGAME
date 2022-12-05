using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;
    //public PlayerManager playerManager;
    public ScoreboardItem item;
    public PlayerManager playerManager;
    public string playerName;
    PhotonView PV;
    public bool nameSet;
    Dictionary<Player, ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem>();

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            foreach (Player player in PhotonNetwork.PlayerList) 
            {
                //PV.RPC("AddScoreboardItem", RpcTarget.All, player.NickName);
                //PV.RPC("AddScoreboardItem", RpcTarget.All, PhotonNetwork.NickName);
            }
        }
    }

    void Update()
    {
        if (playerManager.TeamSet==true)
        { 
        if ((item == null) && (PV.IsMine)&&(nameSet==false))
        {
                if (playerManager.AI_Mode == false)
                {
                    PV.RPC("neme", RpcTarget.AllBuffered, PhotonNetwork.NickName);
                }
                else
                {
                    playerName = playerManager.PlayerName;
                    PV.RPC("neme", RpcTarget.AllBuffered, playerName);
                }

        }
        if ((item == null)&& (nameSet == true))
        {
                if (playerName != null)
                {
                    // item.playerManager= playerManager;
                    //PV.RPC("AddScoreboardItem", RpcTarget.All, playerName);
                    AddScoreboardItem(playerName);
                }
        }



        if ((nameSet == true)&&(item != null))
        {
            item.playerName = playerName;
            //nameSet = false;
        }
        }
        if ((item != null) && (playerManager.AI_Mode) && (playerManager.AI_Destroy))
        {
            // item.playerManager= playerManager;
           Destroy(item.gameObject);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //RemoveScoreboardItem(otherPlayer);
    }
    [PunRPC]
    void neme(string _nickname)
    {
        playerName = _nickname;
        nameSet = true;
    }

    //[PunRPC]
    void AddScoreboardItem(string _nickname)
    {
        if (item == null)
        {
            if (playerManager.Team == true)
            {
                container = GameObject.Find("Scoreboard_Red").transform;
            }
            else
            {
                container = GameObject.Find("Scoreboard_Blue").transform;
            }
            item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
            item.playerManager = playerManager;
        }
    }

    void RemoveScoreboardItem(Player player)
    {
        //Destroy(scoreboardItems[player].gameObject);
        //scoreboardItems.Remove(player);
    }
}