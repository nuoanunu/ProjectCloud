using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using System.Data.SqlClient;
using UniversityDbCommon.Models;
using System.Data;
using System.IO;

using System.Net.Mail;
using System.Web.Script.Serialization;

namespace UniversityDbWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private CloudQueue imagesQueue;
        private CloudBlobContainer imagesBlobContainer;
        private string connString;
        private SqlConnection connection;

        public override void Run()
        {
            Trace.TraceInformation("UniversityDbWorker is running");
            CloudQueueMessage message = null;
            if (connection == null) {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = "NHATVHNSE61418\\MS5";
                builder.InitialCatalog = "UniversityDatabase";
                builder.UserID = "sa";
                // builder.Password = "123";
                builder.IntegratedSecurity = true;
      
                connection = new SqlConnection(builder.ConnectionString);
                connection.Open();
            }
        
            while (true)
            {
                try
                {
                    NhatLuoiLamTHemWorkerAhihi();
                  
                }
                catch (StorageException e)
                {
                    if (message != null && message.DequeueCount > 5)
                    {
                        this.imagesQueue.DeleteMessage(message);
                        Trace.TraceError("Deleting poison queue item: '{0}'", message.AsString);
                    }
                    Trace.TraceError("Exception in UniversityDbWorker: '{0}'", e.Message);
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.
            try
            {
                connString = CloudConfigurationManager.GetSetting("UniversityDbConnectionString");

                // Open storage account using credentials from .cscfg file.
                var storageAccount = CloudStorageAccount.Parse
                    (RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

                Trace.TraceInformation("Creating images blob container");
                var blobClient = storageAccount.CreateCloudBlobClient();
                imagesBlobContainer = blobClient.GetContainerReference("university-images");
                if (imagesBlobContainer.CreateIfNotExists())
                {
                    // Enable public access on the newly created "images" container.
                    imagesBlobContainer.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        });
                }

                Trace.TraceInformation("Creating images queue");
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                imagesQueue = queueClient.GetQueueReference("university-images");
                imagesQueue.CreateIfNotExists();

                Trace.TraceInformation("Storage initialized");
            }
            catch (Exception)
            {
                throw;
            }

            Trace.TraceInformation("UniversityDbWorker has been started");

            return base.OnStart();
        }

        public override void OnStop()
        {
            Trace.TraceInformation("UniversityDbWorker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            if (connection != null)
            {
                connection.Close();
            }

            base.OnStop();

            Trace.TraceInformation("UniversityDbWorker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }



        private void UpdateProfileThumbnail(int id, string uri)
        {
            string query = "UPDATE Student SET ProfileThumbnailUrl = @uri WHERE ID=@id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@uri", uri);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        private string GetProfileImage(int id)
        {
            string url = null;

            string query = "SELECT ProfileImageUrl FROM Student WHERE ID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                url = reader[0].ToString();
            }
            reader.Close();

            return url;
        }
        private string getProfileEmail(int id)
        {
            string url = null;

            string query = "SELECT Email FROM Student WHERE ID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                url = reader[0].ToString();
            }
            reader.Close();

            return url;
        }

        public void NhatLuoiLamTHemWorkerAhihi()
        {
            CloudStorageAccount ac = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudQueueClient qclient = ac.CreateCloudQueueClient();
            CloudQueue que = qclient.GetQueueReference("nhatvhnqueue");
            que.CreateIfNotExists();
            CloudQueueMessage mess = que.GetMessage();
            if (mess != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                JsonMessage emailcontent = serializer.Deserialize<JsonMessage>(mess.AsString);
                System.Diagnostics.Debug.WriteLine(emailcontent.CourseID + "FUCKKKKKKKKKKK ");
                guimail(emailcontent);
                que.DeleteMessage(mess);
            }

        }
        public async void guimail(JsonMessage mailcontent)
        {
            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            var message = new MailMessage();
            message.To.Add(new MailAddress("nhatvhn99@gmail.com"));  // replace with valid value 
            message.From = new MailAddress("dwarpro@gmail.com");  // replace with valid value
            message.Subject = "Thong bao sua diem";
            String content = "";

            content = content + " <p style = 'text-align: center;' ><img title = 'TinyMCE Logo' src = 'www.tinymce.com/images/glyph-tinymce@2x.png' alt = 'TinyMCE Logo' width = '110' height = '97' /></p>";
            content = content + " <h1 style = 'text-align: center;' > Hello " + getProfileEmail(int.Parse(mailcontent.StudentID)) + " </h1>";
            content = content + "  <p> There are some changes in your grades of the  " + mailcontent.CourseID + " :&nbsp;</p>";
            content = content + "   <p> &nbsp;</p>";
            content = content + "      <table style = 'text -align: center;' border='1' >";
            content = content + "       <thead>";
            content = content + "      <tr>";
            content = content + "      <th> &nbsp;</th>";
            content = content + "         <th> Quiz 1 </th>";
            content = content + "           <th> Quiz 2 </th>";
            content = content + "              <th> Quiz 3 </th>";
            content = content + "                 <th> Project </th>";
            content = content + "                <th> Midterm </th>";
            content = content + "               <th> Final </th>";
            content = content + "               </tr>";
            content = content + "               </thead>";
            content = content + "               <tbody>";
            content = content + "              <tr>";
            content = content + "              <td> Old </td>";
            content = content + "              <td>[OldQuiz1] </td>";
            content = content + "             <td>[OldQuiz2] </td>";
            content = content + "             <td>[OldQuiz3] </td>";
            content = content + "            <td>[OldProject] </td>";
            content = content + "            <td>[OldMidterm] </td>";
            content = content + "           <td>[OldFinal] </td>";
            content = content + "           </tr>";
            content = content + "           <tr>";
            content = content + "           <td> New </td>";
            content = content + "          <td>[Quiz1] </td>";
            content = content + "          <td>[Quiz2] </td>";
            content = content + "         <td>[Quiz3] </td>";
            content = content + "        <td>[Project] </td>";
            content = content + "        <td>[Midterm] </td>";
            content = content + "       <td>[Final] </td>";
            content = content + "      </tr>";
            content = content + "        </tbody>";
            content = content + "        </table>";


            content = content.Replace("[OldQuiz1]", mailcontent.oldQuiz1);
            content = content.Replace("[OldQuiz2]", mailcontent.oldQuiz2);
            content = content.Replace("[OldQuiz3]", mailcontent.oldQuiz3);
            content = content.Replace("[OldProject]", mailcontent.oldProject);
            content = content.Replace("[OldMidterm]", mailcontent.oldMidterm);
            content = content.Replace("[OldFinal]", mailcontent.oldFinal);
            content = content.Replace("[Quiz1]", mailcontent.Quiz1);
            content = content.Replace("[Quiz2]", mailcontent.Quiz2);
            content = content.Replace("[Quiz3]", mailcontent.Quiz3);
            content = content.Replace("[Project]", mailcontent.Project);
            content = content.Replace("[Final]", mailcontent.Final);
            content = content.Replace("[Midterm]", mailcontent.Midterm);






            message.Body = string.Format(body, "dwarpro@gmail.com", "dwarpro@gmail.com", content);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "dwarpro@gmail.com",  // replace with valid value
                    Password = "320395qww"  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);

            }

        }
    }

    public class JsonMessage
    {
        public JsonMessage(string json)
        {

        }
        public String CourseID { get; set; }
        public String StudentID { get; set; }
        public String Quiz1 { get; set; }
        public String Quiz2 { get; set; }
        public String Quiz3 { get; set; }
        public String Midterm { get; set; }
        public String Project { get; set; }
        public String Final { get; set; }
        public String oldQuiz1 { get; set; }
        public String oldQuiz2 { get; set; }
        public String oldQuiz3 { get; set; }
        public String oldMidterm { get; set; }
        public String oldProject { get; set; }
        public String oldFinal { get; set; }
        public JsonMessage()
        {
            CourseID = "";
            StudentID = "";
            Quiz1 = "";
            Quiz2 = "";
            Quiz3 = "";
            Midterm = "";
            Project = "";
            Final = "";
            oldQuiz1 = "";
            oldQuiz2 = "";
            oldQuiz3 = "";
            oldMidterm = "";
            oldProject = "";
            oldFinal = "";
        }
    }
}
