﻿using System.Collections.ObjectModel;

namespace FEMSharp.FEM2D
{
    internal interface IMesh
    {
        ReadOnlyCollection<Node> Nodes { get; }
        ReadOnlyCollection<Node> InteriorNodes { get; }
        ReadOnlyCollection<Node> BoundaryNodes { get; }
        ReadOnlyCollection<FiniteElement> FiniteElements { get; }
    }
}