using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridDataTable
{
    static class Program
    {
        private static Mutex M;

        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            M = new Mutex(true, Application.ProductName.ToString(), out bool first);
            if ((first))
            {
                if (IsAdministrator() == false)
                {
                    // Reinicia o aplicativo com privilégios administrativos
                    string exeName = Process.GetCurrentProcess().MainModule.FileName;
                    ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                    startInfo.Verb = "runas";
                    Process.Start(startInfo);
                    Application.Exit();
                    return;
                }
                else
                {
                    Application.Run(new frmPrincipal());
                }
            }
            else
            {
                MessageBox.Show("Este aplicativo já está em execução!", "Instância", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
