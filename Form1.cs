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

namespace Rogalik_s_3D
{
    public partial class Form : System.Windows.Forms.Form
    {
        Bitmap map;
        Graphics graphics;
        Pen pen = new Pen(Color.White, 3);
        Pen axes = new Pen(Color.Aqua, 3);

        enum State { Position, Rotation, Scale, Draw }
        enum Plane { X, Y, Z, XYZ }
        State state = State.Position;
        Plane plane = Plane.XYZ;
        PointF mousePosition;
        List<PointF> points = new();
        PointXYZ camera = new();
        List<Polyhedron> polyhedrons = new List<Polyhedron>();
        bool isMouseDown = false;
        int index = 0;
        bool isPerspective = true;

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
            polygons[0] = new(new PointXYZ[3]
                { points[0], points[1], points[2] });
            polygons[1] = new(new PointXYZ[3]
                { points[0], points[1], points[3] });
            polygons[2] = new(new PointXYZ[3]
                { points[1], points[2], points[3] });
            polygons[3] = new(new PointXYZ[3]
                { points[2], points[0], points[2] });

            Polyhedron polyhedron = new(polygons, point);
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
            polygons[0] = new(new PointXYZ[4]
                { points[0], points[1], points[2], points[3] });
            polygons[1] = new(new PointXYZ[4]
                { points[0], points[1], points[5], points[4] });
            polygons[2] = new(new PointXYZ[4]
                { points[0], points[3], points[7], points[4] });
            polygons[3] = new(new PointXYZ[4]
                { points[1], points[2], points[6], points[5] });
            polygons[4] = new(new PointXYZ[4]
                { points[2], points[3], points[7], points[6] });
            polygons[5] = new(new PointXYZ[4]
                { points[4], points[5], points[6], points[7] });

            Polyhedron polyhedron = new(polygons, point);
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
            polygons[0] = new(new PointXYZ[3]
                { points[0], points[1], points[4] });
            polygons[1] = new(new PointXYZ[3]
                { points[1], points[2], points[4] });
            polygons[2] = new(new PointXYZ[3]
                { points[2], points[3], points[4] });
            polygons[3] = new(new PointXYZ[3]
                { points[3], points[0], points[4] });
            polygons[4] = new(new PointXYZ[3]
                { points[0], points[1], points[5] });
            polygons[5] = new(new PointXYZ[3]
                { points[1], points[2], points[5] });
            polygons[6] = new(new PointXYZ[3]
                { points[2], points[3], points[5] });
            polygons[7] = new(new PointXYZ[3]
                { points[3], points[0], points[5] });

            Polyhedron polyhedron = new(polygons, point);
            return polyhedron;
        }

        private PointXYZ Equation(float x, float y)
        {
            double r = x * x + y * y;
            return new(x, y, (float)(Math.Cos(r) / (r + 1)));
        }

        private Polyhedron Graph(PointXYZ point, float x1, float x2, float y1, float y2, float t = 20)
        {
            List<Polygon> polygons = new();

            float distX = Math.Abs(x2 - x1);
            float distY = Math.Abs(y2 - y1);
            for (float x = x1; x < x2; x += distX / t)
            {
                for (float y = y1; y < y2; y += distY / t)
                    polygons.Add(new(new PointXYZ[4] { Equation(x, y), Equation(x + 1, y),
                        Equation(x + 1, y + 1), Equation(x, y + 1) }));
            }
            return new(polygons.ToArray(), point);
        }

        private void DrawLine(Line line, PointXYZ point)
        {
            PointF point1 = new(map.Width / 2 + point.x + line.point1.x,
                map.Height / 2 - point.y - line.point1.y);
            PointF point2 = new(map.Width / 2 + point.x + line.point2.x,
                map.Height / 2 - point.y - line.point2.y);

            graphics.DrawLine(pen, point1, point2);
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

            foreach (var polygon in polyhedron.polygons)
                for (int i = 0; i < polygon.lines.Length; i++)
                    polygon.ChangePoint(i, Multiplication(
                        polygon.lines[i].point1, matrix));
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

            foreach (var polygon in polyhedron.polygons)
                for (int i = 0; i < polygon.lines.Length; i++)
                    polygon.ChangePoint(i, Multiplication(
                        polygon.lines[i].point1, matrix));
        }

