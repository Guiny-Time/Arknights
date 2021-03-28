using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour{

    public void LoadLevel(){       
        SceneManager.LoadScene("Level1"); 
    }
    //退出
    public void Exit(){
        Application.Quit();
    }
}
