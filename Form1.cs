using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms.VisualStyles;
using System;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;
using static System.Windows.Forms.AxHost;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Diagnostics.Metrics;

namespace Rogalik_s_3D
{
    public partial class Form : System.Windows.Forms.Form
    {
        Bitmap map;
        Graphics graphics;
        Pen pen = new Pen(Color.White, 3);
        //Pen axes = new Pen(Color.Aqua, 3);

        enum State { Position, Rotation, Scale, Draw }
        enum Plane { X, Y, Z, XYZ }
        State state = State.Position;
        Plane plane = Plane.XYZ;
        PointF mousePosition;
        List<PointF> pointsRotation = new();
        PointXYZ cameraPosition = new(0, 0, -5);
        List<Polyhedron> polyhedrons = new List<Polyhedron>();
        bool isMouseDown = false;
        int indexPolyhedron = 0;
        bool isPerspective = false;

        public Form()
        {
            InitializeComponent();
            map = new Bitmap(pictureBox.Width, pictureBox.Height);
            graphics = Graphics.FromImage(map);
            comboBox.SelectedItem = "XYZ";
            ShowPolyhedrons();
        }

        private Polyhedron Tetrahedron(float side, PointXYZ point)
        {
            float halfSide = side / 2;
            float upY = (float)Math.Sqrt(6) / 4 * side;
            float downY = (float)(1 / (2 * Math.Sqrt(6))) * side;
            side = (float)(1 / (2 * Math.Sqrt(3))) * side;

            PointXYZ[] points = new PointXYZ[4];
            points[0] = new PointXYZ(-halfSide, -downY, -side);
            points[1] = new PointXYZ(halfSide, -downY, -side);
            points[2] = new PointXYZ(0, -downY, 2 * side);
            points[3] = new PointXYZ(0, upY, 0);

            Polygon[] polygons = new Polygon[4];
            polygons[0] = new(new int[] { 0, 1, 2 });
            polygons[1] = new(new int[] { 0, 1, 3 });
            polygons[2] = new(new int[] { 1, 2, 3 });
            polygons[3] = new(new int[] { 0, 2, 3 });

            Polyhedron polyhedron = new(point, points, polygons);
            return polyhedron;
        }

        private Polyhedron Hexahedron(float side, PointXYZ point)
        {
            side = side / 2;

            PointXYZ[] points = new PointXYZ[8];
            points[0] = new PointXYZ(-side, -side, -side);
            points[1] = new PointXYZ(side, -side, -side);
            points[2] = new PointXYZ(side, -side, side);
            points[3] = new PointXYZ(-side, -side, side);
            points[4] = new PointXYZ(-side, side, -side);
            points[5] = new PointXYZ(side, side, -side);
            points[6] = new PointXYZ(side, side, side);
            points[7] = new PointXYZ(-side, side, side);

            Polygon[] polygons = new Polygon[6];
            polygons[0] = new(new int[] { 0, 1, 2, 3 });
            polygons[1] = new(new int[] { 0, 1, 5, 4 });
            polygons[2] = new(new int[] { 0, 3, 7, 4 });
            polygons[3] = new(new int[] { 1, 2, 6, 5 });
            polygons[4] = new(new int[] { 2, 3, 7, 6 });
            polygons[5] = new(new int[] { 4, 5, 6, 7 });

            Polyhedron polyhedron = new(point, points, polygons);
            return polyhedron;
        }

        private Polyhedron Octahedron(float side, PointXYZ point)
        {
            side = (float)Math.Sqrt(side * side / 2);

            PointXYZ[] points = new PointXYZ[6];
            points[0] = new PointXYZ(0, 0, -side);
            points[1] = new PointXYZ(side, 0, 0);
            points[2] = new PointXYZ(0, 0, side);
            points[3] = new PointXYZ(-side, 0, 0);
            points[4] = new PointXYZ(0, side, 0);
            points[5] = new PointXYZ(0, -side, 0);

            Polygon[] polygons = new Polygon[8];
            polygons[0] = new(new int[] { 0, 1, 4 });
            polygons[1] = new(new int[] { 1, 2, 4 });
            polygons[2] = new(new int[] { 2, 3, 4 });
            polygons[3] = new(new int[] { 3, 0, 4 });
            polygons[4] = new(new int[] { 0, 1, 5 });
            polygons[5] = new(new int[] { 1, 2, 5 });
            polygons[6] = new(new int[] { 2, 3, 5 });
            polygons[7] = new(new int[] { 3, 0, 5 });

            Polyhedron polyhedron = new(point, points, polygons);
            return polyhedron;
        }

