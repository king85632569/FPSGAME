using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuantityControl : MonoBehaviour
{
    PhotonView PV;
    public bool Team;
    public GameObject[] Team_All_PlayerManager;
    public GameObject[] All_PlayerManager;
    public int Player_Character_Number;
    public RoomManager roomManager;
    public bool AI_set;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        Player_Character_Number = 1;
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (AI_set == false)
            {
                AI_Set(roomManager.mod);
            }
            else
            {
                GetPlayer();
            }
        }
    }


    public void GetPlayer()
    {
        All_PlayerManager = GameObject.FindGameObjectsWithTag("PlayerManager");
        for (int i = 0; i < All_PlayerManager.Length; i++)
        {
            if ((All_PlayerManager[i].GetComponent<PlayerManager>().Team == Team)&&(All_PlayerManager[i].GetComponent<PlayerManager>().AI_Mode == false)&& (All_PlayerManager[i].GetComponent<PlayerManager>().Replace == false))
            {
                if (roomManager.mod == 1)
                {
                    Team_All_PlayerManager[0].GetComponent<PlayerManager>().Destroy = true;
                    All_PlayerManager[i].GetComponent<PlayerManager>().AI_Replace(true);
                }
                else
                {
                    if ((All_PlayerManager[i].GetComponent<PlayerManager>().Character_Number == 0) && (Team_All_PlayerManager[0].GetComponent<PlayerManager>().Destroy == false))
                    {
                        Team_All_PlayerManager[0].GetComponent<PlayerManager>().Destroy = true;
                        All_PlayerManager[i].GetComponent<PlayerManager>().AI_Replace(true);
                    }
                    else if (All_PlayerManager[i].GetComponent<PlayerManager>().Character_Number > 0)
                    {
                        for (int x = 1; x < Team_All_PlayerManager.Length; x++)
                        {
                            if (Team_All_PlayerManager[x].GetComponent<PlayerManager>().Destroy == false)
                            {
                                Team_All_PlayerManager[x].GetComponent<PlayerManager>().Destroy = true;
                                All_PlayerManager[i].GetComponent<PlayerManager>().AI_Replace(true);
                                Debug.Log("埃AI");
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    void AI_Set(int mod)
    {
        PV.RPC("RPC_AI_Set", RpcTarget.AllBuffered, mod);
    }

    [PunRPC]
    void RPC_AI_Set(int mod)
    {
        AI_set = true;
        if (mod == 1)//家Α1(1v1家Α)
        {
            for (int i = 1; i < Team_All_PlayerManager.Length; i++)
            {
                Team_All_PlayerManager[i].GetComponent<PlayerManager>().Destroy = true;
            }
        }
        else if (mod == 2)//家Α2(2v2家Α)
        {
            for (int i = 1; i < Team_All_PlayerManager.Length-1; i++)
            {
                Team_All_PlayerManager[i].GetComponent<PlayerManager>().Destroy = true;
            }
        }
        else if (mod == 3)//家Α3(3v3家Α)
        {
            for (int i = 1; i < Team_All_PlayerManager.Length-2; i++)
            {
                Team_All_PlayerManager[i].GetComponent<PlayerManager>().Destroy = true;
            }
        }
    }

}
