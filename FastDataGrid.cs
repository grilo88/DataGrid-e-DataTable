using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridDataTable
{
    public class FastDataGrid
    {
        DataGridView dg;
        BindingSource bs = new BindingSource();

        public DataTable DataSource { get => (DataTable)bs.DataSource; set => DefinirFonteDeDados(value); }

        public FastDataGrid(DataGridView dataGridView)
        {
            dg = dataGridView;
            dg.VirtualMode = true;
            dg.AllowUserToAddRows = true;

            dg.NewRowNeeded += Dg_NewRowNeeded;
            dg.CellValueNeeded += Dg_CellValueNeeded;
            dg.CellValuePushed += Dg_CellValuePushed;
            dg.RowsAdded += Dg_RowsAdded;
            bs.ListChanged += Bs_ListChanged;
            bs.CurrentItemChanged += Bs_CurrentItemChanged;
            bs.CurrentChanged += Bs_CurrentChanged;
            bs.DataSourceChanged += Bs_DataSourceChanged;
            bs.PositionChanged += Bs_PositionChanged;
        }

        private void Dg_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            
        }

        void DefinirFonteDeDados(DataTable dt)
        {
            if (dt == null)
            {
                bs.DataSource = null;
                dg.Rows.Clear();
                dg.Columns.Clear();
            }
            else
            {
                bs.DataSource = dt;
                
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    DataGridViewColumn found = dg.Columns.Cast<DataGridViewColumn>().Where(x => x.Name == dt.Columns[c].ColumnName).FirstOrDefault();
                    if (found != null) continue;

                    DataGridViewColumn dgCol;
                    if (dt.Columns[c].DataType == typeof(bool))
                        dgCol = new DataGridViewCheckBoxColumn();
                    else if (dt.Columns[c].DataType.IsEnum)
                        dgCol = new DataGridViewComboBoxColumn();
                    else
                        dgCol = new DataGridViewTextBoxColumn();

                    dgCol.ValueType = dt.Columns[c].DataType;
                    dgCol.Name = dt.Columns[c].ColumnName;
                    dgCol.HeaderText = dt.Columns[c].Caption;
                    dgCol.DataPropertyName = dt.Columns[c].ColumnName;
                    dg.Columns.Add(dgCol);
                }

                if (((DataTable)bs.DataSource).Rows.Count > 0)
                {
                    int count = ((DataTable)bs.DataSource).Rows.Count;
                    if (dg.AllowUserToAddRows) count++; // Adiciona a new row no contador

                    dg.RowCount = count;
                }
            }
        }

        private void Bs_PositionChanged(object sender, EventArgs e)
        {
        }

        private void Bs_DataSourceChanged(object sender, EventArgs e)
        {
        }

        private void Bs_CurrentChanged(object sender, EventArgs e)
        {
        }

        private void Bs_CurrentItemChanged(object sender, EventArgs e)
        {
        }

        private void Bs_ListChanged(object sender, ListChangedEventArgs e)
        {
            
        }

        private void Dg_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            // Atualização da célula
            if (bs.DataSource != null)
            {
                e.Value = ((DataTable)bs.DataSource).Rows[e.RowIndex][e.ColumnIndex];
            }
        }

        private void Dg_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            // Alteração da Célula
            ((DataTable)bs.DataSource).Rows[e.RowIndex][e.ColumnIndex] = e.Value;
        }

        private void Dg_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            // Nova row
            if (e.Row.Index > ((DataTable)bs.DataSource).Rows.Count - 1)
            {
                DataRow newRow = ((DataTable)bs.DataSource).NewRow();
                ((DataTable)bs.DataSource).Rows.Add(newRow);
            }
        }
    }
}
