using ILGPU.Runtime;
using ILGPU;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

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
            {1,2,3 },
            {2,3,4 },
            {3,4,5 } 
            };
            var matrixB = new float[3, 3] {
            {2,3,4 },{2,3,4 },{3,4,5 } };

            //var s = Stopwatch.StartNew();
            PrintMatrix(MatrixAddGPU(matrixA));


        }

        public static float[] MatrixAddGPU(float[,] matrixA)
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
                kernel = AxeleratorgpuDevice.LoadAutoGroupedStreamKernel<Index2D, ArrayView2D<float, Stride2D.DenseX>, ArrayView1D<float, Stride1D.Dense>> (MatrixMultiplyKernel);


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


    }
}
