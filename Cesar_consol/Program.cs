using ILGPU.Runtime;
using ILGPU;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using ILGPU.Runtime.OpenCL;

namespace Cesar_consol
{
    internal class Program
    {
        public static void PrintMatrix(float[] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                Console.Write(matrix[i].ToString() + " ");
                Console.WriteLine();
            }
        }
        public static void PrintMatrix(float[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j].ToString() + " ");
                }
                Console.WriteLine();
            }
        }
        
        public static void PrintResult (Result[] result)
        {
            for(int i = 0;i < result.Length;i++)
            {
                Console.WriteLine(result[i].Type);
                foreach (SimpleResult resultItem in result[i].Results)
                {
                    Console.WriteLine(resultItem.ToString());
                }
            }
            
        }
        public static void PrintMatrix(Matrix matrix)
        {
            for (int i = 0; i < matrix.Size; i++)
            {
                for (int j = 0; j < matrix.Size; j++)
                {
                    Console.Write(matrix[i, j].ToString() + " ");
                }
                Console.WriteLine();
            }
        }
        public static void Main()
        {
           CesarBenchmark cesarBenchmark = new CesarBenchmark();
            Matrix matrix = new Matrix(1000);
            matrix.RandFil();
            var a = cesarBenchmark.RunSumBenchmark(matrix, 100, 1000, 100);
            PrintResult(a);
        }
        

    }
}
