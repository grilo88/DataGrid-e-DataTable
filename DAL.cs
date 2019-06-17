using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridDataTable
{

    public enum Banco
    {
        MySql = 1,
        MsSql = 2
    }

    public static class DAL
    {
        public static Banco Banco { get; private set; } = Banco.MySql;
        static string strConexao { get; set; } 

        // Construtor Estático
        static DAL()
        {
            switch (Banco)
            {
                case Banco.MySql:
                    MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();

                    // Conexão
                    sb.ConnectionTimeout = 60;
                    sb.Server = "localhost";
                    sb.UserID = "root";
                    sb.Password = "123456";
                    sb.Database = "Teste";

                    // Segurança
                    sb.SslMode = MySqlSslMode.None;
                    sb.AllowPublicKeyRetrieval = true;

                    // Pool
                    sb.Pooling = true;
                    sb.MinimumPoolSize = 5;
                    sb.MaximumPoolSize = 160;
                    sb.ConnectionLifeTime = 60;

                    strConexao = sb.ToString();
                    break;
                case Banco.MsSql:
                    strConexao = "Server=localhost,3741;Database=Teste;User Id=sa;Password=d120588$788455;Pooling=true;Min Pool Size=5;Max Pool Size=160;Connect Timeout=60;Connection Lifetime=60;";
                    break;
                default:
                    throw new Exception(nameof(Banco));
            }
        }

        public static Task OpenAsync(IDbConnection con)
        {
            switch (Banco)
            {
                case Banco.MySql:
                    return ((MySqlConnection)con).OpenAsync();
                case Banco.MsSql:
                    return ((SqlConnection)con).OpenAsync();
                default:
                    throw new Exception(nameof(Banco));
            }
        }

        public static IDbConnection CrossConnection(string conexao)
        {
            switch (Banco)
            {
                case Banco.MySql:
                    return new MySqlConnection(conexao);
                case Banco.MsSql:
                    return new SqlConnection(conexao);
                default:
                    throw new Exception(nameof(Banco));
            }
        }

        public static IDbCommand CrossCommand(IDbConnection con)
        {
            switch (Banco)
            {
                case Banco.MySql:
                    return new MySqlCommand("", (MySqlConnection)con);
                case Banco.MsSql:
                    return new SqlCommand("", (SqlConnection)con);
                default:
                    throw new Exception(nameof(Banco));
            }
        }

        public static DataTable Carregar(string tabela)
        {
            DataTable dt = new DataTable();
            try
            {
                using (IDbConnection con = CrossConnection(strConexao))
                using (IDbCommand com = CrossCommand(con))
                {
                    con.Open();

                    string tmpTabela = tabela;
                    switch (Banco)
                    {
                        case Banco.MySql:
                            tmpTabela = $"{tabela}";
                            break;
                        case Banco.MsSql:
                            tmpTabela = $"[{tabela}]";
                            break;
                        default:
                            break;
                    }

                    com.CommandText = $"SELECT * FROM {tabela} LIMIT 10";
                    using (IDataReader dr = com.ExecuteReader())
                    {
                        dt.TableName = tabela;
                        dt.BeginLoadData();
                        dt.Load(dr);
                        dt.EndLoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
            return dt;
        }

        //static string BancoDeDados(IDbConnection con, bool ponto = true)
        //{
        //    string bd = "";
        //    switch (Banco)
        //    {
        //        case Banco.MySql:
        //            bd = PalavraBanco(con.Database) + (ponto ? "." : "");
        //            break;
        //        case Banco.MsSql:
        //            bd = PalavraBanco(con.Database) + (ponto ? ".." : "");
        //            break;
        //        default:
        //            break;
        //    }
        //    return bd;
        //}

        static string PalavraBanco(string palavra)
        {
            switch (Banco)
            {
                case Banco.MySql:
                    palavra = $"`{palavra}`";
                    break;
                case Banco.MsSql:
                    palavra = $"[{palavra}]";
                    break;
                default:
                    break;
            }
            return palavra;
        }

        static object ValorBanco(object valor)
        {
            switch (Banco)
            {
                case Banco.MySql:
                    if (valor is bool) valor = (bool)valor ? 1 : 0;
                    else if (valor is string)
                    {
                        valor = MySqlHelper.EscapeString((string)valor);
                    }
                    break;
                case Banco.MsSql:
                    valor = $"'{valor}'";
                    break;
                default:
                    throw new Exception(nameof(Banco));
            }

            return valor;
        }

        public static async Task<int> Aplicar(DataTable dt)
        {
            int quantLote = 150;

            int afetados_add = 0,
                afetados_del = 0,
                afetados_alt = 0;

            try
            {
                DataTable add = dt.GetChanges(DataRowState.Added);
                DataTable del = dt.GetChanges(DataRowState.Deleted);
                DataTable alt = dt.GetChanges(DataRowState.Modified);

                using (IDbConnection con = CrossConnection(strConexao))
                using (IDbCommand com = CrossCommand(con))
                {
                    await OpenAsync(con);
                    com.Transaction = con.BeginTransaction(IsolationLevel.RepeatableRead);

                    DataColumn id = dt.Columns.Cast<DataColumn>().Where(x => x.AutoIncrement).FirstOrDefault();
                    if (id == null)
                        throw new Exception("É necessário criar uma coluna Auto Incremento");

                    int idx_id = dt.Columns.IndexOf(id);

                    if (add != null)
                    {
                        DataColumn[] cols = add.Columns.Cast<DataColumn>()
                            .Where(x => !x.AutoIncrement) // Ignora colunas Auto Incremento
                            .ToArray();

                        string sColunas = string.Join(",", cols.Select(x => PalavraBanco(x.ColumnName)));

                        string values = "", reg = "";
                        string insertinto = $"INSERT INTO {PalavraBanco(dt.TableName)}(" + sColunas + ")VALUES";

                        for (int r = 0, lote = 1; r < add.Rows.Count; r++, lote++)
                        {
                            #region Monta Registro
                            for (int c = 0; c < cols.Count(); c++)
                            {
                                if (reg != "") reg += ",";
                                reg += $"{ValorBanco(add.Rows[r][add.Columns.IndexOf(cols[c])])}";
                            }
                            #endregion

                            #region Monta lista Values
                            if (values != "") values += ",";
                            values += "(" + reg + ")";
                            reg = "";
                            #endregion

                            #region Gravação
                            if (lote == quantLote ||
                                r == add.Rows.Count - 1 /* Último Row? */)
                            {
                                com.CommandText = insertinto + values;
                                afetados_add += com.ExecuteNonQuery();
                                lote = 0;
                            }
                            #endregion
                        }
                    }
                    if (del != null)
                    {
                        for (int r = 0, lote = 1; r < del.Rows.Count; r++, lote++)
                        {
                            object id_val = del.Rows[r][idx_id, DataRowVersion.Original];

                            com.CommandText +=
                                $"DELETE FROM {PalavraBanco(dt.TableName)} " +
                                $"WHERE {PalavraBanco(id.ColumnName)}={ValorBanco(id_val)};\r\n";

                            if (lote == quantLote || r == del.Rows.Count - 1)
                            {
                                afetados_del += com.ExecuteNonQuery();
                                com.CommandText = "";
                                lote = 0;
                            }
                        }
                    }
                    if (alt != null)
                    {
                        DataColumn[] cols = alt.Columns.Cast<DataColumn>()
                            .Where(x => !x.AutoIncrement) // Ignora colunas Auto Incremento
                            .ToArray();

                        for (int r = 0, lote = 1; r < alt.Rows.Count; r++, lote++)
                        {
                            object id_val = alt.Rows[r][idx_id];

                            string update = "UPDATE ";
                            string finalUpdate = "";

                            switch (Banco)
                            {
                                case Banco.MySql:
                                    finalUpdate = " LIMIT 1";
                                    break;
                                case Banco.MsSql:
                                    update += "TOP(1) ";
                                    break;
                                default:
                                    throw new Exception(nameof(Banco));
                            }

                            update += $"{PalavraBanco(dt.TableName)} SET " +
                                string.Join(",", cols.Select(
                                    col => $"{PalavraBanco(col.ColumnName)}={ValorBanco(alt.Rows[r][alt.Columns.IndexOf(col)])}")) +
                                $" WHERE {PalavraBanco(id.ColumnName)}={ValorBanco(id_val)}";

                            com.CommandText += update + finalUpdate + ";\r\n";

                            if (lote == quantLote || r == alt.Rows.Count - 1)
                            {
                                afetados_alt += com.ExecuteNonQuery();
                                com.CommandText = "";
                                lote = 0;
                            }
                        }
                    }

                    com.Transaction.Commit();
                }
                dt.AcceptChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
                dt.RejectChanges();
            }

            return afetados_add + afetados_del + afetados_alt;
        }
    }
}
