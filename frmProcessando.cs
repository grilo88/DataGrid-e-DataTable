using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
        public static void Exibir()
        {
            Processando = true;
            if (task != null && !task.IsCompleted) return; // Encerra se tela de processamento já estiver em execução

            task = new Task(() =>
            {
                int tick = Environment.TickCount;
                using (frmProcessando frm = new frmProcessando())
                {
                    //frm.Owner = owner;
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
            DoubleBuffered = true;

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, pnForm, new object[] { true });
        }

        private void FrmProcessando_Load(object sender, EventArgs e)
        {
            Util.DefinirTemaEscuro(pnForm);
        }

        int quant = 10;
        float PosX, PosY;
        float rot = 0F;
        float raio = 0F;
        float tam = 0F;
        float fatorAni = 0F;
        float corR, corG, corB;
        float fatorR = 100, fatorG = 0, fatorB = 200;

        bool sair = false;
        private void PnForm_Paint(object sender, PaintEventArgs e)
        {
            g = pnForm.CreateGraphics();
            bmp = new Bitmap(pnForm.ClientRectangle.Width, pnForm.ClientRectangle.Height, g);

            //Show();
            //Refresh();

            //while (!sair)
            //{
            //    Render();
            //    //pnForm.BackgroundImage = bmp;
            //    //pnForm.Refresh();
            //    //Refresh();
            //    Application.DoEvents();
            //}
        }

        bool pararCrescimento = false;

        Bitmap bmp;

        private void FrmProcessando_FormClosing(object sender, FormClosingEventArgs e)
        {
            sair = true;
        }

        Graphics g;

        long TempoDelta = 0;
        int tick_fps;

        void Render()
        {
            TempoDelta = Environment.TickCount - tick_fps;
            tick_fps = Environment.TickCount;

            g.Clear(pnForm.BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            PosX = ClientRectangle.Width / 2;
            PosY = ClientRectangle.Height / 2;

            if (!pararCrescimento)
            {
                fatorAni += 1 * 0.01F;
                //float raioAnt = raio;
                float novoRaio = raio + (float)Math.Cos(fatorAni);

                //if (novoRaio > raioAnt)
                raio = novoRaio;
                //else
                //{
                //pararCrescimento = true;
                //}
            }

            rot += 0.01F;

            fatorR += 0.01F;
            fatorG += 0.01F;
            fatorB += 0.01F;

            corR += (float)Math.Cos(fatorR);
            corG += (float)Math.Sin(fatorG);
            corB += (float)Math.Cos(fatorB);
            tam = raio / 3;

            float parte = (float)(Math.PI * 2) / quant;
            for (int i = 0; i < quant; i++)
            {
                RectangleF elipse = new RectangleF();
                float x = (float)Math.Sin(i * parte + rot) * raio;
                float y = (float)Math.Cos(i * parte + rot) * raio;

                elipse.Size = new SizeF(tam, tam);
                elipse.Location = new PointF(PosX + x - elipse.Width / 2, PosY + y - elipse.Height / 2);

                corR = Math.Abs(corR);
                corG = Math.Abs(corG);
                corB = Math.Abs(corB);

                g.FillEllipse(new SolidBrush(Color.FromArgb((int)corR, (int)corG, (int)corB)), elipse);
            }
        }

        private void TmrAni_Tick(object sender, EventArgs e)
        {
            
        }
    }
}
