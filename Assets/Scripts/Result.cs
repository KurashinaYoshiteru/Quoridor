using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
    private Text winner;
    private int id = GameManager.winner;

    // Start is called before the first frame update
    void Start()
    {        
        winner = GameObject.Find("Winner").GetComponent<Text>();
        winner.text = "Player" + id + " Win!!";
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Option()
    {
        SceneManager.LoadScene("StartScene");
    }
}
