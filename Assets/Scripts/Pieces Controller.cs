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

    public bool isTest;

    // Start is called before the first frame update
    void Start()
    {
        initPiecesData();
        initAllPiecesObject();
        initAllPrismSel();
        enableAllPS();
    }

    public void restart()
    {
        initPiecesData();

        // 棋子重建
        destoryAllPiecesObject();
        initAllPiecesObject();

        enableAllPS();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initPiecesData()
    {
        System.Array.Fill(piecesArray, 0);
        // 铺棋盘
        if (isTest)
        {
            piecesArray[0] = -4;
            piecesArray[1] = -2;
            piecesArray[2] = -4;
            piecesArray[3] = -2;
            piecesArray[5] = -3;
            piecesArray[26] = 2;
            return;
        }
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

    void destoryAllPiecesObject()
    {
        // 遍历二维数组并销毁每个元素
        for (int i = 0; i < objs.GetLength(0); i++)
        {
            for (int j = 0; j < objs.GetLength(1); j++)
            {
                if (objs[i, j] != null)
                {
                    Destroy(objs[i, j]);
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
        // 右下的终点槽，放白子，为24位置
        // 右上的终点槽，放黑子，为25位置
        pss[24] = GameObject.Find("Right Sel DOWN");
        pss[25] = GameObject.Find("Right Sel UP");
    }

    public void enableAllPS()
    {
        for (int i = 0; i < 26; i++)
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
            // 计算叠在第几层
            int layer = 0;
            if (pieceIndex < 5) layer = 0;
            else if (pieceIndex < 9) layer = 1;
            else if (pieceIndex < 12) layer = 2;
            else if (pieceIndex < 14) layer = 3;
            else layer = 4;
            // 计算Z坐标
            if (layer == 0) targetZ = -6.3f +  pieceIndex               * 1.25f;
            if (layer == 1) targetZ = -6.3f + (pieceIndex -  5f + 0.5f) * 1.25f;
            if (layer == 2) targetZ = -6.3f + (pieceIndex -  9f +   1f) * 1.25f;
            if (layer == 3) targetZ = -6.3f + (pieceIndex - 12f + 1.5f) * 1.25f;
            if (layer == 4) targetZ = -6.3f + (pieceIndex - 14f +   2f) * 1.25f;
            if (posIndex >= 12) targetZ = -targetZ;
            // 计算Y坐标
            targetY += 0.2f * layer;
            return new Vector3(targetX, targetY, targetZ);
        }
        else if (posIndex <= 25)
        {
            // 右下的终点槽，放白子，为24位置
            // 右上的终点槽，放黑子，为25位置
            if (posIndex == 24)
                return new Vector3(11.889f, 0.28f, -1.45f - pieceIndex * 0.325f);
            else
                return new Vector3(11.889f, 0.28f, 6.45f - pieceIndex * 0.325f);
        }
        else if (posIndex <= 27)
        {
            // 中下的被吃棋位置，放黑子，为26位置
            // 中上的被吃棋位置，放白子，为27位置
            float targetX = 0f;
            float targetY = 0.3f;
            float targetZ = 1f;
            // 计算Z坐标
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

    public void moveAPieceData(int oriPos, int targetPos)
    {
        if (piecesArray[oriPos] == 0) return;
        GameObject go = objs[oriPos, Mathf.Abs(piecesArray[oriPos]) - 1];
        Vector3 targetV = getPieceXYZ(targetPos, Mathf.Abs(piecesArray[targetPos]));
        // 调整数据
        objs[targetPos, Mathf.Abs(piecesArray[targetPos])] = go;
        objs[oriPos, Mathf.Abs(piecesArray[oriPos]) - 1] = null;
        // 播放动画
        PieceController pCon = go.GetComponent<PieceController>();
        pCon.MoveToTarget(targetV,true);
        pCon.ChangeBackColor();
    }

    private GameObject pickuped;

    public void pickupAPiece(int pos)
    {
        pickuped = objs[pos, Mathf.Abs(piecesArray[pos]) - 1];
        pickuped.GetComponent<PieceController>().pickup();
    }

    public void removePickup()
    {
        pickuped.GetComponent<PieceController>().unPickup();
        pickuped = null;
        enableAllPS();
    }

    public void shakeDied(int worldPos)
    {
        ShakeAndHighlight sah = objs[worldPos, Mathf.Abs(piecesArray[worldPos]) - 1].GetComponent<ShakeAndHighlight>();
        sah.TriggerShakeAndHighlight();
    }
}