        void AxonometricProjection()
        {
            List<Polyhedron> newPolyhedrons = new List<Polyhedron>();
            for (int i = 0; i < polyhedrons.Count; i++)
            {
                var point = new PointXYZ(polyhedrons[i].point);
                var newPolygons = new Polygon[polyhedrons[i].polygons.Length];
                for (int j = 0; j < newPolygons.Length; j++)
                {
                    var newLines = new Line[polyhedrons[i].polygons[j].lines.Length];
                    for (int l = 0; l < newLines.Length; l++)
                        newLines[l] = new Line(
                            new(polyhedrons[i].polygons[j].lines[l].point1),
                            new(polyhedrons[i].polygons[j].lines[l].point2));
                    newPolygons[j] = new Polygon(newLines);
                }
                newPolyhedrons.Add(new Polyhedron(newPolygons, point));
            }

            var sf = (float)Math.Sqrt(1.0 / 3.0);
            var cf = (float)Math.Sqrt(2.0 / 3.0);
            var sp = (float)Math.Sqrt(1.0 / 2.0);
            var cp = (float)Math.Sqrt(1.0 / 2.0);

            float[,] matrix = new float[4, 4];
            matrix[0, 0] = cp;
            matrix[0, 1] = sf * sp;
            matrix[1, 1] = cf;
            matrix[2, 0] = sp;
            matrix[2, 1] = -sf * cp;
            matrix[3, 3] = 1;

            foreach (var polyhedron in newPolyhedrons)
                foreach (var polygon in polyhedron.polygons)
                {
                    for (int i = 0; i < polygon.lines.Length; i++)
                        polygon.ChangePoint(i, Multiplication(
                            polygon.lines[i].point1, matrix));
                    foreach (var line in polygon.lines)
                        DrawLine(line, polyhedron.point);
                }

            int cathet = (int)Math.Sqrt(Math.Pow(map.Width / 2, 2) / 3);
            Point xyz = new(map.Width / 2, map.Height / 2);
            Point x = new(map.Width, map.Height / 2 + cathet);
            Point y = new(map.Width / 2, 0);
            Point z = new(0, map.Height / 2 + cathet);

            graphics.DrawLine(axes, xyz, x);
            graphics.DrawLine(axes, xyz, y);
            graphics.DrawLine(axes, xyz, z);

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
            List<Polyhedron> newPolyhedrons = new List<Polyhedron>();
            for (int i = 0; i < polyhedrons.Count; i++)
            {
                var point = new PointXYZ(polyhedrons[i].point);
                var newPolygons = new Polygon[polyhedrons[i].polygons.Length];
                for (int j = 0; j < newPolygons.Length; j++)
                {
                    var newLines = new Line[polyhedrons[i].polygons[j].lines.Length];
                    for (int l = 0; l < newLines.Length; l++)
                        newLines[l] = new Line(
                            new(polyhedrons[i].polygons[j].lines[l].point1),
                            new(polyhedrons[i].polygons[j].lines[l].point2));
                    newPolygons[j] = new Polygon(newLines);
                }
                newPolyhedrons.Add(new Polyhedron(newPolygons, point));
            }

            float[,] matrix = new float[4, 4];
            matrix[0, 0] = 1;
            matrix[1, 1] = 1;
            matrix[3, 2] = -1 / 2000;
            matrix[3, 3] = 1;

            foreach (var polyhedron in newPolyhedrons)
                foreach (var polygon in polyhedron.polygons)
                {
                    for (int i = 0; i < polygon.lines.Length; i++)
                    {
                        matrix[2, 3] = -1 / (polyhedron.point.z +
                            polygon.lines[i].point1.z);
                        polygon.ChangePoint(i, MultiplicationP(
                            polygon.lines[i].point1, matrix));
                    }

                    foreach (var line in polygon.lines)
                        DrawLine(line, polyhedron.point);
                }

            Point xy = new(map.Width / 2, map.Height / 2);
            Point x = new(map.Width, map.Height / 2);
            Point y = new(map.Width / 2, 0);
            graphics.DrawLine(axes, xy, x);
            graphics.DrawLine(axes, xy, y);
            pictureBox.Image = map;
        }

        private Polyhedron RotatePoints(int parts)
        {
            float[,] matrix = new float[4, 4];
            matrix[1, 1] = 1;
            matrix[3, 3] = 1;
            double angle = 360.0 / parts * Math.PI / 180.0;
            matrix[0, 0] = (float)Math.Cos(-angle);
            matrix[2, 2] = (float)Math.Cos(-angle);
            matrix[2, 0] = (float)Math.Sin(-angle);
            matrix[0, 2] = -(float)Math.Sin(-angle);

            PointXYZ[,] newPoints = new PointXYZ[parts + 1, points.Count];

            for (int i = 0; i < points.Count; i++)
                newPoints[0, i] = new(points[i].X - map.Width / 2,
                    points[i].Y - map.Height / 2, 0);

            Polygon[] polygons = new Polygon[parts];
            for (int i = 1; i <= parts; i++)
            {
                PointXYZ[] tempPoints = new PointXYZ[points.Count * 2];
                for (int j = 0; j < points.Count; j++)
                    tempPoints[j] = newPoints[i - 1, points.Count - j - 1];
                for (int j = 0; j < points.Count; j++)
                {
                    newPoints[i, j] = Multiplication(newPoints[i - 1, j], matrix);
                    tempPoints[points.Count + j] = newPoints[i, j];
                }
                polygons[i - 1] = new(tempPoints);
            }
            return new(polygons, new());
        }

