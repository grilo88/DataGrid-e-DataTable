using System;
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
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BtnCarregar_Click(sender, e);
        }

        private async void BtnCarregar_Click(object sender, EventArgs e)
        {
            if (dg.DataSource == null)
            {
                dg.DataSource = new DataTable("Tabela");
            }

            dg.DataSource = await DAL.Carregar(DAL.ObterDataTable(dg.DataSource));
        }

        private async void BtnAplicar_Click(object sender, EventArgs e)
        {
            if (DAL.ObterDataTable(dg.DataSource).GetChanges() == null)
            {
                MessageBox.Show(this, "Não há alterações a serem aplicadas!", "Aplicar");
                return;
            }

            await DAL.Aplicar(DAL.ObterDataTable(dg.DataSource));
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int tick = Environment.TickCount;
            DataTable alt = DAL.ObterDataTable(dg.DataSource);

            Task<int>[] tarefas = new Task<int>[100];

            for (int i = 0; i < tarefas.Length; i++)
            {
                tarefas[i] = DAL.Aplicar(DAL.ObterDataTable(dg.DataSource));
            }

            Task.WaitAll(tarefas);

            MessageBox.Show(
                $"Tarefas executadas com sucesso em {Environment.TickCount - tick}ms.\r\n" +
                $"Registros afetados: {tarefas.Sum(x => x.Result)}");
        }

        private void VirtualDataGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmVirtualDataGrid frm = new frmVirtualDataGrid();
            frm.ShowDialog();
        }

        private void BtnGerarRows_Click(object sender, EventArgs e)
        {
            dg.DataSource = DAL.GerarRows(DAL.ObterDataTable(dg.DataSource), 1000, "col1");
        }

        private async void BtnRecarregar_Click(object sender, EventArgs e)
        {
            dg.DataSource = null;
            dg.DataSource = new DataTable("Tabela");
            dg.DataSource = await DAL.Carregar(DAL.ObterDataTable(dg.DataSource));
        }

        private void TxtPesquisar_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = DAL.ObterDataTable(dg.DataSource);
            dg.DataSource = DAL.Pesquisar(dt, txtPesquisar.Text, dt.Columns[0], dt.Columns[1]);
        }
    }
}
