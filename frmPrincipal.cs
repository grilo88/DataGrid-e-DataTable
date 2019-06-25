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
        DAL dal;
        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void Dal_ResultadoPesquisa(object sender, PesquisaEventArgs e)
        {
            dg.DataSource = e.Resultado;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dal = new DAL(this);
            dal.ResultadoPesquisa += Dal_ResultadoPesquisa;

            cboCondicaoPesquisa.DataSource = Enum.GetValues(typeof(CondicaoPesquisa));
            cboCondicaoPesquisa.SelectedIndex = 5;
            BtnCarregar_Click(sender, e);
        }

        private async void BtnCarregar_Click(object sender, EventArgs e)
        {
            if (dg.DataSource == null)
            {
                dg.DataSource = new DataTable("Tabela");
            }

            dg.DataSource = await dal.Carregar(DAL.ObterDataTable(dg.DataSource));
        }

        private async void BtnAplicar_Click(object sender, EventArgs e)
        {
            if (DAL.ObterDataTable(dg.DataSource).GetChanges() == null)
            {
                MessageBox.Show(this, "Não há alterações a serem aplicadas!", "Aplicar");
                return;
            }

            await dal.Aplicar(DAL.ObterDataTable(dg.DataSource));
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int tick = Environment.TickCount;
            DataTable alt = DAL.ObterDataTable(dg.DataSource);

            Task<int>[] tarefas = new Task<int>[100];

            for (int i = 0; i < tarefas.Length; i++)
            {
                tarefas[i] = dal.Aplicar(DAL.ObterDataTable(dg.DataSource));
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
            dg.DataSource = DAL.GerarRows(dg, 50000, "col1");
        }

        private async void BtnRecarregar_Click(object sender, EventArgs e)
        {
            dg.DataSource = null;
            dg.DataSource = new DataTable("Tabela");
            dg.DataSource = await dal.Carregar(DAL.ObterDataTable(dg.DataSource));
        }

        private void TxtPesquisar_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = DAL.ObterDataTable(dg.DataSource);

            dal.Pesquisar(
                dt, txtPesquisar.Text,
                chkDiferenteDe.Checked,
                (CondicaoPesquisa)cboCondicaoPesquisa.SelectedValue, 
                dt.Columns[0], dt.Columns[1], dt.Columns[2]);
        }

        private void CboCondicaoPesquisa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dg.DataSource != null) TxtPesquisar_TextChanged(sender, e);
        }

        private void ChkDiferenteDe_CheckedChanged(object sender, EventArgs e)
        {
            if (dg.DataSource != null) TxtPesquisar_TextChanged(sender, e);
        }

        private void FrmPrincipal_Activated(object sender, EventArgs e)
        {
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            if (dg.DataSource != null)
            {
                DataTable dt = DAL.ObterDataTable(dg.DataSource);
                dg.DataSource = null;
                dt.RejectChanges();
                dg.DataSource = dt;
            }
        }
    }
}
