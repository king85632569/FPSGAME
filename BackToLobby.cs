using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class BackToLobby : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Disconnect()
    {
        StartCoroutine(I_Disconnect());
    }

    private IEnumerator I_Disconnect()
    {
        PhotonNetwork.Disconnect();

        while (PhotonNetwork.IsConnected) // wait to fully be disconnected!
            yield return null;

        Destroy(RoomManager.Instance.gameObject); // Destroy the room manager because there is already one in the main menu
        SceneManager.LoadScene(0);
    }
}
