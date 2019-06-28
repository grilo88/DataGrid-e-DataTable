namespace DataGridDataTable
{
    partial class frmProcessando
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
            this.lblProcessando = new System.Windows.Forms.Label();
            this.lblLegenda = new System.Windows.Forms.Label();
            this.pnForm = new System.Windows.Forms.Panel();
            this.tmrAni = new System.Windows.Forms.Timer(this.components);
            this.pnForm.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblProcessando
            // 
            this.lblProcessando.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProcessando.BackColor = System.Drawing.Color.Transparent;
            this.lblProcessando.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessando.Location = new System.Drawing.Point(3, 22);
            this.lblProcessando.Name = "lblProcessando";
            this.lblProcessando.Size = new System.Drawing.Size(318, 27);
            this.lblProcessando.TabIndex = 0;
            this.lblProcessando.Text = "Processando, aguarde...";
            this.lblProcessando.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLegenda
            // 
            this.lblLegenda.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLegenda.BackColor = System.Drawing.Color.Transparent;
            this.lblLegenda.Location = new System.Drawing.Point(6, 49);
            this.lblLegenda.Name = "lblLegenda";
            this.lblLegenda.Size = new System.Drawing.Size(315, 23);
            this.lblLegenda.TabIndex = 1;
            this.lblLegenda.Text = "######################";
            this.lblLegenda.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnForm
            // 
            this.pnForm.BackColor = System.Drawing.Color.White;
            this.pnForm.Controls.Add(this.lblProcessando);
            this.pnForm.Controls.Add(this.lblLegenda);
            this.pnForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnForm.Location = new System.Drawing.Point(1, 1);
            this.pnForm.Name = "pnForm";
            this.pnForm.Size = new System.Drawing.Size(324, 98);
            this.pnForm.TabIndex = 2;
            this.pnForm.Paint += new System.Windows.Forms.PaintEventHandler(this.PnForm_Paint);
            // 
            // tmrAni
            // 
            this.tmrAni.Enabled = true;
            this.tmrAni.Interval = 1;
            this.tmrAni.Tick += new System.EventHandler(this.TmrAni_Tick);
            // 
            // frmProcessando
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Green;
            this.ClientSize = new System.Drawing.Size(326, 100);
            this.Controls.Add(this.pnForm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmProcessando";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Processando";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmProcessando_FormClosing);
            this.Load += new System.EventHandler(this.FrmProcessando_Load);
            this.pnForm.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblProcessando;
        private System.Windows.Forms.Label lblLegenda;
        private System.Windows.Forms.Panel pnForm;
        private System.Windows.Forms.Timer tmrAni;
    }
}