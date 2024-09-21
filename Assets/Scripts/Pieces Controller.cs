using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesController : MonoBehaviour
{
    // 该数组记录所有位置的棋子数量
    // 从右下角开始数，顺时针旋转，为0~23位置
    // 右下的终点槽，放白子，为24位置
    // 右上的终点槽，放黑子，为25位置
    // 中下的被吃棋位置，放黑子，为26位置
    // 中上的被吃棋位置，放白子，为27位置
    // 黑棋目标为顺时针走到右上终点，白棋目标为逆时针走到右下终点
    // 数组正数表示黑棋，负数表示白棋，0表示没有棋
    public int[] piecesArray = new int[28];

    public GameObject blackPrefab;
    public GameObject whitePrefab;
    public GameObject piecesParent;

    // 存放所有棋子GameObject, [posIndex,pieceIndex]
    private GameObject[,] objs;

    // Start is called before the first frame update
    void Start()
    {
        initPiecesData();
        initAllPiecesObject();
        initAllPrismSel();
        enableAllPS();
    }

    public Transform objectToMove; // 要移动的物体
    private float timer; // 用于控制插值的计时器
    private Vector3 startPosM; // 起始位置
    private Vector3 targetPosM; // 目标位置

    private Transform nextObjectToMove; // 下一个要移动的物体
    private float nextTimer;
    private Vector3 nextStartPosM;
    private Vector3 nextTargetPosM;

    // Update is called once per frame
    void Update()
    {
        // 获取水平和垂直轴的输入值
        //float horizontal = Input.GetAxis("Horizontal");
        //float vertical = Input.GetAxis("Vertical");
        //if (horizontal < 0)
        //{
        //    moveAPiece(0, 4);
        //}
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    lightOnePS(10);
        //}
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    enableAllPS();
        //}
        if (objectToMove != null)
        {
            timer += Time.deltaTime * 1.5f;
            objectToMove.position = Vector3.Lerp(startPosM, targetPosM, timer);
            if (objectToMove.position == targetPosM)
            {
                objectToMove = nextObjectToMove;
                timer = nextTimer;
                startPosM = nextStartPosM;
                targetPosM = nextTargetPosM;
                nextObjectToMove = null;
            }
        }
    }

    void initPiecesData()
    {
        System.Array.Fill(piecesArray, 0);
        // 铺棋盘
        piecesArray[0] = 2;
        piecesArray[5] = -5;
        piecesArray[7] = -3;
        piecesArray[11] = 5;
        piecesArray[12] = -5;
        piecesArray[16] = 3;
        piecesArray[18] = 5;
        piecesArray[23] = -2;
    }

    void initAllPiecesObject()
    {
        objs = new GameObject[28, 15];
        for (int i = 0; i < piecesArray.Length; i++)
        {
            if (piecesArray[i] > 0)
            {
                // 黑棋
                for (int j = 0; j < piecesArray[i]; j++)
                {
                    GameObject prefabInstance = Instantiate(blackPrefab);
                    prefabInstance.transform.position = getPieceXYZ(i, j);
                    prefabInstance.transform.parent = piecesParent.transform;
                    objs[i, j] = prefabInstance;
                }
            }
            else if (piecesArray[i] < 0)
            {
                // 白棋
                for (int j = 0; j > piecesArray[i]; j--)
                {
                    GameObject prefabInstance = Instantiate(whitePrefab);
                    prefabInstance.transform.position = getPieceXYZ(i, -j);
                    prefabInstance.transform.parent = piecesParent.transform;
                    objs[i, -j] = prefabInstance;
                }
            }
        }
    }

    public GameObject prismSelPrefab;
    private GameObject[] pss;

    void initAllPrismSel()
    {
        pss = new GameObject[28];
        for (int i = 0; i < 24; i++)
        {
            GameObject prefabInstance = Instantiate(prismSelPrefab);
            Vector3 pos = getPieceXYZ(i, 0);
            pos.y = 0f;
            if (i < 12) pos.z = -4f;
            else pos.z = 4f;
            prefabInstance.transform.position = pos;
            if (i >= 12) prefabInstance.transform.Rotate(0, 0, 180);
            prefabInstance.transform.parent = piecesParent.transform;
            pss[i] = prefabInstance;
        }
    }

    public void enableAllPS()
    {
        for (int i = 0; i < 24; i++)
        {
            if (pss[i] != null) pss[i].GetComponent<Renderer>().enabled = false;
        }
    }

    public void lightOnePS(int i)
    {
        if (pss[i] != null) pss[i].GetComponent<Renderer>().enabled = true;
    }

    // 棋盘的横向坐标
    private float[] xxx = new float[] { 10f, 8.4f, 6.8f, 5.2f, 3.6f, 2f };

    Vector3 getPieceXYZ(int posIndex, int pieceIndex)
    {
        if (posIndex <= 23)
        {
            float targetX = 1f;
            float targetY = 0.06f;
            float targetZ = 1f;
            // 计算X坐标
            if (posIndex <= 5)
            {
                targetX = xxx[posIndex];
            }
            else if (posIndex <= 11)
            {
                targetX = -xxx[11 - posIndex];
            }
            else if (posIndex <= 17)
            {
                targetX = -xxx[posIndex - 12];
            }
            else if (posIndex <= 23)
            {
                targetX = xxx[23 - posIndex];
            }
            // 计算Z坐标, todo: Y坐标叠二层的逻辑
            if (posIndex <= 11)
            {
                targetZ = -6.3f + pieceIndex * 1.25f;
            }
            else
            {
                targetZ = 6.3f - pieceIndex * 1.25f;
            }
            return new Vector3(targetX, targetY, targetZ);
        }
        else if (posIndex <= 25)
        {
            // 右下的终点槽，放白子，为24位置
            // 右上的终点槽，放黑子，为25位置
            return new Vector3(1f, 1f, 1f);
        }
        else if (posIndex <= 27)
        {
            // 中下的被吃棋位置，放黑子，为26位置
            // 中上的被吃棋位置，放白子，为27位置
            float targetX = 0f;
            float targetY = 0.8f;
            float targetZ = 1f;
            // 计算Z坐标, todo: Y坐标叠二层的逻辑
            if (posIndex == 26)
            {
                targetZ = -5.5f + pieceIndex * 2f;
            }
            else if (posIndex == 27)
            {
                targetZ = 5.5f - pieceIndex * 2f;
            }
            return new Vector3(targetX, targetY, targetZ);
        }
        else
        {
            return new Vector3(1f, 1f, 1f);
        }
    }

    public void moveAPiece(int oriPos, int targetPos)
    {
        if (piecesArray[oriPos] == 0) return;
        GameObject go = objs[oriPos, Mathf.Abs(piecesArray[oriPos]) - 1];
        Vector3 targetV = getPieceXYZ(targetPos, Mathf.Abs(piecesArray[targetPos]));
        // 移动棋子
        if (objectToMove == null)
        {
            objectToMove = go.transform;
            timer = 0f;
            startPosM = objectToMove.position;
            targetPosM = targetV;
        }
        else
        {
            nextObjectToMove = go.transform;
            nextTimer = 0f;
            nextStartPosM = nextObjectToMove.position;
            nextTargetPosM = targetV;
        }
        // 调整数据
        objs[targetPos, Mathf.Abs(piecesArray[targetPos])] = go;
        objs[oriPos, Mathf.Abs(piecesArray[oriPos]) - 1] = null;
    }

    private GameObject pickuped;

    public void pickupAPiece(int pos)
    {
        pickuped = objs[pos, Mathf.Abs(piecesArray[pos]) - 1];
        pickuped.transform.position += new Vector3(0, 0.3f, 0);
    }

    public void removePickup()
    {
        pickuped.transform.position += new Vector3(0, -0.3f, 0);
        pickuped = null;
        enableAllPS();
    }
}
