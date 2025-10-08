


// 1. Opret Mesh
using Bjælke2DFEM.Models;

double startX = 0.0;
double startY = 0.0;
double endX = 1.0;  
double endY = 0.0;  

int nElements = 8;
double E = 200e9;           // Elastic modulus for stål (Pa)
double I = 6.67e-10;        // Moment of inertia (m^4)
double A = 8e-4;           // Cross-sectional areal (m^2)

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