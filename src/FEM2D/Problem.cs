﻿using System;
using System.Collections.Generic;

namespace FEM_NET.FEM2D
{
    internal class Problem
    {
        private const double VERY_LARGE_VALUE = 1e30;

        private readonly IFiniteElementSpace finiteElementSpace;
        private readonly Func<Vector2, double> rightHandSide;
        private readonly Dictionary<int, Func<Vector2, double>> boundaryConditions;
        private readonly BilinearForm bilinearForm;
        private readonly double accuracy;

        private Matrix A;
        private Vector rhs;

        public Problem(IFiniteElementSpace finiteElementSpace, Dictionary<int, Func<Vector2, double>> boundaryConditions,
                        BilinearForm bilinearForm, Func<Vector2, double> rightHandSide,
                        double accuracy)
        {
            this.finiteElementSpace = finiteElementSpace;
            this.boundaryConditions = boundaryConditions;
            this.bilinearForm = bilinearForm;
            this.rightHandSide = rightHandSide;
            this.accuracy = accuracy;
        }

        public IFiniteElementFunction Solve()
        {
            CalculateMatrixAndRHS();
            var solution = Calculator.Solve(A, rhs, accuracy).vector;
            return new FiniteElementFunction(finiteElementSpace, solution);
        }

        private void CalculateMatrixAndRHS()
        {
            var n = finiteElementSpace.Mesh.Vertices.Count;
            A = new Matrix(n, n);
            var rhs = new double[n];

            foreach (var vertex in finiteElementSpace.Mesh.Vertices)
            {
                bool isDirichletNode = boundaryConditions.ContainsKey(vertex.Reference);
                if (isDirichletNode)
                {
                    var value = boundaryConditions[vertex.Reference](vertex.Position);
                    int i = vertex.Index;
                    rhs[i] += VERY_LARGE_VALUE*value;
                    A[i, i] += VERY_LARGE_VALUE;
                }
            }

            foreach (var finiteElement in finiteElementSpace.FiniteElements)
                foreach (var node in finiteElement.Nodes)
                {
                    int i = node.Vertex.Index;
                    rhs[i] += Calculator.Integrate(v => rightHandSide(v) * node.Phi(v), finiteElement.Triangle);
                    foreach (var otherNode in finiteElement.Nodes)
                    {
                        int j = otherNode.Vertex.Index;
                        Func<Vector2, double> localBilinearForm =
                            v => bilinearForm(node.Phi(v), otherNode.Phi(v), node.GradPhi(v), otherNode.GradPhi(v));
                        var integral = Calculator.Integrate(localBilinearForm, finiteElement.Triangle);
                        A[i, j] += integral;
                        // Can be cached.
                    }
                }

            this.rhs = new Vector(rhs);
        }
    }
}
