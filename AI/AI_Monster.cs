using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;


public class AI_Monster : MonoBehaviour
{
    private NavMeshAgent Navi;
    public GameObject Target, PatrolTarget,EnemyTarget;
    public float lookRadio = 50;
    public bool Find;
    public bool Dead;
    public float distance;
    public GameObject[] PatrolPosition = new GameObject[3];
    public float CDs;
    public float CDTimes;
    public float delta = 0.5f;
    public int PatrolPosition1 = 0;
    public float HP;
    public float MaxHP = 1200;
    public Slider Blood;
    public Rigidbody m_Rigidbody;
    public GameObject Hurt_vfx;
    public GameObject vfx_Attack;
    public GameObject vfx_Attack_transform;
    public float Hurt, Ch_Target_CD;
    public GameObject HurtPrefab, BigHurtPrefab;
    public Vector3 offset = new Vector3(0, 20, 0);
    public float time_des = 0;
    public Animator anim;
    public float Player_distance, HightDistance;
    public bool Monster_Team;//Ture=Red ; False=Blue
    public bool Octopus, Prisoner;
    public bool Decipher;
    public PhotonView PV;
    public GameObject[] All_Player;
    public float distance_min = 1000;
    public int id = 0;
    public bool Walk, Attack;
    public float damage;
    public bool CanSwim;
    public float SwimSpeed;
    public float Hight_Distance;
    public int Anim_Number, Anim_Number_Old;
    public int Player_ViewID, Target_Player_ViewID;
    public GameObject FollowTarget;
    public float Follow_distance;
    public float Follow_HightDistance;

    public float Enemy_Hight_Distance = 5;
    public float Enemy_distance, Enemy_HightDistance;
    float Enemydistance;
    float EnemyHightDistance;
    public bool TargetSet_ForPrisoner;
    public GameObject Target_ForPrisoner;
    public Octopus_Shot Octopus_S;



    //功能 : 平面範圍偵測 概念導向
    #region Enum
    public enum Density
    {
        Low,
        Medium,
        High
    }
    #endregion

    #region Serialize Field
    [SerializeField]
    private int Angle = 20;
    [SerializeField]
    [Range(1, 100)]
    private float Range = 5;
    [SerializeField]
    [Range(-180, 180)]
    private int StartAngle = 180;
    [SerializeField]
    private Density _rayCount = Density.Low;
    #endregion

    #region Readonly
    private readonly int Count = 30;
    #endregion

    #region Unity Method

    void Start()
    {
        PV = GetComponent<PhotonView>();
        m_Rigidbody = GetComponent<Rigidbody>();
        Find = false;
        if (CanSwim == false)
        {
            Navi = GetComponent<NavMeshAgent>();
        }
        Dead = false;
        HP = MaxHP;
        Decipher = false;
        //Ch_Target_CD = 100;
        //Blood = Slider.Instantiate(HPBar);
        //Blood.transform.SetParent(GameObject.Find("Canvas").transform, false);
    }

    void Update()
    {
        if (Dead == false)
        {
            Monster_Anim(Anim_Number);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            if ((Find == true) && (Dead == false))
            {
                //Blood.gameObject.SetActive(false);
                //Blood.gameObject.SetActive(true);
                GetPlayer();
                M_Move();
            }
            if ((Find == false) && (Dead == false) && (Octopus == false))
            {
                patrol();
                if (CanSwim == true)
                {
                    InWaterGetPlayer();
                }
                //Blood.gameObject.SetActive(false);
            }
            if ((HP < 0) && (Dead == false))
            {
                if (CanSwim == false)
                {
                    Navi.enabled = true;
                    Navi.ResetPath();
                }
                else
                {
                    SwimSpeed = 0f;
                }
                //Dead = true;
                PV.RPC("RPC_Dead", RpcTarget.AllBuffered, true);
            }
            if((Decipher == false)&&(CanSwim == false)&&(Octopus==false)&&(Prisoner==false))
            {
             
                //getRotation();
                getPlayer();
            }
            CD();
        }
        if (Dead == false)
        {
            Blood.value = HP / MaxHP;
        }
        if (Dead == true)
        {
            if (Blood != null)
            {
                Destroy(Blood.gameObject);
            }
            time_des += Time.deltaTime;
            if (time_des >= 8)
            {
                Destroy(this.gameObject);
            }
        }
        if ((Decipher == true)&&((Octopus || Prisoner)))
        {
            if (Blood == null)
                return;
            Navi.enabled = true;
            Blood.gameObject.GetComponent<Blood_Text>().Decipher = true;
            //Ture=Red ; False=Blue
            if (Monster_Team == true)
            {
                //Blood.color = Color.red;
                Blood.gameObject.GetComponent<Blood_Text>().Team = true;
            }
            else
            {
                Blood.gameObject.GetComponent<Blood_Text>().Team = false;
            }
        }
        else if ((Decipher == false) && ((Octopus || Prisoner)))
        {
            Navi.enabled = false;
            Blood.gameObject.GetComponent<Blood_Text>().Decipher = false;
        }
    }

