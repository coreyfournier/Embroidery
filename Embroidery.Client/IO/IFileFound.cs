using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.IO
{
    public interface IFileFound
    {
        /// <summary>
        /// Called once a new file is found and added to the database
        /// </summary>
        /// <param name="file"></param>
        void NewFileInDatabase(Models.File file);

        /// <summary>
        /// Any process change
        /// </summary>
        /// <param name="message"></param>
        void StatusChange(string message);
    }
}
