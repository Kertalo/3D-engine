using System.Numerics;
using System.Globalization;
using static System.Windows.Forms.AxHost;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Rogalik_s_3D
{
    public partial class Form : System.Windows.Forms.Form
    {
        Bitmap map;
        Graphics graphics;
        Pen pen = new Pen(Color.White, 3);

        enum State { Position, Rotation, Scale, Draw }
        enum Plane { X, Y, Z, XYZ }
        State state = State.Position;
        Plane plane = Plane.XYZ;
        PointF mousePosition;
        List<PointF> pointsRotation = new();
        Vector3 cameraPosition = new();
        Vector3 cameraRotation = new();

        List<Polyhedron> polyhedrons = new List<Polyhedron>();
        bool isMouseDown = false;
        int indexPolyhedron = 0;
        bool isPerspective = true;

        private float[,] zBuffer;
        private Color[,] frameBuffer;

        public Form()
        {
            InitializeComponent();
            map = new Bitmap(pictureBox.Width, pictureBox.Height);
            graphics = Graphics.FromImage(map);

            zBuffer = new float[pictureBox.Width, pictureBox.Height];
            frameBuffer = new Color[pictureBox.Width, pictureBox.Height];

            comboBox.SelectedItem = "XYZ";
            SetCamera(new(0, 0, -10), new(0, 0, 0));
            ShowPolyhedrons();
        }

        private void SetCamera(Vector3 point, Vector3 vector)
        {
            cameraPosition = point;
            cameraRotation = vector;
            pX.Text = cameraPosition.X.ToString();
            pY.Text = cameraPosition.Y.ToString();
            pZ.Text = cameraPosition.Z.ToString();
            rX.Text = cameraRotation.X.ToString();
            rY.Text = cameraRotation.Y.ToString();
            rZ.Text = cameraRotation.Z.ToString();
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
            polygons[0] = new(new int[] { 0, 1, 2 });
            polygons[1] = new(new int[] { 0, 1, 3 });
            polygons[2] = new(new int[] { 1, 2, 3 });
            polygons[3] = new(new int[] { 0, 2, 3 });

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

        private Vector3 Equation(float x, float y)
        {
            double r = x * x + y * y;
            return new(x, y, (float)(Math.Cos(r) / (r + 1)));
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

        void Shift(ref Polyhedron polyhedron, Vector3 vector)
        {
            float[,] matrix = new float[,]
                { {  1,  0, 0, 0 },
                  {  0,  1, 0, 0 },
                  {  0,  0, 1, 0 },
                  { vector.X, vector.Y, 0, 1 } };

            polyhedron.point = new(Multiplication(polyhedron.point, matrix));
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
            if (!isMouseDown || polyhedrons.Count() < 1 || state == State.Draw)
                return;

            PointsDistance(mousePosition, e.Location, out float dx, out float dy);
            Polyhedron polyhedron = polyhedrons[indexPolyhedron];

            if (state == State.Position)
                Shift(ref polyhedron, new(dx / 100, -dy / 100, 0));
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
                float.TryParse(rX.Text, out float rx) &&
                float.TryParse(rY.Text, out float ry) &&
                float.TryParse(rZ.Text, out float rz))
            {
                cameraPosition = new(px, py, pz);
                cameraRotation = new(rx, ry, rz);
            }

            ShowPolyhedrons();
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
                    p1.X = (int)(map.Width / 2 + p1.X * 100);
                    p1.Y = (int)(map.Height / 2 - p1.Y * 100);
                    p1.Z = (int)(p1.Z * 100);

                    var p2 = vertex[1];
                    p2.X = (int)(map.Width / 2 + p2.X * 100);
                    p2.Y = (int)(map.Height / 2 - p2.Y * 100);
                    p2.Z = (int)(p2.Z * 100);

                    var p3 = vertex[2];
                    p3.X = (int)(map.Width / 2 + p3.X * 100);
                    p3.Y = (int)(map.Height / 2 - p3.Y * 100);
                    p3.Z = (int)(p3.Z * 100);

                    map.SetPixel((int)p1.X, (int)p1.Y, Color.White);
                    map.SetPixel((int)p2.X, (int)p2.Y, Color.White);
                    map.SetPixel((int)p3.X, (int)p3.Y, Color.White);
                    Random rnd = new Random();
                    var color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    ConvertToRaster(ref p1, ref p2, ref p3, color);
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

                    ApplyZBufferAlgorithm(x, y, 1, color);
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
    }
}

    public class Polygon
    {
        public int[] indexes;
        public Vector3 normal;

        public Polygon(int[] indexes)
        {
            this.indexes = indexes;
        }

        public Polygon(int[] indexes, Vector3 normal)
        {
            this.indexes = indexes;
            this.normal = normal;
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