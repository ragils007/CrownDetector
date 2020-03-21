using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
// using ICSharpCode.SharpZipLib.Core;
// using ICSharpCode.SharpZipLib.Zip;
using System.IO.Compression;

namespace Msdfa.Tools
{
    public class Compress
    {

        public static byte[] UnzipSingleFileToByteArray(string zipPath, string fileToUnzip)
        {
            throw new NotImplementedException();

            /*
			byte[] ret = null;
			ZipFile zf = new ZipFile(zipPath);
			ZipEntry ze = zf.GetEntry(fileToUnzip);

			if (ze != null)
			{
				Stream s = zf.GetInputStream(ze);
				ret = new byte[ze.Size];
				s.Read(ret, 0, ret.Length);
			}
			return ret;
			*/
        }

        public static string UnzipSingleFileToString(string zipPath, string fileToUnzip)
        {
            throw new NotImplementedException();

            /*
			ZipFile zf = new ZipFile(zipPath);
			ZipEntry ze = zf.GetEntry(fileToUnzip);

			if (ze != null)
			{
				Stream s = zf.GetInputStream(ze);
				StreamReader reader = new StreamReader( s );
				return reader.ReadToEnd();		
			}
			return null;
			*/
        }

        // Compresses single file, and creates a zip file on disk named as outPathname.
        public static void ZipFile(string fileName, string outPathname, string password = null)
        {
            throw new NotImplementedException();


            //  string[] files = new string[] { fileName };
            //  ZipFiles(outPathname, files, password);
        }

        // Compresses the files in the nominated folder, and creates a zip file on disk named as outPathname.
        public static void ZipFolder(string outPathname, string folderName, string password = null)
        {
            throw new NotImplementedException();

            // string[] files = Directory.GetFiles(folderName);
            // ZipFiles(outPathname, files, password);
        }

        // Compresses the files in the nominated folder, and creates a zip file on disk named as outPathname.
        //
        public static void ZipFiles(string outPathname, string[] fileNames, string password = null)
        {
            throw new NotImplementedException();

            // FileStream fsOut = File.Create(outPathname);
            // ZipOutputStream zipStream = new ZipOutputStream(fsOut);

            // zipStream.SetLevel(9);
            // zipStream.Password = password;

            // foreach (string filename in fileNames)
            // {
            // 	FileInfo fi = new FileInfo(filename);

            // 	string entryName = ZipEntry.CleanName(filename); // Removes drive from name and fixes slash direction
            // 	ZipEntry newEntry = new ZipEntry(entryName);
            // 	newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity
            // 	newEntry.Size = fi.Length;
            // 	zipStream.PutNextEntry(newEntry);

            // 	byte[] buffer = new byte[4096];
            // 	using (FileStream streamReader = File.OpenRead(filename))
            // 	{
            // 		StreamUtils.Copy(streamReader, zipStream, buffer);
            // 	}
            // 	zipStream.CloseEntry();
            // }
            // zipStream.IsStreamOwner = true;	// Makes the Close also Close the underlying stream
            // zipStream.Close();
        }

        public static void ZipStreams(string outPathname, Dictionary<string, MemoryStream> streams, string password = null)
        {
            throw new NotImplementedException();

            // FileStream fsOut = File.Create(outPathname);
            // ZipOutputStream zipStream = new ZipOutputStream(fsOut);
            // zipStream.SetLevel(9);
            // zipStream.Password = password;
            // foreach (KeyValuePair<string, MemoryStream> pair in streams)
            // {
            // 	string entryName = ZipEntry.CleanName(pair.Key); // Removes drive from name and fixes slash direction
            // 	ZipEntry newEntry = new ZipEntry(entryName);
            // 	newEntry.DateTime = DateTime.Now;
            // 	zipStream.UseZip64 = UseZip64.Off;
            // 	zipStream.PutNextEntry(newEntry);
            // 	pair.Value.Position = 0;

            // 	byte[] buffer = new byte[4096];
            // 	StreamUtils.Copy((MemoryStream)pair.Value, zipStream, buffer);
            // 	zipStream.CloseEntry();
            // }
            // zipStream.IsStreamOwner = true;	// Makes the Close also Close the underlying stream
            // zipStream.Close();
        }

        public static void ZipByteArrays(string outPathname, Dictionary<string, byte[]> streams, string password = null)
        {
            throw new NotImplementedException();

            // FileStream fsOut = File.Create(outPathname);
            // ZipOutputStream zipStream = new ZipOutputStream(fsOut);
            // zipStream.SetLevel(9);
            // zipStream.Password = password;
            // foreach (KeyValuePair<string, byte[]> pair in streams)
            // {
            // 	string entryName = ZipEntry.CleanName(pair.Key); // Removes drive from name and fixes slash direction
            // 	ZipEntry newEntry = new ZipEntry(entryName);
            // 	newEntry.DateTime = DateTime.Now;
            // 	zipStream.UseZip64 = UseZip64.Off;
            // 	zipStream.PutNextEntry(newEntry);
            // 	//pair.Value.Position = 0;

            // 	byte[] buffer = new byte[4096];
            // 	StreamUtils.Copy(new MemoryStream(pair.Value), zipStream, buffer);
            // 	zipStream.CloseEntry();
            // }
            // zipStream.IsStreamOwner = true;	// Makes the Close also Close the underlying stream
            // zipStream.Close();
        }

        public static byte[] CompressToByteArray(byte[] oryginalBytes)
        {
            throw new NotImplementedException();

            //  using (var msi = new MemoryStream(oryginalBytes))
            //  using (var mso = new MemoryStream())
            //  {
            //     using (var gs = new DeflateStream(mso, CompressionMode.Compress))
            //     {
            //        msi.CopyTo(gs);
            //     }

            //     return mso.ToArray();
            //  }
        }

        public static byte[] DecompressToByteArray(byte[] compressedBytes)
        {
            throw new NotImplementedException();
            // var output = new MemoryStream();

            // using (var compressStream = new MemoryStream(compressedBytes))
            // using (var decompressor = new DeflateStream(compressStream, CompressionMode.Decompress))
            //    decompressor.CopyTo(output);

            // output.Position = 0;
            // return output.ToArray();
        }


        //public static void ZipFile(string inFile, string outFile)
        //{
        //   using (Ionic.Zip.ZipFile zf = new Ionic.Zip.ZipFile())
        //   {
        //      zf.AddFile(inFile, "");
        //      zf.Save(outFile);
        //   }
        //}
    }
}
