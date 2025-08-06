using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject panel;
    public static bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void TogglePause()
    {
        isPaused = !panel.activeSelf;
        panel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.visible = isPaused;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (NPCController.rabbitCounter == 10)
        {
            panel.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
}
