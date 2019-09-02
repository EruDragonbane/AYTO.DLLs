using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace AYTO.NewFile
{
    public class NewFileDLL
    {
        SqlConnection newFileConnection = new SqlConnection("Data Source = ERU; Initial Catalog = deneme; Integrated Security = SSPI");
        //Belge eklerken kullanılan parametrelerden birisi durumNo'dur. Tablodaki "Yeni" değeri döndürülmektedir.
        public int StatusNameTableValue()
        {
            int returnStatusValue = 0;
            string firstStatus = "Yeni";
            newFileConnection.Close();
            try
            {
                string statusNameCmdText = "SELECT drm.durumNo FROM durumlar AS drm WHERE drm.durumAdi = @durumAdi";
                using (SqlCommand statusNameCmd = new SqlCommand(statusNameCmdText, newFileConnection))
                {
                    statusNameCmd.Parameters.AddWithValue("@durumAdi", firstStatus);
                    newFileConnection.Open();
                    using (SqlDataReader statusNameReader = statusNameCmd.ExecuteReader())
                    {
                        if (statusNameReader.Read() == false)
                        {
                            newFileConnection.Close();
                            string addNewStatusCmdText = "INSERT INTO durumlar (durumAdi) VALUES ('Yeni')";
                            using (SqlCommand addNewStatusCmd = new SqlCommand(addNewStatusCmdText, newFileConnection))
                            {
                                newFileConnection.Open();
                                addNewStatusCmd.ExecuteNonQuery();
                            }
                        }
                    }
                    newFileConnection.Close();
                    newFileConnection.Open();
                    using (SqlDataReader statusNameReaderAgain = statusNameCmd.ExecuteReader())
                    {
                        if (statusNameReaderAgain.Read())
                        {
                            returnStatusValue = Convert.ToInt32(statusNameReaderAgain["durumNo"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            newFileConnection.Close();
            return returnStatusValue;
        }
        //Amaç eklenen belgenin var olup olmadığını kontrol etmektir.
        public Tuple<string, string, string> NewFile_AddButton_Check(int UserId2, string fileDirectory, string fileName)
        {
            string returnValue = "";
            string userName = "";
            string userSurname = "";

            //Veritabanı bağlantısı aktif değilse aktif yap
            newFileConnection.Close();

            string checkCmdText = "SELECT blg.belgeDizini, blg.belgeAdi, blg.kullaniciNo, klnc.kullaniciAdi, klnc.kullaniciSoyadi FROM belgelerim AS blg INNER JOIN kullanicilar AS klnc ON blg.kullaniciNo = klnc.kullaniciNo WHERE blg.belgeDizini = @belgeDizini AND blg.belgeAdi = @belgeAdi";
            using (SqlCommand checkCmd = new SqlCommand(checkCmdText, newFileConnection))
            {
                checkCmd.Parameters.AddWithValue("@belgeDizini", fileDirectory);
                checkCmd.Parameters.AddWithValue("@belgeAdi", fileName);
                newFileConnection.Open();
                using (SqlDataReader checkCmdReader = checkCmd.ExecuteReader())
                {
                    //Eğer böyle bir belge adı ya da dizini yok ise false koşulu çalışır. Varsa true koşulu çalışır ve belge güncelleme ya da değiştirme seçeneği sunar.
                    if (checkCmdReader.Read() == false)
                    {
                        returnValue = "false";
                    }
                    else
                    {
                        if (checkCmdReader["kullaniciNo"].ToString() == UserId2.ToString())
                        {
                            returnValue = "existingFile";
                        }
                        else
                        {
                            //Belgeyi eklemiş olan kullanıcının bilgileri döndürülür.
                            userName = checkCmdReader["kullaniciAdi"].ToString();
                            userSurname = checkCmdReader["kullaniciSoyadi"].ToString();
                        }
                    }
                }
            }
            newFileConnection.Close();

            var checksTuple = new Tuple<string, string, string>(returnValue, userName, userSurname);
            return checksTuple;
        }
        //Belge ekleme
        public void  NewFile_AddButton_Register(int UserId2, string fileTitle, string fileName, string fileDirectory, string fileTypeLabel, string serverPath, string fileExplain, string NewFileDateTimePicker)
        {
            int? fileStatusNo = StatusNameTableValue();
            //Veritabanına bilgileri ekle
            newFileConnection.Close();
            if(fileStatusNo != 0 && fileStatusNo != null)
            {
                try
                {
                    string registerCmdText = "INSERT INTO belgelerim (kullaniciNo, belgeBasligi, belgeAdi, belgeDizini, belgeVeriTipiveAdi, belgeServerDizini, belgeAciklamasi, eklenmeTarihi, sistemEklenmeTarihi, durumNo) values (@kullaniciNo, @belgeBasligi, @belgeAdi, @belgeDizini, @belgeVeriTipiveAdi, @belgeServerDizini, @belgeAciklamasi, @eklenmeTarihi, @sistemEklenmeTarihi, @durumNo)";
                    using (SqlCommand registerCmd = new SqlCommand(registerCmdText, newFileConnection))
                    {
                        registerCmd.Parameters.AddWithValue("@kullaniciNo", UserId2);
                        registerCmd.Parameters.AddWithValue("@belgeBasligi", fileTitle);
                        registerCmd.Parameters.AddWithValue("@belgeAdi", fileName);
                        registerCmd.Parameters.AddWithValue("@belgeDizini", fileDirectory);
                        registerCmd.Parameters.AddWithValue("@belgeVeriTipiveAdi", fileTypeLabel);
                        registerCmd.Parameters.AddWithValue("@belgeServerDizini", serverPath);
                        registerCmd.Parameters.AddWithValue("@belgeAciklamasi", fileExplain);
                        registerCmd.Parameters.AddWithValue("@eklenmeTarihi", DateTime.Parse(NewFileDateTimePicker));
                        registerCmd.Parameters.AddWithValue("@sistemEklenmeTarihi", DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                        registerCmd.Parameters.AddWithValue("@durumNo", fileStatusNo); //Yeni eklenen belgeler her zaman Yeni olarak işaretlenir.
                        newFileConnection.Open();
                        registerCmd.ExecuteNonQuery();
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                newFileConnection.Close();
            }
            else
            {
                Console.WriteLine("Null");
            }
        }
    }
}
