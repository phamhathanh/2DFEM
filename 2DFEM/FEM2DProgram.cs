﻿using FEMSharp.FEM2D;
using System;

namespace FEMSharp
{
    internal static class FEM2DProgram
    {
        private const double a0 = 1;
        private const int n = 127, m = n;

        private static double F(Vector2 v)
        {
            double x = v.x,
                   y = v.y;

            return -4 / ((x + y + 1) * (x + y + 1) * (x + y + 1)) + 2 * Math.PI * Math.PI * Math.Sin(Math.PI * x) * Math.Sin(Math.PI * y) + a0 * U(v);
        }

        private static double G(Vector2 v)
            => U(v);

        private static double U(Vector2 v)
        {
            double x = v.x,
                   y = v.y;

            return 1 / (x + y + 1) + Math.Sin(Math.PI * x) * Math.Sin(Math.PI * y) + 99;
        }

        public static void Run()
        {
            var laplaceEquation = new LaplaceEquation(a0, F, G, U);
            laplaceEquation.SolveAndDisplay();
            Console.ReadLine();
        }
    }
}