    void Monster_Anim(int Anim_Number)
    {
        if (Anim_Number == 1)
        {
            anim.SetBool("Walk", true);
        }
        else if (Anim_Number == 2)
        {
            anim.SetBool("Walk", false);
        }
        else if (Anim_Number == 3)
        {
            anim.SetBool("Attack", true);
        }
        else if (Anim_Number == 4)
        {
            anim.SetBool("Attack", false);
        }
    }
    public void TargetPlayerViewID(int ViewID)
    {
        if((Octopus==false)&&(Prisoner==false))
        {
            //Debug.Log(TargetSet_ForPrisoner);
            if (Target_Player_ViewID != ViewID)
            {
                PV.RPC("RPC_TargetPlayerViewID", RpcTarget.AllBuffered, ViewID);
            }
            if (Target_ForPrisoner == null)
                return;
            if (Target_ForPrisoner.GetComponent<PlayerController>().EnemyTarget != this.gameObject)
            {
                PV.RPC("RPC_TargetSet", RpcTarget.AllBuffered, false);
            }
        }
    }
    [PunRPC]
    void RPC_TargetSet(bool TargetSet)
    {
        TargetSet_ForPrisoner = TargetSet;
        Debug.Log(TargetSet_ForPrisoner);
    }


    [PunRPC]
    void RPC_TargetPlayerViewID(int ViewID)
    {
        Target_Player_ViewID = ViewID;
    }

    [PunRPC]
    void RPC_Monster_Anim(int Number)
    {
        //Debug.Log("怪物動畫改變");
        Anim_Number = Number;
        Anim_Number_Old = Number;
    }

    void Anim(int Anim_Number)
    {
        if ((Anim_Number_Old != Anim_Number) && (PhotonNetwork.IsMasterClient))
        {
            PV.RPC("RPC_Monster_Anim", RpcTarget.AllBuffered, Anim_Number);
        }
        else if (Anim_Number_Old == Anim_Number)
        {
            //Debug.Log("怪物動畫無改變");
        }
    }

    void patrol()
    {
        PatrolTarget = PatrolPosition[PatrolPosition1];
        float distance = Vector3.Distance(PatrolTarget.transform.position, transform.position);
        if (CanSwim == false)
        {
            Navi.enabled = true;
            Navi.SetDestination(PatrolTarget.transform.position);
            //Navi.stoppingDistance = 1;
        }
        else
        {
            //transform.LookAt(PatrolTarget.transform.position);
            Vector3 targetDir = PatrolTarget.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, 0.3f*Time.deltaTime * SwimSpeed, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.Translate(Vector3.forward * Time.deltaTime * SwimSpeed);
        }
        if (distance > 1.5f)
        {
            Anim(1);
        }
        else if (distance <= 1.5f)
        {
            Anim(2);
            PatrolPosition1 = PatrolPosition1 + 1;
            if (PatrolPosition1 == PatrolPosition.Length)
            {
                PatrolPosition1 = 0;
            }
        }
    }


