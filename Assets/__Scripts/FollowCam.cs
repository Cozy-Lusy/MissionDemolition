using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    static public GameObject POI; //Point of interest - интересующий объект

    [Header("Set in Inspector")]
    public float Easing = 0.05f;
    public Vector2 MinXY = Vector2.zero;

    [Header("Set Dynamically")]
    public float СamZ;

    private void Awake()
    {
        СamZ = this.transform.position.z;
    }

    private void FixedUpdate()
    {
        Vector3 destination;
        //Если нет интересующего объекта вернуть Р:[0,0,0]
        if (POI == null)
        {
            destination = Vector3.zero;
        } 
        else
        {
            //Получить позицию интересующего объекта
            destination = POI.transform.position;
            //Если интересующий объект - снаряд, убедиться, что он остановился
            if (POI.tag == "Bullet")
            {
                //Если он остановился вернуть исходные настройки поля зрения камеры
                if (POI.GetComponent<Rigidbody>().IsSleeping())
                {
                    POI = null;
                    return;
                }
            }
        }

        //Ограничить X и Y минимальными значениями
        destination.x = Mathf.Max(MinXY.x, destination.x);
        destination.y = Mathf.Max(MinXY.y, destination.y);

        destination = Vector3.Lerp(transform.position, destination, Easing);
        destination.z = СamZ; //Отодвинуть камеру подальше
        transform.position = destination;

        Camera.main.orthographicSize = destination.y + 10; //Изменить размер orthographicSize камеры чтобы земля оставалась в поле зрения
    }
}
