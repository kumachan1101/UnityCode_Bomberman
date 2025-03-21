using System;
using System.Reflection;
using ExitGames.Client.Photon;
using UnityEngine;

public static class CustomTypes
{
    private static readonly byte[] memBomParameters;

    private static int BomParametersSize;

    static CustomTypes()
    {
        BomParametersSize = CalculateBomParametersSize(typeof(BomParameters));
        //Debug.Log(BomParametersSize);

        // BomParametersSizeを計算した後でmemBomParametersを初期化
        memBomParameters = new byte[BomParametersSize];
    }


    private static int CalculateBomParametersSize(Type type)
    {
        int totalSize = 0;

        // フィールドを取得
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            // フィールドの型に基づいてサイズを計算
            if (field.FieldType == typeof(int))
            {
                totalSize += sizeof(int);
            }
            else if (field.FieldType == typeof(float))
            {
                totalSize += sizeof(float);
            }
            else if (field.FieldType == typeof(bool))
            {
                totalSize += sizeof(bool);
            }
            else if (field.FieldType == typeof(Vector3))
            {
                totalSize += sizeof(float) * 3; // Vector3はfloatが3つ
            }
            else if (field.FieldType == typeof(string))
            {
                totalSize += sizeof(int); // 文字列の長さ（int型）
                totalSize += 256;         // 最大文字列サイズ（固定長）
            }
            else if (field.FieldType.IsEnum) // enum型の処理
            {
                Type underlyingType = Enum.GetUnderlyingType(field.FieldType); // enumの基底型を取得
                if (underlyingType == typeof(int))
                {
                    totalSize += sizeof(int);
                }
                else if (underlyingType == typeof(byte))
                {
                    totalSize += sizeof(byte);
                }
                else if (underlyingType == typeof(short))
                {
                    totalSize += sizeof(short);
                }
                else
                {
                    throw new NotSupportedException($"Unsupported enum underlying type: {underlyingType}");
                }
            }
            else
            {
                throw new NotSupportedException($"Unsupported field type: {field.FieldType}");
            }
        }

        return totalSize;
    }



    public static void Register()
    {
        PhotonPeer.RegisterType(typeof(BomParameters), (byte)'B', SerializeBomParameters, DeserializeBomParameters);
    }


    private static short SerializeBomParameters(StreamBuffer outStream, object customObject)
    {
        BomParameters bomParams = (BomParameters)customObject;
        lock (memBomParameters)
        {
            byte[] bytes = memBomParameters;
            int index = 0;

            Protocol.Serialize(bomParams.position.x, bytes, ref index);
            Protocol.Serialize(bomParams.position.y, bytes, ref index);
            Protocol.Serialize(bomParams.position.z, bytes, ref index);

            Protocol.Serialize((int)bomParams.bomKind, bytes, ref index);
            Protocol.Serialize(bomParams.viewID, bytes, ref index);
            Protocol.Serialize(bomParams.explosionNum, bytes, ref index);
            bytes[index++] = (byte)(bomParams.bomKick ? 1 : 0);

            byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(bomParams.materialType);
            Protocol.Serialize(stringBytes.Length, bytes, ref index);
            System.Buffer.BlockCopy(stringBytes, 0, bytes, index, stringBytes.Length);
            index += stringBytes.Length;

            bytes[index++] = (byte)(bomParams.bomAttack ? 1 : 0);

            Protocol.Serialize(bomParams.direction.x, bytes, ref index);
            Protocol.Serialize(bomParams.direction.y, bytes, ref index);
            Protocol.Serialize(bomParams.direction.z, bytes, ref index);

            outStream.Write(bytes, 0, index);
        }
        return (short)memBomParameters.Length;
    }

    private static object DeserializeBomParameters(StreamBuffer inStream, short length)
    {
        BomParameters bomParams = new BomParameters();
        lock (memBomParameters)
        {
            byte[] bytes = memBomParameters;
            inStream.Read(bytes, 0, length);
            int index = 0;

            float x = 0, y = 0, z = 0;
            Protocol.Deserialize(out x, bytes, ref index);
            Protocol.Deserialize(out y, bytes, ref index);
            Protocol.Deserialize(out z, bytes, ref index);
            bomParams.position = new Vector3(x, y, z);

            int bomKind = 0;
            Protocol.Deserialize(out bomKind, bytes, ref index);
            bomParams.bomKind = (BOM_KIND)bomKind;

            Protocol.Deserialize(out bomParams.viewID, bytes, ref index);
            Protocol.Deserialize(out bomParams.explosionNum, bytes, ref index);
            bomParams.bomKick = bytes[index++] == 1;

            int stringLength = 0;
            Protocol.Deserialize(out stringLength, bytes, ref index);
            byte[] stringBytes = new byte[stringLength];
            System.Buffer.BlockCopy(bytes, index, stringBytes, 0, stringLength);
            bomParams.materialType = System.Text.Encoding.UTF8.GetString(stringBytes);
            index += stringLength;

            bomParams.bomAttack = bytes[index++] == 1;

            Protocol.Deserialize(out x, bytes, ref index);
            Protocol.Deserialize(out y, bytes, ref index);
            Protocol.Deserialize(out z, bytes, ref index);
            bomParams.direction = new Vector3(x, y, z);
        }
        return bomParams;
    }
}
