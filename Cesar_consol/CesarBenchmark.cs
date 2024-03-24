using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesar_consol
{
    public class CesarBenchmark
    {
        private Matrix inputMatrix = null;
        public CesarBenchmark() { }
        //public CesarBenchmark(TMatrix initialMatrix) => inputMatrix = initialMatrix;

        public Result[] RunSumBeBenchmark(Matrix matrix, int startSize, int endSize, int step)
        {
            CPUBenchmark cPUBenchmark = new CPUBenchmark();
            CPUMultiThredBenchmark cPUMultiThredBenchmark = new CPUMultiThredBenchmark();
            GPUBenchmark gpuBenchmark = new GPUBenchmark();

            List<SimpleResult> cPUBenchmarkResult = cPUBenchmark.RunSumTest(matrix, startSize, endSize, step);
            List<SimpleResult> cPUMultiThredBenchmarkResult = cPUMultiThredBenchmark.RunSumTestTestAsync(matrix, startSize, endSize, step);
            List<SimpleResult> gpuBenchmarkResult = gpuBenchmark.RunSumTestGPU(matrix, startSize, endSize, step);
            Result[] results = new Result[]{
            new Result(cPUBenchmarkResult, "CPU"),
            new Result(cPUMultiThredBenchmarkResult, "CPUMultiThred"),
            new Result(gpuBenchmarkResult, "GPU")};
            return results;
        }

        public Result[] RunMultBenchmark(Matrix matrixA, Matrix matrixB, int startSize, int endSize, int step)
        {
            CPUBenchmark cPUBenchmark = new CPUBenchmark();
            CPUMultiThredBenchmark cPUMultiThredBenchmark = new CPUMultiThredBenchmark();
            GPUBenchmark gpuBenchmark = new GPUBenchmark();

            List<SimpleResult> cPUBenchmarkResult = cPUBenchmark.RunMultTest(matrixA, matrixB, startSize, endSize, step);
            List<SimpleResult> cPUMultiThredBenchmarkResult = cPUMultiThredBenchmark.RunMultTestAsync(matrixA, matrixB, startSize, endSize, step);
            List<SimpleResult> gpuBenchmarkResult = gpuBenchmark.RunMultTestGPU(matrixA, matrixB, startSize, endSize, step);
            Result[] results = new Result[]{
            new Result(cPUBenchmarkResult, "CPU"),
            new Result(cPUMultiThredBenchmarkResult, "CPUMultiThred"),
            new Result(gpuBenchmarkResult, "GPU")};
            return results;
        }

        public Result[] RunSingularityBeBenchmark(Matrix matrix, int startSize, int endSize, int step)
        {
            CPUBenchmark cPUBenchmark = new CPUBenchmark();
            CPUMultiThredBenchmark cPUMultiThredBenchmark = new CPUMultiThredBenchmark();
            GPUBenchmark gpuBenchmark = new GPUBenchmark();

            List<SimpleResult> cPUBenchmarkResult = cPUBenchmark.RunSingularityTest(matrix, startSize, endSize, step);
            List<SimpleResult> cPUMultiThredBenchmarkResult = cPUMultiThredBenchmark.RunSingularityTestAsync(matrix, startSize, endSize, step);
            List<SimpleResult> gpuBenchmarkResult = gpuBenchmark.RunSingularityTestGPU(matrix, startSize, endSize, step);
            Result[] results = new Result[]{
            new Result(cPUBenchmarkResult, "CPU"),
            new Result(cPUMultiThredBenchmarkResult, "CPUMultiThred"),
            new Result(gpuBenchmarkResult, "GPU")};
            return results;
        }
    }
}
