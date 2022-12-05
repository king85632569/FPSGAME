using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
	[SerializeField] Image healthbarImage;
	[SerializeField] GameObject ui;

	[SerializeField] public GameObject cameraHolder;

	[SerializeField] public float mouseSensitivity, sprintSpeed, smoothTime;

	public Item[] items;

	public int itemIndex;
	int previousItemIndex = -1;

	float verticalLookRotation;
	public bool grounded;
	public Vector3 smoothMoveVelocity;
	Vector3 moveAmount;

	Rigidbody rb;

	public PhotonView PV;

	public const float maxHealth = 100f;
	public float currentHealth = maxHealth;

	public PlayerManager playerManager, EnemyManager;
	public Joystick Joystick, Aim_Joystick;
	public GameObject Joystick_obj, Aim_Joystick_obj, Mobile_UI, Animal_obj, Cam_Obj, Cinemachine_Obj, Cam1, Cam2, Cam3, Aim_Button, Skill_Button1, Skill_Button2, Skill_Button_UI1, Skill_Button_UI2, Hit_Obj;
	public float walkSpeed, animalSpeed;
	public bool PC,Ride_State, Ride_Try, Run_State, Jump_State, Shoot_State, Run_Button, Aim_State, Skill_State1, Skill_State2, OpenDoor_State, FirstPerson, AI_Skill_Use1;

	public float Horizontal,Vertical, Aim_Horizontal, Aim_Vertical;
	public GetAllAnimal GetAllAnimal;

	public Animator Anim;
	public GameObject[] Character;
	public bool CharacterSetActive;
	public int Character_Number=-1;

	public bool Jump_Over, Attack1_Over, Attack2_Over;

	public bool Team;//Ture=Red ; False=Blue
	public bool Select_Over, death;
	public GameObject Enemy;
	public CharacterController CharacterController;

	public float jumpForce;
	public float gravity = 0.15f;
	public float Lock_CD_Time = 0;
	Vector3 vDir = Vector3.zero;
	public GameObject HurtPrefab, BigHurtPrefab, BloodAdditionPrefab, BigBloodAdditionPrefab;
	public bool AI_Mode,AI_Walk;
	public string PlayerName;
	private UnityEngine.AI.NavMeshAgent Navi;
	public float Temple_Time = 0;
	public float Train_Trigger_Time = 0;
	public bool Can_Train_Trigger;
	public bool InWater,InMoveObj, Drowning, InMoveCollision, GetMoveObj;
	public GameObject father_gameObject;
	public Slider Blood;
	public Image BloodImage;
	public GameObject PlayerManager_Obj;
	public PlayerManager My_PlayerManager;

	public GameObject Move_Obj, Move_Obj_Collider;
	public Vector3 Offset,Current;
	public Vector3 Hurt_Show_Offset = new Vector3(0,20,0);
	public float rota_Speed=20;
	//public Vector3 offset;
	public float rotaOffset, rotaCurrent;


	public GameObject Target, FollowTarget, EnemyTarget;
	public GameObject Aim_Target;
	public GameObject[] All_Player;
	public GameObject[] All_Enemy;
	public float distance_min = 1000;
	public float Enemy_distance_min = 1000;
	public int id = 0;
	public float Hight_Distance;
	public float Player_distance, HightDistance;
	public float Enemy_Hight_Distance;
	public float Enemy_distance, Enemy_HightDistance;
	float Follow_distance;
	float Follow_HightDistance;
	float Enemydistance;
	float EnemyHightDistance;
	public bool Dizzy, Fall_Damage, Fall;
	public bool Pray;
	public float Dizzy_Time;
	public GameObject Old_Pos;
	public float TakeDamageNerf;
	public GameObject Fly_Bubble;
	public bool AI_isGrounded;

	void Awake()
	{
		CharacterSetActive = false;
		Jump_Over = true;
		Attack1_Over = true;
		Select_Over=false;
		death = false;
		Mobile_UI = GameObject.Find("MobileCanvas");
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();
		walkSpeed = 1;
		animalSpeed = 1;
		GetAllAnimal = GetComponent<GetAllAnimal>();
		if (AI_Mode == false)
		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
		CharacterController = GetComponent<CharacterController>();
		Navi = GetComponent<UnityEngine.AI.NavMeshAgent>();

		PlayerManager_Obj = GameObject.Find("RoomManager").GetComponent<RoomManager>().PlayerManager;
		My_PlayerManager = PlayerManager_Obj.GetComponent<PlayerManager>();
		TakeDamageNerf = 1;
		//Old_Pos = GameObject.Find("PlayrControl");
	}

	void Start()
	{
		if((PV.IsMine)&&(AI_Mode == false))
		{
			//Cam_Obj.SetActive(true);
			//Cam_Obj.GetComponent<Camera>().enabled= true;
			Cinemachine_Obj.SetActive(true);
			//Hit_Obj.SetActive(false);
			Aim_Button = GameObject.FindWithTag("Aim_Button");
			Skill_Button1 = GameObject.FindWithTag("Skill_Button1");
			Skill_Button2 = GameObject.FindWithTag("Skill_Button2");
			Old_Pos = new GameObject();
		}
		else
		{
			Destroy(rb);
			Destroy(ui);
			playerManager.controller = this.gameObject;
		}
		if (AI_Mode == false)
		{
			PlayerName = PV.Owner.NickName;
			Navi.enabled = false;
		}
	}

	void Update()
	{
		Blood.value = currentHealth / maxHealth;
		if ((AI_Mode == false)&&(PV.IsMine))
		{
			healthbarImage.fillAmount = currentHealth / maxHealth;
		}
		if (My_PlayerManager.Team == Team)
		{
			//Color newColor = new Vector4(0.4f, 0.4f, 0.4f);
			BloodImage.color = Color.yellow;
			//BloodImage.color = Color.gray;
		}
		else
		{
			BloodImage.color = Color.red;
		}

		if (itemIndex != Character_Number)
		{
			itemIndex = Character_Number;
		}

		if ((Dizzy)&&(Dizzy_Time<3))
		{
			Dizzy_Time += Time.deltaTime;
		}
		else if ((Dizzy) && (Dizzy_Time >= 3))
		{
			SetDizzy(false);
		}

		if (Train_Trigger_Time < 1f)
		{
			Can_Train_Trigger = false;
			Train_Trigger_Time += Time.deltaTime;
		}
		else
		{
			Can_Train_Trigger = true;
		}

		if (Team != playerManager.GetComponent<PlayerManager>().Team)
		{
			Team = playerManager.GetComponent<PlayerManager>().Team;
		}

		if ((Character_Number>=0)&& (PV.IsMine))
		{
			if (Select_Over == false)
			{
				PV.RPC("CharacterSelect", RpcTarget.AllBuffered, Character_Number);
				PV.RPC("TeamSelect", RpcTarget.AllBuffered, Team);
				Select_Over = true;
			}
		}
		if ((((playerManager.death) || (Dizzy)) && (AI_Mode == true))&&(Navi.enabled != false))
		{
			Navi.ResetPath();
		}

		if (Dizzy)
		{
			Bubble_Fly();
			if (AI_Mode == true)
			{
				Navi.enabled = false;
			}
			else
			{
				grounded = false;
			}
		}
		else if (AI_Mode == true)
		{
			AI_Jump();
		}

		if (EnemyManager != null)
		{
			if ((playerManager.death == true)&&(death==false))
			{
				EnemyManager.kill_times_add();
				death = true;
			}
		}

		if (!CharacterSetActive)
			return;
		if ((playerManager.death) ||(Dizzy))
		{
			AnimControl();
			Shoot_State = false;
			return;
		}
		SeppdControl();

		if ((PhotonNetwork.IsMasterClient) && (AI_Mode == true))
		{
			FirstPerson = true;
			AnimControl();
			GetPlayer();
			//GetEnemy();
			//View();
			//SeppdControl();
		}
		if (!PV.IsMine)
			return;
		if (AI_Mode == false)
		{
			AnimControl();
			Move();
			//Jump();
			Look();
			View();
		}
		if (transform.position.y < -10f) // Die if you fall out of the world
		{
			//Die();
		}
	}
	void AI_Move()
	{
		Navi.stoppingDistance = 1;
		Navi.speed = walkSpeed*0.7f;
		float distance;
		float HightDistance;

		if (Target == null)
		{
			AI_Walk = false;
			Shoot_State = false;
			AI_Skill_Use1 = false;
			Navi.ResetPath();
			return;
		}
		if (FollowTarget != null)
		{
		    Follow_distance = Vector3.Distance(FollowTarget.transform.position, transform.position);
			Follow_HightDistance = FollowTarget.transform.position.y - transform.position.y;
		}
		if (EnemyTarget != null)
		{
			Enemydistance = Vector3.Distance(EnemyTarget.transform.position, transform.position);
			EnemyHightDistance = EnemyTarget.transform.position.y - transform.position.y;
		}
		distance = Vector3.Distance(Target.transform.position, transform.position);
	    HightDistance = Target.transform.position.y - transform.position.y;



		
		if ((EnemyTarget != null) && (Enemydistance < 20) && (EnemyHightDistance < 5) && (EnemyHightDistance > -5))
		{
			Lock_CD_Time += Time.deltaTime;
			if (Enemydistance < 5)
			{
				Navi.SetDestination(EnemyTarget.transform.position);
				AI_Walk = false;
				if (Lock_CD_Time >= 5f)//一秒
				{
					Lock_CD_Time = 0;
					cameraHolder.transform.LookAt(EnemyTarget.transform);
					Shoot_State = false;
				}
				else
				{
					Shoot_State = true;
				}
				if (currentHealth <= 70)
				{
					AI_Skill_Use1 = true;
				}
				else
				{
					AI_Skill_Use1 = false;
				}
			}
			else
			{
				Navi.SetDestination(EnemyTarget.transform.position);
				Shoot_State = true;
				AI_Skill_Use1 = false;
			}


		}
		else if ((EnemyTarget != null) && (Enemydistance < 30) && (EnemyHightDistance < 5) && (EnemyHightDistance > -5))
		{
			Navi.SetDestination(EnemyTarget.transform.position);
			AI_Walk = true;
			Shoot_State = false;
			AI_Skill_Use1 = false;
		}
		else if ((distance < 20) && (HightDistance < 5) && (HightDistance > -5))
		{
			Lock_CD_Time += Time.deltaTime;
			if (distance < 5)
			{
				Navi.SetDestination(Target.transform.position);
				AI_Walk = false;
				if (Lock_CD_Time >= 5f)//一秒
				{
					Lock_CD_Time = 0;
					cameraHolder.transform.LookAt(Target.transform);
					Shoot_State = false;
				}
				else
				{
					Shoot_State = true;
				}
				if (currentHealth <= 70)
				{
					AI_Skill_Use1 = true;
				}
				else
				{
					AI_Skill_Use1 = false;
				}
			}
			else
			{
				Shoot_State = true;
				AI_Skill_Use1 = false;
				Navi.SetDestination(Target.transform.position);
			}
		}
		else if ((Character_Number != 0) && (distance < 30) && (HightDistance < 5) && (HightDistance > -5))
		{
			Navi.SetDestination(Target.transform.position);
			AI_Walk = true;
			Shoot_State = false;
			AI_Skill_Use1 = false;
		}
		else if((Character_Number == 0) && (distance < 5000) && (HightDistance < 5) && (HightDistance > -5))
		{
			Navi.SetDestination(Target.transform.position);
			AI_Walk = true;
			Shoot_State = false;
			AI_Skill_Use1 = false;
		}
		else if ((FollowTarget!=null) && (Follow_distance < 2) && (Follow_HightDistance < 15) && (Follow_HightDistance > -15))
		{
			Navi.SetDestination(FollowTarget.transform.position);
			AI_Walk = false;
			Shoot_State = false;
			AI_Skill_Use1 = false;
		}
		else if ((FollowTarget != null) && (Follow_distance < 5000) && (Follow_HightDistance < 15) && (Follow_HightDistance > -15))
		{
			Navi.SetDestination(FollowTarget.transform.position);
			AI_Walk = true;
			Shoot_State = false;
			AI_Skill_Use1 = false;
		}
		else if ((FollowTarget == null) && (Character_Number != 0) && (distance < 5000) && (HightDistance < 5) && (HightDistance > -5))
		{
			Navi.SetDestination(Target.transform.position);
			AI_Walk = true;
			Shoot_State = false;
			AI_Skill_Use1 = false;
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
			if ((All_Player[i]!= null)&& (All_Player[i].GetComponent<PlayerController>().Team != Team))
			{
				HightDistance = All_Player[i].transform.position.y - transform.position.y;
				Player_distance = Vector3.Distance(transform.position, All_Player[i].transform.position);
				if ((Player_distance < distance_min) && (HightDistance < Hight_Distance) && (HightDistance > -Hight_Distance))
				{
					distance_min = Player_distance;
					id = i;
					Target = All_Player[id];
				}
			}
			if ((All_Player[i] != null) && (All_Player[i].GetComponent<PlayerController>().Character_Number == 0) && (All_Player[i].GetComponent<PlayerController>().Team == Team))
			{
				FollowTarget = All_Player[i];
			}
		}
	}
	void View()
	{
		if (Aim_Button != null) 
		{
			if (Character_Number == 4)
			{
				Aim_Button.SetActive(true);
			}
			else
			{
				Aim_Button.SetActive(false);
			}
		}

		if (Character_Number == 4)
		{
			Skill_Button_UI1.SetActive(false);
			Skill_Button_UI2.SetActive(false);
		}
		else
		{
			Skill_Button_UI1.SetActive(true);
		}
		if (Character_Number != 0)
		{
			Skill_Button_UI2.SetActive(false);
		}
		else
		{
			Skill_Button_UI2.SetActive(true);
		}



		if (Skill_Button1 != null)
		{
			if (Character_Number == 4)
			{
				Skill_Button1.SetActive(false);
				Skill_Button2.SetActive(false);
			}
			else
			{
				Skill_Button1.SetActive(true);
			}
		}
		if (Skill_Button2 != null)
		{
			if (Character_Number != 0)
			{
				Skill_Button2.SetActive(false);
			}
			else
			{
				Skill_Button2.SetActive(true);
			}
		}

		if ((Aim_State) && (Character_Number == 4))
		{
			Cam_Obj.transform.position = Cam1.transform.position;
			//Cam_Obj.GetComponent<Camera>().fieldOfView = 20;
			//Cam_Obj.GetComponent<CinemachineVirtualCamera>().Follow = Cam1.transform;
			Cinemachine_Obj.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 20;
		}
		else if (FirstPerson)
		{
			Cam_Obj.transform.position = Cam1.transform.position;
			Cinemachine_Obj.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60;
		}
		else if (!FirstPerson)
		{
			Cinemachine_Obj.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60;
			if (Ride_State)
			{
				Cam_Obj.transform.position = Cam3.transform.position;
			}
			else
			{
				Cam_Obj.transform.position = Cam2.transform.position;
			}
		}
	}

	void Bubble_Fly()
	{
		if (Fly_Bubble == null)
			return;
		AI_isGrounded = false;
		this.transform.position = Fly_Bubble.transform.position;
		//this.transform.eulerAngles = Fly_Bubble.transform.eulerAngles;
	}
	void AI_Jump()
	{
		if (playerManager.death)
			return;

		if (Navi.enabled == false)
		{
			Vector3 move = this.transform.position;
			CharacterController.Move(vDir * Time.deltaTime);
			if (AI_isGrounded == false)
			{
				move = new Vector3(move.x, move.y - 10 * Time.deltaTime, move.z);
				this.transform.position = move;
				vDir.y -= gravity;
			}
			else
			{
				Navi.enabled = true;
				if (Fall)
				{
					PV.RPC("RPC_SetFall_Damage", RpcTarget.AllBuffered, true);
					PV.RPC("RPC_SetFall", RpcTarget.AllBuffered, false);
				}
			}
		}
		else
		{
			AI_Move();
		}
	}

	void SeppdControl() 
	{
		if (Ride_State == true)
		{
			animalSpeed = GetAllAnimal.Target.GetComponent<Animal>().AnimalSpeed;
			if (GetAllAnimal.Target != null)
			{
				Drowning = GetAllAnimal.Target.GetComponent<Animal>().Drowning;
			}
		}
		else
		{
			animalSpeed = 1f;
			Drowning = true;
		}
		if (Run_State == true)
		{
			walkSpeed = 15f;
		}
		else
		{
			walkSpeed = 10f;
		}

    }

	public void Bubble_Addition()
	{
		items[itemIndex].GetComponent<SingleShotGun>().Bubble_Addition();
	}

	public void SetDizzy(bool DizzyState)
	{
		PV.RPC("RPC_SetDizzy", RpcTarget.AllBuffered, DizzyState);
	}

	public void SetFall(bool FallState)
	{
		PV.RPC("RPC_SetFall", RpcTarget.AllBuffered, FallState);
	}

	public void SetFall_Damage(bool Fall_Damage_State)
	{
		PV.RPC("RPC_SetFall_Damage", RpcTarget.AllBuffered, Fall_Damage_State);
	}

	[PunRPC]
	public void RPC_SetDizzy(bool DizzyState)
	{
		Dizzy = DizzyState;
		if (DizzyState == true)
		{
			Dizzy_Time = 0;
		}
		else
		{
			PV.RPC("RPC_SetFall", RpcTarget.AllBuffered, true);
		}
		Debug.Log(Dizzy);
	}

	[PunRPC]
	public void RPC_SetFall(bool FallState)
	{
		Fall = FallState;
		Debug.Log("Fall狀態" + Fall);
	}

	[PunRPC]
	public void RPC_SetFall_Damage(bool Fall_Damage_State)
	{
		Fall_Damage = Fall_Damage_State;
	}

	//[PunRPC]
	public void Animal_Anim(int Anim_Number)
	{
		if((GetAllAnimal.Target==null)||(GetAllAnimal.Target.GetComponent<Animal>().Patrol == true))
			return;
		GetAllAnimal.Target.GetComponent<Animal>().Animal_Anim_Number=Anim_Number;
	}
	[PunRPC]
	public void CharacterSelect(int CharacterNumber)
	{
		EquipItem(CharacterNumber);
		Character[CharacterNumber].SetActive(true);
		Anim = Character[CharacterNumber].GetComponent<Animator>();
		CharacterSetActive = true;
		items[CharacterNumber].gameObject.SetActive(true);
		Character_Number = CharacterNumber;
		playerManager.Character_Number = Character_Number;
	}
	[PunRPC]
	public void TeamSelect(bool Team_Select)
	{
		Team = Team_Select;
	}

	void AnimControl() 
	{
		AnimatorStateInfo stateinfo = Anim.GetCurrentAnimatorStateInfo(0);
		if (stateinfo.IsName("Jump"))
		{
			Anim.SetBool("Jump", false);
			Jump_Over = true;
		}
		if (stateinfo.IsName("Pray"))
		{
			Anim.SetBool("Pray", false);
			Pray = false;
		}
		if (stateinfo.IsName("Attack2"))
		{
			Anim.SetBool("Attack2", false);
		}
		if (stateinfo.IsName("Attack1"))
		{
			Anim.SetBool("Attack1", false);
			Attack1_Over = true;
		}
		if ((items[itemIndex].GetComponent<SingleShotGun>().DoubleGunnerBuff == false) && (items[itemIndex].GetComponent<SingleShotGun>().ShieldBuff == false))
		{
			Anim.SetBool("Attack2over", true);
			Attack2_Over = true;
		}

		if (playerManager.death == false)
		{
			if ((Pray)&& (grounded) && (Ride_State == false))
			{
				Anim.SetBool("Pray", true);
				Jump_Over = true;
				Attack2_Over = true;
				Attack1_Over = true;
			}
			if ((Jump_State) && (grounded) && (Ride_State == false))
			{
				Anim.SetBool("Jump", true);
				Jump_Over = false;
				Attack1_Over = true;
				Attack2_Over = true;
				Pray = false;
				//Attack2_Over = true;
				//Debug.Log("1222");
			}
			if (((items[itemIndex].GetComponent<SingleShotGun>().DoubleGunnerBuff)|| (items[itemIndex].GetComponent<SingleShotGun>().ShieldBuff))&&(Attack2_Over))
			{
				Anim.SetBool("Attack2", true);
				Anim.SetBool("Attack2over", false);
				Jump_Over = true;
				Attack1_Over = true;
				Attack2_Over = false;
				Pray = false;
			}
            if ((Shoot_State) && (Ride_State == false) && (Attack1_Over) && (items[itemIndex].GetComponent<SingleShotGun>().CanAttack == true) && (items[itemIndex].GetComponent<SingleShotGun>().BubbleObj == null))
			{
				Anim.SetBool("Attack1", true);
				items[itemIndex].Use();
				Jump_Over = true;
				Attack2_Over = true;
				Attack1_Over = false;
				Pray = false;
				//Attack2_Over = true;
			}
			if (items[itemIndex].GetComponent<SingleShotGun>().Fly_Start == true)
			{
				Anim.SetBool("Attack1", true);
				Jump_Over = true;
				Attack2_Over = true;
				Attack1_Over = false;
				Pray = false;
				items[itemIndex].GetComponent<SingleShotGun>().BubbleFly_Start(false);
			}
			if ((Ride_State == false) &&(items[itemIndex].GetComponent<SingleShotGun>().CanSkillAttack1 == true))
			{
				if (((Skill_State1) || (AI_Skill_Use1)) && (Character_Number == 0))
				{
					items[itemIndex].GetComponent<SingleShotGun>().Skill_Use1(0);
				}
				if (((Skill_State1) || (AI_Skill_Use1)) && (Character_Number == 1) && (AI_Mode == false))
				{
					items[itemIndex].GetComponent<SingleShotGun>().Skill_Use1(1);
				}
				if (((Skill_State1) || (AI_Skill_Use1)) && (Character_Number == 3))
				{
					items[itemIndex].GetComponent<SingleShotGun>().Skill_Use1(3);
				}
				if (((Skill_State1) || (AI_Skill_Use1)) && (Character_Number == 5))
				{
					items[itemIndex].GetComponent<SingleShotGun>().Skill_Use1(5);
				}
			}
			if ((Ride_State == false) && (items[itemIndex].GetComponent<SingleShotGun>().CanSkillAttack2 == true))
			{
				if ((Skill_State2) && (Character_Number == 0))
				{
					items[itemIndex].GetComponent<SingleShotGun>().Skill_Use2(0);
				}
			}
		}
		else
		{
			Jump_Over = true;
			Attack1_Over = true;
			Anim.SetBool("Pray", false);
			Anim.SetBool("Jump", false);
			Anim.SetBool("Attack1", false);
			Anim.SetBool("Attack2", false);
		}

		if ((Jump_Over)&&(Attack1_Over)&&(playerManager.death == false))
		{
			Anim.SetBool("Jump", false);
			Anim.SetBool("Attack1", false);
			if (Ride_State == true)
			{

				Anim.SetBool("Ride", true);
				Anim.SetBool("Walk", false);
				Anim.SetBool("Run", false);
				if((Horizontal == 0) && (Vertical == 0))
				{
					Animal_Anim(1);
				}
				else if (((Horizontal != 0) || (Vertical != 0)) && (Run_State == false))
				{
					Animal_Anim(2);
				}
				else if (((Horizontal != 0) || (Vertical != 0)) && (Run_State == true))
				{
					Animal_Anim(3);
				}
				if (Shoot_State == true)
				{
					Animal_Anim(4);
				}
			}
			if (Ride_State == false)
			{
				Anim.SetBool("Ride", false);
				if ((Horizontal == 0) && (Vertical == 0)&&(AI_Walk==false))
				{
					Anim.SetBool("Walk", false);
					Anim.SetBool("Run", false);
				}
				if (((Horizontal != 0) || (Vertical != 0)|| (AI_Walk == true)) && (Run_State == false))
				{
					Anim.SetBool("Walk", true);
				}
				else
				{
					Anim.SetBool("Walk", false);
				}
				if (((Horizontal != 0) || (Vertical != 0)) && (Run_State == true))
				{
					Anim.SetBool("Run", true);
				}
				else
				{
					Anim.SetBool("Run", false);
				}
			}
			else
			{
				Anim.SetBool("Walk", false);
				Anim.SetBool("Run", false);
			}
		}
		else
		{
			Anim.SetBool("Ride", false);
			Anim.SetBool("Walk", false);
			Anim.SetBool("Run", false);
		}
	}

	public void RideControl(bool RideState)
	{
		PV.RPC("Ride_State_Control", RpcTarget.AllBuffered, RideState);
	}

	[PunRPC]
    void Ride_State_Control(bool RideState)
	{
		Ride_State = RideState;
	}



	void Look()
	{

		transform.Rotate(Vector3.up * Aim_Horizontal * mouseSensitivity);
		verticalLookRotation += Aim_Vertical * mouseSensitivity;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -80f, 80f);
		cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
	}

	void Move()
	{
		if (PC == true)
		{
			Aim_Horizontal = Input.GetAxisRaw("Mouse X");
			Aim_Vertical = Input.GetAxisRaw("Mouse Y");
			Horizontal = Input.GetAxisRaw("Horizontal");
			Vertical = Input.GetAxisRaw("Vertical");
			if (Mobile_UI.activeSelf == true) 
			{
				Mobile_UI.SetActive(false);
			}
		}
		else if (PC == false)
		{
			if (Mobile_UI.activeSelf == true)
			{
				Joystick_obj = GameObject.Find("Variable Joystick");
				Joystick = Joystick_obj.GetComponent<VariableJoystick>();

				Aim_Joystick_obj = GameObject.Find("Aim Joystick");
				Aim_Joystick = Aim_Joystick_obj.GetComponent<VariableJoystick>();

				Horizontal = Joystick.Horizontal;
				Vertical = Joystick.Vertical;

				Aim_Horizontal = Aim_Joystick.Horizontal;
				Aim_Vertical = Aim_Joystick.Vertical;
			}
			else if (Mobile_UI.activeSelf == false)
			{
				Mobile_UI.SetActive(true);
			}
		}
	}


	void Jump()
	{
		

		if (InMoveObj)
		{
			Offset = Move_Obj.transform.position - Current;
			Current = Move_Obj.transform.position;
			rotaOffset = Move_Obj.transform.rotation.eulerAngles.y - rotaCurrent;
			rotaCurrent = Move_Obj.transform.rotation.eulerAngles.y;
			rota_Speed = rotaOffset;
			Old_Pos.transform.position += Offset;
			Old_Pos.transform.RotateAround(Move_Obj.transform.position, Vector3.up, rota_Speed);
			transform.position += Offset;
			transform.RotateAround(Move_Obj.transform.position, Vector3.up, rota_Speed);
			if (InMoveCollision == false)
			{
				transform.Translate(new Vector3(Horizontal, 0, Vertical) * walkSpeed * animalSpeed * Time.deltaTime);
			}
		}
		else if (items[itemIndex].GetComponent<SingleShotGun>().KingBuff2 == true)
		{
			transform.Translate(new Vector3(0, 0, 1) * 5 * walkSpeed * animalSpeed * Time.deltaTime);
		}
		else
		{
			Vector3 moveDirectionVertical = Vertical * transform.forward * walkSpeed * animalSpeed;
			Vector3 moveDirectionHorizontal = Horizontal * transform.right * walkSpeed * animalSpeed;
			CharacterController.Move((moveDirectionVertical + moveDirectionHorizontal + vDir) * Time.deltaTime);
			if ((CharacterController.isGrounded == false) || (Dizzy))
			{
				grounded = false;
			}
			else if ((CharacterController.isGrounded) && (!InWater))
			{
				grounded = true;
			}
			if ((Fall) && (grounded))
			{
				Debug.Log("grounded=" + grounded);
				PV.RPC("RPC_SetFall_Damage", RpcTarget.AllBuffered, true);
				PV.RPC("RPC_SetFall", RpcTarget.AllBuffered, false);
			}
		}
		if ((InMoveCollision)&&(Horizontal==0)&&(Vertical==0))
		{
			transform.position = Old_Pos.transform.position;
		}
		if (InWater)
		{
			if (Ride_State == true)
			{
				bool CanSwim = GetAllAnimal.Target.GetComponent<Animal>().CanSwim;
				if (CanSwim)
				{
					vDir.y -= gravity * 0.3f;
				}
				else
				{
					vDir.y = 0;
				}
			}
			if (Jump_State)
			{
				vDir.y = jumpForce;
			}
			else if (Ride_State == false)
			{
				vDir.y -= gravity * 0.3f;
			}
		}
		else
		{
			if (((grounded) && (Jump_State) || (items[itemIndex].GetComponent<SingleShotGun>().DoubleGunnerBuff))&&(InMoveObj==false))
			{
				//InMoveObj = false;
				vDir.y = jumpForce;
				grounded = false;
				Jump_State = false;
			}
			else if (!grounded)
			{
				//InMoveObj = false;
				//this.transform.SetParent(null);
				vDir.y -= gravity;
			}
		}
	}

	void EquipItem(int _index)
	{
		if(_index == previousItemIndex)
			return;

		itemIndex = _index;

		items[itemIndex].itemGameObject.SetActive(true);

		if(previousItemIndex != -1)
		{
			items[previousItemIndex].itemGameObject.SetActive(false);
		}

		previousItemIndex = itemIndex;

		if(PV.IsMine)
		{
			Hashtable hash = new Hashtable();
			hash.Add("itemIndex", itemIndex);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		}
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if(!PV.IsMine && targetPlayer == PV.Owner)
		{
			EquipItem((int)changedProps["itemIndex"]);
		}
	}
	void InWater_Set()
	{
		vDir = Vector3.zero;
		CancelInvoke("InWater_Set");
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.gameObject.layer == LayerMask.NameToLayer("Floor"))
		{
			AI_isGrounded = true;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Water")
		{
			InWater = true;
			Invoke("InWater_Set", 0.2f);
		}
		if ((other.gameObject.tag == "Train_Trigger") && (PV.IsMine) && (Can_Train_Trigger) &&(Train_Trigger_Time>=1))
		{
			Train_Trigger_Time = 0;
			PV.RPC("RPC_TakeDamage", RpcTarget.All, 30f, false);
		}
		if ((other.gameObject.tag == "Move_Obj_Collider") && (InMoveObj) && (PV.IsMine))
		{
			CancelInvoke("Set_Old_Pos");
			//Move_Obj_Collider = other.gameObject;
			InMoveCollision = true;
		}
		if ((other.gameObject.tag == "Move_Obj") && (PV.IsMine))
		{
			GetMoveObj = true;
			Move_Obj = other.gameObject;
			Current = Move_Obj.transform.position;
			rotaCurrent = Move_Obj.transform.rotation.eulerAngles.y;
			if (Old_Pos != null)
			{
				Set_Old_Pos();
			}
		}
	}
	void OnTriggerExit(Collider other)
	{
		if ((other.gameObject.tag == "Move_Obj")&& (PV.IsMine))
		{
			//this.transform.SetParent(null);
			InMoveObj = false;
			GetMoveObj = false;
		}
		if (other.gameObject.tag == "Water")
		{
			InWater = false;
		}
		if ((other.gameObject.tag == "Move_Obj_Collider") && (PV.IsMine))
		{
			InMoveCollision = false;
		}
	}
	void OnTriggerStay(Collider other)
	{
		if ((other.gameObject.tag == "Temple") && (PV.IsMine))
		{
			Temple_Time += Time.deltaTime;
			if (Temple_Time >= 3)
			{
				Temple_Time = 0;
				Pray = true;
				PV.RPC("RPC_TakeDamage", RpcTarget.All, 10f, true);
			}
		}
		if ((other.gameObject.tag == "Move_Obj") && (grounded==true) && (PV.IsMine))
		{
			InMoveObj = true;
			if ((Old_Pos != null)&&(InMoveCollision==false))
			{
				Invoke("Set_Old_Pos", 0.1f);
			}
		}

	}

	void Set_Old_Pos()
	{
		if (Old_Pos != null)
		{
			Old_Pos.transform.position = transform.position;
			CharacterController.Move(vDir* Time.deltaTime);
		}
	}


	void FixedUpdate()
	{
		if (!PV.IsMine)
			return;
		if (!CharacterSetActive)
			return;
		if ((playerManager.death)||(Dizzy))
			return;
		if((PhotonNetwork.IsMasterClient) && (AI_Mode == true))
		{
		}
		if (AI_Mode == false)
		{
			Jump();
		}

	}

	public void Hurt_Show(float Hurt, bool BloodAddition)
	{

		//Hurt = Hurt * TakeDamageNerf;
		Hurt = Mathf.Round(Hurt * TakeDamageNerf);
		if (Hurt <= 50)
		{
			Vector3 TargetTransform = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
			GameObject temp;
			if (BloodAddition)
			{
				temp = GameObject.Instantiate(BloodAdditionPrefab);
			}
			else
			{
				temp = GameObject.Instantiate(HurtPrefab);
			}
			temp.transform.SetParent(GameObject.Find("Canvas").transform, false);
			temp.transform.position = Camera.main.WorldToScreenPoint(TargetTransform) + Hurt_Show_Offset;
			temp.GetComponent< TMP_Text> ().text = Hurt.ToString();

		}
		else if (Hurt > 50)
		{
			Vector3 TargetTransform = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
			GameObject temp;
			if (BloodAddition)
			{
				temp = GameObject.Instantiate(BigBloodAdditionPrefab);
			}
			else
			{
				temp = GameObject.Instantiate(BigHurtPrefab);
			}
			temp.transform.SetParent(GameObject.Find("Canvas").transform, false);
			temp.transform.position = Camera.main.WorldToScreenPoint(TargetTransform) + Hurt_Show_Offset;
			temp.GetComponent<TMP_Text>().text = Hurt.ToString();
		}
	}

	public void TakeDamage_Monster(float damage)
	{
		EnemyManager = null; 
		PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, false);
	}

	public void TakeDamage(float damage, PlayerManager Murderer)
	{
		//Enemy = Murderer;
		EnemyManager = Murderer;
		PV.RPC("RPC_TakeDamage", RpcTarget.All, damage,false);
	}

	public void BloodAddition(float damage)
	{
		PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, true);
	}

	[PunRPC]
	void RPC_TakeDamage(float damage,bool addition)
	{
		if (addition)
		{
			currentHealth += damage;
		}
		else
		{
			damage = Mathf.Round(damage * TakeDamageNerf);
			currentHealth -= damage;
		}
		if ((currentHealth <= 0)&&(playerManager.death==false)&&(PV.IsMine))
		{
			Die();
		}
		if ((currentHealth >= 100) && (playerManager.death == false))
		{
			currentHealth = 100;
		}
	}

	void Die()
	{
		if (AI_Mode == false)
		{
			Mobile_UI.SetActive(true);
		}
		Aim_Button = null;
		if (Ride_State == true)
		{
			Animal_Anim(1);
			//GetAllAnimal.Target.GetComponent<Animal>().AnimalState(bool Ride_State)
			//GetAllAnimal.AnimalState(false);
			//GetAllAnimal.Target.GetComponent<Animal>().AnimalState(false);
			GetAllAnimal.Target.GetComponent<Animal>().Ride = false;
			GetAllAnimal.Target.GetComponent<Animal>().Ride_Change = false;
			Ride_State = false;
		}
		playerManager.Die();
	}
}