using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y < -50)
        {
            player.transform.position = new Vector3(-20, 0, 0);
            SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameOver"));
        }
        else if(player.transform.position.y > 20)
        {

            player.transform.position = new Vector3(-20, 0, 0);
            SceneManager.LoadScene("Win", LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Win"));
        }
    }
}
