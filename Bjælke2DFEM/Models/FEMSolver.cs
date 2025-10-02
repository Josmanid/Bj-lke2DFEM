using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bjælke2DFEM.Models
{


    public class FEMSolver
    {
        //Gemmer strukturen af bjælken
        private BeamMesh mesh;
        //Gemmer påførte kræfter ved hver frihedsgrad (DOF) 
        //Type: Array af doubles for numerisk præcision
        //Formål: Repræsenterer højre side af ligningssystemet F = K×d
        private double[] forceVector;
        //Gemmer beregnede forskydninger ved hver DOF
        //Formål: Indeholder løsningen (forskydninger og rotationer)
        private double[] displacements;

        public FEMSolver(BeamMesh mesh) {
            //Uden this. ville mesh = mesh; bare tildele parameteren til sig selv (gør ingenting)
            //referer altså til vores instance field
            this.mesh = mesh;
            // sætter vores dofs= 9
            int dofs = mesh.Nodes.Count * 3;
            // GOD KODE - allokerer én gang i konstruktøren
            //Sammenfatning: Hukommelsesallokering er som at reservere pladser i computerens "lager". 
            //Jo færre gange du skal reservere/frigive, jo hurtigere kører dit program.
            // Fylder et array med 6 nuller
            forceVector = new double[dofs];
            displacements = new double[dofs];
        }

        /// <summary>
        /// Denne metode påfører en punktbelastning (kraft) på et specifikt knudepunkt i bjælkestrukturen.
        /// </summary>
        /// <param name="nodeIndex">Dette er nummeret på det knudepunkt hvor kraften skal påføres</param>
        /// <param name="force">Kraften målt i Newton (N) eller kilonewton (kN)</param>
        public void ApplyPointLoad(int nodeIndex, double forceX, double forceY, double moment ) {
            //Vi peger her på forskydningens frihedsgraden.
            //Dette giver også vores vector/array til metoder ExtractSubvector()
            // sætter et givent sted på vores array/vector til en hvis kraft.
            //Det sted på denne vector er nu...
            forceVector[3 * nodeIndex] = forceX; // ux DOF
            forceVector[3 * nodeIndex + 1] = forceY; //uy DOF
            forceVector[3 * nodeIndex + 2] = moment; //θz DOF

        }

        public void Solve() {
            //TRIN 1: Opsætning og Initialisering
            //Vi fylder en tom matrix med nuller store K
            int size = mesh.Nodes.Count * 3;
            double[,] K = new double[size, size];

            // Saml global stivhedsmatrix
            for (int e = 0; e < mesh.Elements.Count; e++)
            {
                BeamElement elem = mesh.Elements[e];
                double[,] k_local = elem.GetLocalStiffnessMatrix(); // fra BeamELement klassen
                double phi = elem.Angle; // angle in radians, you must have this stored in BeamElement
                double[,] k_global_elem = MatrixUtils.TransformLocalToGlobal(k_local, phi);

                int i = elem.StartNode.Id;
                int j = elem.EndNode.Id;

                int[] dofs = { 3 * i, 3 * i + 1, 3 * i + 2, 3 * j, 3 * j + 1, 3 * j + 2 };
                // vi fylder vores lokale stivhedsmatrix ind i den globale Matrix
                for (int m = 0; m < 6; m++)
                    for (int n = 0; n < 6; n++)
                        K[dofs[m], dofs[n]] += k_global_elem[m, n];
            }

            // Anvend randbetingelser (fastspændt ved node 0) derfor minus de 3 første frihedsgrader
            //skal fastsættes på samme sted som ved 1D
            int[] freeDofs = new int[size - 3];
            for (int i = 0; i < freeDofs.Length; i++)
                freeDofs[i] = i + 3;

            //vi ved jo at første node er indspændt derfor ingen grund til at have dem med i regnestykket
            double[,] K_reduced = MatrixUtils.ExtractSubmatrix(K, freeDofs);
            double[] f_reduced = MatrixUtils.ExtractSubvector(forceVector, freeDofs);

            double[] d_reduced = MatrixUtils.SolveLinearSystem(K_reduced, f_reduced);
            // loop is for putting the solved reduced displacement vector
            // d_reduced back into the full displacements vector at the correct DOF positions.
            for (int i = 0; i < freeDofs.Length; i++)
                displacements[freeDofs[i]] = d_reduced[i];
        }

        public void PrintDisplacements() {
            for (int i = 0; i < mesh.Nodes.Count; i++)
                Console.WriteLine(
            $"Node {i}: uX = {displacements[3 * i]:F6} m, " +
            $"uY = {displacements[3 * i + 1]:F6} m, " +
            $"thetaZ = {displacements[3 * i + 2]:F6} rad"
        );

        }
    }

}
