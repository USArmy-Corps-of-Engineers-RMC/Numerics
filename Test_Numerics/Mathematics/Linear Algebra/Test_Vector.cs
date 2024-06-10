using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numerics.Mathematics.LinearAlgebra;
using System;

namespace Mathematics.Linear_Algebra
{
    [TestClass]
    public class Test_Vector
    {
        [TestMethod]
        public void Test_Projection()
        {
            // https://math.libretexts.org/Bookshelves/Applied_Mathematics/Mathematics_for_Game_Developers_(Burzynski)/02%3A_Vectors_In_Two_Dimensions/2.06%3A_The_Vector_Projection_of_One_Vector_onto_Another
            var u = new Vector(new[] { 4d, 3d });
            var v = new Vector(new[] { 2d, 8d });
            var vp = Vector.Project(u, v);

            Assert.AreEqual(16d / 17d, vp[0], 1E-6);
            Assert.AreEqual(64d / 17d, vp[1], 1E-6);
        }
    }
}
