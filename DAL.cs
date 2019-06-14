using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridDataTable
{
    public static class DAL
    {
        static string strConexao = "Server=localhost,3741;Database=Teste;User Id=sa;Password=d120588$788455";

        public static DataTable Carregar()
        {
            using (SqlConnection con = new SqlConnection(strConexao))
            using (SqlCommand com = new SqlCommand("", con))
            {
                con.Open();

                com.CommandText = "SELECT * FROM Teste..Tabela";
                using (SqlDataReader dr = com.ExecuteReader())
                {
                    DataTable dt = new DataTable();

                    dt.BeginLoadData();
                    dt.Load(dr);
                    dt.EndLoadData();

                    return dt;
                }
            }
        }

        public static async Task<int> Aplicar(DataTable dt, string tabela)
        {
            int quantLote = 2;

            int afetados_add = 0,
                afetados_del = 0,
                afetados_alt = 0;
            
            try
            {
                DataTable add = dt.GetChanges(DataRowState.Added);
                DataTable del = dt.GetChanges(DataRowState.Deleted);
                DataTable alt = dt.GetChanges(DataRowState.Modified);

                using (SqlConnection con = new SqlConnection(strConexao))
                using (SqlCommand com = new SqlCommand("", con))
                {
                    await con.OpenAsync();
                    com.Transaction = con.BeginTransaction(IsolationLevel.RepeatableRead);
                    
                    DataColumn id = dt.Columns.Cast<DataColumn>().Where(x => x.AutoIncrement).FirstOrDefault();
                    int idx_id = dt.Columns.IndexOf(id);

                    if (id == null)
                    {
                        throw new Exception("É necessário criar uma coluna Auto Incremento");
                    }

                    if (add != null)
                    {
                        DataColumn[] cols = add.Columns.Cast<DataColumn>()
                            .Where(x => !x.AutoIncrement) // Ignora colunas Auto Incremento
                            .ToArray();

                        string sColunas = string.Join(",", cols.Select(x => x.ColumnName));

                        string values = "", reg = "";
                        string comando = $"INSERT INTO {tabela}(" + sColunas + ")VALUES";

                        for (int r = 0, lote = 1; r < add.Rows.Count; r++, lote++)
                        {
                            #region Monta Registro
                            for (int c = 0; c < cols.Count(); c++)
                            {
                                if (reg != "") reg += ",";
                                reg += $"'{add.Rows[r][add.Columns.IndexOf(cols[c])]}'";
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
                                com.CommandText = comando + values + ";";
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
                            com.CommandText += $"DELETE FROM {tabela} WHERE {id.ColumnName}='{id_val}';\r\n";

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
                            object id_val = alt.Rows[r][idx_id, DataRowVersion.Original];

                            com.CommandText += $"UPDATE TOP 1 {tabela} " + 
                                string.Join(",", cols.Select(col => $"SET {col.ColumnName}='{alt.Rows[r][alt.Columns.IndexOf(col)]}'")) +
                                $" WHERE {id}='{id_val}';\r\n";

                            if (lote == quantLote || r == alt.Rows.Count - 1)
                            {
                                afetados_alt += com.ExecuteNonQuery();
                                com.CommandText = "";
                            }
                        }
                    }

                    com.Transaction.Commit();
                }

                dt.AcceptChanges();
            }
            catch (Exception ex)
            {
                
            }
            
            return afetados_add + afetados_del + afetados_alt;
        }
    }
}
