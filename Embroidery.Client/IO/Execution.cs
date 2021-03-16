using Embroidery.Client.Models;
using Embroidery.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Embroidery.Client.IO
{
    public class Execution : IDisposable
    {
        private System.Threading.CancellationTokenSource _cancellationToken;
        private bool _isRunning = false;
        private object _executionLocker = new object();
        public Execution()
        {
            
        }

        /// <summary>
        /// Starts the process of crawling a directory for files
        /// </summary>
        /// <param name="pathToSearch"></param>
        /// <param name="tempFolder">Creates the folder if it doesn't exists.</param>
        /// <param name="fileHandler"></param>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        public void Run(string pathToSearch, string tempFolder, IFileFound fileHandler)
        {
            if (!System.IO.Directory.Exists(pathToSearch))
                throw new System.IO.DirectoryNotFoundException($"The path to search ({pathToSearch}) was not found");

            if (!System.IO.Directory.Exists(tempFolder))
                System.IO.Directory.CreateDirectory(tempFolder);            

            var task = new Task(() => {
                if (_isRunning)
                    return;
                else
                {
                    lock (_executionLocker)
                    {
                        if (_isRunning)
                            return;
                        else
                        {
                            _isRunning = true;
                            _cancellationToken = new System.Threading.CancellationTokenSource();
                            System.Diagnostics.Debug.WriteLine("Starting Crawler");
                        }
                    }
                }

                var token = _cancellationToken.Token;

                using (var db = new DataContext())
                {
                    Dictionary<string, int> tagNameLookup = new Dictionary<string, int>();
                    Dictionary<string, int> folderLookup = new Dictionary<string, int>();
                    
                    //Creating a lookup 
                    foreach (var item in db.Folders.Select(x => new { x.Id, x.Path }))
                        folderLookup.Add(item.Path, item.Id);

                    foreach (var item in db.Tags.Select(x => new { x.Id, x.Name }))
                        tagNameLookup.Add(item.Name, item.Id);

                    Refresh(pathToSearch, (foundFile) =>
                    {
                        var fileNameNoExtension = System.IO.Path.GetFileNameWithoutExtension(foundFile);
                        var filePath = System.IO.Directory.GetParent(foundFile).FullName;

                        if (token.IsCancellationRequested)
                            return;

                        if (folderLookup.ContainsKey(filePath) && db.Files.Any(x => x.Name == fileNameNoExtension && x.FolderId == folderLookup[filePath]))
                        {
                            //System.Diagnostics.Debug.WriteLine($"Already indexed {foundFile}");
                            //Anything to update?
                            //Will need to check the file for changes
                            if(fileHandler != null)
                                fileHandler.StatusChange($"Already indexed {foundFile}");
                        }
                        else
                        {
                            var imageFile = System.IO.Path.Combine(tempFolder, Guid.NewGuid() + ".jpg");
                            List<Tag> fileTags = new List<Tag>();

                            if (fileHandler != null)
                                fileHandler.StatusChange($"Working on {foundFile}");

                            //System.Diagnostics.Debug.WriteLine($"Working on {foundFile}");

                            PesToTargetFile(foundFile, imageFile);

                            if (fileHandler != null)
                                fileHandler.StatusChange($"Converted {foundFile} to jpg");

                            var associations = AddAndAssocitateTags(foundFile, db, token, tagNameLookup);

                            if (fileHandler != null)
                                fileHandler.StatusChange($"Done finding tags");

                            if (token.IsCancellationRequested)
                                return;

                            //If the folder doesn't exists, then add it and then to the lookup
                            if (!folderLookup.ContainsKey(filePath))
                            {
                                if (fileHandler != null)
                                    fileHandler.StatusChange($"Found new folder {filePath}");

                                var newFolder = new Folder()
                                {
                                    Path = filePath,
                                    CreatedDate = DateTime.Now
                                };
                                db.Folders.Add(newFolder);
                                db.SaveChanges();

                                folderLookup.Add(filePath, newFolder.Id);
                                //System.Diagnostics.Debug.WriteLine($"Found folder {filePath}");                                
                            }
                            var newFile = new Models.File(imageFile, foundFile, folderLookup[filePath]);
                            db.Files.Add(newFile);

                            if (fileHandler != null)
                                fileHandler.StatusChange($"Saving....");

                            System.IO.File.Delete(imageFile);
                            //System.Diagnostics.Debug.Write($"Saving '{foundFile}'");
                            db.SaveChanges();

                            foreach (var item in associations)
                                item.FileId = newFile.Id;

                            db.FileTagRelationships.AddRange(associations);
                            db.SaveChanges();

                            if (fileHandler != null)
                                fileHandler.NewFileInDatabase(newFile);
                            //AddFileToList(fileList, newFile);
                        }
                    });
                }

            });

            task.Start();
        }

        private IEnumerable<FileTagRelationship> AddAndAssocitateTags(string foundFile, DataContext db, CancellationToken token, Dictionary<string, int> tagNameLookup)
        {
            var foundTags = TokenizeName(System.IO.Path.GetFileNameWithoutExtension(foundFile));
            List<FileTagRelationship> relationships = new List<FileTagRelationship>();            

            //Add any new tags, and then add them to the lookup table
            foreach (var tag in foundTags)
            {
                if (token.IsCancellationRequested)
                    return new FileTagRelationship[] { };

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
                relationships.Add(new FileTagRelationship() { 
                    TagId = tagNameLookup[tag]
                });
            }

            return relationships;
        }

        private void AddFileToList(ObservableCollection<Models.View.GroupedFile> fileList, Models.File newFile)
        {
            var foundExistingFile = fileList.FirstOrDefault(x => x.CleanName == newFile.CleanName);

            if (foundExistingFile == null)
            {
                fileList.Add(new Models.View.GroupedFile()
                {
                    CleanName = newFile.DisplayName,
                    FirstFileId = newFile.Id,
                    TotalLikeFiles = 1
                });
            }
            else
            {
                fileList.Remove(foundExistingFile);
                foundExistingFile.TotalLikeFiles += 1;
                fileList.Add(foundExistingFile);
            }
        }

        /// <summary>
        /// Converts the pes file to what ever the target file type is
        /// </summary>
        /// <param name="pesFile"></param>
        /// <param name="targetFile"></param>
        ///<exception cref="ImageMagickNotFoundException"></exception>
        public static void PesToTargetFile(string pesFile, string targetFile)
        {
            //System.Diagnostics.Debug.WriteLine($"Converting {pesFile}");
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "magick";
            //-trim +repage -depth 4 -compress RLE -type palette BMP3:
            startInfo.Arguments = $"convert \"{pesFile}\" \"{targetFile}\"";
            process.StartInfo = startInfo;

            try
            {
                process.Start();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                if (ex.Message.Contains("The system cannot find the file specified"))
                {
                    throw new ImageMagickNotFoundException(ex);
                }
                else
                    throw ex;
            }
            process.WaitForExit();
        }

        /// <summary>
        /// Tokenizes the name and returns a distinct list of values
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string[] TokenizeName(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            List<string> list = new List<string>();

            fileName = fileName
                .Replace('-', ' ')
                .Replace('_', ' ')
                .Replace('(', ' ')
                .Replace(')', ' ')
                .Replace('.',' ');

            fileName = FileNameAttributeParser.regExNumberUnit.Replace(fileName, " ");
            fileName = FileNameAttributeParser.regExLxW.Replace(fileName, " ");

            fileName = fileName
                .Replace("  ", "")
                .Trim();

            if (string.IsNullOrEmpty(fileName))
                return new string[] { };

            var charArray = fileName.ToCharArray();
            byte sameLetterCount = 0;
            char lastLetter =  char.MinValue;
            System.Diagnostics.Debug.WriteLine($"'{fileName}'");

            for (int i = 0; i < charArray.Length; i++)
            {                
                if (i != 0)
                {
                    //If the letter is upper case, but don't have a space before the word, then we captured a whole word
                    if (Char.IsUpper(charArray[i]) && charArray[i-1] != ' ')
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
                {
                    sb.Append(charArray[i]);

                    if (lastLetter != charArray[i])
                    {
                        lastLetter = charArray[i];
                        sameLetterCount = 1;
                    }
                    else
                        sameLetterCount++;
                }
           }

            //Same letter is repeated, only return the single letter
            if (sb.Length > 0 && sameLetterCount == sb.Length )
                list.Add(sb.ToString().Substring(0, 1).ToUpper());
            else
                list.Add(sb.ToString());

            if (list.Any(x => x == "" || x == " "))
                System.Diagnostics.Debug.Assert(list.Any(x => x == "" || x == " "));

            return list
                .Distinct()
                .ToArray();
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

        /// <summary>
        /// Stops the crawler execution
        /// </summary>
        public void Stop() 
        {
            System.Diagnostics.Debug.WriteLine("Stopping crawler");
            if (_cancellationToken != null)
                _cancellationToken.Cancel();
            
            _isRunning = false;
        }

        public void Dispose()
        {
            Stop();
        }       
    }
}
