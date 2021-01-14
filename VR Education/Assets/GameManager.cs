using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    string playerName = "Professor", roomName = "UAIC 2021";
    GameObject player;

    public GameObject puzzCompletedText;
    public GameObject loadingPanel;

    public Transform cubeSpawnPoint, VRTK_cam;

    public int correctPuzzleCount = 0;

    void Start()
    {
        playerName = "Student " + Random.Range(1, 9999);

        PlayerPrefs.DeleteAll();
        Debug.Log("Connecting to Photon Network");

        ConnectToPhoton();
    }

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            player.transform.position = new Vector3(VRTK_cam.position.x, VRTK_cam.position.y - 1.36f, VRTK_cam.position.z);
            player.transform.rotation = new Quaternion(VRTK_cam.rotation.x, VRTK_cam.rotation.y, VRTK_cam.rotation.z, VRTK_cam.rotation.w);
        }
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public void SetRoomName(string name)
    {
        roomName = name;
    }

    void ConnectToPhoton()
    {
        Debug.Log("Connecting...");
        PhotonNetwork.GameVersion = "1.0"; //1
        PhotonNetwork.ConnectUsingSettings(); //2
    }

    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log("Connected to Photon!");

        PhotonNetwork.LocalPlayer.NickName = playerName;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from server for reason: " + cause.ToString());

        if(cause != DisconnectCause.DisconnectByClientLogic)
        {
            Debug.Log("Trying to reconnect...");
            ConnectToPhoton();
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room successfully!");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed for reason: " + message);
    }

    public override void OnJoinedRoom()
    {
        GameObject cube;

        player = PhotonNetwork.Instantiate("Student", VRTK_cam.position, VRTK_cam.rotation, 0);
        //player.transform.parent = VRTK_cam.transform;
        player.transform.localPosition = new Vector3(0, -1.36f, -0.3f);
        VRTK_cam.gameObject.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            cube = PhotonNetwork.Instantiate("RedCube 1", cubeSpawnPoint.position, cubeSpawnPoint.rotation, 0);
            cube.transform.name = "Cube_1";
            //cube.GetComponentInChildren<TextMeshPro>().text = "1";
            cube = PhotonNetwork.Instantiate("RedCube 3", new Vector3(cubeSpawnPoint.position.x, cubeSpawnPoint.position.y, cubeSpawnPoint.position.z-0.4f), cubeSpawnPoint.rotation, 0);
            cube.transform.name = "Cube_3";
            //cube.GetComponentInChildren<TextMeshPro>().text = "3";
            cube = PhotonNetwork.Instantiate("RedCube 4", new Vector3(cubeSpawnPoint.position.x-0.4f, cubeSpawnPoint.position.y, cubeSpawnPoint.position.z), cubeSpawnPoint.rotation, 0);
            cube.transform.name = "Cube_4";
            //cube.GetComponentInChildren<TextMeshPro>().text = "4";
            cube = PhotonNetwork.Instantiate("RedCube 2", new Vector3(cubeSpawnPoint.position.x-0.4f, cubeSpawnPoint.position.y, cubeSpawnPoint.position.z - 0.4f), cubeSpawnPoint.rotation, 0);
            cube.transform.name = "Cube_2";
            //cube.GetComponentInChildren<TextMeshPro>().text = "2";
        }
        Debug.Log("Joined room successfully: " + PhotonNetwork.CurrentRoom.Name);
        loadingPanel.SetActive(false);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room join failed for reason: " + message);

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        //PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby successfully: " + PhotonNetwork.CurrentLobby.Name); //null is default lobby
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");
        Debug.Log("Trying to Create/Join Room " + roomName);

        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), TypedLobby.Default);
    }

    public void PuzzlePieceCount(int p)
    {
        correctPuzzleCount += p;
        if (correctPuzzleCount == 4)
            puzzCompletedText.SetActive(true);
        else
            puzzCompletedText.SetActive(false);
    }
}
