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
            //Debug.Log(randomNum[0] + "," + randomNum[1]);
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
                // 确定走棋后

                // 如果有被吃掉的棋子
                if (removeWorldIndexList.Count > 0)
                {
                    foreach (int removedPos in removeWorldIndexList)
                    {
                        if (PC.piecesArray[removedPos] == 1)
                        {
                            // 死了黑棋
                            PC.moveAPieceData(removedPos, 26);
                            PC.piecesArray[removedPos]--;
                            PC.piecesArray[26]++;
                            Debug.Log("死黑棋位置：" + removedPos);
                        }
                        else if (PC.piecesArray[removedPos] == -1)
                        {
                            // 死了白棋
                            PC.moveAPieceData(removedPos, 27);
                            PC.piecesArray[removedPos]++;
                            PC.piecesArray[27]--;
                            Debug.Log("死白棋位置：" + removedPos);
                        }
                    }
                }
                // 走棋逻辑
                PC.moveAPieceData(moveI2J[0], moveI2J[1]);
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
                // 清空moveI2J，等待下次点击
                moveI2J = new List<int>();
                // 骰子用光，交换回合
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

    // 更新双方结束情况
    private void updateCanFinish()
    {
        int blackFinishMax = -1;
        int whiteFinishMax = -1;
        if (currentState == State.blackMoving)
        {
            // 判断黑棋
            int sumBF = 0;
            for (int i = 18; i < 24; i++) sumBF += PC.piecesArray[i];
            sumBF += PC.piecesArray[25];
            if (sumBF == 15)
            {
                blackCanFinish = true;
                // 对于已经到最后一象限的情况，为骰子溢出的情况预计算
                for (int i = 23; i >= 18; i--)
                {
                    if (PC.piecesArray[i] > 0)
                    {
                        blackFinishMax = 24 - i;
                    }
                }
                Debug.Log("blackFinishMax " + blackFinishMax);
                for (int i = 0; i < 4; i++)
                    if (randomNum[i] > blackFinishMax)
                        randomNum[i] = blackFinishMax;
            }
            else blackCanFinish = false;
        }
        else if (currentState == State.whiteMoving)
        {
            // 判断白棋
            int sumWF = 0;
            for (int i = 0; i < 6; i++) sumWF -= PC.piecesArray[i];
            sumWF -= PC.piecesArray[24];
            if (sumWF == 15)
            {
                whiteCanFinish = true;
                for (int i = 0; i <= 5; i++)
                {
                    if (PC.piecesArray[i] < 0)
                    {
                        whiteFinishMax = i + 1;
                    }
                }
                Debug.Log("whiteFinishMax " + whiteFinishMax);
                for (int i = 0; i < 4; i++)
                    if (randomNum[i] > whiteFinishMax)
                        randomNum[i] = whiteFinishMax;
            }
            else whiteCanFinish = false;
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

    // 执行的起点，响应鼠标点击事件
    public void ReceiveMouseDown(int index)
    {
        if (moveI2J.Count == 0)
        {
            // 第一步
            if (PC.piecesArray[index] == 0 || index == 24 || index == 25) return;
            if (currentState == State.blackMoving && PC.piecesArray[index] < 0) return;
            if (currentState == State.whiteMoving && PC.piecesArray[index] > 0) return;
            moveI2J.Add(index);
            PC.pickupAPiece(index);
            updateCanFinish();
            // 判断在起点有被吃子
            if (PC.piecesArray[logic2world(0)] != 0 && index != logic2world(0))
            {
                PC.shakeDied(logic2world(0));
                moveI2J.RemoveAt(0); PC.removePickup(); return;
            }
            judgeAllCanMove();
        }
        else if (moveI2J.Count == 1)
        {
            // 第二步
            if (!canMovePos.Contains(world2logic(index)))
            {
                moveI2J.RemoveAt(0); PC.removePickup(); return;
            }
            // 确定移动
            List<int> moveList = stepsResult[canMovePos.IndexOf(world2logic(index))];
            int start = world2logic(moveI2J[0]);
            removeWorldIndexList = new List<int>();
            // 模拟移动，构建被吃子list
            foreach (int i in moveList)
            {
                start += randomNum[i];
                int pa = PC.piecesArray[logic2world(start)];
                if (
                    currentState == State.blackMoving && pa == -1
                    ||
                    currentState == State.whiteMoving && pa == 1
                   )
                {
                    removeWorldIndexList.Add(logic2world(start));
                }
                randomNum[i] = 0;
            }
            moveI2J.Add(index);
            updateNumText();
            PC.enableAllPS();
        }
    }

    // 被吃掉的子合集
    private List<int> removeWorldIndexList;

    private bool blackCanFinish = false;
    private bool whiteCanFinish = false;

    private void judgeAllCanMove()
    {
        // 核心方法，当第一个棋子落下「moveI2J[0]」后，判断棋子所有可能落点，并记录路径
        // 「moveI2J[0]」和index都是棋盘的绝对坐标
        // 已掷出的点数或剩余的点数：randomNum数组

        // 右下的终点槽，放白子，为24位置
        // 右上的终点槽，放黑子，为25位置
        // 中下的被吃棋位置，放黑子，为26位置
        // 中上的被吃棋位置，放白子，为27位置

        stepsResult = new List<List<int>>();
        canMovePos = new List<int>();

        currentPos = world2logic(moveI2J[0]);
        currentSteps = new List<int>();
        recur(0);
        currentPos = world2logic(moveI2J[0]);
        currentSteps = new List<int>();
        recur(1);
        currentPos = world2logic(moveI2J[0]);
        currentSteps = new List<int>();
        recur(2);
        currentPos = world2logic(moveI2J[0]);
        currentSteps = new List<int>();
        recur(3);
    }

    private int currentPos;
    private List<int> canMovePos;
    private List<int> currentSteps;
    private List<List<int>> stepsResult;

    private void recur(int index)
    {
        // index代表randomNum数组的下标，0~3
        // 步数为0，退出
        if (randomNum[index] == 0) return;
        // 重复访问，退出
        if (currentSteps.Contains(index)) return;
        currentPos += randomNum[index];
        if (currentPos >= 26)
        {
            // 超出范围
            currentPos -= randomNum[index]; return;
        }
        if (currentPos == 25)
        {
            // 达到终点
            if (currentState == State.blackMoving && !blackCanFinish) { currentPos -= randomNum[index]; return; }
            if (currentState == State.whiteMoving && !whiteCanFinish) { currentPos -= randomNum[index]; return; }
        }
        // 当前位置是否可落子
        //Debug.Log("currentPos " + currentPos);
        int pa = PC.piecesArray[logic2world(currentPos)];
        bool canMove = false;
        //Debug.Log("canMove " + canMove);
        if (currentState == State.blackMoving && pa >= -1) canMove = true;
        if (currentState == State.whiteMoving && pa <= 1) canMove = true;
        if (canMove)
        {
            // 记录
            currentSteps.Add(index);
            stepsResult.Add(new List<int>(currentSteps));
            PC.lightOnePS(logic2world(currentPos));
            canMovePos.Add(currentPos);
            recur(0);
            recur(1);
            recur(2);
            recur(3);
            currentSteps.Remove(index);
        }
        currentPos -= randomNum[index];
    }

    private int world2logic(int worldIndex)
    {
        // 世界坐标：棋盘的坐标
        // --> 逻辑坐标：
        //     0为被吃的位置
        //     1~24为前进路线第一格到最后一格
        //     25为终点位置
        //     -1表示错误
        if (currentState == State.blackMoving)
        {
            if (worldIndex == 26) return 0;
            else if (worldIndex == 25) return 25;
            else if (worldIndex >= 0 && worldIndex <= 23) return worldIndex + 1;
        }
        else if (currentState == State.whiteMoving)
        {
            if (worldIndex == 27) return 0;
            else if (worldIndex == 24) return 25;
            else if (worldIndex >= 0 && worldIndex <= 23) return 24 - worldIndex;
        }
        return -1;
    }

    private int logic2world(int logicIndex)
    {
        // 逻辑坐标 --> 世界坐标
        if (currentState == State.blackMoving)
        {
            if (logicIndex == 0) return 26;
            else if (logicIndex == 25) return 25;
            else if (logicIndex >= 1 && logicIndex <= 24) return logicIndex - 1;
        }
        else if (currentState == State.whiteMoving)
        {
            if (logicIndex == 0) return 27;
            else if (logicIndex == 25) return 24;
            else if (logicIndex >= 1 && logicIndex <= 24) return 24 - logicIndex;
        }
        return -1;
    }
}