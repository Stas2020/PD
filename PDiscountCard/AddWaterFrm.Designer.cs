namespace PDiscountCard
{
    partial class AddWaterFrm
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
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnNo = new System.Windows.Forms.Button();
            this.BtnYes = new System.Windows.Forms.Button();
            this.unitKeyboard1 = new PDiscountCard.UnitKeyboard();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(517, 103);
            this.label1.TabIndex = 0;
            this.label1.Text = "Введите номер официанта";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(143, 506);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(220, 40);
            this.button1.TabIndex = 2;
            this.button1.Text = "Отмена";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtnNo);
            this.panel1.Controls.Add(this.BtnYes);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 103);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(517, 51);
            this.panel1.TabIndex = 3;
            this.panel1.Visible = false;
            // 
            // BtnNo
            // 
            this.BtnNo.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtnNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BtnNo.Location = new System.Drawing.Point(297, 0);
            this.BtnNo.Margin = new System.Windows.Forms.Padding(10);
            this.BtnNo.Name = "BtnNo";
            this.BtnNo.Size = new System.Drawing.Size(220, 51);
            this.BtnNo.TabIndex = 4;
            this.BtnNo.Text = "Нет";
            this.BtnNo.UseVisualStyleBackColor = true;
            this.BtnNo.Click += new System.EventHandler(this.BtnNo_Click);
            // 
            // BtnYes
            // 
            this.BtnYes.Dock = System.Windows.Forms.DockStyle.Left;
            this.BtnYes.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BtnYes.Location = new System.Drawing.Point(0, 0);
            this.BtnYes.Margin = new System.Windows.Forms.Padding(10);
            this.BtnYes.Name = "BtnYes";
            this.BtnYes.Size = new System.Drawing.Size(220, 51);
            this.BtnYes.TabIndex = 3;
            this.BtnYes.Text = "Да";
            this.BtnYes.UseVisualStyleBackColor = true;
            this.BtnYes.Click += new System.EventHandler(this.BtnYes_Click);
            // 
            // unitKeyboard1
            // 
            this.unitKeyboard1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.unitKeyboard1.Location = new System.Drawing.Point(143, 160);
            this.unitKeyboard1.MaximumSize = new System.Drawing.Size(220, 314);
            this.unitKeyboard1.MinimumSize = new System.Drawing.Size(220, 314);
            this.unitKeyboard1.Name = "unitKeyboard1";
            this.unitKeyboard1.Size = new System.Drawing.Size(220, 314);
            this.unitKeyboard1.TabIndex = 1;
            // 
            // AddWaterFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 578);
            this.Controls.Add(this.unitKeyboard1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "AddWaterFrm";
            this.Text = "Назначить официанта на чек";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private UnitKeyboard unitKeyboard1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtnNo;
        private System.Windows.Forms.Button BtnYes;
    }
}