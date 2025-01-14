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
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Eczane_otomasyonu
{
    public partial class HastaTakip : Form
    {
        private Index mainForm;

        public HastaTakip(Index form)
        {
            InitializeComponent();
            this.mainForm = form;
            LoadTcKimlikToComboBoxEdit();
        }

        private void GeriPicBox_Click(object sender, EventArgs e)
        {
            if (mainForm.Panel2.Controls.Count > 0)
                mainForm.Panel2.Controls[0].Dispose();

            PSecimEkranı form = new PSecimEkranı(mainForm);
            mainForm.OpenFormInPanel(form);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");
            string query = "SELECT Ilac_Barkod, Ilac_Kullanimi, Kullanim_Sekli FROM HastaBilgileri WHERE TC_Kimlik_No = @tc_kimlik";
            SqlDataAdapter adtr = new SqlDataAdapter(query, baglanti);
            adtr.SelectCommand.Parameters.AddWithValue("@tc_kimlik", textEdit1.Text);

            DataTable dt = new DataTable();

            try
            {
                adtr.Fill(dt);
                dataGridView1.DataSource = dt;

                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.BackgroundColor = Color.White;
                dataGridView1.RowHeadersVisible = false;

                dataGridView1.Columns[0].HeaderText = "İlaç Barkod";
                dataGridView1.Columns[1].HeaderText = "İlaç Kullanımı";
                dataGridView1.Columns[2].HeaderText = "Kullanım Şekli";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
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
                    label4.Text = oku["Adi_Soyadi"].ToString();
                    textBox1.Text = oku["Adresi"].ToString();
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

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");
            string query = "SELECT Ilac_Kullanimi FROM HastaBilgileri WHERE TC_Kimlik_No = @tc_kimlik";
            SqlDataAdapter adtr = new SqlDataAdapter(query, baglanti);
            adtr.SelectCommand.Parameters.AddWithValue("@tc_kimlik", textEdit1.Text);

            DataTable dt = new DataTable();

            try
            {
                adtr.Fill(dt);
                dataGridView1.DataSource = dt;

                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.BackgroundColor = Color.White;
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.Columns[0].HeaderText = "İlaç Kullanımı";
                this.dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");
            string query = "SELECT Kullanim_Sekli FROM HastaBilgileri WHERE TC_Kimlik_No = @tc_kimlik";
            SqlDataAdapter adtr = new SqlDataAdapter(query, baglanti);
            adtr.SelectCommand.Parameters.AddWithValue("@tc_kimlik", textEdit1.Text);

            DataTable dt = new DataTable();

            try
            {
                adtr.Fill(dt);
                dataGridView1.DataSource = dt;

                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.BackgroundColor = Color.White;
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.Columns[0].HeaderText = "Kullanım Şekli";
                this.dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
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

        private void simpleButton4_Click(object sender, EventArgs e)
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
    }
}
