using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;



namespace Bjælke2DFEM.Models
{
   /// <summary>
   /// Hjælpe klasse gemt i den lokale hukommelse så den forsyne FEMSolver med de simple matematiske funtioner.
   /// </summary>

    public static class MatrixUtils
    {

        public static double[,] TransformLocalToGlobal(double[,] kLocal, double phi) {
            //Calculate rotation values
            double c = Math.Cos(phi);
            double s = Math.Sin(phi);

            // Transformation matrix (6x6 for 2D beam)
            double[,] T = new double[,]
            {
        {  c,  s, 0, 0, 0, 0 }, // Node 1: transform local ux to global X,Y
        { -s,  c, 0, 0, 0, 0 }, // Node 1: transform local uy to global X,Y
        {  0,  0, 1, 0, 0, 0 }, // Node 1: rotation θz unchanged
        {  0,  0, 0,  c, s, 0 }, // Node 2: transform local ux to global X,Y
        {  0,  0, 0, -s, c, 0 }, // Node 2: transform local uy to global X,Y
        {  0,  0, 0,  0, 0, 1 }  // Node 2: rotation θz unchanged
            };
            //Matrix multiplication
            var Tmat = DenseMatrix.OfArray(T);
            var Klocal = DenseMatrix.OfArray(kLocal);
            var Kglobal = Tmat.Transpose() * Klocal * Tmat;
            //Returns the transformed global stiffness matrix as a normal 2D array.
            return Kglobal.ToArray();
        }

        public static double[,] Multiply(double scalar, double[,] matrix) {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = scalar * matrix[i, j];

            return result;
        }

        /// <summary>
        /// I FEM (Finite Element Method) bruges den, når du:
        /// Har en global stivhedsmatrix K(som fx 6x6 hvis du har 3 noder med 2 frihedsgrader hver),
        /// Og du vil fjerne de rækker og kolonner, der hører til låste frihedsgrader(DOFs),
        /// Så du kun løser for de frie DOFs.
        /// </summary>
        /// <param name="matrix">Her er det vores K matrix</param>
        /// <param name="indices">Her er det vores længde af frihedsgrader</param>
        /// <returns></returns>
        public static double[,] ExtractSubmatrix(double[,] matrix, int[] indices) {
            //FORBERED TOM MATRIX:
            //Du vil udtrække en kvadratisk size x size matrix.
            //Fx: hvis indices = [2, 3, 4, 5], så får du en 4 x 4 matrix.
            int size = indices.Length; 
            double[,] result = new double[size, size];
            //Fyld ind i den
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    result[i, j] = matrix[indices[i], indices[j]];

            return result;
        }

        //Eks. double[] f_reduced = MatrixUtils.ExtractSubvector(forceVector, freeDofs);
        /// <summary>
        /// Det samme som ovenstående bare med vectorer istedet
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        public static double[] ExtractSubvector(double[] vector, int[] indices) {
            // vi skal kun bruge en parameter her, da det er en vector.
            //Så vi kan bare tage dens længde direkte som er på 4 karakterer.
            double[] result = new double[indices.Length];

            // Da vi ikke skal proppe i en matrix, behøver vi kun et for loop. vi skal bare fylde 
            //På et array
            for (int i = 0; i < indices.Length; i++)
                result[i] = vector[indices[i]];
            return result;
        }

        public static double[] SolveLinearSystem(double[,] A, double[] b) {
            var matrix = DenseMatrix.OfArray(A); // Konverter 2D array to Math.NET DenseMatrix
            var vector = DenseVector.OfArray(b); // Konverter 1D array to DenseVector

            //Solve Ax = b for x, hvor A=Matrix(reduced global stiffness matrix),
            //b=vector(reduced force vector) and x=result(unknown displacements)
            //Der benyttes ikke inverse(too slow) tilgengæld LU decomposition for effektiv beregning.
            var result = matrix.Solve(vector); 
            return result.ToArray();
        }
    }

}
