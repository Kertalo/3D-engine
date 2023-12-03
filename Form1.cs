using System.Numerics;
using System.Globalization;
using static System.Windows.Forms.AxHost;
using System.Drawing.Drawing2D;
using System.Drawing;
using static System.Windows.Forms.DataFormats;

namespace Rogalik_s_3D
{
    public partial class Form : System.Windows.Forms.Form
    {
        Bitmap map;
        Bitmap texture;
        Graphics graphics;
        Pen pen = new Pen(Color.White, 3);

        Random rnd = new Random();

        enum State { Position, Rotation, Scale, Draw }
        enum Plane { X, Y, Z, XYZ }
        State state = State.Position;
        Plane plane = Plane.XYZ;
        PointF mousePosition;
        List<PointF> pointsRotation = new();
        Vector3 cameraPosition = new();
        Vector3 cameraUp = new();
        Vector3 cameraRight = new();
        Vector3 cameraRotation = new();

        List<Polyhedron> polyhedrons = new List<Polyhedron>();
        bool isMouseDown = false;
        int indexPolyhedron = 0;
        bool isPerspective = true;

        private float[,] zBuffer;
        private Color[,] frameBuffer;

        private Vector3 LightSourcePosition;

        int[] HorizontalTop;
        int[] HorizontalDown;

        public Form()
        {
            InitializeComponent();
            map = new Bitmap(pictureBox.Width, pictureBox.Height);
            graphics = Graphics.FromImage(map);

            texture = new Bitmap(map);

            LightSourcePosition = new Vector3(1, 1, 1);
            zBuffer = new float[pictureBox.Width, pictureBox.Height];
            frameBuffer = new Color[pictureBox.Width, pictureBox.Height];

            HorizontalTop = new int[map.Width];
            HorizontalDown = new int[map.Width];

            comboBox.SelectedItem = "XYZ";
            SetCamera(new(0, 0, -10), new(0, 1, 0), new(0.5f, 0, -0.5f));
            ShowPolyhedrons();
        }

        void UpdateCameraText()
        {
            uY.Text = cameraUp.Y.ToString();
            uX.Text = cameraUp.X.ToString();
            uZ.Text = cameraUp.Z.ToString();
            rY.Text = cameraRight.Y.ToString();
            rX.Text = cameraRight.X.ToString();
            rZ.Text = cameraRight.Z.ToString();
        }

        private void SetCamera(Vector3 point, Vector3 vector1, Vector3 vector2)
        {
            cameraPosition = point;
            cameraUp = Normalize(vector1);
            cameraRight = Normalize(vector2);
            pX.Text = cameraPosition.X.ToString();
            pY.Text = cameraPosition.Y.ToString();
            pZ.Text = cameraPosition.Z.ToString();
            UpdateCameraText();
        }

        private Polyhedron Tetrahedron(float side, Vector3 point)
        {
            float halfSide = side / 2;
            float upY = (float)Math.Sqrt(6) / 4 * side;
            float downY = (float)(1 / (2 * Math.Sqrt(6))) * side;
            side = (float)(1 / (2 * Math.Sqrt(3))) * side;

            Vector3[] points = new Vector3[4];
            points[0] = new Vector3(-halfSide, -downY, -side);
            points[1] = new Vector3(halfSide, -downY, -side);
            points[2] = new Vector3(0, -downY, 2 * side);
            points[3] = new Vector3(0, upY, 0);

            Polygon[] polygons = new Polygon[4];
            polygons[0] = new(new int[] { 0, 1, 2 }, NormolizeVectorCrossProduct(points[0], points[1], points[2]));
            polygons[1] = new(new int[] { 0, 3, 1 }, NormolizeVectorCrossProduct(points[0], points[3], points[1]));
            polygons[2] = new(new int[] { 1, 2, 3 }, NormolizeVectorCrossProduct(points[1], points[2], points[3]));
            polygons[3] = new(new int[] { 0, 3, 2 }, NormolizeVectorCrossProduct(points[0], points[3], points[2]));

            var color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[0].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[1].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[2].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[3].Color = color;

            polygons[0].TextureCoord.Add(new Vector2(0, 0));
            polygons[0].TextureCoord.Add(new Vector2(1, 0));
            polygons[0].TextureCoord.Add(new Vector2(1, 1));

            polygons[1].TextureCoord.Add(new Vector2(0, 0));
            polygons[1].TextureCoord.Add(new Vector2(1, 0));
            polygons[1].TextureCoord.Add(new Vector2(1, 1));

            polygons[2].TextureCoord.Add(new Vector2(0, 0));
            polygons[2].TextureCoord.Add(new Vector2(1, 0));
            polygons[2].TextureCoord.Add(new Vector2(1, 1));

            polygons[3].TextureCoord.Add(new Vector2(0, 0));
            polygons[3].TextureCoord.Add(new Vector2(1, 0));
            polygons[3].TextureCoord.Add(new Vector2(1, 1));

            Polyhedron polyhedron = new(point, points, polygons);
            return polyhedron;
        }

        private Polyhedron Hexahedron(float side, Vector3 point)
        {
            side = side / 2;

            Vector3[] points = new Vector3[8];
            points[0] = new Vector3(-side, -side, -side);
            points[1] = new Vector3(side, -side, -side);
            points[2] = new Vector3(side, -side, side);
            points[3] = new Vector3(-side, -side, side);
            points[4] = new Vector3(-side, side, -side);
            points[5] = new Vector3(side, side, -side);
            points[6] = new Vector3(side, side, side);
            points[7] = new Vector3(-side, side, side);

            Polygon[] polygons = new Polygon[12];
            polygons[0] = new(new int[] { 0, 1, 2 }, NormolizeVectorCrossProduct(points[0], points[1], points[2]));
            polygons[1] = new(new int[] { 0, 2, 3 }, NormolizeVectorCrossProduct(points[0], points[2], points[3]));
            polygons[2] = new(new int[] { 0, 1, 5, }, NormolizeVectorCrossProduct(points[0], points[1], points[5]));
            polygons[3] = new(new int[] { 0, 5, 4 }, NormolizeVectorCrossProduct(points[0], points[5], points[4]));
            polygons[4] = new(new int[] { 0, 3, 7 }, NormolizeVectorCrossProduct(points[0], points[3], points[7]));
            polygons[5] = new(new int[] { 0, 7, 4 }, NormolizeVectorCrossProduct(points[0], points[7], points[4]));
            polygons[6] = new(new int[] { 1, 2, 6 }, NormolizeVectorCrossProduct(points[1], points[2], points[6]));
            polygons[7] = new(new int[] { 1, 6, 5 }, NormolizeVectorCrossProduct(points[1], points[6], points[5]));
            polygons[8] = new(new int[] { 2, 3, 7 }, NormolizeVectorCrossProduct(points[2], points[3], points[7]));
            polygons[9] = new(new int[] { 2, 7, 6 }, NormolizeVectorCrossProduct(points[2], points[7], points[6]));
            polygons[10] = new(new int[] { 4, 5, 6 }, NormolizeVectorCrossProduct(points[4], points[5], points[6]));
            polygons[11] = new(new int[] { 4, 6, 7 }, NormolizeVectorCrossProduct(points[4], points[6], points[7]));

            var color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[0].Color = color;
            polygons[1].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[2].Color = color;
            polygons[3].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[4].Color = color;
            polygons[5].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[6].Color = color;
            polygons[7].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[8].Color = color;
            polygons[9].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[10].Color = color;
            polygons[11].Color = color;

            polygons[0].TextureCoord.Add(new Vector2(0, 0));
            polygons[0].TextureCoord.Add(new Vector2(0, 1));
            polygons[0].TextureCoord.Add(new Vector2(1, 1));
            polygons[1].TextureCoord.Add(new Vector2(0, 0));
            polygons[1].TextureCoord.Add(new Vector2(1, 0));
            polygons[1].TextureCoord.Add(new Vector2(1, 1));

            polygons[2].TextureCoord.Add(new Vector2(0, 0));
            polygons[2].TextureCoord.Add(new Vector2(0, 1));
            polygons[2].TextureCoord.Add(new Vector2(1, 1));
            polygons[3].TextureCoord.Add(new Vector2(0, 0));
            polygons[3].TextureCoord.Add(new Vector2(1, 0));
            polygons[3].TextureCoord.Add(new Vector2(1, 1));

            polygons[4].TextureCoord.Add(new Vector2(0, 0));
            polygons[4].TextureCoord.Add(new Vector2(0, 1));
            polygons[4].TextureCoord.Add(new Vector2(1, 1));
            polygons[5].TextureCoord.Add(new Vector2(0, 0));
            polygons[5].TextureCoord.Add(new Vector2(1, 0));
            polygons[5].TextureCoord.Add(new Vector2(1, 1));


            polygons[6].TextureCoord.Add(new Vector2(0, 0));
            polygons[6].TextureCoord.Add(new Vector2(0, 1));
            polygons[6].TextureCoord.Add(new Vector2(1, 1));
            polygons[7].TextureCoord.Add(new Vector2(0, 0));
            polygons[7].TextureCoord.Add(new Vector2(1, 0));
            polygons[7].TextureCoord.Add(new Vector2(1, 1));

            polygons[8].TextureCoord.Add(new Vector2(0, 0));
            polygons[8].TextureCoord.Add(new Vector2(0, 1));
            polygons[8].TextureCoord.Add(new Vector2(1, 1));
            polygons[9].TextureCoord.Add(new Vector2(0, 0));
            polygons[9].TextureCoord.Add(new Vector2(1, 0));
            polygons[9].TextureCoord.Add(new Vector2(1, 1));

            polygons[10].TextureCoord.Add(new Vector2(0, 0));
            polygons[10].TextureCoord.Add(new Vector2(0, 1));
            polygons[10].TextureCoord.Add(new Vector2(1, 1));
            polygons[11].TextureCoord.Add(new Vector2(0, 0));
            polygons[11].TextureCoord.Add(new Vector2(1, 0));
            polygons[11].TextureCoord.Add(new Vector2(1, 1));

            Polyhedron polyhedron = new(point, points, polygons);
            return polyhedron;
        }

