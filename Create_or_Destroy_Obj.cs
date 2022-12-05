using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Create_or_Destroy_Obj : MonoBehaviour
{
    public GameObject[] Create_Obj;
    public GameObject[] Destroy_Obj;
    public GameObject[] Trigger_Obj;
    public BoxCollider Collider;
    public bool Trigger, Trigger_Over;
    public TMP_Text text;
    public float time,Anim_Time;
    public bool CountDown;
    public PhotonView PV;
    public bool Team;//Ture=Red ; False=Blue
    public Animator Anim, Decode_Anim;
    public bool Start_Over;
    public GameObject Octopus_Obj, Octopus_transform;
    public int Player_Character_Number;
    public bool Need_King, Need_King_Main;
    public int Trigger_Count;
    public bool Patrol;
    public bool Octopus_Decode;
    public int Player_ViewID;
    public GameObject Decode_Light;//Decode_Obj

    public void Start()
    {
        Trigger = false;
        Collider = GetComponent<BoxCollider>();
        PV = GetComponent<PhotonView>();
        Create_or_Destroy(false);
    }
    public void GetTrigger()
    {
        Trigger_Count = 0;
        int x = 0;
        for (int i = 0; i < Trigger_Obj.Length; i++)
        {
            x += 1;
            if ((Trigger_Obj[i].GetComponent<Create_or_Destroy_Obj>().Need_King == true) && (Trigger_Obj[i].GetComponent<Create_or_Destroy_Obj>().Trigger == true))
            {
                Trigger_Count += 1;
            }
            if (Octopus_Decode)
            {
                if (Trigger_Obj[i].GetComponent<Create_or_Destroy_Obj>().Trigger == false)
                {
                    Trigger_Obj[i].GetComponent<Create_or_Destroy_Obj>().Trigger_True(true);
                }
                Trigger_Obj[i].GetComponent<Create_or_Destroy_Obj>().text.text = "達成條件";
            }
        }
        if (x == Trigger_Obj.Length)
        {
            if (Trigger_Count >= 5)
            {
                Octopus_Decode = true;
                if ((Anim != null) && (Start_Over == false))
                {
                    Anim.SetBool("Start", true);
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Octopus_Obj.transform.position = Octopus_transform.transform.position;
                        Octopus_Obj.transform.rotation = Octopus_transform.transform.rotation;
                    }
                    Invoke("Anim_Over", Anim_Time);
                    //Anim_Over();
                }
                else if ((Anim != null) && (Start_Over == true))
                {
                    Create_or_Destroy(true);
                    Trigger_Over = true;
                }
                //text.text = "成功解鎖";
            }
        }
    }

    public void OnTriggerEnter(Collider c)
    {
        if ((PhotonNetwork.IsMasterClient) && (c.CompareTag("Move_Collider")) && (Trigger == false))
        {
            Team = c.gameObject.GetComponent<PlayerController>().Team;
            Player_Character_Number = c.gameObject.GetComponent<PlayerController>().Character_Number;
            Player_ViewID = c.gameObject.GetComponent<PhotonView>().ViewID;
            PV.RPC("RPC_Player_ViewID", RpcTarget.AllBuffered, Player_ViewID);

            //if ((Need_King) && (Player_Character_Number == 0))
            if (Need_King)
            {
                PV.RPC("RPC_CountDown", RpcTarget.AllBuffered, true, time);
            }
            else if(Need_King == false)
            {
                PV.RPC("RPC_CountDown", RpcTarget.AllBuffered, true, time);
            }
        }
    }
    public void OnTriggerExit(Collider c)
    {
        if ((PV.IsMine) && (c.CompareTag("Move_Collider")) && (Trigger == false))
        {
            PV.RPC("RPC_CountDown", RpcTarget.AllBuffered, false, time);
        }
    }

    public void Trigger_True(bool trigger)
    {
        PV.RPC("RPC_CountDown", RpcTarget.AllBuffered, false, time);
        PV.RPC("RPC_Trigger", RpcTarget.AllBuffered, trigger);
    }
    void Anim_Over()
    {
        CancelInvoke("Anim_Over");
        PV.RPC("RPC_Anim_Over", RpcTarget.AllBuffered);
    }
    [PunRPC]
    void RPC_Anim_Over()
    {
        Start_Over = true;
    }

    [PunRPC]
    void RPC_Player_ViewID(int RPC_ViewID)
    {
        Player_ViewID = RPC_ViewID;
    }


    [PunRPC]
    void RPC_Trigger(bool trigger)
    {
        if (trigger)
        {
            Trigger = true;
        }
        else
        {
            Trigger = false;
        }
    }

    public void Update()
    {
        if ((Need_King)&&(Need_King_Main))
        {
            GetTrigger();
        }

        if ((Trigger == true) && (Trigger_Over == false))
        {
            //Trigger_True(false);
            if (Anim == null)
            {
                Create_or_Destroy(true);
                Trigger_Over = true;
            }
            else if ((Anim != null) && (Start_Over == false) && (Need_King == false))
            {
                Anim.SetBool("Start", true);
                if (PhotonNetwork.IsMasterClient)
                {
                    Octopus_Obj.transform.position = Octopus_transform.transform.position;
                    Octopus_Obj.transform.rotation = Octopus_transform.transform.rotation;
                }
                Invoke("Anim_Over", Anim_Time);
                //Anim_Over();
            }
            else if ((Anim != null) && (Start_Over == true))
            {
                Create_or_Destroy(true);
                Trigger_Over = true;
            }
            if (Octopus_Decode == false)
            {
                text.text = "成功解鎖";
            }
        }
        else if (Trigger == false)
        {
            //Create_or_Destroy(false);
            text.text = time.ToString("#0.0") + "秒";
        }
        if ((time <= 0f)&&(Trigger_Over==false)&&(Trigger == false))
        {
            Collider.enabled = false;
            Trigger_True(true);

        }
        else if(CountDown == true)
        {
            time -= Time.deltaTime;
            Decode_Anim.SetBool("Decode", true);
            Decode_Light.SetActive(true);

        }
        else if (CountDown == false)
        {
            Decode_Anim.SetBool("Decode", false);
            Decode_Light.SetActive(false);
        }
    }
    public void Create_or_Destroy(bool Create)
    {
        if (Create)
        {
            for (int i = 0; i < Create_Obj.Length; i++)
            {
                if (Create_Obj[i].tag == "Door")
                {
                    Create_Obj[i].GetComponent<OpenDoor>().Decode = true;
                }
                else if (Create_Obj[i].tag == "Animal")
                {
                    Create_Obj[i].GetComponent<Animal>().Decode = true;
                }
                else if ((PhotonNetwork.IsMasterClient) && (Create_Obj[i].tag == "Enemy"))
                {
                    Create_Obj[i].GetComponent<AI_Monster>().Decipher_Status(Team);
                    Create_Obj[i].GetComponent<AI_Monster>().Player_ViewID = Player_ViewID;
                }
            }
        }
        else
        {
            for (int i = 0; i < Destroy_Obj.Length; i++)
            {
                if (Create_Obj[i].tag == "Door")
                {
                    Create_Obj[i].GetComponent<OpenDoor>().Decode = false;
                }
                else if (Create_Obj[i].tag == "Animal")
                {
                    Create_Obj[i].GetComponent<Animal>().Decode = false;
                    if (Patrol)
                    {
                        Create_Obj[i].GetComponent<Animal>().Patrol_Set(true);
                    }
                }
            }
        }

    }
    [PunRPC]
    void RPC_CountDown(bool Open,float UploadCount)
    {
        if (Open)
        {
            CountDown = true;
            time = UploadCount;
        }
        else
        {
            CountDown = false;
            time = UploadCount;
        }
    }
}
