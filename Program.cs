using System;

namespace obj_deserializer
{
    class Program
    {
        //Below is only testing code as the Obj class and the deserializer are designed to be packaged into a DLL for use in other projects
        //obj used for testing can be found at https://gist.github.com/MaikKlein/0b6d6bb58772c13593d0a0add6004c1c
        static void Main(string[] args)
        {
            Obj test = ObjDeserializer.Deserialize(@"M:\cube.obj");
            Console.Write("name:\n" + test.ObjectName + "\n");
            Console.Write("\nvertexes:\n");
            foreach (float[] f in test.v)
            {
                Console.Write($"{f[0]} {f[1]} {f[2]}\n");
            }
            Console.Write("\ntexture vertexes:\n");
            foreach (float[] f in test.vt)
            {
                Console.Write($"{f[0]} {f[1]}\n");
            }
            Console.Write("\nvertex normals:\n");
            foreach (float[] f in test.vn)
            {
                Console.Write($"{f[0]} {f[1]} {f[2]}\n");
            } 
            Console.Write("\nfaces:\n");
            foreach (int[][] f in test.f)
            {
                Console.Write($"{f[0][0]}/{f[0][1]}/{f[0][2]} {f[1][0]}/{f[1][1]}/{f[1][2]} {f[2][0]}/{f[2][1]}/{f[2][2]}\n");
            }
            Console.ReadLine();
        }
    }

}
