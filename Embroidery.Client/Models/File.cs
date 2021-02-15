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
    [Index(nameof(FolderId), nameof(Name), IsUnique = true), 
        Index(nameof(FileHash), IsUnique = false), 
        Index(nameof(HasError), IsUnique = false),
        Index(nameof(CleanName), IsUnique = false)]
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
            if (System.IO.File.Exists(thumbnail))
            {
                ImageThumbnail = System.IO.File.ReadAllBytes(thumbnail);
                HasError = false;
            }
            else
                HasError = true;
            Name = System.IO.Path.GetFileNameWithoutExtension(fullFilePath);
            FullName = System.IO.Path.GetFileName(fullFilePath);
            //Remove the leading dot
            Extension = System.IO.Path.GetExtension(fullFilePath).ToLower();
            Extension = Extension.Substring(1, Extension.Length - 1);
            SizeInKb = (int)((decimal)(new System.IO.FileInfo(fullFilePath).Length) / 1024M);
            //Nullable doesn't work for some reason
            UpdatedDate = DateTime.MinValue;
            FolderId = folderId;

            var attributes = Utilities.FIleNameAttributeParser.ParseAndSetForLengthAndWidth(Name);
            if (attributes != null)
            {
                Length = attributes.Length;
                Width = attributes.Width;
                FontSize = attributes.FontSize;
                Letter = attributes.Letter;

                if (attributes.Position > 0)
                {
                    if (attributes.Position < 4)
                    {
                        //The information was found at the front of the name
                        CleanName = Name.Substring(attributes.Position, Name.Length - attributes.Position);
                    }
                    else // The information was found at the end
                        CleanName = Name.Substring(0, attributes.Position);
                }
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
        /// The name of the file after font information and or size has been removed.
        /// This can be used to find file that are similar
        /// </summary>
        [MaxLength(128)]
        public string? CleanName { get; set; }

        public byte? Length { get; set; }
        public byte? Width { get; set; }

        /// <summary>
        /// If it's a font the size of it
        /// </summary>
        public Single FontSize { get; set; }

        /// <summary>
        /// If it's a font what letter it is
        /// </summary>
        public char Letter{get;set;}
                
        public Folder Folder { get; set; }

        [Required]
        public int FolderId { get; set; }

        public bool HasError { get; set; }

        [MaxLength(512)]
        public string? ErrorMessage { get; set; }

        public string DisplayName {
            get {
                if (string.IsNullOrEmpty(CleanName))
                    return Name;
                else
                    return CleanName;
            }
        }

        public string DisplaySize { get {
                if (FontSize > 0)
                    return $"{FontSize}";
                else if (Length.HasValue && Width.HasValue)
                    return $"{Length.Value}x{Width.Value}";
                else
                    return string.Empty;
            } 
        }
    }
}
