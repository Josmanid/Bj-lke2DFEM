using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Bjælke2DFEM.Models
{
    /// <Formål>
    /// Klassen BeamMesh bygger hele modellen: både noderne og bjælkeelementerne mellem dem.
    /// </summary>

    public class BeamMesh
    {
        //En liste over alle noder i modellen(punkter på bjælken). som også bliver initialiseret her.
        public List<Node> Nodes { get; } = new();

        public List<BeamElement> Elements { get; } = new();
        //Giver indekset for sidste node (bruges ofte til at identificere randbetingelser m.m.).
        //første del er declaration af en type int. fra => og frem betyder:
        //Når nogen spørger om værdien af LastNodeIndex, så returnér Nodes.Count - 1.
        //Da den skal pege på forskydningen
        public int LastNodeIndex => Nodes.Count - 1;

        public BeamMesh(double startX, double startY, double endX, double endY,
                   int nElements, double E, double I, double A) {
            double dx = (endX - startX) / nElements;
            double dy = (endY - startY) / nElements;

            // Create the nodes
            for (int i = 0; i <= nElements; i++)
                Nodes.Add(new Node(i, startX + i * dx, startY + i * dy));

            // Create the elements
            for (int i = 0; i < nElements; i++)
                Elements.Add(new BeamElement(Nodes[i], Nodes[i + 1], E, I, A));
        }

    }

}