        private Polyhedron Octahedron(float side, Vector3 point)
        {
            side = (float)Math.Sqrt(side * side / 2);

            Vector3[] points = new Vector3[6];
            points[0] = new Vector3(0, 0, -side);
            points[1] = new Vector3(side, 0, 0);
            points[2] = new Vector3(0, 0, side);
            points[3] = new Vector3(-side, 0, 0);
            points[4] = new Vector3(0, side, 0);
            points[5] = new Vector3(0, -side, 0);

            Polygon[] polygons = new Polygon[8];
            polygons[0] = new(new int[] { 0, 1, 4 }, NormolizeVectorCrossProduct(points[0], points[1], points[4]));
            polygons[1] = new(new int[] { 1, 2, 4 }, NormolizeVectorCrossProduct(points[1], points[2], points[4]));
            polygons[2] = new(new int[] { 2, 3, 4 }, NormolizeVectorCrossProduct(points[2], points[3], points[4]));
            polygons[3] = new(new int[] { 3, 0, 4 }, NormolizeVectorCrossProduct(points[3], points[0], points[4]));
            polygons[4] = new(new int[] { 0, 1, 5 }, NormolizeVectorCrossProduct(points[0], points[1], points[5]));
            polygons[5] = new(new int[] { 1, 2, 5 }, NormolizeVectorCrossProduct(points[1], points[2], points[5]));
            polygons[6] = new(new int[] { 2, 3, 5 }, NormolizeVectorCrossProduct(points[2], points[3], points[5]));
            polygons[7] = new(new int[] { 3, 0, 5 }, NormolizeVectorCrossProduct(points[3], points[0], points[5]));

            var color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[0].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[1].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[2].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[3].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[4].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[5].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[6].Color = color;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            polygons[7].Color = color;


            polygons[0].TextureCoord.Add(new Vector2(0, 0));
            polygons[0].TextureCoord.Add(new Vector2(0, 1));
            polygons[0].TextureCoord.Add(new Vector2(1, 1));

            polygons[1].TextureCoord.Add(new Vector2(0, 0));
            polygons[1].TextureCoord.Add(new Vector2(1, 0));
            polygons[1].TextureCoord.Add(new Vector2(1, 1));

            polygons[2].TextureCoord.Add(new Vector2(0, 0));
            polygons[2].TextureCoord.Add(new Vector2(0, 1));
            polygons[2].TextureCoord.Add(new Vector2(1, 1));

            polygons[3].TextureCoord.Add(new Vector2(0, 0));
            polygons[3].TextureCoord.Add(new Vector2(1, 0));
            polygons[3].TextureCoord.Add(new Vector2(1, 1));

            polygons[4].TextureCoord.Add(new Vector2(0, 0));
            polygons[4].TextureCoord.Add(new Vector2(0, 1));
            polygons[4].TextureCoord.Add(new Vector2(1, 1));

            polygons[5].TextureCoord.Add(new Vector2(0, 0));
            polygons[5].TextureCoord.Add(new Vector2(1, 0));
            polygons[5].TextureCoord.Add(new Vector2(1, 1));

            polygons[6].TextureCoord.Add(new Vector2(0, 0));
            polygons[6].TextureCoord.Add(new Vector2(0, 1));
            polygons[6].TextureCoord.Add(new Vector2(1, 1));

            polygons[7].TextureCoord.Add(new Vector2(0, 0));
            polygons[7].TextureCoord.Add(new Vector2(1, 0));
            polygons[7].TextureCoord.Add(new Vector2(1, 1));

            Polyhedron polyhedron = new(point, points, polygons);
            return polyhedron;
        }

        private Vector3 Equation(float x, float y)
        {
            return new(x, y, (float)(x*x + y*y + 1));
        }

        private Polyhedron Graph(Vector3 point, float x1, float x2, float y1, float y2, int split)
        {
            List<Polygon> polygons = new();
            List<Vector3> points = new();

            int pointsCount = split + 1;
            float distX = Math.Abs(x2 - x1);
            float distY = Math.Abs(y2 - y1);
            float x = x1;

            for (int i = 0; i < pointsCount; i++)
            {
                float y = y1;
                for (int j = 0; j < pointsCount; j++)
                {
                    points.Add(Equation(x, y));
                    y += distY / split;
                }
                x += distX / split;
            }
            for (int i = 0; i < pointsCount - 1; i++)
            {
                for (int j = 0; j < pointsCount - 1; j++)
                {
                    int newIndex = j * pointsCount + i;
                    polygons.Add(new(new int[]{ newIndex, newIndex + 1,
                        newIndex + 1 + pointsCount, newIndex + pointsCount }));
                }
            }
            return new(point, points.ToArray(), polygons.ToArray());
        }

        private void DrawLine(Vector3 p1, Vector3 p2)
        {
            graphics.DrawLine(pen, new PointF(map.Width / 2 + p1.X * 100, map.Height / 2 - p1.Y * 100),
                new PointF(map.Width / 2 + p2.X * 100, map.Height / 2 - p2.Y * 100));
        }

