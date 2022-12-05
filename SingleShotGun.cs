using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class SingleShotGun : Gun
{
	[SerializeField] Camera cam;

	public float CD, CD_Time, Recoil, stabilize_CD, stabilize_CD_Time;
	public float Skill_CD, Skill_CD_Time, Skill_CD2, Skill_CD_Time2;
	public float Buff_CD, Buff_CD_Time, Buff_CD2, Buff_CD_Time2;
	public GameObject Player;
	public bool CanAttack, CanSkillAttack1, CanSkillAttack2;
	public PhotonView PV;
	public bool BloodAddition, Buff_BloodGunner;
	public bool DoubleGunnerBuff, Buff_DoubleGunner;
	public bool KingBuff1, Buff_King1, KingBuff2, Buff_King2;
	public bool ShieldBuff, Buff_Shield, Shield_Buff;
	public float Buff_Frequency_Time, Buff_Frequency_Self;
	public Collider Shield_Collider;
	public PlayerManager playerManager;
	public bool KingGun;
	public int Player_ViewID;
	public bool BubbleShoot, BubbleState, Fly_Start;
	public bool Team;
	public GameObject BubbleObj;
	public TMP_Text Bubble_Number_UI;
	public TMP_Text Skill_CD_UI1, Skill_CD_UI2;
	public Image Skill_UI1, Skill_UI2;
	public int Bubble_Number;
	public GameObject Aim;
	public GameObject Muzzle, Ride_Muzzle;
	public GameObject[] Muzzle_Array;
	public int Muzzle_Index;
	public bool Ride_Shoot;


	#region Serialize Field
	[SerializeField]
	private int Angle = 20;
	[SerializeField]
	[Range(1, 500)]
	private float Range = 5;
	[SerializeField]
	[Range(-180, 180)]
	private int StartAngle = 180;
	#endregion

	public int Count = 30;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
		//Player_ViewID = Player.GetComponent<PhotonView>().ViewID;
		CD = CD_Time;
		stabilize_CD = stabilize_CD_Time;
		Skill_CD = Skill_CD_Time;
	    Buff_CD = Buff_CD_Time;
		Skill_CD2 = Skill_CD_Time2;
		Buff_CD2 = Buff_CD_Time2;
		playerManager=Player.GetComponent<PlayerController>().playerManager;
		Player_ViewID = playerManager.GetComponent<PhotonView>().ViewID;
		Team = Player.GetComponent<PlayerController>().Team;
		//ShieldStateChange = false;
		if (Muzzle_Array.Length == 0)
		{
			Muzzle = this.gameObject;
		}
		else
		{
			Muzzle_Change();
		}
	}

	void Update()
	{
		//Shoot();
		if (PV.IsMine)
		{
			CD_Control();
			SetBubbleState();
		}
		SetBubbleTransform();
	}
	public void Muzzle_Change()
	{
		if (Muzzle_Index == Muzzle_Array.Length-1)
		{
			Muzzle_Index = 0;
		}
		else
		{
			Muzzle_Index += 1;
		}
		Muzzle = Muzzle_Array[Muzzle_Index];
	}

	public void CD_Control()
	{
		Bubble_Number_UI.text = Bubble_Number.ToString();
		KingBuff_Frequency();
		if (CD < CD_Time)
		{
			CD += Time.deltaTime;
			CanAttack = false;
		}
		else
		{
			CanAttack = true;
		}
		if (Skill_CD < Skill_CD_Time)
		{
			if ((Skill_CD_UI1) && (Skill_UI1))
			{
				float cd = Skill_CD_Time - Skill_CD;
				Skill_CD_UI1.text = cd.ToString("#0.0");
				Skill_UI1.fillAmount = 1-(Skill_CD / Skill_CD_Time);
			}
			Skill_CD += Time.deltaTime;
			CanSkillAttack1 = false;
		}
		else
		{
			if ((Skill_CD_UI1) && (Skill_UI1))
			{
				Skill_CD_UI1.text = null;
				Skill_UI1.fillAmount = 0;
			}
			CanSkillAttack1 = true;
		}

		if (Skill_CD2 < Skill_CD_Time2)
		{
			if ((Skill_CD_UI2) && (Skill_UI2))
			{
				float cd = Skill_CD_Time2 - Skill_CD2;
				Skill_CD_UI2.text = cd.ToString("#0.0");
				Skill_UI2.fillAmount = 1-(Skill_CD2 / Skill_CD_Time2);
			}
			Skill_CD2 += Time.deltaTime;
			CanSkillAttack2 = false;
		}
		else
		{
			if ((Skill_CD_UI2) && (Skill_UI2))
			{
				Skill_CD_UI2.text = null;
				Skill_UI2.fillAmount = 0;
			}
			CanSkillAttack2 = true;
		}



		if (Buff_CD2 < Buff_CD_Time2)
		{
			Buff_CD2 += Time.deltaTime;
			if (Buff_King2)
			{
				KingBuff2 = true;
			}
		}
		else
		{
			KingBuff2 = false;
		}

	    if (Buff_CD < Buff_CD_Time)
		{
			Buff_CD += Time.deltaTime;

			if (Buff_BloodGunner)
			{
				BloodAddition = true;
			}
			if (Buff_DoubleGunner)
			{
				DoubleGunnerBuff = true;
			}

			if (Buff_King1)
			{
				KingBuff1 = true;
			}


			if (Buff_Shield)
			{
				ShieldBuff = true;
				Shield(ShieldBuff);
				Buff_Shield = false;
			}
		}
		else
		{
			BloodAddition = false;
			DoubleGunnerBuff = false;
			KingBuff1 = false;
			if (ShieldBuff == true) 
			{
				ShieldBuff = false;
				Shield(ShieldBuff);
			}
		}

		/*
		if (stabilize_CD < stabilize_CD_Time)
		{
			stabilize_CD += Time.deltaTime;
		}
		//transform.position = Vector3.Lerp(transform.position, Vector3.zero, 0.1f);
		transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 10 * Time.deltaTime);
		//Player = GameObject.Find("PlayrControl").GetComponent<ButtonControl>().Player;
		float H = Random.Range(-0.2f, 0.2f);
		float V = Random.Range(-0.1f, 0.3f);
		if (CD <= 0.05f)
		{
			Player.GetComponent<PlayerController>().Aim_Horizontal += H * 20 * Recoil * Time.deltaTime;
			Player.GetComponent<PlayerController>().Aim_Vertical += V * 20 * Recoil * Time.deltaTime;
			//transform.Translate(Vector3.forward * -Recoil * Time.deltaTime);
		}
		if (stabilize_CD <= stabilize_CD_Time)
		{
			Player.GetComponent<PlayerController>().Aim_Vertical -= V * 5 * Recoil * Time.deltaTime;
		}*/
	}

	void KingBuff_Frequency()
	{
		if ((KingBuff1)&&(playerManager.death == false))
		{
			if (Player == null)
				return;
			Buff_Frequency_Self += Time.deltaTime;
			if (Buff_Frequency_Self >= 2)
			{
				Buff_Frequency_Self = 0;
				Player.GetComponent<PlayerController>().Hurt_Show(50, true);
				Player.GetComponent<PlayerController>().BloodAddition(50);
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		if ((other.gameObject.tag == "Play_Trigger")&&(KingBuff1))
		{
			if (Player == null)
				return;
			Buff_Frequency_Time += Time.deltaTime;
			if (Buff_Frequency_Time >= 2)
			{
				Buff_Frequency_Time = 0;
				bool Team = Player.GetComponent<PlayerController>().Team;
				other.GetComponent<Collider>().gameObject.GetComponent<Hit>()?.Hit_Trigger(50, Team, KingBuff1, playerManager, false);
			}
		}
	}

	public void Skill_Use2(int Character_Number)
	{
		if ((Skill_CD2 >= Skill_CD_Time2) && (Character_Number == 0))
		{
			Skill_CD2 = 0;
			Buff_CD2 = 0;
			Buff_King2 = true;
		}
	}

	public void Skill_Use1(int Character_Number)
	{
		if ((Skill_CD >= Skill_CD_Time) && (Character_Number == 0))
		{
			Skill_CD = 0;
			Buff_CD = 0;
			Buff_King1 = true;
		}
		if ((Skill_CD >= Skill_CD_Time) && (Character_Number == 1))
		{
			Skill_CD = 0;
			Buff_CD = 0;
			Buff_DoubleGunner = true;
			Player.GetComponent<PlayerController>().SetFall(false);
		}
		if ((Skill_CD >= Skill_CD_Time) && (Character_Number == 3))
		{
			Skill_CD = 0;
			Buff_CD = 0;
			Buff_Shield = true;
		}
		if ((Skill_CD >= Skill_CD_Time)&&(Character_Number==5))
		{
			Skill_CD = 0;
			Buff_CD = 0;
			Buff_BloodGunner = true;
		}
	}


	public void SetBubbleState()
	{
		if (BubbleState != Player.GetComponent<PlayerController>().Shoot_State)
		{
			PV.RPC("RPC_BubbleState", RpcTarget.All, Player.GetComponent<PlayerController>().Shoot_State);
		}
	}
	public void SetBubbleTransform()
	{
		if ((BubbleState) && (BubbleObj != null))
		{
			if (Ride_Shoot)
			{
				BubbleObj.transform.position = Ride_Muzzle.transform.position;
			}
			else
			{
				BubbleObj.transform.position = Muzzle.transform.position;
			}
			//BubbleObj.transform.rotation = this.transform.rotation;
			BubbleObj.transform.LookAt(Aim.transform.position);
		}
	}

	public void Bubble_Addition()
	{
		PV.RPC("RPC_Bubble_Addition", RpcTarget.All);
	}
	[PunRPC]
	void RPC_Bubble_Addition()
	{
		Bubble_Number += 1;
	}


	[PunRPC]
	void RPC_Bubble()
	{
		if (PV.IsMine)
		{
			Bubble_Number-=1;
			if (Ride_Shoot)
			{
				GameObject bubbleImpactObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BubbleImpact"), Ride_Muzzle.transform.position, Ride_Muzzle.transform.rotation, 0, new object[] { PV.ViewID });
				BubbleObj = bubbleImpactObj;
				BubbleObj.gameObject.GetComponent<Bubble>().Ride_Shoot = true;
			}
			else
			{
				GameObject bubbleImpactObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BubbleImpact"), Muzzle.transform.position, Muzzle.transform.rotation, 0, new object[] { PV.ViewID });
				BubbleObj = bubbleImpactObj;
			}
			//GameObject bubbleImpactObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BubbleImpact"), this.transform.position, this.transform.rotation);
			BubbleObj.gameObject.GetComponent<Bubble>()?.Bubble_Set(((GunInfo)itemInfo).damage, Team, BloodAddition, true, Player_ViewID,false);
			//BubbleObj.gameObject.GetComponent<Bubble>().Player_ViewID = Player_ViewID;
		}
	}

	[PunRPC]
	void RPC_BubbleState(bool ShootState)
	{
		//Debug.Log(ShootState);
		BubbleState = ShootState;
		if (BubbleState == false)
		{
			if (BubbleObj != null)
			{
				BubbleObj.gameObject.GetComponent<Bubble>().Fly_Start = true;
				BubbleFly_Start(true);
			}
			BubbleObj = null;
		}
	}

	public void BubbleFly_Start(bool FlyState)
	{
		PV.RPC("RPC_Fly_Start", RpcTarget.All, FlyState);
	}

	[PunRPC]
	void RPC_Fly_Start(bool FlyState)
	{
		Fly_Start = FlyState;
	}

	public override void Use()
	{
		if ((CanAttack) && (BubbleObj == null))
		{
			Shoot();
		}
		//Debug.Log(BubbleState);
	}

	void Shield(bool ShieldBuff)
	{
		PV.RPC("RPC_Shield", RpcTarget.All, ShieldBuff);
	}

	[PunRPC]
	void RPC_Shield(bool ShieldBuff)
	{
		Shield_Collider.enabled = ShieldBuff;
	}

	void Shoot()
	{
		CD = 0;
		stabilize_CD = 0;
		if ((BloodAddition)||(KingGun))
		{
			Quaternion rot = transform.rotation;
			for (int i = 0; i < Count; i++)
			{
				Quaternion q = Quaternion.Euler(rot.x, rot.y + StartAngle + (Angle * i), rot.z);
				Vector3 newVec = q * cam.transform.forward * Range;
				OnRay(newVec);
				Vector3 RayPosition = cam.transform.position;
				Ray ray = new Ray(RayPosition, newVec);
				//RaycastHit hit;
				//bool isCollider = Physics.Raycast(ray, out hit);

				RaycastHit[] hits;
				hits = Physics.RaycastAll(ray, Range);

				for (int x = 0; x < hits.Length; x++)
				{
					RaycastHit hit = hits[x];
					if (BloodAddition)
					{
						if (hit.collider.gameObject.tag == "Play_Trigger")
						{
							PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
							hit.collider.gameObject.GetComponent<Hit>()?.Hit_Trigger(((GunInfo)itemInfo).damage, Team, BloodAddition, playerManager, false);
						}
					}
					if (KingGun)
					{
						//PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
						if (hit.collider.gameObject.tag == "Play_Trigger")
						{
							Debug.Log("國王打到敵人");
							hit.collider.gameObject.GetComponent<Hit>()?.Hit_Trigger(((GunInfo)itemInfo).damage, Team, BloodAddition, playerManager, false);

						}
						if (hit.collider.gameObject.tag == "Bubble")
						{
							Debug.Log(hit.collider.gameObject.GetComponent<Bubble>().Team);
							if ((Team != hit.collider.gameObject.GetComponent<Bubble>().Team) && (KingGun))
							{
								hit.collider.gameObject.GetComponent<Bubble>()?.Bubble_Destroy();
							}
						}
						if (hit.collider.gameObject.tag == "Enemy")
						{
							//bool Team = Player.GetComponent<PlayerController>().Team;
							hit.collider.gameObject.GetComponent<AI_Monster>().Hit_Trigger(((GunInfo)itemInfo).damage, Team);

							hit.collider.gameObject.GetComponent<AI_Monster>().TargetPlayerViewID(Player_ViewID);
							//PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
						}

					}
					/*
					if (hit.collider.gameObject.tag == "Shield")
					{
						x = hits.Length;
						PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
					}
					if (hit.collider.gameObject.tag == "Grass")
					{
						hit.collider.gameObject.GetComponent<GrassBroken>().Broken = true;
					}
					if (hit.collider.gameObject.tag == "Enemy")
					{
						bool Team = Player.GetComponent<PlayerController>().Team;
						hit.collider.gameObject.GetComponent<AI_Monster>().Hit_Trigger(((GunInfo)itemInfo).damage, Team);

						hit.collider.gameObject.GetComponent<AI_Monster>().TargetPlayerViewID(Player_ViewID);
						PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
						//Debug.Log(hit.collider.gameObject);
					}*/
				}
			}
		}
		else if ((BubbleShoot)&&(Bubble_Number>=1))
		{
			//PV.RPC("RPC_Bubble", RpcTarget.All);
			//RPC_Bubble();
			if (Muzzle_Array.Length != 0)
			{
				Muzzle_Change();
			}
			PV.RPC("RPC_Bubble", RpcTarget.All);
		}


	}

	[PunRPC]
	void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
	{
		Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
		if (colliders.Length != 0)
		{
			GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
			Destroy(bulletImpactObj, 0.3f);
			bulletImpactObj.transform.SetParent(colliders[0].transform);
		}
	}

	private void OnRay(Vector3 vec)
	{
		Debug.DrawRay(cam.transform.position, vec, Color.red);
	}

}
