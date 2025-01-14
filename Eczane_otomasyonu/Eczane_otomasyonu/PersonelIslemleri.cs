using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Helpers;
using System.Data.SqlClient;
using Eczane_otomasyonu;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;


namespace Eczane_otomasyonu
{
    public partial class PersonelIslemleri : Form
    {
        private int currentRowIndex = 0; // Yazdırma işlemi için mevcut satır indeksi

        private Index mainForm;
        SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");
        sqlbaglantisi bag = new sqlbaglantisi();// sql bağlantı classımızdan nesne oluşturduk ve bağlantı için kullanacağız
        SqlCommand kmt = new SqlCommand(); //sql ekleme silme güncelleme listeleme işlemleri için sqlcommand nesnesi oluşturduk
        DataSet dtst = new DataSet();//datagridviewlere sql serverdaki tabloları aktarmak için kullanıyoruz.

        public PersonelIslemleri(Index form)
        {
            InitializeComponent();

            this.mainForm = form;

            // PrintDocument olayını bağla
            printDocument1.PrintPage += printDocument1_PrintPage;
        }
        
        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void GeriPicBox_Click(object sender, EventArgs e)
        {
            PSecimEkranı form = new PSecimEkranı(mainForm);
            this.Hide();
            mainForm.OpenFormInPanel(form); // Formu panel2 içinde aç
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            Listele();
        }

        public void Listele()
        {
            string komut = "select * from Personel";
            SqlDataAdapter da = new SqlDataAdapter(komut, bag.baglan());
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];

        }
        



            public void Ekle()
        {
            try
            {
                // SQL bağlantısının açılması
                baglanti.Open();

                SqlCommand cmd = new SqlCommand("insert into Personel(TC_Kimlik_No,Adi_Soyadi,Adresi,Dogum_Tarihi,Cinsiyeti,Hakkinda,Yaka_Kart_No,Telefon,Mail,Ise_Giris_Tarihi,Sigortasi,Maas,Fotograf)" +
                   "values(@tc_kimlik_no,@adi_soyadi,@adresi,@dogum_tarihi,@cinsiyeti,@hakkinda,@yaka_kart_no,@telefon,@mail,@ise_giris_tarihi,@sigortasi,@maas,@fotograf)", baglanti);

                // Parametrelerin eklenmesi
                cmd.Parameters.AddWithValue("@tc_kimlik_no", textEdit1.Text);
                cmd.Parameters.AddWithValue("@adi_soyadi", textEdit4.Text);
                cmd.Parameters.AddWithValue("@adresi", textEdit7.Text);

                // Doğum Tarihi
                DateTime dogumTarihi;
                if (DateTime.TryParse(textEdit2.Text, out dogumTarihi))
                {
                    cmd.Parameters.AddWithValue("@dogum_tarihi", dogumTarihi);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@dogum_tarihi", DBNull.Value);
                }

                // Cinsiyet
                string cinsiyet = textEdit5.Text.Trim();
                if (cinsiyet.Length > 10) // Veritabanındaki sütun uzunluğunu kontrol edin
                {
                    MessageBox.Show("Cinsiyet değeri çok uzun! Lütfen 10 karakteri aşmayın.");
                    return;
                }

                cmd.Parameters.AddWithValue("@cinsiyeti", textEdit5.Text);

                // Diğer alanlar
                cmd.Parameters.AddWithValue("@hakkinda", txtHakkinda.Text);
                cmd.Parameters.AddWithValue("@yaka_kart_no", textEdit13.Text);
                cmd.Parameters.AddWithValue("@telefon", textEdit8.Text);
                cmd.Parameters.AddWithValue("@mail", textEdit3.Text);

                // İşe Giriş Tarihi
                DateTime isegiristarihi;
                if (DateTime.TryParse(textEdit10.Text, out isegiristarihi))
                {
                    cmd.Parameters.AddWithValue("@ise_giris_tarihi", isegiristarihi);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ise_giris_tarihi", DBNull.Value);
                }

                // Sigorta
                cmd.Parameters.AddWithValue("@sigortasi", textEdit6.Text);

                // Maaş
                decimal maasValue;
                if (decimal.TryParse(textEdit12.Text, out maasValue))
                {
                    cmd.Parameters.AddWithValue("@maas", maasValue);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@maas", DBNull.Value);
                }

                // Fotoğraf
                if (PEFotograf.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        PEFotograf.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] photoData = ms.ToArray();
                        cmd.Parameters.AddWithValue("@fotograf", photoData);
                    }
                }
                else
                {
                    MessageBox.Show("Fotoğraf Eklenmedi!");
                    cmd.Parameters.AddWithValue("@fotograf", DBNull.Value);
                }

