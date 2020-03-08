using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [Header("Set in Inspector")]
    [SerializeField] private GameObject _cloudSphere = null;
    public int NumSpheresMin = 6;
    public int NumSpheresMax = 10;
    public Vector3 SphereOffsetScale = new Vector3(5, 2, 1);
    public Vector2 SphereScaleRangeX = new Vector2(4, 8);
    public Vector2 SphereScaleRangeY = new Vector2(3, 4);
    public Vector2 SphereScaleRangeZ = new Vector2(2, 4);
    public float ScaleYMin = 2f;

    private List<GameObject> _spheres;

    private void Start()
    {
        _spheres = new List<GameObject>();

        int num = Random.Range(NumSpheresMin, NumSpheresMax);
        for (int i = 0; i < num; i++)
        {
            GameObject sp = Instantiate(_cloudSphere);
            _spheres.Add(sp);
            Transform spTrans = sp.transform;
            spTrans.SetParent(this.transform);

            //Выбрать случайное местоположение
            Vector3 offset = Random.insideUnitSphere;
            offset.x *= SphereOffsetScale.x;
            offset.y *= SphereOffsetScale.y;
            offset.z *= SphereOffsetScale.z;
            spTrans.localPosition = offset;

            //Выбрать случайный масштаб
            Vector3 scale = Vector3.one;
            scale.x = Random.Range(SphereScaleRangeX.x, SphereScaleRangeX.y);
            scale.y = Random.Range(SphereScaleRangeY.x, SphereScaleRangeY.y);
            scale.z = Random.Range(SphereScaleRangeZ.x, SphereScaleRangeZ.y);

            //Скорректировать масштаб y по расстоянию x от центра
            scale.y *= 1 - (Mathf.Abs(offset.x) / SphereOffsetScale.x);
            scale.y = Mathf.Max(scale.y, ScaleYMin);

            spTrans.localScale = scale;
        }
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            Restart();
        }*/
    }

    private void Restart()
    {
        //Удалить старые сферы, составляющие облако
        foreach (GameObject sp in _spheres)
        {
            Destroy(sp);
        }

        Start();
    }
}
