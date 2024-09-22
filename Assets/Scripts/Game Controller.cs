using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{

    private PiecesController PC;

    public GameObject Btip;
    public GameObject Wtip;

    // ��Ϸ����״̬��
    private enum State
    {
        initDice
        , blackDice, blackMoving
        , whiteDice, whiteMoving
    }

    private State currentState;

    private TMP_Text blackTextBox;
    private TMP_Text whiteTextBox;

    // Start is called before the first frame update
    void Start()
    {
        GameObject pcgo = GameObject.Find("PiecesController");
        PC = pcgo.GetComponent<PiecesController>();
        // �Ƚ����ɫ���ӽ׶�
        currentState = State.blackDice;
        // �ҵ�TextMeshPro������ڵ�GameObject
        GameObject textMeshProGameObjectB = GameObject.Find("Bnum");
        GameObject textMeshProGameObjectW = GameObject.Find("Wnum");
        // ��ȡTextMeshPro���
        blackTextBox = textMeshProGameObjectB.GetComponent<TMP_Text>();
        whiteTextBox = textMeshProGameObjectW.GetComponent<TMP_Text>();
    }

    private int[] randomNum;

    private List<int> moveI2J;

    // Update is called once per frame
    void Update()
    {
        if (currentState == State.blackDice || currentState == State.whiteDice)
        {
            randomNum = new int[4];
            // ���1~6֮�������
            randomNum[0] = Random.Range(1, 7);
            randomNum[1] = Random.Range(1, 7);
            if (randomNum[0] == randomNum[1])
            {
                randomNum[2] = randomNum[0];
                randomNum[3] = randomNum[0];
            }
            updateNumText();
            Debug.Log(randomNum[0] + "," + randomNum[1]);
            if (currentState == State.blackDice)
            {
                currentState = State.blackMoving;
                moveI2J = new List<int>();
                Debug.Log("�ֵ�����");
                Btip.SetActive(true);
                Wtip.SetActive(false);
            }
            else if (currentState == State.whiteDice)
            {
                currentState = State.whiteMoving;
                moveI2J = new List<int>();
                Debug.Log("�ֵ�����");
                Btip.SetActive(false);
                Wtip.SetActive(true);
            }
        }
        if (currentState == State.blackMoving || currentState == State.whiteMoving)
        {
            if (moveI2J.Count == 2)
            {
                if (die > 0)
                {
                    // 0-û����1-���˺��壬2-���˰���
                    // ���������߼�
                    if (die == 1)
                    {
                        PC.moveAPiece(moveI2J[1], 26);
                        PC.piecesArray[moveI2J[1]]--;
                        PC.piecesArray[26]++;
                    }
                    else if (die == 2)
                    {
                        PC.moveAPiece(moveI2J[1], 27);
                        PC.piecesArray[moveI2J[1]]++;
                        PC.piecesArray[27]--;
                    }
                    die = 0;
                }
                PC.moveAPiece(moveI2J[0], moveI2J[1]);
                if (currentState == State.blackMoving)
                {
                    PC.piecesArray[moveI2J[0]]--;
                    PC.piecesArray[moveI2J[1]]++;
                }
                else if (currentState == State.whiteMoving)
                {
                    PC.piecesArray[moveI2J[0]]++;
                    PC.piecesArray[moveI2J[1]]--;
                }
                updateCanFinish();
                moveI2J = new List<int>();
                if (randomNum[0] + randomNum[1] + randomNum[2] + randomNum[3] == 0)
                {
                    if (currentState == State.blackMoving)
                    {
                        currentState = State.whiteDice;
                    }
                    else if (currentState == State.whiteMoving)
                    {
                        currentState = State.blackDice;
                    }
                }
            }
        }
    }

    private void updateCanFinish()
    {
        // �жϺ���
        int sumBF = 0;
        for (int i = 18; i < 24; i++) sumBF += PC.piecesArray[i];
        sumBF += PC.piecesArray[25];
        if (sumBF == 15) blackCanFinish = true;
        else blackCanFinish = false;
        // �жϰ���
        int sumWF = 0;
        for (int i = 0; i < 6; i++) sumWF -= PC.piecesArray[i];
        sumWF -= PC.piecesArray[24];
        if (sumWF == 15) whiteCanFinish = true;
        else whiteCanFinish = false;
    }

    private void updateNumText()
    {
        if (currentState == State.blackDice || currentState == State.blackMoving)
        {
            blackTextBox.text = randomNum[0] + " " + randomNum[1] + " " + randomNum[2] + " " + randomNum[3];
        }
        else if (currentState == State.whiteDice || currentState == State.whiteMoving)
        {
            whiteTextBox.text = randomNum[0] + " " + randomNum[1] + " " + randomNum[2] + " " + randomNum[3];
        }
    }

    private List<int> positiveMoves;

    private int die = 0; // 0-û����1-���˺��壬2-���˰���

    public void ReceiveMouseDown(int index)
    {
        // �������������˳�
        if (PC.objectToMove != null) return;
        // ���µ��յ�ۣ��Ű��ӣ�Ϊ24λ��
        // ���ϵ��յ�ۣ��ź��ӣ�Ϊ25λ��
        //if (index == 24 || index == 25)
        //{
        //    //���24,25��������
        //    if (moveI2J.Count == 0) return;
        //    else if (moveI2J.Count == 1)
        //    {
        //        bool canMove = true;
        //        if(currentState == State.blackMoving &&)
        //        // �޷��ƶ�
        //        if (!canMove) { moveI2J.RemoveAt(0); PC.removePickup(); return; }
        //        // ȷ���ƶ�
        //    }
        //}
        if (moveI2J.Count == 0)
        {
            if (PC.piecesArray[index] == 0 || index == 24 || index == 25) return;
            if (currentState == State.blackMoving && PC.piecesArray[index] < 0) return;
            if (currentState == State.whiteMoving && PC.piecesArray[index] > 0) return;
            moveI2J.Add(index);
            Debug.Log("ѡ����λ��1");
            PC.pickupAPiece(index);
            initPosTar();
            for (int i = 0; i < 26; i++)
            {
                if (judgeCanMove(i, true))
                {
                    PC.lightOnePS(i);
                }
            }
        }
        else if (moveI2J.Count == 1)
        {
            bool canMove = true;
            canMove = judgeCanMove(index, false);
            if (!canMove) { moveI2J.RemoveAt(0); PC.removePickup(); return; }
            // ȷ���ƶ�
            int moveLength = Mathf.Abs(index - moveI2J[0]);
            if (index == 24) moveLength = 1 + moveI2J[0];
            if (index == 25) moveLength = 24 - moveI2J[0];
            if (moveI2J[0] == 26) moveLength = index + 1;
            if (moveI2J[0] == 27) moveLength = 24 - index;
            reSum = moveLength;
            recurDel(0);
            moveI2J.Add(index);
            Debug.Log("ѡ����λ��2");
            updateNumText();
            PC.enableAllPS();
        }
    }

    private bool blackCanFinish = false;
    private bool whiteCanFinish = false;

    private bool judgeCanMove(int index, bool isSim)
    {
        // �ж���ŵ��Ƿ����
        if (moveI2J[0] == index) return false;
        // ���µ��յ�ۣ��Ű��ӣ�Ϊ24λ��
        // ���ϵ��յ�ۣ��ź��ӣ�Ϊ25λ��
        // ���µı�����λ�ã��ź��ӣ�Ϊ26λ��
        // ���ϵı�����λ�ã��Ű��ӣ�Ϊ27λ��
        if (currentState == State.blackMoving)
        {
            if (index == 24 || moveI2J[0] == 27) return false;
            if (index == 25)
            {
                if (positiveMoves.Contains(24 - moveI2J[0]) && blackCanFinish) return true;
                else return false;
            }
            if (moveI2J[0] == 26)
            {
                // ������ͼ��������
                if (!positiveMoves.Contains(index + 1)) return false;
            }
            else
            {
                if (!positiveMoves.Contains(index - moveI2J[0])) { Debug.Log("������ƥ��"); return false; }
            }
            if (PC.piecesArray[index] <= -2) { Debug.Log("�а�����"); return false; }
            if (PC.piecesArray[index] == -1) { Debug.Log("�Ե�����"); if (!isSim) die = 2; }
        }
        else if (currentState == State.whiteMoving)
        {
            if (index == 25 || moveI2J[0] == 26) return false;
            if (index == 24)
            {
                if (positiveMoves.Contains(1 + moveI2J[0]) && whiteCanFinish) return true;
                else return false;
            }
            if (moveI2J[0] == 27)
            {
                // ������ͼ��������
                if (!positiveMoves.Contains(24 - index)) return false;
            }
            else
            {
                if (!positiveMoves.Contains(moveI2J[0] - index)) { Debug.Log("������ƥ��"); return false; }
            }
            if (PC.piecesArray[index] >= 2) { Debug.Log("�к�����"); return false; }
            if (PC.piecesArray[index] == 1) { Debug.Log("�Ե�����"); if (!isSim) die = 1; }
        }
        return true;
    }

    private int reSum;

    private void initPosTar()
    {
        positiveMoves = new List<int>();
        reSum = 0;
        recur(0);
        Debug.Log("===== positiveMoves =====");
        positiveMoves.ForEach(item => Debug.Log(item));
    }

    private void recur(int start)
    {
        if (reSum > 0 && !positiveMoves.Contains(reSum)) positiveMoves.Add(reSum);
        if (start == randomNum.Length) return;
        recur(start + 1);
        reSum += randomNum[start];
        recur(start + 1);
        reSum -= randomNum[start];
    }

    private bool recurDel(int start)
    {
        if (reSum == 0) return true;
        if (start == randomNum.Length) return false;
        // ʹ������
        int tmp = randomNum[start];
        reSum -= tmp;
        randomNum[start] = 0;
        if (recurDel(start + 1)) return true;
        reSum += tmp;
        randomNum[start] = tmp;
        // ��������
        if (recurDel(start + 1)) return true;
        return false;
    }
}
