using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static readonly float UNIT_LENGTH = 2.0f;          //単位長

    private int BOARD_SIZE;        //盤面の大きさ　Option.csから
    private int PLAYER_NUM;        //プレイヤ人数　Option.csから  
    private int GRID_SIZE;         //枠の大きさ = 盤面の大きさ×２
    private int ALL_WALL_STOCK;    //全ての壁の総数 = 盤面の大きさ×２＋２
    private int OWN_WALL_STOCK;    //各プレイヤの持つ壁の数 = 壁の総数 / プレイヤ人数

    private int timer;        //ゲーム時間を測るタイマー
    
    private GameObject fieldParent = null;               //盤面の親
    public GameObject panelObject = null;                //パネルオブジェクト

    private GameObject player1Parent = null;             //プレイヤ１の親 
    private GameObject player2Parent = null;             //プレイヤ２の親  
    private GameObject player3Parent = null;             //プレイヤ３の親     
    private GameObject player4Parent = null;             //プレイヤ４の親  
    public GameObject playerObject1 = null;              //プレイヤ１オブジェクト
    public GameObject playerObject2 = null;              //プレイヤ２オブジェクト
    public GameObject playerObject3 = null;              //プレイヤ３オブジェクト    
    public GameObject playerObject4 = null;              //プレイヤ４オブジェクト

    private GameObject wallParent = null;                //壁の親  
    public GameObject wallObject = null;                 //壁オブジェクト

    public int totalTurn;          //総ターン数
    public int playerTurn;         //現手番（１～）
    
    public Player[] playerInstance;    //各プレイヤーに付くPlayer.csスクリプト（インスタンス）
    public Field fieldScript;          //Field.csスクリプト

    public bool sp;             //コマの移動を選択中にtrue（selectPlayer）　　　/*プレイヤーのインスタンスから引き出す*/
    public bool sw;             //壁の設置を選択中にtrue　（selectWall）　　　　/*これらはUIで使用　　　　　　　　　　*/

    private void Awake()
    {
        //データの初期化
        BOARD_SIZE = Option.boardSize;
        PLAYER_NUM = Option.playerNum;
        GRID_SIZE = BOARD_SIZE + 1;
        ALL_WALL_STOCK = BOARD_SIZE * 2 + 2;
        OWN_WALL_STOCK = ALL_WALL_STOCK / PLAYER_NUM;

        //Field.csに盤面の大きさと枠の大きさを渡す
        fieldScript = GameObject.Find("Field").GetComponent<Field>();
        fieldScript.boardSize = BOARD_SIZE;
        fieldScript.gridSize = GRID_SIZE;
    }


    private void Start()
    {

        CreateField();              //盤面オブジェクトの生成
        CreatePlayerAndWall();      //プレイヤーオブジェクトと壁オブジェクトの生成

        //ターンの初期化
        totalTurn = 1;
        playerTurn = 1;

        //カメラの位置設定
        Camera.main.transform.position = new Vector3((BOARD_SIZE + 3) / 2.0f * UNIT_LENGTH, (BOARD_SIZE - 1) * UNIT_LENGTH / 2.0f, -10);
        Camera.main.orthographicSize = BOARD_SIZE / 9.0f * 10.0f; 
    }

    // Update is called once per frame
    void Update()
    {
        //現ターンのプレイヤーの選択状態を常に保持
        sp = playerInstance[playerTurn - 1].selectPlayer;
        sw = playerInstance[playerTurn - 1].selectWall;
    }


    //盤面オブジェクトの生成
    private void CreateField()
    {
        fieldParent = GameObject.Find("Field");
                
        GameObject panelObject;      //パネル"仮"オブジェクト
        Vector3 position;            //各パネルの位置

        //パネル生成
        int boardX;
        int boardY;
        for(boardX = 0; boardX < BOARD_SIZE; boardX++)
        {
            for(boardY = 0; boardY < BOARD_SIZE; boardY++)
            {
                position = new Vector3(boardX, boardY, 0) * UNIT_LENGTH;                                     //位置決定
                panelObject = Instantiate(this.panelObject, position, Quaternion.identity) as GameObject;    //生成
                panelObject.name = "panel(" + boardX + "," + boardY + ")";                                   //名前決定
                panelObject.transform.localScale = Vector3.one * UNIT_LENGTH;                                //大きさ決定
                panelObject.transform.parent = fieldParent.transform;                                        //Fieldオブジェクトの子に入れる
            }
        }        
    }

    private void CreatePlayerAndWall()
    {
        GameObject playerParent;               //プレイヤの"仮"親オブジェクト
        GameObject playerObject;               //プレイヤの"仮"子オブジェクト
        GameObject instanceObject;             //プレイヤの子オブジェクトを一時的に保存
        Vector3 playerPos;                     //座標上の各プレイヤの位置

        GameObject wallObject;                 //壁の"仮"子オブジェクト
        Vector3 wallStartPos;                  //最初に生成する壁の位置
        Vector3 wallPos;                       //生成する壁の位置


        playerInstance = new Player[PLAYER_NUM];     //プレイヤ１→playerInstance[0]
                                                     //プレイヤ２→playerInstance[1] …以下略        

        //各プレイヤの初期位置
        int startPosX;      
        int startPosY;

        //プレイヤ生成　　　　playerID(0～３)
        int playerID;
        for (playerID = 0; playerID < PLAYER_NUM; playerID++)
        {
            //各プレイヤ、壁の生成位置と初期座標の設定
            switch (playerID)
            {
                //プレイヤー１→左端中央
                case 0:
                    instanceObject = playerObject1;
                    playerPos = new Vector3(0, BOARD_SIZE / 2 * UNIT_LENGTH, 0);
                    startPosX = 0;
                    startPosY = BOARD_SIZE / 2;
                    
                    wallStartPos = new Vector3(-2.5f * UNIT_LENGTH, (BOARD_SIZE - 0.5f) * UNIT_LENGTH, 0);
                    break;

                //プレイヤー２→右端中央
                case 1:
                    instanceObject = playerObject2;
                    playerPos = new Vector3((BOARD_SIZE - 1) * UNIT_LENGTH, (BOARD_SIZE - 1) / 2 * UNIT_LENGTH, 0);
                    startPosX = BOARD_SIZE - 1;
                    startPosY = (BOARD_SIZE - 1) / 2;
                    
                    wallStartPos = new Vector3((BOARD_SIZE + 1.5f) * UNIT_LENGTH, (BOARD_SIZE - 0.5f) * UNIT_LENGTH, 0);
                    break;

                //プレイヤー３→上端中央
                case 2:
                    instanceObject = playerObject3;
                    playerPos = new Vector3(BOARD_SIZE / 2 * UNIT_LENGTH, (BOARD_SIZE - 1) * UNIT_LENGTH, 0);
                    startPosX = BOARD_SIZE / 2;
                    startPosY = BOARD_SIZE - 1;
                    
                    wallStartPos = new Vector3(-2.5f * UNIT_LENGTH, -0.5f * UNIT_LENGTH, 0);
                    break;

                //プレイヤー４→下端中央
                case 3:
                    instanceObject = playerObject4;
                    playerPos = new Vector3((BOARD_SIZE - 1) / 2 * UNIT_LENGTH, 0, 0);
                    startPosX = (BOARD_SIZE - 1) / 2;
                    startPosY = 0;
                    
                    wallStartPos = new Vector3((BOARD_SIZE + 1.5f) * UNIT_LENGTH, -0.5f * UNIT_LENGTH, 0);
                    break;

                //例外
                default:
                    instanceObject = null;
                    playerPos = new Vector3(0, 0, 0);
                    wallStartPos = new Vector3(0, 0, 0);
                    startPosX = 0;
                    startPosY = 0;
                    break;
            }

            //プレイヤの親オブジェクト生成
            playerParent = new GameObject();                                            /* Player(playerID+1)Parent */
            playerParent.name = "Player" + (playerID + 1) + "Parent";                   /* ┗Player(playerID+1)    */
                                                                                        /* ┗Player(playerID+1)WallParent */
                                                                                        /*    ┗Wall(playerID+1)_(wallNum)*/

            //プレイヤの子オブジェクト生成
            playerObject = Instantiate(instanceObject, playerPos, Quaternion.identity);    //生成　位置はswitchで決定されている
            playerObject.name = "Player" + (playerID + 1);                                 //名前決定
            playerObject.transform.localScale = Vector3.one * UNIT_LENGTH;                 //大きさ決定
            playerObject.transform.parent = playerParent.transform;                        //playerParentの子に入れる

            //盤面上のプレイヤの居る場所は各プレイヤのナンバーを置いとく
            fieldScript.SetPlayerPos(startPosX, startPosY, 0, 0, playerID + 1);

            //各プレイヤにPlayer.csのインスタンスを追加
            playerInstance[playerID] = playerParent.AddComponent<Player>();            //プレイヤーオブジェクトのインスタンスを初期化していく
            playerInstance[playerID].myPlayerID = playerID + 1;                        //myPlayerID = (1～4)
            playerInstance[playerID].currentPosX = startPosX;                          //playerInstance[].currentPos(XorY)を常に各々保持  
            playerInstance[playerID].currentPosY = startPosY;                          //初期位置をcurrentPosに代入

            playerInstance[playerID].unitLength = UNIT_LENGTH;                         //単位長　　　ここはPlayer.csで書くほうがいい
            playerInstance[playerID].wallStock = OWN_WALL_STOCK;                       //所持壁数　　ここもPlayer.csで書くほうがいい　UNとOWSをpublicに
            playerInstance[playerID].bordSize = BOARD_SIZE;                            //盤面サイズ　ここもPlayer.csで書くほうがいい

            //親プレイヤの子に親壁を追加
            wallParent = new GameObject();
            wallParent.name = "Player" + (playerID + 1) + "WallParent";
            wallParent.transform.parent = playerParent.transform;
            
            //親壁の下に子壁を生成
            for (int i = 0; i < OWN_WALL_STOCK; i++)
            {
                float x = 0.5f;                   //壁保持の間隔
                if (PLAYER_NUM == 2) x = 1.0f;    //プレイヤーが2人のときは盤面サイズ全部を使い、一人分を表示
                if (playerID > 1) x = -0.5f;      //プレイヤー３と４はstartPos(下)から上に向かって並べる

                wallPos = wallStartPos - new Vector3(0, UNIT_LENGTH * BOARD_SIZE * x / OWN_WALL_STOCK * i, 0);   //位置決定
                wallObject = Instantiate(this.wallObject, wallPos, this.wallObject.transform.rotation);          //生成
                wallObject.name = "Wall" + (playerID + 1) + "_" + i;                                             //名前決定
                wallObject.transform.localScale = new Vector3(UNIT_LENGTH * 1.9f, UNIT_LENGTH * 0.15f, 1);       //大きさ決定
                wallObject.transform.parent = wallParent.transform;                                              //WallParentの子に入れる
            }
        }
    }
        

    public void LordData(int choice, int x, int y, int w, int z, int id)
    {
        switch (choice)
        {
            //コマの移動を選択されたとき
            case 0:
                fieldScript.SetPlayerPos(x, y, w, z, id);
                playerInstance[id - 1].GoalCheck(id);
                break;
            //横向きで壁が置かれたとき
            case 1:
                fieldScript.SetHorizontalWall(x, y);
                fieldScript.choicedPoint[x][y] = true;
                break;
            //縦向きで壁が置かれたとき
            case 2:
                fieldScript.SetVerticalWall(x, y);
                fieldScript.choicedPoint[x][y] = true;
                break;
        }

        totalTurn++;
        playerTurn++;
        if(playerTurn > PLAYER_NUM)
        {
            playerTurn = 1;
        }
    }

    public static int winner; 
    public void Goal(int id)
    {
        winner = id;
        SceneManager.LoadScene("ResultScene");
    }
}
