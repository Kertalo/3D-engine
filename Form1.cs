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

namespace Rogalik_s_3D
{
    public partial class Form : System.Windows.Forms.Form
    {
        Bitmap map;
        Graphics graphics;
        Pen pen = new Pen(Color.Black, 3);
        Pen axes = new Pen(Color.Black, 1);

        enum State { Position, Rotation, Scale, Reflection, RotationAroundLine, ToAxon }
        enum Plane { XY, YZ, XZ }
        State state = State.Position;
        Plane plane = Plane.XY;
        PointF mousePosition;
        List<Polyhedron> polyhedrons = new List<Polyhedron>();
        bool isMouseDown = false;
        int index = 0;
        bool isPerspective = true;

        public Form()
        {
            InitializeComponent();
            map = new Bitmap(pictureBox.Width, pictureBox.Height);
            graphics = Graphics.FromImage(map);
            polyhedrons.Add(Tetrahedron(200, new PointXYZ(0, 0, 100)));
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

        private Polyhedron Icosahedron(float side, PointXYZ point)
        {
            return Octahedron(side, point);
        }

        private void DrawLine(Line line, PointXYZ point)
        {
            PointF point1 = new(map.Width / 2 + point.x + line.point1.x,
                map.Height / 2 - point.y - line.point1.y);
            PointF point2 = new(map.Width / 2 + point.x + line.point2.x,
                map.Height / 2 - point.y - line.point2.y);

            graphics.DrawLine(pen, point1, point2);
        }

        private void DrawLine2(Line line, PointXYZ point)
        {
            PointF point1 = new(map.Width / 2 + (point.x + line.point1.x) /
                (point.z + line.point1.z), map.Height / 2 -
                (point.y + line.point1.y) / (point.z + line.point1.z));
            PointF point2 = new(map.Width / 2 + (point.x + line.point2.x) /
                (point.z + line.point2.z), map.Height / 2 -
                (point.y + line.point2.y) / (point.z + line.point2.z));

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

        void Rotate(ref Polyhedron polyhedron, float rotate, int xyz)
        {
            float[,] matrix = new float[4, 4];
            matrix[xyz, xyz] = 1;
            matrix[3, 3] = 1;
            double angle = rotate * 180 / Math.PI;
            if (xyz == 0)
            {
                matrix[1, 1] = (float)Math.Cos(angle);
                matrix[2, 2] = (float)Math.Cos(angle);
                matrix[2, 1] = (float)Math.Sin(angle);
                matrix[1, 2] = -(float)Math.Sin(angle);
            }
            if (xyz == 1)
            {
                matrix[0, 0] = (float)Math.Cos(-angle);
                matrix[2, 2] = (float)Math.Cos(-angle);
                matrix[2, 0] = (float)Math.Sin(-angle);
                matrix[0, 2] = -(float)Math.Sin(-angle);
            }
            if (xyz == 2)
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

        void Scale(ref Polyhedron polyhedron, float scale)
        {
            float[,] matrix = new float[4, 4];
            matrix[0, 0] = scale;
            matrix[1, 1] = scale;
            matrix[2, 2] = scale;
            matrix[3, 3] = 1;

            foreach (var polygon in polyhedron.polygons)
                for (int i = 0; i < polygon.lines.Length; i++)
                    polygon.ChangePoint(i, Multiplication(
                        polygon.lines[i].point1, matrix));
        }

        void Reflection(ref Polyhedron polyhedron)
        {
            float[,] matrix = new float[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (i == j)
                        matrix[i, j] = 1;
                    else
                        matrix[i, j] = 0;

            if (plane == Plane.XY)
                matrix[2, 2] *= -1;
            else if (plane == Plane.YZ)
                matrix[0, 0] *= -1;
            else if (plane == Plane.XZ)
                matrix[1, 1] *= -1;

            foreach (var polygon in polyhedron.polygons)
                for (int i = 0; i < polygon.lines.Length; i++)
                    polygon.ChangePoint(i, Multiplication(
                        polygon.lines[i].point1, matrix));
        }

        void RotationAroundLine(ref Polyhedron polyhedron, PointXYZ point1, PointXYZ point2, int angle)
        {
            PointXYZ vector = new PointXYZ(point2.x - point1.x,
                point2.y - point1.y,
                point2.z - point1.z);

            double length = Math.Sqrt((point2.x - point1.x) * (point2.x - point1.x)
                + (point2.y - point1.y) * (point2.y - point1.y)
                + (point2.z - point1.z) * (point2.z - point1.z));

            PointXYZ normal_vector = new PointXYZ((float)(vector.x / length),
                (float)(vector.y / length), (float)(vector.z / length));

            double angleSin = Math.Sin(angle * Math.PI / 180);
            double angleCos = Math.Cos(angle * Math.PI / 180);

            float[,] matrix = new float[4, 4];
            float l = normal_vector.x;
            float m = normal_vector.y;
            float n = normal_vector.z;
            matrix[0, 0] = (float)(l * l + angleCos * (1 - l * l));
            matrix[0, 1] = (float)(l * (1 - angleCos) * m + n * angleSin);
            matrix[0, 2] = (float)(l * (1 - angleCos) * n - m * angleSin);
            matrix[0, 3] = 0;
            matrix[1, 0] = (float)(l * (1 - angleCos) * m - n * angleSin);
            matrix[1, 1] = (float)(m * m + angleCos * (1 - m * m));
            matrix[1, 2] = (float)(m * (1 - angleCos) * n + l * angleSin);
            matrix[1, 3] = 0;
            matrix[2, 0] = (float)(l * (1 - angleCos) * n + m * angleSin);
            matrix[2, 1] = (float)(m * (1 - angleCos) * n - l * angleSin);
            matrix[2, 2] = (float)(n * n + angleCos * (1 - n * n));
            matrix[2, 3] = 0;
            matrix[3, 0] = 0;
            matrix[3, 1] = 0;
            matrix[3, 2] = 0;
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
            matrix[0, 2] = 0;
            matrix[0, 3] = 0;
            matrix[1, 0] = 0;
            matrix[1, 1] = cf;
            matrix[1, 2] = 0;
            matrix[1, 3] = 0;
            matrix[2, 0] = sp;
            matrix[2, 1] = -sf * cp;
            matrix[2, 2] = 0;
            matrix[2, 3] = 0;
            matrix[3, 0] = 0;
            matrix[3, 1] = 0;
            matrix[3, 2] = 0;
            matrix[3, 3] = 1;

            foreach (var polygon in polyhedron.polygons)
                for (int i = 0; i < polygon.lines.Length; i++)
                    polygon.ChangePoint(i, Multiplication(
                        polygon.lines[i].point1, matrix));
        }

        void PerspectiveProjection()
        {
            foreach (var polyhedron in polyhedrons)
                foreach (var polygon in polyhedron.polygons)
                    foreach (var line in polygon.lines)
                        DrawLine(line, polyhedron.point);
            Point xy = new(map.Width / 2, map.Height / 2);
            Point x = new(map.Width, map.Height / 2);
            Point y = new(map.Width / 2, 0);
            graphics.DrawLine(axes, xy, x);
            graphics.DrawLine(axes, xy, y);
            pictureBox.Image = map;
        }

        private void ShowPolyhedrons()
        {
            graphics.Clear(Color.White);
            if (isPerspective)
                PerspectiveProjection();
            else
                AxonometricProjection();
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
        }

        private void PictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouseDown || polyhedrons.Count() < 1)
                return;

            float dx;
            float dy;
            PointsDistance(mousePosition, e.Location, out dx, out dy);
            Polyhedron polyhedron = polyhedrons[index];

            if (state == State.Position)
                Shift(ref polyhedron, dx, -dy);
            else if (state == State.Rotation)
            {
                Rotate(ref polyhedron, dy / 3000, 0);
                Rotate(ref polyhedron, dx / 3000, 1);
            }
            else if (state == State.Scale)
                Scale(ref polyhedron, dx < 0 ? 1 / (1 + (-dx / 100)) : 1 + dx / 100);
            else if (state == State.Reflection)
                buttonReflect.Enabled = true;
            else if (state == State.RotationAroundLine)
            {
                buttonRotation.Enabled = true;
                textAngel.Enabled = true;
                textX1.Enabled = true;
                textX2.Enabled = true;
                textY1.Enabled = true;
                textY2.Enabled = true;
                textZ1.Enabled = true;
                textZ2.Enabled = true;
            }
            else if (state == State.ToAxon)
                buttonToAxonometric.Enabled = true;
            polyhedrons[index] = polyhedron;
            ShowPolyhedrons();
            mousePosition = e.Location;
        }

        private void PictureBoxMouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void AddPolyhedron(int polyhedron)
        {
            if (polyhedrons.Count == 0)
                index = 0;
            if (polyhedron == 0)
                polyhedrons.Add(Tetrahedron(200, new PointXYZ(0, 0, 0)));
            else if (polyhedron == 1)
                polyhedrons.Add(Hexahedron(200, new PointXYZ(0, 0, 0)));
            else if (polyhedron == 2)
                polyhedrons.Add(Octahedron(200, new PointXYZ(0, 0, 0)));
            ShowPolyhedrons();
        }

        private void FormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
                state = State.Position;
            else if (e.KeyCode == Keys.S)
                state = State.Rotation;
            else if (e.KeyCode == Keys.D)
                state = State.Scale;
            else if (e.KeyCode == Keys.F)
                state = State.Reflection;
            else if (e.KeyCode == Keys.G)
                state = State.RotationAroundLine;
            else if (e.KeyCode == Keys.H)
                state = State.ToAxon;
            else if (e.KeyCode == Keys.D1)
                plane = Plane.XY;
            else if (e.KeyCode == Keys.D2)
                plane = Plane.YZ;
            else if (e.KeyCode == Keys.D3)
                plane = Plane.XZ;
            else if (e.KeyCode == Keys.Q)
                AddPolyhedron(0);
            else if (e.KeyCode == Keys.W)
                AddPolyhedron(1);
            else if (e.KeyCode == Keys.E)
                AddPolyhedron(2);
            else if (e.KeyCode == Keys.OemMinus)
            {
                index--;
                if (index < 0)
                    index = polyhedrons.Count - 1;
            }
            else if (e.KeyCode == Keys.Oemplus)
            {
                index++;
                if (index >= polyhedrons.Count)
                    index = 0;
            }
            else if (e.KeyCode == Keys.Back)
            {
                if (polyhedrons.Count < 1)
                    return;

                polyhedrons.RemoveAt(index);
                if (index >= polyhedrons.Count)
                    index = polyhedrons.Count - 1;
                ShowPolyhedrons();
            }
            else if (e.KeyCode == Keys.Z)
            {
                isPerspective = false;
                ShowPolyhedrons();
            }
            else if (e.KeyCode == Keys.X)
            {
                isPerspective = true;
                ShowPolyhedrons();
            }
        }

        private void ButtonRotateAroundLine(object sender, EventArgs e)
        {
            Polyhedron polyhedron = polyhedrons[0];

            PointXYZ point1 = new PointXYZ();
            PointXYZ point2 = new PointXYZ();
            int angel = int.Parse(textAngel.Text);
            point1.x = int.Parse(textX1.Text);
            point1.y = int.Parse(textY1.Text);
            point1.z = int.Parse(textZ1.Text);
            point2.x = int.Parse(textX2.Text);
            point2.y = int.Parse(textY2.Text);
            point2.z = int.Parse(textZ2.Text);
            RotationAroundLine(ref polyhedron, point1, point2, angel);

            polyhedrons[0] = polyhedron;
            ShowPolyhedrons();
            buttonRotation.Enabled = false;
            textX1.Enabled = false;
            textX2.Enabled = false;
            textY1.Enabled = false;
            textY2.Enabled = false;
            textZ1.Enabled = false;
            textZ2.Enabled = false;
            textAngel.Enabled = false;
        }

        private void Buttonreflect(object sender, EventArgs e)
        {
            Polyhedron polyhedron = polyhedrons[0];
            if (state == State.Reflection)
                Reflection(ref polyhedron);
            polyhedrons[0] = polyhedron;
            ShowPolyhedrons();
            buttonReflect.Enabled = false;
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