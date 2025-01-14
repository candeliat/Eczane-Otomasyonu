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
    public partial class HastaKayitIslemleri : Form
    {
        private int currentRowIndex = 0; // Yazdırma işlemi için mevcut satır indeksi

        //Data Source=CAN\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True
        private Index mainForm;
        SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");
        sqlbaglantisi bag = new sqlbaglantisi();// sql bağlantı classımızdan nesne oluşturduk ve bağlantı için kullanacağız
        SqlCommand kmt = new SqlCommand(); //sql ekleme silme güncelleme listeleme işlemleri için sqlcommand nesnesi oluşturduk
        DataSet dtst = new DataSet();//datagridviewlere sql serverdaki tabloları aktarmak için kullanıyoruz.

        public HastaKayitIslemleri(Index form)
        {
            InitializeComponent();

            this.mainForm = form;
            // PrintDocument olayını bağla
            printDocument1.PrintPage += printDocument1_PrintPage;
        }

        private void GeriPicBox_Click(object sender, EventArgs e)
        {

            PSecimEkranı form = new PSecimEkranı(mainForm);
            this.Hide();
            mainForm.OpenFormInPanel(form); // Formu panel2 içinde aç


        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

       
        
        public void Listele()
        {
            string komut = "select * from HastaBilgileri";
            SqlDataAdapter da = new SqlDataAdapter(komut, bag.baglan());
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];

        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            Listele();
        }

        public void Sil()
        {
            // Silme işlemi için onay almak
            DialogResult onay = MessageBox.Show("Kaydı Silmek İstediğinizden Emin misiniz ?", "Onay Kutusu", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (onay == DialogResult.Yes)
            {
                try
                {
                    // TC Kimlik No'yu al
                    string TC = dataGridView1.CurrentRow.Cells["TC_Kimlik_No"].Value.ToString();

                    // SQL komutunu oluştur
                    string komut = "DELETE FROM HastaBilgileri WHERE TC_Kimlik_No = @tcKimlikNo";
                    SqlCommand cmd = new SqlCommand(komut, baglanti);

                    // Parametreyi ekle
                    cmd.Parameters.AddWithValue("@tcKimlikNo", TC);

                    // Bağlantıyı aç ve komutu çalıştır
                    baglanti.Open();
                    cmd.ExecuteNonQuery();
                    baglanti.Close();

                    // Listeyi güncelle
                    Listele();

                    MessageBox.Show("Kayıt başarıyla silindi.");
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("SQL Hatası: " + ex.Message);
                }
                finally
                {
                    if (baglanti.State == ConnectionState.Open)
                        baglanti.Close(); // Bağlantıyı her durumda kapat
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Sil();
        }

        public void Ekle()
        {
            // Bağlantıyı aç
            baglanti.Open();

            // SQL Komutu
            SqlCommand cmd = new SqlCommand(
        "INSERT INTO IlacBilgileri ([TC_Kimlik_No], [Adi_Soyadi], [Dogum_Tarihi], [Cinsiyeti], [Sigorta_Turu], [Telefon], [Adresi], [Ilac_Barkod], [Ilac_Kullanimi], [Kullanim_Sekli]) " +
        "VALUES (@TC_Kimlik_No, @Adi_Soyadi, @Dogum_Tarihi, @Cinsiyeti, @Sigorta_Turu, @Telefon, @Adresi, @Ilac_Barkod, @Ilac_Kullanimi, @Kullanim_Sekli)",
        baglanti);

            // Parametrelerin eklenmesi
            cmd.Parameters.AddWithValue("@TC_Kimlik_No", textEdit13.Text);
            cmd.Parameters.AddWithValue("@Adi_Soyadi", textEdit8.Text);

            // Doğum Tarihi (Tarih formatını kontrol etme)
            if (DateTime.TryParse(textEdit10.Text, out DateTime DogumTarihi))
            {
                cmd.Parameters.AddWithValue("@Dogum_Tarihi", DogumTarihi);
            }
            else
            {
                cmd.Parameters.AddWithValue("@Dogum_Tarihi", DBNull.Value);
            }

            cmd.Parameters.AddWithValue("@Cinsiyeti", textEdit6.Text);
            cmd.Parameters.AddWithValue("@Sigorta_Turu", textEdit3.Text);
            cmd.Parameters.AddWithValue("@Telefon", textEdit12.Text);
            cmd.Parameters.AddWithValue("@Adresi", textEdit7.Text);

            // İlac Barkod (int değer kontrolü)
            if (int.TryParse(textEdit1.Text, out int IlacBarkod))
            {
                cmd.Parameters.AddWithValue("@Ilac_Barkod", IlacBarkod);
            }
            else
            {
                MessageBox.Show("İlaç barkodunu sayı olarak giriniz!");
                return;
            }

            cmd.Parameters.AddWithValue("@Ilac_Kullanimi", textEdit2.Text);
            cmd.Parameters.AddWithValue("@Kullanim_Sekli", textEdit4.Text);

            // Sorguyu çalıştır
            cmd.ExecuteNonQuery();

            // Bilgilendirme mesajı
            MessageBox.Show("Kayıt başarıyla eklendi!");

            // Bağlantıyı kapat
            baglanti.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Ekle();
        }

        public void Güncelle()
        {

            // SQL sorgusu
            string query = "UPDATE HastaBilgileri SET " +
               "TC_Kimlik_No=@TC_Kimlik_No, " +
               "Adi_Soyadi=@Adi_Soyadi, " +
               "Dogum_Tarihi=@Dogum_Tarihi, " +
               "Cinsiyeti=@Cinsiyeti, " +
               "Sigorta_Turu=@Sigorta_Turu, " +
               "Telefon=@Telefon, " +
               "Adresi=@Adresi, " +
               "Ilac_Barkod=@Ilac_Barkod " +
               "Ilac_Kullanimi=@Ilac_Kullanimi " +
               "Kullanim_Sekli=@Kullanim_Sekli ";

            using (SqlCommand cmd = new SqlCommand(query, baglanti))
            {
                // Parametrelerin eklenmesi
                cmd.Parameters.AddWithValue("@TC_Kimlik_No", textEdit13.Text);
                cmd.Parameters.AddWithValue("@Adi_Soyadi", textEdit8.Text);
                cmd.Parameters.AddWithValue("@Cinsiyeti", textEdit6.Text);
                cmd.Parameters.AddWithValue("@Sigorta_Turu", textEdit3.Text);
                cmd.Parameters.AddWithValue("@Telefon", textEdit12.Text);
                cmd.Parameters.AddWithValue("@Adresi", textEdit7.Text);
                cmd.Parameters.AddWithValue("@Ilac_Kullanimi", textEdit2.Text);
                cmd.Parameters.AddWithValue("@Kullanim_Sekli", textEdit4.Text);

                // Kutu Sayısı
                if (int.TryParse(textEdit1.Text, out int Ilac_Barkod))
                {
                    cmd.Parameters.AddWithValue("@Ilac_Barkod", Ilac_Barkod);
                }
                else
                {
                    MessageBox.Show("Ilac Barkod geçerli bir sayı olmalıdır.");
                    return;
                }



                // Dogum_Tarihi
                if (DateTime.TryParse(textEdit10.Text, out DateTime Dogum_Tarihi))
                {
                    cmd.Parameters.AddWithValue("@Dogum_Tarihi", Dogum_Tarihi);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Dogum_Tarihi", DBNull.Value);
                }

                // Veritabanı bağlantısı açma ve komutu yürütme
                baglanti.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Güncelleme işlemi başarıyla tamamlandı.");
            }

            


            // Bağlantıyı kapat
            if (baglanti.State == System.Data.ConnectionState.Open)
            {
                baglanti.Close();
            }
            Listele();
        }

        // Listeleme işlemi
        
    
        public void BilgiCek()
        {
            try
            {
                // DataGridView'den verileri çekme
                textEdit13.Text = dataGridView1.CurrentRow.Cells["TC_Kimlik_No"].Value.ToString();
                textEdit8.Text = dataGridView1.CurrentRow.Cells["Adi_Soyadi"].Value.ToString();
                textEdit10.Text = dataGridView1.CurrentRow.Cells["Dogum_Tarihi"].Value.ToString();
                textEdit6.Text = dataGridView1.CurrentRow.Cells["Cinsiyeti"].Value.ToString();
                textEdit3.Text = dataGridView1.CurrentRow.Cells["Sigorta_Turu"].Value.ToString();
                textEdit12.Text = dataGridView1.CurrentRow.Cells["Telefon"].Value.ToString();
                textEdit7.Text = dataGridView1.CurrentRow.Cells["Adresi"].Value.ToString();
                textEdit1.Text = dataGridView1.CurrentRow.Cells["Ilac_Barkod"].Value.ToString();
                textEdit2.Text = dataGridView1.CurrentRow.Cells["Ilac_Kullanimi"].Value.ToString();
                textEdit4.Text = dataGridView1.CurrentRow.Cells["Kullanim_Sekli"].Value.ToString();

                // Güncelleme butonunun metni
                simpleButton3.Text = "Güncelle";
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj göster
                MessageBox.Show($"Bilgi çekilirken bir hata oluştu: {ex.Message}");
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (simpleButton3.Text == "Güncelle")
            {
                Güncelle();
            }
            BilgiCek();

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

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

        private void ExportToPdf(DataGridView dataGridView, string filePath)
        {
            try
            {

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
    }
}
