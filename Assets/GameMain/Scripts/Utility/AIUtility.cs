//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{


    /// <summary>
    /// AI 工具类。
    /// </summary>
    public static class AIUtility
    {
        private static float[] m_Positons = { 4.55f, 10.55f, 16.55f, 22.55f, 28.55f };
        /// <summary>
        /// 获取车道索引根据x坐标
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static int GetRoadIndex(float x)
        {
            int index = 0;
            if (x < 7.55f)
            {
                index = 0;
            }
            else if (x < 13.55f)
            {
                index = 1;
            }
            else if (x < 19.55f)
            {
                index = 2;
            }
            else if (x < 25.55f)
            {
                index = 3;
            }
            else if (x < 31.55f)
            {
                index = 4;
            }
            return index;
        }

        public static bool IsBoundLeft(float x)
        {
            return x <= 4.55f;
        }
        public static bool IsBoundRight(float x)
        {
            return x >= 28.55f;
        }
        public static bool IsBound(float x)
        {
            return x <= 4.55 || x >= 28.55;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static float GetRoadPositon(int index)
        {
            return m_Positons[index];
        }

        /// <summary>
        /// Moves from "from" to "to" by the specified amount and returns the corresponding value
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="amount">Amount.</param>
        public static float Approach(float from, float to, float amount)
        {
            if (from < to)
            {
                from += amount;
                if (from > to)
                {
                    return to;
                }
            }
            else
            {
                from -= amount;
                if (from < to)
                {
                    return to;
                }
            }
            return from;
        }
    }
}
