using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyKhachSan.Phong
{
    public partial class FormDSPhong : Form
    {
        public FormDSPhong()
        {
            InitializeComponent();
            this.Name = "FormDSPhong";
           

        }
        public void LoadlaiDataphong()
        {
            loadData();
        }

        private void FormDSPhong_Load(object sender, EventArgs e)
        {
            loadData();           
            
        }
        #region
        string connectionString = "Data Source=DESKTOP-JKB5K32\\SQLEXPRESS;Initial Catalog=KhachSan;Integrated Security=True;Encrypt=False";
        string query = "select IDPhong as ID,TenP as 'Tên Phòng',TenLoaiP as 'Loại Phòng',Gia as 'Giá',SoGiuong as 'Số Giường',SoNguoi as 'Số Người',TenTrangThai as 'Trạng Thái' " +
            "from tb_Phong inner join tb_LoaiPhong on tb_Phong.IDLoaiP = tb_LoaiPhong.IDLoaiP " +
            "inner join tb_TrangThai on tb_Phong.IDTrangThai = tb_TrangThai.IDTrangThai ";
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adt;
        #endregion
        void loadData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {

                try
                {
                    con.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.Columns["Giá"].DefaultCellStyle.Format = "N0";

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return;
                }

            }
            binding(dataGridView1);
            LoadLoaiP(cmbLoaiP);
            LoadTrangThai(cmbTrangThai);
        }  
        void binding(DataGridView dtgv)
        {
            Binding bdID = new Binding("Text", dtgv.DataSource, "ID", true, DataSourceUpdateMode.OnPropertyChanged);
            txbID.DataBindings.Clear();
            txbID.DataBindings.Add(bdID);

            Binding bdTenP = new Binding("Text", dtgv.DataSource, "Tên Phòng", true, DataSourceUpdateMode.OnPropertyChanged);
            txbTenP.DataBindings.Clear();
            txbTenP.DataBindings.Add(bdTenP);

            Binding bdGia = new Binding("Text", dtgv.DataSource, "Giá", true, DataSourceUpdateMode.OnPropertyChanged);
            bdGia.Format += (s, e) => { e.Value = string.Format("{0:N0}", e.Value); };
            bdGia.Parse += (s, e) => { e.Value = float.Parse(e.Value.ToString(), System.Globalization.NumberStyles.AllowThousands); };
            txbGia.DataBindings.Clear();
            txbGia.DataBindings.Add(bdGia);

            Binding bdSoNguoi = new Binding("Text", dtgv.DataSource, "Số Người", true, DataSourceUpdateMode.OnPropertyChanged);
            txbSoNguoi.DataBindings.Clear();
            txbSoNguoi.DataBindings.Add(bdSoNguoi);

            Binding bdSoG = new Binding("Text", dtgv.DataSource, "Số Giường", true, DataSourceUpdateMode.OnPropertyChanged);
            txbSoGiuong.DataBindings.Clear();
            txbSoGiuong.DataBindings.Add(bdSoG);

        }
        void LoadLoaiP(ComboBox cmb)
        {
            using(KhachSanEntities db = new KhachSanEntities())
            {
                cmbLoaiP.DataSource = db.tb_LoaiPhong.ToList();
                cmbLoaiP.DisplayMember = "TenLoaiP";
            }
        }
        void LoadTrangThai(ComboBox cmb)
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                cmbTrangThai.DataSource = db.tb_TrangThai.ToList();
                cmbTrangThai.DisplayMember = "TenTrangThai";
            }
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0 || dataGridView1.SelectedCells[0].OwningRow == null)
                return;

            // Cập nhật cmbLoaiP
            if (cmbLoaiP.DataSource == null) return;
            
                string loai = dataGridView1.SelectedCells[0].OwningRow.Cells["Loại Phòng"].Value.ToString();
                int index = 0;
                using (KhachSanEntities db = new KhachSanEntities())
                {
                    index = db.tb_LoaiPhong.Select(p => p.TenLoaiP).ToList().IndexOf(loai);
                   
                }
                cmbLoaiP.SelectedIndex = index;
            

            // Cập nhật cmbTrangThai
            if (cmbTrangThai.DataSource == null)return;
            
                string trangthai = dataGridView1.SelectedCells[0].OwningRow.Cells["Trạng Thái"].Value.ToString();
                int index1 = 0;
                using (KhachSanEntities db = new KhachSanEntities())
                {
                    index1 = db.tb_TrangThai.Select(p => p.TenTrangThai).ToList().IndexOf(trangthai);
                  
                }
                cmbTrangThai.SelectedIndex = index1;
            



        }
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            Them();
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            Xoa();
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            Sua();
        }
        void Them()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                var maxId = db.tb_Phong.Select(m => m.IDPhong).Max();
                string id = (maxId == null) ? "1" : (int.Parse(maxId) + 1).ToString();
                string ten = txbTenP.Text;
                if (db.tb_Phong.Select(t => t.TenP).Contains(ten))
                {
                    MessageBox.Show("Tên Phòng Đã Tồn Tại");
                    return;
                }
                int loai = (cmbLoaiP.SelectedValue as tb_LoaiPhong).IDLoaiP;
                int trangthai = (cmbTrangThai.SelectedValue as tb_TrangThai).IDTrangThai;



                tb_Phong p = new tb_Phong()
                {
                    IDPhong = id,
                    TenP = ten,
                    IDLoaiP = loai,
                    IDTrangThai = trangthai,
                  

                };
                db.tb_Phong.Add(p);
                db.SaveChanges();
                MessageBox.Show("Thêm Phòng Thành Công");
                loadData();
            }
        }
        void Xoa()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                string id = txbID.Text;

                // Tìm phòng theo ID
                tb_Phong phong = db.tb_Phong.Find(id);
                if (phong == null)
                {
                    MessageBox.Show("Không tìm thấy phòng với ID đã cho.");
                    return; // Kết thúc nếu không tìm thấy phòng
                }

                // Kiểm tra trạng thái của phòng
                if (phong.IDTrangThai == db.tb_TrangThai.FirstOrDefault(tt => tt.TenTrangThai == "Đã Thuê")?.IDTrangThai)
                {
                    MessageBox.Show("Không thể xóa phòng này vì nó đang trong trạng thái Đã Thuê.");
                    return; // Kết thúc nếu phòng đang trong trạng thái Đã Thuê
                }

                // Xóa phòng
                db.tb_Phong.Remove(phong);
                db.SaveChanges();
                MessageBox.Show("Xóa Phòng Thành Công");
                ResetID(); // Gọi phương thức ResetID nếu cần thiết
                loadData(); // Tải lại dữ liệu
            }
        }
        void Sua()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                string id = txbID.Text;
                tb_Phong phong = db.tb_Phong.Find(id);
                
                int loai = (cmbLoaiP.SelectedValue as tb_LoaiPhong).IDLoaiP;
                int trangthai = (cmbTrangThai.SelectedValue as tb_TrangThai).IDTrangThai;
                phong.TenP = txbTenP.Text;
                phong.IDLoaiP = loai;
                phong.IDTrangThai = trangthai;
                db.SaveChanges();
                MessageBox.Show("Sửa Phòng Thành Công");
                loadData();
            }
        }


        void ResetID()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                var allRooms = db.tb_Phong.OrderBy(p => p.IDPhong).ToList();
                List<tb_Phong> reorderedRooms = new List<tb_Phong>();
                int newId = 1;

                foreach (var room in allRooms)
                {
                    tb_Phong newRoom = new tb_Phong
                    {
                        // IDPhong là khóa chính và không thể thay đổi
                        IDPhong = newId.ToString(), // Không thể sửa đổi IDPhong của phòng đã tồn tại
                        TenP = room.TenP,
                        IDLoaiP = room.IDLoaiP,
                        IDTrangThai = room.IDTrangThai,
                    };
                    reorderedRooms.Add(newRoom);
                    newId++;
                }

                // Xóa tất cả các phòng hiện có
                db.tb_Phong.RemoveRange(db.tb_Phong);
                // Thêm lại các phòng với ID mới
                db.tb_Phong.AddRange(reorderedRooms);
                db.SaveChanges();
             //   MessageBox.Show("Reset ID Phòng Thành Công");
            }
        }

    }



}
    

