using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint_Group : MonoBehaviourPunCallbacks
{
    public GameObject[] SpawnPointsGroup;
    public int Range=-1;
    public PhotonView PV;
    public bool SelectOver;
    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            Range = Random.Range(0, SpawnPointsGroup.Length);
            PV.RPC("RangeSelect", RpcTarget.AllBuffered, Range);
        }
        if ((Range >= 0) && (SelectOver == false))
        {
            SpawnPointsGroup[Range].SetActive(true);
            SelectOver = true;
        }
    }
    [PunRPC]
    void RangeSelect(int Range_RPC)
    {
        Range = Range_RPC;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if ((Range >= 0) && (SelectOver == false))
            {
                SpawnPointsGroup[Range].SetActive(true);
                SelectOver = true;
            }
        }
    }
}
