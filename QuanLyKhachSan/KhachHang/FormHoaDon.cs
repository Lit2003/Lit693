using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyKhachSan.KhachHang
{
    public partial class FormHoaDon : Form
    {
        private PrintDocument printDocument = new PrintDocument();
        public FormHoaDon()
        {
            InitializeComponent();
            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);
        }

        private void FormHoaDon_Load(object sender, EventArgs e)
        {
           
        }
        public void DisplayInvoice(string invoiceID, string customerID,DateTime NgayDat, string songayo, string employeeID, float totalAmount, string note, string DVdadung, string TenP, float TienPhong, float TienDV)
        {
            txbIDHD.Text = invoiceID;
            txbIDKH.Text = customerID;
            txbIDNV.Text = employeeID;
            txbNote.Text = note;
            txbDV.Text = DVdadung;
            txbIDP.Text = TenP;
            txbTongTien.Text = totalAmount.ToString("N0");
            dtpNgayDat.Value = NgayDat;
            txbSoNgayO.Text = songayo;
            txbTongTienP.Text = TienPhong.ToString("N0");
            txbTongTienDV.Text = TienDV.ToString("N0");
           

        }

        private void button1_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }
        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Tạo một Font để in
            Font font = new Font("Arial", 12);

            // Vẽ nội dung hóa đơn lên trang in
            float yPos = 100; // Vị trí bắt đầu in từ đây
            float xOffset = 50; // Lề trái
            // Vẽ nội dung các control trên form
            e.Graphics.DrawString($"Hóa đơn số: {txbIDHD.Text}", font, Brushes.Black, xOffset, yPos);
            yPos += 20;
            e.Graphics.DrawString($"Khách hàng: {txbIDKH.Text}", font, Brushes.Black, xOffset, yPos);
            yPos += 20;
            e.Graphics.DrawString($"Nhân viên: {txbIDNV.Text}", font, Brushes.Black, xOffset, yPos);
            yPos += 20;
            e.Graphics.DrawString($"Ngày đặt: {dtpNgayDat.Value:dd/MM/yyyy}", font, Brushes.Black, xOffset, yPos);
            yPos += 20;
            e.Graphics.DrawString($"Ngày trả phòng: {dtpNgayTra.Value:dd/MM/yyyy}", font, Brushes.Black, xOffset, yPos);
            yPos += 20;        
            e.Graphics.DrawString($"Dịch vụ đã dùng: {txbDV.Text}", font, Brushes.Black, xOffset, yPos);
            yPos += 20;
            e.Graphics.DrawString($"Phòng đã đặt: {txbIDP.Text}", font, Brushes.Black, xOffset, yPos);
            yPos += 20;        
            e.Graphics.DrawString($"Tổng tiền phòng: {txbTongTienP.Text:N0}", font, Brushes.Black, xOffset, yPos);
            yPos += 20;
            e.Graphics.DrawString($"Tổng tiền dịch vụ: {txbTongTienDV.Text:N0,numberFormat}", font, Brushes.Black, xOffset, yPos);
            yPos += 20;
            e.Graphics.DrawString($"Ghi chú: {txbNote.Text}", font, Brushes.Black, xOffset, yPos);
            yPos += 20;
            e.Graphics.DrawString($"Tổng tiền: {txbTongTien.Text:N0} VNĐ", font, Brushes.Black, xOffset, yPos);
        }

        private void dtpNgayDat_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
