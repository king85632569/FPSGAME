using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Animal : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public int RidePlayer;
    public float Collider_Height;
    public float Collider_Value_Y;
    public float AnimalSpeed;
    public bool Ride;
    public Vector3 Ride_Vector;
    public Animator Anim;
    public float Attack_CD, Attack_CD_Time;
    public bool CanAttack, Attack1_Over, Ride_Change;
    public GameObject Player;
    public float damage;

    public Collider Attack_Collider, Door_Trigger_Collider;
    public GameObject Attack_Effect;

    public int Animal_Anim_Number, Anim_Number_Old;
    public bool CanSwim,Drowning,Continue_Attack, Can_Attack;
    public bool Decode;

    public NavMeshAgent Navi;
    public int PatrolPosition1 = 0;
    public GameObject[] PatrolPosition = new GameObject[3];
    public GameObject Patrol_Outside, PatrolTarget;
    public bool Patrol;
    public float SwimSpeed;

    public float TakeDamageNerf;
    public GameObject Bubble, Pull_Pos;
    public bool Pull, PullBuff;
    public float PullBuff_CD, PullBuff_CD_Time;
    public bool Shoot;
    public GameObject Muzzle;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        Attack_CD = Attack_CD_Time;
        Attack1_Over = true;
        Ride_Change = false;
        //Decode = true;

        Attack_Collider = GetComponent<BoxCollider>();

        if (Continue_Attack)
        {
            Attack_Collider.enabled = true;
            if (Attack_Effect != null)
            {
                Attack_Effect.SetActive(true);
            }
        }
        else
        {
            Attack_Collider.enabled = false;
            if (Attack_Effect != null)
            {
                Attack_Effect.SetActive(false);
            }
        }

    }
    void InPatrol()
    {
        PatrolTarget = PatrolPosition[PatrolPosition1];
        float distance = Vector3.Distance(PatrolTarget.transform.position, transform.position);
        if (Patrol == true)
        {
            Navi.enabled = true;
            Navi.SetDestination(PatrolTarget.transform.position);
            //Navi.stoppingDistance = 1;
        }

        if (distance > 1.5f)
        {
            Animal_Anim_Number = 2;
            //Debug.Log("走路");
            //Debug.Log(distance);
        }
        else if (distance <= 1.5f)
        {
            //Debug.Log("換目標");
            Animal_Anim_Number = 1;
            PatrolPosition1 = PatrolPosition1 + 1;
            if (PatrolPosition1 == PatrolPosition.Length)
            {
                PatrolPosition1 = 0;
            }
        }
    }

    public void Animal_Anim(int Anim_Number)
    {
        if ((Anim_Number == 4) && (CanAttack == true)&&(Can_Attack == true))
        {
            if (Continue_Attack==false)
            {
                if (Shoot)
                {
                    int itemIndex = Player.GetComponent<PlayerController>().itemIndex;
                    SingleShotGun ShotGun = Player.GetComponent<PlayerController>().items[itemIndex].GetComponent<SingleShotGun>();
                    ShotGun.Ride_Shoot=true;
                    ShotGun.Ride_Muzzle = Muzzle;
                    ShotGun.Use();
                }
                else
                {
                    Animal_Attack();
                }
            }
            Anim.SetBool("Attack1", true);
            Anim.SetBool("Walk", false);
            Anim.SetBool("Run", false);
            Attack_CD = 0;
            Attack1_Over = false;
        }
        if (Attack1_Over == true)
        {
            if (Anim_Number == 1)
            {
                Anim.SetBool("Attack1", false);
                Anim.SetBool("Walk", false);
                Anim.SetBool("Run", false);
            }
            else if (Anim_Number == 2)
            {
                Anim.SetBool("Walk", true);
                Anim.SetBool("Run", false);
                Anim.SetBool("Attack1", false);
            }
            else if (Anim_Number == 3)
            {
                Anim.SetBool("Run", true);
                Anim.SetBool("Walk", false);
                Anim.SetBool("Attack1", false);
            }
        }
        //Anim_Number_Old = Anim_Number;
    }

    public void Patrol_Set(bool Patrol_State)
    {
        PV.RPC("RPC_Patrol_Set", RpcTarget.AllBuffered, Patrol_State);
    }

    [PunRPC]
    void RPC_Patrol_Set(bool Patrol_State)
    {
        Patrol = Patrol_State;
        //Navi.enabled = false;
    }

    [PunRPC]
    void RPC_Animal_Anim(int Anim_Number)
    {
        Animal_Anim_Number = Anim_Number;
        Anim_Number_Old = Anim_Number;
    }
    public void Animal_Attack()
    {
        if (Pull)
        {
            if ((PullBuff == false) && (Bubble == null))
            {
                Attack_Collider.enabled = true;
                if (Attack_Effect != null)
                {
                    Attack_Effect.SetActive(true);
                }
                Invoke("Animal_Attack_Over", 0.5f);
            }
            else if ((PullBuff == true) && (Bubble != null))
            {
                Bubble = null;
                PullBuff = false;
            }
        }
        else
        {
            Attack_Collider.enabled = true;
            if (Attack_Effect != null)
            {
                Attack_Effect.SetActive(true);
            }
            Invoke("Animal_Attack_Over", 0.5f);
        }
    }
    public void Animal_Attack_Over()
    {
        CancelInvoke("Animal_Attack_Over");
        if (Pull==false)
        {
            Bubble = null;
        }
        Attack_Collider.enabled = false;
        if (Attack_Effect != null)
        {
            Attack_Effect.SetActive(false);
        }
    }

    public void Pull_Obj() 
    {
        if (Bubble == null)
        {
            PullBuff = false;
            return;
        }
        else
        {
            if (Bubble.GetComponent<Bubble>().FlyOut == true)
            {
                Bubble = null;
            }
        }
        //if ((Pull) && (PullBuff) && (PullBuff_CD <= PullBuff_CD_Time))
        if ((Pull) && (PullBuff)&& (Bubble != null))
        {
           // PullBuff_CD += Time.deltaTime;
            /*
            Rigidbody rb = Bubble.GetComponent<Rigidbody>();
            Bubble.transform.LookAt(Pull_Pos.transform);
            rb.AddForce(Bubble.transform.forward * 5);*/
            Bubble.transform.position = Pull_Pos.transform.position;
            Debug.Log("吸住");
        }
    }

    void Update()
    {

        if ((PhotonNetwork.IsMasterClient) && (Patrol==true) && (Navi != null))
        {
            if ((Decode)&& (Navi.enabled == true))
            {
                Navi.SetDestination(Patrol_Outside.transform.position);
                float distance = Vector3.Distance(Patrol_Outside.transform.position, transform.position);
                if (distance <= 1.5f)
                {
                    Navi.ResetPath();
                    Patrol_Set(false);
                    //Patrol = false;
                }
            }
            else
            {
                InPatrol();
            }
        }
        else if((Navi!=null)&& (Patrol==false))
        {
            Navi.enabled = false;
        }

        AnimatorStateInfo stateinfo = Anim.GetCurrentAnimatorStateInfo(0);
        if (stateinfo.IsName("Attack1"))
        {
            if (stateinfo.normalizedTime < 0.7f)
            {
                //Attack_Collider.enabled = true;
            }
            else
            {
                Attack1_Over = true;
            }
        }


        if ((RidePlayer == 0)&& (Patrol==false))
        {
            Animal_Anim_Number = 1;
        }
        if (Anim_Number_Old != Animal_Anim_Number)
        {
            if (Player != null)
            {
                PhotonView Player_PV = Player.GetComponent<PlayerController>().PV;
                if (Player_PV.IsMine)
                {
                    PV.RPC("RPC_Animal_Anim", RpcTarget.AllBuffered, Animal_Anim_Number);
                    //Debug.Log("變更狀態");
                }
            }
            else if((PhotonNetwork.IsMasterClient)&&(Player == null))
            {
                PV.RPC("RPC_Animal_Anim", RpcTarget.AllBuffered, Animal_Anim_Number);
            }
        }
        else
        {
            Animal_Anim(Animal_Anim_Number);
        }


        if (Attack_CD < Attack_CD_Time)
        {
            Attack_CD += Time.deltaTime;
            CanAttack = false;
        }
        else
        {
            CanAttack = true;
        }
        if ((PV.IsMine)&&(Ride_Change==false))
        {
            //PV.RPC("SetStatus", RpcTarget.AllBuffered, Ride);
            //SetStatus(Ride);

        }
        if ((Ride == true) && (Player != null))
        {
            //Ride_Change = false;
            RidePlayer = 1;
            //Patrol = false;
            if (Door_Trigger_Collider != null)
            {
                Door_Trigger_Collider.enabled = false;
            }
            Patrol_Set(false);
            Pull_Obj();
        }
        else
        {
            if (Door_Trigger_Collider != null)
            {
                Door_Trigger_Collider.enabled = true;
            }
            RidePlayer = 0;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Play_Trigger")
        {
            if (Player == null)
                return;
            bool Team = Player.GetComponent<PlayerController>().Team;
            other.GetComponent<Collider>().gameObject.GetComponent<Hit>()?.Hit_Trigger(damage, Team,false, Player.GetComponent<PlayerController>().playerManager, false);
        }
        if (other.transform.tag == "Enemy")
        {
            bool Team = Player.GetComponent<PlayerController>().Team;
            int Player_ViewID = Player.GetComponent<PhotonView>().ViewID;
            other.GetComponent<AI_Monster>().Hit_Trigger(damage, Team);
            other.GetComponent<AI_Monster>().TargetPlayerViewID(Player_ViewID);
        }
        if ((other.gameObject.tag == "Bubble") && (PullBuff == false))
        {
            Bubble = other.gameObject;
            PullBuff = true;
            //PullBuff_CD = 0;
            PV.RPC("TriggerBubble", RpcTarget.AllBuffered);
        }
        //Ride_Change = true;
    }

    [PunRPC]
    void TriggerBubble()
    {
        if (Bubble == null)
            return;
        Rigidbody rb = Bubble.GetComponent<Rigidbody>();
        if(Pull)
        {
            Bubble.transform.LookAt(this.transform);
            rb.AddForce(Bubble.transform.forward * 5);
        }
        else
        {
            rb.AddForce(transform.forward * 60);
        }
    }


    [PunRPC]
    void SetStatus(bool Ride)
    {
        if (Ride == true)
        {
            PV.RPC("Animal_Number", RpcTarget.AllBuffered, 1);
            //Player.GetComponent<GetAllAnimal>().Target= this.gameObject;
        }
        else
        {
            PV.RPC("Animal_Number", RpcTarget.AllBuffered, 0);
        }
        Ride_Change = true;
    }
    [PunRPC]
    void Animal_Number(int _index)
    {
        RidePlayer = _index;
    }
}
