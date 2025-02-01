using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // 用于访问 UI 元素

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

    private HighlightableImage hiBlack;
    private HighlightableImage hiWhite;

    public SoundController sc;

    private int round;

    private DiceController[,] DiceCons;

    // 引用“重新开始”按钮
    public Button restartButton;

    // 游戏状态变量
    private bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        GameObject pcgo = GameObject.Find("PiecesController");
        PC = pcgo.GetComponent<PiecesController>();
        // 先进入黑色骰子阶段
        currentState = State.blackDice;
        round = 0;
        // 找到TextMeshPro组件所在的GameObject
        GameObject textMeshProGameObjectB = GameObject.Find("Bnum");
        GameObject textMeshProGameObjectW = GameObject.Find("Wnum");
        // 获取TextMeshPro组件
        blackTextBox = textMeshProGameObjectB.GetComponent<TMP_Text>();
        whiteTextBox = textMeshProGameObjectW.GetComponent<TMP_Text>();
        // 获取ui上方左右的走棋提示
        hiBlack = GameObject.Find("BlackUITip").GetComponent<HighlightableImage>();
        hiWhite = GameObject.Find("WhiteUITip").GetComponent<HighlightableImage>();
        // 获取骰子脚本对象
        DiceCons = new DiceController[2, 4];
        DiceCons[0, 0] = GameObject.Find("DiceB1").GetComponent<DiceController>();
        DiceCons[0, 1] = GameObject.Find("DiceB2").GetComponent<DiceController>();
        DiceCons[0, 2] = GameObject.Find("DiceB3").GetComponent<DiceController>();
        DiceCons[0, 3] = GameObject.Find("DiceB4").GetComponent<DiceController>();
        DiceCons[1, 0] = GameObject.Find("DiceW1").GetComponent<DiceController>();
        DiceCons[1, 1] = GameObject.Find("DiceW2").GetComponent<DiceController>();
        DiceCons[1, 2] = GameObject.Find("DiceW3").GetComponent<DiceController>();
        DiceCons[1, 3] = GameObject.Find("DiceW4").GetComponent<DiceController>();

        // 初始化游戏状态
        isGameOver = false;

        // 设置按钮的初始状态：不可见且不可点击
        restartButton.gameObject.SetActive(false);
        restartButton.interactable = false;

        // 设置按钮点击事件监听器
        restartButton.onClick.AddListener(RestartGame);
    }

    // 游戏结束逻辑（由其他脚本调用）
    public void GameEnd(string winner)
    {
        isGameOver = true;

        // 显示获胜信息和“重新开始”按钮
        // 假设你有一个 UI 元素来显示获胜者信息
        // winText.text = "Winner: " + winner; // winText 是一个 Text 组件的引用
        if(winner=="black") blackTextBox.text = "Black Win";
        else whiteTextBox.text = "White Win";

        // 显示“重新开始”按钮
        restartButton.gameObject.SetActive(true);
        restartButton.interactable = true;
    }

    // 重置游戏逻辑
    public void RestartGame()
    {
        // 重置棋盘和棋子（具体实现取决于你的游戏逻辑）
        ResetBoardAndPieces();

        // 重置游戏状态
        isGameOver = false;

        // 隐藏“重新开始”按钮并禁用其交互性
        restartButton.gameObject.SetActive(false);
        restartButton.interactable = false;

        // 重新初始化其他必要的游戏逻辑脚本（如果有的话）
    }

    // 重置棋盘和棋子的具体实现（需要根据你的游戏逻辑来实现）
    private void ResetBoardAndPieces()
    {
        // 先进入黑色骰子阶段
        currentState = State.blackDice;
        round = 0;

        // 初始化游戏状态
        isGameOver = false;

        // 设置按钮的初始状态：不可见且不可点击
        restartButton.gameObject.SetActive(false);
        restartButton.interactable = false;

        // 一些类变量的重新初始化
        moveI2J = new List<int>();
        isPaused = false;
        blackCanFinish = false;
        whiteCanFinish = false;

        // 铺棋盘
        PC.restart();

        // 文本
        blackTextBox.text = "";
        whiteTextBox.text = "";
    }

    private int[] randomNum;

    private List<int> moveI2J = new List<int>();

    // Update is called once per frame
    void Update()
    {
        if (isPaused || isGameOver) return;
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

            int dIndex = -1;
            if (currentState == State.blackDice) dIndex = 0;
            else dIndex = 1;
            
            DiceCons[dIndex, 0].RotateAndDown(randomNum[0]);
            DiceCons[dIndex, 2].RotateAndDown(randomNum[1]);
            if (randomNum[2] > 0) DiceCons[dIndex, 1].appear(randomNum[2]);
            if (randomNum[3] > 0) DiceCons[dIndex, 3].appear(randomNum[3]);
            
            // 等待骰子转完
            StartCoroutine(DelayExample(2.5f));

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
                        }
                        else if (PC.piecesArray[removedPos] == -1)
                        {
                            // 死了白棋
                            PC.moveAPieceData(removedPos, 27);
                            PC.piecesArray[removedPos]++;
                            PC.piecesArray[27]--;
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

                // 判断是否胜利
                if (currentState == State.blackMoving && PC.piecesArray[25] == 15)
                {
                    // 黑方胜利
                    Debug.Log("black win");
                    GameEnd("black");
                }
                else if (currentState == State.whiteMoving && PC.piecesArray[24] == -15)
                {
                    // 白方胜利
                    Debug.Log("white win");
                    GameEnd("white");
                }

                // 骰子用光，交换回合
                if (randomNum[0] + randomNum[1] + randomNum[2] + randomNum[3] == 0)
                {
                    changeTurn();
                    return;
                }
                judgeHasNoCanMove();
            }
        }
    }

    // 协程方法
    private IEnumerator DelayExample(float delay)
    {
        // 打印开始信息
        //Debug.Log("开始等待 " + delay + " 秒...");

        if (currentState == State.blackDice) { hiBlack.ScaleImageUp(); hiWhite.RestoreOriginalSize(); }
        if (currentState == State.whiteDice) { hiWhite.ScaleImageUp(); hiBlack.RestoreOriginalSize(); }

        // 等待指定的秒数
        isPaused = true;
        yield return new WaitForSeconds(delay);
        isPaused = false;

        if (currentState == State.blackDice)
        {
            currentState = State.blackMoving;
            moveI2J = new List<int>();
            if (round++ < 10) Btip.SetActive(true);
            Wtip.SetActive(false);
            judgeHasNoCanMove();
        }
        else if (currentState == State.whiteDice)
        {
            currentState = State.whiteMoving;
            moveI2J = new List<int>();
            if (round++ < 10) Wtip.SetActive(true);
            Btip.SetActive(false);
            judgeHasNoCanMove();
        }

        sc.PlayDiceSound();

        // 打印结束信息
        //Debug.Log("等待结束！");
    }

    private void judgeHasNoCanMove()
    {
        // 开始做“独游”判断
        bool hasNoCanMove = false;
        // 判断在起点有被吃子
        if (PC.piecesArray[logic2world(0)] != 0)
        {
            // 有被吃子
            int index = logic2world(0);
            moveI2J.Add(index);
            judgeAllCanMove();
            if (canMovePos.Count == 0) hasNoCanMove = true;
            // 清空moveI2J
            moveI2J = new List<int>();
            PC.enableAllPS();
        }
        else
        {
            // 无被吃子
            bool canMove = false;
            for (int i = 0; i < 24; i++)
            {
                int index = logic2world(i);
                int pa = PC.piecesArray[logic2world(i)];
                if(currentState == State.blackMoving && pa > 0)
                {
                    moveI2J.Add(index);
                    judgeAllCanMove();
                    if (canMovePos.Count > 0) canMove = true;
                    // 清空moveI2J
                    moveI2J = new List<int>();
                    PC.enableAllPS();
                }
                else if(currentState == State.whiteMoving && pa < 0)
                {
                    moveI2J.Add(index);
                    judgeAllCanMove();
                    if (canMovePos.Count > 0) canMove = true;
                    // 清空moveI2J
                    moveI2J = new List<int>();
                    PC.enableAllPS();
                }
            }
            if (!canMove) hasNoCanMove = true;
        }
        updateCanFinish();
        if (currentState == State.blackMoving && blackCanFinish)
        {
            hasNoCanMove = false;
        }
        if (currentState == State.whiteMoving && whiteCanFinish)
        {
            hasNoCanMove = false;
        }
        // “独游”的处理
        if (hasNoCanMove)
        {
            Debug.Log("独游一轮");
            // 清空骰子
            randomNum[0] = 0;
            randomNum[1] = 0;
            randomNum[2] = 0;
            randomNum[3] = 0;
            Wtip.SetActive(false);
            Btip.SetActive(false);
            updateNumText(true);
            changeTurn(true);
        }
    }

    private void changeTurn(bool isSkipTurn = false)
    {
        Wtip.SetActive(false);
        Btip.SetActive(false);
        if (!isSkipTurn)
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
        else
        {
            PauseForSeconds(3f);
        }
    }

    public void PauseForSeconds(float seconds)
    {
        StartCoroutine(PauseCoroutine(seconds));
    }

    private IEnumerator PauseCoroutine(float seconds)
    {
        isPaused = true;
        yield return new WaitForSeconds(seconds);
        if (currentState == State.blackMoving)
        {
            currentState = State.whiteDice;
        }
        else if (currentState == State.whiteMoving)
        {
            currentState = State.blackDice;
        }
        isPaused = false;
    }

    private bool isPaused = false;

    // 更新双方结束情况
    private void updateCanFinish()
    {
        int blackFinishMax = -1;
        int whiteFinishMax = -1;
        if (currentState == State.blackMoving)
        {
            // 判断黑棋
            int sumBF = 0;
            for (int i = 18; i < 24; i++) if(PC.piecesArray[i]>0) sumBF += PC.piecesArray[i];
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
            for (int i = 0; i < 6; i++) if(PC.piecesArray[i]<0) sumWF -= PC.piecesArray[i];
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
                for (int i = 0; i < 4; i++)
                    if (randomNum[i] > whiteFinishMax)
                        randomNum[i] = whiteFinishMax;
            }
            else whiteCanFinish = false;
        }
    }

    private void updateNumText(bool isSkipTurn = false)
    {
        if (!isSkipTurn)
        {
            //if (currentState == State.blackDice || currentState == State.blackMoving)
            //{
            //    blackTextBox.text = randomNum[0] + " " + randomNum[1] + " " + randomNum[2] + " " + randomNum[3];
            //}
            //else if (currentState == State.whiteDice || currentState == State.whiteMoving)
            //{
            //    whiteTextBox.text = randomNum[0] + " " + randomNum[1] + " " + randomNum[2] + " " + randomNum[3];
            //}
        }
        else
        {
            if (currentState == State.blackDice || currentState == State.blackMoving)
            {
                blackTextBox.text = "Skip";
                Invoke("DisBlack", 5.0f);
            }
            else if (currentState == State.whiteDice || currentState == State.whiteMoving)
            {
                whiteTextBox.text = "Skip";
                Invoke("DisWhite", 5.0f);
            }
        }
    }

    void DisBlack()
    {
        blackTextBox.text = "";
        for(int i = 0;i < 4; i++)
        {
            if (DiceCons[0, i].transform.position.y > 0)
            {
                DiceCons[0, i].Disappear();
            }
        }
    }

    void DisWhite()
    {
        whiteTextBox.text = "";
        for (int i = 0; i < 4; i++)
        {
            if (DiceCons[1, i].transform.position.y > 0)
            {
                DiceCons[1, i].Disappear();
            }
        }
    }

    // 执行的起点，响应鼠标点击事件
    public void ReceiveMouseDown(int index)
    {
        if (isPaused || isGameOver) return;
        if (moveI2J.Count == 0)
        {
            // 第一步
            if (PC.piecesArray[index] == 0 || index == 24 || index == 25) return;
            if (currentState == State.blackMoving && PC.piecesArray[index] < 0) return;
            if (currentState == State.whiteMoving && PC.piecesArray[index] > 0) return;
            moveI2J.Add(index);
            updateCanFinish();
            // 判断在起点有被吃子
            if (PC.piecesArray[logic2world(0)] != 0 && index != logic2world(0))
            {
                PC.shakeDied(logic2world(0));
                moveI2J.RemoveAt(0);
                return;
            }
            PC.pickupAPiece(index);
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
                // 骰子挪到地下
                int dIndex = -1;
                if (currentState == State.blackMoving) dIndex = 0;
                else dIndex = 1;
                int ii = -1;
                if (i == 0) ii = 0;
                else if (i == 1) ii = 2;
                else if (i == 2) ii = 1;
                else if (i == 3) ii = 3;
                DiceCons[dIndex, ii].Disappear();
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
        int pa = PC.piecesArray[logic2world(currentPos)];
        bool canMove = false;
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