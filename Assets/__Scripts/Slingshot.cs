using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static private Slingshot S;

    [Header("Set In Inspector")]
    [SerializeField] private GameObject _prefabBullet = null;
    public float VeloсityMult = 8f;

    [Header("Set Dynamically")]
    public GameObject LaunchPoint;
    public GameObject Bullet;
    public Vector3 LaunchPos;
    public bool AimingMode;

    private Rigidbody _bulletRigidbody;

    static public Vector3 LAUNCH_POS
    {
        get
        {
            if (S == null) return Vector3.zero;
            return S.LaunchPos;
        }
    }

    private void Awake()
    {
        S = this;

        Transform launchPointTrans = transform.Find("LaunchPoint");
        LaunchPoint = launchPointTrans.gameObject;
        LaunchPoint.SetActive(false);
        LaunchPos = launchPointTrans.position;
    }

    private void OnMouseEnter()
    {
        //print("Slingshot: OnMouseEnter()");
        LaunchPoint.SetActive(true);
    }

    private void OnMouseExit()
    {
        //print("Slingshot: OnMouseExit()");
        LaunchPoint.SetActive(false);
    }

    private void OnMouseDown()
    {
        AimingMode = true;

        Bullet = Instantiate(_prefabBullet) as GameObject;

        Bullet.transform.position = LaunchPos; //Поместить снаряд в точку запуска

        _bulletRigidbody = Bullet.GetComponent<Rigidbody>(); //Сделать его кинематическим
        _bulletRigidbody.isKinematic = true;
    }

    private void Update()
    {
        if (!AimingMode) return; //Не выполнять если рогатка не в режиме прицеливания

        //Получить текущие экранные координаты указателя мыши
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - LaunchPos; //Найти разность координат

        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        //Если mouseDelta длиннее maxMagnitude его длина усекается до maxMagnitude
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        //Передвинуть снаряд в новую позицию
        Vector3 bulletPos = LaunchPos + mouseDelta;
        Bullet.transform.position = bulletPos;

        //Если кнопка мыши отпущена
        if (Input.GetMouseButtonUp(0))
        {
            AimingMode = false;
            _bulletRigidbody.isKinematic = false;
            _bulletRigidbody.velocity = -mouseDelta * VeloсityMult;
            FollowCam.POI = Bullet;
            MissionDemolition.ShotFired();
            BulletLine.S.poi = Bullet;
            Bullet = null;
        }
    }
}
