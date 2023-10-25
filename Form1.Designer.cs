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
            pictureBox = new PictureBox();
            textAngel = new TextBox();
            label1 = new Label();
            textX1 = new TextBox();
            label2 = new Label();
            textY1 = new TextBox();
            label3 = new Label();
            textZ1 = new TextBox();
            label4 = new Label();
            textX2 = new TextBox();
            label5 = new Label();
            textY2 = new TextBox();
            label6 = new Label();
            textZ2 = new TextBox();
            label7 = new Label();
            buttonRotation = new Button();
            buttonReflect = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // pictureBox
            // 
            pictureBox.Location = new Point(12, 12);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(958, 629);
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            pictureBox.MouseDown += PictureBoxMouseDown;
            pictureBox.MouseMove += PictureBoxMouseMove;
            pictureBox.MouseUp += PictureBoxMouseUp;
            // 
            // textAngel
            // 
            textAngel.Enabled = false;
            textAngel.Location = new Point(12, 78);
            textAngel.Name = "textAngel";
            textAngel.Size = new Size(55, 27);
            textAngel.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(73, 78);
            label1.Name = "label1";
            label1.Size = new Size(48, 20);
            label1.TabIndex = 2;
            label1.Text = "Angel";
            // 
            // textX1
            // 
            textX1.Enabled = false;
            textX1.Location = new Point(12, 12);
            textX1.Name = "textX1";
            textX1.Size = new Size(55, 27);
            textX1.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(73, 15);
            label2.Name = "label2";
            label2.Size = new Size(26, 20);
            label2.TabIndex = 4;
            label2.Text = "X1";
            // 
            // textY1
            // 
            textY1.Enabled = false;
            textY1.Location = new Point(97, 12);
            textY1.Name = "textY1";
            textY1.Size = new Size(55, 27);
            textY1.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(158, 15);
            label3.Name = "label3";
            label3.Size = new Size(25, 20);
            label3.TabIndex = 6;
            label3.Text = "Y1";
            // 
            // textZ1
            // 
            textZ1.Enabled = false;
            textZ1.Location = new Point(181, 12);
            textZ1.Name = "textZ1";
            textZ1.Size = new Size(55, 27);
            textZ1.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(242, 15);
            label4.Name = "label4";
            label4.Size = new Size(26, 20);
            label4.TabIndex = 8;
            label4.Text = "Z1";
            // 
            // textX2
            // 
            textX2.Enabled = false;
            textX2.Location = new Point(12, 45);
            textX2.Name = "textX2";
            textX2.Size = new Size(55, 27);
            textX2.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(73, 48);
            label5.Name = "label5";
            label5.Size = new Size(26, 20);
            label5.TabIndex = 10;
            label5.Text = "X2";
            // 
            // textY2
            // 
            textY2.Enabled = false;
            textY2.Location = new Point(97, 45);
            textY2.Name = "textY2";
            textY2.Size = new Size(55, 27);
            textY2.TabIndex = 11;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(158, 48);
            label6.Name = "label6";
            label6.Size = new Size(25, 20);
            label6.TabIndex = 12;
            label6.Text = "Y2";
            // 
            // textZ2
            // 
            textZ2.Enabled = false;
            textZ2.Location = new Point(181, 45);
            textZ2.Name = "textZ2";
            textZ2.Size = new Size(55, 27);
            textZ2.TabIndex = 13;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(242, 48);
            label7.Name = "label7";
            label7.Size = new Size(26, 20);
            label7.TabIndex = 14;
            label7.Text = "Z2";
            // 
            // buttonRotation
            // 
            buttonRotation.Enabled = false;
            buttonRotation.Location = new Point(12, 111);
            buttonRotation.Name = "buttonRotation";
            buttonRotation.Size = new Size(63, 29);
            buttonRotation.TabIndex = 15;
            buttonRotation.Text = "Rotate";
            buttonRotation.UseVisualStyleBackColor = true;
            buttonRotation.Click += ButtonRotateAroundLine;
            // 
            // buttonReflect
            // 
            buttonReflect.Enabled = false;
            buttonReflect.Location = new Point(12, 146);
            buttonReflect.Name = "buttonReflect";
            buttonReflect.Size = new Size(63, 29);
            buttonReflect.TabIndex = 16;
            buttonReflect.Text = "Reflect";
            buttonReflect.UseVisualStyleBackColor = true;
            buttonReflect.Click += Buttonreflect;
            // 
            // Form
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 653);
            Controls.Add(buttonReflect);
            Controls.Add(buttonRotation);
            Controls.Add(label7);
            Controls.Add(textZ2);
            Controls.Add(label6);
            Controls.Add(textY2);
            Controls.Add(label5);
            Controls.Add(textX2);
            Controls.Add(label4);
            Controls.Add(textZ1);
            Controls.Add(label3);
            Controls.Add(textY1);
            Controls.Add(label2);
            Controls.Add(textX1);
            Controls.Add(label1);
            Controls.Add(textAngel);
            Controls.Add(pictureBox);
            Name = "Form";
            Text = "Rogalik's 3D";
            KeyDown += FormKeyDown;
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox;
        private TextBox textAngel;
        private Label label1;
        private TextBox textX1;
        private Label label2;
        private TextBox textY1;
        private Label label3;
        private TextBox textZ1;
        private Label label4;
        private TextBox textX2;
        private Label label5;
        private TextBox textY2;
        private Label label6;
        private TextBox textZ2;
        private Label label7;
        private Button buttonRotation;
        private Button buttonReflect;
    }
}