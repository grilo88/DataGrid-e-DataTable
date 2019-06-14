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

        }

        private void BtnCarregar_Click(object sender, EventArgs e)
        {
            DataTable dt = DAL.Carregar();
            dg.DataSource = dt;
        }

        private void BtnAplicar_Click(object sender, EventArgs e)
        {
            Task<int> tk = DAL.Aplicar((DataTable)dg.DataSource, "Tabela");
        }
    }
}
