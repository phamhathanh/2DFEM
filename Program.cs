﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace _2DFEM
{
    class Program
    {
        private static void Main(string[] args)
        {
            TimeSpan initTime, calcTime, solveTime, totalTime;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            //
            // khoi tao
            //

            Mesh mesh = new Mesh();
            
            Node[] nodes = mesh.GetNodes(),
                    interiorNodes = mesh.GetInteriorNodes(),
                    boundaryNodes = mesh.GetBoundaryNodes();
            FiniteElement[] finiteElements = mesh.GetFiniteElements();
            
            SparseMatrix A = new SparseMatrix(interiorNodes.Length, interiorNodes.Length);
            SparseMatrix Ag = new SparseMatrix(interiorNodes.Length, boundaryNodes.Length);

            double[] cg = new double[boundaryNodes.Length];
            for (int i = 0; i < boundaryNodes.Length; i++)
                cg[i] = Input.G(boundaryNodes[i].Position);
            Vector Cg = new Vector(cg);
            
            initTime = stopWatch.Elapsed;
            Console.WriteLine("FEM for solving equation: -Laplace(u) + a0 * u = F");
            Console.WriteLine("Number of interior vertices: {0}", interiorNodes.Length);
            Console.WriteLine("Number of boundary vertices: {0}", boundaryNodes.Length);
            Console.WriteLine("Number of finite elements: {0}", finiteElements.Length);
            Console.WriteLine("Initialization time: {0:F3} sec", initTime.TotalSeconds);

            //
            // tich luy tren tung phan tu huu han
            //

            double[] f = new double[interiorNodes.Length];
            foreach (FiniteElement fe in finiteElements)
                for (int i = 0; i < 3; i++)
                    if (fe.nodes[i].IsInside)
                    {
                        int I = fe.nodes[i].Index;
                        for (int j = 0; j < 3; j++)
                            if (fe.nodes[j].IsInside)
                            {
                                int J = fe.nodes[j].Index;
                                A[I, J] += fe.GetLocalA(i, j);
                            }

                        f[I] += fe.GetLocalF(i);
                    }
                    else
                    {
                        int J = fe.nodes[i].Index;
                        for (int j = 0; j < 3; j++)
                            if (fe.nodes[j].IsInside)
                            {
                                int I = fe.nodes[j].Index;
                                Ag[I, J] += fe.GetLocalA(i, j);
                            }
                    }

            Vector F = new Vector(f);

            calcTime = stopWatch.Elapsed - initTime;
            Console.WriteLine("Matrix & RHS calculation time: {0:F3} sec", calcTime.TotalSeconds);

            // giai he

            Calculator.CGResult result = Calculator.SolveByCG(A, F - Ag * Cg, Input.e);
            Vector C = result.vector;

            solveTime = stopWatch.Elapsed - calcTime;
            Console.WriteLine("CG completed successfully: {0} iterations. Residual: {1:0.###e+00}",
                                                                    result.iterations, result.error);
            Console.WriteLine("Matrix solution time: {0:F3} sec", solveTime.TotalSeconds);
            

            //
            // display error
            //
            
            Func<Vector2, double> Uh = (v) =>
                {
                    double output = 0;
                    foreach (FiniteElement fe in finiteElements)
                        if (fe.Contains(v))
                        {
                            for (int i = 0; i < 3; i++)
                                if (fe.nodes[i].IsInside)
                                    output += fe.Phi(v, i) * C[fe.nodes[i].Index];
                                else
                                    output += fe.Phi(v, i) * Cg[fe.nodes[i].Index];
                            break;
                        }
                    return output;
                };

            Vector2 v0 = new Vector2(0.40594, 0.52323);
            double U0 = Input.U(v0),
                   Uh0 = Uh(v0);

            Console.WriteLine("Exact solution at  {0}: {1}", v0, U0);
            Console.WriteLine("Approx solution at {0}: {1}", v0, Uh0);
            Console.WriteLine("The error at point {0}: {1}", v0, Uh0 - U0);

            double squareError = 0;
            foreach (FiniteElement fe in finiteElements)
                squareError += fe.GetLocalSquareError(C, Cg);

            totalTime = stopWatch.Elapsed;
            stopWatch.Stop();
            Console.WriteLine("L2 error in domain: {0}", Math.Sqrt(squareError));
            Console.WriteLine("Total time: {0:0.000} sec", totalTime.TotalSeconds);
            Console.WriteLine("Press any key to continue...");

            Console.ReadKey(true);
        }
    }
}
