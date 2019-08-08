using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace AYTO.UpdateFile
{
    public class UpdateFileDLL
    {
        SqlConnection updateFileConnection = new SqlConnection("server=ERU; Initial Catalog=deneme;Integrated Security=SSPI");

        //Belge Güncelleme Ekranını ilgili belgenin bilgileriyle dolduran metottdur. 
        public Tuple<string, string, string, string, string, string> TextGridFromOtherForm(string BelgeNo)
        {
            string fileTitle = "";
            string fileDirectory = "";
            string fileName = "";
            string fileExplain = "";
            string fileDate = "";
            string fileStatus = "";

            updateFileConnection.Close();
            string updateFileCmdText = "SELECT blg.belgeBasligi, blg.belgeDizini, blg.belgeAdi, blg.belgeAciklamasi, blg.eklenmeTarihi, drm.durumAdi FROM belgelerim AS blg INNER JOIN durumlar As drm ON blg.durumNo = drm.durumNo WHERE belgeNo = @belgeNo";
            using (SqlCommand updateFileCmd = new SqlCommand(updateFileCmdText, updateFileConnection))
            {
                updateFileCmd.Parameters.AddWithValue("@belgeNo", BelgeNo);
                updateFileConnection.Open();
                using (SqlDataReader updateFileReader = updateFileCmd.ExecuteReader())
                {
                    if (updateFileReader.Read())
                    {
                        fileTitle = updateFileReader["belgeBasligi"].ToString();
                        fileDirectory = updateFileReader["belgeDizini"].ToString();
                        fileName = updateFileReader["belgeAdi"].ToString();
                        fileExplain = updateFileReader["belgeAciklamasi"].ToString();
                        fileDate = updateFileReader["eklenmeTarihi"].ToString();
                        fileStatus = updateFileReader["durumAdi"].ToString();
                    }
                }
            }
            updateFileConnection.Close();

            var textGridTuple = new Tuple<string, string, string, string, string, string>(fileTitle, fileDirectory, fileName, fileExplain, fileDate, fileStatus);
            return textGridTuple;
        }
        //Kontrol veya Güncelleme öncesinde kullanıcının yetkisini kontrol eder.
        public string UserIdCheckForPermission(string fileDirectory, string fileName)
        {
            string returnValue = "";

            updateFileConnection.Close();
            string userCheckCmdText = "SELECT blg.belgeDizini, blg.belgeAdi, blg.kullaniciNo FROM belgelerim AS blg WHERE blg.belgeDizini = @belgeDizini AND blg.belgeAdi = @belgeAdi";
            using (SqlCommand userCheckCmd = new SqlCommand(userCheckCmdText, updateFileConnection))
            {
                userCheckCmd.Parameters.AddWithValue("@belgeDizini", fileDirectory);
                userCheckCmd.Parameters.AddWithValue("@belgeAdi", fileName);
                updateFileConnection.Open();
                using (SqlDataReader userCheckReader = userCheckCmd.ExecuteReader())
                {
                    //Böyle bir belge varsa
                    if (userCheckReader.Read())
                    {
                        returnValue = userCheckReader["kullaniciNo"].ToString();
                    }
                    else
                    {
                        returnValue = "nullValue";
                    }
                }
            }
            return returnValue;
        }
        /* Aynı pencerede belgenin bir kaç kez değişmesine karşı önlem olarak OldFileCheckMethod yaratılmıştır.
         * BelgeNo üzerinden belgeyi veritabanından kontrol ederek textboxlardaki ismi ile 
         * veritabanındaki ismi arasında karşılaştırma yapar
         */
        public string OldFileCheck(string BelgeNo, string fileName, string fileDirectory)
        {
            string returnValue = "";

            updateFileConnection.Close();
            string oldFileCmdText = "SELECT blg.belgeDizini, blg.belgeAdi FROM belgelerim AS blg WHERE blg.belgeNo = @belgeNo";
            using (SqlCommand oldFileCmd = new SqlCommand(oldFileCmdText, updateFileConnection))
            {
                oldFileCmd.Parameters.AddWithValue("@belgeNo", BelgeNo);
                updateFileConnection.Open();
                using (SqlDataReader oldFileReader = oldFileCmd.ExecuteReader())
                {
                    if (oldFileReader.Read())
                    {
                        //Belgeyi değiştirdiğinde yine aynı belgeyi eklediyse CheckFileMethod metotunu çalıştırmaya gerek duymadan diğer metota geçmektedir.
                        if (oldFileReader["belgeAdi"].ToString() == fileName && oldFileReader["belgeDizini"].ToString() == fileDirectory)
                        {
                            returnValue = "update";
                        }
                        else
                        {
                            returnValue = "check";
                        }
                    }
                }
            }
            updateFileConnection.Close();

            return returnValue;
        }
        //Belgeyi güncelleme işleminden önce bu belgenin var olup olmadığını kontrol eder. Yok ise updateFileMethod çalıştırır.
        public string CheckFile(string fileName, string fileDirectory)
        {
            string returnValue = "";

            updateFileConnection.Close();
            string checkCmdText = "SELECT blg.belgeAdi FROM belgelerim AS blg INNER JOIN kullanicilar AS klnc ON blg.kullaniciNo = klnc.kullaniciNo WHERE blg.belgeDizini = @belgeDizini AND blg.belgeAdi = @belgeAdi";
            using (SqlCommand checkCmd = new SqlCommand(checkCmdText, updateFileConnection))
            {
                checkCmd.Parameters.AddWithValue("@belgeDizini", fileDirectory);
                checkCmd.Parameters.AddWithValue("@belgeAdi", fileName);
                updateFileConnection.Open();
                using (SqlDataReader checkCmdReader = checkCmd.ExecuteReader())
                {
                    //Eğer böyle bir belge adı ya da dizini yok ise false koşulu çalışır. Varsa true koşulu çalışır ve belge güncelleme ya da değiştirme seçeneği sunar.
                    if (checkCmdReader.Read() == false)
                    {
                        returnValue = "false";
                    }
                    //Böyle bir belge adı veya dizini varsa true döndürerek işlemi iptal eder.
                    else
                    {
                        returnValue = "true";
                    }
                }
            }
            updateFileConnection.Close();

            return returnValue;
        }

        public void UpdateFile(string BelgeNo, string fileTitle, string fileName, string fileDirectory, string fileExplain, string comboboxSelectedItem, string updateDateTimePicker, int WhoUpdatedId)
        {
            updateFileConnection.Close();

            string updateFileCmdText = "UPDATE belgelerim SET belgeBasligi = @belgeBasligi, belgeAdi = @belgeAdi, belgeDizini = @belgeDizini, belgeAciklamasi = @belgeAciklamasi, durumNo = @durumNo, guncellenmeTarihi = @guncellenmeTarihi, sistemGuncellenmeTarihi = @sistemGuncellenmeTarihi, guncelleyenKisiNo = @guncelleyenKisiNo WHERE belgeNo = " + BelgeNo;
            using (SqlCommand updateFileCmd = new SqlCommand(updateFileCmdText, updateFileConnection))
            {
                updateFileCmd.Parameters.AddWithValue("@belgeBasligi", fileTitle);
                updateFileCmd.Parameters.AddWithValue("@belgeAdi", fileName);
                updateFileCmd.Parameters.AddWithValue("@belgeDizini", fileDirectory);
                updateFileCmd.Parameters.AddWithValue("@belgeAciklamasi", fileExplain);
                updateFileCmd.Parameters.AddWithValue("@durumNo", StatusNameTableValue(comboboxSelectedItem));
                updateFileCmd.Parameters.AddWithValue("@guncellenmeTarihi", DateTime.Parse(updateDateTimePicker));
                updateFileCmd.Parameters.AddWithValue("@sistemGuncellenmeTarihi", DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                updateFileCmd.Parameters.AddWithValue("@guncelleyenKisiNo", WhoUpdatedId);
                updateFileConnection.Open();
                updateFileCmd.ExecuteNonQuery();
            }
            updateFileConnection.Close();

        }
        /*Belge eklerken kullanılan parametrelerden birisi durumNo'dur. Tablodaki "Yeni" değerini döndürülmektedir.
         * Durum adı Yeni veya Güncellenmiş değilse OtherStatusNameTableValue metotuna geçiş yapar. 
         */
        public int StatusNameTableValue(string comboBoxSelectedItem)
        {
            /*Eğer belge yeni ise kayıt yapılırken Güncellenmiş olarak kaydedilir.
             *Güncellenme yapılırken Yeni durumu seçilirse yine Güncellenmiş olarak kaydedilir.
             * Combobox verilerinin indeksi 0 ile başladığından dolayı SelectedIndex+1 ile kaydedilmektedirler.
             */
            string comboBoxValue = "";
            if (comboBoxSelectedItem == "Yeni" || comboBoxSelectedItem == "Güncellenmiş")
            {
                comboBoxValue = "Güncellenmiş";
            }
            else
            {
                comboBoxValue = comboBoxSelectedItem;
            }
            updateFileConnection.Close();

            int returnStatusValue = 0;
            string statusNameCmdText = "SELECT durumNo FROM durumlar WHERE durumAdi = @durumAdi";
            using (SqlCommand statusNameCmd = new SqlCommand(statusNameCmdText, updateFileConnection))
            {
                statusNameCmd.Parameters.AddWithValue("@durumAdi", comboBoxValue);
                updateFileConnection.Open();
                using (SqlDataReader statusNameReader = statusNameCmd.ExecuteReader())
                {
                    if (statusNameReader.Read())
                    {
                        returnStatusValue = Convert.ToInt32(statusNameReader["durumNo"]);
                    }
                    else
                    {
                        updateFileConnection.Close();
                        using (SqlCommand addNewStatusCmd = new SqlCommand("INSERT INTO durumlar (durumAdi) VALUES ('Güncellenmiş')", updateFileConnection))
                        {
                            updateFileConnection.Open();
                            addNewStatusCmd.ExecuteNonQuery();
                            updateFileConnection.Close();
                            StatusNameTableValue(comboBoxSelectedItem);
                        }
                    }
                }
            }
            updateFileConnection.Close();
            return returnStatusValue;
        }
    }
}