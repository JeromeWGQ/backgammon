using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{

    public int currentIndex;
    private GameController GC;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gcgo = GameObject.Find("GameController");
        GC = gcgo.GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // ���������������ʱ���ô˺���
    void OnMouseDown()
    {
        GC.ReceiveMouseDown(currentIndex);
    }
}
