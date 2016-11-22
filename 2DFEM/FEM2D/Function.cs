﻿using System;

namespace _2DFEM
{
    class Function : IFunction<Vector2, double>
    {
        private readonly Func<Vector2, double> function;

        public Function(Func<Vector2, double> function)
        {
            this.function = function;
        }

        public double GetValueAt(Vector2 input)
            => function(input);
    }
}