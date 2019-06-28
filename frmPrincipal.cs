using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridDataTable
{
    public partial class frmPrincipal : Form
    {
        DAL dal = new DAL();

        //[DllImport("user32.dll")]
        //static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

        //[DllImport("User32.dll")]
        //private static extern IntPtr GetWindowDC(IntPtr hWnd);

        public frmPrincipal()
        {
            InitializeComponent();
        }

        //protected override void WndProc(ref Message m)
        //{
        //    base.WndProc(ref m);
        //    const int WM_NCPAINT = 0x85;
        //    if (m.Msg == WM_NCPAINT)
        //    {
        //        IntPtr hdc = GetWindowDC(m.HWnd);
        //        if ((int)hdc != 0)
        //        {
        //            Graphics g = Graphics.FromHdc(hdc);
        //            g.FillRectangle(Brushes.Green, new Rectangle(0, 0, 4800, 30));
        //            g.Flush();
        //            ReleaseDC(m.HWnd, hdc);
        //        }
        //    }
        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            Util.DefinirTemaEscuro(pnForm);

            pnDgVScroll.BackColor = Color.SpringGreen;

            // Barra de Rolagem personalizada
            pnDg.BackColor = Color.Green;
            dg.ScrollBars = ScrollBars.None;
            dg.Scroll += Dg_Scroll;

            // Resultado da Pesquisa
            dal.ResultadoPesquisa += (_sender, _e) => 
            {
                if (chkModoSelecao.Checked)
                {
                    DataTable dt = DAL.ObterDataTable(_e.Resultado);
                    dg.DataSource = dt;
                    dg.Invoke(new MethodInvoker(() => dg.DataSource = dt));

                    //if (_e.Resultado is DataView)
                    //{
                    //    List<DataRow> s = ((DataView)_e.Resultado).ToTable().Rows.Cast<DataRow>().ToList();

                    //    foreach (DataRow item in s)
                    //    {
                    //        dg.Rows.IndexOf(s)
                    //    }

                    //    ((DataView)_e.Resultado).ToTable().Rows.Cast<DataRow>().ToList()
                    //    .ForEach(r =>
                    //    {
                    //        var found = dg.Rows.Cast<DataGridViewRow>().Where(
                    //            vRow => vRow.DataBoundItem != null && 
                    //            ((DataRowView)vRow.DataBoundItem).Row == r).FirstOrDefault();

                    //        if (found != null)
                    //        {
                    //            found.Cells[0].Selected = true;
                    //        }
                    //    });
                    //}
                }
                else
                {

                    dg.Invoke(new MethodInvoker(() =>
                    {
                        //dg.DataSource = null;
                        dg.DataSource = _e.Resultado;
                    }));
                }
            };

            // Inicializa a Combo Condição
            cboCondicaoPesquisa.DataSource = Enum.GetValues(typeof(CondicaoPesquisa));
            cboCondicaoPesquisa.SelectedIndex = 5;

            Show();
            Refresh();

            // Carrega os dados para a Grid
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

        MouseEventArgs mouseDown;
        private void PnDgVScroll_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = e;
        }

        private void PnDgVScroll_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int novoTop = pnDgVScroll.Top;
                novoTop += e.Y - mouseDown.Y;

                // Calcula o Topo mínimo e máximo da translação vertical
                int minTop = pnDg.Padding.Top + dg.ColumnHeadersHeight;
                int maxBottom = pnDg.Height - pnDg.Padding.Bottom;

                // Define limites de translação vertical para a barra de rolagem
                if (novoTop < minTop) novoTop = pnDgVScroll.Top;
                if (novoTop + pnDgVScroll.Height > maxBottom) novoTop = pnDgVScroll.Top;

                pnDgVScroll.Top = novoTop;
            }

            //dg.FirstDisplayedScrollingRowIndex = dg.RowCount;
        }

        private void Dg_Scroll(object sender, ScrollEventArgs e)
        {
            int minTop = pnDg.Padding.Top + dg.ColumnHeadersHeight;
            int maxBottom = pnDg.Height - pnDg.Padding.Bottom;

            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                int height = (pnDg.Height - pnDgVScroll.Height);
                //int diff = Math.Abs(e.NewValue - e.OldValue);
                pnDgVScroll.Top = ((int)((double)height / dg.RowCount) * e.NewValue) + minTop;
            }
        }

        private void PnDg_MouseUp(object sender, MouseEventArgs e)
        {

        }
    }
}
