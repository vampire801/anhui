using UnityEngine;
using System.Collections;


    public class BitOperate
    {
        //获取int某一位的值 从0开始
        public static int GetBitValue(int num,int index)
        {
            if (index < 0 || index > 32)
            {
                return -1;
            }

            int temp = num & (int)(Mathf.Pow(2, index));
            if(temp == Mathf.Pow(2, index))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
      
        //设置某一位的值  从1开始
        public static int SetBitValue(int num, int index,bool flag)
        {
            if (index < 1 || index > 32)
            {
                return 0;
            }

            int temp = index < 2 ? index : (2 << (index - 2));
            return flag ? (num | temp) : (num & ~temp);
        }
    }