        private Vector3 NormolizeVectorCrossProduct(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 result = new Vector3();

            Vector3 v1 = new Vector3(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            Vector3 v2 = new Vector3(p3.X - p1.X, p3.Y - p1.Y, p3.Z - p1.Z);

            result.X = v1.Y * v2.Z - v1.Z * v2.Y;
            result.Y = v1.Z * v2.X - v1.X * v2.Z;
            result.Z = v1.X * v2.Y - v1.Y * v2.X;

            return Normalize(result);
        }

        private Vector3 Normalize(Vector3 vector)
        {
            float length = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            return new Vector3(vector.X / length, vector.Y / length, vector.Z / length);
        }

        float[] Multiplication(Vector3 point, float[,] matrix)
        {
            float[] newPoint = new float[4];
            float[] pointArray = new float[]
                { point.X, point.Y, point.Z, 1 };
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    newPoint[i] += pointArray[j] * matrix[j, i];
            return newPoint;
        }

        float[,] MultiplicationMatrix(float[,] matrix1, float[,] matrix2)
        {
            float[,] resMatrix = new float[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    for (int k = 0; k < 4; k++)
                        resMatrix[i, j] += matrix1[i, k] * matrix2[k, j];
            return resMatrix;
        }

        void Shift(ref Polyhedron polyhedron, Vector3 vector)
        {
            float[,] matrix = new float[,]
                { {  1,  0, 0, 0 },
                  {  0,  1, 0, 0 },
                  {  0,  0, 1, 0 },
                  { vector.X, vector.Y, 0, 1 } };

            polyhedron.point = new(Multiplication(polyhedron.point, matrix));
        }

        void Rotate(ref Polyhedron polyhedron, Vector3 vector)
        {
            float[,] matrix = new float[4, 4];
            matrix[0, 0] = 1;
            matrix[1, 1] = 1;
            matrix[2, 2] = 1;
            matrix[3, 3] = 1;

            if (vector.X != 0)
            {
                float[,] addMatrix = new float[4, 4];
                addMatrix[0, 0] = 1;
                addMatrix[1, 1] = (float)Math.Cos(vector.X * Math.PI);
                addMatrix[2, 2] = (float)Math.Cos(vector.X * Math.PI);
                addMatrix[2, 1] = (float)Math.Sin(vector.X * Math.PI);
                addMatrix[1, 2] = -(float)Math.Sin(vector.X * Math.PI);
                addMatrix[3, 3] = 1;
                matrix = MultiplicationMatrix(matrix, addMatrix);
            }
            else if (vector.Y != 0)
            {
                float[,] addMatrix = new float[4, 4];
                addMatrix[1, 1] = 1;
                addMatrix[0, 0] = (float)Math.Cos(-vector.Y * Math.PI);
                addMatrix[2, 2] = (float)Math.Cos(-vector.Y * Math.PI);
                addMatrix[2, 0] = (float)Math.Sin(-vector.Y * Math.PI);
                addMatrix[0, 2] = -(float)Math.Sin(-vector.Y * Math.PI);
                addMatrix[3, 3] = 1;
                matrix = MultiplicationMatrix(matrix, addMatrix);
            }
            else if (vector.Z != 0)
            {
                float[,] addMatrix = new float[4, 4];
                addMatrix[2, 2] = 1;
                addMatrix[0, 0] = (float)Math.Cos(vector.Z * Math.PI);
                addMatrix[1, 1] = (float)Math.Cos(vector.Z * Math.PI);
                addMatrix[1, 0] = (float)Math.Sin(vector.Z * Math.PI);
                addMatrix[0, 1] = -(float)Math.Sin(vector.Z * Math.PI);
                addMatrix[3, 3] = 1;
                matrix = MultiplicationMatrix(matrix, addMatrix);
            }

            for (int i = 0; i < polyhedron.points.Length; i++)
                polyhedron.points[i] = new(Multiplication(polyhedron.points[i], matrix));
        }

        void Scale(ref Polyhedron polyhedron, float scale, Plane xyz)
        {
            float[,] matrix = new float[4, 4];
            int xyzI = xyz == Plane.X ? 0 : xyz == Plane.Y ? 1 : 2;
            matrix[0, 0] = 1;
            matrix[1, 1] = 1;
            matrix[2, 2] = 1;
            matrix[xyzI, xyzI] = scale;
            if (xyz == Plane.XYZ)
            {
                matrix[0, 0] = scale;
                matrix[1, 1] = scale;
                matrix[2, 2] = scale;
            }
            matrix[3, 3] = 1;

            for (int i = 0; i < polyhedron.points.Length; i++)
                polyhedron.points[i] = new(Multiplication(polyhedron.points[i], matrix));
        }

        void AxonometricProjection()
        {
            cameraRotation = Normalize(new(-cameraRight.Z, 0, cameraRight.X));
            Vector3[][] points = new Vector3[polyhedrons.Count][];
            for (int i = 0; i < polyhedrons.Count; i++)
            {
                points[i] = new Vector3[polyhedrons[i].points.Length];
                for (int j = 0; j < points[i].Length; j++)
                    points[i][j] = new Vector3(polyhedrons[i].points[j].X,
                        polyhedrons[i].points[j].Y, polyhedrons[i].points[j].Z);
            }

            Polyhedron[] newPolyhedrons = new Polyhedron[polyhedrons.Count];
            for (int i = 0; i < polyhedrons.Count; i++)
                newPolyhedrons[i] = new(polyhedrons[i].point, points[i], polyhedrons[i].polygons);

            float[,] matrix = { { cameraRight.X, cameraRight.Y, cameraRight.Z, 0 },
                                { cameraUp.X, cameraUp.Y, cameraUp.Z, 0 },
                                { cameraRotation.X, cameraRotation.Y, cameraRotation.Z, 0 },
                                { 0, 0, 0, 1 } };

            float[,] matrix2 = { { 1, 0, 0, -cameraPosition.X },
                                 { 0, 1, 0, -cameraPosition.Y },
                                 { 0, 0, 1, -cameraPosition.Z },
                                 { 0, 0, 0, 1 } };

            matrix = MultiplicationMatrix(matrix, matrix2);

            for (int i = 0; i < newPolyhedrons.Count(); i++)
                for (int j = 0; j < newPolyhedrons[i].points.Length; j++)
                    newPolyhedrons[i].points[j] = new(Multiplication(newPolyhedrons[i].points[j], matrix));

            foreach (var polyhedron in newPolyhedrons)
                for (int i = 0; i < polyhedron.polygons.Length; i++)
                {
                    for (int j = 0; j < polyhedron.polygons[i].indexes.Length - 1; j++)
                        DrawLine(polyhedron.points[polyhedron.polygons[i].indexes[j]] + polyhedron.point - cameraPosition,
                            polyhedron.points[polyhedron.polygons[i].indexes[j + 1]] + polyhedron.point - cameraPosition);
                    DrawLine(polyhedron.points[polyhedron.polygons[i].indexes[polyhedron.polygons[i].indexes.Length - 1]] + polyhedron.point - cameraPosition,
                            polyhedron.points[polyhedron.polygons[i].indexes[0]] + polyhedron.point - cameraPosition);
                }

            pictureBox.Image = map;
        }

        void PerspectiveProjection()
        {
            float[,] matrix = new float[4, 4];
            matrix[0, 0] = 1;
            matrix[1, 1] = 1;
            matrix[3, 3] = 1;

            Vector3[][] points = new Vector3[polyhedrons.Count][];
            for (int i = 0; i < polyhedrons.Count; i++)
            {
                points[i] = new Vector3[polyhedrons[i].points.Length];
                for (int j = 0; j < points[i].Length; j++)
                {
                    matrix[2, 3] = -1 / (polyhedrons[i].point.Z + polyhedrons[i].points[j].Z - cameraPosition.Z);
                    float[] newPoint = Multiplication(polyhedrons[i].points[j], matrix);
                    points[i][j] = new Vector3(newPoint[0] / newPoint[3], newPoint[1] / newPoint[3], newPoint[2])
                        + polyhedrons[i].point - cameraPosition;
                }
            }

            for (int i = 0; i < polyhedrons.Count; i++)
                for (int j = 0; j < polyhedrons[i].polygons.Length; j++)
                {
                    for (int l = 0; l < polyhedrons[i].polygons[j].indexes.Length - 1; l++)
                        DrawLine(points[i][polyhedrons[i].polygons[j].indexes[l]],
                            points[i][polyhedrons[i].polygons[j].indexes[l + 1]]);
                    DrawLine(points[i][polyhedrons[i].polygons[j].indexes[polyhedrons[i].polygons[j].indexes.Length - 1]],
                            points[i][polyhedrons[i].polygons[j].indexes[0]]);
                }

            pictureBox.Image = map;
        }

        private Polyhedron RotatePoints(Vector3 point, int parts)
        {
            float[,] matrix = new float[4, 4];
            Plane xyz = plane == Plane.XYZ ? Plane.Y : plane;
            int xyzI = xyz == Plane.X ? 0 : xyz == Plane.Y ? 1 : 2;
            matrix[xyzI, xyzI] = 1;
            matrix[3, 3] = 1;
            double angle = 360.0 / parts * Math.PI / 180.0;
            if (xyz == Plane.X)
            {
                matrix[1, 1] = (float)Math.Cos(angle);
                matrix[2, 2] = (float)Math.Cos(angle);
                matrix[2, 1] = (float)Math.Sin(angle);
                matrix[1, 2] = -(float)Math.Sin(angle);
            }
            if (xyz == Plane.Y)
            {
                matrix[0, 0] = (float)Math.Cos(-angle);
                matrix[2, 2] = (float)Math.Cos(-angle);
                matrix[2, 0] = (float)Math.Sin(-angle);
                matrix[0, 2] = -(float)Math.Sin(-angle);
            }
            if (xyz == Plane.Z)
            {
                matrix[0, 0] = (float)Math.Cos(angle);
                matrix[1, 1] = (float)Math.Cos(angle);
                matrix[1, 0] = (float)Math.Sin(angle);
                matrix[0, 1] = -(float)Math.Sin(angle);
            }

            Vector3[] points = new Vector3[parts * pointsRotation.Count];

            for (int i = 0; i < pointsRotation.Count; i++)
                points[i] = new((pointsRotation[i].X - map.Width / 2) / 100,
                    -(pointsRotation[i].Y - map.Height / 2) / 100, 0);

            for (int i = 1; i < parts; i++)
                for (int j = 0; j < pointsRotation.Count; j++)
                    points[i * pointsRotation.Count + j] =
                        new(Multiplication(points[(i - 1) * pointsRotation.Count + j], matrix));

            List<Polygon> polygons = new();

            for (int i = 0; i < parts - 1; i++)
                for (int j = 0; j < pointsRotation.Count - 1; j++)
                {
                    polygons.Add(new(new int[] { i * pointsRotation.Count + j,
                        i * pointsRotation.Count + 1 + j, (i + 1) * pointsRotation.Count + j }));
                    polygons.Add(new(new int[] { i * pointsRotation.Count + 1 + j,
                        (i + 1) * pointsRotation.Count + j, (i + 1) * pointsRotation.Count + 1 + j }));
                }
            for (int j = 0; j < pointsRotation.Count - 1; j++)
            {
                polygons.Add(new(new int[] { (parts - 1) * pointsRotation.Count + j,
                    (parts - 1) * pointsRotation.Count + 1 + j, j }));
                polygons.Add(new(new int[] { j, j + 1, (parts - 1) * pointsRotation.Count + 1 + j }));
            }

            int[] side = new int[parts];
            for (int j = 0; j < parts; j++)
                side[j] = j * pointsRotation.Count;
            polygons.Add(new(side));
            side = new int[parts];
            for (int j = 0; j < parts; j++)
                side[j] = (j + 1) * pointsRotation.Count - 1;
            polygons.Add(new(side));

            pointsRotation.Clear();
            return new(point, points, polygons.ToArray());
        }

        private void ShowPolyhedrons()
        {
            graphics.Clear(pictureBox.BackColor);
            if (isPerspective)
                PerspectiveProjection();
            else
                AxonometricProjection();
            if (pointsRotation.Count == 0)
                return;
            if (pointsRotation.Count == 1)
                graphics.DrawEllipse(pen, pointsRotation[0].X, pointsRotation[0].Y, 3, 3);
            else
                for (int i = 0; i < pointsRotation.Count - 1; i++)
                    graphics.DrawLine(pen, pointsRotation[i].X, pointsRotation[i].Y,
                        pointsRotation[i + 1].X, pointsRotation[i + 1].Y);
        }

        private void PointsDistance(PointF p1, PointF p2, out float dx, out float dy)
        {
            dx = p2.X - p1.X;
            dy = p2.Y - p1.Y;
        }

        private void PictureBoxMouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            mousePosition = e.Location;

            if (state == State.Draw)
            {
                pointsRotation.Add(mousePosition);
                ShowPolyhedrons();
            }
        }

        private void PictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            /*x += 0.5f;
            if (x >= 360)
                x = 0;
            double temp_x = x / 180 * Math.PI;
            SetCamera(cameraPosition, new(0, 1, 0), new((float)Math.Cos(temp_x), 0, (float)Math.Sin(temp_x)));
            ShowPolyhedrons();*/
            if (!isMouseDown || polyhedrons.Count() < 1 || state == State.Draw)
                return;

            PointsDistance(mousePosition, e.Location, out float dx, out float dy);
            Polyhedron polyhedron = polyhedrons[indexPolyhedron];

            if (state == State.Position)
                Shift(ref polyhedron, new(dx / 100, -dy / 100, 0));
            else if (state == State.Rotation)
            {
                if (plane == Plane.XYZ)
                    Rotate(ref polyhedron, new(-dy / 1800, -dx / 1800, 0));
                else if (plane == Plane.X)
                    Rotate(ref polyhedron, new((dx + dy) / 1800, 0, 0));
                else if (plane == Plane.Y)
                    Rotate(ref polyhedron, new(0, (dx + dy) / 1800, 0));
                else if (plane == Plane.Z)
                    Rotate(ref polyhedron, new(0, 0, (dx + dy) / 1800));
            }
            else if (state == State.Scale)
                Scale(ref polyhedron, (dx + dy) < 0 ? 1 / (1 +
                    (-(dx - dy) / 100)) : 1 + (dx - dy) / 100,
                    plane);
            polyhedrons[indexPolyhedron] = polyhedron;
            ShowPolyhedrons();
            mousePosition = e.Location;
        }