    void M_Move()
    {
        if (Target == null)
            return;
        float distance = Vector3.Distance(Target.transform.position, transform.position);
        float HightDistance = Target.transform.position.y - transform.position.y;
        if (FollowTarget != null)
        {
            Follow_distance = Vector3.Distance(FollowTarget.transform.position, transform.position);
            Follow_HightDistance = FollowTarget.transform.position.y - transform.position.y;
            EnemyTarget = FollowTarget.GetComponent<PlayerController>().EnemyTarget;
        }
        if (EnemyTarget != null)
        {
            Enemydistance = Vector3.Distance(EnemyTarget.transform.position, transform.position);
            EnemyHightDistance = EnemyTarget.transform.position.y - transform.position.y;
        }
        if (CanSwim == false)
        {
            if (Prisoner == false)
            {
                //Navi.stoppingDistance = 1;
                Navi.SetDestination(Target.transform.position);
            }
        }
        else
        {
            Vector3 targetVector = new Vector3(Target.transform.position.x, Target.transform.position.y+1f, Target.transform.position.z);
            Vector3 targetDir = targetVector - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, 0.3f * Time.deltaTime * SwimSpeed, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.Translate(Vector3.forward * Time.deltaTime * SwimSpeed);
            if ((Target.GetComponent<PlayerController>().InWater == false) || (Target.GetComponent<PlayerController>().Drowning == false))
            {
                if (Find != false)
                {
                    PV.RPC("RPC_Find", RpcTarget.AllBuffered, false);
                }
            }
        }
        if ((distance > lookRadio) || (HightDistance > Hight_Distance) || (HightDistance < -Hight_Distance))
        {
            if (!Octopus && !Prisoner) 
            {
                if (CanSwim == false)
                {
                    Navi.ResetPath();
                }
                if (Find != false)
                {
                    PV.RPC("RPC_Find", RpcTarget.AllBuffered, false);
                }
            }
        }
        if (CanSwim == true)
            return;
        if ((Prisoner == true)&&(Navi.enabled == true))
        {
            if ((EnemyTarget != null) && (Enemydistance < 3) && (EnemyHightDistance < 5) && (EnemyHightDistance > -5))
            {
                Navi.SetDestination(EnemyTarget.transform.position);
                Anim(2);
            }
            else if ((EnemyTarget != null) && (Enemydistance < 10) && (EnemyHightDistance < 5) && (EnemyHightDistance > -5))
            {
                Navi.SetDestination(EnemyTarget.transform.position);
                Anim(1);

            }
            else if ((distance <= lookRadio) && (FollowTarget != null) && (distance >= 1.5f) && (HightDistance < 5) && (HightDistance > -5))
            {
                Navi.SetDestination(Target.transform.position);
                Anim(1);
            }
            else if ((distance <= 5000) && (FollowTarget == null) && (distance >= 1.5f) && (HightDistance < 5) && (HightDistance > -5))
            {
                Navi.SetDestination(Target.transform.position);
                Anim(1);
            }
            else if ((distance <= 1.5f) && (HightDistance < 5) && (HightDistance > -5))
            {
                Navi.SetDestination(Target.transform.position);
                Anim(2);
            }
            else if((FollowTarget != null) && (Follow_distance < 2) && (Follow_HightDistance < 15) && (Follow_HightDistance > -15))
            {
                Navi.SetDestination(FollowTarget.transform.position);
                Anim(2);
            }
            else if ((FollowTarget != null) && (Follow_distance < 5000) && (Follow_HightDistance < 15) && (Follow_HightDistance > -15))
            {
                Navi.SetDestination(FollowTarget.transform.position);
                Anim(1);
            }

            if ((CDs >= CDTimes) && (Enemydistance <= 3))
            {
                if ((Prisoner) && (EnemyTarget != null))
                {
                    EnemyTarget.GetComponent<AI_Monster>().Hit_Trigger(damage, Monster_Team);
                    Anim(3);
                    CDs = 0;
                }
            }
            if ((CDs >= CDTimes) && (distance <= 3))
            {
                if ((Prisoner) && (Monster_Team != Target.GetComponent<PlayerController>().Team))
                {
                    Target.GetComponent<PlayerController>().TakeDamage_Monster(damage);
                    Anim(3);
                    CDs = 0;
                }
                else
                {
                    Target.GetComponent<PlayerController>().TakeDamage_Monster(damage);
                    Anim(3);
                    CDs = 0;
                }
                Target.GetComponent<PlayerController>().EnemyTarget = this.gameObject;
                //Instantiate(vfx_Attack, vfx_Attack_transform.transform.position, vfx_Attack_transform.transform.rotation);
            }
            else if (CDs <= CDTimes)
            {
                Anim(4);
            }
        }
        else if (Prisoner == false)
        {
            if ((distance <= Navi.stoppingDistance) && (distance >= 1.5f) && (HightDistance < 5) && (HightDistance > -5))
            {
                Anim(2);
            }
            else if ((distance <= 5000) && (distance >= 1.5f) && (HightDistance < 5) && (HightDistance > -5))
            {
                Anim(1);
            }
            if ((CDs >= CDTimes) && (distance <= 20))
            {
                if ((Octopus) && (Monster_Team != Target.GetComponent<PlayerController>().Team))
                {
                    Octopus_S.AI_Shot();
                    Anim(3);
                    CDs = 0;
                }
                else if(distance <= 3)
                {
                    Target.GetComponent<PlayerController>().TakeDamage_Monster(damage);
                    Anim(3);
                    CDs = 0;
                }
                if (distance <= 15) 
                {
                    Target.GetComponent<PlayerController>().EnemyTarget = this.gameObject;
                }
                //Instantiate(vfx_Attack, vfx_Attack_transform.transform.position, vfx_Attack_transform.transform.rotation);
            }
            else if (CDs <= CDTimes)
            {
                Anim(4);
            }
        }
    }



