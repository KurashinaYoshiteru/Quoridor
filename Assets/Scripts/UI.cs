using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private GameManager gm;
    private int playerTurn;
    private int totalTurn;

    private GameObject[] UIchild;
    private GameObject playerID;
    private GameObject turnIcon;
    private GameObject timer;
    private GameObject turnNum;

    private Text id;
    private Image icon;
    private Text select;
    private Text turn;

    private bool selectPlayer;
    private bool selectWall;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        totalTurn = gm.GetComponent<GameManager>().totalTurn;

        UIchild = new GameObject[15];

        int i = 0;
        foreach(Transform child in transform)
        {
            UIchild[i] = transform.GetChild(i).gameObject;
            i++;
        }

        playerID = UIchild[4];
        turnIcon = UIchild[5];
        timer = UIchild[9];
        turnNum = UIchild[13];

        id = transform.GetChild(4).gameObject.GetComponent<Text>();
        icon = transform.GetChild(5).GetComponent<Image>();
        select = timer.GetComponent<Text>();
        turn = turnNum.GetComponent<Text>();

    }

    //UIchild[4]  = playerID
    //UIchild[5]  = TurnIcon
    //UIchild[9]  = Timer
    //UIchild[13] = TurnNum

    private void Update()
    {
        playerTurn = gm.playerTurn;
        totalTurn = gm.totalTurn;
        selectPlayer = gm.sp;
        selectWall = gm.sw;
        
        id.text = "Player" + playerTurn.ToString();
        switch (playerTurn)
        {
            case 1:
                icon.color = Color.red;
                break;
            case 2:
                icon.color = Color.blue;
                break;
            case 3:
                icon.color = Color.green;
                break;
            case 4:
                icon.color = Color.yellow;
                break;
        }

        if (selectPlayer)
        {
            select.text = "Player";
        }
        if (selectWall)
        {
            select.text = "Wall";
        }
        if(!selectPlayer && !selectWall)
        {
            select.text = "Idle";
        }

        turn.text = totalTurn.ToString();
    }




}
