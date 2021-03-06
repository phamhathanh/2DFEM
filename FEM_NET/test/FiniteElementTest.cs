using FEM_NET.FEM2D;
using Xunit;

namespace FEM_NET.Tests
{
    public class FiniteElementTest
    {
        private Vector2 point1, point2, point3;
        private P1Element element;

        public FiniteElementTest()
        {
            point1 = new Vector2(3.14412280564, 2.83125387555);
            point2 = new Vector2(3.19406000334, 2.75777352054);
            point3 = new Vector2(3.09485254952, 2.78690898426);
            var triangle = new Triangle(new Vertex(point1, 0), new Vertex(point2, 0), new Vertex(point3, 0));
            element = new P1Element(triangle);
        }

        [Fact]
        public void InteriorCase()
        {
            var center = (1.0/3)*(point1 + point2 + point3);
            Assert.True(element.Contains(center));
        }

        [Fact]
        public void EdgeCase()
        {
            var middle = 0.5*(point1 + point2);
            Assert.True(element.Contains(middle));
        }

        [Fact]
        public void CornerCase()
        {
            Assert.True(element.Contains(point1));
            Assert.True(element.Contains(point2));
            Assert.True(element.Contains(point3));
        }
    }
}