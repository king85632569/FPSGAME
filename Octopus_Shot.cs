using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Octopus_Shot : MonoBehaviour
{
    public PhotonView PV;
    public GameObject Aim;
    public GameObject BubbleObj;
    public GameObject Muzzle;
    public bool Team;
    public AI_Monster AI_Mon;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        AI_Mon= GetComponent<AI_Monster>();
        Aim = AI_Mon.Target;
    }

    // Update is called once per frame
    void Update()
    {
        //Shoot();
        if (PhotonNetwork.IsMasterClient)
        {
            Team = AI_Mon.Monster_Team;
            if (AI_Mon.Target != null)
            {
                Aim = AI_Mon.Target;
            }
        }
        SetBubbleTransform();
    }
    public void SetBubbleTransform()
    {
        if (BubbleObj != null)
        {
            BubbleObj.transform.position = Muzzle.transform.position;
            if (Aim != null)
            {
                BubbleObj.transform.LookAt(Aim.transform.position);
            }
        }
    }
    [PunRPC]
    void RPC_Bubble()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject bubbleImpactObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BubbleImpact"), Muzzle.transform.position, Muzzle.transform.rotation, 0, new object[] { PV.ViewID });
            BubbleObj = bubbleImpactObj;
            BubbleObj.gameObject.GetComponent<Bubble>().Ride_Shoot = true;
            BubbleObj.gameObject.GetComponent<Bubble>()?.Bubble_Set(30, Team, false, false, 0, true);
        }
    }
    public void SetBubbleStart()
    {
        PV.RPC("RPC_BubbleStart", RpcTarget.All);
    }
    [PunRPC]
    void RPC_BubbleStart()
    {
        if (BubbleObj != null)
        {
            BubbleObj.gameObject.GetComponent<Bubble>().Fly_Start = true;
        }
        BubbleObj = null;
    }
    public void AI_Shot()
    {
        if (BubbleObj == null)
        {
            Shoot();
            Invoke("SetBubbleStart", 2);
        }
        //Debug.Log(BubbleState);
    }
    void Shoot()
    {
        PV.RPC("RPC_Bubble", RpcTarget.All);
    }
}
