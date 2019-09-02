using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AYTO.SendFile
{
    public class SendFileDLL
    {
        SqlConnection sendFileConnection = new SqlConnection("Data Source = ERU; Initial Catalog = deneme; Integrated Security = SSPI");

        public Tuple<string, string> TextGridFromMainPage(string BelgeNo)
        {
            string fileName = "";
            string fileDirectory = "";

            sendFileConnection.Close();

            string sendFileCmdText = "SELECT blg.belgeDizini, blg.belgeAdi FROM belgelerim AS blg INNER JOIN kullanicilar AS klnc ON blg.kullaniciNo = klnc.kullaniciNo WHERE belgeNo = @belgeNo";
            using (SqlCommand sendFileCmd = new SqlCommand(sendFileCmdText, sendFileConnection))
            {
                sendFileCmd.Parameters.AddWithValue("@belgeNo", BelgeNo);
                sendFileConnection.Open();
                using (SqlDataReader sendFileReader = sendFileCmd.ExecuteReader())
                {
                    if (sendFileReader.Read())
                    {
                        fileName = sendFileReader["belgeAdi"].ToString();
                        fileDirectory = sendFileReader["belgeDizini"].ToString();
                    }
                }
            }
            sendFileConnection.Close();

            var textGridTuple = new Tuple<string, string>(fileName, fileDirectory);
            return textGridTuple;
        }

        public string FileNameWithFormat(string BelgeNo)
        {
            string returnValue = "";
            sendFileConnection.Close();

            string fileNameCmdText = "SELECT blg.belgeVeriTipiveAdi FROM belgelerim AS blg WHERE blg.belgeNo = @belgeNo";

            using (SqlCommand fileNameCmd = new SqlCommand(fileNameCmdText, sendFileConnection))
            {
                fileNameCmd.Parameters.AddWithValue("@belgeNo", BelgeNo);
                sendFileConnection.Open();
                using(SqlDataReader fileNameReader = fileNameCmd.ExecuteReader())
                {
                    if (fileNameReader.Read())
                    {
                        returnValue = fileNameReader["belgeVeriTipiveAdi"].ToString();
                    }
                }
            }
            sendFileConnection.Close();

            return returnValue;

        }
        //Verilerin kaydedilmesi
        public void InsertTheValuesToTableForSend(string BelgeNo, int UserId, string UserNo)
        {
            sendFileConnection.Close();

            string insertFileCmdText = "INSERT INTO gonderilmisBelgeler (gonderilmisBelgeNo, gonderenKisiNo, gonderilenKisiNo, gonderilmeTarihi) VALUES (@gonderilmisBelgeNo, @gonderenKisiNo, @gonderilenKisiNo, @gonderilmeTarihi)";
            using (SqlCommand insertFileCmd = new SqlCommand(insertFileCmdText, sendFileConnection))
            {
                insertFileCmd.Parameters.Clear();

                insertFileCmd.Parameters.AddWithValue("@gonderilmisBelgeNo", BelgeNo);
                insertFileCmd.Parameters.AddWithValue("@gonderenKisiNo", UserId);
                insertFileCmd.Parameters.AddWithValue("@gonderilenKisiNo", UserNo);
                insertFileCmd.Parameters.AddWithValue("@gonderilmeTarihi", DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                sendFileConnection.Open();
                insertFileCmd.ExecuteNonQuery();
                sendFileConnection.Close();
            }
            sendFileConnection.Close();
        }
        //Gelen Verilerin Kaydedilmesi
        public void InsertTheValuesToTableForInbox(string BelgeNo, string UserNo, int UserId, string SentTitle, string SentExplain)
        {
            sendFileConnection.Close();

            string insertFileCmdText = "INSERT INTO gelenBelgeler (gelenBelgeNo, gelenKisiNo, gelenGonderenKisiNo, gelmeTarihi, gelenBelgeBasligi, gelenBelgeAciklamasi) VALUES (@gelenBelgeNo, @gelenKisiNo, @gelenGonderenKisiNo, @gelmeTarihi, @gelenBelgeBasligi, @gelenBelgeAciklamasi)";
            using (SqlCommand insertFileCmd = new SqlCommand(insertFileCmdText, sendFileConnection))
            {
                insertFileCmd.Parameters.Clear();

                insertFileCmd.Parameters.AddWithValue("@gelenBelgeNo", BelgeNo);
                insertFileCmd.Parameters.AddWithValue("@gelenKisiNo", UserNo);
                insertFileCmd.Parameters.AddWithValue("@gelenGonderenKisiNo", UserId);
                insertFileCmd.Parameters.AddWithValue("@gelmeTarihi", DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                insertFileCmd.Parameters.AddWithValue("@gelenBelgeBasligi", SentTitle);
                insertFileCmd.Parameters.AddWithValue("@gelenBelgeAciklamasi", SentExplain);
                sendFileConnection.Open();
                insertFileCmd.ExecuteNonQuery();
                sendFileConnection.Close();
            }
            sendFileConnection.Close();
        }
    }
}
