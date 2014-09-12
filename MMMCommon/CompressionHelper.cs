using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace MMMCommon.Zipper
{
    public class CompressionHelper
    {
        public static string UncompressContent(byte[] zippedContent)
        {
            try
            {
                MemoryStream inp = new MemoryStream(zippedContent);
                ZipInputStream zipin = new ZipInputStream(inp);
                ZipEntry entryin = zipin.GetNextEntry();
                byte[] buffout = new byte[(int)zipin.Length];
                zipin.Read(buffout, 0, (int)zipin.Length);

                MemoryStream decompress = new MemoryStream(buffout);

                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                
                string result = enc.GetString(decompress.ToArray());
                decompress.Dispose();
                inp.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }

        }

        /// <summary>
        /// Compress an string using ZIP
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static byte[] CompressContent(string contentToZip)
        {

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] buff = encoding.GetBytes(contentToZip);

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (ZipOutputStream zipout = new ZipOutputStream(stream))
                    {
                        zipout.SetLevel(9);
                        ZipEntry entry = new ZipEntry("zipfile.zip");
                        entry.DateTime = DateTime.Now;
                        zipout.PutNextEntry(entry);
                        zipout.Write(buff, 0, buff.Length);
                        zipout.Finish();
                        byte[] outputbyte = new byte[(int)stream.Length];
                        stream.Position = 0;
                        stream.Read(outputbyte, 0, (int)stream.Length);
                        return outputbyte;
                    }

                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }


    }
}