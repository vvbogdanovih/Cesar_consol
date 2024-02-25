using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cesar_consol
{
    internal class CPUBenchmark
    {
        private Stopwatch stopWatch = new Stopwatch();
        public CPUBenchmark()
        {
            
        }

        /// <summary>
        /// The size of the matrix must be at least "startSize"
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="startSize"></param>
        /// <param name="endSize"></param>
        public List<SimpleResult> RunSumTest(Matrix matrix, int startSize, int step)
        {
            List<SimpleResult> Results = new List<SimpleResult>();            
            int endSize = matrix.Size / step;
            stopWatch.Restart();

            // Performs a series of tests from "startSize" to "endSize"
            for (int size = startSize; size <= endSize; size += step)
            {
                stopWatch.Start();

                // Adding matrices
                float sum = 0;
                float[] sumArray = new float[size];
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        sum += matrix.matrix[i, j];
                    }
                    sumArray[i] = sum;
                    sum = 0;
                }
                stopWatch.Stop();
                Results.Add(new SimpleResult(size, stopWatch.Elapsed.TotalMilliseconds.ToString()));
                stopWatch.Restart();
            }

            return Results;
        }

        /// <summary>
        /// The size of the matrixA and matrixB must be at least "startSize".  
        /// Термін "сингулярність матриці" в математиці вказує на те, коли матриця не має оберненої. Матриця має обернену, якщо існує інша матриця, яка, помножена на початкову, дає одиничну матрицю.
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <param name="startSize"></param>
        /// <param name="endSize"></param>
        public List<SimpleResult> RunMultTest(Matrix matrixA, Matrix matrixB, int startSize, int step)
        {
            List<SimpleResult> Results = new List<SimpleResult>();
            int endSize = matrixA.Size / step;
            stopWatch.Restart();

            // Performs a series of tests from "startSize" to "endSize"
            for (int size = startSize; size <= endSize; size += step)
            {
                Matrix matrixC = new Matrix(size);

                // Matrix multiplication
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        for (int k = 0; k < size; k++)
                        {
                            matrixC[i, j] += matrixA[i, k] * matrixB[k, j];
                        }
                    }
                }
                stopWatch.Stop();
                Results.Add(new SimpleResult(size, stopWatch.Elapsed.TotalMilliseconds.ToString()));
                stopWatch.Restart();
            }

            return Results;
        }

        public List<SimpleResult> RunSingularityTest(Matrix matrix, int startSize, int step)
        {
            static float det(Matrix a)
            {
                if (a.Size == 2) return Matrix.Det2x2(a);
                float res = 0;

                for (int i = 0; i < a.Size; i++)
                {
                    if ((i % 2) == 0)
                    {
                        res += a[0, i] * (-det(Matrix.GetMinor(a, i)));
                    }
                    else
                    {
                        res += a[0, i] * det(Matrix.GetMinor(a, i));
                    }

                }

                return res;
            }
            List<SimpleResult> Results = new List<SimpleResult>();
            int endSize = matrix.Size / step;
            stopWatch.Restart();

            // Performs a series of tests from "startSize" to "endSize" 
            for (int size = startSize; size <= endSize; size += step)
            {
                Matrix matrixC = new Matrix(size);

                // Fils matrixC
                for (int i = 0;i < size; i++)
                {
                    for(int j = 0;j < size; j++)
                    {
                        matrixC[i, j] = matrix[i, j];
                    }
                }

                // Determinant
                float a = det(new Matrix(matrixC));

                stopWatch.Stop();
                Results.Add(new SimpleResult(size, stopWatch.Elapsed.TotalMilliseconds.ToString()));
                stopWatch.Restart();
            }

            return Results;
        }
        
    }
}
