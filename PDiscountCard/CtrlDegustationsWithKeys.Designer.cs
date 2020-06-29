namespace PDiscountCard
{
    partial class CtrlDegustationsWithKeys
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.keyBoardControl1 = new PDiscountCard.KeyBoardControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(861, 51);
            this.label1.TabIndex = 0;
            this.label1.Text = "Введите имя";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // keyBoardControl1
            // 
            this.keyBoardControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyBoardControl1.Location = new System.Drawing.Point(0, 51);
            this.keyBoardControl1.Name = "keyBoardControl1";
            this.keyBoardControl1.Size = new System.Drawing.Size(861, 419);
            this.keyBoardControl1.TabIndex = 2;
            this.keyBoardControl1.Load += new System.EventHandler(this.keyBoardControl1_Load);
            // 
            // CtrlDegustationsWithKeys
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.keyBoardControl1);
            this.Controls.Add(this.label1);
            this.Name = "CtrlDegustationsWithKeys";
            this.Size = new System.Drawing.Size(861, 470);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        internal KeyBoardControl keyBoardControl1;
    }
}
