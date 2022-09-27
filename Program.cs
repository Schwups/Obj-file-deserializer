using System;

namespace obj_deserializer
{
    class Program
    {
        //Below is only testing code as the Obj class and the deserializer are designed to be used in other projects
        static void Main(string[] args)
        {
            
            Obj test = ObjDeserializer.Deserialize(@"S:\cube.obj");
            Console.WriteLine(test.ObjectName);
            int i = 0;
            foreach (float f in test.v)
            {
                i++;
                Console.Write(f);
                if (i % 3 == 0)
                {
                    Console.Write("\n");
                }
            }
            i = 0;
            foreach (Obj.Face f in test.f)
            {
                i++;
                Console.Write($"{f.v}/{f.vt}/{f.vn} ");
                if (i % 3 == 0)
                {
                    Console.Write("\n");
                }
            }
            Console.ReadLine();
        }
    }

}
