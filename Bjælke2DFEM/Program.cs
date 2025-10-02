/// Tasks for 2D Cantilever FEM
/// 1. Expand the DOFs per node
///    - Each node in 2D has displacement in X, displacement(Bend sideways) in Y, and rotation θz
///    - 3 DOFs per node → 6×6 stiffness matrix for 2-node elements
///
/// 2. Make the element stiffness matrix bigger
///    - From 4×4 (1D beam) to 6×6 (2D beam)
///    - Include axial + bending stiffness
///
/// 3. Add rotation transformation
///    - Compute stiffness in local coordinates
///    - Use transformation matrix T to rotate into global coordinates


// 1. Opret Mesh
using Bjælke2DFEM.Models;

double startX = 0.0;
double startY = 0.0;
double endX = 1.0;  
double endY = 0.0;  

int nElements = 8;
double E = 200e9;           // Elastic modulus for steel (Pa)
double I = 6.67e-10;        // Moment of inertia (m^4)
double A = 8e-4;           // Cross-sectional area (m^2)

BeamMesh mesh = new BeamMesh(startX, startY, endX, endY,nElements, E,I,A);

// 2. Opret FEM-Solver
FEMSolver solver = new FEMSolver(mesh);

// 3. Opret kraft
double force = -100.0;
solver.ApplyPointLoad(2,0.0, force,0.0); // Anvend kraft på node 2 (3. node i 0-indekseret liste)

// 4. løs systemet
solver.Solve();

// 5. vis resultatet
solver.PrintDisplacements();


Console.ReadKey();