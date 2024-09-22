using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{

    private PiecesController PC;

    public GameObject Btip;
    public GameObject Wtip;

    // 游戏流程状态机
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
        // 先进入黑色骰子阶段
        currentState = State.blackDice;
        // 找到TextMeshPro组件所在的GameObject
        GameObject textMeshProGameObjectB = GameObject.Find("Bnum");
        GameObject textMeshProGameObjectW = GameObject.Find("Wnum");
        // 获取TextMeshPro组件
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
            // 随机1~6之间的整数
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
                Debug.Log("轮到黑走");
                Btip.SetActive(true);
                Wtip.SetActive(false);
            }
            else if (currentState == State.whiteDice)
            {
                currentState = State.whiteMoving;
                moveI2J = new List<int>();
                Debug.Log("轮到白走");
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
                    // 0-没死，1-死了黑棋，2-死了白棋
                    // 处理被吃棋逻辑
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
        // 判断黑棋
        int sumBF = 0;
        for (int i = 18; i < 24; i++) sumBF += PC.piecesArray[i];
        sumBF += PC.piecesArray[25];
        if (sumBF == 15) blackCanFinish = true;
        else blackCanFinish = false;
        // 判断白棋
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

    private int die = 0; // 0-没死，1-死了黑棋，2-死了白棋

    public void ReceiveMouseDown(int index)
    {
        // 动画播放中则退出
        if (PC.objectToMove != null) return;
        // 右下的终点槽，放白子，为24位置
        // 右上的终点槽，放黑子，为25位置
        //if (index == 24 || index == 25)
        //{
        //    //点击24,25单独处理
        //    if (moveI2J.Count == 0) return;
        //    else if (moveI2J.Count == 1)
        //    {
        //        bool canMove = true;
        //        if(currentState == State.blackMoving &&)
        //        // 无法移动
        //        if (!canMove) { moveI2J.RemoveAt(0); PC.removePickup(); return; }
        //        // 确定移动
        //    }
        //}
        if (moveI2J.Count == 0)
        {
            if (PC.piecesArray[index] == 0 || index == 24 || index == 25) return;
            if (currentState == State.blackMoving && PC.piecesArray[index] < 0) return;
            if (currentState == State.whiteMoving && PC.piecesArray[index] > 0) return;
            moveI2J.Add(index);
            Debug.Log("选择了位置1");
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
            // 确定移动
            int moveLength = Mathf.Abs(index - moveI2J[0]);
            if (index == 24) moveLength = 1 + moveI2J[0];
            if (index == 25) moveLength = 24 - moveI2J[0];
            if (moveI2J[0] == 26) moveLength = index + 1;
            if (moveI2J[0] == 27) moveLength = 24 - index;
            reSum = moveLength;
            recurDel(0);
            moveI2J.Add(index);
            Debug.Log("选择了位置2");
            updateNumText();
            PC.enableAllPS();
        }
    }

    private bool blackCanFinish = false;
    private bool whiteCanFinish = false;

    private bool judgeCanMove(int index, bool isSim)
    {
        // 判断落脚点是否合理
        if (moveI2J[0] == index) return false;
        // 右下的终点槽，放白子，为24位置
        // 右上的终点槽，放黑子，为25位置
        // 中下的被吃棋位置，放黑子，为26位置
        // 中上的被吃棋位置，放白子，为27位置
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
                // 黑棋试图从起点出发
                if (!positiveMoves.Contains(index + 1)) return false;
            }
            else
            {
                if (!positiveMoves.Contains(index - moveI2J[0])) { Debug.Log("点数不匹配"); return false; }
            }
            if (PC.piecesArray[index] <= -2) { Debug.Log("有白棋在"); return false; }
            if (PC.piecesArray[index] == -1) { Debug.Log("吃掉白棋"); if (!isSim) die = 2; }
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
                // 白棋试图从起点出发
                if (!positiveMoves.Contains(24 - index)) return false;
            }
            else
            {
                if (!positiveMoves.Contains(moveI2J[0] - index)) { Debug.Log("点数不匹配"); return false; }
            }
            if (PC.piecesArray[index] >= 2) { Debug.Log("有黑棋在"); return false; }
            if (PC.piecesArray[index] == 1) { Debug.Log("吃掉黑棋"); if (!isSim) die = 1; }
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
        // 使用自身
        int tmp = randomNum[start];
        reSum -= tmp;
        randomNum[start] = 0;
        if (recurDel(start + 1)) return true;
        reSum += tmp;
        randomNum[start] = tmp;
        // 不用自身
        if (recurDel(start + 1)) return true;
        return false;
    }
}
