using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;

    [Header("Set in Inspector")]
    [SerializeField] private TextMeshProUGUI _uitLevel = null;
    [SerializeField] private TextMeshProUGUI _uitShots = null;
    [SerializeField] private TextMeshProUGUI _uitButton = null;
    [SerializeField] private GameObject[] _castles = null;
    public Vector3 CastlePos;

    [Header("Set Dynamically")]
    public int Level;
    public int LevelMax;
    public int ShotsTaken;
    public GameObject Castle;
    public GameMode Mode = GameMode.idle;
    public string Showing = "Show Slingshot";

    private void Start()
    {
        S = this;

        Level = 0;
        LevelMax = _castles.Length;
        StartLevel();
    }

    private void StartLevel()
    {
        //Уничтожить существующий замок
        if (Castle != null)
        {
            Destroy(Castle);
        }

        //Уничтожить предыдущие снаряды, если есть
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }

        //Создать новый замок
        Castle = Instantiate<GameObject>(_castles[Level]);
        Castle.transform.position = CastlePos;
        ShotsTaken = 0;

        //Переустановить камеру в начальную позицию
        SwitchView("Show Both");
        BulletLine.S.Clear();

        //Сбросить цель
        Goal.GoalMet = false;

        UpdateGUI();

        Mode = GameMode.playing;
    }

    private void UpdateGUI()
    {
        //Показать данные в элементах UI
        _uitLevel.text = "Level: " + (Level + 1) + " of " + LevelMax;
        _uitShots.text = "Shots Taken: " + ShotsTaken;
    }

    private void Update()
    {
        UpdateGUI();

        //Проверить завершение уровня
        if ((Mode == GameMode.playing) && Goal.GoalMet)
        {
            Mode = GameMode.levelEnd;
            SwitchView("Show Both");
            Invoke("NextLevel", 2f);
        }

        //Выход из игры
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    private void NextLevel()
    {
        Level++;
        if (Level == LevelMax)
        {
            Level = 0;
        }
        StartLevel();
    }

    public void SwitchView(string eView = "")
    {
        if (eView == "")
        {
            eView = _uitButton.text;
        }
        Showing = eView;
        switch (Showing)
        {
            case "Show Slingshot":
                FollowCam.POI = null;
                _uitButton.text = "Show Castle";
                break;
            case "Show Castle":
                FollowCam.POI = S.Castle;
                _uitButton.text = "Show Both";
                break;
            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                _uitButton.text = "Show Slingshot";
                break;
        }
    }

    public static void ShotFired()
    {
        S.ShotsTaken++;
    }
}