    public void OnTriggerEnter(Collider c)
    {
        //if ((c.CompareTag("Hurt")) && (Dead == false))
        // {
        //Instantiate(Hurt_vfx, c.transform.position, transform.rotation);
        //Hurt_Show(50);
        //}
    }


    public void Hit_Trigger(float damage, bool Team)
    {
        if (Octopus || Prisoner)
        {
            if ((Team != Monster_Team)&& (Decipher==true))
            {
                //Debug.Log("怪物受傷");
                Hurt_Show(damage);
            }
        }
        else
        {
            Hurt_Show(damage);
            if (Find != true)
            {
                PV.RPC("RPC_Find", RpcTarget.AllBuffered, true);
            }
        }
    }
    void Hurt_Show(float Hurt)
    {
        if (Hurt <= 50)
        {
            if (Octopus == false)
            {
                PV.RPC("RPC_HP", RpcTarget.AllBuffered, Hurt);
            }
            Vector3 TargetTransform = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            GameObject temp = GameObject.Instantiate(HurtPrefab);
            temp.transform.SetParent(GameObject.Find("Canvas").transform, false);
            temp.transform.position = Camera.main.WorldToScreenPoint(TargetTransform) + offset;
            temp.GetComponent<TMP_Text>().text = Hurt.ToString();

        }
        else if (Hurt > 50)
        {
            if (Octopus == false)
            {
                PV.RPC("RPC_HP", RpcTarget.AllBuffered, Hurt);
            }
            Vector3 TargetTransform = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            GameObject temp = GameObject.Instantiate(BigHurtPrefab);
            temp.transform.SetParent(GameObject.Find("Canvas").transform, false);
            temp.transform.position = Camera.main.WorldToScreenPoint(TargetTransform) + offset;
            temp.GetComponent<TMP_Text>().text = Hurt.ToString();
        }
    }

    [PunRPC]
    void RPC_Find(bool Monster_Find)
    {
        Find = Monster_Find;
    }
    [PunRPC]
    void RPC_HP(float Hurt)
    {
        HP = HP - Hurt;
    }
    [PunRPC]
    void RPC_Dead(bool Monster_Dead)
    {
        Dead = Monster_Dead;
    }

    void CD()
    {
        if (CDs < CDTimes)
        {
            CDs += Time.deltaTime;
        }
    }
    public void Decipher_Status(bool Player_Team)
    {
        if (Find != true)
        {
            PV.RPC("RPC_Find", RpcTarget.AllBuffered, true);
        }
        PV.RPC("Team", RpcTarget.AllBuffered, Player_Team);
    }

