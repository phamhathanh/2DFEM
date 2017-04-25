﻿using System;
using System.Collections.ObjectModel;

namespace FEM_NET.FEM2D
{
    internal interface IFiniteElement
    {
        ReadOnlyCollection<INode> Nodes { get; }
        Triangle Triangle { get; }

        bool Contains(Vector2 point);
    }

    internal interface INode
    {
        Vertex Vertex { get; }
        Func<Vector2, double> Phi { get; }
        Func<Vector2, Vector2> GradPhi { get; }
    }
    
    internal delegate IFiniteElement IFiniteElementFactory(Triangle triangle);
}
