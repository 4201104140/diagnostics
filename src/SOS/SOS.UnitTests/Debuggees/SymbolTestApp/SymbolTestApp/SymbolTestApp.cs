using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace SymbolTestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //string dllPath = args[0];
            string dllPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..", "SymbolTestDll\\Debug\\netcoreapp2.1");
            Console.WriteLine("SymbolTestApp starting {0}", dllPath);
            Foo1(42, dllPath);
        }

        static int Foo1(int x, string dllPath)
        {
            return Foo2(x, dllPath);
        }

        static int Foo2(int x, string dllPath)
        {
            Foo4(dllPath);
            return x;
        }

        static void Foo4(string dllPath)
        {
            Stream dll = File.OpenRead(Path.Combine(dllPath, @"SymbolTestDll.dll"));
            Stream pdb = null;
            string pdbFile = Path.Combine(dllPath, @"SymbolTestDll.pdb");
            if (File.Exists(pdbFile))
            {
                pdb = File.OpenRead(pdbFile);
            }
            Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(dll, pdb);

            Type dllType = assembly.GetType("SymbolTestDll.TestClass");
            MethodInfo dllMethod = dllType.GetMethod("ThrowException");
            dllMethod.Invoke(null, new object[] { "This is the exception message" });
        }
    }
}
