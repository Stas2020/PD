namespace PDiscountCard
{
    partial class FTrposxRunComplited
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
            this.components = new System.ComponentModel.Container();
            this.label2 = new System.Windows.Forms.Label();
            this.panel13 = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Lucida Sans Unicode", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(60, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(808, 276);
            this.label2.TabIndex = 10;
            this.label2.Text = "label2";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel13
            // 
            this.panel13.BackgroundImage = global::PDiscountCard.Properties.Resources.phone;
            this.panel13.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panel13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel13.Location = new System.Drawing.Point(0, 0);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(100, 50);
            this.panel13.TabIndex = 11;
            this.panel13.Visible = false;
            this.panel13.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel13_MouseDown);
            this.panel13.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel13_MouseMove);
            this.panel13.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel13_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FTrposxRunComplited
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.panel13);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(11, 9, 11, 9);
            this.Name = "FTrposxRunComplited";
            this.Text = "FTrposxRunComplited";
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.panel13, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Timer timer1;

    }
}