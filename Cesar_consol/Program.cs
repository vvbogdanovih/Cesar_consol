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
            var matrixA = new float[3, 3] {
            {1,2,33 },
            {6,3,4 },
            {3,4,5 }
            };
            var matrixB = new float[3, 3] {
            {2,3,4 },{2,3,4 },{3,4,5 } };

            //var s = Stopwatch.StartNew();
            //PrintMatrix(MatrixAddGPU(matrixA));

            float[,] m = new float[3, 3] {
            {1,2,33 },
            {6,3,4 },
            {3,4,5 }
            };

            Matrix test = new Matrix(1000);
            test.RandFil();

            /*
            Console.WriteLine(det(test.ToFloat()));
            Console.WriteLine(MatrixAddGPU(test.ToFloat()));
            */
            Console.WriteLine(ParalelDet(test.ToFloat()));
            Console.WriteLine(det(test.ToFloat()));
            
            

            
            //Console.WriteLine("Determinant of m computed via decomposition = " + det.ToString("F1"));
        }

        public static float det(float[,] matrix)
        {
            float det = 1;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for(int j = i+1;  j < matrix.GetLength(1); j++)
                {                    
                    float coef = matrix[j,i] / matrix[i,i];

                    for(int k = 0; k < matrix.GetLength(0); k++)
                    {
                        matrix[j, k] = matrix[j, k] - (matrix[i, k] * coef);
                    }
                }
                det *= matrix[i,i];
            }
            return det;
            
        }

        public static float ParalelDet(float[,] matrix)
        {
            float det = 1;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = i + 1; j < matrix.GetLength(1); j++)
                {
                    float coef = matrix[j, i] / matrix[i, i];
                    Parallel.For(0, matrix.GetLength(1), k =>
                    {
                        matrix[j, k] = matrix[j, k] - (matrix[i, k] * coef);
                    });
                }
                det *= matrix[i, i];
            }
            return det;
        }

        

    }
}
