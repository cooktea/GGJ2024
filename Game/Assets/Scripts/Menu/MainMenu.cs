using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject stageMenu;

    int stages;
    // Start is called before the first frame update
    void Start()
    {
        stages = SceneManager.sceneCount;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectStage()
    {
        mainMenu.SetActive(false);
        stageMenu.SetActive(true);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void EnterScene(int scene)
    {
        if (scene <= stages)
        {
            SceneManager.LoadScene(scene);
        }
        else
        {
            Debug.Log($"No such scene! index:{scene}");
        }
    }

    public void ExitStageSelect()
    {
        mainMenu.SetActive(true);
        stageMenu.SetActive(false);
    }
}
