using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesar_consol
{
    internal class CPUMultiThredBenchmark
    {
        private Stopwatch stopWatch = new Stopwatch();

        public CPUMultiThredBenchmark()
        {
            
        }

        public List<SimpleResult> RunSumTestTestAsync(Matrix matrix, int startSize, int step)
        {
            void SumRow(Matrix matrix, int i, float[] arrForSum)
            {
                float sum = 0;
                for (int j = 0; j < matrix.Size; j++)
                {
                    sum += matrix.matrix[i, j];
                }
                arrForSum[i] = sum;
            }
            List<SimpleResult> Results = new List<SimpleResult>();            
            int endSize = matrix.Size / step;
            stopWatch.Restart();

            // Performs a series of tests from "startSize" to "endSize"
            for (int size = startSize; size < endSize; size += step)
            {                
                stopWatch.Start();

                // Adding matrices
                
                float[] sumArray = new float[matrix.Size];
                List<Thread> threads = new List<Thread>();
                for (int i = 0; i < matrix.Size; i++)
                {
                    int copyOfI = i;
                    Thread thread = new Thread(() => SumRow(matrix, copyOfI, sumArray));
                    threads.Add(thread);
                    thread.Start();
                }
                foreach (Thread thread in threads)
                {
                    thread.Join();
                }

                stopWatch.Stop();
                Results.Add(new SimpleResult(size, stopWatch.Elapsed.TotalMilliseconds.ToString()));
                stopWatch.Restart();
            }

            return Results;
        }

        public List<SimpleResult> RunMultTestAsync(Matrix matrixA, Matrix matrixB, int startSize, int step)
        {
            static void MultAsync(Matrix matrixA, Matrix matrixB, Matrix matrixResult)
            {
                static void Mult(Matrix matrixA, Matrix matrixB, Matrix matrixResult, int i)
                {
                    for (int j = 0; j < matrixB.Size; j++)
                    {
                        for (int k = 0; k < matrixA.Size; k++)
                        {
                            matrixResult[i, j] += matrixA[i, k] * matrixB[k, j];
                        }
                    }
                }

                List<Thread> threads = new List<Thread>();
                for (int i = 0; i < matrixA.Size; i++)
                {
                    int copyOfI = i;
                    Thread thread = new Thread(() => Mult(matrixA, matrixB, matrixResult, copyOfI));
                    threads.Add(thread);
                    thread.Start();
                }

                foreach (Thread thread in threads)
                {
                    thread.Join();
                }
            }

            List<SimpleResult> Results = new List<SimpleResult>();
            int endSize = matrixA.Size / step;
            stopWatch.Restart();

            // Performs a series of tests from "startSize" to "endSize" 
            for (int size = startSize; size < endSize; size += step)
            {
                Matrix matrixResult = new Matrix(size);
                stopWatch.Start();                

                // Множення матриць
                MultAsync(matrixA, matrixB, matrixResult);

                stopWatch.Stop();
                Results.Add(new SimpleResult(size, stopWatch.Elapsed.TotalMilliseconds.ToString()));
                stopWatch.Restart();
            }
            
            return Results;
        }
        
        public List<SimpleResult> RunSingularityTestAsync(Matrix matrix, int startSize, int step)
        {
            static float ParallelDet(Matrix matrix)
            {
                float det = 1;
                for (int i = 0; i < matrix.Size; i++)
                {
                    for (int j = i + 1; j < matrix.Size; j++)
                    {
                        float coef = matrix[j, i] / matrix[i, i];

                        for (int k = 0; k < matrix.Size; k++)
                        {
                            matrix[j, k] = matrix[j, k] - (matrix[i, k] * coef);
                        }
                    }
                    det *= matrix[i, i];
                }
                return det;
            }
            List<SimpleResult> Results = new List<SimpleResult>();
            int endSize = matrix.Size / step;
            stopWatch.Restart();

            // Performs a series of tests from "startSize" to "endSize" 
            for (int size = startSize; size < endSize; size += step)
            {
                Matrix matrixResult = new Matrix(size);
                stopWatch.Start();

                // Fils matrixResult
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        matrixResult[i, j] = matrix[i, j];
                    }
                }

                // Determinant
                float a = ParallelDet(new Matrix(matrixResult));
                stopWatch.Stop();
                Results.Add(new SimpleResult(size, stopWatch.Elapsed.TotalMilliseconds.ToString()));
                stopWatch.Restart();
            }

            return Results;
        }        
    }
}
