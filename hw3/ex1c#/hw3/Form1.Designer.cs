namespace hw3
{
    partial class Form1
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
            pictureBox1 = new PictureBox();
            nAttacks = new TextBox();
            nSystems = new TextBox();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(52, 53);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(698, 385);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // nAttacks
            // 
            nAttacks.Location = new Point(12, 12);
            nAttacks.Name = "nAttacks";
            nAttacks.PlaceholderText = "Attacks";
            nAttacks.Size = new Size(125, 27);
            nAttacks.TabIndex = 1;
            // 
            // nSystems
            // 
            nSystems.Location = new Point(12, 53);
            nSystems.Name = "nSystems";
            nSystems.PlaceholderText = "Systems";
            nSystems.Size = new Size(125, 27);
            nSystems.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(12, 97);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 3;
            button1.Text = "Calculate";
            button1.UseVisualStyleBackColor = true;
            button1.Click += buttonCalculate_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(nSystems);
            Controls.Add(nAttacks);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private TextBox nAttacks;
        private TextBox nSystems;
        private Button button1;
    }
}