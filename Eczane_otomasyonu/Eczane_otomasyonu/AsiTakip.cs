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
    public partial class AsiTakip : Form
    {
        private int currentRowIndex = 0; // Yazdırma işlemi için mevcut satır indeksi

        SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");

        sqlbaglantisi bag = new sqlbaglantisi();// sql bağlantı classımızdan nesne oluşturduk ve bağlantı için kullanacağız
        SqlCommand kmt = new SqlCommand(); //sql ekleme silme güncelleme listeleme işlemleri için sqlcommand nesnesi oluşturduk
        DataSet dtst = new DataSet();//datagridviewlere sql serverdaki tabloları aktarmak için kullanıyoruz.

        private Index mainForm;

        public AsiTakip(Index form)
        {
            InitializeComponent();
            this.mainForm = form;

            LoadTcKimlikToComboBoxEdit();
            PLoadTcKimlikToComboBoxEdit();
        }

        private void GeriPicBox_Click(object sender, EventArgs e)
        {
            if (mainForm.Panel2.Controls.Count > 0)
                mainForm.Panel2.Controls[0].Dispose();

            PSecimEkranı form = new PSecimEkranı(mainForm);
            mainForm.OpenFormInPanel(form);
        }
        public void Listele()
        {
            string komut = "select * from AsiBilgileri";
            SqlDataAdapter da = new SqlDataAdapter(komut, bag.baglan());
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];

        }
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            Listele();
        }

        public void Ekle()
        {
            try
            {
                // SQL bağlantısının açılması
                baglanti.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO AsiBilgileri (Asi_Adi, Etki_Suresi, Asi_Vurulma_Tarihi, Etkisi, Asi_Vuran_Personel_TC, Asi_Olan_Hasta_TC) " +
                                                "VALUES (@Asi_Adi, @Etki_Suresi, @Asi_Vurulma_Tarihi, @Etkisi, @Asi_Vuran_Personel_TC, @Asi_Olan_Hasta_TC)", baglanti);

                // Parametrelerin eklenmesi
                cmd.Parameters.AddWithValue("@Asi_Adi", textEdit13.Text);
                cmd.Parameters.AddWithValue("@Etki_Suresi", textEdit8.Text);
                cmd.Parameters.AddWithValue("@Etkisi", textEdit6.Text);
                cmd.Parameters.AddWithValue("@Asi_Vuran_Personel_TC", textEdit3.Text);
                cmd.Parameters.AddWithValue("@Asi_Olan_Hasta_TC", textEdit1.Text);

                // Asi_Vurulma_Tarihi parametresinin eklenmesi
                DateTime Asi_Vurulma_Tarihi;
                if (DateTime.TryParse(textEdit10.Text, out Asi_Vurulma_Tarihi))
                {
                    cmd.Parameters.AddWithValue("@Asi_Vurulma_Tarihi", Asi_Vurulma_Tarihi);
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir tarih girin!");
                    return; // Geçersiz tarih varsa metodu sonlandır
                }

                // SQL sorgusunun çalıştırılması
                cmd.ExecuteNonQuery();
                MessageBox.Show("Personel başarıyla eklendi!");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            Ekle();
        }

        private void textEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");
            SqlCommand kmt = new SqlCommand("SELECT Adi_Soyadi, Adresi FROM HastaBilgileri WHERE TC_Kimlik_No = @tc_kimlik", baglanti);
            kmt.Parameters.AddWithValue("@tc_kimlik", textEdit1.Text);

            try
            {
                baglanti.Open();
                SqlDataReader oku = kmt.ExecuteReader();

                if (oku.Read())
                {
                    textEdit2.Text = oku["Adi_Soyadi"].ToString();
                    
                }
                oku.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void LoadTcKimlikToComboBoxEdit()
        {
            // SQL bağlantısını tanımlıyoruz
            SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");

            // TC Kimlik Numaralarını çekmek için SQL sorgusu
            SqlCommand kmt = new SqlCommand("SELECT DISTINCT TC_Kimlik_No FROM HastaBilgileri", baglanti);

            try
            {
                // Bağlantıyı açıyoruz
                baglanti.Open();

                // Sorguyu çalıştırıyoruz
                SqlDataReader oku = kmt.ExecuteReader();

                // ComboBoxEdit kontrolünü temizliyoruz
                textEdit1.Properties.Items.Clear();

                // Verileri okuyor ve ComboBoxEdit'e ekliyoruz
                while (oku.Read())
                {
                    textEdit1.Properties.Items.Add(oku["TC_Kimlik_No"].ToString());
                }

                // Veri okuma işlemini kapatıyoruz
                oku.Close();
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj gösteriyoruz
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                // Bağlantıyı kapatıyoruz
                baglanti.Close();
            }
        }

        private void textEdit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");
            SqlCommand kmt = new SqlCommand("SELECT Adi_Soyadi, Adresi FROM Personel WHERE TC_Kimlik_No = @tc_kimlik", baglanti);
            kmt.Parameters.AddWithValue("@tc_kimlik", textEdit3.Text);

            try
            {
                baglanti.Open();
                SqlDataReader oku = kmt.ExecuteReader();

                if (oku.Read())
                {
                    textEdit5.Text = oku["Adi_Soyadi"].ToString();

                }
                oku.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }
        private void PLoadTcKimlikToComboBoxEdit()
        {
            // SQL bağlantısını tanımlıyoruz
            SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");

            // TC Kimlik Numaralarını çekmek için SQL sorgusu
            SqlCommand kmt = new SqlCommand("SELECT DISTINCT TC_Kimlik_No FROM Personel", baglanti);

            try
            {
                // Bağlantıyı açıyoruz
                baglanti.Open();

                // Sorguyu çalıştırıyoruz
                SqlDataReader oku = kmt.ExecuteReader();

                // ComboBoxEdit kontrolünü temizliyoruz
                textEdit3.Properties.Items.Clear();

                // Verileri okuyor ve ComboBoxEdit'e ekliyoruz
                while (oku.Read())
                {
                    textEdit3.Properties.Items.Add(oku["TC_Kimlik_No"].ToString());
                }

                // Veri okuma işlemini kapatıyoruz
                oku.Close();
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj gösteriyoruz
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                // Bağlantıyı kapatıyoruz
                baglanti.Close();
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
                    string barkod = dataGridView1.CurrentRow.Cells["Asi_ID"].Value.ToString();

                    // SQL komutunu oluştur
                    string komut = "DELETE FROM AsiBilgileri WHERE Asi_ID = @Asi_ID";
                    SqlCommand cmd = new SqlCommand(komut, baglanti);

                    // Parametreyi ekle
                    cmd.Parameters.AddWithValue("@Asi_ID", barkod);

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
            Sil();
        }
        public void Güncelle()
        {
            try
            {
                // SQL sorgusu
                string query = "UPDATE AsiBilgileri SET " +
                    
                   "Asi_Adi=@Asi_Adi, " +
                   "Etki_Suresi=@Etki_Suresi, " +
                   "Asi_Vurulma_Tarihi=@Asi_Vurulma_Tarihi, " +
                   "Etkisi=@Etkisi, " +
                   "Asi_Vuran_Personel_TC=@Asi_Vuran_Personel_TC, " +
                   "Asi_Olan_Hasta_TC=@Asi_Olan_Hasta_TC " ;

                using (SqlCommand cmd = new SqlCommand(query, baglanti))
                {
                    // Parametrelerin eklenmesi
                    
                    cmd.Parameters.AddWithValue("@Asi_Adi", textEdit13.Text);
                    cmd.Parameters.AddWithValue("@Etkisi", textEdit6.Text);
                    cmd.Parameters.AddWithValue("@Asi_Vuran_Personel_TC", textEdit3.Text);
                    cmd.Parameters.AddWithValue("@Asi_Olan_Hasta_TC", textEdit1.Text);
                    //Etki_Suresi
                    if (int.TryParse(textEdit8.Text, out int Etki_Suresi))
                    {
                        cmd.Parameters.AddWithValue("@Etki_Suresi", Etki_Suresi);
                    }
                    else
                    {
                        MessageBox.Show("Kutu sayısı geçerli bir sayı olmalıdır.");
                        return;
                    }
 
                    // Asi_Vurulma_Tarihi
                    if (DateTime.TryParse(textEdit10.Text, out DateTime Asi_Vurulma_Tarihi))
                    {
                        cmd.Parameters.AddWithValue("@Asi_Vurulma_Tarihi", Asi_Vurulma_Tarihi);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Asi_Vurulma_Tarihi", DBNull.Value);
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
                
                textEdit13.Text = dataGridView1.CurrentRow.Cells["Asi_Adi"].Value.ToString();
                textEdit8.Text = dataGridView1.CurrentRow.Cells["Etki_Suresi"].Value.ToString();
                textEdit10.Text = dataGridView1.CurrentRow.Cells["Asi_Vurulma_Tarihi"].Value.ToString();
                textEdit6.Text = dataGridView1.CurrentRow.Cells["Etkisi"].Value.ToString();
                textEdit3.Text = dataGridView1.CurrentRow.Cells["Asi_Vuran_Personel_TC"].Value.ToString();
                textEdit1.Text = dataGridView1.CurrentRow.Cells["Asi_Olan_Hasta_TC"].Value.ToString();
                

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

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

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

        private void simpleButton9_Click(object sender, EventArgs e)
        {
            dtst.Clear();
            //dataseti temizledik
            SqlDataAdapter adtr = new SqlDataAdapter("select * From AsiBilgileri where Asi_Vuran_Personel_TC='" + textEdit3.Text + "'", bag.baglan());
            //tcye göre listeleme komutumuzu çalıştırdık
            adtr.Fill(dtst, "AsiBilgileri");
            dataGridView1.DataMember = "AsiBilgileri";
            dataGridView1.DataSource = dtst;
            adtr.Dispose();
            //tablolarımızı doldurduk
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // seçilen satırın tamamını seçmesini sağladık ve aşşağıdaki kodlarla kolonlardaki başlıkları düzenledik
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Aşı Adı";
            dataGridView1.Columns[2].HeaderText = "Etki Süresi";
            dataGridView1.Columns[3].HeaderText = "Aşı vurulma Tarihi";
            dataGridView1.Columns[4].HeaderText = "Etkisi";
            dataGridView1.Columns[5].HeaderText = "Aşı Vuran TC";
            dataGridView1.Columns[6].HeaderText = "Aşı Vurulan TC";
        }

        private void simpleButton10_Click(object sender, EventArgs e)
        {
            dtst.Clear();
            //dataseti temizledik
            SqlDataAdapter adtr = new SqlDataAdapter("select * From AsiBilgileri where Asi_Olan_Hasta_TC='" + textEdit1.Text + "'", bag.baglan());
            //tcye göre listeleme komutumuzu çalıştırdık
            adtr.Fill(dtst, "AsiBilgileri");
            dataGridView1.DataMember = "AsiBilgileri";
            dataGridView1.DataSource = dtst;
            adtr.Dispose();
            //tablolarımızı doldurduk
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // seçilen satırın tamamını seçmesini sağladık ve aşşağıdaki kodlarla kolonlardaki başlıkları düzenledik
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Aşı Adı";
            dataGridView1.Columns[2].HeaderText = "Etki Süresi"; 
            dataGridView1.Columns[3].HeaderText = "Aşı vurulma Tarihi"; 
            dataGridView1.Columns[4].HeaderText = "Etkisi";
            dataGridView1.Columns[5].HeaderText = "Aşı Vuran TC"; 
            dataGridView1.Columns[6].HeaderText = "Aşı Vurulan TC";
        }
    }
}
