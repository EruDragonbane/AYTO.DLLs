using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace AYTO.AdminPage
{
    public class AdminPageDLL
    {
        SqlConnection adminDllConnection = new SqlConnection("server = ERU; Initial Catalog = deneme; Integrated Security = SSPI");

        //Kullanıcı kaydı için görev şart olduğundan dolayı bu iki tablonun içerik kontrolü yapılır
        public string CheckDataBaseTablePosition()
        {
            string returnValue = "";

            adminDllConnection.Close();

            string checkDtCmdText = "SELECT grv.gorevAdi FROM gorevler AS grv";
            SqlCommand checkDtCmd = new SqlCommand(checkDtCmdText, adminDllConnection);
            adminDllConnection.Open();
            SqlDataReader checkDtReader = checkDtCmd.ExecuteReader();
            if (checkDtReader.Read() == false)
            {
                returnValue = "false";
            }
            else
            {
                returnValue = "true";
            }
            checkDtReader.Close();
            adminDllConnection.Close();
            return returnValue;

        }
        //Belge Güncelleme
        public Tuple<string, string, int> AdminUpdateFile(string currentCellValue)
        {
            string returnValue = "";
            string selectedRowFileNo = "";
            int selectedRowUserNo = 0;

            adminDllConnection.Close();

            string selectedFileCmdText = "SELECT blg.belgeNo, klnc.kullaniciNo FROM belgelerim AS blg INNER JOIN kullanicilar AS klnc ON blg.kullaniciNo = klnc.kullaniciNo WHERE blg.belgeAdi = @belgeAdi";
            SqlCommand selectedFileCmd = new SqlCommand(selectedFileCmdText, adminDllConnection);
            selectedFileCmd.Parameters.AddWithValue("@belgeAdi", currentCellValue);
            adminDllConnection.Open();
            SqlDataReader selectedFileReader = selectedFileCmd.ExecuteReader();
            if (selectedFileReader.Read())
            {
                selectedRowFileNo = selectedFileReader["belgeNo"].ToString();
                selectedRowUserNo = Convert.ToInt32(selectedFileReader["kullaniciNo"].ToString());
                returnValue = "true";
            }
            else
            {
                returnValue = "false";
            }
            selectedFileReader.Close();
            adminDllConnection.Close();

            var updateFileTuple = new Tuple<string, string, int>(returnValue, selectedRowFileNo, selectedRowUserNo);
            return updateFileTuple;
        }
        //Kullanıcı, Durum, Görev Güncelleme
        public Tuple<string, string> AdminUpdateCommon(string curretCellValue, string columnName, string tableName, string columnSecondName)
        {
            string returnValue = "";
            string selectedRowColumnDataNo = "";

            adminDllConnection.Close();

            string selectedColumnCmdText = "SELECT " + columnName + "No FROM " + tableName + " WHERE " + columnName + columnSecondName + " = @hucreDegeri";
            SqlCommand selectedColumnCmd = new SqlCommand(selectedColumnCmdText, adminDllConnection);
            selectedColumnCmd.Parameters.AddWithValue("@hucreDegeri", curretCellValue);
            adminDllConnection.Open();
            SqlDataReader selectedColumnReader = selectedColumnCmd.ExecuteReader();
            if (selectedColumnReader.Read())
            {
                selectedRowColumnDataNo = selectedColumnReader[columnName + "No"].ToString();
                returnValue = "true";
            }
            else
            {
                returnValue = "false";
            }
            selectedColumnReader.Close();
            adminDllConnection.Close();

            var updateCommonTuple = new Tuple<string, string>(returnValue, selectedRowColumnDataNo);
            return updateCommonTuple;
        }
        //Verilerin silinmesi öncesi bu veriler başka tablolara taşınır.
        public void InsertDataToDatabaseBeforeDelete(string currentCellValue, string tableName, int AdminUserId)
        {
            string deletedDataCmdText = "";
            string deletedByCmdText = "";
            adminDllConnection.Close();
            if (tableName == "kullanicilar")
            {

                deletedDataCmdText = "INSERT INTO silinenKullanicilar (silinenKullanicino, silinenKullaniciAdi, silinenKullaniciSoyadi, silinenKullaniciGiris, silinenSistemKayitTarihi, silinenKullaniciKurumu, silinenGorevNo, silinenYetkiNo) SELECT kullaniciNo, kullaniciAdi, kullaniciSoyadi, kullaniciGiris, sistemKayitTarihi, kullaniciKurumu, gorevNo, yetkiNo FROM kullanicilar WHERE kullaniciNo = @gelenVeri";
                deletedByCmdText = "UPDATE silinenKullanicilar SET silenKisi = @silenKisi WHERE silinenKullaniciNo = @gelenVeri";
            }
            else if (tableName == "belgelerim")
            {
                deletedDataCmdText = "INSERT INTO silinenBelgeler (silinenKullaniciNo, silinenBelgeNo, silinenBelgeBasligi, silinenBelgeAdi, silinenBelgeDizini, silinenBelgeVeriTipiveAdi, silinenBelgeServerDizini, silinenBelgeAciklamasi, silinenEklenmeTarihi, silinenSistemEklenmeTarihi, silinenGuncellenmeTarihi, silinenSistemGuncellenmeTarihi, silinenGuncelleyenKisiNo, silinenDurumNo) SELECT kullaniciNo, belgeNo, belgeBasligi, belgeAdi, belgeDizini, belgeVeriTipiveAdi, belgeServerDizini, belgeAciklamasi, eklenmeTarihi, sistemEklenmeTarihi, guncellenmeTarihi, sistemGuncellenmeTarihi, guncelleyenKisiNo, durumNo FROM belgelerim WHERE belgeAdi = @gelenVeri";
                deletedByCmdText = "UPDATE silinenBelgeler SET silenKisi = @silenKisi WHERE silinenBelgeAdi = @gelenVeri";
            }
            else
            {
                deletedDataCmdText = "";
            }
            SqlCommand deletedDataCmd = new SqlCommand(deletedDataCmdText, adminDllConnection);
            deletedDataCmd.Parameters.AddWithValue("@gelenVeri", currentCellValue);
            adminDllConnection.Open();
            deletedDataCmd.ExecuteNonQuery();
            adminDllConnection.Close();

            SqlCommand deletedByCmd = new SqlCommand(deletedByCmdText, adminDllConnection);
            deletedByCmd.Parameters.AddWithValue("@silenKisi", AdminUserId);
            deletedByCmd.Parameters.AddWithValue("@gelenVeri", currentCellValue);
            adminDllConnection.Open();
            deletedByCmd.ExecuteNonQuery();
            adminDllConnection.Close();
        }
        //Verilerin ilişkisinin kontrolü
        public Tuple<string, string> RelationShipCheckBeforeDelete(string currentCellValue, string tableName, string datagridColumnName)
        {
            string returnValue = "";
            string datagridColumnValue = "";
            string checkBeforeDeleteCmdText = "";

            adminDllConnection.Close();

            if (tableName == "gorevler")
            {
                checkBeforeDeleteCmdText = "SELECT grv.gorevAdi FROM gorevler AS grv INNER JOIN kullanicilar AS klnc ON grv.gorevNo = klnc.gorevNo WHERE grv.gorevAdi = @veriAdi";
            }
            else if (tableName == "durumlar")
            {
                checkBeforeDeleteCmdText = "SELECT drm.durumAdi FROM durumlar AS drm INNER JOIN belgelerim AS blg ON drm.durumNo = blg.durumNo WHERE drm.durumAdi = @veriAdi";
            }
            else if (tableName == "kullanicilar")
            {
                checkBeforeDeleteCmdText = "SELECT klnc.kullaniciNo FROM kullanicilar AS klnc INNER JOIN belgelerim AS blg ON klnc.kullaniciNo = blg.kullaniciNo WHERE klnc.kullaniciNo = @veriAdi";
            }
            else
            {
                //İlişki yok döndür
                //adminPageDLL.InsertDataToDatabaseBeforeDelete(currentCellValue, tableName, AdminUserId);
                //RefreshLogAndDeleteEvent();
                //return;
            }
            SqlCommand checkBeforeDeleteCmd = new SqlCommand(checkBeforeDeleteCmdText, adminDllConnection);
            checkBeforeDeleteCmd.Parameters.AddWithValue("@veriAdi", currentCellValue);
            adminDllConnection.Open();
            SqlDataReader checkBeforeDeleteReader = checkBeforeDeleteCmd.ExecuteReader();
            if (checkBeforeDeleteReader.Read() == true)
            {
                datagridColumnValue = checkBeforeDeleteReader[datagridColumnName].ToString();
                returnValue = "true";
            }
            else
            {
                returnValue = "false";
            }

            var checkDeleteTuple = new Tuple<string, string>(returnValue, datagridColumnValue);
            return checkDeleteTuple;
        }
        //Verilerin Silinmesi
        public void DeleteDataAfterCheck(string currentCellValue, string tableName, string datagridColumnName)
        {
            adminDllConnection.Close();

            string deleteFileCmdText = "DELETE FROM " + tableName + " WHERE " + datagridColumnName + " = @veriAdi";
            SqlCommand deleteFileCmd = new SqlCommand(deleteFileCmdText, adminDllConnection);
            deleteFileCmd.Parameters.AddWithValue("@veriAdi", currentCellValue);
            adminDllConnection.Open();
            deleteFileCmd.ExecuteNonQuery();
            adminDllConnection.Close();
        }
        //Kullanıcının aktifliğini değiştirir.
        public void InactiveOrActiveUser(string currentCellValue, int columnValue)
        {
            adminDllConnection.Close();

            string updateUserActiveCmdText = "UPDATE kullanicilar SET kullaniciAktifligi = @kullaniciAktifligi WHERE kullaniciNo = @kullaniciNo";
            SqlCommand updateUserActiveCmd = new SqlCommand(updateUserActiveCmdText, adminDllConnection);
            updateUserActiveCmd.Parameters.AddWithValue("@kullaniciAktifligi", columnValue);
            updateUserActiveCmd.Parameters.AddWithValue("@kullaniciNo", currentCellValue);
            adminDllConnection.Open();
            updateUserActiveCmd.ExecuteNonQuery();
            updateUserActiveCmd.Dispose();
        }
    }
}
