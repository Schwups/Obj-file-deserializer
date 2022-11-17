using System;
using System.Collections.Generic;

namespace Obj_Class
{
    public class Obj : IDisposable
    {
        private bool disposedValue;

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

        //IDisposible Implementation added to be able to free up memory when a large obj Object is no longer needed
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ObjectFilePath = null;
                    ObjectName = null;
                    v = null;
                    vt = null;
                    vn = null;
                    f = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
            GC.Collect();
        }
    }
}