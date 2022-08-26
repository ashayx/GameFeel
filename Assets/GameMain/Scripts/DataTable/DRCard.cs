//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2022-05-16 16:20:14.763
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    /// <summary>
    /// 卡牌配置表。
    /// </summary>
    public class DRCard : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取特性名字。
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取优先级。
        /// </summary>
        public int Priority
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取攻击范围。
        /// </summary>
        public int Range
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取攻击伤害。
        /// </summary>
        public int Damage
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取描述。
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取花费。
        /// </summary>
        public int Cost
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取卡牌类型,1单体敌人2直接使用。
        /// </summary>
        public int Type
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取使用次数。
        /// </summary>
        public int Amount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取图片。
        /// </summary>
        public string Icon
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取特效。
        /// </summary>
        public int Effect
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取buff。
        /// </summary>
        public int[] BuffType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取buff持续回合。
        /// </summary>
        public int Round
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取buff数值。
        /// </summary>
        public int[] BuffValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取牌分类，1:攻击2：恢复3：闪电4:减益5：防御。
        /// </summary>
        public int Tag
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            index++;
            Name = columnStrings[index++];
            Priority = int.Parse(columnStrings[index++]);
            Range = int.Parse(columnStrings[index++]);
            Damage = int.Parse(columnStrings[index++]);
            Description = columnStrings[index++];
            Cost = int.Parse(columnStrings[index++]);
            Type = int.Parse(columnStrings[index++]);
            Amount = int.Parse(columnStrings[index++]);
            Icon = columnStrings[index++];
            Effect = int.Parse(columnStrings[index++]);
            BuffType = DataTableExtension.ParseInt32Array(columnStrings[index++]);
            Round = int.Parse(columnStrings[index++]);
            BuffValue = DataTableExtension.ParseInt32Array(columnStrings[index++]);
            Tag = int.Parse(columnStrings[index++]);

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.Read7BitEncodedInt32();
                    Name = binaryReader.ReadString();
                    Priority = binaryReader.Read7BitEncodedInt32();
                    Range = binaryReader.Read7BitEncodedInt32();
                    Damage = binaryReader.Read7BitEncodedInt32();
                    Description = binaryReader.ReadString();
                    Cost = binaryReader.Read7BitEncodedInt32();
                    Type = binaryReader.Read7BitEncodedInt32();
                    Amount = binaryReader.Read7BitEncodedInt32();
                    Icon = binaryReader.ReadString();
                    Effect = binaryReader.Read7BitEncodedInt32();
                    BuffType = binaryReader.ReadInt32Array();
                    Round = binaryReader.Read7BitEncodedInt32();
                    BuffValue = binaryReader.ReadInt32Array();
                    Tag = binaryReader.Read7BitEncodedInt32();
                }
            }

            GeneratePropertyArray();
            return true;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
