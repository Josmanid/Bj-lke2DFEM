using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bjælke2DFEM.Models
{
    /// <summary>
    /// Repræsenterer en enkelt node (knudepunkt) i en 2D bjælkestruktur til Finite Element Method (FEM) analyse.
    /// 
    /// En node definerer et punkt i 2D-rummet hvor elementer forbindes, og hvor vi beregner forskydninger og rotationer.
    /// I 2D bjælketeori har hver node 3 frihedsgrader (DOF - Degrees of Freedom):
    /// - Horizontal forskydning (u_x)
    /// - Vertikal forskydning (u_y) 
    /// - Rotation om z-aksen (θ_z)
    /// 
    /// Eksempel: En cantilever bjælke opdelt i 4 elementer vil have nodes ved X = 0, 0.25, 0.5, 0.75, 1.0
    /// </summary>
    public class Node
    {
        #region Properties - Geometri (Read-only efter konstruktion)

        /// <summary>
        /// Unik identifikator for noden. Bruges til at referere til noden i element-definitioner og boundary conditions.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// X-position - hvor noden sidder langs bjælken.
        /// 
        /// Som at måle afstand langs en lineal:
        /// - X = 0: ved indspændingen (der hvor du holder linealen fast)
        /// - X = 0.5: midt på bjælken
        /// - X = 1.0: ved den frie ende (der hvor du kan bøje linealen)
        /// 
        /// Eksempel: En 2 meter cantilever bjælke opdelt i 4 elementer har nodes ved X = 0, 0.5, 1.0, 1.5, 2.0
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Y-position - hvor noden sidder i højden.
        /// 
        /// For en lige cantilever bjælke (som en lige lineal):
        /// - Y = 0 for alle nodes når bjælken er lige og ikke belastet
        /// - Kun når bjælken bøjer, ændrer nodernes faktiske position sig (via DisplacementY)
        /// 
        /// Eksempel: En vandret træbjælke har Y = 0 for alle nodes i udgangspositionen
        /// </summary>
        public double Y { get; }

        #endregion

        #region Properties - FEM Resultater (Beregnes af løser)

        /// <summary>
        /// Horizontal forskydning - hvor meget noden flytter sig sidelæns.
        /// 
        /// Tænk på det som:
        /// - At skubbe en lineal til siden - den flytter sig vandret
        /// - Hvis du kun belaster en bjælke lodret (som at lægge en bog på en lineal), bliver dette næsten 0
        /// - Hvis du skubber bjælken vandret (som at skubbe linealen til siden), får du denne forskydning
        /// 
        /// Eksempel: En 1m cantilever bjælke skubbet med 100N vandret kraft -> måske 2-5mm forskydning ved spidsen
        /// </summary>
        public double DisplacementX { get; set; }

        /// <summary>
        /// Vertikal forskydning - hvor meget noden bøjer op eller ned.
        /// 
        /// Det er ligesom når du:
        /// - Bøjer en lineal ved at trykke ned på den ene ende - den bøjer nedad
        /// - Holder en træpind i den ene ende og hænger en vægt på den anden - spidsen bøjer nedad
        /// - Kigger på en flagstang der bøjer i vinden
        /// 
        /// Dette er den klassiske "bjælke bøjning" - præcis det samme som i 1D modeller.
        /// 
        /// Eksempel: 
        /// - 1m stålllineal med 1kg vægt på spidsen -> måske 10-20mm nedadgående bøjning
        /// - Jo længere ude på bjælken, des mere bøjer den (maksimum ved den frie ende)
        /// </summary>
        public double DisplacementY { get; set; }

        /// <summary>
        /// Rotation - hvor meget noden "vipper" eller roterer.
        /// 
        /// Tænk på det som:
        /// - En lineal der ikke bare bøjer, men også "vippes" - enden peger ikke vandret længere
        /// - Som en vippe på en legeplads - den ene ende går op, den anden ned, og midten roterer
        /// - En flagstang der ikke kun bøjer, men spidsen også "peger" i en anden retning
        /// 
        /// Præcis det samme som rotation i 1D bjælkemodeller - samme "vippe-effekt".
        /// 
        /// Eksempel:
        /// - 1m lineal med vægt på spidsen: spidsen bøjer 20mm ned OG roterer måske 0.02 radianer (~1 grad)
        /// - Jo mere bjælken bøjer, des mere roterer den også
        /// - Ved indspændingen (cantilever): rotation = 0 (fastholdt)
        /// - Ved den frie ende: maksimal rotation
        /// </summary>
        public double Rotation { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Opretter en ny node med specificeret ID og position.
        /// Forskydninger og rotation initialiseres til 0 og beregnes senere af FEM-løseren.
        /// </summary>
        /// <param name="id">Unik identifikator for noden</param>
        /// <param name="x">X-koordinat i globalt koordinatsystem</param>
        /// <param name="y">Y-koordinat i globalt koordinatsystem</param>
        public Node(int id, double x, double y) {
            Id = id;
            X = x;
            Y = y;
            // Displacement og Rotation initialiseres automatisk til 0
        }

        #endregion
    }

}
