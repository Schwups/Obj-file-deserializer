using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Obj_Deserializer
{
    public class ObjDeserializer
    {
        public static Obj_Class.Obj Deserialize(string fileLocation)
        {
            Debug.WriteLine("Obj Deserializer:");
            Debug.Indent();
            try
            {
                if (fileLocation.Substring(fileLocation.LastIndexOf('.')).ToLower() != ".obj")
                {
                    Debug.WriteLine("Invalid Filetype");
                    throw new InvalidDataException($"File at location {fileLocation} is not a .obj file");
                }
                Obj_Class.Obj obj;
                using (StreamReader SR = new StreamReader(fileLocation))
                {
                    obj = new Obj_Class.Obj(fileLocation);

                    while (SR.Peek() != -1)
                    {
                        string currentLine = SR.ReadLine().Trim();
                        int IndexOfSpace = currentLine.IndexOf(' ');
                        if (IndexOfSpace == -1)
                        {
                            continue;
                        }
                        string key = currentLine.Substring(0, IndexOfSpace).ToLower();
                        currentLine = currentLine.Substring(currentLine.IndexOf(' ') + 1);

                        switch (key)
                        {
                            case "o":
                                {
                                    obj.SetName(currentLine);
                                    break;
                                }
                            case "v":
                                {
                                    string[] values = currentLine.Split(' ');
                                    if (values.Length != 3)
                                    {
                                        //invalid vertex
                                        break;
                                    }
                                    List<float> floatValues = ExtractFloats(values);
                                    if (floatValues == null)
                                    {
                                        //error while extracting floats
                                        break;
                                    }
                                    obj.AddVertex(floatValues[0], floatValues[1], floatValues[2]);
                                    break;
                                }
                            case "vt":
                                {
                                    string[] values = currentLine.Split(' ');
                                    if (values.Length != 2)
                                    {
                                        //invalid texture vertex
                                        break;
                                    }
                                    List<float> floatValues = ExtractFloats(values);
                                    if (floatValues == null)
                                    {
                                        //error while extracting floats
                                        break;
                                    }
                                    obj.AddTextureVertex(floatValues[0], floatValues[1]);
                                    break;
                                }
                            case "vn":
                                {
                                    string[] values = currentLine.Split(' ');
                                    if (values.Length != 3)
                                    {
                                        //invalid vertex normal
                                        break;
                                    }
                                    List<float> floatValues = ExtractFloats(values);
                                    if (floatValues == null)
                                    {
                                        //error while extracting floats
                                        break;
                                    }
                                    obj.AddVertexNormal(floatValues[0], floatValues[1], floatValues[2]);
                                    break;
                                }
                            case "f":
                                {
                                    string[] faces = currentLine.Split(' ');
                                    if (faces.Length != 3)
                                    {
                                        // not valid face
                                    }
                                    List<int[]> faceDataList = new List<int[]>();
                                    foreach (string s in faces)
                                    {
                                        string[] data = s.Split('/');
                                        if (data.Length != 3)
                                        {
                                            //invalid face
                                            break;
                                        }
                                        try
                                        {
                                            int[] intData = new int[3];
                                            for (int i = 0; i < data.Length; i++)
                                            {
                                                if (data[i] == null)
                                                {
                                                    intData[i] = -1;
                                                }
                                                else
                                                {
                                                    intData[i] = Convert.ToInt32(data[i]);
                                                }
                                            }
                                            if (intData.Length != 3)
                                            {
                                                //error while extracting faces
                                                Debug.WriteLine("error while extracting faces");
                                                Debug.Unindent();
                                                throw new ApplicationException("Error while extracting faces");
                                                //see comment in ExtractFloats Subroutine
                                            }
                                            faceDataList.Add(intData);
                                        }
                                        catch (FormatException)
                                        {
                                            //invalid int in face
                                            break;
                                        }
                                    }
                                    obj.CreateFace(faceDataList[0], faceDataList[1], faceDataList[2]);
                                    break;
                                }
                                List<float> ExtractFloats(string[] values)
                                {
                                    if (values == null)
                                    {
                                        return null;
                                    }
                                    List<float> floatValues = new List<float>();
                                    foreach (string v in values)
                                    {
                                        bool parseSucess = float.TryParse(v, out float floatValue);
                                        if (!parseSucess)
                                        {
                                            //invalid float
                                            return null;
                                        }
                                        floatValues.Add(floatValue);
                                    }
                                    if (floatValues.Count != values.Length)
                                    {
                                        //error while adding floats to list
                                        Debug.WriteLine("Error while extracting floats");
                                        Debug.Unindent();
                                        throw new ApplicationException("Error while extracting floats");
                                        /*
                                         * program throws exception here instead of just failing because theoretically the only way this if statement is true is due to 
                                         * an issue in the previous foreach statement and should never occour due to an erronious input into the subroutine
                                        */
                                    }
                                    return floatValues;
                                }
                        }
                    }
                    Debug.WriteLine("Deserialized without issue");
                    Debug.Unindent();
                    return obj;
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                Debug.WriteLine("Requested file not found");
                Debug.Unindent();
                throw;
            }
        }
    }
}