        private void PictureBoxMouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void PolyhedronClick(object sender, EventArgs e)
        {
            if (sender.Equals(polyhedron1))
                polyhedrons.Add(Tetrahedron(1, new()));
            else if (sender.Equals(polyhedron2))
                polyhedrons.Add(Hexahedron(1, new()));
            else if (sender.Equals(polyhedron3))
                polyhedrons.Add(Octahedron(1, new()));

            indexPolyhedron = polyhedrons.Count - 1;

            ShowPolyhedrons();
        }

        private void StateClick(object sender, EventArgs e)
        {
            if (state == State.Draw)
                pointsRotation.Clear();

            if (sender.Equals(position))
                state = State.Position;
            else if (sender.Equals(rotation))
                state = State.Rotation;
            else if (sender.Equals(scale))
                state = State.Scale;
            else
                state = State.Draw;
        }

        private void ProjectionClick(object sender, EventArgs e)
        {
            if (sender.Equals(axonometric))
                isPerspective = false;
            else
                isPerspective = true;
            ShowPolyhedrons();
        }

        private void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox.SelectedItem.ToString() == "X")
                plane = Plane.X;
            else if (comboBox.SelectedItem.ToString() == "Y")
                plane = Plane.Y;
            else if (comboBox.SelectedItem.ToString() == "Z")
                plane = Plane.Z;
            else if (comboBox.SelectedItem.ToString() == "XYZ")
                plane = Plane.XYZ;
        }

        private void DownloadClick(object sender, EventArgs e)
        {
            // Открытие диалогового окна для выбора файла для загрузки
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "3D objects (*.obj)|*.obj";
            openFileDialog.ShowDialog();

            // Если файл был выбран, загрузка модели
            if (openFileDialog.FileName != "")
            {
                // Создание потока чтения из файла
                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    string line;
                    List<Vector3> points = new();
                    List<Polygon> polygons = new();
                    // Чтение файла построчно
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] split = line.Split(' ');
                        if (split[0] == "v")
                            points.Add(new(float.Parse(split[1], CultureInfo.InvariantCulture),
                                float.Parse(split[2], CultureInfo.InvariantCulture),
                                float.Parse(split[3], CultureInfo.InvariantCulture)));
                        else if (split[0] == "f")
                        {
                            List<int> polygonIndexes = new();
                            for (int i = 1; i < split.Length; i++)
                            {
                                if (split[i].Contains('/'))
                                    polygonIndexes.Add(int.Parse(split[i][..split[i].IndexOf('/')]) - 1);
                                else
                                    polygonIndexes.Add(int.Parse(split[i]) - 1);
                            }
                            polygons.Add(new(polygonIndexes.ToArray()));
                        }
                    }
                    polyhedrons.Add(new(new(), points.ToArray(), polygons.ToArray()));
                }
                indexPolyhedron = polyhedrons.Count - 1;
                // Отображение загруженной модели
                ShowPolyhedrons();
            }
        }

        private void UploadClick(object sender, EventArgs e)
        {
            if (polyhedrons.Count == 0)
                return;
            // Открытие диалогового окна для выбора места сохранения файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "3D objects (*.obj)|*.obj";
            saveFileDialog.ShowDialog();

            // Если файл был выбран, сохранение модели
            if (saveFileDialog.FileName != "")
            {
                // Создание потока записи в файл
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    // Запись информации о каждой грани многогранника в файл
                    foreach (var point in polyhedrons[indexPolyhedron].points)
                    {
                        writer.WriteLine("v " + string.Format(CultureInfo.InvariantCulture, "{0:f}", point.X) + " "
                            + string.Format(CultureInfo.InvariantCulture, "{0:f}", point.Y) + " "
                            + string.Format(CultureInfo.InvariantCulture, "{0:f}", point.Z));
                    }
                    foreach (Polygon polygon in polyhedrons[indexPolyhedron].polygons)
                    {
                        writer.Write("f");
                        foreach (var index in polygon.indexes)
                            writer.Write(" " + (index + 1));
                        writer.Write("\n");
                    }
                }
            }
        }

        private void GraphClick(object sender, EventArgs e)
        {
            if (int.TryParse(graphX1.Text, out int x1) &&
                int.TryParse(graphX2.Text, out int x2) && x1 < x2 &&
                int.TryParse(graphY1.Text, out int y1) &&
                int.TryParse(graphY2.Text, out int y2) && y1 < y2 &&
                int.TryParse(graphSplit.Text, out int split) && split >= 2 && split <= 100)
            {
                polyhedrons.Add(Graph(new(), x1, x2, y1, y2, split));

                indexPolyhedron = polyhedrons.Count - 1;
                ShowPolyhedrons();
            }
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            if (polyhedrons.Count < 1)
                return;

            polyhedrons.RemoveAt(indexPolyhedron);
            if (indexPolyhedron >= polyhedrons.Count)
                indexPolyhedron = polyhedrons.Count - 1;
            ShowPolyhedrons();
        }

        private void LeftRightClick(object sender, EventArgs e)
        {
            if (sender.Equals(left))
            {
                indexPolyhedron--;
                if (indexPolyhedron < 0)
                    indexPolyhedron = polyhedrons.Count - 1;
            }
            else
            {
                indexPolyhedron++;
                if (indexPolyhedron >= polyhedrons.Count)
                    indexPolyhedron = 0;
            }
        }

        private void CreateRotationButton(object sender, EventArgs e)
        {
            if (pointsRotation.Count == 0 || !int.TryParse(rotationSplit.Text, out int split))
                return;

            polyhedrons.Add(RotatePoints(new(), split));
            indexPolyhedron = polyhedrons.Count - 1;
            state = State.Position;
            ShowPolyhedrons();
        }

        private void TransformOkClick(object sender, EventArgs e)
        {
            if (float.TryParse(pX.Text, out float px) &&
                float.TryParse(pY.Text, out float py) &&
                float.TryParse(pZ.Text, out float pz) &&
                float.TryParse(uX.Text, out float ux) &&
                float.TryParse(uY.Text, out float uy) &&
                float.TryParse(uZ.Text, out float uz) &&
                float.TryParse(rX.Text, out float rx) &&
                float.TryParse(rY.Text, out float ry) &&
                float.TryParse(rZ.Text, out float rz))
            {
                cameraPosition = new(px, py, pz);
                cameraUp = Normalize(new(ux, uy, uz));
                cameraRight = Normalize(new(rx, ry, rz));

                UpdateCameraText();

                ShowPolyhedrons();
            }
        }

        private void zBufferButton_Click(object sender, EventArgs e)
        {
            zBuffer = new float[pictureBox.Width, pictureBox.Height];
            frameBuffer = new Color[pictureBox.Width, pictureBox.Height];

            ClearBuffers();
            graphics.Clear(pictureBox.BackColor);

            foreach (Polyhedron polyhedron in polyhedrons)
            {
                foreach (Polygon poly in polyhedron.polygons)
                {
                    Vector3[] vertex = new Vector3[3];
                    for (int i = 0; i < 3; i++)
                        vertex[i] = polyhedron.points[poly.indexes[i]];

                    var p1 = vertex[0];
                    p1.X = (int)(map.Width / 2 + p1.X * 100 + polyhedron.point.X * 100);
                    p1.Y = (int)(map.Height / 2 - p1.Y * 100 - polyhedron.point.Y * 100);
                    p1.Z = (int)(p1.Z * 100);

                    var p2 = vertex[1];
                    p2.X = (int)(map.Width / 2 + p2.X * 100 + polyhedron.point.X * 100);
                    p2.Y = (int)(map.Height / 2 - p2.Y * 100) - polyhedron.point.Y * 100;
                    p2.Z = (int)(p2.Z * 100);

                    var p3 = vertex[2];
                    p3.X = (int)(map.Width / 2 + p3.X * 100 + polyhedron.point.X * 100);
                    p3.Y = (int)(map.Height / 2 - p3.Y * 100 - polyhedron.point.Y * 100);
                    p3.Z = (int)(p3.Z * 100);

                    ConvertToRaster(ref p1, ref p2, ref p3, poly.Color);
                }
            }

            for (int i = 0; i < pictureBox.Width; i++)
                for (int j = 0; j < pictureBox.Height; j++)
                    map.SetPixel(i, j, frameBuffer[i, j]);

            pictureBox.Image = map;
        }

        private void ClearBuffers()
        {
            for (int x = 0; x < pictureBox.Width; x++)
            {
                for (int y = 0; y < pictureBox.Height; y++)
                {
                    zBuffer[x, y] = float.MinValue;
                    frameBuffer[x, y] = pictureBox.BackColor;
                }
            }
        }

        private List<int> Interpolate(int x1, int y1, int x2, int y2)
        {
            List<int> res = new List<int>();
            if (x1 == x2)
            {
                res.Add(y2);
            }
            double step = (y2 - y1) * 1.0f / (x2 - x1);
            double y = y1;
            for (int i = x1; i <= x2; i++)
            {
                res.Add((int)y);
                y += step;
            }
            return res;
        }

        private void ConvertToRaster(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, Color color)
        {
            if (p1.Y < p0.Y)
            {
                var temp = p0;
                p0 = p1;
                p1 = temp;
            }

            if (p2.Y < p0.Y)
            {
                var temp = p0;
                p0 = p2;
                p2 = temp;
            }

            if (p2.Y < p1.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            var x01 = Interpolate((int)p0.Y, (int)p0.X, (int)p1.Y, (int)p1.X);
            var x12 = Interpolate((int)p1.Y, (int)p1.X, (int)p2.Y, (int)p2.X);
            var x02 = Interpolate((int)p0.Y, (int)p0.X, (int)p2.Y, (int)p2.X);

            var z01 = Interpolate((int)p0.Y, (int)p0.Z, (int)p1.Y, (int)p1.Z);
            var z12 = Interpolate((int)p1.Y, (int)p1.Z, (int)p2.Y, (int)p2.Z);
            var z02 = Interpolate((int)p0.Y, (int)p0.Z, (int)p2.Y, (int)p2.Z);

            x01.Remove(x01.Last());
            List<int> x012 = new List<int>();
            x012.AddRange(x01);
            x012.AddRange(x12);

            z01.Remove(z01.Last());
            List<int> z012 = new List<int>();
            z012.AddRange(z01);
            z012.AddRange(z12);

            var m = x012.Count / 2;
            List<int> x_left;
            List<int> x_right;
            List<int> z_left;
            List<int> z_right;
            if (x02[m] < x012[m])
            {
                x_left = x02;
                x_right = x012;

                z_left = z02;
                z_right = z012;
            }
            else
            {
                x_left = x012;
                x_right = x02;

                z_left = z012;
                z_right = z02;
            }

            for (int y = (int)p0.Y; y < (int)p2.Y - 1; y++)
            {
                int x_l = x_left[(int)(y - p0.Y)];
                int x_r = x_right[(int)(y - p0.Y)];

                var z_segment = Interpolate(x_l, z_left[(int)(y - p0.Y)], x_r, z_right[(int)(y - p0.Y)]);
                for (int x = x_l; x < x_r; x++)
                {
                    float depth = z_segment[x - x_l];

                    ApplyZBufferAlgorithm(x, y, depth, color);
                }
            }
        }

        public void ApplyZBufferAlgorithm(int x, int y, float depth, Color color)
        {
            if (depth > zBuffer[x, y])
            {
                frameBuffer[x, y] = color;
                zBuffer[x, y] = depth;
            }
        }

        private void NonFacialButton_Click(object sender, EventArgs e)
        {
            graphics.Clear(pictureBox.BackColor);

            foreach (var polyhedron in polyhedrons)
            {
                foreach (var poly in polyhedron.polygons)
                {
                    Vector3[] vertex = new Vector3[3];
                    for (int i = 0; i < 3; i++)
                        vertex[i] = polyhedron.points[poly.indexes[i]];

                    var VecView = Normalize(new Vector3(vertex[0].X - cameraPosition.X,
                                                vertex[0].Y - cameraPosition.Y,
                                                vertex[0].Z - cameraPosition.Z));

                    float scalarProduct = poly.normal.X * VecView.X
                        + poly.normal.Y * VecView.Y + poly.normal.Z * VecView.Z;
                    if (scalarProduct > 0)
                    {
                        DrawLine(vertex[0] + polyhedron.point, vertex[1] + polyhedron.point);
                        DrawLine(vertex[1] + polyhedron.point, vertex[2] + polyhedron.point);
                        DrawLine(vertex[0] + polyhedron.point, vertex[2] + polyhedron.point);
                    }
                }
            }

            pictureBox.Image = map;
        }

        private void TextureButton_Click(object sender, EventArgs e)
        {
            texture = new Bitmap("../../../textures/box.png");

            zBuffer = new float[pictureBox.Width, pictureBox.Height];
            frameBuffer = new Color[pictureBox.Width, pictureBox.Height];

            ClearBuffers();
            graphics.Clear(pictureBox.BackColor);

            foreach (Polyhedron polyhedron in polyhedrons)
            {
                foreach (Polygon polygon in polyhedron.polygons)
                {
                    Vector3[] polygonVertex = new Vector3[3];
                    for (int i = 0; i < 3; i++)
                        polygonVertex[i] = polyhedron.points[polygon.indexes[i]];

                    var p1 = polygonVertex[0];
                    p1.X = (int)(map.Width / 2 + p1.X * 100 + polyhedron.point.X * 100);
                    p1.Y = (int)(map.Height / 2 - p1.Y * 100 - polyhedron.point.Y * 100);
                    p1.Z = (int)(p1.Z * 100);

                    var p2 = polygonVertex[1];
                    p2.X = (int)(map.Width / 2 + p2.X * 100 + polyhedron.point.X * 100);
                    p2.Y = (int)(map.Height / 2 - p2.Y * 100) - polyhedron.point.Y * 100;
                    p2.Z = (int)(p2.Z * 100);

                    var p3 = polygonVertex[2];
                    p3.X = (int)(map.Width / 2 + p3.X * 100 + polyhedron.point.X * 100);
                    p3.Y = (int)(map.Height / 2 - p3.Y * 100 - polyhedron.point.Y * 100);
                    p3.Z = (int)(p3.Z * 100);

                    ConvertToRasterWithTexture(ref p1, ref p2, ref p3,
                        polygon.TextureCoord[0], polygon.TextureCoord[1], polygon.TextureCoord[2]);
                }
            }

            for (int i = 0; i < pictureBox.Width; i++)
                for (int j = 0; j < pictureBox.Height; j++)
                    map.SetPixel(i, j, frameBuffer[i, j]);

            pictureBox.Image = map;
        }

        private List<Vector2> InterpolateTexture(int x1, Vector2 t1, int x2, Vector2 t2)
        {
            List<Vector2> res = new List<Vector2>();
            if (x1 == x2)
            {
                res.Add(t1);
            }
            Vector2 step = new Vector2((t2.X - t1.X) / (x2 - x1), (t2.Y - t1.Y) / (x2 - x1));

            Vector2 y = t1;
            for (int i = x1; i <= x2; i++)
            {
                res.Add(y);
                y.X += step.X;
                y.Y += step.Y;
            }
            return res;
        }

        public void ApplyZBufferAlgorithmWithTexture(int x, int y, float depth, Vector2 color)
        {
            if (depth > zBuffer[x, y])
            {
                int tx = (int)(color.X * (texture.Width - 1));
                int ty = (int)(color.Y * (texture.Height - 1));
                frameBuffer[x, y] = texture.GetPixel(tx, ty);
                zBuffer[x, y] = depth;
            }
        }

        private void ConvertToRasterWithTexture(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2,
            Vector2 tp0, Vector2 tp1, Vector2 tp2)
        {
            if (p1.Y < p0.Y)
            {
                var temp = p0;
                p0 = p1;
                p1 = temp;
            }

            if (p2.Y < p0.Y)
            {
                var temp = p0;
                p0 = p2;
                p2 = temp;
            }

            if (p2.Y < p1.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            var x01 = Interpolate((int)p0.Y, (int)p0.X, (int)p1.Y, (int)p1.X);
            var x12 = Interpolate((int)p1.Y, (int)p1.X, (int)p2.Y, (int)p2.X);
            var x02 = Interpolate((int)p0.Y, (int)p0.X, (int)p2.Y, (int)p2.X);

            var z01 = Interpolate((int)p0.Y, (int)p0.Z, (int)p1.Y, (int)p1.Z);
            var z12 = Interpolate((int)p1.Y, (int)p1.Z, (int)p2.Y, (int)p2.Z);
            var z02 = Interpolate((int)p0.Y, (int)p0.Z, (int)p2.Y, (int)p2.Z);

            var t01 = InterpolateTexture((int)p0.Y, tp0, (int)p1.Y, tp1);
            var t12 = InterpolateTexture((int)p1.Y, tp1, (int)p2.Y, tp2);
            var t02 = InterpolateTexture((int)p0.Y, tp0, (int)p2.Y, tp2);

            x01.Remove(x01.Last());
            List<int> x012 = new List<int>();
            x012.AddRange(x01);
            x012.AddRange(x12);

            z01.Remove(z01.Last());
            List<int> z012 = new List<int>();
            z012.AddRange(z01);
            z012.AddRange(z12);

            t01.Remove(t01.Last());
            List<Vector2> t012 = new List<Vector2>();
            t012.AddRange(t01);
            t012.AddRange(t12);

            var m = x012.Count / 2;
            List<int> x_left;
            List<int> x_right;

            List<int> z_left;
            List<int> z_right;

            List<Vector2> lefttexture;
            List<Vector2> righttexture;

            if (x02[m] < x012[m])
            {
                x_left = x02;
                x_right = x012;

                z_left = z02;
                z_right = z012;

                lefttexture = t02;
                righttexture = t012;
            }
            else
            {
                x_left = x012;
                x_right = x02;

                z_left = z012;
                z_right = z02;

                lefttexture = t012;
                righttexture = t02;
            }


            for (int y = (int)p0.Y; y < (int)p2.Y - 1; y++)
            {
                int x_l = x_left[(int)(y - p0.Y)];
                int x_r = x_right[(int)(y - p0.Y)];

                var z_segment = Interpolate(x_l, z_left[(int)(y - p0.Y)], x_r, z_right[(int)(y - p0.Y)]);
                var texture_segment = InterpolateTexture(x_l, lefttexture[(int)(y - p0.Y)], x_r, righttexture[(int)(y - p0.Y)]);
                for (int x = x_l; x < x_r; x++)
                {
                    float depth = z_segment[x - x_l];

                    ApplyZBufferAlgorithmWithTexture(x, y, depth, texture_segment[x - x_l]);
                }
            }
        }

        private Vector3 NormalVertex(List<Polygon> polygons, Polyhedron polyhedron)
        {
            Vector3 res = new Vector3(0, 0, 0);
            foreach (var poly in polygons)
            {
                res.X += poly.normal.X;
                res.Y += poly.normal.Y;
                res.Z += poly.normal.Z;
            }
            res.X = res.X / polygons.Count();
            res.Y = res.Y / polygons.Count();
            res.Z = res.Z / polygons.Count();
            return res;
        }

        public void CalculateNormalForVertex(Polyhedron polyhedron)
        {
            foreach (var polygon in polyhedron.polygons)
            {
                Vector3[] vertex = new Vector3[3];
                for (int i = 0; i < 3; i++)
                {
                    List<Polygon> polygons = polyhedron.polygons.Where(x => x.indexes.Contains(polygon.indexes[i])).ToList();
                    polygon.ListVertexNormal.Add(NormalVertex(polygons, polyhedron));
                }
            }
        }

        public double Lambert(Vector3 vertexNorm)
        {
            var v = new Vector3(vertexNorm.X - LightSourcePosition.X,
                                        vertexNorm.Y - LightSourcePosition.Y,
                                        vertexNorm.Z - LightSourcePosition.Z);
            double cos = (vertexNorm.X * v.X + vertexNorm.Y * v.Y + vertexNorm.Z * v.Z) /
                        (Math.Sqrt(vertexNorm.X * vertexNorm.X + vertexNorm.Y * vertexNorm.Y + vertexNorm.Z * vertexNorm.Z) *
                        Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z));
            var l = Math.Max(cos, 0.0);
            return (l + 1) / 2;
        }

        private List<double> InterpolateLight(int x1, double y1, int x2, double y2)
        {
            List<double> res = new List<double>();
            if (x1 == x2)
            {
                res.Add(y2);
            }
            double step = (y2 - y1) / (x2 - x1);
            double y = y1;
            for (int i = x1; i <= x2; i++)
            {
                res.Add(y);
                y += step;
            }
            return res;
        }

        private void ConvertToRasterGuro(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, Color color,
            double l0, double l1, double l2)
        {
            if (p1.Y < p0.Y)
            {
                var temp = p0;
                p0 = p1;
                p1 = temp;
            }

            if (p2.Y < p0.Y)
            {
                var temp = p0;
                p0 = p2;
                p2 = temp;
            }

            if (p2.Y < p1.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            var x01 = Interpolate((int)p0.Y, (int)p0.X, (int)p1.Y, (int)p1.X);
            var x12 = Interpolate((int)p1.Y, (int)p1.X, (int)p2.Y, (int)p2.X);
            var x02 = Interpolate((int)p0.Y, (int)p0.X, (int)p2.Y, (int)p2.X);

            var z01 = Interpolate((int)p0.Y, (int)p0.Z, (int)p1.Y, (int)p1.Z);
            var z12 = Interpolate((int)p1.Y, (int)p1.Z, (int)p2.Y, (int)p2.Z);
            var z02 = Interpolate((int)p0.Y, (int)p0.Z, (int)p2.Y, (int)p2.Z);

            var l01 = InterpolateLight((int)p0.Y, l0, (int)p1.Y, l1);
            var l12 = InterpolateLight((int)p1.Y, l1, (int)p2.Y, l2);
            var l02 = InterpolateLight((int)p0.Y, l0, (int)p2.Y, l2);

            x01.Remove(x01.Last());
            List<int> x012 = new List<int>();
            x012.AddRange(x01);
            x012.AddRange(x12);

            z01.Remove(z01.Last());
            List<int> z012 = new List<int>();
            z012.AddRange(z01);
            z012.AddRange(z12);

            l01.Remove(l01.Last());
            List<double> l012 = new List<double>();
            l012.AddRange(l01);
            l012.AddRange(l12);

            var m = x012.Count / 2;
            List<int> x_left;
            List<int> x_right;
            List<int> z_left;
            List<int> z_right;
            List<double> l_left;
            List<double> l_right;
            if (x02[m] < x012[m])
            {
                x_left = x02;
                x_right = x012;

                z_left = z02;
                z_right = z012;

                l_left = l02;
                l_right = l012;
            }
            else
            {
                x_left = x012;
                x_right = x02;

                z_left = z012;
                z_right = z02;

                l_left = l012;
                l_right = l02;
            }

            for (int y = (int)p0.Y; y < (int)p2.Y - 1; y++)
            {
                int x_l = x_left[(int)(y - p0.Y)];
                int x_r = x_right[(int)(y - p0.Y)];

                var z_segment = Interpolate(x_l, z_left[(int)(y - p0.Y)], x_r, z_right[(int)(y - p0.Y)]);
                List<double> light_segment = InterpolateLight(x_l, l_left[(int)(y - p0.Y)], x_r, l_right[(int)(y - p0.Y)]);
                for (int x = x_l; x < x_r; x++)
                {
                    float depth = z_segment[x - x_l];
                    double light = light_segment[x - x_l];

                    ApplyZBufferAlgorithm(x, y, depth, Color.FromArgb((int)(light * color.R), (int)(light * color.G), (int)(light * color.B)));
                }
            }
        }

        private void GuroButton_Click(object sender, EventArgs e)
        {
            zBuffer = new float[pictureBox.Width, pictureBox.Height];
            frameBuffer = new Color[pictureBox.Width, pictureBox.Height];

            ClearBuffers();
            graphics.Clear(pictureBox.BackColor);

            foreach (Polyhedron polyhedron in polyhedrons)
            {
                CalculateNormalForVertex(polyhedron);
                foreach (var polygon in polyhedron.polygons)
                {
                    foreach (var vert in polygon.ListVertexNormal)
                        polygon.lightness.Add(Lambert(vert));

                    Vector3[] vertex = new Vector3[3];
                    for (int i = 0; i < 3; i++)
                        vertex[i] = polyhedron.points[polygon.indexes[i]];

                    var p1 = vertex[0];
                    p1.X = (int)(map.Width / 2 + p1.X * 100 + polyhedron.point.X * 100);
                    p1.Y = (int)(map.Height / 2 - p1.Y * 100 - polyhedron.point.Y * 100);
                    p1.Z = (int)(p1.Z * 100);

                    var p2 = vertex[1];
                    p2.X = (int)(map.Width / 2 + p2.X * 100 + polyhedron.point.X * 100);
                    p2.Y = (int)(map.Height / 2 - p2.Y * 100) - polyhedron.point.Y * 100;
                    p2.Z = (int)(p2.Z * 100);

                    var p3 = vertex[2];
                    p3.X = (int)(map.Width / 2 + p3.X * 100 + polyhedron.point.X * 100);
                    p3.Y = (int)(map.Height / 2 - p3.Y * 100 - polyhedron.point.Y * 100);
                    p3.Z = (int)(p3.Z * 100);

                    ConvertToRasterGuro(ref p1, ref p2, ref p3, polygon.Color,
                        polygon.lightness[0], polygon.lightness[1], polygon.lightness[2]);
                }
            }

            for (int i = 0; i < pictureBox.Width; i++)
                for (int j = 0; j < pictureBox.Height; j++)
                    map.SetPixel(i, j, frameBuffer[i, j]);

            pictureBox.Image = map;
        }

        private int Visibility(Vector3 point, ref double[] upHorizon, ref double[] downHorizon)
        {
            if (point.X < 0 || point.X > map.Width)
                return 0;
            if (point.Y >= upHorizon[(int)point.X])              
                return 1;

            if (point.Y <= downHorizon[(int)point.X])
                return -1;

            return 0;
        }

        void updateHorizons(Vector3 last, Vector3 curr, ref double[] upHorizon, ref double[] downHorizon)
        {
            if (last.X < 0 || curr.X >= map.Width)
            {
                return;
            }
            if (curr.X - last.X == 0)
            {
                upHorizon[(int)curr.X] = Math.Max(upHorizon[(int)curr.X], curr.Y);
                downHorizon[(int)curr.X] = Math.Min(downHorizon[(int)curr.X], curr.Y);
            }
            else
            {
                var tg = (curr.Y - last.Y) / (curr.X - last.X);
                for (int x = (int)last.X; x <= curr.X; x++)
                {
                    var y = tg * (x - last.X) + last.Y;
                    upHorizon[x] = Math.Max(upHorizon[x], y);
                    downHorizon[x] = Math.Min(downHorizon[x], y);
                }
            }
        }

        Vector3 intersect(Vector3 previous, Vector3 curr, ref double[] upHorizon, ref double[] downHorizon)
        {
            double xStep = (curr.X - previous.X * 1.0) / 20;
            double yStep = (curr.Y - previous.Y * 1.0) / 20;
            for (int i = 1; i <= 20; i++)
            {
                var point = new Vector3((float)Math.Clamp(previous.X + i * xStep, 0, map.Width - 1), (float)(previous.Y + i * yStep), curr.Z);
                if (Visibility(point, ref upHorizon, ref downHorizon) == 0)
                {
                    return point;
                }
            }

            return curr;
        }

        private void DrawLineV(Vector3 p1, Vector3 p2)
        {
            graphics.DrawLine(pen, new PointF(p1.X, p1.Y + 300),
                new PointF(p2.X, p2.Y + 300));
        }

        private double function(double x, double y)
        {
            return Math.Sqrt(x*x + y*y + 1);
        }

        private void GraphRasterbutton_Click(object sender, EventArgs e)
        {
            graphics.Clear(pictureBox.BackColor);

            double threshold = 5;
            double splitting = 50;

            double step = threshold * 2.0 / splitting;

            var upHorizon = new double[map.Width];
            var downHorizon = new double[map.Width];

            for (int i = 0; i < map.Width; i++)
            {
                upHorizon[i] = double.MinValue;
                downHorizon[i] = double.MaxValue;
            }

            for (double z = threshold; z >= -threshold; z -= step)
            {
                Vector3 previous = new Vector3();
                previous.X = (int)(map.Width / 2 + (-threshold) * 100);
                previous.Y = (int)(map.Height / 2 - function(-threshold, z) * 100);
                previous.Z = (int)(z * 100);
                for (double x = -threshold; x <= threshold; x += step)
                {
                    Vector3 current = new Vector3();
                    current.X = (int)(map.Width / 2 + x * 100);
                    current.Y = (int)(map.Height / 2 - function(x, z) * 100);
                    current.Z = (int)(z * 100);

                    if (Visibility(current, ref upHorizon, ref downHorizon) == 1)
                    {
                        if (Visibility(previous, ref upHorizon, ref downHorizon) != 0)
                        {
                            DrawLineV(previous, current);
                            updateHorizons(previous, current, ref upHorizon, ref downHorizon);
                        }
                        else
                        {
                            var mid = intersect(current, previous, ref upHorizon, ref downHorizon);
                            DrawLine(current, mid);
                            updateHorizons(current, mid, ref upHorizon, ref downHorizon);
                        }

                    }
                    else if (Visibility(current, ref upHorizon, ref downHorizon) == -1)
                    {
                        if (Visibility(previous, ref upHorizon, ref downHorizon) == 0)
                        {
                            DrawLine(previous, current);
                            updateHorizons(previous, current, ref upHorizon, ref downHorizon);
                        }
                        else
                        {
                            var mid = intersect(current, previous, ref upHorizon, ref downHorizon);
                            DrawLine(current, mid);
                            updateHorizons(current, mid, ref upHorizon, ref downHorizon);
                        }

                    }
                    else
                    {
                        if (Visibility(previous, ref upHorizon, ref downHorizon) == 1)
                        {
                            var mid = intersect(current, previous, ref upHorizon, ref downHorizon);
                            DrawLine(previous, mid);
                            updateHorizons(previous, mid, ref upHorizon, ref downHorizon);
                        }
                        else if (Visibility(previous, ref upHorizon, ref downHorizon) == -1)
                        {
                            var mid = intersect(current, previous, ref upHorizon, ref downHorizon);
                            DrawLine(previous, mid);
                            updateHorizons(previous, mid, ref upHorizon, ref downHorizon);
                        }
                    }

                    previous = current;
                }
            }

            pictureBox.Image = map;
        }
    }
}

public class Polygon
{
    public int[] indexes;
    public Vector3 normal;
    public Color Color;
    public List<Vector2> TextureCoord;
    public List<Vector3> ListVertexNormal;
    public List<double> lightness;

    public Polygon(int[] indexes)
    {
        this.indexes = indexes;
        TextureCoord = new List<Vector2>();
        ListVertexNormal = new List<Vector3>();
        lightness = new List<double>();
    }

    public Polygon(int[] indexes, Vector3 normal)
    {
        this.indexes = indexes;
        this.normal = normal;
        TextureCoord = new List<Vector2>();
        ListVertexNormal = new List<Vector3>();
        lightness = new List<double>();
    }
}

public class Polyhedron
{
    public Vector3 point;
    public Vector3[] points;
    public Polygon[] polygons;

    public Polyhedron(Vector3 point, Vector3[] points, Polygon[] polygons)
    {
        this.point = point;
        this.points = points;
        this.polygons = polygons;
    }
}
