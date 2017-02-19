using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Leayal.SoulWorker
{
    public static class Resource
    {
        private const Int32 MD5HashLength = 32;
        static public Dictionary<UInt64, List<string>> readTranslation(string filepath)
        {
            Dictionary<UInt64, List<string>> result;
            using (var s = new StreamReader(filepath))
                result = readTranslation(s);
            return result;
        }

        static public Dictionary<UInt64, List<string>> readTranslation(StreamReader reader)
        {
            Dictionary<UInt64, List<string>> theTranslation = new Dictionary<UInt64, List<string>>();
            List<string> currentNode = null;
            string bufferedString = null;
            UInt64 tmpIndex = 0;
                while (!reader.EndOfStream)
                {
                    bufferedString = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(bufferedString))
                {
                    if (bufferedString.ToLower().StartsWith("id="))
                    {
                        tmpIndex = UInt64.Parse(bufferedString.Remove(0, 3));
                        if (!theTranslation.ContainsKey(tmpIndex))
                        {
                            currentNode = new List<string>();
                            theTranslation.Add(tmpIndex, currentNode);
                        }
                        else
                            currentNode = theTranslation[tmpIndex];
                    }
                    else
                    {
                        if (currentNode != null)
                        {
                            if (bufferedString.IndexOf("\\n") > -1) //must be \\n, even with @string
                                bufferedString = bufferedString.Replace("\\n", "\n");
                            currentNode.Add(bufferedString);
                        }
                    }
                }
            }
            return theTranslation;
        }

        static public void MergeResource(Stream databaseStream, Stream originalFile, Stream outputFile, int idIndex, string CountByte, string formatstring)
        {
            Dictionary<UInt64, List<string>> result;
            using (var s = new StreamReader(databaseStream))
                result = readTranslation(s);
            MergeResource(result, originalFile, outputFile, idIndex, CountByte, formatstring);
        }

        static public void MergeResource(Dictionary<UInt64, List<string>> inputTable, Stream originalFile, Stream outputFile, int idIndex, string countFormat, string formatstring)
        {
            //. . . 5    4    2 1 len 2
            //     id  count   format

            string[] formatArray = formatstring.Split(' '); // skip idIndex and countFormat

            ulong dataCount = 0;
            ulong dataSum = 0;

            byte[] hash = new byte[MD5HashLength];
            int lineCount = 0;

            for (int i = 0; i < formatArray.Length; i++)
                if (formatArray[i] == "len")
                {
                    lineCount++;
                    i++;
                }            
            using (var br = new BinaryReader(originalFile))
            using (var bw = new BinaryWriter(outputFile))
            {
                switch (countFormat)
                {
                    case "1":
                        dataCount = br.ReadByte();
                        bw.Write(Convert.ToByte(dataCount));
                        break;
                    case "2":
                        dataCount = br.ReadUInt16();
                        bw.Write(Convert.ToUInt16(dataCount));
                        break;
                    case "4":
                        dataCount = br.ReadUInt32();
                        bw.Write(Convert.ToUInt32(dataCount));
                        break;
                    case "8":
                        dataCount = br.ReadUInt64();
                        bw.Write(Convert.ToUInt64(dataCount));
                        break;
                }
                ulong value = 0;

                for (ulong i = 0; i < dataCount; i++)
                {
                    #region Object Reading
                    object[] current = new object[formatArray.Length];
                    for (int j = 0; j < formatArray.Length; j++)
                    {
                        switch (formatArray[j])
                        {
                            case "1":
                                current[j] = Convert.ToByte(br.ReadByte());
                                break;
                            case "2":
                                current[j] = Convert.ToUInt16(br.ReadUInt16());
                                break;
                            case "4":
                                current[j] = Convert.ToUInt32(br.ReadUInt32());
                                break;
                            case "8":
                                current[j] = Convert.ToUInt64(br.ReadUInt64());
                                break;
                            case "len":
                                switch (formatArray[++j])
                                {
                                    case "1":
                                        value = br.ReadByte();
                                        current[j] = Convert.ToByte(br.ReadByte());
                                        break;
                                    case "2":
                                        value = br.ReadUInt16();
                                        current[j] = Convert.ToUInt16(value);
                                        break;
                                    case "4":
                                        value = br.ReadUInt32();
                                        current[j] = Convert.ToUInt32(value);
                                        break;
                                    case "8":
                                        value = br.ReadUInt64();
                                        current[j] = Convert.ToUInt64(value);
                                        break;
                                }
                                ulong strBytesLength = value * 2;
                                byte[] strBytes = new byte[strBytesLength];
                                current[j] = strBytes;

                                for (ulong k = 0; k < strBytesLength; k++)
                                    strBytes[k] = br.ReadByte();
                                break;
                        }
                    }
                    #endregion

                    #region Object Writing
                    int lenPosition = 0;
                    for (int j = 0; j < formatArray.Length; j++)
                    {
                        switch (formatArray[j])
                        {
                            case "1":
                                value = Convert.ToByte(current[j]);
                                bw.Write(Convert.ToByte(value));
                                break;
                            case "2":
                                value = Convert.ToUInt16(current[j]);
                                bw.Write(Convert.ToUInt16(value));
                                break;
                            case "4":
                                value = Convert.ToUInt32(current[j]);
                                bw.Write(Convert.ToUInt32(value));
                                break;
                            case "8":
                                value = Convert.ToUInt64(current[j]);
                                bw.Write(Convert.ToUInt64(value));
                                break;
                            case "len":
                                byte[] strBytes = null;
                                j++;
                                ulong id = Convert.ToUInt64(current[idIndex]);
                                if (inputTable.ContainsKey(id))
                                    strBytes = Encoding.Unicode.GetBytes(inputTable[id][lenPosition++]);
                                else
                                    strBytes = current[j] as byte[];
                                value = Convert.ToUInt64(strBytes.Length / 2);

                                switch (formatArray[j])
                                {
                                    case "1":
                                        bw.Write(Convert.ToByte(value));
                                        break;
                                    case "2":
                                        bw.Write(Convert.ToUInt16(value));
                                        break;
                                    case "4":
                                        bw.Write(Convert.ToUInt32(value));
                                        break;
                                    case "8":
                                        bw.Write(Convert.ToUInt64(value));
                                        break;
                                }

                                foreach (byte b in strBytes)
                                {
                                    dataSum += b;
                                    bw.Write(b);
                                }
                                break;
                        }

                        dataSum += value;
                    }
                    #endregion
                }

                bw.Write(MD5HashLength);
                string hashString = GetMD5(Convert.ToString(dataSum));
                for (int i = 0; i < MD5HashLength; i++)
                    hash[i] = Convert.ToByte(hashString[i]);
                bw.Write(hash);
            }
        }
        private static string GetMD5(string text)
        {
            using (var md5 = MD5.Create())
            {
                byte[] result = md5.ComputeHash(Encoding.ASCII.GetBytes(text));
                StringBuilder sb = new StringBuilder();

                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}
