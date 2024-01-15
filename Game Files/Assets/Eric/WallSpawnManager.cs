using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawnManager : MonoBehaviour
{

    public GameObject spawnObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 offset = new Vector3(0, 0, 10);
            Instantiate(spawnObject, pos + offset, Quaternion.identity);
        }
    }
}
