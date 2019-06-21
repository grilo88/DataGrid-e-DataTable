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
    public partial class frmProcessando : Form
    {
        #region Estático
        public static bool Processando { get; set; } = false;
        public static int ExibirAposMilisegundos { get; set; } = 100;
        public static int FecharMilisegundos { get; set; } = 100;
        public static string TextoProcessando { get; set; } = "Processando, aguarde...";
        public static string TextoLegenda { get; set; } = "";
        
        static Task task;
        public static void TelaProcessando()
        {
            Processando = true;
            if (task != null && !task.IsCompleted) return; // Encerra se tela de processamento já estiver em execução

            task = new Task(() => 
            {
                int tick = Environment.TickCount;
                using (frmProcessando frm = new frmProcessando())
                {
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.TopMost = true;
                    frm.ShowInTaskbar = false;
                    IntPtr ponteiro = frm.Handle;
                    while (Processando && Environment.TickCount - tick > ExibirAposMilisegundos) ; // Enquanto processa, aguarda criar a janela e passar o tempo mínimo para exibição
                    if (Processando) // Continua processando?
                    {
                        frm.Show(); // Exibe a tela de processamento

                    ContinuarTelaProcessamento:
                        do  
                        {
                            if (frm.lblProcessando.Text != TextoProcessando) frm.lblProcessando.Text = TextoProcessando;
                            if (frm.lblLegenda.Text != TextoLegenda) frm.lblLegenda.Text = TextoLegenda;
                            Application.DoEvents();
                        } while (Processando); // Tela aberta enquanto processa...

                        // Pré-fechamento da tela de processamento
                        tick = Environment.TickCount;
                        while (!Processando && Environment.TickCount - tick < FecharMilisegundos) Application.DoEvents(); // Tolera N milésimos antes do fechamento
                        if (Processando) goto ContinuarTelaProcessamento; // Se entrou em um novo processamento, evita o fechamento da tela.
                    }
                }
            });
            task.Start();
        }
        #endregion

        public frmProcessando()
        {
            InitializeComponent();
        }
    }
}
