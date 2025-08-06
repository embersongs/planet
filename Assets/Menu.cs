using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void StartLevel()
    {
        SceneManager.LoadScene("Level1");
    }

    public void RestrartLevel()
    {
        // Получаем индекс текущей сцены и загружаем её заново
        GameController.isPaused = false;

        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        GameController.isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
}
