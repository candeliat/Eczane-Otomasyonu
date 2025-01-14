using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace Eczane_otomasyonu
{
    class sqlbaglantisi
    {
        public SqlConnection baglan()
        {   //Data Source=CAN\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True
            //Data Source=can\SQLEXPRESS;Initial Catalog=DBEczaneOtomasyonu;Integrated Security=True
            SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");
            //sql bağlantı komutumuzu oluşturduk
            baglanti.Open();//bağlantıyı açtık
            SqlConnection.ClearPool(baglanti);
            SqlConnection.ClearAllPools();
            //geçmiş bağlantıları temizledik
            return (baglanti);//bağlantıyı döndürdük
        }
    }
}
