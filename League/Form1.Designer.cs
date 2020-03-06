namespace League
{
    partial class Form1
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
            this.label2 = new System.Windows.Forms.Label();
            this.status = new System.Windows.Forms.Label();
            this.ChampPref0 = new System.Windows.Forms.Label();
            this.ChampPref1 = new System.Windows.Forms.Label();
            this.ChampPref2 = new System.Windows.Forms.Label();
            this.formdebug = new System.Windows.Forms.RichTextBox();
            this.autoAccept = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(43, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Status:";
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status.Location = new System.Drawing.Point(128, 45);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(79, 25);
            this.status.TabIndex = 4;
            this.status.Text = "Status:";
            // 
            // ChampPref0
            // 
            this.ChampPref0.AutoSize = true;
            this.ChampPref0.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChampPref0.Location = new System.Drawing.Point(27, 124);
            this.ChampPref0.Name = "ChampPref0";
            this.ChampPref0.Size = new System.Drawing.Size(60, 24);
            this.ChampPref0.TabIndex = 5;
            this.ChampPref0.Text = "label1";
            this.ChampPref0.DoubleClick += new System.EventHandler(this.ChampPref0_DoubleClick);
            // 
            // ChampPref1
            // 
            this.ChampPref1.AutoSize = true;
            this.ChampPref1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChampPref1.Location = new System.Drawing.Point(27, 170);
            this.ChampPref1.Name = "ChampPref1";
            this.ChampPref1.Size = new System.Drawing.Size(60, 24);
            this.ChampPref1.TabIndex = 6;
            this.ChampPref1.Text = "label1";
            this.ChampPref1.DoubleClick += new System.EventHandler(this.ChampPref1_DoubleClick);
            // 
            // ChampPref2
            // 
            this.ChampPref2.AutoSize = true;
            this.ChampPref2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChampPref2.Location = new System.Drawing.Point(27, 219);
            this.ChampPref2.Name = "ChampPref2";
            this.ChampPref2.Size = new System.Drawing.Size(60, 24);
            this.ChampPref2.TabIndex = 7;
            this.ChampPref2.Text = "label1";
            this.ChampPref2.DoubleClick += new System.EventHandler(this.ChampPref2_DoubleClick);
            // 
            // formdebug
            // 
            this.formdebug.Location = new System.Drawing.Point(342, 12);
            this.formdebug.Name = "formdebug";
            this.formdebug.Size = new System.Drawing.Size(1017, 600);
            this.formdebug.TabIndex = 16;
            this.formdebug.Text = "";
            // 
            // autoAccept
            // 
            this.autoAccept.AutoSize = true;
            this.autoAccept.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoAccept.Location = new System.Drawing.Point(61, 398);
            this.autoAccept.Name = "autoAccept";
            this.autoAccept.Size = new System.Drawing.Size(196, 28);
            this.autoAccept.TabIndex = 17;
            this.autoAccept.Text = "Auto Accept Queue";
            this.autoAccept.UseVisualStyleBackColor = true;
            this.autoAccept.CheckedChanged += new System.EventHandler(this.autoAccept_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1371, 624);
            this.Controls.Add(this.autoAccept);
            this.Controls.Add(this.formdebug);
            this.Controls.Add(this.ChampPref2);
            this.Controls.Add(this.ChampPref1);
            this.Controls.Add(this.ChampPref0);
            this.Controls.Add(this.status);
            this.Controls.Add(this.label2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Label ChampPref0;
        private System.Windows.Forms.Label ChampPref1;
        private System.Windows.Forms.Label ChampPref2;
        private System.Windows.Forms.RichTextBox formdebug;
        private System.Windows.Forms.CheckBox autoAccept;
    }
}

