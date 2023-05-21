using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Model
{
    //Модель для работы с BookEFContext на основе менеджера
    public class EFBookModel : IDataManager<Book>
    {
        BookEFContext _dbContext;
        public EFBookModel() {
            _dbContext = new BookEFContext();
        }
        IEnumerable<Book> IDataManager<Book>.GetCollection()
        {
            return _dbContext.Books;
        }

        public void AddItem(Book Item)
        {
            _dbContext.Books.Add(Item);
        }

        public void AddItems(IEnumerable<Book> Items)
        {
            _dbContext.Books.AddRange(Items);
        }

        public void DeleteItem(Book Item)
        {
            _dbContext.Books.Remove(Item);
        }

        public void EditItem(Book Item)
        {
           var b = _dbContext.Books.Find(Item.Id);
            b.Name = Item.Name;
            b.Author = Item.Author;
            b.YearOfIssue = Item.YearOfIssue;
            b.Theme = Item.Theme;
            _dbContext.Books.Update(b);
        }

        public void DeleteItems(IEnumerable<Book> Items)
        {
            _dbContext.Books.RemoveRange(Items);
        }

        public void SaveData(IEnumerable<Book> collection = null)
        {
            _dbContext.SaveChanges();
        }
    }
}
