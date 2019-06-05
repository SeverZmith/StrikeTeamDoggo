using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Hex hex = null;
    public Hexmap hexMap = null;
    float wallLife = 10.0f;
    float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= wallLife)
        {
            hex.IsHexEmpty = true;
            Destroy(this.gameObject);
            hexMap.flowField(hexMap.GetHexAtIndex(hexMap.ObjectiveXIndex, hexMap.ObjectiveYIndex));
        }
    }
}
