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
    public partial class Form1 : Form
    {
        DataTable dt;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BtnCarregar_Click(sender, e);
        }

        private void BtnCarregar_Click(object sender, EventArgs e)
        {
            DataTable dt = DAL.Carregar("Tabela");
            dg.DataSource = dt;
        }

        private void BtnAplicar_Click(object sender, EventArgs e)
        {
            Task<int> tk = DAL.Aplicar((DataTable)dg.DataSource, "Tabela");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int tick = Environment.TickCount;
            DataTable alt = ((DataTable)dg.DataSource);

            Task<int>[] tarefas = new Task<int>[100];

            for (int i = 0; i < tarefas.Length; i++)
            {
                tarefas[i] = DAL.Aplicar((DataTable)dg.DataSource, "Tabela");
            }

            Task.WaitAll(tarefas);

            MessageBox.Show(
                $"Tarefas executadas com sucesso em {Environment.TickCount - tick}ms.\r\n" +
                $"Registros afetados: {tarefas.Sum(x => x.Result)}");
        }
    }
}
