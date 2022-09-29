﻿using System;
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
        public List<float[]> v { get; protected set; } //float arrays should have 3 values in form of x, y, z
        public List<float[]> vt { get; protected set; }//float arrays should have 2 values in form of u, v
        public List<float[]> vn { get; protected set; }//float arrays should have 3 values in form of i, j, k
        public List<Face> f { get; protected set; }

        public struct Face
        {
            //structure of the Face struct is made to be similar to the Obj file specification of a face statement: f v1/vt1/vn1 v2/vt2/vn2 ...
            public int v { get; }
            public int vt { get; }
            public int vn { get; }
            public Face(int v, int vt, int vn)
            {
                this.v = v;
                this.vt = vt;
                this.vn = vn;
            }
        }

        //Constructors
        public Obj(string fileLocation)
        {
            ObjectFilePath = fileLocation;
            v = new List<float[]>();
            vt = new List<float[]>();
            vn = new List<float[]>();
            f = new List<Face>();
        }
        private Obj() //only constructor setting the ObjectFilePath exposed to the program to prevent null values however this constructor is still valid
        {
            ObjectFilePath = null;
            v = new List<float[]>();
            vt = new List<float[]>();
            vn = new List<float[]>();
            f = new List<Face>();
        }

        //Methods
        public void SetName(string name)
        {
            ObjectName = name;
        }
        public void CreateFace(int v, int vt, int vn)
        {
            f.Add(new Face(v, vt, vn));
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
                                    foreach (string s in faces)
                                    {
                                        string[] data = s.Split('/');
                                        if (data.Length <= 0 || data.Length >= 4)
                                        {
                                            //invalid face
                                            break;
                                        }

                                        try
                                        {
                                            if (data.Length == 1)
                                            {
                                                obj.CreateFace(Convert.ToInt32(data[0]), -1, -1);
                                            }
                                            else
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
                                                obj.CreateFace(intData[0], intData[1], intData[2]);
                                            }
                                        }
                                        catch (FormatException)
                                        {
                                            // invalid int in face
                                            break;
                                        }
                                    }
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
