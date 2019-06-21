﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridDataTable
{
    public partial class frmVirtualDataGrid : Form
    {
        FastDataGrid vdg;

        public frmVirtualDataGrid()
        {
            InitializeComponent();
            vdg = new FastDataGrid(dg);
        }

        private void FrmVirtualDataGrid_Load(object sender, EventArgs e)
        {
            
        }

        private async void BtnCarregar_Click(object sender, EventArgs e)
        {
            if (vdg.DataSource == null)
            {
                vdg.DataSource = new DataTable("Tabela");
            }

            vdg.DataSource = await DAL.Carregar(vdg.DataSource);
        }

        private async void BtnRecarregar_Click(object sender, EventArgs e)
        {
            vdg.DataSource = null;
            vdg.DataSource = new DataTable("Tabela");
            vdg.DataSource = await DAL.Carregar(vdg.DataSource);
        }
    }
}
