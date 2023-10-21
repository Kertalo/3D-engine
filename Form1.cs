using System.Drawing.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Rogalik_s_3D
{
    public partial class Form : System.Windows.Forms.Form
    {
        Bitmap map;
        Graphics graphics;
        Pen pen = new Pen(Color.Black, 1);

        enum State { Position, Rotation, Scale }
        State state = State.Position;
        PointF mousePosition;
        List<Polyhedron> polyhedrons = new List<Polyhedron>();
        bool isMouseDown = false;

        public Form()
        {
            InitializeComponent();
            map = new Bitmap(pictureBox.Width, pictureBox.Height);
            graphics = Graphics.FromImage(map);
            Polygon polygon1 = new(new PointXYZ[3]
            { new PointXYZ(100, 0, 0), new PointXYZ(-100, 0, 0), new PointXYZ(0, 0, 100) });
            Polygon polygon2 = new(new PointXYZ[3]
            { new PointXYZ(100, 0, 0), new PointXYZ(-100, 0, 0), new PointXYZ(0, 200, 0) });
            Polygon polygon3 = new(new PointXYZ[3]
            { new PointXYZ(100, 0, 0), new PointXYZ(0, 0, 100), new PointXYZ(0, 200, 0) });
            Polygon polygon4 = new(new PointXYZ[3]
            { new PointXYZ(-100, 0, 0), new PointXYZ(0, 0, 100), new PointXYZ(0, 200, 0) });
            Polygon[] polygons = new Polygon[] { polygon1, polygon2, polygon3, polygon4 };
            Polyhedron polyhedron = new Polyhedron(polygons, new PointXYZ(0, 0, 0));
            polyhedrons.Add(polyhedron);
            ShowPolyhedrons();
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
                (point.z + line.point1.z), map.Height / 2 -
                (point.y + line.point2.y) / (point.z + line.point1.z));

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

        private void ShowPolyhedrons()
        {
            graphics.Clear(Color.White);
            foreach (var polyhedron in polyhedrons)
                foreach (var polygon in polyhedron.polygons)
                    foreach (var line in polygon.lines)
                        DrawLine(line, polyhedron.point);
            pictureBox.Image = map;
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
            if (!isMouseDown)
                return;

            float dx;
            float dy;
            PointsDistance(mousePosition, e.Location, out dx, out dy);
            Polyhedron polyhedron = polyhedrons[0];

            if (state == State.Position)
                Shift(ref polyhedron, dx, -dy);
            else if (state == State.Rotation)
            {
                Rotate(ref polyhedron, dx / 3000, 0);
                Rotate(ref polyhedron, dy / 3000, 1);
            }
            else
                Scale(ref polyhedron, dx < 0 ? 1 / (1 + (-dx / 100)) : 1 + dx / 100);
            polyhedrons[0] = polyhedron;
            ShowPolyhedrons();
            mousePosition = e.Location;
        }

        private void PictureBoxMouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void FormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
                state = State.Position;
            else if (e.KeyCode == Keys.S)
                state = State.Rotation;
            else if (e.KeyCode == Keys.D)
                state = State.Scale;
        }
    }

    public class PointXYZ
    {
        public float x;
        public float y;
        public float z;
        public PointXYZ()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }
        public PointXYZ(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
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