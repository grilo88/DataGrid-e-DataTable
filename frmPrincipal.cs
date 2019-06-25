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

        /// <summary>
        /// Retorna todos os controles associados ao controle Pai recursivamente
        /// </summary>
        /// <param name="controles"></param>
        /// <returns></returns>
        public static IEnumerable<Control> RecursivoControles(IEnumerable<Control> controles)
        {
            foreach (Control controle in controles)
            {
                yield return controle;

                if (controle.Controls.Count > 0)
                {
                    foreach (Control subitem in RecursivoControles(controle.Controls.Cast<Control>()))
                    {
                        yield return subitem;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Estilo do Form
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.ForeColor = Color.White;

            // Estilo do GroupBox Pesquisa
            groupBox1.ForeColor = this.ForeColor;

            RecursivoControles(this.Controls.Cast<Control>()).ToList().ForEach(ctrl =>
            {
                if (ctrl.BackColor != Color.Transparent) ctrl.BackColor = BackColor;
                ctrl.ForeColor = ForeColor;
            });

            pnDgVScroll.BackColor = Color.SpringGreen;

            // Estilo dos botões
            Controls.OfType<Button>().ToList().ForEach(btn =>
            {
                btn.Paint += (_sender, _e) => // Pinta o botão
                {
                    _e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, 60, 60)), _e.ClipRectangle);
                    _e.Graphics.DrawRectangle(new Pen(Color.WhiteSmoke, 2), _e.ClipRectangle);

                    SizeF size =_e.Graphics.MeasureString(btn.Text, btn.Font);
                    RectangleF rectText = new RectangleF(new PointF(), size);

                    rectText.Location = new PointF(
                        (_e.ClipRectangle.Width / 2 - size.Width / 2) + 1,
                        (_e.ClipRectangle.Height / 2 - size.Height / 2) + 1);

                    _e.Graphics.DrawString(
                        btn.Text, btn.Font, new SolidBrush(Color.White), 
                        rectText, StringFormat.GenericDefault);
                };
            });

            // Estilo do DataGridView
            pnDg.BackColor = Color.Green;
            dg.BorderStyle = BorderStyle.None;
            dg.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dg.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dg.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dg.EnableHeadersVisualStyles = false; // Importante
            dg.ColumnHeadersHeight = 60;
            dg.ColumnHeadersDefaultCellStyle.BackColor = Color.Green;
            dg.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dg.RowHeadersDefaultCellStyle.BackColor = Color.Green;
            dg.RowHeadersDefaultCellStyle.ForeColor = Color.White;
            dg.ForeColor = Color.DarkBlue;
            dg.GridColor = BackColor;
            dg.BackgroundColor = BackColor;
            dg.DefaultCellStyle.SelectionBackColor = Color.Gray;
            dg.DefaultCellStyle.SelectionForeColor = Color.White;
            dg.ScrollBars = ScrollBars.None;
            dg.Scroll += Dg_Scroll;

            // Estilo do Barra de Status
            statusStrip1.BackColor = Color.Green;

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

        private void Dg_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
        }

        private void Dg_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //((DataGridView)sender).Rows[e.RowIndex].
        }

        private void Dg_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            bool impar = e.RowIndex % 2 > 0;

            Color Cor = BackColor;
            Color ForeColor = Color.Gray;
            int diff = 10;

            if (impar)
            {
                ((DataGridView)sender).Rows[e.RowIndex].DefaultCellStyle.BackColor = Cor;
                ((DataGridView)sender).Rows[e.RowIndex].DefaultCellStyle.ForeColor = ForeColor;
            }
            else
            {
                ((DataGridView)sender).Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(Cor.R - diff, Cor.G - diff, Cor.B - diff);
                ((DataGridView)sender).Rows[e.RowIndex].DefaultCellStyle.ForeColor = ForeColor;
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
