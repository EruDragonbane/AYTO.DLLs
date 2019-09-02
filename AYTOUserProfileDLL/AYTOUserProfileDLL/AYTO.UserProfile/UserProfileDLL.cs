using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace AYTO.UserProfile
{
    public class UserProfileDLL
    {
        
        SqlConnection userProfileConnection = new SqlConnection("Data Source = ERU; Initial Catalog = deneme; Integrated Security = SSPI");
        //Kullanıcı Bilgileri
        public Tuple<string, string, string, string> InformationAbourtUser(int UserId6)
        {
            string userNameSurname = "";
            string userCorp = "";
            string userPosition = "";
            string userID = "";

            userProfileConnection.Close();

            string infoAboutUserCmdText = "SELECT klnc.kullaniciAdi, klnc.kullaniciNo, klnc.kullaniciSoyadi, klnc.kullaniciKurumu, grv.gorevAdi FROM kullanicilar AS klnc INNER JOIN gorevler AS grv ON klnc.gorevNo = grv.gorevNo WHERE klnc.kullaniciNo = @kullaniciNo";
            using (SqlCommand infoAboutUserCmd = new SqlCommand(infoAboutUserCmdText, userProfileConnection))
            {
                infoAboutUserCmd.Parameters.AddWithValue("@kullaniciNo", UserId6);
                userProfileConnection.Open();
                using (SqlDataReader infoAboutUserReader = infoAboutUserCmd.ExecuteReader())
                {
                    if (infoAboutUserReader.Read())
                    {
                        userNameSurname = infoAboutUserReader["kullaniciAdi"].ToString() + ' ' + infoAboutUserReader["kullaniciSoyadi"].ToString();
                        userID = infoAboutUserReader["kullaniciNo"].ToString();
                        userCorp = infoAboutUserReader["kullaniciKurumu"].ToString();
                        userPosition = infoAboutUserReader["gorevAdi"].ToString();
                    }
                }
            }
            userProfileConnection.Close();

            var infoAboutUserTuple = new Tuple<string, string, string, string>(userNameSurname, userCorp, userPosition, userID);
            return infoAboutUserTuple;
        }
        //MD% Hashing
        public string Mda5Hash(string inputPass)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            StringBuilder hashString = new StringBuilder();
            byte[] hashArray = md5.ComputeHash(Encoding.UTF8.GetBytes(inputPass));
            foreach (byte byteHash in hashArray)
                hashString.Append(byteHash.ToString("x2"));
            return hashString.ToString();
        }
        //Şifre Kontrolü
        public string CheckPasswordBeforeChange(string CurrentPassword, int UserId)
        {
            string returnValue = "";
            userProfileConnection.Close();

            string checkPasswordCmdText = "SELECT klnc.kullaniciParola FROM kullanicilar AS klnc WHERE klnc.kullaniciNo = @kullaniciNo";

            using (SqlCommand checkPasswordCmd = new SqlCommand(checkPasswordCmdText, userProfileConnection))
            {
                checkPasswordCmd.Parameters.AddWithValue("@kullaniciNo", UserId);
                userProfileConnection.Open();
                using (SqlDataReader checkPasswordReader = checkPasswordCmd.ExecuteReader())
                {
                    if (checkPasswordReader.Read())
                    {
                        string password = checkPasswordReader["kullaniciParola"].ToString();
                        if (password == Mda5Hash(CurrentPassword))
                        {
                            returnValue = "true";
                        }
                        else
                        {
                            returnValue = "false";
                        }
                    }
                    else
                    {
                        returnValue = "false";
                    }
                }
            }
            userProfileConnection.Close();

            return returnValue;
        }
        //Şifre Değiştirme
        public void ChangePassword(int UserId, string userNewPassword)
        {
            userProfileConnection.Close();

            string changePasswordCmdText = "UPDATE kullanicilar SET kullaniciParola = @kullaniciParola WHERE kullaniciNo = " + UserId;

            using (SqlCommand changePasswordCmd = new SqlCommand(changePasswordCmdText, userProfileConnection))
            {
                changePasswordCmd.Parameters.AddWithValue("@kullaniciParola", Mda5Hash(userNewPassword));
                userProfileConnection.Open();
                changePasswordCmd.ExecuteNonQuery();
            }
            userProfileConnection.Close();

        }
    }
}
