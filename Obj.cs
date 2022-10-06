using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace obj_deserializer
{
    class Obj
    {
        //Properties
        public string ObjectFilePath { get; protected set; }
        //property names derived from the Wavefront Obj file specification
        public string ObjectName { get; protected set; }
        public List<float[]> v { get; protected set; } //vertex float arrays should have 3 values in form of x, y, z
        public List<float[]> vt { get; protected set; }//texture vertex float arrays should have 2 values in form of u, v
        public List<float[]> vn { get; protected set; }//vertex normal float arrays should have 3 values in form of i, j, k
        public List<int[][]> f { get; protected set; }//face int 2d arrays should have 3 arrays each containing 3 values in the form v,vt,vn

        //Constructors
        public Obj(string fileLocation)
        {
            ObjectFilePath = fileLocation;
            v = new List<float[]>();
            vt = new List<float[]>();
            vn = new List<float[]>();
            f = new List<int[][]>();
        }
        private Obj() //only constructor setting the ObjectFilePath exposed to the program to prevent null values however this constructor is still valid
        {
            ObjectFilePath = null;
            v = new List<float[]>();
            vt = new List<float[]>();
            vn = new List<float[]>();
            f = new List<int[][]>();
        }

        //Methods
        public void SetName(string name)
        {
            ObjectName = name;
        }
        public void CreateFace(int[] f1, int[] f2, int[] f3)
        {
            int[][] newFace = new int[3][];
            newFace[0] = f1;
            newFace[1] = f2;
            newFace[2] = f3;
            f.Add(newFace);
        }
        public void AddVertex(float x, float y, float z)
        {
            v.Add(new float[3] { x, y, z });
        }
        public void AddTextureVertex(float u, float v)
        {
            vt.Add(new float[2] { u, v });
        }
        public void AddVertexNormal(float i, float j, float k)
        {
            vn.Add(new float[3] { i, j, k });
        }
    }

    class ObjDeserializer
    {
        public static Obj Deserialize(string fileLocation)
        {
            try
            {
                Obj obj;
                using (StreamReader SR = new StreamReader(fileLocation))
                {
                    obj = new Obj(fileLocation);

                    while (SR.Peek() != -1)
                    {
                        string currentLine = SR.ReadLine().Trim();
                        string key = currentLine.Substring(0, currentLine.IndexOf(' ')).ToLower();
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
                                            if(intData.Length != 3)
                                            {
                                                //error while extracting faces
                                                Debug.WriteLine("error while extracting faces");
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
                                if  (values == null)
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


                    return obj;
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                Debug.WriteLine("Requested file not found");
                throw;
            }
        }
    }
}
