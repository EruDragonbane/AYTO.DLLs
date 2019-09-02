using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.SqlClient;
namespace AYTO.Login
{
    public class LoginDLL
    {
        private string Mda5Hash(string loginPassword)
        {
            //Girilen parola MD5 ile şifrelenerek veritabanı kontrolü yapılır.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            StringBuilder hashString = new StringBuilder();
            byte[] hashArray = md5.ComputeHash(Encoding.UTF8.GetBytes(loginPassword));
            foreach (byte byteHash in hashArray)
                hashString.Append(byteHash.ToString("x2"));
            return hashString.ToString();
            //MD5 Şifreleme
        }

        public Tuple<string, int, string, string, string, string> LoginPage_LoginButton(string loginId, string loginPassword)
        {
            string returnValue = "";
            string userAuthority = "";
            int kullaniciNo = 0;
            string kullaniciAdi = "";
            string kullaniciSoyadi = "";
            string silinmeDurumu = "";

            using (SqlConnection loginConnection = new SqlConnection("Data Source = ERU; Initial Catalog = deneme; Integrated Security = SSPI"))
            {

                if (!string.IsNullOrWhiteSpace(loginId) && !string.IsNullOrWhiteSpace(loginPassword))
                {
                    loginConnection.Close();
                    string loginCmdText = "SELECT klnc.kullaniciAdi, klnc.kullaniciSoyadi, klnc.kullaniciNo, klnc.silinmeDurumu, ytk.yetkiNo, klnc.kullaniciAktifligi FROM kullanicilar AS klnc INNER JOIN yetkiler AS ytk ON klnc.yetkiNo = ytk.yetkiNo WHERE klnc.kullaniciGiris = @loginId AND klnc.kullaniciParola = @loginPassword";
                    using (SqlCommand loginCmd = new SqlCommand(loginCmdText, loginConnection))
                    {
                        loginCmd.Parameters.AddWithValue("@loginId", loginId);
                        loginCmd.Parameters.AddWithValue("@loginPassword", Mda5Hash(loginPassword));
                        loginConnection.Open();

                        using (SqlDataReader loginReader = loginCmd.ExecuteReader())
                        {
                            if (loginReader.Read())
                            {
                                kullaniciNo = Convert.ToInt32(loginReader["kullaniciNo"]);
                                kullaniciAdi = loginReader["kullaniciAdi"].ToString();
                                kullaniciSoyadi = loginReader["kullaniciSoyadi"].ToString();
                                silinmeDurumu = loginReader["silinmeDurumu"].ToString();

                                if (loginReader["kullaniciAktifligi"].ToString() == "True")
                                {
                                    if (loginReader["yetkiNo"].ToString() == "1")
                                    {
                                        returnValue = "yetkili";
                                        userAuthority = "yapan Yetkili";
                                    }
                                    else
                                    {
                                        returnValue = "kullanici";
                                        userAuthority = "yapan Kullanıcı";
                                    }
                                }
                                else
                                {
                                    returnValue = "inactive";
                                    userAuthority = "denemesi yapan Pasif Kullanıcı";
                                }
                            }
                            else
                            {
                                returnValue = "wrongIdOrPassword";
                            }
                        }
                        loginConnection.Close();
                    }
                }
                else
                {
                    returnValue = "nullValue";
                }

                var loginTuple = new Tuple<string, int, string, string, string, string>(returnValue, kullaniciNo, userAuthority, kullaniciAdi, kullaniciSoyadi, silinmeDurumu);
                return loginTuple;
            }
        }
    }
}
