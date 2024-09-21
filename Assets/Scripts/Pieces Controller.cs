using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesController : MonoBehaviour
{
    // �������¼����λ�õ���������
    // �����½ǿ�ʼ����˳ʱ����ת��Ϊ0~23λ��
    // ���µ��յ�ۣ��Ű��ӣ�Ϊ24λ��
    // ���ϵ��յ�ۣ��ź��ӣ�Ϊ25λ��
    // ���µı�����λ�ã��ź��ӣ�Ϊ26λ��
    // ���ϵı�����λ�ã��Ű��ӣ�Ϊ27λ��
    // ����Ŀ��Ϊ˳ʱ���ߵ������յ㣬����Ŀ��Ϊ��ʱ���ߵ������յ�
    // ����������ʾ���壬������ʾ���壬0��ʾû����
    public int[] piecesArray = new int[28];

    public GameObject blackPrefab;
    public GameObject whitePrefab;
    public GameObject piecesParent;

    // �����������GameObject, [posIndex,pieceIndex]
    private GameObject[,] objs;

    // Start is called before the first frame update
    void Start()
    {
        initPiecesData();
        initAllPiecesObject();
        initAllPrismSel();
        enableAllPS();
    }

    public Transform objectToMove; // Ҫ�ƶ�������
    private float timer; // ���ڿ��Ʋ�ֵ�ļ�ʱ��
    private Vector3 startPosM; // ��ʼλ��
    private Vector3 targetPosM; // Ŀ��λ��

    private Transform nextObjectToMove; // ��һ��Ҫ�ƶ�������
    private float nextTimer;
    private Vector3 nextStartPosM;
    private Vector3 nextTargetPosM;

    // Update is called once per frame
    void Update()
    {
        // ��ȡˮƽ�ʹ�ֱ�������ֵ
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
        // ������
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
                // ����
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
                // ����
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

    // ���̵ĺ�������
    private float[] xxx = new float[] { 10f, 8.4f, 6.8f, 5.2f, 3.6f, 2f };

    Vector3 getPieceXYZ(int posIndex, int pieceIndex)
    {
        if (posIndex <= 23)
        {
            float targetX = 1f;
            float targetY = 0.06f;
            float targetZ = 1f;
            // ����X����
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
            // ����Z����, todo: Y�����������߼�
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
            // ���µ��յ�ۣ��Ű��ӣ�Ϊ24λ��
            // ���ϵ��յ�ۣ��ź��ӣ�Ϊ25λ��
            return new Vector3(1f, 1f, 1f);
        }
        else if (posIndex <= 27)
        {
            // ���µı�����λ�ã��ź��ӣ�Ϊ26λ��
            // ���ϵı�����λ�ã��Ű��ӣ�Ϊ27λ��
            float targetX = 0f;
            float targetY = 0.8f;
            float targetZ = 1f;
            // ����Z����, todo: Y�����������߼�
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
        // �ƶ�����
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
        // ��������
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
