using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Model
{
    // Интерфейс для работы с разными системами хранения данных
    public interface IDataManager<T>
    {
        public IEnumerable<T> GetCollection();
        public void AddItem(T Item);
        public void AddItems(IEnumerable<T> Items);
        public void DeleteItem(T Item); 
        public void EditItem( T Item);
        public void DeleteItems(IEnumerable<T> Items);
        public void SaveData(IEnumerable<T> collection = null);
    }
}
