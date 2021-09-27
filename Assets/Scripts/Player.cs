using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{

    public int myPlayerID;               //GameManager.csで設定

    public int currentPosX;
    public int currentPosY;
    public int oldPosX;
    public int oldPosY;
    //m_bord[currentPosX][currentPosY] = playerID という状態を保っておく

    public int myGoalLine;
    public int[][] myGoal;

    public bool myTurn = false;
    public bool selectPlayer;
    public bool selectWall;

    public int bordSize;
    public float unitLength;
    public int wallStock;

    public GameObject field;
    public GameObject[] canMovePanel;
    public int cmpNumber = 0;

    public GameManager gm;
    public Field fieldScript;

    private GameObject[] myWall;
    private int wallNum;
    private Vector3 tmpWallPos;
    public bool wallIsVertical;
    public bool wallIsHrizontal;

    private GameObject player;

    private GameObject UI;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        field = GameObject.Find("Field");
        fieldScript = field.GetComponent<Field>();

        wallIsHrizontal = true;
        wallIsVertical = false;

        myWall = new GameObject[wallStock];

        canMovePanel = new GameObject[20];

        player = transform.GetChild(0).gameObject;

        int i = 0;
        foreach (Transform wallchild in transform.GetChild(1).gameObject.transform)
        {
            myWall[i] = wallchild.gameObject;
            i++;
        }

        tmpWallPos = myWall[0].transform.position;
        wallNum = 0;

        SetMyGoal(myPlayerID);
    }


    // Update is called once per frame
    void Update()
    {
        CheckMyTurn();

        if (myTurn)
        {
            SelectAction();

            if (selectPlayer)
            {
                PlayerMove();
            }

            if (selectWall)
            {
                SetWall();
            }
        }



    }

    private void CheckMyTurn()
    {
        if (gm.playerTurn == myPlayerID && !myTurn)
        {
            myTurn = true;
            InitializeTurn();
        }
        else if(gm.playerTurn != myPlayerID)
        {
            myTurn = false;
        }
    }

    private void InitializeTurn()
    {
        selectPlayer = false;
        selectWall = false;

        oldPosX = currentPosX;
        oldPosY = currentPosY;

        //壁の初期位置保存
        tmpWallPos = myWall[wallNum].transform.position;

    }
        
    

    private void SelectAction()
    {

        if (Input.GetKeyDown(KeyCode.P) && !selectPlayer)
        {
            selectPlayer = true;
            selectWall = false;
            myWall[wallNum].transform.position = tmpWallPos;
            myWall[wallNum].transform.eulerAngles = new Vector3(0, 0, 0);
            //プレイヤの移動可能場所確認
            CheckAround();
            myWall[wallNum].GetComponent<SpriteRenderer>().color = Color.black;
        }

        if (Input.GetKeyDown(KeyCode.W) && !selectWall)
        {
            selectWall = true;
            selectPlayer = false;
            tmpWallPos = myWall[wallNum].transform.position;
            myWall[wallNum].GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 127);
            ResetCanMovePanel();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            selectPlayer = false;
            selectWall = false;
            myWall[wallNum].transform.position = tmpWallPos;
            myWall[wallNum].transform.eulerAngles = new Vector3(0, 0, 0);
            ResetCanMovePanel();
            myWall[wallNum].GetComponent<SpriteRenderer>().color = Color.black;
        }
    }

    private void PlayerMove()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CreateRay(currentPosX + 1, currentPosY);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CreateRay(currentPosX, currentPosY - 1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CreateRay(currentPosX - 1, currentPosY);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CreateRay(currentPosX, currentPosY + 1);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CreateRay(mousePos.x / unitLength, mousePos.y / unitLength);
        }

    }

    private void CreateRay(float posX, float posY)
    {
        int panelPosX;
        int panelPosY;

        Vector3 pos = new Vector3(posX * unitLength, posY * unitLength, 0f);
        int layerMask = LayerMask.GetMask(new string[] { "panel" });
        Ray2D ray = new Ray2D(pos, new Vector3(0, 0, 1));

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 10.0f, layerMask);
        if (hit.collider != null)
        {
            panelPosX = Mathf.FloorToInt(hit.collider.gameObject.transform.position.x / unitLength);
            panelPosY = Mathf.FloorToInt(hit.collider.gameObject.transform.position.y / unitLength);
            currentPosX = panelPosX;
            currentPosY = panelPosY;
            gm.LordData(0, currentPosX, currentPosY, oldPosX, oldPosY, myPlayerID);
            finalizeTurn();
        }
    }

    private void finalizeTurn()
    {
        player.transform.position = new Vector3(currentPosX * unitLength, currentPosY * unitLength, 0);
        ResetCanMovePanel();
    }    

    private void SetWall()
    {
        int nearestPointX;
        int nearestPointY;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        mousePos.x = Mathf.Clamp(mousePos.x, 0, (bordSize - 2) * unitLength);
        mousePos.y = Mathf.Clamp(mousePos.y, 0, (bordSize - 2) * unitLength);

        (nearestPointX, nearestPointY, myWall[wallNum].transform.position) = GetNearestPoint(mousePos);

        if (Input.GetKeyDown(KeyCode.V))
        {
            wallIsVertical  = true;
            wallIsHrizontal = false;
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            wallIsHrizontal = true;
            wallIsVertical  = false;
        }

        if (wallIsHrizontal)
        {
            myWall[wallNum].transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (wallIsVertical)
        {
            myWall[wallNum].transform.eulerAngles = new Vector3(0, 0, 90.0f);
        }

        if (Input.GetMouseButtonDown(0))
        {

            if (wallIsHrizontal)
            {

                if (fieldScript.horizontalGrid[nearestPointX    ][nearestPointY + 1] ||
                    fieldScript.horizontalGrid[nearestPointX + 1][nearestPointY + 1] ||
                    fieldScript.choicedPoint[nearestPointX][nearestPointY])
                {
                    return;
                }
                gm.LordData(1, nearestPointX, nearestPointY, 0, 0, myPlayerID);
                finalizeTurn();
            }
            if (wallIsVertical)
            {
                if (fieldScript.verticalGrid[nearestPointX + 1][nearestPointY    ] ||
                    fieldScript.verticalGrid[nearestPointX + 1][nearestPointY + 1] ||
                    fieldScript.choicedPoint[nearestPointX][nearestPointY])
                {
                    return;
                }
                gm.LordData(2, nearestPointX, nearestPointY, 0, 0, myPlayerID);
                finalizeTurn();
            }
            myWall[wallNum].GetComponent<SpriteRenderer>().color = Color.black;
            wallNum++;
        }
    }

    private (int pointX, int pointY, Vector3 pos) GetNearestPoint(Vector3 mPos)
    {        
        int nearPointX = 0, nearPointY = 0;
        Vector3 nearPos = new Vector3(0, 0, 0);

        nearPointX = Mathf.FloorToInt(mPos.x / unitLength);
        nearPointY = Mathf.FloorToInt(mPos.y/ unitLength);
        nearPos = new Vector3((nearPointX + 0.5f) * unitLength, (nearPointY + 0.5f) * unitLength, 0);

        return (nearPointX, nearPointY, nearPos);
    }

    private void SetMyGoal(int id)
    {
        if (id == 1)
        {
            myGoalLine = bordSize - 1;
        }
        if (id == 2)
        {
            myGoalLine = 0;
        }
        if (id == 3)
        {
            myGoalLine = 0;
        }
        if (id == 4)
        {
            myGoalLine = bordSize - 1;
        }
    }
    


    public void GoalCheck(int id)
    {
        if (id == 1 || id == 2)
        {
            if (currentPosX == myGoalLine)
            {
                gm.Goal(id);
                Debug.Log("Player" + id + " Win!!");
            }
        }
        if (id == 3 || id == 4)
        {
            if (currentPosY == myGoalLine)
            {
                gm.Goal(id);
                Debug.Log("Player" + id + " Win!!");
            }
        }
    }

    private void CheckAround()
    {
        for (int i = 0; i < 4; i++)
        {
            WallCheck(currentPosX, currentPosY, i, false);
        }
    }

    public void WallCheck(int posX, int posY, int dir, bool second)
    {
        switch (dir)
        {
            case 0:
                if (fieldScript.verticalGrid[posX + 1][posY])
                {
                    if (second) return;
                    WallCheck(posX, posY, 1, true);
                    WallCheck(posX, posY, 3, true);
                }
                else
                {
                    PlayerCheck(posX, posY, 0);
                }
                break;
            case 1:
                if (fieldScript.horizontalGrid[posX][posY])
                {
                    if (second) return;
                    WallCheck(posX, posY, 0, true);
                    WallCheck(posX, posY, 2, true);
                }
                else
                {
                    PlayerCheck(posX, posY, 1);
                }
                break;
            case 2:
                if (fieldScript.verticalGrid[posX][posY])
                {
                    if (second) return;
                    WallCheck(posX, posY, 1, true);
                    WallCheck(posX, posY, 3, true);
                }
                else
                {
                    PlayerCheck(posX, posY, 2);
                }
                break;
            case 3:
                if (fieldScript.horizontalGrid[posX][posY + 1])
                {
                    if (second) return;
                    WallCheck(posX, posY, 0, true);
                    WallCheck(posX, posY, 2, true);
                }
                else
                {
                    PlayerCheck(posX, posY, 3);
                }
                break;
        }

        
    }

    public void PlayerCheck(int posX, int posY, int dir)
    {
        int canMovePosX = 0;
        int canMovePosY = 0;
        switch (dir)
        {
            case 0:
                if (fieldScript.board[posX + 1][posY] != 0)
                {
                    WallCheck(posX + 1, posY, 0, false);
                    return;
                }
                else
                {
                    canMovePosX = posX + 1;
                    canMovePosY = posY;
                }
                break;
            case 1:
                if (fieldScript.board[posX][posY - 1] != 0)
                {
                    WallCheck(posX, posY - 1, 1, false);
                    return;
                }
                else
                {
                    canMovePosX = posX;
                    canMovePosY = posY - 1;
                }
                break;
            case 2:
                if (fieldScript.board[posX - 1][posY] != 0)
                {
                    WallCheck(posX - 1, posY, 2, false);
                    return;
                }
                else
                {
                    canMovePosX = posX - 1;
                    canMovePosY = posY;
                }
                break;
            case 3:
                if(fieldScript.board[posX][posY + 1] != 0)
                {
                    WallCheck(posX, posY + 1, 3, false);
                    return;
                }
                else
                {
                    canMovePosX = posX;
                    canMovePosY = posY + 1;
                }
                break;
        }
        SetCanMovePanel(canMovePosX, canMovePosY);
    }

    public void SetCanMovePanel(int posX, int posY)
    {
        canMovePanel[cmpNumber] = field.transform.GetChild(posX * bordSize + posY).gameObject;
        canMovePanel[cmpNumber].GetComponent<BoxCollider2D>().enabled = true;
        canMovePanel[cmpNumber].GetComponent<SpriteRenderer>().color = new Color32(171, 102, 21, 124);
        cmpNumber++;
    }

    public void ResetCanMovePanel()
    {
        for (int i = 0; i < cmpNumber; i++)
        {
            canMovePanel[i].GetComponent<BoxCollider2D>().enabled = false;
            canMovePanel[i].GetComponent<SpriteRenderer>().color = new Color32(171, 102, 21, 255);
        }
        cmpNumber = 0;
    }
        
}