                // SQL sorgusunun çalıştırılması
                cmd.ExecuteNonQuery();
                MessageBox.Show("Personel başarıyla eklendi!");

                // Bağlantının kapatılması
                baglanti.Close();
            }
            catch (SqlException sqlEx)
            {
                // SQL hatalarını yakala
                MessageBox.Show($"Veritabanı hatası: {sqlEx.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException formatEx)
            {
                // Format hatalarını yakala
                MessageBox.Show($"Veri formatı hatası: {formatEx.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Diğer hataları yakala
                MessageBox.Show($"Beklenmeyen bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Ekle();
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
                    string komut = "DELETE FROM Personel WHERE TC_Kimlik_No = @tcKimlikNo";
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

        public void Güncelle()
        {

            try
            {
                string query = "UPDATE Personel SET " +
                               "TC_Kimlik_No=@tcKimlikNo, " +
                               "Adi_Soyadi=@adiSoyadi, " +
                               "Adresi=@adresi, " +
                               "Dogum_Tarihi=@dogumTarihi, " +
                               "Cinsiyeti=@cinsiyet, " +
                               "Hakkinda=@hakkinda, " +
                               "Yaka_Kart_No=@yakaKartNo, " +
                               "Telefon=@telefon, " +
                               "Mail=@mail, " +
                               "Ise_Giris_Tarihi=@iseGirisTarihi, " +
                               "Sigortasi=@sigorta, " +
                               "Maas=@maas, " +
                               "Fotograf=@fotograf " + // Fotoğrafı parametre olarak ekliyoruz
                               "WHERE TC_Kimlik_No=@tcKimlikNo";

                SqlCommand cmd = new SqlCommand(query, baglanti);
                cmd.Parameters.AddWithValue("@tcKimlikNo", textEdit1.Text);
                cmd.Parameters.AddWithValue("@adiSoyadi", textEdit4.Text);
                cmd.Parameters.AddWithValue("@adresi", textEdit7.Text);
                cmd.Parameters.AddWithValue("@dogumTarihi", textEdit2.Text);
                cmd.Parameters.AddWithValue("@cinsiyet", textEdit5.Text);
                cmd.Parameters.AddWithValue("@hakkinda", txtHakkinda.Text);
                cmd.Parameters.AddWithValue("@yakaKartNo", textEdit13.Text);
                cmd.Parameters.AddWithValue("@telefon", textEdit8.Text);
                cmd.Parameters.AddWithValue("@mail", textEdit3.Text);
                cmd.Parameters.AddWithValue("@iseGirisTarihi", textEdit10.Text);
                cmd.Parameters.AddWithValue("@sigorta", textEdit6.Text);
                cmd.Parameters.AddWithValue("@maas", textEdit12.Text);

                baglanti.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Güncelleme sırasında bir hata oluştu: {ex.Message}");
            }
            finally
            {
                baglanti.Close();
                Listele();
            }


        }
        public void BilgiCek()
        {
            try
            {
                textEdit1.Text = dataGridView1.CurrentRow.Cells["TC_Kimlik_No"].Value.ToString();
                textEdit4.Text = dataGridView1.CurrentRow.Cells["Adi_Soyadi"].Value.ToString();
                textEdit7.Text = dataGridView1.CurrentRow.Cells["Adresi"].Value.ToString();
                textEdit2.Text = dataGridView1.CurrentRow.Cells["Dogum_Tarihi"].Value.ToString();
                textEdit5.Text = dataGridView1.CurrentRow.Cells["Cinsiyeti"].Value.ToString();
                txtHakkinda.Text = dataGridView1.CurrentRow.Cells["Hakkinda"].Value.ToString();
                textEdit13.Text = dataGridView1.CurrentRow.Cells["Yaka_Kart_No"].Value.ToString();
                textEdit8.Text = dataGridView1.CurrentRow.Cells["Telefon"].Value.ToString();
                textEdit3.Text = dataGridView1.CurrentRow.Cells["Mail"].Value.ToString();
                textEdit10.Text = dataGridView1.CurrentRow.Cells["Ise_Giris_Tarihi"].Value.ToString();
                textEdit6.Text = dataGridView1.CurrentRow.Cells["Sigortasi"].Value.ToString();
                textEdit12.Text = dataGridView1.CurrentRow.Cells["Maas"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bilgiler çekilirken bir hata oluştu: {ex.Message}");
                return; // Eğer hata oluşursa güncelleme işlemini başlatma
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                BilgiCek();
                Güncelle();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Beklenmeyen bir hata oluştu: {ex.Message}");
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