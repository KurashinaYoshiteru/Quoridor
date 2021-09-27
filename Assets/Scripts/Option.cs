using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Option : MonoBehaviour
{

    private GameObject setBoardSize;
    private InputField inputField;
    public static int boardSize;

    private GameObject setPlayerNum;
    private Dropdown dropDown;
    public static int playerNum;
    

    // Start is called before the first frame update
    void Start()
    {

        setBoardSize = GameObject.Find("SetBoardSize");
        inputField = setBoardSize.GetComponent<InputField>();

        setPlayerNum = GameObject.Find("SetPlayerNum");
        dropDown = setPlayerNum.GetComponent<Dropdown>();
        
    }

    // Update is called once per frame
    void Update()
    {
        playerNum = dropDown.value + 2;
        boardSize = int.Parse(inputField.text);
    }
    
    public void OnClick()
    {
        SceneManager.LoadScene("GameScene");
    }
    
}
