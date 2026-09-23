using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum Direct
    {
        East,West,South,North
    }
public class Door : MonoBehaviour
{
    [SerializeField] private bool isOpen = false;
    
    
    public Direct direction;
    public bool connected;

    public void SetOpen(bool open)
    {
        isOpen = open;
        GetComponent<TilemapRenderer>().enabled = !open;

        TilemapCollider2D col = GetComponent<TilemapCollider2D>();
        if (col != null) col.enabled = !open;



    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Tilemap>().CompressBounds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
