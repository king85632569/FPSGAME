using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.EventSystems;

public class Chat : MonoBehaviourPun
{
    public static Chat Instance;
    public TMP_InputField input;
    public Transform chatField;
    public GameObject chatItemPrefab;
    public GameObject Scrollbar;
    public PhotonView PV;
    bool chatCooldown;
    public bool chatSelected;
    public bool Enter;

    private void Start()
    {
        chatCooldown = false;
        Instance = this;
        //input = GameObject.FindGameObjectWithTag("ChatInput").GetComponent<TMP_InputField>();
        //chatField = GameObject.FindGameObjectWithTag("ChatField").transform;
        PV.RPC("JoinMsg", RpcTarget.All, PhotonNetwork.NickName + " 加入遊戲");
        if (Scrollbar != null)
        {
            Scrollbar.GetComponent<Scrollbar>().value = 0;
        }
    }

    private void Update()
    {
        Chatting();
    }
    public void Enter_True()
    {
        Enter = true;
    }

    void Chatting()
    {
        if (Input.GetKey(KeyCode.Return) && !EventSystem.current.currentSelectedGameObject == input)
        {
            Invoke("SelectInputField", 0.1f);
        }

        if ((Input.GetKey(KeyCode.Return))||(Enter==true))
        {
            Debug.Log("Sending Chat RPC");
            chatSelected = false;
            PV.RPC("SendMsg", RpcTarget.All, input.text, PhotonNetwork.NickName);
            input.text = null;
            Enter = false;
            if (Scrollbar != null)
            {
                Scrollbar.GetComponent<Scrollbar>().value = 0;
            }
        }
        
    }

    [PunRPC]
    IEnumerator JoinMsg(string _input)
    {
        Debug.Log("Instantiating Text");
        var chatMessage = Instantiate(chatItemPrefab, chatField);
        chatMessage.gameObject.GetComponent<TMP_Text>().text = _input;
        yield return new WaitForSeconds(0.1f);
        UpdateParentLayout(chatMessage);
        if (Scrollbar != null)
        {
            Scrollbar.GetComponent<Scrollbar>().value = 0;
        }
        yield return new WaitForSeconds(9.9f);
        Destroy(chatMessage.gameObject);
        if (Scrollbar != null)
        {
            Scrollbar.GetComponent<Scrollbar>().value = 0;
        }
    }

    [PunRPC]
    IEnumerator SendMsg(string _input, string _nickname)
    {
        if (_input != "" && !chatCooldown == true)
        {
            StartCoroutine("Cooldown");
            EventSystem.current.SetSelectedGameObject(null);
            Debug.Log("Instantiating Text");
            var chatMessage = Instantiate(chatItemPrefab, chatField);
            chatMessage.gameObject.GetComponent<TMP_Text>().text = _nickname + ": " + _input;
            yield return new WaitForSeconds(0.1f);
            UpdateParentLayout(chatMessage);
            if (Scrollbar != null)
            {
                Scrollbar.GetComponent<Scrollbar>().value = 0;
            }
            yield return new WaitForSeconds(30f);
            Destroy(chatMessage.gameObject);
            Debug.Log("Success");
            if (Scrollbar != null)
            {
                Scrollbar.GetComponent<Scrollbar>().value = 0;
            }
        }
        else
        {
            Debug.Log("Failed");
        }
    }

    
    IEnumerator Cooldown()
    {
        chatCooldown = true;
        yield return new WaitForSeconds(1f);
        chatCooldown = false;
    }

    void UpdateParentLayout(GameObject _chatMessage)
    {
        _chatMessage.gameObject.SetActive(false);
        _chatMessage.gameObject.SetActive(true);
    }

    void SelectInputField()
    {
        EventSystem.current.SetSelectedGameObject(input.gameObject, null);
        chatSelected = true;
    }
}
