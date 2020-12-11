using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace _20201209_db_data_adapter
{
    public partial class Form1 : Form
    {
        SqlConnection connection;
        DataSet set = null;
        SqlDataAdapter adapter = null;

        public Form1()
        {
            InitializeComponent();

            string connString = ConfigurationManager
                .ConnectionStrings["localDbCS"]
                .ConnectionString;

            connection = new SqlConnection(connString);

            set = new DataSet();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            try
            {
                string selectQuery = txtQuery.Text;

                adapter = new SqlDataAdapter(selectQuery, connection);
                SqlCommandBuilder cb = new SqlCommandBuilder(adapter);

                //modifyUpdateCommand();

                modifyUpdateCommandByProcedure();

                set.Tables.Clear();
                adapter.Fill(set, "buf_table");                
                dgvMain.DataSource = set.Tables["buf_table"];
            }
            catch (Exception ex)
            {

                MessageBox.Show($"ERROR: {ex.Message}");
            }
        }

        private void modifyUpdateCommand()
        {
            string query = "update users set nickname = @p_nickname, password = @p_password where id = @p_id";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.Add(new SqlParameter("@p_nickname", SqlDbType.NVarChar)
            {
                SourceVersion = DataRowVersion.Current, 
                SourceColumn = "nickname"
            });

            cmd.Parameters.Add(new SqlParameter("@p_password", SqlDbType.NVarChar)
            {
                SourceVersion = DataRowVersion.Current,
                SourceColumn = "password"
            });

            cmd.Parameters.Add(new SqlParameter("@p_id", SqlDbType.Int)
            {
                SourceVersion = DataRowVersion.Original,
                SourceColumn = "id"
            });

            adapter.UpdateCommand = cmd;
        }


        private void modifyUpdateCommandByProcedure()
        {
            SqlCommand cmd = new SqlCommand("updateUsersAdapterCommand", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@nickname", SqlDbType.NVarChar, 50, "nickname");
            cmd.Parameters.Add("@password", SqlDbType.NVarChar, 50, "password");
            cmd.Parameters.Add("@id", SqlDbType.Int, 0, "id")
                .SourceVersion = DataRowVersion.Original;

            adapter.UpdateCommand = cmd;

        }

        private void modifySelectCommandByProcedure()
        {
            SqlCommand cmd = new SqlCommand("selectUsersAdapterCommand", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@email", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@nickname", SqlDbType.NVarChar, 50);

            adapter.SelectCommand = cmd;
        }

        private void modifyInsertCommandByProcedure()
        {
            SqlCommand cmd = new SqlCommand("insertUsersAdapterCommand", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@email", SqlDbType.NVarChar, 50, "email");
            cmd.Parameters.Add("@nickname", SqlDbType.NVarChar, 50, "nickname");
            cmd.Parameters.Add("@password", SqlDbType.NVarChar, 50, "password");

            adapter.InsertCommand = cmd;
        }

        private void modifyDeleteCommandByProcedure()
        {
            SqlCommand cmd = new SqlCommand("deleteUsersAdapterCommand", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@id", SqlDbType.Int, 0, "id")
                .SourceVersion = DataRowVersion.Original;

            adapter.DeleteCommand = cmd;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //adapter.Update(set, "buf_table");
                DataTable table = set.Tables["buf_table"];
                adapter.Update(table.Select(null, null, DataViewRowState.Deleted));
                adapter.Update(table.Select(null, null, DataViewRowState.ModifiedCurrent));
                adapter.Update(table.Select(null, null, DataViewRowState.Added));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR: {ex.Message}");
            }
        }
    }
}