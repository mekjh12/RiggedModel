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
            this.ckBoneBindPose = new System.Windows.Forms.CheckBox();
            this.cbAction = new System.Windows.Forms.ComboBox();
            this.btnIKSolved = new System.Windows.Forms.Button();
            this.lbPrint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.Animation = true;
            this.glControl1.BackColor = System.Drawing.Color.Gray;
            this.glControl1.ColorBits = ((uint)(24u));
            this.glControl1.DepthBits = ((uint)(24u));
            this.glControl1.Location = new System.Drawing.Point(13, 12);
            this.glControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.glControl1.MultisampleBits = ((uint)(0u));
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(706, 539);
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
            this.ckBoneVisible.Location = new System.Drawing.Point(872, 12);
            this.ckBoneVisible.Name = "ckBoneVisible";
            this.ckBoneVisible.Size = new System.Drawing.Size(95, 16);
            this.ckBoneVisible.TabIndex = 1;
            this.ckBoneVisible.Text = "Bone Visible";
            this.ckBoneVisible.UseVisualStyleBackColor = true;
            this.ckBoneVisible.CheckedChanged += new System.EventHandler(this.ckBoneVisible_CheckedChanged);
            // 
            // ckBoneBindPose
            // 
            this.ckBoneBindPose.AutoSize = true;
            this.ckBoneBindPose.Location = new System.Drawing.Point(739, 12);
            this.ckBoneBindPose.Name = "ckBoneBindPose";
            this.ckBoneBindPose.Size = new System.Drawing.Size(118, 16);
            this.ckBoneBindPose.TabIndex = 7;
            this.ckBoneBindPose.Text = "BindPose visible";
            this.ckBoneBindPose.UseVisualStyleBackColor = true;
            this.ckBoneBindPose.CheckedChanged += new System.EventHandler(this.ckBoneBindPose_CheckedChanged);
            // 
            // cbAction
            // 
            this.cbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAction.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAction.FormattingEnabled = true;
            this.cbAction.Location = new System.Drawing.Point(730, 510);
            this.cbAction.Name = "cbAction";
            this.cbAction.Size = new System.Drawing.Size(209, 26);
            this.cbAction.TabIndex = 11;
            this.cbAction.SelectedIndexChanged += new System.EventHandler(this.cbAction_SelectedIndexChanged);
            // 
            // btnIKSolved
            // 
            this.btnIKSolved.Location = new System.Drawing.Point(945, 510);
            this.btnIKSolved.Name = "btnIKSolved";
            this.btnIKSolved.Size = new System.Drawing.Size(75, 26);
            this.btnIKSolved.TabIndex = 12;
            this.btnIKSolved.Text = "IKSolved";
            this.btnIKSolved.UseVisualStyleBackColor = true;
            // 
            // lbPrint
            // 
            this.lbPrint.AutoSize = true;
            this.lbPrint.Location = new System.Drawing.Point(728, 539);
            this.lbPrint.Name = "lbPrint";
            this.lbPrint.Size = new System.Drawing.Size(49, 12);
            this.lbPrint.TabIndex = 14;
            this.lbPrint.Text = "LB Print";
            // 
            // Form3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1032, 563);
            this.Controls.Add(this.lbPrint);
            this.Controls.Add(this.btnIKSolved);
            this.Controls.Add(this.cbAction);
            this.Controls.Add(this.ckBoneBindPose);
            this.Controls.Add(this.ckBoneVisible);
            this.Controls.Add(this.glControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form3D";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rigged Model";
            this.Load += new System.EventHandler(this.Form3D_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenGL.GlControl glControl1;
        private System.Windows.Forms.CheckBox ckBoneVisible;
        private System.Windows.Forms.CheckBox ckBoneBindPose;
        private System.Windows.Forms.ComboBox cbAction;
        private System.Windows.Forms.Button btnIKSolved;
        private System.Windows.Forms.Label lbPrint;
    }
}