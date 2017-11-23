using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SQLForm
{
    public static class DatabaseConnection
    {
        private static string sqlSelectQuery = "";
        private static string currentTableData = "";
        private static string PKColumnName = "";
        private static int currentRecord = 0;
        private static string currentPKValue = "";
        private static FlowLayoutPanel tableData = null;
        private static string connectionString =
            "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = " + System.IO.Directory.GetCurrentDirectory() + "\\database.mdf; Integrated Security = True; Connect Timeout = 30";
        private static string getPKFromTable =
            "SELECT u.COLUMN_NAME, c.CONSTRAINT_NAME " +
            "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS c " +
            "INNER JOIN " +
            "INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS u " +
            "ON c.CONSTRAINT_NAME = u.CONSTRAINT_NAME " +
            "WHERE u.TABLE_NAME = 'table' AND c.TABLE_NAME = 'table' " + 
            "AND c.CONSTRAINT_TYPE = 'PRIMARY KEY'";

        private static void HandleError(SqlException e)
        {
            string message = e.Message.ToString();
            if (message.Contains("null"))
            {
                message = "Expected field value not supplied. Please check form details";
            }
            if (message.Contains("duplicate key"))
            {
                message = "Unique key entry already exists in database. Please use new value";
            }

            MessageBox.Show(message, "SQL Error");
        }

        public static List<string> GetTableList()
        {
            List<string> tables = new List<string>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM sys.Tables",con))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return tables;
        }

        public static List<string> GetTableFields(string table)
        {
            List<string> fields = new List<string>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand("SELECT * FROM " + table, con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                fields.Add(reader.GetName(i));
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    HandleError(e);
                }
            }

            return fields;
        }

        public static void GetSearchResults(int x, int y, string table, string where, string contains)
        {
            string sql = "SELECT * FROM " + table + " WHERE " + where + " LIKE '%" + contains + "%'";
            CreateInitialFlowLayout(x, y);
            currentTableData = table;
            CreateFieldLabelsandText(sql, true);
            sqlSelectQuery = sql;
            LoadTableData();
        }

        private static void CreateInitialFlowLayout(int x, int y)
        {
            if (tableData != null)
            {
                foreach (Control c in tableData.Controls)
                {
                    ControlGenerator.RemoveControl(c);
                }
                tableData.Controls.Clear();
            }
            ControlGenerator.RemoveControl(tableData);
            tableData = ControlGenerator.CreateFlowLayoutPanel();
            tableData.Height = Settings.formHeight - y - 20;
            tableData.Width = Settings.formWidth - x - 20;
            tableData.Location = new Point(x, y);
        }
        private static void CreateFieldLabelsandText(string sql, bool useData = false)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string field = reader.GetName(i);
                                Label l = ControlGenerator.CreateLabel(0, 0, field);
                                tableData.Controls.Add(l);
                                if (useData)
                                {
                                    TextBox t = ControlGenerator.CreateTextBox(0, 0, Settings.formWidth - 50, 50);
                                    t.Name = "textBox" + field;
                                    tableData.Controls.Add(t);
                                    tableData.SetFlowBreak(t, true);
                                }
                                else
                                {
                                    tableData.SetFlowBreak(l, true);
                                }
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    HandleError(e);
                }
                try
                {
                    string f = getPKFromTable.Replace("table", currentTableData);
                    using (SqlCommand command = new SqlCommand(f, con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            PKColumnName = reader.GetString(0);
                        }
                    }
                }
                catch (SqlException e)
                {
                    HandleError(e);
                }
            }
        }

        public static void PopulateDataSheet(int x, int y, string table, bool useData = false, bool loadData = false)
        {
            CreateInitialFlowLayout(x, y);
            currentTableData = table;
            CreateFieldLabelsandText("SELECT * FROM " + table,useData);
            sqlSelectQuery = "SELECT * FROM " + table;
            if (loadData)
                LoadTableData();
        }

        public static void LoadTableData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(sqlSelectQuery, con))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            foreach (Control c in tableData.Controls)
                            {
                                if (c.Name.StartsWith("textBox"))
                                {
                                    c.Text = "";
                                }
                            }
                            currentRecord = 0;
                            currentPKValue = "";
                        }
                        DataTable data = new DataTable();
                        data.Load(reader);
                        if (data.Rows.Count > currentRecord)
                        {
                            var columns = data.Rows[currentRecord].ItemArray.Select(x => x.ToString()).ToArray();
                            foreach (Control c in tableData.Controls)
                            {
                                if (c.Name.StartsWith("textBox"))
                                {
                                    c.Text = columns[data.Columns[c.Name.Substring(7)].Ordinal];
                                }
                            }
                            currentPKValue = columns[data.Columns[PKColumnName].Ordinal];
                        }
                        else
                        {
                            currentRecord = data.Rows.Count - 1;
                            if (currentRecord < 0)
                                currentRecord = 0;
                        }
                    }
                }
            }
        }

        public static void ShiftRecord(bool forward)
        {
            if (currentTableData == "")
                return;
            if (forward)
            {
                currentRecord++;
                LoadTableData();
            }else
            {
                currentRecord--;
                if (currentRecord < 0)
                    currentRecord = 0;
                LoadTableData();
            }
        }

        public static void RunDelete()
        {
            if (tableData == null)
            {
                MessageBox.Show("Please select table");
                return;
            }
            DialogResult dialogueResult = MessageBox.Show("About to delete record. Proceed?", "Data delete", MessageBoxButtons.YesNo);
            if (dialogueResult == DialogResult.No)
                return;
            string sql = "DELETE FROM " + currentTableData + " WHERE " + PKColumnName + " = " + currentPKValue;
            bool successful = true;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        HandleError(e);
                        successful = false;
                    }
                }
            }
            if (successful)
            {
                dialogueResult = MessageBox.Show("Entry successfully updated.", "Update successful");
                ShiftRecord(false);
            }
        }

        public static void RunUpdate()
        {
            if (tableData == null)
            {
                MessageBox.Show("Please select table");
                return;
            }
            if (currentPKValue == "")
            {
                MessageBox.Show("No record selected");
                return;
            }
            DialogResult dialogueResult = MessageBox.Show("About to update record. Proceed?", "Data update", MessageBoxButtons.YesNo);
            if (dialogueResult == DialogResult.No)
                return;
            List<string> columns = new List<string>();
            List<string> vals = new List<string>();
            foreach (Control c in tableData.Controls)
            {
                if (c.Name.StartsWith("textBox"))
                {
                    if (c.Text != "")
                    {
                        columns.Add(c.Name.Substring(7));
                        vals.Add(c.Text);
                    }
                }
            }
            string sql = "UPDATE " + currentTableData + " SET";
            for (int i = 0; i < vals.Count; i++)
            {
                sql += " " + columns[i] + " = @param" + i;
                if (i < vals.Count - 1)
                    sql += ",";
            }
            sql += " WHERE " + PKColumnName + " = " + currentPKValue;
            bool successful = true;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    for (int i = 0; i < vals.Count; i++)
                    {
                        command.Parameters.AddWithValue("@param" + i, vals[i]);
                    }
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        HandleError(e);
                        successful = false;
                    }
                }
            }
            if (successful)
            {
                dialogueResult = MessageBox.Show("Entry successfully updated.", "Update successful");
            }
        }
        public static void RunInsert()
        {
            if (tableData == null)
            {
                MessageBox.Show("Please select table");
                return;
            }
            DialogResult dialogueResult = MessageBox.Show("About to insert data to database. Proceed?", "Data insert", MessageBoxButtons.YesNo);
            if (dialogueResult == DialogResult.No)
                return;
            List<string> columns = new List<string>();
            List<string> vals = new List<string>();
            foreach(Control c in tableData.Controls)
            {
                if (c.Name.StartsWith("textBox"))
                {
                    if (c.Text != "")
                    {
                        columns.Add(c.Name.Substring(7));
                        vals.Add(c.Text);
                    }
                }
            }
            string sql = "INSERT INTO " + currentTableData + "(";
            string insertColumns = "";
            string insertParams = "";
            for(int i = 0; i < vals.Count; i++)
            {
                insertColumns += columns[i];
                insertParams += "@param" + i.ToString();
                if (i < vals.Count - 1)
                {
                    insertColumns += ",";
                    insertParams += ",";
                }
            }
            if (insertColumns == "")
            {
                MessageBox.Show("No data entered", "No data");
                return;
            }
            sql += insertColumns + ") VALUES (" + insertParams + ")";
            bool successful = true;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    for (int i = 0; i < vals.Count; i++)
                    {
                        if(vals[i] != null)
                            command.Parameters.AddWithValue("@param" + i,vals[i]);
                        else
                            command.Parameters.AddWithValue("@param" + i,"");
                    }
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch(SqlException e)
                    {
                        HandleError(e);
                        successful = false;
                    }
                }
            }
            if (successful)
            {
                dialogueResult = MessageBox.Show("Entry successfully entered. Clear fields for next entry?", "Insert successful", MessageBoxButtons.YesNo);
                if (dialogueResult == DialogResult.Yes)
                {
                    foreach (Control c in tableData.Controls)
                    {
                        if (c.Name.StartsWith("textBox"))
                        {
                            c.Text = "";
                        }
                    }
                }
            }
        }
    }
}
