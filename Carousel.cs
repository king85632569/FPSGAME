using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carousel : MonoBehaviour
{
    public PhotonView PV;
    public float Speed;
    public GameObject Carousel_Obj;
    public bool Trigger;
    public int Close_Time;
    public Animator Anim;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        Anim = Carousel_Obj.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((PhotonNetwork.IsMasterClient) && (Trigger))
        {
            Carousel_Obj.transform.Rotate(Vector3.forward * Speed * Time.deltaTime);

        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Move_Collider")
        {
            //Debug.Log("進入旋轉木馬");
            if ((other.GetComponent<PlayerController>().OpenDoor_State)&&(Trigger==false))
            {
                PV.RPC("Open_Carousel", RpcTarget.AllBuffered, true);
                other.GetComponent<PlayerController>().OpenDoor_State = false;
            }
            else if((other.GetComponent<PlayerController>().OpenDoor_State) && (Trigger == true))
            {
                PV.RPC("Open_Carousel", RpcTarget.AllBuffered, false);
                other.GetComponent<PlayerController>().OpenDoor_State = false;
            }

        }
    }

    [PunRPC]
    void Open_Carousel(bool Open)
    {
        if (Open)
        {
           Trigger = true;
           Anim.SetBool("Run", true);
           Invoke("Close_Carousel", Close_Time);
        }
        else
        {
            Trigger = false;
            Anim.SetBool("Run", false);
            CancelInvoke("Close_Carousel");
        }
    }
    void Close_Carousel()
    {
        PV.RPC("Open_Carousel", RpcTarget.AllBuffered, false);
    }
}
