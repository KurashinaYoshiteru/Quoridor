using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour {

    public int boardSize;
    public int gridSize;

    public int[][] board;                //盤面
    public bool[][] verticalGrid;       //縦に伸びている枠
    public bool[][] horizontalGrid;     //横に伸びている枠

    public bool[][] choicedPoint;

    private void Awake()
    {
        InitializeBoard();

        InitializeGridHorizontal();
        InitializeGridVertical();

        choicedPoint = new bool[boardSize - 1][];
        for(int i = 0; i < boardSize - 1; i++)
        {
            choicedPoint[i] = new bool[boardSize - 1];
        }

        for(int i = 0; i < boardSize - 1; i++)
        {
            for(int j = 0; j < boardSize - 1; j++)
            {
                choicedPoint[i][j] = false;
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //盤面の初期化
    public void InitializeBoard()
    {
        int boardX;
        int boardY;

        board = new int[boardSize][];
        for (boardX = 0; boardX < boardSize; boardX++)
        {
            board[boardX] = new int[boardSize];
        }

        for (boardX = 0; boardX < boardSize; boardX++)
        {
            for (boardY = 0; boardY < boardSize; boardY++)
            {
                board[boardX][boardY] = 0;
            }
        }
    }

    //縦枠の初期化（bool）
    public void InitializeGridVertical()
    {
        int gridX;
        int gridY;
        bool wallFlag;

        verticalGrid = new bool[gridSize][];
        for (gridX = 0; gridX < gridSize; gridX++)
        {
            verticalGrid[gridX] = new bool[boardSize];
        }

        //左端と右端は最初から壁で塞いでおく
        for (gridX = 0; gridX < gridSize; gridX++)
        {
            for (gridY = 0; gridY < boardSize; gridY++)
            {
                wallFlag = false;

                if (gridX == 0 || gridX == gridSize - 1)
                {
                    wallFlag = true;
                }

                verticalGrid[gridX][gridY] = wallFlag;
            }
        }
    }

    //横枠の初期化（bool）
    public void InitializeGridHorizontal()
    {
        int gridX;
        int gridY;
        bool wallFlag;

        horizontalGrid = new bool[boardSize][];
        for (gridX = 0; gridX < boardSize; gridX++)
        {
            horizontalGrid[gridX] = new bool[gridSize];
        }

        //上端と下端は最初から壁で塞いでおく
        for (gridX = 0; gridX < boardSize; gridX++)
        {
            for (gridY = 0; gridY < gridSize; gridY++)
            {
                wallFlag = false;

                if (gridY == 0 || gridY == gridSize - 1)
                {
                    wallFlag = true;
                }

                horizontalGrid[gridX][gridY] = wallFlag;
            }
        }
    }
    

    public void SetPlayerPos(int posX, int posY, int oldX, int oldY, int id)
    {
        board[posX][posY] = id;
        board[oldX][oldY] = 0;
    }

    public void SetHorizontalWall(int pointX, int pointY)
    {
        horizontalGrid[pointX    ][pointY + 1] = true;
        horizontalGrid[pointX + 1][pointY + 1] = true;
    }

    public void SetVerticalWall(int pointX, int pointY)
    {
        verticalGrid[pointX + 1][pointY    ] = true;
        verticalGrid[pointX + 1][pointY + 1] = true;
    }

    public void Check()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Debug.Log(i.ToString() + j.ToString() +  verticalGrid[i][j]);
            }
        }
    }

}
