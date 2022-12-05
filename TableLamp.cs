using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableLamp : MonoBehaviour
{
    public PhotonView PV;
    public GameObject TableLamp_Obj;
    public bool Trigger;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Trigger)
        {
            TableLamp_Obj.SetActive(true);
        }
        else 
        {
            TableLamp_Obj.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if  (other.gameObject.tag == "Move_Collider")
        {
            if ((other.GetComponent<PlayerController>().OpenDoor_State)&& (Trigger == false))
            {
                PV.RPC("Open_TableLamp", RpcTarget.AllBuffered, true);
                other.GetComponent<PlayerController>().OpenDoor_State = false;
            }
            else if ((other.GetComponent<PlayerController>().OpenDoor_State) && (Trigger == true))
            {
                PV.RPC("Open_TableLamp", RpcTarget.AllBuffered, false);
                other.GetComponent<PlayerController>().OpenDoor_State = false;
            }
        }
    }

    [PunRPC]
    void Open_TableLamp(bool Open)
    {
        if (Open)
        {
            Trigger = true;
        }
        else
        {
            Trigger = false;
        }
    }
}
