﻿using ILGPU.Runtime;
using ILGPU;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace Cesar_consol
{
    internal class Program
    {
        
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
            {1,2,3 },{2,3,4 },{3,4,5 } };
            var matrixB = new float[3, 3] {
            {2,3,4 },{2,3,4 },{3,4,5 } };

            var s = Stopwatch.StartNew();

            
        }

        
        

    }
}
