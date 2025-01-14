using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Eczane_otomasyonu
{
    public partial class İlacBilgileri : Form
    {
        private int currentRowIndex = 0; // Yazdırma işlemi için mevcut satır indeksi

        SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");
        sqlbaglantisi bag = new sqlbaglantisi();// sql bağlantı classımızdan nesne oluşturduk ve bağlantı için kullanacağız
        SqlCommand kmt = new SqlCommand(); //sql ekleme silme güncelleme listeleme işlemleri için sqlcommand nesnesi oluşturduk
        DataSet dtst = new DataSet();//datagridviewlere sql serverdaki tabloları aktarmak için kullanıyoruz.

        private Index mainForm;

        public İlacBilgileri(Index form)
        {
            try
            {
                InitializeComponent();
                this.mainForm = form;

                // PrintDocument olayını bağla
                printDocument1.PrintPage += printDocument1_PrintPage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Form başlatılırken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GeriPicBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (mainForm.Panel2.Controls.Count > 0)
                    mainForm.Panel2.Controls[0].Dispose();

                PSecimEkranı form = new PSecimEkranı(mainForm);
                mainForm.OpenFormInPanel(form);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Geri dönüş sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Listele()
        {

            try
            {
                string komut = "select * from IlacBilgileri";
                SqlDataAdapter da = new SqlDataAdapter(komut, bag.baglan());
                DataSet ds = new DataSet();
                da.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Veritabanı hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Listeleme sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                Listele();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Listeleme işlemi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Ekle()
        {
            try
            {
                // SQL bağlantısının açılması
                baglanti.Open();

                SqlCommand cmd = new SqlCommand("insert into IlacBilgileri(Barkod_No,Ilac_Adi,Uretici_Firma,Kutu_Sayisi,Fiyati,Teslim_Alan_Personel,Kullanim_Amaci,Yan_Etkileri,Son_Kullanma_Tarihi)" +
                   "values(@barkod_no,@ilac_adi,@uretici_firma,@kutu_sayisi,@fiyati,@teslim_alan_personel,@kullanim_amaci,@yan_etkileri,@son_kullanma_tarihi)", baglanti);

                // Parametrelerin eklenmesi
                cmd.Parameters.AddWithValue("@barkod_no", textEdit13.Text);
                cmd.Parameters.AddWithValue("@ilac_adi", textEdit8.Text);
                cmd.Parameters.AddWithValue("@uretici_firma", textEdit3.Text);
                cmd.Parameters.AddWithValue("@kutu_sayisi", textEdit10.Text);
                cmd.Parameters.AddWithValue("@fiyati", textEdit6.Text);
                cmd.Parameters.AddWithValue("@teslim_alan_personel", textEdit12.Text);
                cmd.Parameters.AddWithValue("@kullanim_amaci", textEdit1.Text);
                cmd.Parameters.AddWithValue("@yan_etkileri", textEdit4.Text);

                // Doğum Tarihi
                DateTime sonkullanmatarihi;
                if (DateTime.TryParse(textEdit2.Text, out sonkullanmatarihi))
                {
                    cmd.Parameters.AddWithValue("@son_kullanma_tarihi", sonkullanmatarihi);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@son_kullanma_tarihi", DBNull.Value);
                }

                // SQL sorgusunun çalıştırılması
                cmd.ExecuteNonQuery();
                MessageBox.Show("Personel başarıyla eklendi!");
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Veritabanı hatası: {sqlEx.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException formatEx)
            {
                MessageBox.Show($"Veri formatı hatası: {formatEx.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Beklenmeyen bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Bağlantının kapatılması
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                Ekle();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ekleme işlemi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Sil()
        {
            try
            {
                // Silme işlemi için onay almak
                DialogResult onay = MessageBox.Show("Kaydı Silmek İstediğinizden Emin misiniz ?", "Onay Kutusu", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (onay == DialogResult.Yes)
                {
                    // TC Kimlik No'yu al
                    string barkod = dataGridView1.CurrentRow.Cells["Barkod_No"].Value.ToString();

                    // SQL komutunu oluştur
                    string komut = "DELETE FROM IlacBilgileri WHERE Barkod_No = @barkod_no";
                    SqlCommand cmd = new SqlCommand(komut, baglanti);

                    // Parametreyi ekle
                    cmd.Parameters.AddWithValue("@barkod_no", barkod);

                    // Bağlantıyı aç ve komutu çalıştır
                    baglanti.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Kayıt başarıyla silindi.");

                    // Listeyi güncelle
                    Listele();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL Hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Beklenmeyen bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                    baglanti.Close();
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                Sil();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Silme işlemi sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Güncelle()
        {
            try
            {
                // SQL sorgusu
                string query = "UPDATE IlacBilgileri SET " +
                   "Ilac_Adi=@ilac_adi, " +
                   "Uretici_Firma=@uretici_firma, " +
                   "Kutu_Sayisi=@kutu_sayisi, " +
                   "Fiyati=@fiyati, " +
                   "Teslim_Alan_Personel=@teslim_alan_personel, " +
                   "Kullanim_Amaci=@kullanim_amaci, " +
                   "Yan_Etkileri=@yan_etkileri, " +
                   "Son_Kullanma_Tarihi=@son_kullanma_tarihi " +
                   "WHERE Barkod_No=@barkod_no";

                using (SqlCommand cmd = new SqlCommand(query, baglanti))
                {
                    // Parametrelerin eklenmesi
                    cmd.Parameters.AddWithValue("@barkod_no", textEdit13.Text);
                    cmd.Parameters.AddWithValue("@ilac_adi", textEdit8.Text);
                    cmd.Parameters.AddWithValue("@uretici_firma", textEdit3.Text);

                    // Kutu Sayısı
                    if (int.TryParse(textEdit10.Text, out int kutuSayisi))
                    {
                        cmd.Parameters.AddWithValue("@kutu_sayisi", kutuSayisi);
                    }
                    else
                    {
                        MessageBox.Show("Kutu sayısı geçerli bir sayı olmalıdır.");
                        return;
                    }

                    // Fiyat
                    if (decimal.TryParse(textEdit6.Text, out decimal fiyati))
                    {
                        cmd.Parameters.AddWithValue("@fiyati", fiyati);
                    }
                    else
                    {
                        MessageBox.Show("Fiyat geçerli bir sayı olmalıdır.");
                        return;
                    }

                    cmd.Parameters.AddWithValue("@teslim_alan_personel", textEdit12.Text);
                    cmd.Parameters.AddWithValue("@kullanim_amaci", textEdit1.Text);
                    cmd.Parameters.AddWithValue("@yan_etkileri", textEdit4.Text);

                    // Son Kullanma Tarihi
                    if (DateTime.TryParse(textEdit2.Text, out DateTime sonkullanmatarihi))
                    {
                        cmd.Parameters.AddWithValue("@son_kullanma_tarihi", sonkullanmatarihi);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@son_kullanma_tarihi", DBNull.Value);
                    }

                    // Veritabanı bağlantısı açma ve komutu yürütme
                    baglanti.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Güncelleme işlemi başarıyla tamamlandı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
            finally
            {
                // Bağlantıyı kapat
                if (baglanti.State == System.Data.ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }

            // Listeleme işlemi
            Listele();
        }
        public void BilgiCek()
        {


            try
            {
                textEdit13.Text = dataGridView1.CurrentRow.Cells["Barkod_No"].Value.ToString();
                textEdit8.Text = dataGridView1.CurrentRow.Cells["Ilac_Adi"].Value.ToString();
                textEdit3.Text = dataGridView1.CurrentRow.Cells["Uretici_Firma"].Value.ToString();
                textEdit10.Text = dataGridView1.CurrentRow.Cells["Kutu_Sayisi"].Value.ToString();
                textEdit6.Text = dataGridView1.CurrentRow.Cells["Fiyati"].Value.ToString();
                textEdit12.Text = dataGridView1.CurrentRow.Cells["Teslim_Alan_Personel"].Value.ToString();
                textEdit1.Text = dataGridView1.CurrentRow.Cells["Kullanim_Amaci"].Value.ToString();
                textEdit4.Text = dataGridView1.CurrentRow.Cells["Yan_Etkileri"].Value.ToString();
                textEdit2.Text = dataGridView1.CurrentRow.Cells["Son_Kullanma_Tarihi"].Value.ToString();

                simpleButton3.Text = "Güncelle";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bilgi çekme sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

            private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (simpleButton3.Text == "Güncelle")
                {
                    Güncelle();
                }
                BilgiCek();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void ExportToPdf(DataGridView dataGridView, string filePath)
        {
            try { 
            
                // PDF dosyası oluştur
                using (PdfWriter writer = new PdfWriter(filePath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);

                        // Tablo oluştur
                        Table table = new Table(UnitValue.CreatePercentArray(dataGridView.ColumnCount)).UseAllAvailableWidth();

                        // Başlıkları ekle
                        foreach (DataGridViewColumn column in dataGridView.Columns)
                        {
                            table.AddHeaderCell(new Cell().Add(new Paragraph(column.HeaderText)));
                        }

                        // Satırları ekle
                        foreach (DataGridViewRow row in dataGridView.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    string cellValue = cell.Value?.ToString() ?? string.Empty;
                                    table.AddCell(new Cell().Add(new Paragraph(cellValue)));
                                }
                            }
                        }

                        // Tabloyu PDF'e ekle
                        document.Add(table);
                    }
                }
            MessageBox.Show("PDF başarıyla oluşturuldu!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

}

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcıdan dosya adı alın
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Dosyası (*.pdf)|*.pdf",
                    Title = "PDF Dosyasını Kaydet"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToPdf(dataGridView1, saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            try
            {
                // PrintDocument yazıcısının geçerliliğini kontrol et
                if (!printDocument1.PrinterSettings.IsValid)
                {
                    MessageBox.Show("Geçerli bir yazıcı bulunamadı. Lütfen bir yazıcı yükleyin veya varsayılan yazıcı ayarlarını kontrol edin.",
                                    "Yazıcı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Yazdırma önizleme
                PrintPreviewDialog previewDialog = new PrintPreviewDialog
                {
                    Document = printDocument1,
                    Width = 800,
                    Height = 600
                };

                previewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yazdırma önizleme sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                // Başlangıç pozisyonu ve hücre ölçüleri
                int startX = 50;
                int startY = 50;
                int cellHeight = 30;
                int cellWidth = 100;

                // Başlıkları yazdır
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    e.Graphics.DrawRectangle(Pens.Black, startX + (i * cellWidth), startY, cellWidth, cellHeight);
                    e.Graphics.DrawString(dataGridView1.Columns[i].HeaderText, new Font("Arial", 10, FontStyle.Bold),
                        Brushes.Black, startX + (i * cellWidth), startY + 5);
                }

                // Satırları yazdır
                startY += cellHeight; // Başlıkların altına geç
                for (int rowIndex = currentRowIndex; rowIndex < dataGridView1.RowCount; rowIndex++)
                {
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];

                    for (int colIndex = 0; colIndex < dataGridView1.ColumnCount; colIndex++)
                    {
                        string cellValue = row.Cells[colIndex].Value?.ToString() ?? string.Empty;

                        e.Graphics.DrawRectangle(Pens.Black, startX + (colIndex * cellWidth), startY, cellWidth, cellHeight);
                        e.Graphics.DrawString(cellValue, new Font("Arial", 10), Brushes.Black,
                            startX + (colIndex * cellWidth), startY + 5);
                    }

                    startY += cellHeight;

                    // Sayfa sonu kontrolü
                    if (startY + cellHeight > e.MarginBounds.Height)
                    {
                        currentRowIndex = rowIndex + 1; // Bir sonraki sayfa için devam et
                        e.HasMorePages = true;
                        return;
                    }
                }

                // Yazdırma işlemi tamamlandı
                e.HasMorePages = false;
                currentRowIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yazdırma sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void simpleButton7_Click(object sender, EventArgs e)
        {
            try
            {
                // PrintDocument yazıcısının geçerliliğini kontrol et
                if (!printDocument1.PrinterSettings.IsValid)
                {
                    MessageBox.Show("Geçerli bir yazıcı bulunamadı. Lütfen bir yazıcı yükleyin veya varsayılan yazıcı ayarlarını kontrol edin.",
                                    "Yazıcı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // PrintPreviewDialog ile önizleme penceresini göster
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yazdırma önizleme sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            try
            {
                // Yazıcının geçerli olup olmadığını kontrol et
                if (!printDocument1.PrinterSettings.IsValid)
                {
                    MessageBox.Show("Geçerli bir yazıcı bulunamadı. Lütfen bir yazıcı yükleyin veya varsayılan yazıcı ayarlarını kontrol edin.",
                                    "Yazıcı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Yazıcı geçerliyse, PageSetupDialog'u ayarla ve göster
                pageSetupDialog1.Document = printDocument1;
                pageSetupDialog1.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sayfa ayarları sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }
    }
}
