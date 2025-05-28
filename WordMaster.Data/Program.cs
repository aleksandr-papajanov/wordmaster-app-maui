using Realms;
using Realms.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.Exceptions;
using WordMaster.Data.Infrastructure;
using WordMaster.Data.Infrastructure.Interfaces;
using WordMaster.Data.Models;

namespace WordMaster.Data
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IDataContext dataContext = new RealmDataContext();
            IRepository<Word> wordRepository = new RealmRepository<Word>(dataContext);

            wordRepository.Create(new Word
            {
                Id = Guid.NewGuid(),
                Text = "Hello"
            });
        }
    }
}