        private PointXYZ Equation(float x, float y)
        {
            double r = x * x + y * y;
            return new(x, y, (float)(Math.Cos(r) / (r + 1)));
        }

        private Polyhedron Graph(PointXYZ point, float x1, float x2, float y1, float y2, int split)
        {
            List<Polygon> polygons = new();
            List<PointXYZ> points = new();

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

        private void DrawLine(PointXYZ point1, PointXYZ point2)
        {
            PointF p1 = new(map.Width / 2 + point1.x * 100, map.Height / 2 - point1.y * 100);
            PointF p2 = new(map.Width / 2 + point2.x * 100, map.Height / 2 - point2.y * 100);

            graphics.DrawLine(pen, p1, p2);
        }

        PointXYZ Multiplication(PointXYZ point, float[,] matrix)
        {
            float[] newPoint = new float[4];
            float[] pointArray = new float[]
                { point.x, point.y, point.z, 1 };
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    newPoint[i] += pointArray[j] * matrix[j, i];
            return new PointXYZ(newPoint[0], newPoint[1], newPoint[2]);
        }

        void Shift(ref Polyhedron polyhedron, float dx, float dy)
        {
            float[,] matrix = new float[4, 4];
            matrix[0, 0] = 1;
            matrix[1, 1] = 1;
            matrix[2, 2] = 1;
            matrix[3, 3] = 1;

            matrix[3, 0] = dx;
            matrix[3, 1] = dy;
            matrix[3, 2] = 0;

            polyhedron.point = Multiplication(polyhedron.point, matrix);
        }

        void Rotate(ref Polyhedron polyhedron, float rotate, Plane xyz)
        {
            float[,] matrix = new float[4, 4];
            int xyzI = xyz == Plane.X ? 0 : xyz == Plane.Y ? 1 : 2;
            matrix[xyzI, xyzI] = 1;
            matrix[3, 3] = 1;
            double angle = rotate * 180 / Math.PI;
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

            for (int i = 0; i < polyhedron.points.Length; i++)
                polyhedron.points[i] = Multiplication(polyhedron.points[i], matrix);
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
                polyhedron.points[i] = Multiplication(polyhedron.points[i], matrix);
        }

        void AxonometricProjection()
        {

            foreach (var polyhedron in polyhedrons)
                for (int i = 0; i < polyhedron.polygons.Length; i++)
                {
                    for (int j = 0; j < polyhedron.polygons[i].indexes.Length - 1; j++)
                        DrawLine(polyhedron.points[polyhedron.polygons[i].indexes[j]] + polyhedron.point,
                            polyhedron.points[polyhedron.polygons[i].indexes[j + 1]] + polyhedron.point);
                    DrawLine(polyhedron.points[polyhedron.polygons[i].indexes[polyhedron.polygons[i].indexes.Length - 1]] + polyhedron.point,
                            polyhedron.points[polyhedron.polygons[i].indexes[0]] + polyhedron.point);
                }

            pictureBox.Image = map;
        }

        PointXYZ MultiplicationP(PointXYZ point, float[,] matrix)
        {
            float[] newPoint = new float[4];
            float[] pointArray = new float[]
                { point.x, point.y, point.z, 1 };
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    newPoint[i] += pointArray[j] * matrix[j, i];
            return new PointXYZ(newPoint[0] / newPoint[3], newPoint[1] / newPoint[3], newPoint[2]);
        }

        void PerspectiveProjection()
        {
            float[,] matrix = new float[4, 4];
            matrix[0, 0] = 1;
            matrix[1, 1] = 1;
            matrix[3, 3] = 1;

            PointXYZ[][] points = new PointXYZ[polyhedrons.Count][];
            for (int i = 0; i < polyhedrons.Count; i++)
            {
                points[i] = new PointXYZ[polyhedrons[i].points.Length];
                for (int j = 0; j < points[i].Length; j++)
                {
                    matrix[2, 3] = -1 / (polyhedrons[i].point.z + polyhedrons[i].points[j].z - cameraPosition.z);
                    points[i][j] = MultiplicationP(polyhedrons[i].points[j], matrix) + polyhedrons[i].point - cameraPosition;
                }
            }

            /*Point xy = new(map.Width / 2, map.Height / 2);
            Point x = new(map.Width, map.Height / 2);
            Point y = new(map.Width / 2, 0);
            graphics.DrawLine(axes, xy, x);
            graphics.DrawLine(axes, xy, y);*/

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

        private Polyhedron RotatePoints(PointXYZ point, int parts)
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

            PointXYZ[] points = new PointXYZ[parts * pointsRotation.Count];

            for (int i = 0; i < pointsRotation.Count; i++)
                points[i] = new((pointsRotation[i].X - map.Width / 2) / 100,
                    -(pointsRotation[i].Y - map.Height / 2) / 100, 0);

            for (int i = 1; i < parts; i++)
                for (int j = 0; j < pointsRotation.Count; j++)
                    points[i * pointsRotation.Count + j] =
                        Multiplication(points[(i - 1) * pointsRotation.Count + j], matrix);

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
            if (!isMouseDown || polyhedrons.Count() < 1 || state == State.Draw)
                return;

            float dx;
            float dy;
            PointsDistance(mousePosition, e.Location, out dx, out dy);
            Polyhedron polyhedron = polyhedrons[indexPolyhedron];

            if (state == State.Position)
                Shift(ref polyhedron, dx / 100, -dy / 100);
            else if (state == State.Rotation)
            {
                if (plane == Plane.XYZ)
                {
                    Rotate(ref polyhedron, -dy / 3000, Plane.X);
                    Rotate(ref polyhedron, dx / 3000, Plane.Y);
                }
                else
                    Rotate(ref polyhedron, (dx + dy) / 3000, plane);
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
            {
                pointsRotation.Clear();
            }

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
                    List<PointXYZ> points = new();
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
                    foreach (PointXYZ point in polyhedrons[indexPolyhedron].points)
                    {
                        writer.WriteLine("v " + string.Format(CultureInfo.InvariantCulture, "{0:f}", point.x) + " "
                            + string.Format(CultureInfo.InvariantCulture, "{0:f}", point.y) + " "
                            + string.Format(CultureInfo.InvariantCulture, "{0:f}", point.z));
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
            if (int.TryParse(pX.Text, out int x) &&
                int.TryParse(pY.Text, out int y) &&
                int.TryParse(pZ.Text, out int z))
                cameraPosition = new(x, y, z);
            ShowPolyhedrons();
        }
    }

    public class PointXYZ
    {
        public float x;
        public float y;
        public float z;
        public PointXYZ(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public PointXYZ(PointXYZ point)
        {
            x = point.x;
            y = point.y;
            z = point.z;
        }
        public PointXYZ()
        {
            x = 0;
            y = 0;
            z = 0;
        }
        public static PointXYZ operator +(PointXYZ p1, PointXYZ p2)
        {
            return new(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
        }
        public static PointXYZ operator -(PointXYZ p1, PointXYZ p2)
        {
            return new(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
        }
    }

    public class Polygon
    {
        public int[] indexes;

        public Polygon(int[] indexes)
        {
            this.indexes = indexes;
        }
    }

    public class Polyhedron
    {
        public PointXYZ point;
        public PointXYZ[] points;
        public Polygon[] polygons;

        public Polyhedron(PointXYZ point, PointXYZ[] points, Polygon[] polygons)
        {
            this.point = point;
            this.points = points;
            this.polygons = polygons;
        }
    }
}