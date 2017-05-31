using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

public class secureWipe
{

    public void secureFileWipe(string origFile)
    {
        if (File.Exists(origFile))
        {

            try
            {
                File.SetAttributes(origFile, FileAttributes.Normal);
                double sectors = Math.Ceiling(new FileInfo(origFile).Length / 512.0);
                byte[] dummyBuffer = new byte[512];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                FileStream inputStream = new FileStream(origFile, FileMode.Open);
                inputStream.Position = 0;
                for (int sectorsWritten = 0; sectorsWritten < sectors; sectorsWritten++)
                {
                    rng.GetBytes(dummyBuffer);
                    inputStream.Write(dummyBuffer, 0, dummyBuffer.Length);
                }
                inputStream.SetLength(0);
                inputStream.Close();
                DateTime dt = new DateTime(2037, 1, 1, 0, 0, 0);
                File.SetCreationTime(origFile, dt);
                File.SetLastAccessTime(origFile, dt);
                File.SetLastWriteTime(origFile, dt);
                File.SetCreationTimeUtc(origFile, dt);
                File.SetLastAccessTimeUtc(origFile, dt);
                File.SetLastWriteTimeUtc(origFile, dt);
                File.Delete(origFile); // Finally, delete the file
                Debug.WriteLine("Successfully securely deleted file '" + Path.GetFileName(origFile) + "'");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error securely deleting file");
            }
        }
    }

}
