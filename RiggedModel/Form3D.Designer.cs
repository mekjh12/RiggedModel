namespace LSystem
{
    partial class Form3D
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.glControl1 = new OpenGL.GlControl();
            this.ckBoneVisible = new System.Windows.Forms.CheckBox();
            this.trFov = new System.Windows.Forms.TrackBar();
            this.lblFov = new System.Windows.Forms.Label();
            this.trTime = new System.Windows.Forms.TrackBar();
            this.lblTime = new System.Windows.Forms.Label();
            this.trAxisLength = new System.Windows.Forms.TrackBar();
            this.ckBoneBindPose = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trFov)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trAxisLength)).BeginInit();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.Animation = true;
            this.glControl1.BackColor = System.Drawing.Color.Gray;
            this.glControl1.ColorBits = ((uint)(24u));
            this.glControl1.DepthBits = ((uint)(24u));
            this.glControl1.Location = new System.Drawing.Point(0, 0);
            this.glControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.glControl1.MultisampleBits = ((uint)(0u));
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(675, 590);
            this.glControl1.StencilBits = ((uint)(8u));
            this.glControl1.TabIndex = 0;
            this.glControl1.Render += new System.EventHandler<OpenGL.GlControlEventArgs>(this.glControl1_Render);
            this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyDown);
            this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyUp);
            this.glControl1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDoubleClick);
            this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
            this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
            this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
            this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
            // 
            // ckBoneVisible
            // 
            this.ckBoneVisible.AutoSize = true;
            this.ckBoneVisible.Checked = true;
            this.ckBoneVisible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckBoneVisible.Location = new System.Drawing.Point(683, 13);
            this.ckBoneVisible.Name = "ckBoneVisible";
            this.ckBoneVisible.Size = new System.Drawing.Size(92, 16);
            this.ckBoneVisible.TabIndex = 1;
            this.ckBoneVisible.Text = "bone visible";
            this.ckBoneVisible.UseVisualStyleBackColor = true;
            this.ckBoneVisible.CheckedChanged += new System.EventHandler(this.ckBoneVisible_CheckedChanged);
            // 
            // trFov
            // 
            this.trFov.Location = new System.Drawing.Point(682, 35);
            this.trFov.Maximum = 160;
            this.trFov.Minimum = 30;
            this.trFov.Name = "trFov";
            this.trFov.Size = new System.Drawing.Size(226, 45);
            this.trFov.TabIndex = 2;
            this.trFov.Value = 30;
            this.trFov.Scroll += new System.EventHandler(this.trFov_Scroll);
            // 
            // lblFov
            // 
            this.lblFov.AutoSize = true;
            this.lblFov.Location = new System.Drawing.Point(692, 68);
            this.lblFov.Name = "lblFov";
            this.lblFov.Size = new System.Drawing.Size(43, 12);
            this.lblFov.TabIndex = 3;
            this.lblFov.Text = "Fov=45";
            // 
            // trTime
            // 
            this.trTime.Location = new System.Drawing.Point(683, 545);
            this.trTime.Maximum = 160;
            this.trTime.Minimum = 30;
            this.trTime.Name = "trTime";
            this.trTime.Size = new System.Drawing.Size(226, 45);
            this.trTime.TabIndex = 4;
            this.trTime.Value = 30;
            this.trTime.ValueChanged += new System.EventHandler(this.trTime_ValueChanged);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(692, 530);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(56, 12);
            this.lblTime.TabIndex = 5;
            this.lblTime.Text = "Time=0.0";
            // 
            // trAxisLength
            // 
            this.trAxisLength.Location = new System.Drawing.Point(683, 96);
            this.trAxisLength.Maximum = 300;
            this.trAxisLength.Minimum = 10;
            this.trAxisLength.Name = "trAxisLength";
            this.trAxisLength.Size = new System.Drawing.Size(226, 45);
            this.trAxisLength.TabIndex = 6;
            this.trAxisLength.Value = 30;
            this.trAxisLength.Scroll += new System.EventHandler(this.trAxisLength_Scroll);
            // 
            // ckBoneBindPose
            // 
            this.ckBoneBindPose.AutoSize = true;
            this.ckBoneBindPose.Location = new System.Drawing.Point(781, 13);
            this.ckBoneBindPose.Name = "ckBoneBindPose";
            this.ckBoneBindPose.Size = new System.Drawing.Size(118, 16);
            this.ckBoneBindPose.TabIndex = 7;
            this.ckBoneBindPose.Text = "BindPose visible";
            this.ckBoneBindPose.UseVisualStyleBackColor = true;
            // 
            // Form3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(920, 590);
            this.Controls.Add(this.ckBoneBindPose);
            this.Controls.Add(this.trAxisLength);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.trTime);
            this.Controls.Add(this.lblFov);
            this.Controls.Add(this.trFov);
            this.Controls.Add(this.ckBoneVisible);
            this.Controls.Add(this.glControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form3D";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rigged Model";
            this.Load += new System.EventHandler(this.Form3D_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trFov)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trAxisLength)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenGL.GlControl glControl1;
        private System.Windows.Forms.CheckBox ckBoneVisible;
        private System.Windows.Forms.TrackBar trFov;
        private System.Windows.Forms.Label lblFov;
        private System.Windows.Forms.TrackBar trTime;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.TrackBar trAxisLength;
        private System.Windows.Forms.CheckBox ckBoneBindPose;
    }
}