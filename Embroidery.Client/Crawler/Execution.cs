﻿using Embroidery.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Crawler
{
    public class Execution : IDisposable
    {
        private System.Threading.CancellationTokenSource _cancellationToken;
        public Execution(System.Threading.CancellationTokenSource cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathToSearch"></param>
        /// <param name="tempFolder"></param>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        public void Run(string pathToSearch, string tempFolder)
        {
            if (!System.IO.Directory.Exists(pathToSearch))
                throw new System.IO.DirectoryNotFoundException($"The path to search ({pathToSearch}) was not found");

            if (!System.IO.Directory.Exists(tempFolder))
                System.IO.Directory.CreateDirectory(tempFolder);

            var token = _cancellationToken.Token;
            var task = new Task(() => {
                using (var db = new DataContext())
                {
                    Dictionary<string, int> tagNameLookup = new Dictionary<string, int>();

                    foreach (var item in db.Tags.Select(x => new { x.Id, x.Name }))
                        tagNameLookup.Add(item.Name, item.Id);

                    Refresh(pathToSearch, (newFile) =>
                    {
                        var fileNameNoExtension = System.IO.Path.GetFileNameWithoutExtension(newFile);
                        var fileNameWithExtension = System.IO.Path.GetFileName(newFile);
                        var filePath = System.IO.Path.GetFullPath(newFile);

                        if (db.Files.Any(x => x.Name == fileNameNoExtension && x.Path == filePath))
                        {
                            System.Diagnostics.Debug.WriteLine($"Already indexed {newFile}");
                            //Anything to update?
                        }
                        else
                        {
                            var tempFile = System.IO.Path.Combine(tempFolder, Guid.NewGuid() + ".bmp");
                            string fileHash;
                            List<Tag> fileTags = new List<Tag>();

                            System.Diagnostics.Debug.WriteLine($"Working on {newFile}");

                            PesToTargetFile(newFile, tempFile);

                            var foundTags = TokenizeName(System.IO.Path.GetFileNameWithoutExtension(newFile));

                            //Add any new tags, and then add them to the lookup table
                            foreach (var tag in foundTags)
                            {
                                if (token.IsCancellationRequested)
                                    return;
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

                            if (token.IsCancellationRequested)
                                return;

                            db.Files.Add(new Models.File(tempFile, newFile));

                            System.IO.File.Delete(tempFile);
                            System.Diagnostics.Debug.Write($"Saving '{newFile}'");
                            db.SaveChanges();
                        }
                    });
                }

            }, token);


            task.Start();
        }



        /// <summary>
        /// Converts the pes file to what ever the target file type is
        /// </summary>
        /// <param name="pesFile"></param>
        /// <param name="targetFile"></param>
        public static void PesToTargetFile(string pesFile, string targetFile)
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
            startInfo.Arguments = $"convert \"{pesFile}\" -trim +repage -depth 4 -compress RLE -type palette BMP3:\"{targetFile}\"";
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

                if (charArray[i] != ' ')
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

        public void Dispose()
        {
            _cancellationToken.Cancel();
        }
    }
}
