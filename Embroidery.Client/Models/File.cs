using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Models
{
    [Index(nameof(FolderId), nameof(Name), IsUnique = true), Index(nameof(FileHash), IsUnique = false)]
    public class File
    {
        public File() { }

        public File(string thumbnail, string fullFilePath, int folderId)
        {
            using (var fileStream = new System.IO.FileStream(fullFilePath, FileMode.Open))
            {
                FileHash = GetChecksumBuffered(fileStream);
            }

            CreatedDate = DateTime.Now;
            ImageThumbnail = System.IO.File.ReadAllBytes(thumbnail);
            Name = System.IO.Path.GetFileNameWithoutExtension(fullFilePath);
            FullName = System.IO.Path.GetFileName(fullFilePath);
            //Remove the leading dot
            Extension = System.IO.Path.GetExtension(fullFilePath).ToLower();
            Extension = Extension.Substring(1, Extension.Length - 1);
            SizeInKb = (int)((decimal)(new System.IO.FileInfo(fullFilePath).Length) / 1024M);
            //Nullable doesn't work for some reason
            UpdatedDate = DateTime.MinValue;
            FolderId = folderId;

            ParseAndSetForLengthAndWidth(Name);
        }

        /// <summary>
        /// Look at the file name and try to parse out the size of the file
        /// </summary>
        /// <param name="fileName"></param>
        private void ParseAndSetForLengthAndWidth(string fileName)
        {
            var regEx = new System.Text.RegularExpressions.Regex("(([0-9]+)[xX]([0-9]+))");

            var match = regEx.Match(fileName);

            if (match.Success && match.Groups.Count == 4)
            {
                Length = byte.Parse(match.Groups[2].Value);
                Width = byte.Parse(match.Groups[3].Value);
            }
            else
            {
                Length = null;
                Width = null;
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

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// File name no extension
        /// </summary>
        [MaxLength(128), Required]
        public string Name { get; set; } = "";

        /// <summary>
        /// Extension only
        /// </summary>
        [MaxLength(4)]
        public string Extension { get; set; }

        /// <summary>
        /// Path + name + extension
        /// </summary>
        [MaxLength(388), Required]
        public string FullName { get; set; }

        public int SizeInKb { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public List<Tag>? Tags { get; set; }

        public byte[]? ImageThumbnail { get; set; }

        /// <summary>
        /// Hash of the contents of the file
        /// </summary>
        [MaxLength(64)]
        public string FileHash { get; set; }

        /// <summary>
        /// Files come in difference sizes
        /// </summary>
        public File LikeFile { get; set; }
        public int? LikeFileId { get; set; }

        public byte? Length { get; set; }
        public byte? Width { get; set; }
                
        public Folder Folder { get; set; }

        [Required]
        public int FolderId { get; set; }
    }
}
