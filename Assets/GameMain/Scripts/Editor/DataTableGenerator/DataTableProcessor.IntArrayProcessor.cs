//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.IO;

namespace Game.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class IntArrayProcessor : GenericDataProcessor<int[]>
        {
            public override bool IsSystem
            {
                get
                {
                    return true;
                }
            }

            public override string LanguageKeyword
            {
                get
                {
                    return "int[]";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "int[]",
                    "int32[]",
                    "system.int32[]"
                };
            }

            public override int[] Parse(string value)
            {
                string[] splitValue = value.Split(',');
                int[] result = new int[splitValue.Length];
                for (int i = 0; i < splitValue.Length; i++)
                {
                    result[i] = int.Parse(splitValue[i]);
                }

                return result;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                int[] floatArray = Parse(value);
                binaryWriter.Write(floatArray.Length);
                foreach (var elementValue in floatArray)
                {
                    binaryWriter.Write(elementValue);
                }
            }
        }
    }
}
