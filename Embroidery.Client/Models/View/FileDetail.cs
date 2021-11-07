using Embroidery.Client.ViewModels;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Models.View
{
    public class FileDetail : ViewModelBase, INotifyPropertyChanged, IObservable<bool>, IDisposable
    {
        GroupedFile _groupedFile;
        public FileDetail(GroupedFile groupedFile)
        {
            _groupedFile = groupedFile;
            
            LoadTags();
        }

        private void LoadTags()
        {
            using (var db = new DataContext())
            {
                var simpleFiles = db.SimpleFiles
                    .FromSqlInterpolated(@$"SELECT 
                        Files.Id,
                     [FullName],
                     Path
                    FROM 
                     [Files]
                    INNER JOIN Folders ON Folders.Id = FolderId
                    WHERE
                     CleanName = {_groupedFile.CleanName}").ToArray();

                GroupedFile = _groupedFile;
                SimpleFiles = simpleFiles;

                Tags = new ObservableCollection<Tag>(db.FileTagRelationships
                        .Where(x => x.FileId == _groupedFile.FirstFileId)
                        .Select(x => x.Tag)
                        .OrderBy(x => x.Name).ToArray());
                    
            }
        }

        public GroupedFile GroupedFile { get; set; }

        public IEnumerable<SimpleFile> SimpleFiles { get; set; }
        public ObservableCollection<Tag> Tags { get; private set; }

        public ReactiveCommand<Tag, Unit> Add { get; }

        public void AddTag()
        {
            System.Diagnostics.Debug.WriteLine($"{nameof(AddTag)} to '{GroupedFile.CleanName}'");
        }

        public void RemoveTag(Tag tag)
        {
            System.Diagnostics.Debug.WriteLine($"{nameof(RemoveTag)} id='{tag.Id}'");

            using (var db = new DataContext())
            {
                var relationships = db.FileTagRelationships.Where(x => x.TagId == tag.Id && x.FileId == _groupedFile.FirstFileId);

                db.FileTagRelationships.RemoveRange(relationships);
                db.SaveChanges();
            }
            Tags.Remove(tag);
        }

        public void Dispose()
        {
            
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            return this;
        }

        private void AddEventHandler(Tag sender)
        {
            System.Diagnostics.Debug.WriteLine($"{sender}");
        }
    }
}
