using MathNet.Numerics.Distributions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Bjælke2DFEM.Models
{
    /// <Formål>
    /// Et BeamElement repræsenterer ét finitte element i bjælken. I en 2D bjælkemodel 
    /// er det det “stykke” af bjælken mellem to noder vi kører lineElement.
    /// Typisk gør et element dette:
    /// Holder styr på sine to tilknyttede noder(start og slut)
    /// Beregner sin lokale stivhedsmatrix ud fra geometri og materialeegenskaber
    /// Bidrager til den globale matrix i FEM-løsningen
    public class BeamElement
    {
        public Node StartNode { get; }
        public Node EndNode { get; }
        //Benytter pythagoras for at finde afstanden i 2D. Men kunne godt have taget absolut værdien
        //Da vi kun har en figur i X planen.
        public double Length => Math.Sqrt(Math.Pow(EndNode.X - StartNode.X, 2) + Math.Pow(EndNode.Y - StartNode.Y, 2));
        public double E { get; } // Young's Modulus (materialets stivhed – fx stål = 200 GPa)
        public double I { get; } //Arealinertimoment (bjælkens modstand mod bøjning)
        public double A { get; }  // Tværsnitsareal altså tykkelse, bredde eller højde — til bjælken.
        //beregner allerede Length. Vinklen her er bare udgangspunktet
        public double Angle => Math.Atan2(EndNode.Y - StartNode.Y, EndNode.X - StartNode.X);



        //Når et BeamElement oprettes,
        //skal man give start- og slutnoden samt materialets elasticitetsmodul E, arealinertimoment I og dens tværsnitareal.
        //Disse sættes til objektets properties.
        public BeamElement(Node start, Node end, double E, double I, double A) {
            StartNode = start;
            EndNode = end;
            this.E = E;
            this.I = I;
            this.A = A;
        }

        public double[,] GetLocalStiffnessMatrix() {
            // Beregner bjælkens længde i 2D baseret på start- og slutnoden (Pythagoras)
            // Dette er mere generelt end kun forskel i X-led.
            double L = Length;

            // Axial (træk-/tryk-) stivhedskoefficient
            // E*A/L: Elasticitetsmodul * tværsnitsareal delt med længde
            // Beskriver bjælkens modstand mod aksiale deformationer (længdeændringer)
            double axialFactor = E * A / L;

            // Bøjningens stivhedskoefficient
            // E*I/L^3: Elasticitetsmodul * inertimoment delt med længde^3
            // Beskriver bjælkens modstand mod bøjning og rotation
            double bendingFactor = E * I / Math.Pow(L, 3);

            // Her returneres den lokale stivhedsmatrix som en 6x6 matrix.
            // Hver node har nu 3 frihedsgrader: 
            //  1) forskydning i X-retningen (aksial)
            //  2) forskydning i Y-retningen (bøjning)
            //  3) rotation omkring Z-aksen
            // Matrixen kombinerer både aksiale og bøjningseffekter.
            return new double[,]
            {
        // Række 0: Kraft i X retning ved node 1 (Start)
        { axialFactor,       0,                0,           -axialFactor,        0,                 0 },

        // Række 1: Kraft i Y retning ved node 1 (Start)
        { 0,           12 * bendingFactor,  6 * L * bendingFactor,  0,      -12 * bendingFactor,  6 * L * bendingFactor },

        // Række 2: Moment ved node 1 (Start)
        { 0,           6 * L * bendingFactor, 4 * L * L * bendingFactor,  0,      -6 * L * bendingFactor,  2 * L * L * bendingFactor },

        // Række 3: Kraft i X retning ved node 2 (Slut)
        { -axialFactor,      0,                0,            axialFactor,        0,                 0 },

        // Række 4: Kraft i Y retning ved node 2 (Slut)
        { 0,          -12 * bendingFactor, -6 * L * bendingFactor,  0,       12 * bendingFactor, -6 * L * bendingFactor },

        // Række 5: Moment ved node 2 (Slut)
        { 0,           6 * L * bendingFactor, 2 * L * L * bendingFactor,  0,      -6 * L * bendingFactor,  4 * L * L * bendingFactor }
            };
        }

    }

}
