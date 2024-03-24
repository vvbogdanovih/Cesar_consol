using ILGPU.Runtime;
using ILGPU;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesar_consol
{
    public class GPUBenchmark
    {
        private Stopwatch stopWatch = new Stopwatch();

        public GPUBenchmark() { }

        // Sum Test
        public List<SimpleResult> RunSumTestGPU(Matrix matrix, int startSize, int endSize, int step)
        {
            List<SimpleResult> Results = new List<SimpleResult>();
            stopWatch.Restart();

            // Performs a series of tests from "startSize" to "endSize" 
            for (int size = startSize; size <= endSize; size += step)
            {
                Matrix tempMatrix = new Matrix(size);

                // Fils matrixResult
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        tempMatrix[i, j] = matrix[i, j];
                    }
                }

                float[] matrixResult;
                stopWatch.Start();

                // Множення матриць
                matrixResult = MatrixAddGPU(tempMatrix.ToFloat());

                stopWatch.Stop();
                Results.Add(new SimpleResult(size, stopWatch.Elapsed.TotalMilliseconds.ToString()));
                stopWatch.Restart();
            }

            return Results;
        }
        private float[] MatrixAddGPU(float[,] matrixA)
        {
            void MatrixMultiplyKernel(Index2D index, ArrayView2D<float, Stride2D.DenseX> matrixA, ArrayView1D<float, Stride1D.Dense> output)
            {
                for (var i = 0; i < matrixA.IntExtent.Y; i++)
                    output[index.X] += matrixA[new Index2D(index.X, i)];
            }

            Context ctx = Context.CreateDefault();
            Accelerator AxeleratorgpuDevice = ctx.GetPreferredDevice(preferCPU: false)
                                      .CreateAccelerator(ctx);

            // Karnal for MatrixMultiplyKernel
            Action<Index2D, ArrayView2D<float, Stride2D.DenseX>, ArrayView1D<float, Stride1D.Dense>>
                kernel = AxeleratorgpuDevice.LoadAutoGroupedStreamKernel<Index2D, ArrayView2D<float, Stride2D.DenseX>, ArrayView1D<float, Stride1D.Dense>>(MatrixMultiplyKernel);


            int Rows = matrixA.GetLength(0);
            int Cols = matrixA.GetLength(0);
            MemoryBuffer2D<float, Stride2D.DenseX> mA = AxeleratorgpuDevice.Allocate2DDenseX<float>(new Index2D(Rows, Cols));
            MemoryBuffer1D<float, Stride1D.Dense> mB = AxeleratorgpuDevice.Allocate1D<float>(Rows);


            mA.CopyFromCPU(matrixA);

            kernel(mA.Extent.ToIntIndex(), mA.View, mB.View);
            float[] a = new float[Rows];

            mB.CopyToCPU<float>(a);

            // Dispose
            ctx.Dispose();
            AxeleratorgpuDevice.Dispose();
            mA.Dispose();
            mB.Dispose();
            return a;
        }

        // Multiplay Test
        public List<SimpleResult> RunMultTestGPU(Matrix matrixA, Matrix matrixB, int startSize, int endSize, int step)
        {
            List<SimpleResult> Results = new List<SimpleResult>();
            stopWatch.Restart();

            // Performs a series of tests from "startSize" to "endSize" 
            for (int size = startSize; size <= endSize; size += step)
            {
                Matrix matrixResult;
                Matrix tempMatrixA = new Matrix(size);
                Matrix tempMatrixB = new Matrix(size);

                // Fils matrixResult
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        tempMatrixA[i, j] = matrixA[i, j];
                        tempMatrixB[i, j] = matrixB[i, j];
                    }
                }
                stopWatch.Start();

                // Множення матриць
                matrixResult = new Matrix(MatrixMultiplayGPU(tempMatrixA.ToFloat(), tempMatrixB.ToFloat()));
                
                stopWatch.Stop();
                Results.Add(new SimpleResult(size, stopWatch.Elapsed.TotalMilliseconds.ToString()));
                stopWatch.Restart();
            }

            return Results;
        }
        private float[,] MatrixMultiplayGPU(float[,] matrixA, float[,] matrixB)
        {
            void MatrixMultiplyKernel(Index2D index, ArrayView2D<float, Stride2D.DenseX> matrixA, ArrayView2D<float, Stride2D.DenseX> matrixB, ArrayView2D<float, Stride2D.DenseX> output)
            {
                for (var i = 0; i < matrixA.IntExtent.Y; i++)
                    output[index] += matrixA[new Index2D(index.X, i)] * matrixB[new Index2D(i, index.Y)];
            }

            Context ctx = Context.CreateDefault();
            Accelerator AxeleratorgpuDevice = ctx.GetPreferredDevice(preferCPU: false)
                                      .CreateAccelerator(ctx);

            // Karnal for MatrixMultiplyKernel
            Action<Index2D, ArrayView2D<float, Stride2D.DenseX>, ArrayView2D<float, Stride2D.DenseX>, ArrayView2D<float, Stride2D.DenseX>>
                kernel = AxeleratorgpuDevice.LoadAutoGroupedStreamKernel<Index2D, ArrayView2D<float, Stride2D.DenseX>, ArrayView2D<float, Stride2D.DenseX>, ArrayView2D<float, Stride2D.DenseX>>(MatrixMultiplyKernel);


            int Rows = matrixA.GetLength(0);
            int Cols = matrixA.GetLength(0);
            MemoryBuffer2D<float, Stride2D.DenseX> mA = AxeleratorgpuDevice.Allocate2DDenseX<float>(new Index2D(Rows, Cols));
            MemoryBuffer2D<float, Stride2D.DenseX> mB = AxeleratorgpuDevice.Allocate2DDenseX<float>(new Index2D(Rows, Cols));
            MemoryBuffer2D<float, Stride2D.DenseX> mC = AxeleratorgpuDevice.Allocate2DDenseX<float>(new Index2D(Rows, Cols));

            mA.CopyFromCPU(matrixA);
            mB.CopyFromCPU(matrixB);
            kernel(mC.Extent.ToIntIndex(), mA.View, mB.View, mC.View);

            float[,] a = new float[Rows, Cols];
            mB.CopyToCPU<float>(a);

            // Dispose
            ctx.Dispose();
            AxeleratorgpuDevice.Dispose();
            mA.Dispose();
            mB.Dispose();
            mC.Dispose();
            return a;
        }

        // Det Test
        public List<SimpleResult> RunSingularityTestGPU(Matrix matrix, int startSize, int endSize, int step)
        {
            List<SimpleResult> Results = new List<SimpleResult>();
            stopWatch.Restart();

            // Performs a series of tests from "startSize" to "endSize" 
            for (int size = startSize; size <= endSize; size += step)
            {
                float matrixResult;
                Matrix tempMatrix = new Matrix(size);

                // Fils matrixResult
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        tempMatrix[i, j] = matrix[i, j];
                    }
                }
                stopWatch.Start();

                // Matrix Det
                matrixResult = MatrixDetGPU(tempMatrix.ToFloat());

                stopWatch.Stop();
                Results.Add(new SimpleResult(size, stopWatch.Elapsed.TotalMilliseconds.ToString()));
                stopWatch.Restart();
            }

            return Results;
        }
        private float MatrixDetGPU(float[,] matrixA)
        {
            void DetKernel(Index1D index, ArrayView2D<float, Stride2D.DenseX> matrixA)
            {
                for (int q = 0; q < 2; q++)
                {
                    int i = index;
                    for (var j = i + 1; j < matrixA.IntExtent.Y; j++)
                    {
                        var coef = matrixA[j, i] / matrixA[i, i];

                        for (var k = 0; k < matrixA.IntExtent.Y; k++)
                        {
                            matrixA[j, k] = matrixA[j, k] - (matrixA[i, k] * coef);
                        }
                    }
                }
            }

            Context ctx = Context.CreateDefault();
            Accelerator AxeleratorgpuDevice = ctx.GetPreferredDevice(preferCPU: false)
                                      .CreateAccelerator(ctx);

            // Karnal for DetKernel
            Action<Index1D, ArrayView2D<float, Stride2D.DenseX>>
                kernel = AxeleratorgpuDevice.LoadAutoGroupedStreamKernel<Index1D, ArrayView2D<float, Stride2D.DenseX>>(DetKernel);


            int Rows = matrixA.GetLength(0);
            int Cols = matrixA.GetLength(0);
            float[] outarr = new float[Rows];
            outarr[1] = 1;
            MemoryBuffer2D<float, Stride2D.DenseX> mA = AxeleratorgpuDevice.Allocate2DDenseX<float>(new Index2D(Rows, Cols));
            MemoryBuffer1D<float, Stride1D.Dense> mB = AxeleratorgpuDevice.Allocate1D<float>(Rows);


            mA.CopyFromCPU(matrixA);
            mB.CopyFromCPU(outarr);
            kernel(new Index1D(Cols), mA.View);
            float[,] a = new float[Rows, Cols];

            mA.CopyToCPU<float>(a);

            float det = 1;
            for (int i = 0; i < Cols; i++)
            {
                det *= a[i, i];
            }

            // Dispose
            ctx.Dispose();
            AxeleratorgpuDevice.Dispose();
            mA.Dispose();
            mB.Dispose();
            return det;
        }

    }
}
