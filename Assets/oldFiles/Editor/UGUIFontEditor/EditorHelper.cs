using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;


public class EditorHelper
{
    public const int SUIT_CHARACTER = 1;  //万牌
    public const int SUIT_DOT = 2;  // 筒牌
    public const int SUIT_BAMBOO = 3;   // 索牌
    public const int SUIT_WIND = 4; // 风牌
    public const int SUIT_DRAGON = 5;   // 箭牌
    public const int SUIT_FLOWER = 6;   // 花牌
                                        //保存玩家的手牌信息
    public MahjongGame_AH.MahjongHelper.ResultType resultType = new MahjongGame_AH.MahjongHelper.ResultType();
    [NUnit.Framework.Test]
    public static void  GetAddress()
    {
        int[,] matrix = 
            {{ 11, 3, 3, 3, 0, 1, 0, 1, 0, 0 },
             { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
              { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
              { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
             { 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 }
    }; int row=1; int col=0;
        int pos = (matrix.GetUpperBound(0) + 1) * row + col;
        Debug.Log("pos:" + pos);
        IntPtr ptr = System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(matrix, pos);
        Debug.Log(ptr.ToInt32());
      // Debug.Log( System.Runtime.InteropServices.Marshal.PtrToStructure(ptr, typeof(byte[10])));
    }

    [NUnit.Framework.Test]
    public void DobouleByte2Single()
    {

        byte[,] value = {
    { 11, 3, 3, 3, 0, 1, 0, 1, 0,0},
    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    { 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 } };
        bool outbool = JudgeNormalWin(value, 5, 1);
        Debug.Log("是否可胡：" + outbool);
    }

    //======================================  红中麻将======================================================
    /**
     * 判断是否普通胡牌
     * @param byaTileNum 各种花色的牌各个值的张数，一维的5代表花色；二维的10的0位置保存这个花色的总张数，1～9位置保存对应值的牌张数
     * @param resultType 结果信息
     *  癞子的花色
     *   癞子的牌值@return 是否胡牌
     */
    public bool JudgeNormalWin(byte[,] byaTileNum, byte byLaiziSuit, byte byLaiziValue)
    {
        //byaTileNum=new byte [5, 10];
        byte bySuit = 0;
        byte byValue = 0;
        byte byChangeValue = 0; // 变化的牌值


        byte byLaiziNum = 0; //癞子牌数量
        byte byEmployLaiZiNum = 0; //使用癞子的数量
        byte byIndex = 0; // 当前牌的下标
        byte byRemainingNum = 0; // 去除正常顺子刻子后剩余的牌
        byte[] byaTiles = new byte[14]; // 用于保存需要癞子的牌,包括将牌
        byte[,] byaSaveTileNum = byaTileNum; // 用于保存手牌
        bool bSuccess = false; // 能否胡牌
        bool bJong = false; // 将是否存在
        bool bJongTile = false;//奖牌是否找到
        byte byRemainder = 0; // 余数
        byte byJongSuit = 0; // 将牌的花色
        byte byJongValue = 0; // 将牌的牌值
                              //memset(byaTiles, 0, byaTiles.Length );
                              //保存手牌
                              //memset(byaSaveTileNum, 0, byaSaveTileNum.Length );

        // memcpy(byaSaveTileNum, byaTileNum, byaSaveTileNum.Length );
        //计算癞子的数量
        // 是否满足33332模型 // 这里考虑没有癞子时候 要先找出将牌
        if (byLaiziSuit < 1)
        {
            Debug.LogError("byLaiziSuit:" + byLaiziSuit);
            return false;
        }
        byLaiziNum = byaSaveTileNum[byLaiziSuit - 1, byLaiziValue];
        byaSaveTileNum[byLaiziSuit - 1, byLaiziValue] = 0;
        byaSaveTileNum[byLaiziSuit - 1, 0] -= byLaiziNum;

        for (bySuit = SUIT_CHARACTER; bySuit < SUIT_FLOWER; bySuit++) // 5种花色
        {
            byRemainder = (byte)(byaSaveTileNum[bySuit - 1, 0] % 3);
            if (byLaiziNum == 0)
            {
                if (byRemainder == 1)
                {
                    Debug.LogError("MahjongHelper::JudgeNormalWin A");
                    return false;
                }
                else if (byRemainder == 2)
                {
                    if (bJong)
                    {
                        Debug.LogError("MahjongHelper::JudgeNormalWin B");
                        return false;
                    }
                    byJongSuit = bySuit;
                    bJong = true;
                }
            }
            else if (byLaiziNum > 0)
            {
                if (byRemainder == 1)
                {
                    continue;
                }
                else if (byRemainder == 2)
                {
                    if (bySuit < SUIT_WIND)
                    {
                        for (byValue = 1; byValue < 10; byValue++)
                        {
                            if (byValue < 8 && ((byaSaveTileNum[bySuit - 1, byValue] == 3 && byaSaveTileNum[bySuit - 1, byValue + 1] == 1 && byaSaveTileNum[bySuit - 1, byValue + 2] == 1)
                                || (byaSaveTileNum[bySuit - 1, byValue] == 1 && byaSaveTileNum[bySuit - 1, byValue + 1] == 1 && byaSaveTileNum[bySuit - 1, byValue + 2] == 3)))
                            {
                                if (bJong)
                                {
                                    break;
                                }
                                byJongSuit = bySuit;
                                bJong = true;
                            }
                        }
                    }
                }
            }
        }
        Debug.LogWarningFormat("MahjongHelper::JudgeNormalWin,癞子牌{0},{1}数量{2}", byLaiziSuit, byLaiziValue, byLaiziNum);
        //去除不许要癞子成扑的牌
        for (bySuit = SUIT_CHARACTER; bySuit < SUIT_FLOWER; bySuit++)
        {
            if (bJong && byJongSuit == bySuit)
            {
                for (byValue = 1; byValue < 10; byValue++)
                {
                    if (!bJongTile && byaSaveTileNum[bySuit - 1, byValue] >= 2)
                    {
                        //memset(&tempResult, 0, sizeof(tempResult));
                        // memcpy(&tempResult, &resultType, sizeof(ResultTypeDef));
                        // 除去2张将牌
                        byaSaveTileNum[byJongSuit - 1, byValue] -= 2;
                        byaSaveTileNum[byJongSuit - 1, 0] -= 2;
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                       
                        if (AnalyzeSuit(bySuit > SUIT_BAMBOO ? true : false, byJongSuit))
                        {
                            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                            byaTiles[byRemainingNum] = (byte)(byJongSuit << 4 | byValue);
                            byRemainingNum++;
                            byaTiles[byRemainingNum] = (byte)(byJongSuit << 4 | byValue);
                            byRemainingNum++;
                            byJongValue = byValue;
                            bJongTile = true;
                        }
                        else
                        {
                            // 还原2张将牌
                            byaSaveTileNum[byJongSuit - 1, byValue] += 2;
                            byaSaveTileNum[byJongSuit - 1, 0] += 2;
                        }
                    }
                }
            }
            Debug.Log("==================22222 bySuit：" + bySuit);
            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
            AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);
            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
        }

        // Debug.LogWarningFormat("MahjongHelper::JudgeNormalWin 万总张数{0}:{1}{2}{3}{4}{5}{6}{7}{8}{9}",byaSaveTileNum[0][0],byaSaveTileNum[0][1] ,byaSaveTileNum[0][2],byaSaveTileNum[0][3],byaSaveTileNum[0][4],byaSaveTileNum[0][5],byaSaveTileNum[0][6],byaSaveTileNum[0][7],byaSaveTileNum[0][8],byaSaveTileNum[0][9]);
        //Debug.LogWarningFormat("MahjongHelper::JudgeNormalWin 筒总张数[%d]：" + byaSaveTileNum[1][0] + " " + byaSaveTileNum[1][1] + " " + byaSaveTileNum[1][2] + " " + byaSaveTileNum[1][3] + " " + byaSaveTileNum[1][4] + " " +
        //    byaSaveTileNum[1][5] + " " + byaSaveTileNum[1][6] + " " + byaSaveTileNum[1][7] + " " + byaSaveTileNum[1][8], byaSaveTileNum[1][9]);
        //Debug.LogWarningFormat("MahjongHelper::JudgeNormalWin 索总张数[%d]：" + byaSaveTileNum[2][0] + " " + byaSaveTileNum[2][1] + " " + byaSaveTileNum[2][2] + " " + byaSaveTileNum[2][3] + " " + byaSaveTileNum[2][4] + " " +
        ////   byaSaveTileNum[2][5] + " " + byaSaveTileNum[2][6] + " " + byaSaveTileNum[2][7] + " " + byaSaveTileNum[2][8] + " " + byaSaveTileNum[2][9]);
        //Debug.LogWarningFormat("MahjongHelper::JudgeNormalWin 风总张数[%d]：" + byaSaveTileNum[3][0] + " " + byaSaveTileNum[3][1] + " " + byaSaveTileNum[3][2] + " " + byaSaveTileNum[3][3] + " " + byaSaveTileNum[3][4] + " " +
        //   byaSaveTileNum[3][5] + " " + byaSaveTileNum[3][6] + " " + byaSaveTileNum[3][7] + " " + byaSaveTileNum[3][8] + " " + byaSaveTileNum[3][9]);
        //Debug.LogWarningFormat("MahjongHelper::JudgeNormalWin 箭总张数[%d]：" + byaSaveTileNum[4][0] + " " + byaSaveTileNum[4][1] + " " + byaSaveTileNum[4][2] + " " + byaSaveTileNum[4][3] + " " + byaSaveTileNum[4][4] + " " +
        //   byaSaveTileNum[4][5] + " " + byaSaveTileNum[4][6] + " " + byaSaveTileNum[4][7] + " " + byaSaveTileNum[4][8] + " " + byaSaveTileNum[4][9]);


        // 将剩余的牌加入数组中
        for (bySuit = SUIT_CHARACTER; bySuit < SUIT_FLOWER; bySuit++)
        {
            if (byaSaveTileNum[bySuit - 1, 0] == 0)
            {
                continue;
            }
            for (byValue = 1; byValue < 10; byValue++)
            {
                if (byaSaveTileNum[bySuit - 1, byValue] > 0)
                {
                    byaSaveTileNum[bySuit - 1, byValue]--;
                    byaSaveTileNum[bySuit - 1, 0]--;
                    byaTiles[byRemainingNum] = (byte)(bySuit << 4 | byValue);
                    byRemainingNum++;
                    byValue--;
                }
            }
        }
        Debug.LogWarningFormat("byRemainingNum:{0}\n", byRemainingNum);
        Debug.LogWarningFormat("byaTiles:{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}\n", byaTiles[0], byaTiles[1], byaTiles[2], byaTiles[3], byaTiles[4], byaTiles[5], byaTiles[6], byaTiles[7]);
        //分析需要癞子的牌
        if (byLaiziNum == 0)    // 没有癞子或者不玩癞子
        {
            if (byRemainingNum == 2 && byaTiles[0] == byaTiles[1] && bJong)
            {
                bSuccess = true;
                bySuit = (byte)(byaTiles[0] >> 4);
                byValue = (byte)(byaTiles[0] & 0x0F);
                byaSaveTileNum[bySuit - 1, 0] += 2;
                byaSaveTileNum[bySuit - 1, byValue] += 2;
                //增加结果信息
                byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
                SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
            }
        }
        else
        {
            for (int i = 0; i < byLaiziNum + 1; i++)
            {
                bySuit = (byte)(byaTiles[byIndex] >> 4);
                byValue = (byte)(byaTiles[byIndex] & 0x0F);
                if (byRemainingNum == 0)// 剩下的牌为 0
                {
                    // 癞子总数减去已使用后数为 3 并且 将牌已确定  （这里就组成 红中刻子）(可以胡牌 直接结束)
                    if (byLaiziNum - byEmployLaiZiNum == 3 && bJong)
                    {
                        bSuccess = true;
                        byaSaveTileNum[byLaiziSuit - 1, 0] += byLaiziNum;
                        byaSaveTileNum[byLaiziSuit - 1, byLaiziValue] += byLaiziNum;
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, byLaiziSuit - 1);
                        AddResult(byLaiziSuit, byLaiziValue, byLaiziSuit, byLaiziValue, 1, false);
                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, byLaiziSuit - 1);
                        break;
                    }
                    // 癞子总数减去已使用后数为2 并且 将牌没有确定 （这里就组成 将牌 （红中））（可以胡牌 直接结束）
                    else if (byLaiziNum - byEmployLaiZiNum == 2 && !bJong)
                    {
                        bJong = true;
                        bSuccess = true;
                        byaSaveTileNum[byLaiziSuit - 1, 0] += 2;
                        byaSaveTileNum[byLaiziSuit - 1, byLaiziValue] += 2;
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, byLaiziSuit - 1);
                        AddResult(byLaiziSuit, byLaiziValue, byLaiziSuit, byLaiziValue, 3, false);
                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, byLaiziSuit - 1);
                        break;
                    }
                    else if (byLaiziNum - byEmployLaiZiNum != 0) //  癞子总数减去已使用后数不为 0 直接退出 没有胡牌
                    {
                        Debug.LogError("MahjongHelper::JudgeNormalWin C");
                        return false;
                    }
                }
                else if (byRemainingNum == 1)
                {
                    // 癞子总数减去已使用后数为 1 并且将牌没有确定 （这里就组成将牌） （可以胡牌 直接结束）
                    if (byLaiziNum - byEmployLaiZiNum == 1 && !bJong)
                    {
                        bJong = true;
                        bSuccess = true;
                        byChangeValue = byValue;
                        byaSaveTileNum[bySuit - 1, 0] += 1;
                        byaSaveTileNum[bySuit - 1, byValue] += 1;
                        //增加结果信息
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                        AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, true, bySuit, byChangeValue, 1);
                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                        byEmployLaiZiNum += 1;
                        byRemainingNum -= 1;
                        break;
                    }
                    // 癞子总数减去已使用后数为 2 并且将牌已确定 （这里就组成刻子） （可以胡牌 直接结束）
                    else if (byLaiziNum - byEmployLaiZiNum == 2 && bJong)
                    {
                        bSuccess = true;
                        byChangeValue = byValue;
                        byaSaveTileNum[bySuit - 1, 0] += 1;
                        byaSaveTileNum[bySuit - 1, byValue] += 1;
                        //增加结果信息
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                        AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 1, true, bySuit, byChangeValue, 2);
                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                        byEmployLaiZiNum += 2;
                        byRemainingNum -= 1;
                        break;
                    }
                    else
                    {
                        Debug.LogError("MahjongHelper::JudgeNormalWin D");
                        return false;
                    }
                }
                // 剩下的牌数为 2
                else if (byRemainingNum == 2)
                {
                    if (byaTiles[byIndex] == byaTiles[byIndex + 1] && !bJong)
                    {
                        bJong = true;
                        bSuccess = true;
                        byaSaveTileNum[bySuit - 1, 0] += 2;
                        byaSaveTileNum[bySuit - 1, byValue] += 2;
                        //增加结果信息
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                        AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                        byRemainingNum -= 2;
                        byIndex += 2;
                    }
                    // 癞子总数减去已使用后数大于0 如果两张相同 并且有将牌 （这里就组成刻字牌）
                    else if (byLaiziNum - byEmployLaiZiNum > 0 && byaTiles[byIndex] == byaTiles[byIndex + 1] && bJong)
                    {
                        bSuccess = true;
                        byChangeValue = byValue;
                        byaSaveTileNum[bySuit - 1, 0] += 2;
                        byaSaveTileNum[bySuit - 1, byValue] += 2;
                        //增加结果信息
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                        AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 1, true, bySuit, byChangeValue, 1);
                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                        byRemainingNum -= 2;
                        byIndex += 2;
                        byEmployLaiZiNum += 1;
                    }
                    /*else if (byaTiles[byIndex] == byaTiles[byIndex + 1] && !bJong)
                     {
                     bJong = true;
                     bSuccess = true;
                     byaSaveTileNum[bySuit - 1,0] += 2;
                     byaSaveTileNum[bySuit - 1,byValue] += 2;
                     AddResult(resultType, byaSaveTileNum[bySuit - 1], bySuit,byValue, 3, false);
                     byRemainingNum -= 2;
                     byIndex += 2;
                     break;
                     }*/
                    else if (byaTiles[byIndex + 1] - byaTiles[byIndex] < 0x03 && bySuit < SUIT_WIND)
                    {
                        bool bHu = false;
                        if (byLaiziNum - byEmployLaiZiNum == 1 && bJong) // 癞子总数减去已使用后数为 2 并且将牌已确定 可以胡牌
                        {
                            bSuccess = true;
                            bHu = true;
                        }
                        //增加结果信息
                        byChangeValue = (byte)(byValue + 0x03 - (byaTiles[byIndex + 1] - byaTiles[byIndex]));
                        byaSaveTileNum[bySuit - 1, 0] += 2;
                        byaSaveTileNum[bySuit - 1, byValue] += 1;
                        byaSaveTileNum[bySuit - 1, byValue + 1] += 1;
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                        AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, true, bySuit, byChangeValue, 1);
                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                        byRemainingNum -= 2;
                        byIndex += 2;
                        byEmployLaiZiNum += 1;
                        if (bHu) // 可以胡牌直接結束
                        {
                            break;
                        }
                    }
                    else if (byLaiziNum - byEmployLaiZiNum == 3 && !bJong) // 癞子总数减去已使用后数为 3  并且将牌没有确定 （这里就组成顺子）
                    {
                        byChangeValue = byValue;
                        byRemainingNum -= 1;
                        byaSaveTileNum[bySuit - 1, 0] += 1;
                        byaSaveTileNum[bySuit - 1, byValue] += 1;
                        byIndex++;
                        //增加结果信息
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                        AddResult(byLaiziSuit, byLaiziValue, bySuit, (byte)(byValue == 8 ? byValue - 1 : byValue), 2, true, bySuit, byChangeValue, 2);
                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                        byEmployLaiZiNum += 2;
                    }
                    else
                    {
                        Debug.LogError("MahjongHelper::JudgeNormalWin E");
                        return false;
                    }
                }
                else if (byRemainingNum > 2)
                {
                    if (bJong && byJongSuit == bySuit && byJongValue == byValue)
                    {
                        //增加结果信息
                        byaSaveTileNum[bySuit - 1, 0] += 2;
                        byaSaveTileNum[bySuit - 1, byValue] += 2;
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                        AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                        byRemainingNum -= 2;
                        byIndex += 2;
                    }
                    //  剩下的牌减去2张将加上剩余的癞子数大于2  并且两张牌相同 将牌没有确定 （这里就组成将牌）
                    else if (((byRemainingNum - 2) + (byLaiziNum - byEmployLaiZiNum)) > 2 && byaTiles[byIndex] == byaTiles[byIndex + 1] && !bJong)
                    {
                        bJong = true;
                        //增加结果信息
                        byaSaveTileNum[bySuit - 1, 0] += 2;
                        byaSaveTileNum[bySuit - 1, byValue] += 2;
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                        AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);

                        byRemainingNum -= 2;
                        byIndex += 2;
                    }
                    // 癞子总数减去已使用后数大于 0 并且将牌已确定 （这里就组成刻子）
                    else if (byLaiziNum - (byEmployLaiZiNum + 1) > 0 && byaTiles[byIndex] == byaTiles[byIndex + 1] && bJong)
                    {
                        //增加结果信息
                        byChangeValue = byValue;
                        byaSaveTileNum[bySuit - 1, 0] += 2;
                        byaSaveTileNum[bySuit - 1, byValue] += 2;
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                        AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 1, true, bySuit, byChangeValue, 1);
                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                        byRemainingNum -= 2;
                        byIndex += 2;
                        byEmployLaiZiNum += 1;
                    }
                    // 癞子总数减去已使用后数大于 0 后面的牌值减前面的牌值 小于 3 并且是序数牌  (这里组成顺子)
                    else if (byLaiziNum - (byEmployLaiZiNum + 1) > 0 && byaTiles[byIndex + 1] - byaTiles[byIndex] < 0x03 && bySuit < SUIT_WIND)
                    {
                        //增加结果信息
                        byChangeValue = (byte)(byValue + 0x03 - (byaTiles[byIndex + 1] - byaTiles[byIndex]));
                        byaSaveTileNum[bySuit - 1, 0] += 2;
                        byaSaveTileNum[bySuit - 1, byValue] += 1;
                        byaSaveTileNum[bySuit - 1, byValue + 1] += 1;
                        byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                        AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, true, bySuit, byChangeValue, 1);
                        SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                        byRemainingNum -= 2;
                        byIndex += 2;
                        byEmployLaiZiNum += 1;
                    }
                    else if (byRemainingNum - 2 == 1 && byLaiziNum - byEmployLaiZiNum == 2 && !bJong) // 剩下的牌加剩下的癞子牌一共5张 ，剩下的牌减去2张将牌等于 1 癞子总数减去已使用后数等于2 并且将牌没有确定
                    {
                        bool flag = false;
                        int k = 0;
                        for (k = 0; k < 13; k++)
                        {
                            if (byaTiles[k] > 0 && byaTiles[k] == byaTiles[k + 1]) // 找到两张相同的牌
                            {
                                bJong = true;
                                //增加结果信息
                                bySuit = (byte)(byaTiles[k] >> 4);
                                byValue = (byte)(byaTiles[k] & 0x0F);
                                byaSaveTileNum[bySuit - 1, 0] += 2;
                                byaSaveTileNum[bySuit - 1, byValue] += 2;
                                byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                                AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, false);
                                SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                                byRemainingNum -= 2;
                                flag = true;
                                break;
                            }
                        }
                        if (!flag && ((byRemainingNum - 1) + (byLaiziNum - (byEmployLaiZiNum + 1))) > 2 && !bJong) // 剩下的牌减去1张将加上剩余的癞子数大于2 并且将牌没有确定 （这里组成将牌）
                        {
                            bJong = true;
                            byChangeValue = byValue;
                            byaSaveTileNum[bySuit - 1, 0] += 1;
                            byaSaveTileNum[bySuit - 1, byValue] += 1;
                            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, true, bySuit, byChangeValue, 1);
                            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                            byRemainingNum--;
                            byIndex++;
                            byEmployLaiZiNum += 1;
                        }
                        else if (flag)
                        {
                            //TODO 这里不能直接结束  还需要继续组合
                        }
                        else
                        {
                            Debug.LogError("MahjongHelper::JudgeNormalWin  F111");
                            return false;
                        }
                    }
                    else
                    {
                        // 剩下的牌减去1张将加上剩余的癞子数大于2 并且将牌没有确定 （这里组成将牌）
                        if (((byRemainingNum - 1) + (byLaiziNum - (byEmployLaiZiNum + 1))) > 2 && !bJong)
                        {
                            bJong = true;
                            byChangeValue = byValue;
                            byaSaveTileNum[bySuit - 1, 0] += 1;
                            byaSaveTileNum[bySuit - 1, byValue] += 1;
                            byaSuitTileNum = DobouleByte2Single(byaSaveTileNum, bySuit - 1);
                            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 3, true, bySuit, byChangeValue, 1);
                            SingleByte2Doboule(ref byaSaveTileNum, byaSuitTileNum, bySuit - 1);
                            byRemainingNum--;
                            byIndex++;
                            byEmployLaiZiNum += 1;
                        }
                        else
                        {
                            Debug.Log("MahjongHelper::JudgeNormalWin G");
                            return false;
                        }

                    }
                }
            }
        }
        if (bJong == false)
        {
            Debug.LogError("MahjongHelper::JudgeNormalWin 没有找到将牌");
            bSuccess = false;
        }
        for (bySuit = SUIT_CHARACTER; bySuit < SUIT_FLOWER; bySuit++)
        {
            if (byaSaveTileNum[bySuit - 1, 0] > 0)
            {
                bSuccess = false;
                Debug.LogError("MahjongHelper::JudgeNormalWin 分析牌失败");
                break;
            }
        }
        Debug.LogWarning ("MahjongHelper::JudgeNormalWinb Success=" + bSuccess + "bJong= " + bJong);
        return bSuccess;
    }
    bool AnalyzeSuit(bool bHonorTile, byte bySuit, bool isSaveCardMessage = false)
    {
        //if (bySuit == 1)
        //{
        //    Debug.LogError("wwwvalue:" + byaSuitTileNum[0] + "," + byaSuitTileNum[1] + "," + byaSuitTileNum[2] + "," + byaSuitTileNum[3] + "," + byaSuitTileNum[4] + "," + byaSuitTileNum[5] + "," + byaSuitTileNum[6]
        //        + "," + byaSuitTileNum[7] + "," + byaSuitTileNum[8] + "," + byaSuitTileNum[9]);
        //}
        if (bySuit > 3)
        {
            bHonorTile = true;
        }

        if (byaSuitTileNum[0] == 0)
        {
            return true;
        }

        //寻找最小的值
        byte byValue = 0;

        for (byValue = 1; byValue < 10; byValue++)
        {
            if (byaSuitTileNum[byValue] > 0)
            {
                break;
            }
        }

        //能否分析成功
        bool bSuccess = false;

        //分析刻子
        if (byaSuitTileNum[byValue] > 2)
        {
            //出去刻子的3张牌
            byaSuitTileNum[byValue] -= 3;
            byaSuitTileNum[0] -= 3;

            if (isSaveCardMessage)
            {
                //添加玩家的刻子信息
                MahjongGame_AH.MahjongHelper.TripletTypeDef seq = new MahjongGame_AH.MahjongHelper.TripletTypeDef();
                seq.bySuit = bySuit;
                seq.byValue = byValue;
                resultType.tripletType.Add(seq);
            }

            //分析剩余的牌
            bSuccess = AnalyzeSuit(bHonorTile, bySuit, isSaveCardMessage);

            //还原刻子的三张牌
            byaSuitTileNum[byValue] += 3;
            byaSuitTileNum[0] += 3;

            return bSuccess;
        }

        //分析顺子
        if (!bHonorTile && byValue < 8 && (byaSuitTileNum[byValue + 1] > 0) && (byaSuitTileNum[byValue + 2] > 0))
        {
            //是否限制3以下序数牌成牌,0不限值，1限制
            //if (GameData.Instance.PlayerPlayingPanelData.playingMethodConf.byUnderThreeLimit == 1)
            //{
            //    if (byValue < 4)
            //    {
            //        return false;
            //    }
            //}

            //出去三张顺子的3张牌
            byaSuitTileNum[byValue]--;
            byaSuitTileNum[byValue + 1]--;
            byaSuitTileNum[byValue + 2]--;
            byaSuitTileNum[0] -= 3;


            //分析剩余的牌
            bSuccess = AnalyzeSuit(bHonorTile, bySuit, isSaveCardMessage);

            if (isSaveCardMessage)
            {
                //添加玩家的顺子信息,如果列表已经包含该顺子信息，则不会继续添加   
                MahjongGame_AH.MahjongHelper.SequenceTypeDef seq = new MahjongGame_AH.MahjongHelper.SequenceTypeDef();
                seq.bySuit = bySuit;
                seq.byFirstValue = byValue;
                resultType.sequenceType.Add(seq);
            }

            //还原顺子的3张牌
            byaSuitTileNum[byValue]++;
            byaSuitTileNum[byValue + 1]++;
            byaSuitTileNum[byValue + 2]++;
            byaSuitTileNum[0] += 3;
            return bSuccess;
        }

        return false;
    }
    /**
 * 分析一种花色的牌，分解为刻和顺的组合
 * @param byaSuitTileNum 一种花色的牌各个值的张数
 * @param bySuit 花色：1万牌、2筒牌、3索牌、4风牌、5箭牌
 * @param byLaiziSuit 癞子花色
 * @param byLaiziValue 癞子牌值
 */
    byte[] byaSuitTileNum = new byte[10];
    void AnalyzeSuit(byte bySuit, byte byLaiziSuit, byte byLaiziValue, int Methord)
    {

        Debug.Log("==================11byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
        if (byaSuitTileNum[0] == 0) // 这种花色总数为0
        {
            return;
        }
        // 寻找最小的值
        byte byValue = 0;
        for (byValue = 1; byValue < 10; byValue++) // 各个值
        {
            if (byaSuitTileNum[byValue] > 0)
            {
                Debug.Log("==================11111 bySuit：" + bySuit);
                break;
            }
        }

        // 分析刻子
        if (byaSuitTileNum[byValue] > 2)
        {
            //增加结果信息
            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 1, false);

            // 分析剩余的牌
            AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);
            Debug.Log("==================22byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
            return;
        }
        else if (bySuit < SUIT_WIND && byValue < 8 && byaSuitTileNum[byValue] == 2 && byaSuitTileNum[byValue + 1] > 1 && byaSuitTileNum[byValue + 2] > 1)
        {
            //增加结果信息
            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, false);
            //增加结果信息
            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, false);
            Debug.Log("==================33byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
            // 分析剩余的牌
            AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);

            return;
        }
        // 分析顺子
        else if (bySuit < SUIT_WIND && byValue < 8 && byaSuitTileNum[byValue] == 1 && byaSuitTileNum[byValue + 1] > 0 && byaSuitTileNum[byValue + 2] > 0)
        {
            //增加结果信息
            AddResult(byLaiziSuit, byLaiziValue, bySuit, byValue, 2, false);
            Debug.Log("==================44byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
            // 分析剩余的牌
            AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);

            return;
        }
        else if (byValue < 10)
        {
            byte byTileNum = 0;
            byTileNum = byaSuitTileNum[byValue];
            byaSuitTileNum[byValue] = 0;
            byaSuitTileNum[0] -= byTileNum;

            Debug.Log("==================55byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
            // 分析剩余的牌
            AnalyzeSuit(bySuit, byLaiziSuit, byLaiziValue, 20001);
            byaSuitTileNum[byValue] += byTileNum;
            byaSuitTileNum[0] += byTileNum;
        }
        Debug.Log("==================66byaSuitTileNum：" + byaSuitTileNum[0] + "," + byaSuitTileNum[1]);
        return;
    }
    /**
     * 去除手牌并增加结果信息
     * @param byaSuitTileNum 一种花色的牌各个值的张数
     * @param byLaiziSuit 癞子花色
     * @param byLaiziValue 癞子牌值
     * @param bySuit 添加的花色
     * @param byValue 添加的牌值：刻子为牌值，顺子为第一个的牌值
     * @param byChangeType 变化状态：1刻子，2顺子，3将牌
     * @param byLaizi 是否有癞子
     * @param byChangeSuit 变化花色
     * @param byChangeValue 变化牌值
     * @param byUseLaiziNum 使用癞子的数量
     * @return 无
     */
    void AddResult(byte byLaiziSuit, byte byLaiziValue, byte bySuit, byte byValue, byte byChangeType, bool byLaizi, byte byChangeSuit = 0, byte byChangeValue = 0, byte byUseLaiziNum = 0)
    {
        if (byChangeValue == 10 && byValue == 8)
        {
            byChangeValue = 7;
            byValue = 7;
        }
        if (byLaizi)
        {
            for (byte i = 0; i < byUseLaiziNum; i++)
            {
                byaSuitTileNum[byChangeValue]++;
                byaSuitTileNum[0]++;
            }
        }
        switch (byChangeType)
        {
            case 0:
                break;
            case 1:
                // 除去刻子的3张牌
                byaSuitTileNum[byValue] -= 3;
                byaSuitTileNum[0] -= 3;
                break;
            case 2:
                // 除去顺子的3张牌
                byaSuitTileNum[byValue]--;
                byaSuitTileNum[byValue + 1]--;
                byaSuitTileNum[byValue + 2]--;
                byaSuitTileNum[0] -= 3;
                break;
            case 3:
                // 除去将牌的2张牌
                byaSuitTileNum[byValue] -= 2;
                byaSuitTileNum[0] -= 2;
                break;
            default:
                break;
        }
    }

    byte[] DobouleByte2Single(byte[,] value, int n)
    {
        byte[] single = new byte[value.GetLength(1)];
        //  single = value.GetLongLength(n );
        for (int i = 0; i < single.Length; i++)
        {
            single[i] = value[n, i];
        }
        return single;
    }
    void SingleByte2Doboule(ref byte[,] value,byte [] single,int row)
    {
        for (int i = 0; i < value.GetLength(1); i++)
        {
            value[row, i] = single[i];
        }
    }

}
