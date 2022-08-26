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
        private sealed class FloatArrayProcessor : GenericDataProcessor<float[]>
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
                    return "float[]";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "float[]",
                    "single[]",
                    "system.single[]"
                };
            }

            public override float[] Parse(string value)
            {
                string[] splitValue = value.Split(',');
                float[] result = new float[splitValue.Length];
                for (int i = 0; i < splitValue.Length; i++)
                {
                    result[i] = float.Parse(splitValue[i]);
                }

                return result;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                float[] floatArray = Parse(value);
                binaryWriter.Write(floatArray.Length);
                foreach (var elementValue in floatArray)
                {
                    binaryWriter.Write(elementValue);
                }
            }
        }
    }
}
