using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridDataTable
{
    public class Util
    {
        /// <summary>
        /// Retorna todos os controles associados ao controle Pai
        /// </summary>
        /// <param name="controles"></param>
        /// <returns></returns>
        public static IEnumerable<Control> RecursivoControles(IEnumerable<Control> controles)
        {
            foreach (Control controle in controles)
            {
                yield return controle;
                foreach (Control subitem in RecursivoControles(controle.Controls.Cast<Control>()))
                {
                    yield return subitem;
                }
            }
        }

        public static Color BackColor = Color.FromArgb(40, 40, 40);
        public static Color ForeColor = Color.White;

        public static Form ObterForm(Control control)
        {
            Control frm = control;

            while (frm.Parent != null)
            {
                frm = frm.Parent;
            }

            return frm is Form ? (Form)frm : null;
        }

        public static void DefinirTemaEscuro(Control control)
        {
            Form frm = ObterForm(control);
            frm.BackColor = Color.Green;

            Label lblTitulo = frm.Controls.OfType<Label>().Where(x => x.Name == "lblTitulo").FirstOrDefault();
            if (lblTitulo != null) lblTitulo.Text = frm.Text;

            frm.TextChanged += (sender, e) => {
                if (lblTitulo != null) frm.Text = lblTitulo.Text;
            };

            // O controle é um pnForm?
            if (control is Panel && control.Parent == frm && control.Name == "pnForm")
            {

            }

            // Estilo do Form
            control.BackColor = BackColor;
            control.ForeColor = ForeColor;

            Util.RecursivoControles(control.Controls.Cast<Control>()).ToList().ForEach(ctrl =>
            {
                if (ctrl.BackColor != Color.Transparent) ctrl.BackColor = control.BackColor;
                ctrl.ForeColor = control.ForeColor;

                if (ctrl is DataGridView)
                {
                    DataGridView dg = (DataGridView)ctrl;
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
                    dg.GridColor = control.BackColor;
                    dg.BackgroundColor = control.BackColor;
                    dg.DefaultCellStyle.SelectionBackColor = Color.Gray;
                    dg.DefaultCellStyle.SelectionForeColor = Color.White;
                    dg.RowPrePaint += Dg_RowPrePaint;
                }
                else if (ctrl is StatusStrip)
                {
                    StatusStrip st = (StatusStrip)ctrl;
                    st.BackColor = BackColor;
                    st.ForeColor = ForeColor;
                }
            });

            // Estilo dos botões
            control.Controls.OfType<Button>().ToList().ForEach(btn =>
            {
                btn.Paint += (_sender, _e) => // Pinta o botão
                {
                    _e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, 60, 60)), _e.ClipRectangle);
                    _e.Graphics.DrawRectangle(new Pen(Color.WhiteSmoke, 2), _e.ClipRectangle);

                    SizeF size = _e.Graphics.MeasureString(btn.Text, btn.Font);
                    RectangleF rectText = new RectangleF(new PointF(), size);

                    rectText.Location = new PointF(
                        (_e.ClipRectangle.Width / 2 - size.Width / 2) + 1,
                        (_e.ClipRectangle.Height / 2 - size.Height / 2) + 1);

                    _e.Graphics.DrawString(
                        btn.Text, btn.Font, new SolidBrush(Color.White),
                        rectText, StringFormat.GenericDefault);
                };
            });
        }

        private static void Dg_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
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
    }
}
