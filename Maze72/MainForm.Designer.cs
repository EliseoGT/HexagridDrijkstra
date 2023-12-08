namespace Hexagrid
{
    partial class MainForm
    {
        //Variable de diseñador requerida
        private System.ComponentModel.IContainer components = null;

        //Despeja los recursos que no se esten utilizando
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        //Metodo requerido para usar el diseñor, no se debe modificar con el editor de codigo
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.RbtCuadrado = new System.Windows.Forms.RadioButton();
            this.RbtHexagon = new System.Windows.Forms.RadioButton();
            this.RbtTriangulo = new System.Windows.Forms.RadioButton();
            this.BtnReiniciar = new System.Windows.Forms.Button();
            this.BtnResolver = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // RbtCuadrado
            // 
            this.RbtCuadrado.AutoSize = true;
            this.RbtCuadrado.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.RbtCuadrado.Location = new System.Drawing.Point(17, 55);
            this.RbtCuadrado.Name = "RbtCuadrado";
            this.RbtCuadrado.Size = new System.Drawing.Size(87, 20);
            this.RbtCuadrado.TabIndex = 0;
            this.RbtCuadrado.TabStop = true;
            this.RbtCuadrado.Text = "Cuadrado";
            this.ToolTip.SetToolTip(this.RbtCuadrado, "Depth First Search algorithm");
            this.RbtCuadrado.UseVisualStyleBackColor = true;
            // 
            // RbtHexagon
            // 
            this.RbtHexagon.AutoSize = true;
            this.RbtHexagon.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.RbtHexagon.Location = new System.Drawing.Point(17, 81);
            this.RbtHexagon.Name = "RbtHexagon";
            this.RbtHexagon.Size = new System.Drawing.Size(90, 20);
            this.RbtHexagon.TabIndex = 1;
            this.RbtHexagon.TabStop = true;
            this.RbtHexagon.Text = "Hexagono";
            this.RbtHexagon.UseVisualStyleBackColor = true;
            // 
            // RbtTriangulo
            // 
            this.RbtTriangulo.AutoSize = true;
            this.RbtTriangulo.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.RbtTriangulo.Location = new System.Drawing.Point(17, 29);
            this.RbtTriangulo.Name = "RbtTriangulo";
            this.RbtTriangulo.Size = new System.Drawing.Size(85, 20);
            this.RbtTriangulo.TabIndex = 2;
            this.RbtTriangulo.TabStop = true;
            this.RbtTriangulo.Text = "Triangulo";
            this.RbtTriangulo.UseVisualStyleBackColor = true;
            // 
            // BtnReiniciar
            // 
            this.BtnReiniciar.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.BtnReiniciar.Location = new System.Drawing.Point(537, 154);
            this.BtnReiniciar.Name = "BtnReiniciar";
            this.BtnReiniciar.Size = new System.Drawing.Size(126, 28);
            this.BtnReiniciar.TabIndex = 7;
            this.BtnReiniciar.Text = "Reiniciar";
            this.BtnReiniciar.UseVisualStyleBackColor = true;
            this.BtnReiniciar.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // BtnResolver
            // 
            this.BtnResolver.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.BtnResolver.Location = new System.Drawing.Point(537, 188);
            this.BtnResolver.Name = "BtnResolver";
            this.BtnResolver.Size = new System.Drawing.Size(126, 28);
            this.BtnResolver.TabIndex = 8;
            this.BtnResolver.Text = "Resolver";
            this.BtnResolver.UseVisualStyleBackColor = true;
            this.BtnResolver.Click += new System.EventHandler(this.RealTimeButton_Click);
            // 
            // timer
            // 
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.RbtTriangulo);
            this.groupBox.Controls.Add(this.RbtCuadrado);
            this.groupBox.Controls.Add(this.RbtHexagon);
            this.groupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.groupBox.Location = new System.Drawing.Point(537, 22);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(126, 117);
            this.groupBox.TabIndex = 4;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Forma de la celda";
            this.groupBox.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 547);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.BtnResolver);
            this.Controls.Add(this.BtnReiniciar);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hexagrid con dijkstra";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.Button BtnReiniciar;
        private System.Windows.Forms.Button BtnResolver;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.RadioButton RbtCuadrado;
        private System.Windows.Forms.RadioButton RbtHexagon;
        private System.Windows.Forms.RadioButton RbtTriangulo;
        private System.Windows.Forms.GroupBox groupBox;
    }
}

