using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using PharmacyInventoryApp.Data;

namespace PharmacyInventoryApp.Forms
{
    public class MainForm : Form
    {
        TextBox txtName = new TextBox { Left = 20, Top = 30, Width = 200 };
        TextBox txtCategory = new TextBox { Left = 240, Top = 30, Width = 150 };
        TextBox txtPrice = new TextBox { Left = 410, Top = 30, Width = 100 };
        TextBox txtQuantity = new TextBox { Left = 530, Top = 30, Width = 100 };

        TextBox txtSearch = new TextBox { Left = 20, Top = 90, Width = 200 };
        Button btnAdd = new Button { Text = "Add Medicine", Left = 20, Top = 130, Width = 150 };
        Button btnSearch = new Button { Text = "Search", Left = 240, Top = 90, Width = 100 };
        Button btnUpdateStock = new Button { Text = "Update Stock", Left = 180, Top = 130, Width = 150 };
        Button btnRecordSale = new Button { Text = "Record Sale", Left = 340, Top = 130, Width = 150 };
        Button btnViewAll = new Button { Text = "View All", Left = 500, Top = 130, Width = 120 };

        DataGridView grid = new DataGridView { Left = 20, Top = 180, Width = 760, Height = 300, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

        public MainForm()
        {
            Text = "Pharmacy Inventory System";
            Width = 820;
            Height = 540;
            StartPosition = FormStartPosition.CenterScreen;

            Controls.AddRange(new Control[] {
                new Label { Text = "Name", Left = 20, Top = 10, AutoSize = true },
                txtName,
                new Label { Text = "Category", Left = 240, Top = 10, AutoSize = true },
                txtCategory,
                new Label { Text = "Price", Left = 410, Top = 10, AutoSize = true },
                txtPrice,
                new Label { Text = "Quantity", Left = 530, Top = 10, AutoSize = true },
                txtQuantity,

                new Label { Text = "Search", Left = 20, Top = 70, AutoSize = true },
                txtSearch,
                btnSearch,
                btnAdd, btnUpdateStock, btnRecordSale, btnViewAll,
                grid
            });

            btnAdd.Click += BtnAdd_Click;
            btnSearch.Click += BtnSearch_Click;
            btnViewAll.Click += (s, e) => LoadAll();
            btnUpdateStock.Click += BtnUpdateStock_Click;
            btnRecordSale.Click += BtnRecordSale_Click;

            Load += (s, e) => LoadAll();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtPrice.Text, out var price) || !int.TryParse(txtQuantity.Text, out var qty))
            {
                MessageBox.Show("Invalid price or quantity.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Db.GetConnection())
                using (var cmd = new SqlCommand("AddMedicine", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Name", SqlDbType.VarChar, 120).Value = txtName.Text.Trim();
                    cmd.Parameters.Add("@Category", SqlDbType.VarChar, 120).Value = txtCategory.Text.Trim();
                    cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = price;
                    cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = qty;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Medicine added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add medicine.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = Db.GetConnection())
                using (var cmd = new SqlCommand("SearchMedicine", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SearchTerm", SqlDbType.VarChar, 120).Value = txtSearch.Text.Trim();
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        var table = new DataTable();
                        table.Load(r);
                        grid.DataSource = table;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAll()
        {
            try
            {
                using (var conn = Db.GetConnection())
                using (var cmd = new SqlCommand("GetAllMedicines", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        var table = new DataTable();
                        table.Load(r);
                        grid.DataSource = table;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load failed.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdateStock_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Select a medicine in the grid.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var id = Convert.ToInt32(grid.CurrentRow.Cells["MedicineID"].Value);
            var prompt = Microsoft.VisualBasic.Interaction.InputBox("Enter new quantity:", "Update Stock", "0");
            if (!int.TryParse(prompt, out var newQty))
            {
                MessageBox.Show("Invalid quantity.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Db.GetConnection())
                using (var cmd = new SqlCommand("UpdateStock", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@MedicineID", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = newQty;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Stock updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRecordSale_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Select a medicine in the grid.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var id = Convert.ToInt32(grid.CurrentRow.Cells["MedicineID"].Value);
            var prompt = Microsoft.VisualBasic.Interaction.InputBox("Enter quantity sold:", "Record Sale", "1");
            if (!int.TryParse(prompt, out var qtySold) || qtySold <= 0)
            {
                MessageBox.Show("Invalid quantity.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Db.GetConnection())
                using (var cmd = new SqlCommand("RecordSale", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@MedicineID", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@QuantitySold", SqlDbType.Int).Value = qtySold;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Sale recorded.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAll();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Sale failed.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sale failed.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
