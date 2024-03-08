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
    internal class GPUBenchmark
    {
        private Stopwatch stopWatch = new Stopwatch();

        public GPUBenchmark() { }

        // Sum Test
        public List<SimpleResult> RunSumTestGPU(Matrix matrix, int startSize, int step)
        {
            List<SimpleResult> Results = new List<SimpleResult>();
            int endSize = matrix.Size / step;
            stopWatch.Restart();

            // Performs a series of tests from "startSize" to "endSize" 
            for (int size = startSize; size < endSize; size += step)
            {
                float[] matrixResult;
                stopWatch.Start();

                // Множення матриць
                matrixResult = MatrixAddGPU(matrix.ToFloat());

                stopWatch.Stop();
                Results.Add(new SimpleResult(size, stopWatch.Elapsed.TotalMilliseconds.ToString()));
                stopWatch.Restart();
            }

            return Results;
        }
        public float[] MatrixAddGPU(float[,] matrixA)
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
        public List<SimpleResult> RunMultTestGPU(Matrix matrixA, Matrix matrixB, int startSize, int step)
        {
            List<SimpleResult> Results = new List<SimpleResult>();
            int endSize = matrixA.Size / step;
            stopWatch.Restart();

            // Performs a series of tests from "startSize" to "endSize" 
            for (int size = startSize; size < endSize; size += step)
            {
                Matrix matrixResult; 
                stopWatch.Start();

                // Множення матриць
                matrixResult = new Matrix(MatrixMultiplayGPU(matrixA.ToFloat(), matrixB.ToFloat()));
                
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
    }
}
