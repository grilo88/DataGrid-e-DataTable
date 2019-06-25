using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridDataTable
{
    public enum Banco
    {
        MySql = 1,
        MsSql = 2,
        SqLite = 3
    }

    public enum CondicaoPesquisa
    {
        IgualA,
        MaiorQue,
        MaiorOuIgualA,
        MenorQue,
        MenorOuIgualA,
        Contém,
        IniciaCom,
        TerminaCom,
        Palavra,
    }

    public class DAL
    {
        public Banco Banco { get; private set; } = Banco.SqLite;
        static string strConexao { get; set; }

        /// <summary>
        /// Ao usar valores padrão, apenas as colunas de interesse serão selecionadas na gravação 
        /// do banco de dados melhorando a performance do sistema. No caso do insert, campos preenchidos. 
        /// No caso do update, campos alterados.
        /// </summary>
        public bool UsarValoresPadrao { get; set; } = true;

        /// <summary>
        /// Simplifica o comando Delete reduzindo significamente o consumo de tráfego na conexão
        /// </summary>
        public bool UsarDeleteWhereIn { get; set; } = true;
        readonly Form owner = null;

        public DAL() : this(null) { }
        public DAL(Form owner)
        {
            this.owner = owner;
            switch (Banco)
            {
                case Banco.MySql:
                    MySqlConnectionStringBuilder sb_mysql = new MySqlConnectionStringBuilder();

                    // Conexão
                    sb_mysql.ConnectionTimeout = 5;
                    sb_mysql.Server = "localhost";
                    sb_mysql.UserID = "root";
                    sb_mysql.Password = "123456";
                    sb_mysql.Database = "Teste";

                    // Segurança
                    sb_mysql.SslMode = MySqlSslMode.None;
                    sb_mysql.AllowPublicKeyRetrieval = true;

                    // Pool
                    sb_mysql.Pooling = true;
                    sb_mysql.MinimumPoolSize = 5;
                    sb_mysql.MaximumPoolSize = 15;
                    sb_mysql.ConnectionLifeTime = 60;

                    strConexao = sb_mysql.ToString();
                    break;
                case Banco.MsSql:
                    SqlConnectionStringBuilder sb_mssql = new SqlConnectionStringBuilder();
                    // Conexão
                    sb_mssql.ConnectTimeout = 5;
                    sb_mssql.DataSource = "localhost,3741";
                    sb_mssql.UserID = "sa";
                    sb_mssql.Password = "d120588$788455";
                    sb_mssql.InitialCatalog = "Teste";

                    // Pool
                    sb_mssql.Pooling = true;
                    sb_mssql.MinPoolSize = 5;
                    sb_mssql.MaxPoolSize = 15;

                    strConexao = sb_mssql.ToString();
                    break;
                case Banco.SqLite:
                    SQLiteConnectionStringBuilder sb_sqlite = new SQLiteConnectionStringBuilder();
                    sb_sqlite.DataSource = @"..\..\sqlite.db3";
                    sb_sqlite.Password = "";
                    sb_sqlite.Pooling = true;
                    sb_sqlite.Version = 3;

                    strConexao = sb_sqlite.ToString();
                    break;
                default:
                    throw new NotImplementedException(nameof(Banco));
            }
        }

        public Task OpenAsync(IDbConnection con)
        {
            switch (Banco)
            {
                case Banco.MySql:
                    return ((MySqlConnection)con).OpenAsync();
                case Banco.MsSql:
                    return ((SqlConnection)con).OpenAsync();
                case Banco.SqLite:
                    return ((SQLiteConnection)con).OpenAsync();
                default:
                    throw new NotImplementedException(nameof(Banco));
            }
        }

        public IDbConnection CrossConnection(string conexao)
        {
            switch (Banco)
            {
                case Banco.MySql:
                    return new MySqlConnection(conexao);
                case Banco.MsSql:
                    return new SqlConnection(conexao);
                case Banco.SqLite:
                    return new SQLiteConnection(conexao);
                default:
                    throw new NotImplementedException(nameof(Banco));
            }
        }

        public IDbCommand CrossCommand(IDbConnection con)
        {
            switch (Banco)
            {
                case Banco.MySql:
                    return new MySqlCommand("", (MySqlConnection)con);
                case Banco.MsSql:
                    return new SqlCommand("", (SqlConnection)con);
                case Banco.SqLite:
                    return new SQLiteCommand((SQLiteConnection)con);
                default:
                    throw new NotImplementedException(nameof(Banco));
            }
        }

        public DataTable Schema(IDbCommand com)
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

        public async Task<DataTable> Carregar(DataTable dt) => await Carregar(dt, null, -1);
        public async Task<DataTable> Carregar(DataTable dt, int limite) => await Carregar(dt, null, limite);

        public async Task<DataTable> Carregar(DataTable dt, string[] colunas, int limite)
        {
            if (dt == null) throw new Exception("Instancie o DataTable");
            if (dt.TableName == "") throw new Exception("Informe o nome da tabela ao instanciar o DataTable");
            if (dt.PrimaryKey.Length == 0 && dt.Rows.Count > 0) throw new Exception("É necessário possuir uma coluna chave primária com auto-incremento no DataTable para realizar a união (atualização) dos registros neste recarregamento de dados. Obtenha o modelo através do método estático DAL.Carregar() sobre a definição de chave-primária.");

            frmProcessando.TelaProcessando(owner);
            frmProcessando.TextoLegenda = $"Carregando tabela '{dt.TableName}'";

            try
            {
                using (IDbConnection con = CrossConnection(strConexao))
                using (IDbCommand com = CrossCommand(con))
                {
                    await OpenAsync(con);

                    string select = "SELECT ";
                    string cols = colunas == null || colunas.Length == 0 ? "*" : string.Join(",", colunas.Select(col => PalavraBanco(col)));
                    string finalSelect = "";
                    switch (Banco)
                    {
                        case Banco.MySql:
                        case Banco.SqLite:
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
                frmProcessando.Processando = false;
                MessageBox.Show(ex.Message, "Erro");
            }
            finally
            {
                frmProcessando.Processando = false;
            }

            return dt;
        }

        string BancoDeDados(IDbConnection con, bool ponto = true)
        {
            string bd;
            switch (Banco)
            {
                case Banco.MySql:
                case Banco.SqLite:
                    bd = PalavraBanco(con.Database) + (ponto ? "." : "");
                    break;
                case Banco.MsSql:
                    bd = PalavraBanco(con.Database) + (ponto ? ".." : "");
                    break;
                default:
                    throw new NotImplementedException(nameof(Banco));
            }
            return bd;
        }

        /// <summary>
        /// Previne erros de comando sql quando uma palavra entra em conflito com alguma palavra-chave do banco de dados.
        /// </summary>
        /// <param name="palavra"></param>
        /// <returns></returns>
        string PalavraBanco(string palavra)
        {
            switch (Banco)
            {
                case Banco.MySql:
                case Banco.SqLite:
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
        object ValorBanco(object valor)
        {
            switch (Banco)
            {
                case Banco.MySql:
                case Banco.SqLite:
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
        public async Task<int> Aplicar(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            int quantLote = 150;

            int afetados_add = 0,
                afetados_del = 0,
                afetados_alt = 0;

            try
            {
                frmProcessando.TelaProcessando(owner);
                frmProcessando.TextoLegenda = "Preparando para aplicar alterações...";

                DataTable add = dt.GetChanges(DataRowState.Added);
                DataTable del = dt.GetChanges(DataRowState.Deleted);
                DataTable alt = dt.GetChanges(DataRowState.Modified);

                using (IDbConnection con = CrossConnection(strConexao))
                using (IDbCommand com = CrossCommand(con))
                {
                    await OpenAsync(con);

                    switch (Banco)
                    {
                        case Banco.MySql:
                        case Banco.MsSql:
                            com.Transaction = con.BeginTransaction(IsolationLevel.RepeatableRead);
                            break;
                        case Banco.SqLite:
                            // SQlite não dá suporte para IsolationLevel.RepeatableRead
                            com.Transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                            break;
                        default:
                            throw new NotImplementedException(nameof(Banco));
                    }

                    DataColumn id = dt.Columns.Cast<DataColumn>().Where(x => x.AutoIncrement).FirstOrDefault();
                    if (id == null)
                        throw new Exception("É necessário criar uma coluna Auto Incremento");

                    int idx_id = dt.Columns.IndexOf(id);

                    if (add != null)
                    {
                        frmProcessando.TextoLegenda = "Inserindo registros...";

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
                            // Monta valores da row atual
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

                            // Monta lista de Values
                            if (values != "") values += ",";
                            values += "(" + reg + ")";
                            reg = "";

                            if (lote == quantLote ||
                                r == add.Rows.Count - 1 /* Último Row? */)
                            {
                                if (values != "")
                                {
                                    sb.AppendLine(insertinto + values + ";");
                                    values = "";
                                }

                                com.CommandText = sb.ToString();
                                afetados_add += com.ExecuteNonQuery();
                                lote = 0;
                                sb.Clear();
                            }
                        }
                    }
                    if (del != null)
                    {
                        frmProcessando.TextoLegenda = "Deletando registros...";

                        sb.Clear();
                        if (UsarDeleteWhereIn)
                        {
                            for (int r = 0, lote = 1; r < del.Rows.Count; r++, lote++)
                            {
                                object id_val = del.Rows[r][idx_id, DataRowVersion.Original];
                                sb.Append((sb.Length > 0 ? "," : "") + ValorBanco(id_val));

                                if (lote == quantLote || r == del.Rows.Count - 1)
                                {
                                    com.CommandText = $"DELETE FROM {PalavraBanco(dt.TableName)} WHERE {PalavraBanco(id.ColumnName)} IN ({sb.ToString()})";
                                    afetados_del += com.ExecuteNonQuery();
                                    lote = 0;
                                    sb.Clear();
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
                                    case Banco.SqLite:
                                        // Não dá suporte para LIMIT na instrução DELETE
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
                                    sb.Clear();
                                }
                            }
                        }
                    }
                    if (alt != null)
                    {
                        frmProcessando.TextoLegenda = "Alterando registros...";

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
                                case Banco.SqLite:
                                    // Não dá suporte para LIMIT na instrução UPDATE
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
                                sb.Clear();
                            }
                        }
                    }

                    com.Transaction.Commit();
                }
                dt.AcceptChanges();
            }
            catch (Exception ex)
            {
                frmProcessando.Processando = false;
                MessageBox.Show(ex.Message, "Erro");
                dt.RejectChanges();
            }
            finally
            {
                frmProcessando.Processando = false;
            }

            return
                afetados_add +
                afetados_del +
                afetados_alt;
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

        Timer tmrPesquisa = new Timer();
        public void Pesquisar(DataTable dt, string texto, bool diferenteDe, CondicaoPesquisa condicao, params DataColumn[] colunas)
        {
            tmrPesquisa.Stop();
            tmrPesquisa.Interval = 500; // Aciona o evento após X milisegundos
            tmrPesquisa.Tick += (sender, _e) =>
            {
                tmrPesquisa.Stop();
                OnPesquisa(new PesquisaEventArgs(PesquisarInterno(dt, texto, diferenteDe, condicao, colunas)));
            };
            tmrPesquisa.Start();
        }

        private object PesquisarInterno(
            DataTable dt, string texto,
            bool diferenteDe, CondicaoPesquisa condicao, params DataColumn[] colunas)
        {
            bool isNumero = long.TryParse(texto, out long numero);

            if (texto != "")
            {
                DataView view = dt.AsDataView();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < colunas.Length; i++)
                {
                    if (colunas[i].DataType == typeof(string))
                    {
                        string filtro;
                        switch (condicao)
                        {
                            case CondicaoPesquisa.IgualA:
                                filtro = $"[{colunas[i].ColumnName}]='{texto}'";
                                break;
                            case CondicaoPesquisa.Contém:
                                filtro = $"[{colunas[i].ColumnName}]LIKE '%{texto}%'";
                                break;
                            case CondicaoPesquisa.IniciaCom:
                                filtro = $"[{colunas[i].ColumnName}]LIKE '{texto}%'";
                                break;
                            case CondicaoPesquisa.TerminaCom:
                                filtro = $"[{colunas[i].ColumnName}]LIKE '%{texto}'";
                                break;
                            case CondicaoPesquisa.Palavra:
                                filtro = "(" +
                                    $"[{colunas[i].ColumnName}]='{texto}' OR" +
                                    $"[{colunas[i].ColumnName}]LIKE '{texto} %' OR" +
                                    $"[{colunas[i].ColumnName}]LIKE '% {texto} %' OR" +
                                    $"[{colunas[i].ColumnName}]LIKE '% {texto}'" +
                                    ")";
                                break;
                            default: continue;
                        }

                        if (sb.Length > 0) sb.Append(" OR ");
                        sb.Append(filtro);
                    }
                    else if (isNumero &&
                        (colunas[i].DataType == typeof(int) ||
                        colunas[i].DataType == typeof(long) ||
                        colunas[i].DataType == typeof(short) ||
                        colunas[i].DataType == typeof(byte) ||
                        colunas[i].DataType == typeof(sbyte)))
                    {
                        string filtro;
                        switch (condicao)
                        {
                            case CondicaoPesquisa.IgualA:
                            case CondicaoPesquisa.Contém:
                                filtro = $"[{colunas[i].ColumnName}]={numero}";
                                break;
                            case CondicaoPesquisa.MaiorQue:
                                filtro = $"[{colunas[i].ColumnName}]>{numero}";
                                break;
                            case CondicaoPesquisa.MaiorOuIgualA:
                                filtro = $"[{colunas[i].ColumnName}]>={numero}";
                                break;
                            case CondicaoPesquisa.MenorQue:
                                filtro = $"[{colunas[i].ColumnName}]<{numero}";
                                break;
                            case CondicaoPesquisa.MenorOuIgualA:
                                filtro = $"[{colunas[i].ColumnName}]<={numero}";
                                break;
                            default: continue;
                        }

                        if (sb.Length > 0) sb.Append(" OR ");
                        sb.Append(filtro);
                    }
                }

                view.RowFilter = diferenteDe ? $"NOT({sb.ToString()})" : sb.ToString();
                return view;
            }
            else
            {
                return dt;
            }
        }

        protected virtual void OnPesquisa(PesquisaEventArgs e)
        {
            ResultadoPesquisaHandler handler = ResultadoPesquisa;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        

        public static DataTable ObterDataTable(object datasource)
        {
            DataTable dt;
            if (datasource is DataView)
            {
                dt = ((DataView)datasource).Table;
            }
            else if (datasource is DataTable)
            {
                dt = (DataTable)datasource;
            }
            else
                throw new NotImplementedException(datasource.GetType().Name);

            return dt;
        }

        public static DataTable GerarRows(DataGridView dg, int linhas, params string[] cols)
        {
            frmProcessando.TelaProcessando(null);
            frmProcessando.TextoLegenda = $"Gerando {linhas} registros.";

            DataTable dt = ObterDataTable(dg.DataSource);
            dg.DataSource = null; // Carregamento rápido

            DataColumn[] sel = new DataColumn[cols.Length];
            for (int c = 0; c < cols.Length; c++)
            {
                sel[c] = dt.Columns.Cast<DataColumn>().Where(x => x.ColumnName == cols[c]).FirstOrDefault();
            }

            for (int i = 0; i < linhas; i++)
            {
                DataRow newRow = dt.NewRow();
                for (int c = 0; c < sel.Length; c++)
                {
                    if (sel[c].DataType == typeof(string))
                    {
                        newRow[sel[c]] = "Testando123";
                    }
                    else
                    {
                        throw new NotImplementedException(sel[c].DataType.Name);
                    }
                }
                dt.Rows.Add(newRow);
            }

            frmProcessando.Processando = false;
            return dt;
        }

        public event ResultadoPesquisaHandler ResultadoPesquisa;
    }

    public class PesquisaEventArgs : EventArgs
    {
        public object Resultado;
        public PesquisaEventArgs(object resultado)
        {
            this.Resultado = resultado;
        }
    }

    public delegate void ResultadoPesquisaHandler(object sender, PesquisaEventArgs e);
}
