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
            createGraph = new Button();
            polyhedron1 = new Button();
            polyhedron2 = new Button();
            polyhedron3 = new Button();
            position = new Button();
            rotation = new Button();
            scale = new Button();
            panel1 = new Panel();
            transformOk = new Button();
            uZ = new TextBox();
            uY = new TextBox();
            uX = new TextBox();
            pZ = new TextBox();
            pY = new TextBox();
            transform = new Label();
            pX = new TextBox();
            axonometric = new Button();
            perspective = new Button();
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
            left = new Button();
            right = new Button();
            delete = new Button();
            draw = new Button();
            panel4 = new Panel();
            rotationSplit = new TextBox();
            label3 = new Label();
            createRotation = new Button();
            zBufferButton = new Button();
            rX = new TextBox();
            rY = new TextBox();
            rZ = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel4.SuspendLayout();
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
            pictureBox.Size = new Size(704, 665);
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            pictureBox.MouseDown += PictureBoxMouseDown;
            pictureBox.MouseMove += PictureBoxMouseMove;
            pictureBox.MouseUp += PictureBoxMouseUp;
            // 
            // createGraph
            // 
            createGraph.Location = new Point(20, 96);
            createGraph.Name = "createGraph";
            createGraph.Size = new Size(63, 29);
            createGraph.TabIndex = 21;
            createGraph.Text = "Ok";
            createGraph.UseVisualStyleBackColor = true;
            createGraph.Click += GraphClick;
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
            panel1.Controls.Add(transformOk);
            panel1.Controls.Add(uZ);
            panel1.Controls.Add(uY);
            panel1.Controls.Add(uX);
            panel1.Controls.Add(pZ);
            panel1.Controls.Add(pY);
            panel1.Controls.Add(transform);
            panel1.Controls.Add(pX);
            panel1.Location = new Point(761, 8);
            panel1.Name = "panel1";
            panel1.Size = new Size(239, 171);
            panel1.TabIndex = 30;
            // 
            // transformOk
            // 
            transformOk.Location = new Point(92, 139);
            transformOk.Name = "transformOk";
            transformOk.Size = new Size(63, 29);
            transformOk.TabIndex = 41;
            transformOk.Text = "Ok";
            transformOk.UseVisualStyleBackColor = true;
            transformOk.Click += TransformOkClick;
            // 
            // uZ
            // 
            uZ.Location = new Point(184, 68);
            uZ.Name = "uZ";
            uZ.Size = new Size(55, 27);
            uZ.TabIndex = 37;
            // 
            // uY
            // 
            uY.Location = new Point(108, 68);
            uY.Name = "uY";
            uY.Size = new Size(55, 27);
            uY.TabIndex = 36;
            // 
            // uX
            // 
            uX.Location = new Point(28, 68);
            uX.Name = "uX";
            uX.Size = new Size(55, 27);
            uX.TabIndex = 35;
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
            // axonometric
            // 
            axonometric.BackgroundImage = (Image)resources.GetObject("axonometric.BackgroundImage");
            axonometric.BackgroundImageLayout = ImageLayout.Stretch;
            axonometric.FlatStyle = FlatStyle.Flat;
            axonometric.Location = new Point(8, 448);
            axonometric.Name = "axonometric";
            axonometric.Size = new Size(32, 32);
            axonometric.TabIndex = 31;
            axonometric.UseVisualStyleBackColor = true;
            axonometric.Click += ProjectionClick;
            // 
            // perspective
            // 
            perspective.BackgroundImage = (Image)resources.GetObject("perspective.BackgroundImage");
            perspective.BackgroundImageLayout = ImageLayout.Stretch;
            perspective.FlatStyle = FlatStyle.Flat;
            perspective.Location = new Point(8, 488);
            perspective.Name = "perspective";
            perspective.Size = new Size(32, 32);
            perspective.TabIndex = 32;
            perspective.UseVisualStyleBackColor = true;
            perspective.Click += ProjectionClick;
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
            panel2.Controls.Add(createGraph);
            panel2.Location = new Point(761, 182);
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
            // left
            // 
            left.BackgroundImage = (Image)resources.GetObject("left.BackgroundImage");
            left.BackgroundImageLayout = ImageLayout.Stretch;
            left.FlatStyle = FlatStyle.Flat;
            left.Location = new Point(344, 681);
            left.Name = "left";
            left.Size = new Size(32, 32);
            left.TabIndex = 34;
            left.UseVisualStyleBackColor = true;
            left.Click += LeftRightClick;
            // 
            // right
            // 
            right.BackgroundImage = (Image)resources.GetObject("right.BackgroundImage");
            right.BackgroundImageLayout = ImageLayout.Stretch;
            right.FlatStyle = FlatStyle.Flat;
            right.Location = new Point(424, 681);
            right.Name = "right";
            right.Size = new Size(32, 32);
            right.TabIndex = 33;
            right.UseVisualStyleBackColor = true;
            right.Click += LeftRightClick;
            // 
            // delete
            // 
            delete.BackgroundImage = (Image)resources.GetObject("delete.BackgroundImage");
            delete.BackgroundImageLayout = ImageLayout.Stretch;
            delete.FlatStyle = FlatStyle.Flat;
            delete.Location = new Point(384, 681);
            delete.Name = "delete";
            delete.Size = new Size(32, 32);
            delete.TabIndex = 21;
            delete.UseVisualStyleBackColor = true;
            delete.Click += DeleteClick;
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
            // panel4
            // 
            panel4.Controls.Add(rotationSplit);
            panel4.Controls.Add(label3);
            panel4.Controls.Add(createRotation);
            panel4.Location = new Point(761, 414);
            panel4.Name = "panel4";
            panel4.Size = new Size(239, 128);
            panel4.TabIndex = 39;
            // 
            // rotationSplit
            // 
            rotationSplit.Location = new Point(54, 39);
            rotationSplit.Name = "rotationSplit";
            rotationSplit.Size = new Size(55, 27);
            rotationSplit.TabIndex = 38;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top;
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = Color.White;
            label3.Location = new Point(73, 1);
            label3.Name = "label3";
            label3.Size = new Size(107, 23);
            label3.TabIndex = 32;
            label3.Text = "Rotation 3D";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // createRotation
            // 
            createRotation.Location = new Point(133, 36);
            createRotation.Name = "createRotation";
            createRotation.Size = new Size(63, 29);
            createRotation.TabIndex = 21;
            createRotation.Text = "Ok";
            createRotation.UseVisualStyleBackColor = true;
            createRotation.Click += CreateRotationButton;
            // 
            // zBufferButton
            // 
            zBufferButton.BackgroundImage = (Image)resources.GetObject("zBufferButton.BackgroundImage");
            zBufferButton.BackgroundImageLayout = ImageLayout.Stretch;
            zBufferButton.FlatStyle = FlatStyle.Flat;
            zBufferButton.Location = new Point(761, 608);
            zBufferButton.Name = "zBufferButton";
            zBufferButton.Size = new Size(32, 32);
            zBufferButton.TabIndex = 41;
            zBufferButton.UseVisualStyleBackColor = true;
            zBufferButton.Click += zBufferButton_Click;
            // rX
            // 
            rX.Location = new Point(28, 101);
            rX.Name = "rX";
            rX.Size = new Size(55, 27);
            rX.TabIndex = 42;
            // 
            // rY
            // 
            rY.Location = new Point(108, 101);
            rY.Name = "rY";
            rY.Size = new Size(55, 27);
            rY.TabIndex = 43;
            // 
            // rZ
            // 
            rZ.Location = new Point(184, 101);
            rZ.Name = "rZ";
            rZ.Size = new Size(55, 27);
            rZ.TabIndex = 44;
            // 
            // Form
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DimGray;
            ClientSize = new Size(1006, 721);
            Controls.Add(zBufferButton);
            Controls.Add(left);
            Controls.Add(right);
            Controls.Add(panel4);
            Controls.Add(draw);
            Controls.Add(delete);
            Controls.Add(panel2);
            Controls.Add(upload);
            Controls.Add(download);
            Controls.Add(comboBox);
            Controls.Add(perspective);
            Controls.Add(axonometric);
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
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox;
        private Button createGraph;
        private Button polyhedron1;
        private Button polyhedron2;
        private Button polyhedron3;
        private Button position;
        private Button rotation;
        private Button scale;
        private Panel panel1;
        private Label transform;
        private TextBox pX;
        private Button axonometric;
        private Button perspective;
        private TextBox uZ;
        private TextBox uY;
        private TextBox uX;
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
        private Button left;
        private Button right;
        private Button delete;
        private Button draw;
        private TextBox graphSplit;
        private Panel panel4;
        private TextBox rotationSplit;
        private Label label3;
        private Button createRotation;
        private Button transformOk;
        private Button zBufferButton;
        private TextBox rZ;
        private TextBox rY;
        private TextBox rX;
    }
}
