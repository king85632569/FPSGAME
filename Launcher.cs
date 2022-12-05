using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
//using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
	public static Launcher Instance;

	[SerializeField] TMP_InputField roomNameInputField;
	[SerializeField] TMP_Text errorText;
	[SerializeField] TMP_Text roomNameText;
	[SerializeField] Transform roomListContent;
	[SerializeField] GameObject roomListItemPrefab;
	[SerializeField] Transform playerListContent;
	[SerializeField] GameObject PlayerListItemPrefab;
	[SerializeField] GameObject startGameButton;
	
	string roomNameText_Mod="模式";
	public TMP_Dropdown dropdown;
	public int mod = 1;
	public const string MOD_PROP_KEY = "mod";
	public RoomManager roomManager;
	public float StartTime=30;
	public float StartTime_Set = 60;
	public bool startGame;
	PhotonView PV;
	public bool inRoom;
	public TMP_Text CountDown;

	[SerializeField] private byte maxPlayers = 4;
	[SerializeField] private byte modCode = 1;

	[SerializeField] Player[] players;

	public void Update()
	{
		if ((PhotonNetwork.IsMasterClient)&&(inRoom))
		{
			players = PhotonNetwork.PlayerList;
			Debug.Log(players.Count());
			if ((players.Count() >= 2) && (startGame == false))
			{
				startGameButton.SetActive(false);
                if (StartTime <= 0f)
				{
					StartGame();
				}
				else
				{
					StartTime -= Time.deltaTime;
					CountDown.text = "倒數時間:" + StartTime.ToString("0") + "秒";
				}

			}
			else if(startGame == false)
			{
				startGameButton.SetActive(true);
				StartTime = StartTime_Set;
				CountDown.text = "";
			}
		}
		else if(inRoom)
		{
			startGameButton.SetActive(false);
			if (StartTime > 0f)
			{
				CountDown.text = "倒數時間:"+ StartTime.ToString("0") + "秒";
				StartTime -= Time.deltaTime;
			}
		}
	}
	[PunRPC]
	public void RPC_StartTime(float Time)
	{
		StartTime = Time;
	}

	public void RoomMod()
	{
		mod = dropdown.value;
		if (mod == 0)
		{
			modCode = 1;//模式1(1v1模式)
			maxPlayers = 2;
			roomNameText_Mod = "1v1模式";
			Debug.Log("1v1模式");
			roomManager.mod = 1;
		}
		else if (mod == 1)
		{
			modCode = 2;//模式2(2v2模式)
			maxPlayers = 4;
			roomNameText_Mod = "2v2模式";
			Debug.Log("2v2模式");
			roomManager.mod = 2;
		}
		else if (mod == 2)
		{
			modCode = 3;//模式3(3v3模式)
			maxPlayers = 6;
			roomNameText_Mod = "3v3模式";
			Debug.Log("3v3模式");
			roomManager.mod = 3;
		}
	}

	private void CreateRoom(byte modCode, byte MaxPlayers)
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = MaxPlayers; 
		roomOptions.CustomRoomPropertiesForLobby = new string[] { MOD_PROP_KEY };
		roomOptions.CustomRoomProperties = new Hashtable { { MOD_PROP_KEY, modCode } }; 
		PhotonNetwork.CreateRoom(null, roomOptions, null);
	}

	private void QuickMatch()
	{
		JoinRandomRoom(modCode, maxPlayers);
	}

	private void JoinRandomRoom(byte modCode, byte expectedMaxPlayers)
	{
		Hashtable expectedCustomRoomProperties = new Hashtable { { MOD_PROP_KEY, modCode } };
		PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers);
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		CreateRoom(modCode, maxPlayers);
	}


	void Awake()
	{
		Instance = this;
		PV = GetComponent<PhotonView>();
	}

	void Start()
	{
		Debug.Log("Connecting to Master");
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Master");
		PhotonNetwork.JoinLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public override void OnJoinedLobby()
	{
		MenuManager.Instance.OpenMenu("title");
		Debug.Log("Joined Lobby");
	}

	/*
	public void CreateRoom()
	{
		Debug.Log("創建房間");
		if (string.IsNullOrEmpty(roomNameInputField.text))
		{
			return;
		}
		PhotonNetwork.CreateRoom(roomNameInputField.text);
		MenuManager.Instance.OpenMenu("loading");
	}*/

	public override void OnJoinedRoom()
	{
		Debug.Log("加入房間");
		inRoom = true;
		startGame = false;
		startGameButton.SetActive(false);
		MenuManager.Instance.OpenMenu("room");
		roomNameText.text = PhotonNetwork.CurrentRoom.Name;
		roomNameText.text = roomNameText_Mod;

		players = PhotonNetwork.PlayerList;
		//players = PhotonNetwork.PlayerList;

		foreach (Transform child in playerListContent)
		{
			Destroy(child.gameObject);
		}

		for(int i = 0; i < players.Count(); i++)
		{
			Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
			Debug.Log("OnJoinedRoom");
			//Debug.Log(player.ID);
		}
		

	}
	/*
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}*/

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		errorText.text = "房間創建失敗: 已有相同名稱的房間" ;
		Debug.LogError("房間創建失敗: " + message);
		MenuManager.Instance.OpenMenu("error");
	}


	public void StartGame()
	{
		PhotonNetwork.LoadLevel(1);
		startGame = true;
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("loading");
	}

	public void CloseGame()
	{
		Application.Quit();
	}

	public void JoinRoom(RoomInfo info)
	{
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("loading");
	}

	public override void OnLeftRoom()
	{
		inRoom = false;
		MenuManager.Instance.OpenMenu("title");
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		foreach(Transform trans in roomListContent)
		{
			Destroy(trans.gameObject);
		}

		for(int i = 0; i < roomList.Count; i++)
		{
			if(roomList[i].RemovedFromList)
				continue;
			Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Debug.Log("OnPlayerEnteredRoom");
		Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);

		if (PhotonNetwork.IsMasterClient)
		{
			players = PhotonNetwork.PlayerList;
			if ((StartTime <= 10)||(players.Count() == maxPlayers))
			{
				StartTime = 10;
			}
			PV.RPC("RPC_StartTime", RpcTarget.AllBuffered, StartTime);
		}
	}
}