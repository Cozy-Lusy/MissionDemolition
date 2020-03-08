using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLine : MonoBehaviour
{
    static public BulletLine S; //Одиночка

    [Header("Set in Inspector")]
    public float MinDist = 0.1f;

    private LineRenderer _line;
    private GameObject _poi;
    private List<Vector3> _points;

    private void Awake()
    {
        S = this;

        _line = GetComponent<LineRenderer>();
        _line.enabled = false;
        _points = new List<Vector3>();
    }

    public GameObject poi
    {
        get { return _poi; }
        set
        {
            _poi = value;
            //Если поле _poi содержит действительную ссылку сбросить все параметры в исходное состояние
            if (_poi != null)
            {
                _line.enabled = false;
                _points = new List<Vector3>();
                AddPoint();
            }
        }
    }

    public void Clear()
    {
        //Стираем линию
        _poi = null;
        _line.enabled = false;
        _points = new List<Vector3>();
    }

    public void AddPoint()
    {
        Vector3 pt = _poi.transform.position;

        //Если точка недостаточно далека от предыдущей, то выйти
        if (_points.Count > 0 && (pt - lastPoint).magnitude < MinDist)
        {
            return; 
        }

        if (_points.Count == 0)
        {
            Vector3 launchPosDiff = pt - Slingshot.LAUNCH_POS;
            //Добавить дополнительный фрагмент линии, чтобы лучше прицелиться в будущем
            _points.Add(pt + launchPosDiff);
            _points.Add(pt);
            _line.positionCount = 2;

            //Установить первые две точки
            _line.SetPosition(0, _points[0]);
            _line.SetPosition(1, _points[1]);
            _line.enabled = true;
        } 
        else
        {
            //Обычная последовательность добавления точки
            _points.Add(pt);
            _line.positionCount = _points.Count;
            _line.SetPosition(_points.Count - 1, lastPoint);
            _line.enabled = true;
        }
    }

    public Vector3 lastPoint
    {
        get
        {
            if (_points == null)
            {
                //Если точек нет вернуть Vector3.zero
                return Vector3.zero;
            }
            return _points[_points.Count - 1];
        }
    }

    private void FixedUpdate()
    {
        if (poi == null)
        {
            //Усли свойство poi содержит пустое значение, найти интересующий объект
            if (FollowCam.POI != null)
            {
                if (FollowCam.POI.tag == "Bullet")
                {
                    poi = FollowCam.POI;
                } else { 
                    return; 
                }
            } else { 
                return; 
            }
        }
        //Если интересующий объект не найден, попытаться добавить точку с его координатами в каждом FixedUpdate
        AddPoint();
        if (FollowCam.POI == null)
        {
            poi = null;
        }
    }
}
