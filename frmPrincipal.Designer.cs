﻿namespace DataGridDataTable
{
    partial class frmPrincipal
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.dg = new System.Windows.Forms.DataGridView();
            this.btnCarregar = new System.Windows.Forms.Button();
            this.btnAplicar = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.virtualDataGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRecarregar = new System.Windows.Forms.Button();
            this.btnGerarRows = new System.Windows.Forms.Button();
            this.txtPesquisar = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboCondicaoPesquisa = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkDiferenteDe = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkModoSelecao = new System.Windows.Forms.CheckBox();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.pnDg = new System.Windows.Forms.Panel();
            this.pnDgVScroll = new System.Windows.Forms.Panel();
            this.pnForm = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dg)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnDg.SuspendLayout();
            this.pnForm.SuspendLayout();
            this.SuspendLayout();
            // 
            // dg
            // 
            this.dg.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg.Location = new System.Drawing.Point(0, 0);
            this.dg.Name = "dg";
            this.dg.Size = new System.Drawing.Size(602, 272);
            this.dg.TabIndex = 0;
            // 
            // btnCarregar
            // 
            this.btnCarregar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCarregar.Location = new System.Drawing.Point(45, 425);
            this.btnCarregar.Name = "btnCarregar";
            this.btnCarregar.Size = new System.Drawing.Size(116, 23);
            this.btnCarregar.TabIndex = 1;
            this.btnCarregar.Text = "Carregar";
            this.btnCarregar.UseVisualStyleBackColor = true;
            this.btnCarregar.Click += new System.EventHandler(this.BtnCarregar_Click);
            // 
            // btnAplicar
            // 
            this.btnAplicar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAplicar.Location = new System.Drawing.Point(590, 425);
            this.btnAplicar.Name = "btnAplicar";
            this.btnAplicar.Size = new System.Drawing.Size(107, 23);
            this.btnAplicar.TabIndex = 2;
            this.btnAplicar.Text = "Aplicar";
            this.btnAplicar.UseVisualStyleBackColor = true;
            this.btnAplicar.Click += new System.EventHandler(this.BtnAplicar_Click);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button1.Location = new System.Drawing.Point(459, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Aplicar 100x";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.virtualDataGridToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(719, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // virtualDataGridToolStripMenuItem
            // 
            this.virtualDataGridToolStripMenuItem.Name = "virtualDataGridToolStripMenuItem";
            this.virtualDataGridToolStripMenuItem.Size = new System.Drawing.Size(102, 20);
            this.virtualDataGridToolStripMenuItem.Text = "Virtual DataGrid";
            this.virtualDataGridToolStripMenuItem.Click += new System.EventHandler(this.VirtualDataGridToolStripMenuItem_Click);
            // 
            // btnRecarregar
            // 
            this.btnRecarregar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRecarregar.Location = new System.Drawing.Point(167, 425);
            this.btnRecarregar.Name = "btnRecarregar";
            this.btnRecarregar.Size = new System.Drawing.Size(116, 23);
            this.btnRecarregar.TabIndex = 5;
            this.btnRecarregar.Text = "Recarregar";
            this.btnRecarregar.UseVisualStyleBackColor = true;
            this.btnRecarregar.Click += new System.EventHandler(this.BtnRecarregar_Click);
            // 
            // btnGerarRows
            // 
            this.btnGerarRows.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnGerarRows.Location = new System.Drawing.Point(590, 45);
            this.btnGerarRows.Name = "btnGerarRows";
            this.btnGerarRows.Size = new System.Drawing.Size(107, 23);
            this.btnGerarRows.TabIndex = 6;
            this.btnGerarRows.Text = "Gerar Rows";
            this.btnGerarRows.UseVisualStyleBackColor = true;
            this.btnGerarRows.Click += new System.EventHandler(this.BtnGerarRows_Click);
            // 
            // txtPesquisar
            // 
            this.txtPesquisar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPesquisar.Location = new System.Drawing.Point(258, 31);
            this.txtPesquisar.Name = "txtPesquisar";
            this.txtPesquisar.Size = new System.Drawing.Size(374, 20);
            this.txtPesquisar.TabIndex = 7;
            this.txtPesquisar.TextChanged += new System.EventHandler(this.TxtPesquisar_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(255, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Pesquisar";
            // 
            // cboCondicaoPesquisa
            // 
            this.cboCondicaoPesquisa.FormattingEnabled = true;
            this.cboCondicaoPesquisa.Location = new System.Drawing.Point(107, 31);
            this.cboCondicaoPesquisa.Name = "cboCondicaoPesquisa";
            this.cboCondicaoPesquisa.Size = new System.Drawing.Size(145, 21);
            this.cboCondicaoPesquisa.TabIndex = 9;
            this.cboCondicaoPesquisa.SelectedIndexChanged += new System.EventHandler(this.CboCondicaoPesquisa_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(104, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Condição";
            // 
            // chkDiferenteDe
            // 
            this.chkDiferenteDe.AutoSize = true;
            this.chkDiferenteDe.Location = new System.Drawing.Point(15, 33);
            this.chkDiferenteDe.Name = "chkDiferenteDe";
            this.chkDiferenteDe.Size = new System.Drawing.Size(86, 17);
            this.chkDiferenteDe.TabIndex = 11;
            this.chkDiferenteDe.Text = "Diferente De";
            this.chkDiferenteDe.UseVisualStyleBackColor = true;
            this.chkDiferenteDe.CheckedChanged += new System.EventHandler(this.ChkDiferenteDe_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chkDiferenteDe);
            this.groupBox1.Controls.Add(this.txtPesquisar);
            this.groupBox1.Controls.Add(this.cboCondicaoPesquisa);
            this.groupBox1.Controls.Add(this.chkModoSelecao);
            this.groupBox1.Location = new System.Drawing.Point(45, 68);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(652, 73);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pesquisar";
            // 
            // chkModoSelecao
            // 
            this.chkModoSelecao.AutoSize = true;
            this.chkModoSelecao.BackColor = System.Drawing.Color.Transparent;
            this.chkModoSelecao.Location = new System.Drawing.Point(539, 53);
            this.chkModoSelecao.Name = "chkModoSelecao";
            this.chkModoSelecao.Size = new System.Drawing.Size(93, 17);
            this.chkModoSelecao.TabIndex = 12;
            this.chkModoSelecao.Text = "Modo seleção";
            this.chkModoSelecao.UseVisualStyleBackColor = false;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancelar.Location = new System.Drawing.Point(477, 425);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(107, 23);
            this.btnCancelar.TabIndex = 13;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 455);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(719, 22);
            this.statusStrip.TabIndex = 14;
            this.statusStrip.Text = "Barra de Status";
            // 
            // pnDg
            // 
            this.pnDg.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnDg.BackColor = System.Drawing.Color.Green;
            this.pnDg.Controls.Add(this.pnDgVScroll);
            this.pnDg.Controls.Add(this.dg);
            this.pnDg.Location = new System.Drawing.Point(45, 147);
            this.pnDg.Name = "pnDg";
            this.pnDg.Padding = new System.Windows.Forms.Padding(0, 0, 50, 0);
            this.pnDg.Size = new System.Drawing.Size(652, 272);
            this.pnDg.TabIndex = 15;
            this.pnDg.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PnDg_MouseUp);
            // 
            // pnDgVScroll
            // 
            this.pnDgVScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnDgVScroll.BackColor = System.Drawing.Color.SpringGreen;
            this.pnDgVScroll.Location = new System.Drawing.Point(605, 80);
            this.pnDgVScroll.Name = "pnDgVScroll";
            this.pnDgVScroll.Size = new System.Drawing.Size(44, 20);
            this.pnDgVScroll.TabIndex = 1;
            this.pnDgVScroll.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnDgVScroll_MouseDown);
            this.pnDgVScroll.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PnDgVScroll_MouseMove);
            // 
            // pnForm
            // 
            this.pnForm.BackColor = System.Drawing.SystemColors.Control;
            this.pnForm.Controls.Add(this.pnDg);
            this.pnForm.Controls.Add(this.statusStrip);
            this.pnForm.Controls.Add(this.btnCancelar);
            this.pnForm.Controls.Add(this.menuStrip1);
            this.pnForm.Controls.Add(this.groupBox1);
            this.pnForm.Controls.Add(this.btnGerarRows);
            this.pnForm.Controls.Add(this.btnRecarregar);
            this.pnForm.Controls.Add(this.button1);
            this.pnForm.Controls.Add(this.btnAplicar);
            this.pnForm.Controls.Add(this.btnCarregar);
            this.pnForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnForm.Location = new System.Drawing.Point(5, 30);
            this.pnForm.Name = "pnForm";
            this.pnForm.Size = new System.Drawing.Size(719, 477);
            this.pnForm.TabIndex = 16;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Location = new System.Drawing.Point(2, 3);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(35, 13);
            this.lblTitulo.TabIndex = 17;
            this.lblTitulo.Text = "label3";
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(729, 512);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.pnForm);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmPrincipal";
            this.Padding = new System.Windows.Forms.Padding(5, 30, 5, 5);
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dg)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnDg.ResumeLayout(false);
            this.pnForm.ResumeLayout(false);
            this.pnForm.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dg;
        private System.Windows.Forms.Button btnCarregar;
        private System.Windows.Forms.Button btnAplicar;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem virtualDataGridToolStripMenuItem;
        private System.Windows.Forms.Button btnRecarregar;
        private System.Windows.Forms.Button btnGerarRows;
        private System.Windows.Forms.TextBox txtPesquisar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboCondicaoPesquisa;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkDiferenteDe;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Panel pnDg;
        private System.Windows.Forms.Panel pnDgVScroll;
        private System.Windows.Forms.CheckBox chkModoSelecao;
        private System.Windows.Forms.Panel pnForm;
        private System.Windows.Forms.Label lblTitulo;
    }
}