    [PunRPC]
    void Team(bool Player_Team)
    {
        Decipher = true;
        Monster_Team =Player_Team;
    }
    private void getPlayer()
    {
        All_Player = GameObject.FindGameObjectsWithTag("Move_Collider");
        distance_min = 1000;
        for (int i = 0; i < All_Player.Length; i++)
        {
            if (All_Player[i].activeSelf == true)
            {
                HightDistance = All_Player[i].transform.position.y - transform.position.y;
                Player_distance = Vector3.Distance(transform.position, All_Player[i].transform.position);
                if (Player_distance < 15)
                {
                    distance_min = Player_distance;
                    id = i;
                    if (Find != true)
                    {
                          PV.RPC("RPC_Find", RpcTarget.AllBuffered, true);
                    }
                }
            }
        }
    }
    public void InWaterGetPlayer()
    {
        All_Player = GameObject.FindGameObjectsWithTag("Move_Collider");
        distance_min = 1000;
        for (int i = 0; i < All_Player.Length; i++)
        {
            if (All_Player[i].activeSelf == true)
            {
                HightDistance = All_Player[i].transform.position.y - transform.position.y;
                Player_distance = Vector3.Distance(transform.position, All_Player[i].transform.position);
                if (Player_distance < 50)
                {
                    distance_min = Player_distance;
                    id = i;
                    if ((All_Player[id].GetComponent<PlayerController>().InWater == true) && (All_Player[id].GetComponent<PlayerController>().Drowning == true))
                    {
                        if (Find != true)
                        {
                            PV.RPC("RPC_Find", RpcTarget.AllBuffered, true);
                        }
                    }
                }
            }
        }
    }

    public void GetPlayer()
    {
        All_Player = GameObject.FindGameObjectsWithTag("Move_Collider");
        distance_min = 1000;
        //id = 0;
        //Ch_Target_CD += Time.deltaTime;
        for (int i = 0; i < All_Player.Length; i++)
        {
            if (All_Player[i].activeSelf == true)
            {
                HightDistance = All_Player[i].transform.position.y - transform.position.y;
                Player_distance = Vector3.Distance(transform.position, All_Player[i].transform.position);
               // if ((Player_distance < distance_min) && (HightDistance < 5) && (HightDistance > -2) && (Ch_Target_CD >= 5f))
                if ((Player_distance < distance_min) && (HightDistance < Hight_Distance) && (HightDistance > -Hight_Distance)&& (!Prisoner)&&(!Octopus))
                {
                    distance_min = Player_distance;
                    id = i;
                    if (CanSwim)
                    {
                        if ((All_Player[id].GetComponent<PlayerController>().InWater == true) && (All_Player[id].GetComponent<PlayerController>().Drowning == true))
                        {
                            Target = All_Player[id];
                            //Ch_Target_CD = 0;
                        }
                    }
                    else
                    {
                        Target = All_Player[id];
                        //Ch_Target_CD = 0;
                    }
                }
                else if (((Prisoner) || (Octopus))&&(All_Player[i].GetComponent<PlayerController>().Team != Monster_Team))
                {
                    if (Player_distance < distance_min)
                    {
                        distance_min = Player_distance;
                        id = i;
                        if (All_Player[id].GetComponent<PlayerController>().Team != Monster_Team)
                        {
                            Target = All_Player[id];
                            //Ch_Target_CD = 0;
                        }
                    }
                }
                if ((Prisoner) && (All_Player[i].GetComponent<PhotonView>().ViewID == Player_ViewID) && (All_Player[i].GetComponent<PlayerController>().Team == Monster_Team))
                {
                    FollowTarget = All_Player[i];
                }
                if ((All_Player[i].GetComponent<PhotonView>().ViewID == Target_Player_ViewID) &&(TargetSet_ForPrisoner==false))
                {
                    Target_ForPrisoner = All_Player[i];
                    Target_ForPrisoner.GetComponent<PlayerController>().EnemyTarget = this.gameObject;
                    PV.RPC("RPC_TargetSet", RpcTarget.AllBuffered, true);
                }
            }
        }
    }
    /// <summary>
    /// 執行射線(Ray)判斷
    /// </summary>
    /// <param name="vec"></param>
    private void OnRay(Vector3 vecm, Vector3 RayPosition)
    {
        Debug.DrawRay(RayPosition, vecm, Color.red);
    }
    #endregion
}