//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2022-05-16 16:20:14.768
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
    /// 实体表。
    /// </summary>
    public class DRHero : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取实体编号。
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
        /// 获取角色描述。
        /// </summary>
        public string Desc
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取购买需要金币。
        /// </summary>
        public int Money
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取永久死亡，需要重新购买。
        /// </summary>
        public bool Death
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取金币倍数。
        /// </summary>
        public int MoneyRatio
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取实体。
        /// </summary>
        public int Entity
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取图标。
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
            Desc = columnStrings[index++];
            Money = int.Parse(columnStrings[index++]);
            Death = bool.Parse(columnStrings[index++]);
            MoneyRatio = int.Parse(columnStrings[index++]);
            Entity = int.Parse(columnStrings[index++]);
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
                    Desc = binaryReader.ReadString();
                    Money = binaryReader.Read7BitEncodedInt32();
                    Death = binaryReader.ReadBoolean();
                    MoneyRatio = binaryReader.Read7BitEncodedInt32();
                    Entity = binaryReader.Read7BitEncodedInt32();
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
