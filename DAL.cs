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

        /// <summary>
        /// Ao usar valores padrão, apenas as colunas de interesse serão selecionadas na gravação 
        /// do banco de dados melhorando a performance do sistema. No caso do insert, campos preenchidos. 
        /// No caso do update, campos alterados.
        /// </summary>
        public static bool UsarValoresPadrao { get; set; } = true;

        /// <summary>
        /// Simplifica o comando Delete reduzindo significamente o consumo de tráfego na conexão
        /// </summary>
        public static bool UsarDeleteWhereIn { get; set; } = true;

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
                    throw new NotImplementedException(nameof(Banco));
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
                    throw new NotImplementedException(nameof(Banco));
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
                    throw new NotImplementedException(nameof(Banco));
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
                    throw new NotImplementedException(nameof(Banco));
            }
        }

        public static DataTable Schema(IDbCommand com)
        {
            if (com.CommandText == "")
                throw new Exception("Deve ser chamado após CommandText for preenchido.");

            DataTable schema = new DataTable();
            using (IDataReader dr = com.ExecuteReader(CommandBehavior.SchemaOnly))
            {
                schema = dr.GetSchemaTable();
            }
            return schema;
        }

        public static void Carregar(ref DataTable dt) => Carregar(ref dt, null, -1);
        public static void Carregar(ref DataTable dt, int limite) => Carregar(ref dt, null, limite);

        public static void Carregar(ref DataTable dt, string[] colunas, int limite)
        {
            if (dt == null) throw new Exception("Instancie o DataTable");
            if (dt.TableName == "") throw new Exception("Informe o nome da tabela ao instanciar o DataTable");
            if (dt.PrimaryKey.Length == 0 && dt.Rows.Count > 0) throw new Exception("É necessário possuir uma coluna chave primária com auto-incremento no DataTable para realizar a união (atualização) dos registros neste recarregamento de dados. Obtenha o modelo através do método estático DAL.Carregar() sobre a definição de chave-primária.");

            try
            {
                using (IDbConnection con = CrossConnection(strConexao))
                using (IDbCommand com = CrossCommand(con))
                {
                    con.Open();

                    string select = "SELECT ";
                    string cols = colunas == null || colunas.Length == 0 ? "*" : string.Join(",", colunas.Select(col => PalavraBanco(col)));
                    string finalSelect = "";
                    switch (Banco)
                    {
                        case Banco.MySql:
                            if (limite > -1)
                                finalSelect = $" LIMIT {limite}";
                            break;
                        case Banco.MsSql:
                            if (limite > -1)
                                select += $"TOP({limite}) ";
                            break;
                        default:
                            throw new NotImplementedException(nameof(Banco));
                    }

                    select += $"{cols} FROM {PalavraBanco(dt.TableName)}";
                    com.CommandText = select + finalSelect;

                    if (Banco == Banco.MsSql)
                    {
                        // Previne erro no DataGrid quando alterar dados da coluna AutoIncremento 
                        // durante o union do recarregamento dos dados
                        for (int i = 0; i < dt.PrimaryKey.Length; i++)
                        {
                            dt.PrimaryKey[i].ReadOnly = false; 
                        }
                    } // Se por acaso ocorrer também com outros bancos, este if deve ser removido. Não o conteúdo do if.

                    using (IDataReader dr = com.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        dt.BeginLoadData();
                        dt.Load(dr);
                        dt.EndLoadData();
                    }

                    if (dt.PrimaryKey.Length == 0)
                    {
                        DataColumn id = dt.Columns.Cast<DataColumn>().Where(x => x.AutoIncrement).FirstOrDefault();
                        if (id != null)
                        {
                            // Define a chave primária através da coluna Auto Incremento
                            // Isto é necessário para realizar o union durante o recarregamento dos dados 
                            dt.PrimaryKey = new DataColumn[1] { id };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
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
        //            throw new NotImplementedException(nameof(Banco));
        //    }
        //    return bd;
        //}

        /// <summary>
        /// Previne erros de comando sql quando uma palavra entra em conflito com alguma palavra-chave do banco de dados.
        /// </summary>
        /// <param name="palavra"></param>
        /// <returns></returns>
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
                    throw new NotImplementedException(nameof(Banco));
            }
            return palavra;
        }

        /// <summary>
        /// Converte o valor para o formato aceito pelo interpretador de comandos sql do banco de dados.
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        static object ValorBanco(object valor)
        {
            switch (Banco)
            {
                case Banco.MySql:
                    if (valor is DBNull || valor is null)
                    {
                        valor = "NULL";
                    }
                    else if (valor is bool)
                    {
                        valor = (bool)valor ? 1 : 0;
                    }
                    else if (valor is string)
                    {
                        valor = $"'{MySqlHelper.EscapeString((string)valor)}'";
                    }
                    break;
                case Banco.MsSql:
                    valor = $"'{valor}'";
                    break;
                default:
                    throw new NotImplementedException(nameof(Banco));
            }

            return valor;
        }

        /// <summary>
        /// Aplica as alterações ao banco de dados considerando operações de 
        /// inclusão, atualização e deleção de registros em um escopo de transação.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static async Task<int> Aplicar(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
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
                        sb.Clear();
                        string insertinto = ""; 

                        DataColumn[] sel_cols = null;
                        if (!UsarValoresPadrao)
                        {
                            // Seleciona todas as colunas na inserção de valores
                            sel_cols = add.Columns.Cast<DataColumn>()
                             .Where(x => !x.AutoIncrement) // Ignora colunas Auto Incremento
                             .ToArray();

                            string sColunas = string.Join(",", sel_cols.Select(x => PalavraBanco(x.ColumnName)));
                            insertinto = $"INSERT INTO {PalavraBanco(dt.TableName)}({sColunas})VALUES";
                        }

                        string values = "", reg = "";
                        for (int r = 0, lote = 1; r < add.Rows.Count; r++, lote++)
                        {
                            #region Monta valores da row atual
                            if (UsarValoresPadrao)
                            {
                                DataColumn[] tmp_cols = SelecionarColunas(add, r, true);
                                if (sel_cols == null || !tmp_cols.SequenceEqual(sel_cols)) // Colunas desta row são diferentes das anteriores considerando a ordem e o valor?
                                {
                                    sel_cols = tmp_cols;

                                    if (values != "") // Adiciona insert se possuir valores
                                    {
                                        sb.AppendLine(insertinto + values + ";");
                                        values = "";
                                    }

                                    string sColunas = string.Join(",", sel_cols.Select(x => PalavraBanco(x.ColumnName)));
                                    insertinto = $"INSERT INTO {PalavraBanco(dt.TableName)}({sColunas})VALUES";
                                }
                            }

                            reg += string.Join(",", sel_cols.Select(col => $"{ValorBanco(add.Rows[r][add.Columns.IndexOf(col)])}"));
                            #endregion

                            #region Monta lista de Values
                            if (values != "") values += ",";
                            values += "(" + reg + ")";
                            reg = "";
                            #endregion

                            #region Gravação
                            if (lote == quantLote ||
                                r == add.Rows.Count - 1 /* Último Row? */)
                            {
                                if (values != "")
                                {
                                    sb.AppendLine(insertinto + values + ";");
                                }

                                com.CommandText = sb.ToString();
                                afetados_add += com.ExecuteNonQuery();
                                lote = 0;
                            }
                            #endregion
                        }
                    }
                    if (del != null)
                    {
                        sb.Clear();
                        if (UsarDeleteWhereIn)
                        {
                            for (int r = 0, lote = 1; r < del.Rows.Count; r++, lote++)
                            {
                                object id_val = del.Rows[r][idx_id, DataRowVersion.Original];

                                sb.Append((sb.Length > 0 ? ",": "") + ValorBanco(id_val));

                                if (lote == quantLote || r == del.Rows.Count - 1)
                                {
                                    com.CommandText = $"DELETE FROM {PalavraBanco(dt.TableName)} WHERE {PalavraBanco(id.ColumnName)} IN ({sb.ToString()})" ;
                                    afetados_del += com.ExecuteNonQuery();
                                    lote = 0;
                                }
                            }
                        }
                        else
                        {
                            for (int r = 0, lote = 1; r < del.Rows.Count; r++, lote++)
                            {
                                object id_val = del.Rows[r][idx_id, DataRowVersion.Original];

                                string delete = "DELETE ";
                                string finaldelete = "";

                                switch (Banco)
                                {
                                    case Banco.MySql:
                                        finaldelete = " LIMIT 1";
                                        break;
                                    case Banco.MsSql:
                                        delete += "TOP(1) ";
                                        break;
                                    default:
                                        throw new NotImplementedException(nameof(Banco));
                                }

                                delete +=
                                    $"FROM {PalavraBanco(dt.TableName)} " +
                                    $"WHERE {PalavraBanco(id.ColumnName)}={ValorBanco(id_val)}";

                                sb.AppendLine(delete + finaldelete + ";");

                                if (lote == quantLote || r == del.Rows.Count - 1)
                                {
                                    com.CommandText = sb.ToString();
                                    afetados_del += com.ExecuteNonQuery();
                                    lote = 0;
                                }
                            }
                        }
                    }
                    if (alt != null)
                    {
                        sb.Clear();
                        for (int r = 0, lote = 1; r < alt.Rows.Count; r++, lote++)
                        {
                            object id_val = alt.Rows[r][idx_id];

                            string update = "UPDATE ";
                            string finalUpdate = "";

                            switch (Banco)
                            {
                                case Banco.MySql:
                                    finalUpdate = " LIMIT 1"; // Garante aumento de desempenho
                                    break;
                                case Banco.MsSql:
                                    update += "TOP(1) "; // Garante aumento de desempenho
                                    break;
                                default:
                                    throw new NotImplementedException(nameof(Banco));
                            }

                            DataColumn[] cols = SelecionarColunas(alt, r, false);

                            update += $"{PalavraBanco(dt.TableName)} " +
                                "SET " + string.Join(",", cols.Select(
                                    col => $"{PalavraBanco(col.ColumnName)}={ValorBanco(alt.Rows[r][alt.Columns.IndexOf(col)])}")) +
                                $" WHERE {PalavraBanco(id.ColumnName)}={ValorBanco(id_val)}";

                            sb.AppendLine(update + finalUpdate + ";");

                            if (lote == quantLote || r == alt.Rows.Count - 1)
                            {
                                com.CommandText = sb.ToString();
                                afetados_alt += com.ExecuteNonQuery();
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

        /// <summary>
        /// Seleciona as colunas preenchidas ou alteradas na Row atual.
        /// </summary>
        /// <param name="dt">Datatable que a Row está associada.</param>
        /// <param name="row">Índice do Row.</param>
        /// <param name="insert">Tipo de operação, se true, insert, caso contrário, update.</param>
        /// <returns></returns>
        private static DataColumn[] SelecionarColunas(DataTable dt, int row, bool insert)
        {
            List<DataColumn> sel_cols = new List<DataColumn>();
            for (int col = 0; col < dt.Columns.Count; col++)
            {
                if (dt.Columns[col].AutoIncrement) continue; // Ignora coluna Auto Incremento

                if (insert)
                {
                    // Obtém as colunas preenchidas
                    object current = dt.Rows[row].Field<object>(col, DataRowVersion.Current);  // Obtém o valor atual

                    if (current != null)
                        sel_cols.Add(dt.Columns[col]);
                }
                else
                {
                    // Obtém as colunas alteradas
                    object original = dt.Rows[row].Field<object>(col, DataRowVersion.Original);  // Obtém o valor original
                    object current = dt.Rows[row].Field<object>(col, DataRowVersion.Current);    // Obtém o valor alterado

                    bool selecionar = false;
                    if (!(original is null && current is null)) // Ignora se ambos são nulos.
                    {
                        if (original is null ^ current is null) // Previne erro de referência a objeto quando equals é invocado em objeto nulo antecipando o resultado. Visto que um dos dois são nulos isso significa que os valores são diferentes. No entanto, campo está alterado!
                            selecionar = true;
                        else if (!original.Equals(current)) // Compara valores em seu respectivo tipo quando ambos definitivamente possuem valores, ou seja, nenhum valor é nulo.
                            selecionar = true;
                    }

                    if (selecionar)
                        sel_cols.Add(dt.Columns[col]);
                }
            }

            return sel_cols.ToArray();
        }
    }
}
