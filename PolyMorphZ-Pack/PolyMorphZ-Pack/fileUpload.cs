using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Security;


/// &lt;summary&gt;
/// Simple static class for uploading a file to an FTP server.
/// &lt;/summary&gt;
public static class fileUpload
{
    public static string uploadFile(string file)
    {


        string log = Path.GetRandomFileName();


        // Get the object used to communicate with the server.
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://192.168.1.7/" + log);
        request.Proxy = null;
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.EnableSsl = true; // Use SSL




        // This example assumes the FTP site uses anonymous logon.
        request.Credentials = new NetworkCredential("ben", "front2011");

        ServicePointManager.ServerCertificateValidationCallback +=
        delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                        System.Security.Cryptography.X509Certificates.X509Chain chain,
                        SslPolicyErrors sslPolicyErrors)
        {
            return true; // **** Always accept certificate
        };

        // Copy the entire contents of the file to the request stream.
        StreamReader sourceStream = new StreamReader(file);
        byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
        sourceStream.Close();
        request.ContentLength = fileContents.Length;

        // Upload the file stream to the server.
        Stream requestStream = request.GetRequestStream();
        requestStream.Write(fileContents, 0, fileContents.Length);
        requestStream.Close();

        // Get the response from the FTP server.
        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        // Close the connection = Happy a FTP server.
        response.Close();

        // Return the status of the upload.
        return response.StatusDescription;

    }
}