        private void ShowPolyhedrons()
        {
            graphics.Clear(pictureBox.BackColor);
            if (isPerspective)
                PerspectiveProjection();
            else
                AxonometricProjection();
            for (int i = 0; i < points.Count; i++)
                graphics.DrawEllipse(pen, points[i].X, points[i].Y, 3, 3);
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
                points.Add(mousePosition);
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
            Polyhedron polyhedron = polyhedrons[index];

            if (state == State.Position)
                Shift(ref polyhedron, dx, -dy);
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
            polyhedrons[index] = polyhedron;
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
                polyhedrons.Add(Tetrahedron(200, new PointXYZ(0, 0, 200)));
            else if (sender.Equals(polyhedron2))
                polyhedrons.Add(Hexahedron(200, new PointXYZ(0, 0, 200)));
            else if (sender.Equals(polyhedron3))
                polyhedrons.Add(Octahedron(200, new PointXYZ(0, 0, 200)));

            index = polyhedrons.Count - 1;

            ShowPolyhedrons();
        }

        private void StateClick(object sender, EventArgs e)
        {
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
            if (sender.Equals(perspective))
                isPerspective = true;
            else
                isPerspective = false;
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
                    List<PointXYZ> vertices = new();
                    List<Polygon> polygons = new();
                    // Чтение файла построчно
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] split = line.Split(' ');
                        if (split[0] == "v")
                            vertices.Add(new(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])));
                        else if (split[0] == "f")
                        {
                            List<PointXYZ> polygonPoints = new();
                            for (int i = 1; i < split.Length; i++)
                                polygonPoints.Add(vertices[int.Parse(split[i]) - 1]);
                            polygons.Add(new Polygon(polygonPoints.ToArray()));
                        }
                    }
                    polyhedrons.Add(new Polyhedron(polygons.ToArray(), new PointXYZ(0, 0, 200)));
                }
                index = polyhedrons.Count - 1;
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
                    List<PointXYZ> vertices = new();
                    List<List<int>> savePolygons = new();
                    // Запись информации о каждой грани многогранника в файл
                    foreach (Polygon polygon in polyhedrons[index].polygons)
                    {
                        List<int> savePolygon = new();
                        foreach (Line line in polygon.lines)
                        {
                            if (!vertices.Contains(line.point1))
                            {
                                vertices.Add(line.point1);
                                writer.WriteLine("v " + line.point1.x + " " + line.point1.y + " " + line.point1.z);
                            }
                            savePolygon.Add(vertices.IndexOf(line.point1) + 1);
                        }
                        savePolygons.Add(savePolygon);
                    }
                    foreach (var polygon in savePolygons)
                    {
                        writer.Write("f");
                        foreach (var point in polygon)
                            writer.Write(" " + point);
                        writer.Write("\n");
                    }
                }
            }
        }

        private void GraphClick(object sender, EventArgs e)
        {
            if (int.TryParse(graphX1.Text, out int x1) &&
                int.TryParse(graphX2.Text, out int x2) &&
                int.TryParse(graphY1.Text, out int y1) &&
                int.TryParse(graphY2.Text, out int y2))
            {
                polyhedrons.Add(Graph(new PointXYZ(0, 0, 200),
                x1, x2, y1, y2));

                index = polyhedrons.Count - 1;
                ShowPolyhedrons();
            }
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            if (polyhedrons.Count < 1)
                return;

            polyhedrons.RemoveAt(index);
            if (index >= polyhedrons.Count)
                index = polyhedrons.Count - 1;
            ShowPolyhedrons();
        }

        private void LeftRightClick(object sender, EventArgs e)
        {
            if (sender.Equals(left))
            {
                index--;
                if (index < 0)
                    index = polyhedrons.Count - 1;
            }
            else
            {
                index++;
                if (index >= polyhedrons.Count)
                    index = 0;
            }
        }

        private void Create3DClick(object sender, EventArgs e)
        {
            if (points.Count == 0)
                return;
            polyhedrons.Add(RotatePoints(10));
            points.Clear();
            index = polyhedrons.Count - 1;
            ShowPolyhedrons();
        }
    }

    public class PointXYZ
    {
        public float x;
        public float y;
        public float z;
        public PointXYZ()
        {
            x = 0;
            y = 0;
            z = 0;
        }
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
    }

    public class Line
    {
        public PointXYZ point1;
        public PointXYZ point2;
        public Line(PointXYZ point1, PointXYZ point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }
    }

    public class Polygon
    {
        public Line[] lines;
        public Polygon(Line[] lines)
        {
            this.lines = lines;
        }
        public Polygon(PointXYZ[] points)
        {
            lines = new Line[points.Length];
            Array.Resize(ref points, points.Length + 1);
            points[points.Length - 1] = points[0];
            for (int i = 0; i < points.Length - 1; i++)
                lines[i] = new Line(points[i], points[i + 1]);
        }

        public void ChangePoint(int index, PointXYZ point)
        {
            lines[index].point1 = point;
            if (index == 0)
                lines[lines.Length - 1].point2 = point;
            else
                lines[index - 1].point2 = point;
        }
    }

    public class Polyhedron
    {
        public Polygon[] polygons;
        public PointXYZ point;
        public Polyhedron(Polygon[] polygons, PointXYZ point)
        {
            this.polygons = polygons;
            this.point = point;
        }
    }
}