using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{

    public PiecesController PC;

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
            }
            else if (currentState == State.whiteDice)
            {
                currentState = State.whiteMoving;
                moveI2J = new List<int>();
                Debug.Log("�ֵ�����");
            }
        }
        if (currentState == State.blackMoving || currentState == State.whiteMoving)
        {
            if (moveI2J.Count == 2)
            {
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

    public void ReceiveMouseDown(int index)
    {
        // ִ�е������ʱ��Ҫ�Ĳ����������ӡһ�仰
        //Debug.Log("Game Controller Object Clicked: " + index);
        if (PC.objectToMove != null) return;
        if (moveI2J.Count == 0)
        {
            if (PC.piecesArray[index] == 0) return;
            if (currentState == State.blackMoving && PC.piecesArray[index] < 0) return;
            if (currentState == State.whiteMoving && PC.piecesArray[index] > 0) return;
            moveI2J.Add(index);
            Debug.Log("ѡ����λ��1");
            PC.pickupAPiece(index);
            initPosTar();
        }
        else if (moveI2J.Count == 1)
        {
            // �ж���ŵ��Ƿ����
            bool canMove = true;
            if (moveI2J[0] == index) canMove = false;
            if (currentState == State.blackMoving)
            {
                if (!positiveMoves.Contains(index - moveI2J[0])) { Debug.Log("������ƥ��"); canMove = false; }
                if (PC.piecesArray[index] <= -2) { Debug.Log("�а�����"); canMove = false; }
            }
            else if (currentState == State.whiteMoving)
            {
                if (!positiveMoves.Contains(moveI2J[0] - index)) { Debug.Log("������ƥ��"); canMove = false; }
                if (PC.piecesArray[index] >= 2) { Debug.Log("�к�����"); canMove = false; }
            }
            if (!canMove) { moveI2J.RemoveAt(0); PC.removePickup(); return; }
            // ȷ���ƶ�
            int moveLength = Mathf.Abs(index - moveI2J[0]);
            reSum = moveLength;
            recurDel(0);
            moveI2J.Add(index);
            Debug.Log("ѡ����λ��2");
            updateNumText();
        }
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
