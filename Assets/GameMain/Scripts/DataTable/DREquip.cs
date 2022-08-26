//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2022-05-16 16:20:14.773
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
    /// 界面配置表。
    /// </summary>
    public class DREquip : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取界面编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public string AssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取对应的卡牌。
        /// </summary>
        public int Card
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取对应的buff。
        /// </summary>
        public int Buff
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取buff数值。
        /// </summary>
        public int BuffValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取。
        /// </summary>
        public string Icon
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
            AssetName = columnStrings[index++];
            Card = int.Parse(columnStrings[index++]);
            Buff = int.Parse(columnStrings[index++]);
            BuffValue = int.Parse(columnStrings[index++]);
            Icon = columnStrings[index++];

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
                    AssetName = binaryReader.ReadString();
                    Card = binaryReader.Read7BitEncodedInt32();
                    Buff = binaryReader.Read7BitEncodedInt32();
                    BuffValue = binaryReader.Read7BitEncodedInt32();
                    Icon = binaryReader.ReadString();
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
