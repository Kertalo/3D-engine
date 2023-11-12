namespace Rogalik_s_3D
{
    partial class Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
            pictureBox = new PictureBox();
            graph = new Button();
            polyhedron1 = new Button();
            polyhedron2 = new Button();
            polyhedron3 = new Button();
            position = new Button();
            rotation = new Button();
            scale = new Button();
            panel1 = new Panel();
            rZ = new TextBox();
            rY = new TextBox();
            rX = new TextBox();
            pZ = new TextBox();
            pY = new TextBox();
            transform = new Label();
            pX = new TextBox();
            perspective = new Button();
            axonometric = new Button();
            comboBox = new ComboBox();
            download = new Button();
            upload = new Button();
            panel2 = new Panel();
            graphSplit = new TextBox();
            graphY2 = new TextBox();
            graphY1 = new TextBox();
            graphX2 = new TextBox();
            graphX1 = new TextBox();
            label1 = new Label();
            panel3 = new Panel();
            left = new Button();
            right = new Button();
            label2 = new Label();
            delete = new Button();
            create3d = new Button();
            draw = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox
            // 
            pictureBox.BackColor = Color.DimGray;
            pictureBox.BackgroundImageLayout = ImageLayout.None;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            pictureBox.Cursor = Cursors.Cross;
            pictureBox.Location = new Point(48, 8);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(705, 705);
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            pictureBox.MouseDown += PictureBoxMouseDown;
            pictureBox.MouseMove += PictureBoxMouseMove;
            pictureBox.MouseUp += PictureBoxMouseUp;
            // 
            // graph
            // 
            graph.Location = new Point(20, 96);
            graph.Name = "graph";
            graph.Size = new Size(63, 29);
            graph.TabIndex = 21;
            graph.Text = "Ok";
            graph.UseVisualStyleBackColor = true;
            graph.Click += GraphClick;
            // 
            // polyhedron1
            // 
            polyhedron1.Location = new Point(8, 8);
            polyhedron1.Name = "polyhedron1";
            polyhedron1.Size = new Size(32, 32);
            polyhedron1.TabIndex = 24;
            polyhedron1.UseVisualStyleBackColor = true;
            polyhedron1.Click += PolyhedronClick;
            // 
            // polyhedron2
            // 
            polyhedron2.BackgroundImage = (Image)resources.GetObject("polyhedron2.BackgroundImage");
            polyhedron2.BackgroundImageLayout = ImageLayout.Stretch;
            polyhedron2.FlatStyle = FlatStyle.Flat;
            polyhedron2.Location = new Point(8, 48);
            polyhedron2.Name = "polyhedron2";
            polyhedron2.Size = new Size(32, 32);
            polyhedron2.TabIndex = 25;
            polyhedron2.UseVisualStyleBackColor = true;
            polyhedron2.Click += PolyhedronClick;
            // 
            // polyhedron3
            // 
            polyhedron3.Location = new Point(8, 88);
            polyhedron3.Name = "polyhedron3";
            polyhedron3.Size = new Size(32, 32);
            polyhedron3.TabIndex = 26;
            polyhedron3.Text = "3";
            polyhedron3.UseVisualStyleBackColor = true;
            polyhedron3.Click += PolyhedronClick;
            // 
            // position
            // 
            position.BackgroundImage = (Image)resources.GetObject("position.BackgroundImage");
            position.BackgroundImageLayout = ImageLayout.Stretch;
            position.FlatStyle = FlatStyle.Flat;
            position.Location = new Point(8, 248);
            position.Name = "position";
            position.Size = new Size(32, 32);
            position.TabIndex = 27;
            position.UseVisualStyleBackColor = true;
            position.Click += StateClick;
            // 
            // rotation
            // 
            rotation.BackgroundImage = (Image)resources.GetObject("rotation.BackgroundImage");
            rotation.BackgroundImageLayout = ImageLayout.Stretch;
            rotation.FlatStyle = FlatStyle.Flat;
            rotation.Location = new Point(8, 288);
            rotation.Name = "rotation";
            rotation.Size = new Size(32, 32);
            rotation.TabIndex = 28;
            rotation.UseVisualStyleBackColor = true;
            rotation.Click += StateClick;
            // 
            // scale
            // 
            scale.BackgroundImage = (Image)resources.GetObject("scale.BackgroundImage");
            scale.BackgroundImageLayout = ImageLayout.Stretch;
            scale.FlatStyle = FlatStyle.Flat;
            scale.Location = new Point(8, 328);
            scale.Name = "scale";
            scale.Size = new Size(32, 32);
            scale.TabIndex = 29;
            scale.UseVisualStyleBackColor = true;
            scale.Click += StateClick;
            // 
            // panel1
            // 
            panel1.Controls.Add(rZ);
            panel1.Controls.Add(rY);
            panel1.Controls.Add(rX);
            panel1.Controls.Add(pZ);
            panel1.Controls.Add(pY);
            panel1.Controls.Add(transform);
            panel1.Controls.Add(pX);
            panel1.Location = new Point(761, 8);
            panel1.Name = "panel1";
            panel1.Size = new Size(239, 128);
            panel1.TabIndex = 30;
            // 
            // rZ
            // 
            rZ.Location = new Point(184, 68);
            rZ.Name = "rZ";
            rZ.Size = new Size(55, 27);
            rZ.TabIndex = 37;
            // 
            // rY
            // 
            rY.Location = new Point(108, 68);
            rY.Name = "rY";
            rY.Size = new Size(55, 27);
            rY.TabIndex = 36;
            // 
            // rX
            // 
            rX.Location = new Point(28, 68);
            rX.Name = "rX";
            rX.Size = new Size(55, 27);
            rX.TabIndex = 35;
            // 
            // pZ
            // 
            pZ.Location = new Point(184, 35);
            pZ.Name = "pZ";
            pZ.Size = new Size(55, 27);
            pZ.TabIndex = 34;
            // 
            // pY
            // 
            pY.Location = new Point(108, 35);
            pY.Name = "pY";
            pY.Size = new Size(55, 27);
            pY.TabIndex = 33;
            // 
            // transform
            // 
            transform.Anchor = AnchorStyles.Top;
            transform.AutoSize = true;
            transform.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            transform.ForeColor = Color.White;
            transform.Location = new Point(41, 0);
            transform.Name = "transform";
            transform.Size = new Size(155, 23);
            transform.TabIndex = 32;
            transform.Text = "Transform camera";
            transform.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pX
            // 
            pX.Location = new Point(28, 35);
            pX.Name = "pX";
            pX.Size = new Size(55, 27);
            pX.TabIndex = 31;
            // 
            // perspective
            // 
            perspective.BackgroundImage = (Image)resources.GetObject("perspective.BackgroundImage");
            perspective.BackgroundImageLayout = ImageLayout.Stretch;
            perspective.FlatStyle = FlatStyle.Flat;
            perspective.Location = new Point(8, 448);
            perspective.Name = "perspective";
            perspective.Size = new Size(32, 32);
            perspective.TabIndex = 31;
            perspective.UseVisualStyleBackColor = true;
            perspective.Click += ProjectionClick;
            // 
            // axonometric
            // 
            axonometric.BackgroundImage = (Image)resources.GetObject("axonometric.BackgroundImage");
            axonometric.BackgroundImageLayout = ImageLayout.Stretch;
            axonometric.FlatStyle = FlatStyle.Flat;
            axonometric.Location = new Point(8, 488);
            axonometric.Name = "axonometric";
            axonometric.Size = new Size(32, 32);
            axonometric.TabIndex = 32;
            axonometric.UseVisualStyleBackColor = true;
            axonometric.Click += ProjectionClick;
            // 
            // comboBox
            // 
            comboBox.BackColor = Color.Black;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.ForeColor = Color.White;
            comboBox.FormattingEnabled = true;
            comboBox.Items.AddRange(new object[] { "XYZ", "X", "Y", "Z" });
            comboBox.Location = new Point(0, 168);
            comboBox.Name = "comboBox";
            comboBox.Size = new Size(48, 28);
            comboBox.TabIndex = 33;
            comboBox.SelectedIndexChanged += ComboBoxSelectedIndexChanged;
            // 
            // download
            // 
            download.BackgroundImage = (Image)resources.GetObject("download.BackgroundImage");
            download.BackgroundImageLayout = ImageLayout.Stretch;
            download.FlatStyle = FlatStyle.Flat;
            download.Location = new Point(8, 568);
            download.Name = "download";
            download.Size = new Size(32, 32);
            download.TabIndex = 34;
            download.UseVisualStyleBackColor = true;
            download.Click += DownloadClick;
            // 
            // upload
            // 
            upload.BackgroundImage = (Image)resources.GetObject("upload.BackgroundImage");
            upload.BackgroundImageLayout = ImageLayout.Stretch;
            upload.FlatStyle = FlatStyle.Flat;
            upload.Location = new Point(8, 608);
            upload.Name = "upload";
            upload.Size = new Size(32, 32);
            upload.TabIndex = 35;
            upload.UseVisualStyleBackColor = true;
            upload.Click += UploadClick;
            // 
            // panel2
            // 
            panel2.Controls.Add(graphSplit);
            panel2.Controls.Add(graphY2);
            panel2.Controls.Add(graphY1);
            panel2.Controls.Add(graphX2);
            panel2.Controls.Add(graphX1);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(graph);
            panel2.Location = new Point(761, 144);
            panel2.Name = "panel2";
            panel2.Size = new Size(239, 128);
            panel2.TabIndex = 38;
            // 
            // graphSplit
            // 
            graphSplit.Location = new Point(141, 96);
            graphSplit.Name = "graphSplit";
            graphSplit.Size = new Size(55, 27);
            graphSplit.TabIndex = 38;
            // 
            // graphY2
            // 
            graphY2.Location = new Point(184, 68);
            graphY2.Name = "graphY2";
            graphY2.Size = new Size(55, 27);
            graphY2.TabIndex = 37;
            // 
            // graphY1
            // 
            graphY1.Location = new Point(108, 68);
            graphY1.Name = "graphY1";
            graphY1.Size = new Size(55, 27);
            graphY1.TabIndex = 36;
            // 
            // graphX2
            // 
            graphX2.Location = new Point(184, 35);
            graphX2.Name = "graphX2";
            graphX2.Size = new Size(55, 27);
            graphX2.TabIndex = 34;
            // 
            // graphX1
            // 
            graphX1.Location = new Point(108, 35);
            graphX1.Name = "graphX1";
            graphX1.Size = new Size(55, 27);
            graphX1.TabIndex = 33;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.White;
            label1.Location = new Point(92, 0);
            label1.Name = "label1";
            label1.Size = new Size(59, 23);
            label1.TabIndex = 32;
            label1.Text = "Graph";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            panel3.Controls.Add(left);
            panel3.Controls.Add(right);
            panel3.Controls.Add(label2);
            panel3.Controls.Add(delete);
            panel3.Location = new Point(761, 280);
            panel3.Name = "panel3";
            panel3.Size = new Size(239, 128);
            panel3.TabIndex = 39;
            // 
            // left
            // 
            left.Location = new Point(0, 51);
            left.Name = "left";
            left.Size = new Size(63, 29);
            left.TabIndex = 34;
            left.Text = "<-";
            left.UseVisualStyleBackColor = true;
            left.Click += LeftRightClick;
            // 
            // right
            // 
            right.Location = new Point(88, 50);
            right.Name = "right";
            right.Size = new Size(63, 29);
            right.TabIndex = 33;
            right.Text = "->";
            right.UseVisualStyleBackColor = true;
            right.Click += LeftRightClick;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.White;
            label2.Location = new Point(92, 0);
            label2.Name = "label2";
            label2.Size = new Size(63, 23);
            label2.TabIndex = 32;
            label2.Text = "Object";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // delete
            // 
            delete.Location = new Point(170, 50);
            delete.Name = "delete";
            delete.Size = new Size(63, 29);
            delete.TabIndex = 21;
            delete.Text = "Delete";
            delete.UseVisualStyleBackColor = true;
            delete.Click += DeleteClick;
            // 
            // create3d
            // 
            create3d.Location = new Point(837, 448);
            create3d.Name = "create3d";
            create3d.Size = new Size(87, 29);
            create3d.TabIndex = 35;
            create3d.Text = "Create 3D";
            create3d.UseVisualStyleBackColor = true;
            create3d.Click += Create3DClick;
            // 
            // draw
            // 
            draw.BackgroundImage = (Image)resources.GetObject("draw.BackgroundImage");
            draw.BackgroundImageLayout = ImageLayout.Stretch;
            draw.FlatStyle = FlatStyle.Flat;
            draw.Location = new Point(8, 368);
            draw.Name = "draw";
            draw.Size = new Size(32, 32);
            draw.TabIndex = 40;
            draw.UseVisualStyleBackColor = true;
            draw.Click += StateClick;
            // 
            // Form
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DimGray;
            ClientSize = new Size(1006, 721);
            Controls.Add(draw);
            Controls.Add(create3d);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(upload);
            Controls.Add(download);
            Controls.Add(comboBox);
            Controls.Add(axonometric);
            Controls.Add(perspective);
            Controls.Add(panel1);
            Controls.Add(scale);
            Controls.Add(rotation);
            Controls.Add(position);
            Controls.Add(polyhedron3);
            Controls.Add(polyhedron2);
            Controls.Add(polyhedron1);
            Controls.Add(pictureBox);
            Name = "Form";
            Text = "Rogalik's 3D";
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox;
        private Button graph;
        private Button polyhedron1;
        private Button polyhedron2;
        private Button polyhedron3;
        private Button position;
        private Button rotation;
        private Button scale;
        private Panel panel1;
        private Label transform;
        private TextBox pX;
        private Button perspective;
        private Button axonometric;
        private TextBox rZ;
        private TextBox rY;
        private TextBox rX;
        private TextBox pZ;
        private TextBox pY;
        private ComboBox comboBox;
        private Button download;
        private Button upload;
        private Panel panel2;
        private TextBox graphY2;
        private TextBox graphY1;
        private TextBox graphX2;
        private TextBox graphX1;
        private Label label1;
        private Panel panel3;
        private Button left;
        private Button right;
        private Label label2;
        private Button delete;
        private Button create3d;
        private Button draw;
        private TextBox graphSplit;
    }
}