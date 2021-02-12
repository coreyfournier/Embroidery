using Embroidery.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Crawler
{
    public class Execution
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathToSearch"></param>
        /// <param name="tempFolder"></param>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        public static void Run(string pathToSearch, string tempFolder)
        {
            if (!System.IO.Directory.Exists(pathToSearch))
                throw new System.IO.DirectoryNotFoundException($"The path to search ({pathToSearch}) was not found");

            if (!System.IO.Directory.Exists(tempFolder))
                System.IO.Directory.CreateDirectory(tempFolder);

            using (var db = new DataContext())
            {
                Dictionary<string,int> tagNameLookup = new Dictionary<string, int>();
                
                foreach (var item in db.Tags.Select(x => new { x.Id, x.Name }))
                    tagNameLookup.Add(item.Name, item.Id);

                Refresh(pathToSearch, (newFile) =>
                {
                    var fileName = System.IO.Path.GetFileName(newFile);
                    var filePath = System.IO.Path.GetFullPath(newFile);


                    if (db.Files.Any(x => x.Name == fileName && x.Path == filePath))
                    { 
                        //Anything to update?
                    }
                    else
                    {
                        var tempFile = System.IO.Path.Combine(tempFolder, Guid.NewGuid() + ".jpg");
                        string fileHash;
                        List<Tag> fileTags = new List<Tag>();

                        System.Diagnostics.Debug.WriteLine($"Working on {newFile}");

                        PesToJpg(newFile, tempFile);

                        using (var fileStream = new System.IO.FileStream(newFile, FileMode.Open))
                        {
                            fileHash = GetChecksumBuffered(fileStream);
                        }

                        var foundTags = TokenizeName(System.IO.Path.GetFileNameWithoutExtension(newFile));

                        //Add any new tags, and then add them to the lookup table
                        foreach (var tag in foundTags)
                        {
                            if (!tagNameLookup.ContainsKey(tag))
                            {
                                var newTag = new Tag()
                                {
                                    CreatedDate = DateTime.Now,
                                    Name = tag,
                                    //Nullable doesn't work
                                    UpdatedDate = DateTime.MinValue
                                };
                                db.Tags.Add(newTag);
                                db.SaveChanges();
                                tagNameLookup.Add(tag, newTag.Id);
                            }
                        }

                        db.Files.Add(new Models.File()
                        {
                            CreatedDate = DateTime.Now,
                            ImageThumbnail = System.IO.File.ReadAllBytes(tempFile),
                            Name = fileName,
                            Path = filePath,
                            SizeInKb = (int)((decimal)(new System.IO.FileInfo(newFile).Length) / 1024M),
                            //Nullable doesn't work for some reason
                            UpdatedDate = DateTime.MinValue,
                            FileHash = fileHash
                        });

                        System.IO.File.Delete(tempFile);
                        System.Diagnostics.Debug.Write($"Saving '{newFile}'");
                        db.SaveChanges();
                    }                    
                });
            }
        }

        public static string GetChecksumBuffered(Stream stream)
        {
            using (var bufferedStream = new BufferedStream(stream, 1024 * 32))
            {
                var sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(bufferedStream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }

        public static void PesToJpg(string pesFile, string targetFile)
        {
            System.Diagnostics.Debug.WriteLine($"Converting {pesFile}");
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "magick";
            startInfo.Arguments = $"convert \"{pesFile}\" \"{targetFile}\"";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        public static string[] TokenizeName(string fileName)
        {            
            StringBuilder sb = new StringBuilder();
            List<string> list = new List<string>();

             var charArray = fileName.ToCharArray();

            for (int i = 0; i < charArray.Length; i++)
            {
                //If it's uppcase then add a space before the word, but not if it's the start
                if (i != 0)
                {
                    if (Char.IsUpper(charArray[i]))
                    {
                        list.Add(sb.ToString());
                        sb.Clear();
                    }
                    else if (charArray[i] == ' ')
                    {
                        list.Add(sb.ToString());
                        sb.Clear();
                    }
                }

                if(charArray[i] != ' ')
                    sb.Append(charArray[i]);
            }

            list.Add(sb.ToString());

            return list.ToArray();
        }

        /// <summary>
        /// Traverses the directory and makes a call back to fileFound once a file is found
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileFound"></param>
        public static void Refresh(string path, Action<string> fileFound)
        {
            foreach (var file in System.IO.Directory.GetFiles(path, "*.pes"))
            {
                fileFound(file);
            }

            //Tunnel into each sub folder
            foreach (var folder in System.IO.Directory.GetDirectories(path))
                Refresh(folder, fileFound);
        }
    }
}
