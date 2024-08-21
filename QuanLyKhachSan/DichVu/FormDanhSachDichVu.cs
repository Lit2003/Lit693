using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyKhachSan.DichVu
{
    public partial class FormDanhSachDichVu : Form
    {
        public FormDanhSachDichVu()
        {
            InitializeComponent();
 
        }

        private void FormDanhSachDichVu_Load(object sender, EventArgs e)
        {
            loadData();
            LoadDataIntoComboBox();
     
  
        }
        #region
        string connectionString = "Data Source=DESKTOP-JKB5K32\\SQLEXPRESS;Initial Catalog=KhachSan;Integrated Security=True;Encrypt=False";
        string query = "select IDDV as 'ID', TenDV as 'Tên Dịch Vụ', Gia as 'Giá', LoaiDV as'Loại' from tb_DichVu";
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adt;
        #endregion
        void loadData()
        {

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    // Định dạng cột "Giá" trong DataGridView
                    dataGridView1.Columns["Giá"].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }
            ResetID();
            binding(dataGridView1);
        }
        void binding(DataGridView dtgv)
        {
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = dtgv.DataSource;
            Binding bdID = new Binding("Text", dtgv.DataSource, "ID", true, DataSourceUpdateMode.OnPropertyChanged);
            txbID.DataBindings.Clear();
            txbID.DataBindings.Add(bdID);

            Binding bdTenDV = new Binding("Text", dtgv.DataSource, "Tên Dịch Vụ", true, DataSourceUpdateMode.OnPropertyChanged);
            txbTenDV.DataBindings.Clear();
            txbTenDV.DataBindings.Add(bdTenDV);

            Binding bdGia = new Binding("Text", dtgv.DataSource, "Giá", true, DataSourceUpdateMode.OnPropertyChanged);
            bdGia.Format += (s, e) => { e.Value = string.Format("{0:N0}", e.Value); };
            bdGia.Parse += (s, e) => { e.Value = float.Parse(e.Value.ToString(), System.Globalization.NumberStyles.AllowThousands); };
            txbGia.DataBindings.Clear();
            txbGia.DataBindings.Add(bdGia);

            Binding bdSDT = new Binding("Text", dtgv.DataSource, "Loại", true, DataSourceUpdateMode.OnPropertyChanged);
            txbLoai.DataBindings.Clear();
            txbLoai.DataBindings.Add(bdSDT);


        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            Them();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            Sua();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            Xoa();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        void Them()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                var maxId = db.tb_DichVu.Select(m => m.IDDV).Max();
                string id = (maxId == null) ? "1" : (int.Parse(maxId) + 1).ToString();
                string tendv = txbTenDV.Text;
                if (db.tb_DichVu.Select(t => t.TenDV).Contains(tendv))
                {
                    MessageBox.Show("Tên Dịch Vụ Đã Tồn Tại");
                    return;
                }
                float Gia = float.Parse(txbGia.Text);
                string loai = txbLoai.Text;
               

                tb_DichVu p = new tb_DichVu()
                {
                    IDDV = id,
                    Gia = Gia,
                    LoaiDV = loai,
                    TenDV = tendv,
      
                };
                db.tb_DichVu.Add(p);
                db.SaveChanges();
                MessageBox.Show("Thêm Dịch Vụ Thành Công");
                loadData();
            }
        }
        void Xoa()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                string id = txbID.Text;
                db.tb_DichVu.Remove(db.tb_DichVu.Find(id));
                db.SaveChanges();
                ResetID();
                MessageBox.Show("Xóa Dịch Vụ Thành Công");
                loadData();
            }
        }
        void Sua()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                string id = txbID.Text;
                string ten = txbTenDV.Text;
                string loai = txbLoai.Text;
                float Gia = float.Parse(txbGia.Text);
                tb_DichVu dv = db.tb_DichVu.Find(id);
                dv.TenDV = ten;
                dv.LoaiDV = loai;
                dv.Gia = Gia;

                db.SaveChanges();
                MessageBox.Show("Sửa Dịch Vụ Thành Công");
                loadData();
            }
        }

        void ResetID()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                var allServices = db.tb_DichVu.OrderBy(dv => dv.IDDV).ToList();
                List<tb_DichVu> reorderedServices = new List<tb_DichVu>();
                int newId = 1;

                foreach (var service in allServices)
                {
                    tb_DichVu newService = new tb_DichVu
                    {
                        IDDV = newId.ToString(),
                        TenDV = service.TenDV,
                        Gia = service.Gia,
                        LoaiDV = service.LoaiDV
                    };
                    reorderedServices.Add(newService);
                    newId++;
                }

                db.tb_DichVu.RemoveRange(db.tb_DichVu);
                db.tb_DichVu.AddRange(reorderedServices);
                db.SaveChanges();
            }
        }
        private void LoadDataIntoComboBox()
        {

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT DISTINCT LoaiDV FROM tb_DichVu";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Thêm lựa chọn "Tất cả" vào ComboBox
                    DataRow allRow = dt.NewRow();
                    allRow["LoaiDV"] = "Tất cả";
                    dt.Rows.InsertAt(allRow, 0);

                    comboBox1.DataSource = dt;
                    comboBox1.DisplayMember = "LoaiDV";
                    comboBox1.ValueMember = "LoaiDV";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void btnXem_Click(object sender, EventArgs e)
        {
            string selectedLoai = comboBox1.SelectedValue.ToString();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query;

                    // Nếu lựa chọn là "Tất cả", thì truy vấn lấy tất cả dữ liệu
                    if (selectedLoai == "Tất cả")
                    {
                        query = "select IDDV as 'ID', TenDV as 'Tên Dịch Vụ', Gia as 'Giá', LoaiDV as'Loại' FROM tb_DichVu";

                    }
                    else
                    {
                        query = "select IDDV as 'ID', TenDV as 'Tên Dịch Vụ', Gia as 'Giá', LoaiDV as'Loại' FROM tb_DichVu WHERE LoaiDV = @Loai";
                    }

                    SqlCommand cmd = new SqlCommand(query, con);

                    // Nếu không phải là lựa chọn "Tất cả", thì thêm tham số vào truy vấn
                    if (selectedLoai != "Tất cả")
                    {
                        cmd.Parameters.AddWithValue("@Loai", selectedLoai);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            binding(dataGridView1);
        }

    }
}
